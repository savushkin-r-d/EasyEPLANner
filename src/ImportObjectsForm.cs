using Eplan.EplApi.HEServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public partial class ImportObjectsForm : Form
    {
        public ImportObjectsForm()
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

            TechObjectsImporter.GetInstance()
                .LoadImportingObjects(ofd.FileName);

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

            TechObjectsImporter.GetInstance().Import(checkedItems);
            Editor.Editor.GetInstance().EForm.RefreshTree();
            this.Close();
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
