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
            string fileExtension = ".lua";
            var ofd = new OpenFileDialog();
            ofd.DefaultExt = fileExtension;
            
            DialogResult dialog = ofd.ShowDialog();
            if (dialog == DialogResult.Cancel)
            {
                return;
            }

            // Load objects

            // Display objects in ListBox

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
            //TODO: Import selected objects to the end of tree
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
