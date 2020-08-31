using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NewEditor
{
    public partial class TechObjectsExportForm : Form
    {
        public TechObjectsExportForm()
        {
            InitializeComponent();
            exportButton.Enabled = false;
            InitTreeViewComponents();
        }

        ///<summary>
        ///Инициализация графических компонентов формы
        ///</summary>
        private void InitTreeViewComponents()
        {
            nodeColumn.Sortable = false;
            nodeColumn.Header = "Объекты";
            nodeColumn.Width = 300;

            nodeCheckBox.DataPropertyName = "CheckState";
            nodeCheckBox.VerticalAlign = VerticalAlignment.Center;
            nodeCheckBox.ParentColumn = nodeColumn;
            nodeCheckBox.EditEnabled = true;
            nodeCheckBox.CheckStateChanged +=
                exportingDevicesTree_ChangeCheckBoxState;

            nodeTextBox.DataPropertyName = "Text";
            nodeTextBox.VerticalAlign = VerticalAlignment.Center;
            nodeTextBox.ParentColumn = nodeColumn;
            nodeTextBox.DrawText += 
                new EventHandler<DrawTextEventArgs>(nodeTextBox_DrawText);

            exportingDevicesTree.Columns.Add(nodeColumn);
            exportingDevicesTree.NodeControls.Add(nodeCheckBox);
            exportingDevicesTree.NodeControls.Add(nodeTextBox);
        }

        ///<summary>
        ///Компоненты TreeView для инициализации
        ///</summary>
        TreeColumn nodeColumn = new TreeColumn();
        NodeCheckBox nodeCheckBox = new NodeCheckBox();
        NodeTextBox nodeTextBox = new NodeTextBox();

        /// <summary>
        /// Кнопка "Отмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Событие после закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportObjectsForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Кнопка "Экспортировать".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = saveFileDialogFilter;
            sfd.DefaultExt = luaExtension;

            try
            {
                var checkedItems = GetCheckedItemsNumbers();

                DialogResult saveResult = sfd.ShowDialog();
                if (saveResult == DialogResult.Cancel)
                {
                    return;
                }
                string fileName = sfd.FileName;

                TechObjectsExporter.GetInstance()
                    .Export(fileName, checkedItems);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Close();
        }

        /// <summary>
        /// Получить список номеров выбранных элементов в списке на форме.
        /// </summary>
        /// <returns></returns>
        private List<int> GetCheckedItemsNumbers()
        {
            var checkedItems = new List<int>();
            foreach(var treeNode in exportingDevicesTree.AllNodes)
            {
                var node = treeNode.Tag as Node;
                if(node != null && node.CheckState == CheckState.Checked &&
                    node.Tag is ITreeViewItem item)
                {
                    if(item != null && item.IsMainObject)
                    {
                        List<ITreeViewItem> objects = TechObjectsExporter
                            .GetInstance().Objects.ToList();
                        int objGlobalNum = objects.IndexOf(item) + 1;
                        if (objGlobalNum > 0) 
                        {
                            checkedItems.Add(objGlobalNum);
                        }
                    }
                }
            }

            bool isEmpty = checkedItems.Count == 0;
            if (isEmpty)
            {
                throw new Exception("Выберите хотя бы 1 объект для экспорта");
            }

            return checkedItems;
        }

        /// <summary>
        /// Очистить выделение в списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSelectedObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            foreach(TreeNodeAdv treeNode in exportingDevicesTree.AllNodes)
            {
                var node = treeNode.Tag as Node;
                if(node != null)
                {
                    node.CheckState = CheckState.Unchecked;
                }
            }
        }

        /// <summary>
        /// Выбрать все объекты в списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            foreach (TreeNodeAdv treeNode in exportingDevicesTree.AllNodes)
            {
                var node = treeNode.Tag as Node;
                if (node != null)
                {
                    node.CheckState = CheckState.Checked;
                }
            }
        }

        /// <summary>
        /// Событие при загрузке формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportObjectsForm_Load(object sender, EventArgs e)
        {
            exportingDevicesTree.BeginUpdate();
            exportingDevicesTree.Model = null;
            exportingDevicesTree.Refresh();
            var treeModel = new TreeModel();
            var root = new Node(TechObjectsExporter.GetInstance().ProjectName);
            treeModel.Nodes.Add(root);

            ITreeViewItem[] objects = TechObjectsExporter.GetInstance()
                .RootItems;
            LoadObjectsForExport(objects, root);

            exportingDevicesTree.Model = treeModel;
            exportingDevicesTree.EndUpdate();
        }

        /// <summary>
        /// Рекурсивная загрузка объектов для экспорта
        /// </summary>
        /// <param name="items">Объект родитель</param>
        /// <param name="parent">Узел родитель</param>
        private void LoadObjectsForExport(ITreeViewItem[] items, Node parent)
        {
            if(items == null)
            {
                return;
            }

            foreach(var item in items)
            {
                var newNode = new Node(item.DisplayText[0]);
                newNode.Tag = item;
                parent.Nodes.Add(newNode);

                if(!item.IsMainObject)
                {
                    LoadObjectsForExport(item.Items, newNode);
                }
            }
            exportButton.Enabled = true;
        }

        /// <summary>
        /// Функция обновления состояний чекбоксов
        /// </summary>
        /// <param name="sender">Объект, который вызвал функцию</param>
        /// <param name="e">Контекст переданный вызывающим кодом</param>
        private void exportingDevicesTree_ChangeCheckBoxState(object sender,
            TreePathEventArgs e)
        {
            object nodeObject = e.Path.LastNode;
            Node checkedNode = nodeObject as Node;

            RecursiveCheckParent(checkedNode.Parent);

            RecursiveCheck(checkedNode);
        }

        /// <summary>
        /// Функция установки состояния
        /// отображения узла
        /// </summary>
        /// <param name="node">Выбранный узел</param>
        private void RecursiveCheck(Node node)
        {
            if (node.Nodes.Count > 0)
            {
                List<Node> childNodes = node.Nodes.ToList();

                foreach (Node child in childNodes)
                {
                    if (child.IsHidden != true)
                    {
                        child.CheckState = node.CheckState;
                        RecursiveCheck(child);
                    }
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Установка состояния отображения
        /// для родительского узла выбранного элемента
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        private void RecursiveCheckParent(Node parentNode)
        {
            // 0 - корень (но не Root)
            if (parentNode.Index > -1)
            {
                int countOfCheckedNodes = 0;
                int countOfIndeterminateNodes = 0;
                int countOfNodes = parentNode.Nodes.Count;
                foreach (Node node in parentNode.Nodes)
                {
                    if (node.CheckState == CheckState.Checked)
                    {
                        countOfCheckedNodes++;
                    }

                    if (node.CheckState == CheckState.Indeterminate)
                    {
                        countOfIndeterminateNodes++;
                    }

                    if (node.IsHidden == true)
                    {
                        countOfNodes--;
                    }
                }

                if (parentNode.CheckState != CheckState.Indeterminate)
                {
                    parentNode.CheckState = CheckState.Indeterminate;
                }

                if (countOfCheckedNodes == countOfNodes)
                {
                    parentNode.CheckState = CheckState.Checked;
                }

                if (countOfCheckedNodes == 0 && countOfIndeterminateNodes == 0)
                {
                    parentNode.CheckState = CheckState.Unchecked;
                }

                RecursiveCheckParent(parentNode.Parent);
            }
        }

        private void nodeTextBox_DrawText(object sender, DrawTextEventArgs e)
        {
            e.TextColor = Color.Black;
        }

        private const string luaExtension = "lua";
        private const string saveFileDialogFilter = "Скрипт LUA (.lua)|*.lua";
    }
}
