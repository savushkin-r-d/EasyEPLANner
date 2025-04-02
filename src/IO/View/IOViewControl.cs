using BrightIdeasSoftware;
using EasyEPlanner;
using Editor;
using Eplan.EplApi.Scripting;
using IO.ViewModel;
using PInvoke;
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
        }

        private IOViewControl(IIOViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
            InitStructPLC();
            InitDataStructPLC();
        }

        private void InitStructPLC()
        {
            StructPLC.CanExpandGetter = obj => obj is IExpandable exp && (exp.Items?.Any() ?? false);
            StructPLC.ChildrenGetter = obj => (obj as IExpandable)?.Items;

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

            StructPLC.Roots = DataContext.Roots;

            StructPLC.Columns[0].Width = 100;
            StructPLC.Columns[1].Width = 200;

            StructPLC.Expand(DataContext.Root);
            StructPLC.SelectedIndex = 0;
            StructPLC.SelectedItem.EnsureVisible();

            StructPLC.EndUpdate();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

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
                if (level > 0 && StructPLC.IsExpanded(item) is false)
                {
                    StructPLC.Expand(item);
                }
                else if (level == 0 && StructPLC.IsExpanded(item) == true)
                {
                    StructPLC.Collapse(item);
                }

                if (item != null)
                {
                    ExpandToLevel(level - 1, item.Items);
                }
            }
        }

        private TextBox textBoxCellEditor;

        private void CellEditStarting(object sender, CellEditEventArgs e)
        {
            var item = StructPLC.SelectedObject as IEditable;

            if (item is null ||
                e.Column.Index != 1)
            {
                e.Cancel = true;
                return;
            }

            InitTextBoxCellEditor(item.Description, e.CellBounds);

            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();

            StructPLC.Freeze();
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

            //textBoxCellEditor.LostFocus += editorTView_LostFocus;
            //textBoxCellEditor.KeyDown += CellEditor_KeyDown;

            StructPLC.Controls.Add(textBoxCellEditor);
        }

        private void CellEditFinishing(object sender, CellEditEventArgs e)
        {
            var item = StructPLC.SelectedObject as IEditable;

            if (item is null)
            {
                e.Cancel = true;
                StructPLC.Unfreeze();


                return;
            }
            
            StructPLC.Controls.Remove(textBoxCellEditor);
            var modified = item.SetValue(e.NewValue.ToString());

            if (modified)
            {
                RefreshTree();
            }

            e.Cancel = true;
            StructPLC.Unfreeze();
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
                StructPLC.RefreshObjects(DataContext.Roots.ToList());
            }
        }

        public void RefreshTreeAfterBinding()
        {
            RefreshTree();
        }

        private void ItemExpanded(object sender, TreeBranchExpandedEventArgs e)
        {
            AutoResizeColumns(sender as TreeListView);
        }

        private void ItemCollapsed(object sender, TreeBranchCollapsedEventArgs e)
        {
            AutoResizeColumns(sender as TreeListView);
        }


        public void AutoResizeColumns(TreeListView treeListView)
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

            DataContext.RebuildTree();
            InitDataStructPLC();
        }   
    }
}
