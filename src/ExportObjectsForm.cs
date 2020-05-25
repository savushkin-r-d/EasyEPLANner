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
    public partial class ExportObjectsForm : Form
    {
        public ExportObjectsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Кнопка "Отмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportObjectsForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //TODO: Export objects
        }

        /// <summary>
        /// Очистить выделение в списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSelectedObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            checkedListBox.ClearSelected();
        }

        /// <summary>
        /// Выбрать все объекты в списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllObjects_LinkClicked(object sender, 
            LinkLabelLinkClickedEventArgs e)
        {
            for(int item = 0; item < checkedListBox.Items.Count; item++)
            {
                checkedListBox.SetItemChecked(item, true);
            }
        }
    }
}
