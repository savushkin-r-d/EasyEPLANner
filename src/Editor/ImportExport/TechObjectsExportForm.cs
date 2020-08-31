using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Editor
{
    public partial class TechObjectsExportForm : Form
    {
        public TechObjectsExportForm()
        {
            InitializeComponent();
            exportButton.Enabled = false;
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

        /// <summary>
        /// Кнопка "Экспортировать".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = $"Скрипт LUA (.lua)|*.lua";
            sfd.DefaultExt = "lua";

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

            this.Close();
        }

        /// <summary>
        /// Получить список номеров выбранных элементов в списке на форме.
        /// </summary>
        /// <returns></returns>
        private List<int> GetCheckedItemsNumbers()
        {
            var checkedItems = new List<int>();
            for (int item = 0; item < checkedListBox.Items.Count; item++)
            {
                bool itemChecked = checkedListBox.GetItemChecked(item);
                if (itemChecked)
                {
                    checkedItems.Add(item + 1);
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
            var names = TechObjectsExporter.GetInstance().ExportingObjectsNames;
            bool isEmpty = names.Length == 0;
            if (!isEmpty)
            {
                checkedListBox.Items.AddRange(names);
                exportButton.Enabled = true;
            }
        }
    }
}
