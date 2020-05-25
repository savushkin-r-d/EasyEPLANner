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

        /// <summary>
        /// Событие после закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportObjectsForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //TODO: Export objects

            // Save file dialog

            // Get checked items

            // SaveToLua for items

            // Save script to Lua File
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
        /// Выбрать все объекты в списке.
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

        /// <summary>
        /// Событие при загрузке формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportObjectsForm_Load(object sender, EventArgs e)
        {
            var objectsNames = TechObject.TechObjectManager.GetInstance().Items
                .Select(x => x.DisplayText[0])
                .ToArray();
            checkedListBox.Items.AddRange(objectsNames);
        }
    }
}
