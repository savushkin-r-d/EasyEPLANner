using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public partial class TechObjectsImportForm : Form
    {
        public TechObjectsImportForm()
        {
            InitializeComponent();

            importButton.Enabled = false;
        }

        /// <summary>
        /// Кнопка "Обзор".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overviewButton_Click(object sender, EventArgs e)
        {
            string fileExtension = "lua";

            var ofd = new OpenFileDialog();
            ofd.DefaultExt = fileExtension;
            ofd.Filter = $"Скрипт LUA (.lua)|*.lua";

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
                MessageBox.Show(exception.Message);
                importButton.Enabled = false;
                checkedListBox.Items.Clear();
                return;
            }
            
            checkedListBox.Items.Clear();
            checkedListBox.Items.AddRange(TechObjectsImporter.GetInstance()
                .ImportedObjectsListArray);

            importButton.Enabled = true;
        }

        /// <summary>
        /// Кнопка "Отмена".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
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
                MessageBox.Show(exception.Message);
                Editor.Editor.GetInstance().EForm.RefreshTree();
                return;
            }

            Editor.Editor.GetInstance().EForm.RefreshTree();
            this.Close();
        }

        /// <summary>
        /// Получить номера выбранных для импорта элементов.
        /// </summary>
        /// <returns></returns>
        private List<int> GetCheckedForImportItems()
        {
            var checkedItems = new List<int>();
            for (int item = 0; item < checkedListBox.Items.Count; item++)
            {
                bool itemChecked = checkedListBox.GetItemChecked(item);
                if (itemChecked)
                {
                    var checkedItem = checkedListBox.Items[item] as string;
                    string itemNumber = checkedItem.Split('.')[0];
                    int itemNum = Convert.ToInt32(itemNumber);
                    checkedItems.Add(itemNum);
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
            this.Dispose();
        }

        /// <summary>
        /// Очистить выделение в списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSelectedObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            for (int item = 0; item < checkedListBox.Items.Count; item++)
            {
                checkedListBox.SetItemChecked(item, false);
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
            for(int item = 0; item < checkedListBox.Items.Count; item++)
            {
                checkedListBox.SetItemChecked(item, true);
            }
        }
    }
}
