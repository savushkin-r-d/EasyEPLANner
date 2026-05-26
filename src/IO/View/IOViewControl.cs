using BrightIdeasSoftware;
using EasyEPlanner;
using Editor;
using Eplan.EplApi.Scripting;
using IO.ViewModel;
using PInvoke;
using StaticHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO.View
{
    public partial class IOViewControl : Form
    {
        private bool cancelChanges = false;

        private bool isCellEditing = false;

        private ToolStripMenuItem shiftModulesToolStripMenuItem;

        public static IOViewControl Instance { get; private set; }

        /// <summary>
        /// Модель представления
        /// </summary>
        public static IIOViewModel DataContext { get; private set; }

        /// <summary>
        /// Запуск окна из меню
        /// </summary>
        public static void Start()
        {
            DataContext ??= new IOViewModel(IOManager.GetInstance());
            Instance ??= new IOViewControl(DataContext);

            Instance.InitDataStructPLC();
            Instance.ShowDlg();
        }

        public void Clear()
        {
            DataContext = new IOViewModel(null);
            InitDataStructPLC();
        }

        private IOViewControl(IIOViewModel viewModel)
        {
            InitializeComponent();
            InitStructPLC();
            InitContextMenu();
        }

        private void InitStructPLC()
        {
            StructPLC.CanExpandGetter = obj => obj is IExpandable exp && (exp.Items?.Any() ?? false);
            StructPLC.ChildrenGetter = obj => (obj as IExpandable)?.Items;
            StructPLC.CellToolTipGetter = (col, obj) => (col.Index) switch
            {
                0 => (obj as IToolTip)?.Name ?? (obj as IViewItem).Name,
                1 => (obj as IToolTip)?.Description ?? (obj as IViewItem).Description,
                _ => ""
            };


            var firstColumn = new OLVColumn("Название", nameof(IViewItem.Name))
            {
                ImageGetter = obj => (obj is IHasIcon item)? (int)item.Icon : -1,
                AspectGetter = obj => (obj as IViewItem).Name,
                IsEditable = true,
                Sortable = false,
            };

            var secondColumn = new OLVColumn("Описание", nameof(IViewItem.Description))
            {
                ImageGetter = obj => (obj is IHasDescriptionIcon item) ? (int)item.Icon : -1,
                AspectGetter = obj => (obj as IViewItem).Description,
                IsEditable = true,
                Sortable = false,
            };


            StructPLC.Columns.Add(firstColumn);
            StructPLC.Columns.Add(secondColumn);
        }

        private void InitContextMenu()
        {
            var contextMenuStrip = new ContextMenuStrip(components);

            shiftModulesToolStripMenuItem = new ToolStripMenuItem("Сдвинуть модули");
            shiftModulesToolStripMenuItem.Click += ShiftModules_Click;

            contextMenuStrip.Items.Add(shiftModulesToolStripMenuItem);
            contextMenuStrip.Opening += StructPLCContextMenu_Opening;

            StructPLC.ContextMenuStrip = contextMenuStrip;
            StructPLC.MouseDown += StructPLC_MouseDown;
        }

        private void StructPLC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var item = StructPLC.GetItemAt(e.X, e.Y) as OLVListItem;
            StructPLC.SelectedObject = item?.RowObject;
        }

        private void StructPLCContextMenu_Opening(object sender, CancelEventArgs e)
        {
            shiftModulesToolStripMenuItem.Enabled =
                StructPLC.SelectedObject is IModule module &&
                module.IOModule.Function?.IsValid == true;
        }

        private void ShiftModules_Click(object sender, EventArgs e)
        {
            if (StructPLC.SelectedObject is not IModule module ||
                !TryGetShiftValue(out int shiftValue))
            {
                return;
            }

            try
            {
                ShiftModulesFrom(module, shiftValue);

                EProjectManager.GetInstance().SyncAndSave(false);

                Editor.Editor.GetInstance().EditorForm.RefreshTree();
                DFrm.GetInstance().RefreshTree();

                RebuildTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сдвиг модулей",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool TryGetShiftValue(out int shiftValue)
        {
            const int DefaultShiftValue = 1;
            const int MaxShiftValue = 99;

            using var form = new Form
            {
                Text = "Сдвинуть модули",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(260, 100)
            };

            var label = new Label
            {
                Text = "Сдвинуть на:",
                AutoSize = true,
                Location = new Point(12, 15)
            };

            var countInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = MaxShiftValue,
                Value = DefaultShiftValue,
                Location = new Point(145, 12),
                Width = 95
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(84, 60),
                Width = 75
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(165, 60),
                Width = 75
            };

            form.Controls.AddRange(new Control[]
            {
                label,
                countInput,
                okButton,
                cancelButton
            });

            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            bool accepted = form.ShowDialog() == DialogResult.OK;
            shiftValue = accepted ? (int)countInput.Value : 0;

            return accepted;
        }

        private static void ShiftModulesFrom(IModule selectedModule,
            int shiftValue)
        {
            var modules = selectedModule.IONode.IOModules;
            int selectedIndex = modules.IndexOf(selectedModule.IOModule);
            if (selectedIndex < 0)
            {
                throw new InvalidOperationException(
                    "Выбранный модуль не найден в узле.");
            }

            var modulesToShift = modules
                .Skip(selectedIndex)
                .Where(module => module?.Function?.IsValid == true)
                .OrderByDescending(module => module.PhysicalNumber)
                .ToList();

            if (!modulesToShift.Any())
            {
                throw new InvalidOperationException(
                    "Нет модулей для сдвига.");
            }

            foreach (var module in modulesToShift)
            {
                int newPhysicalNumber = module.PhysicalNumber + shiftValue;
                ValidateModuleNumber(module.PhysicalNumber, newPhysicalNumber);
            }

            foreach (var module in modulesToShift)
            {
                int newPhysicalNumber = module.PhysicalNumber + shiftValue;
                RenameModuleWithClamps(module,
                    $"-A{module.PhysicalNumber}",
                    $"-A{newPhysicalNumber}");
            }
        }

        private static void RenameModuleWithClamps(IIOModule module,
            string oldName, string newName)
        {
            module.Function.Rename(newName);

            foreach (var clampFunction in module.ClampFunctions.Values
                .Where(function => function?.IsValid == true)
                .Distinct())
            {
                RenameClampFunction(clampFunction, oldName, newName);
            }
        }

        private static void RenameClampFunction(
            StaticHelper.IEplanFunction clampFunction, string oldModuleName,
            string newModuleName)
        {
            var newClampName = Regex.Replace(clampFunction.Name,
                $@"{Regex.Escape(oldModuleName)}(?=$|\D)", newModuleName);
            if (newClampName != clampFunction.Name)
            {
                clampFunction.Rename(newClampName);
            }
        }

        private static void ValidateModuleNumber(int currentPhysicalNumber,
            int newPhysicalNumber)
        {
            bool movedToAnotherNode =
                currentPhysicalNumber / 100 != newPhysicalNumber / 100;
            bool becameNodeNumber = newPhysicalNumber % 100 == 0;
            if (movedToAnotherNode || becameNodeNumber)
            {
                throw new InvalidOperationException(
                    $"Невозможно переименовать модуль -A{currentPhysicalNumber} " +
                    $"в -A{newPhysicalNumber}: номер выходит за пределы узла.");
            }
        }

        private void InitDataStructPLC()
        {
            StructPLC.BeginUpdate();

            StructPLC.Roots = DataContext.Roots;

            StructPLC.Columns[0].Width = 100;
            StructPLC.Columns[1].Width = 200;

            StructPLC.Expand(DataContext.Root);

            /// Востановление развертки дерева
            RestoreExpanded(DataContext.Roots);

            StructPLC.SelectedIndex = 0;
            StructPLC.SelectedItem.EnsureVisible();

            StructPLC.EndUpdate();

            AutoResizeColumns(StructPLC);
        }

        private void RestoreExpanded(IEnumerable items)
        {
            foreach (var item in items.OfType<IExpandable>())
            {
                if (item.Expanded)
                {
                    StructPLC.Expand(item);
                }

                RestoreExpanded(item.Items);
            }
        }

        private void Expand_Click(object sender, EventArgs e)
        {
            int level = Convert.ToInt32((sender as ToolStripMenuItem).Tag);

            StructPLC.BeginUpdate();

            // Убираем события для оптимизации множественного вызова
            StructPLC.Expanded -= ItemExpanded;
            StructPLC.Collapsed -= ItemCollapsed;

            StructPLC.SelectedIndex = 0;

            ExpandToLevel(level, StructPLC.Objects);

            StructPLC.Expanded += ItemExpanded;
            StructPLC.Collapsed += ItemCollapsed;
            
            StructPLC.EnsureModelVisible(StructPLC.SelectedObject);
            StructPLC.EndUpdate();

            AutoResizeColumns(StructPLC);
        }

        private void ExpandToLevel(int level, IEnumerable items)
        {
            foreach (var item in items.OfType<IExpandable>())
            {
                if (level > 0 && !StructPLC.IsExpanded(item))
                {
                    StructPLC.Expand(item);
                    item?.Expanded = true;
                }
                
                if (level == 0 && StructPLC.IsExpanded(item))
                {
                    StructPLC.Collapse(item);
                    item?.Expanded = false;
                }

                if (item is not null)
                {
                    ExpandToLevel(level - 1, item.Items);
                }
            }
        }

        private TextBox textBoxCellEditor;

        private void CellEditStarting(object sender, CellEditEventArgs e)
        {
            if (StructPLC.SelectedObject is not IEditable item || e.Column.Index != 1)
            {
                e.Cancel = true;
                return;
            }

            isCellEditing = true;

            InitTextBoxCellEditor(item.Value, e.CellBounds);

            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();

            StructPLC.Freeze();
        }

        private void CellEditFinishing(object sender, CellEditEventArgs e)
        {
            isCellEditing = false;

            if (cancelChanges || StructPLC.SelectedObject is not IEditable item)
            {
                e.Cancel = true;
                cancelChanges = false;

                StructPLC.Unfreeze();

                return;
            }

            StructPLC.Controls.Remove(textBoxCellEditor);

            var modified = item.SetValue(e.NewValue.ToString());

            if (modified)
            {
                RefreshTree();
                DFrm.GetInstance().RefreshTreeAfterBinding();
            }

            e.Cancel = true;
            StructPLC.Unfreeze();
        }

        private void InitTextBoxCellEditor(string text, Rectangle bounds)
        {
            textBoxCellEditor = new TextBox
            {
                Enabled = true,
                Visible = true,
                Text = text,
                Bounds = bounds,
            };


            textBoxCellEditor.LostFocus += CellEditor_LostFocus;
            textBoxCellEditor.KeyDown += CellEditor_KeyDown;
            
            StructPLC.Controls.Add(textBoxCellEditor);
        }


        private void CellEditor_LostFocus(object sender, EventArgs e)
        {
            if (isCellEditing)
                StructPLC.FinishCellEdit();
        }

       
        private void CellEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    StructPLC.FinishCellEdit();
                    break;

                case Keys.Escape:
                    cancelChanges = true;
                    StructPLC.FinishCellEdit();
                    break;

                default:
                    return; // exit without e.Handled
            }

            e.Handled = true;
        }


        public void RefreshTree()
        {
            if (StructPLC.Items.Count == 0)
            {
                StructPLC.BeginUpdate();
                StructPLC.ClearObjects();
                StructPLC.EndUpdate();
            }
            else
            {
                StructPLC.FocusedObject = StructPLC.SelectedObject;
                StructPLC.RefreshObjects(DataContext.Roots.ToList());
            }
        }

        public void RefreshTreeAfterBinding()
        {
            RefreshTree();
        }

        public void RebuildTree()
        {
            DataContext.RebuildTree();
            InitDataStructPLC();
        }

        private void ItemExpanded(object sender, TreeBranchExpandedEventArgs e)
        {
            (e.Model as IExpandable).Expanded = true;
            AutoResizeColumns(sender as TreeListView);
        }


        private void ItemCollapsed(object sender, TreeBranchCollapsedEventArgs e)
        {
            (e.Model as IExpandable).Expanded = false;
            AutoResizeColumns(sender as TreeListView);
        }


        private static void AutoResizeColumns(TreeListView treeListView)
        {
            if (treeListView is null)
                return;

            foreach (ColumnHeader column in treeListView.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if ((sender as TreeListView)?.SelectedObject is IClamp clamp)
            {
                DataContext.SelectedClampFunction = clamp.ClampFunction;
                DataContext.SelectedClamp = clamp;
            }
            else
            {
                DataContext.SelectedClampFunction = null;
                DataContext.SelectedClamp = null;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EProjectManager.GetInstance().SyncAndSave(false);

            
            Editor.Editor.GetInstance().EditorForm.RefreshTree();
            DFrm.GetInstance().RefreshTree();

            RebuildTree();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var tlv = sender as TreeListView;
           
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    (tlv.SelectedObject as IDeletable)?.Delete();
                    break;

                default:
                    return;
            }

            e.Handled = true;
            RefreshTree();
            DFrm.GetInstance().RefreshTreeAfterBinding();
        }

        private void StructPLC_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Model is IClamp clamp && !clamp.Bound)
            {
                e.Item.SubItems[1].ForeColor = Color.LightSlateGray;
            }
        }
    }
}
