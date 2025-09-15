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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO.View
{
    public partial class IOViewControl : Form
    {
        private bool cancelChanges = false;

        private bool isCellEditing = false;

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
            Instance ??= new IOViewControl(new IOViewModel(IOManager.GetInstance()));
            Instance.ShowDlg();

            DataContext = new IOViewModel(IOManager.GetInstance());
            Instance.InitDataStructPLC();
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

        private void InitDataStructPLC()
        {
            StructPLC.BeginUpdate();
            StructPLC.Freeze();

            StructPLC.Roots = DataContext.Roots;

            StructPLC.Columns[0].Width = 100;
            StructPLC.Columns[1].Width = 200;

            StructPLC.Expand(DataContext.Root);
            RestoreExpanded(DataContext.Roots);

            StructPLC.SelectedIndex = 0;
            StructPLC.SelectedItem.EnsureVisible();

            StructPLC.Unfreeze();
            StructPLC.EndUpdate();
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
            StructPLC.CollapseAll();

            ExpandToLevel(level, StructPLC.Objects);
            AutoResizeColumns(StructPLC);

            StructPLC.Expanded += ItemExpanded;
            StructPLC.Collapsed += ItemCollapsed;
            
            StructPLC.EnsureModelVisible(StructPLC.SelectedObject);
            StructPLC.EndUpdate();
        }

        private void ExpandToLevel(int level, IEnumerable items)
        {
            foreach (var item in items.OfType<IExpandable>())
            {
                if (level > 0 && !StructPLC.IsExpanded(item))
                {
                    StructPLC.Expand(item);
                }
                else if (level == 0 && StructPLC.IsExpanded(item))
                {
                    StructPLC.Collapse(item);
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
