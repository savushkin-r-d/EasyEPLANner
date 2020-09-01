using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NewEditor
{
    public partial class TechObjectsImportForm : Form
    {
        public TechObjectsImportForm()
        {
            InitializeComponent();
            importButton.Enabled = false;
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
                importingObjectsTree_ChangeCheckBoxState;

            nodeTextBox.DataPropertyName = "Text";
            nodeTextBox.VerticalAlign = VerticalAlignment.Center;
            nodeTextBox.ParentColumn = nodeColumn;
            nodeTextBox.DrawText +=
                new EventHandler<DrawTextEventArgs>(nodeTextBox_DrawText);

            importingObjectsTree.Columns.Add(nodeColumn);
            importingObjectsTree.NodeControls.Add(nodeCheckBox);
            importingObjectsTree.NodeControls.Add(nodeTextBox);
        }

        ///<summary>
        ///Компоненты TreeView для инициализации
        ///</summary>
        TreeColumn nodeColumn = new TreeColumn();
        NodeCheckBox nodeCheckBox = new NodeCheckBox();
        NodeTextBox nodeTextBox = new NodeTextBox();

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

            var importedObjectsNames = TechObjectsImporter.GetInstance().
                ImportedObjectsNamesArray;
            bool objectsIsEmpty = importedObjectsNames.Length == 0;
            if (!objectsIsEmpty)
            {
                //checkedListBox.Items.AddRange(TechObjectsImporter.GetInstance()
                //   .ImportedObjectsNamesArray);
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
            var checkedItems = GetCheckedForImportItems();

            try
            {
                TechObjectsImporter.GetInstance().Import(checkedItems);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Editor.Editor.GetInstance().EForm.RefreshTree();
                return;
            }

            NewEditor.GetInstance().EditorForm.RefreshTree();
            Close();
        }

        /// <summary>
        /// Получить номера выбранных для импорта элементов.
        /// </summary>
        /// <returns></returns>
        private List<int> GetCheckedForImportItems()
        {
            var checkedItems = new List<int>();
            //for (int item = 0; item < checkedListBox.Items.Count; item++)
            //{
            //    bool itemChecked = checkedListBox.GetItemChecked(item);
            //    if (itemChecked)
            //    {
            //        var checkedItem = checkedListBox.Items[item] as string;
            //        string itemNumber = checkedItem.Split('.')[0];
            //        int itemNum = Convert.ToInt32(itemNumber);
            //        checkedItems.Add(itemNum);
            //    }
            //}

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
    }
}
