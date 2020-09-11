using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Editor
{
    public partial class TechObjectsExportForm : Form
    {
        public TechObjectsExportForm()
        {
            InitializeComponent();
            exportButton.Enabled = false;

            const string columnName = "Объекты";
            StaticHelper.GUIHelper.SetUpAdvTreeView(exportingObjectsTree,
                columnName, nodeTextBox_DrawText, nodeCheckBox,
                exportingObjectsTree_ChangeCheckBoxState);
        }

        ///<summary>
        /// Чекбокс для дерева
        ///</summary>
        NodeCheckBox nodeCheckBox = new NodeCheckBox();

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
            foreach(var treeNode in exportingObjectsTree.AllNodes)
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
            foreach(TreeNodeAdv treeNode in exportingObjectsTree.AllNodes)
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
            foreach (TreeNodeAdv treeNode in exportingObjectsTree.AllNodes)
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
            exportingObjectsTree.BeginUpdate();
            exportingObjectsTree.Model = null;
            exportingObjectsTree.Refresh();
            var treeModel = new TreeModel();
            var root = new Node(TechObjectsExporter.GetInstance().ProjectName);
            treeModel.Nodes.Add(root);

            ITreeViewItem[] objects = TechObjectsExporter.GetInstance()
                .RootItems;
            LoadObjectsForExport(objects, root);

            exportingObjectsTree.Model = treeModel;
            exportingObjectsTree.EndUpdate();
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
        private void exportingObjectsTree_ChangeCheckBoxState(object sender,
            TreePathEventArgs e)
        {
            object nodeObject = e.Path.LastNode;
            Node checkedNode = nodeObject as Node;
            StaticHelper.GUIHelper.CheckCheckState(checkedNode);
        }

        private void nodeTextBox_DrawText(object sender, DrawTextEventArgs e)
        {
            e.TextColor = Color.Black;
        }

        private const string luaExtension = "lua";
        private const string saveFileDialogFilter = "Скрипт LUA (.lua)|*.lua";
    }
}
