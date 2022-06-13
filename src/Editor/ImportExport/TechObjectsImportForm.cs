using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Editor
{
    public partial class TechObjectsImportForm : Form
    {
        public TechObjectsImportForm()
        {
            InitializeComponent();
            importButton.Enabled = false;

            const string columnName = "Объекты";
            StaticHelper.GUIHelper.SetUpAdvTreeView(importingObjectsTree,
                columnName, nodeTextBox_DrawText, nodeCheckBox,
                importingObjectsTree_ChangeCheckBoxState);
        }

        ///<summary>
        /// Чекбокс для дерева
        ///</summary>
        NodeCheckBox nodeCheckBox = new NodeCheckBox();

        /// <summary>
        /// Кнопка "Обзор".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overviewButton_Click(object sender, EventArgs e)
        {
            const string fileExtension = "lua";
            const string fileFilter = "Скрипт LUA (.lua)|*.lua";

            var ofd = new OpenFileDialog();
            ofd.DefaultExt = fileExtension;
            ofd.Filter = fileFilter;

            DialogResult dialog = ofd.ShowDialog();
            if (dialog == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                TechObjectsImporter.GetInstance()
                    .LoadImportingObjects(ofd.FileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Предупреждение", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                importButton.Enabled = false;
                importingObjectsTree.Model = null;
                return;
            }

            FillCheckedListBox();
        }

        /// <summary>
        /// Заполнение списка именами объектов
        /// </summary>
        private void FillCheckedListBox()
        {
            importingObjectsTree.Model = null;
            if(TechObjectsImporter.GetInstance().RootItems.Length != 0)
            {
                importingObjectsTree.BeginUpdate();
                importingObjectsTree.Model = null;
                importingObjectsTree.Refresh();
                var treeModel = new TreeModel();

                ITreeViewItem[] objects = TechObjectsImporter.GetInstance()
                    .RootItems;
                foreach (var s88Obj in objects)
                {
                    var root = new Node(s88Obj.DisplayText[0]);
                    root.Tag = s88Obj;

                    LoadObjectsForImport(s88Obj.Items, root);
                    treeModel.Nodes.Add(root);
                }

                importingObjectsTree.Model = treeModel;
                importingObjectsTree.EndUpdate();
                importButton.Enabled = true;
            }
            else
            {
                MessageBox.Show("Объекты для импорта не найдены",
                    "Предупреждение", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                importButton.Enabled = false;
            }
        }

        /// <summary>
        /// Рекурсивная загрузка объектов для экспорта
        /// </summary>
        /// <param name="items">Объект родитель</param>
        /// <param name="parent">Узел родитель</param>
        private void LoadObjectsForImport(ITreeViewItem[] items, Node parent)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                var newNode = new Node(item.DisplayText[0]);
                newNode.Tag = item;
                parent.Nodes.Add(newNode);

                if (!item.IsMainObject)
                {
                    LoadObjectsForImport(item.Items, newNode);
                }
            }
        }

        /// <summary>
        /// Кнопка "Отмена".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Кнопка "Импортировать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importButton_Click(object sender, EventArgs e)
        {
            List<ITreeViewItem> checkedItems = GetCheckedForImportItems();
            Thread.CurrentThread.CurrentCulture = StaticHelper.CommonConst
                .CultureWithDotInsteadComma;

            try
            {
                TechObjectsImporter.GetInstance().Import(checkedItems);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Editor.GetInstance().EditorForm.RefreshTree();
                return;
            }

            Editor.GetInstance().EditorForm.RefreshTree();
            Close();
        }

        /// <summary>
        /// Получить номера выбранных для импорта элементов.
        /// </summary>
        /// <returns></returns>
        private List<ITreeViewItem> GetCheckedForImportItems()
        {
            var checkedItems = new List<ITreeViewItem>();
            foreach (var treeNode in importingObjectsTree.AllNodes)
            {
                var node = treeNode.Tag as Node;
                if (node != null && node.CheckState == CheckState.Checked &&
                    node.Tag is ITreeViewItem item)
                {
                    if (item != null && item.IsMainObject)
                    {
                        checkedItems.Add(item);
                    }
                }
            }

            return checkedItems;
        }

        /// <summary>
        /// Событие после закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportObjectsForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Очистить выделение в списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSelectedObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            foreach (TreeNodeAdv treeNode in importingObjectsTree.AllNodes)
            {
                var node = treeNode.Tag as Node;
                if (node != null)
                {
                    node.CheckState = CheckState.Unchecked;
                }
            }
        }

        /// <summary>
        /// Выделить все объекты в списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectedAllObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            foreach (TreeNodeAdv treeNode in importingObjectsTree.AllNodes)
            {
                var node = treeNode.Tag as Node;
                if (node != null)
                {
                    node.CheckState = CheckState.Checked;
                }
            }
        }

        /// <summary>
        /// Функция обновления состояний чекбоксов
        /// </summary>
        /// <param name="sender">Объект, который вызвал функцию</param>
        /// <param name="e">Контекст переданный вызывающим кодом</param>
        private void importingObjectsTree_ChangeCheckBoxState(object sender,
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
    }
}
