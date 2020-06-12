using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Device;

namespace EasyEPlanner
{
    public partial class InterprojectExchangeForm : Form
    {
        public InterprojectExchangeForm()
        {
            InitializeComponent();
        }

        private void InterprojectExchangeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (filterForm != null && filterForm.IsDisposed == false)
            {
                filterForm.Close();
            }
            this.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InterprojectExchangeForm_Load(object sender, EventArgs e)
        {
            //TODO: Написать компараторы для сортировки корректной.

            foreach(var dev in DeviceManager.GetInstance().Devices)
            {
                var devDescr = new string[] { dev.Description, dev.Name };
                var item = new ListViewItem(devDescr);
                currentProjSignalsList.Items.Add(item);
            }

            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 1", "Примечание 1" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 2", "Примечание 2" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 3", "Примечание 3" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 4", "Примечание 4" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 5", "Примечание 5" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 6", "Примечание 6" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 7", "Примечание 7" }));
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            if (filterForm == null || filterForm.IsDisposed)
            {
                filterForm = new FilterForm();
            }

            filterForm.ShowDialog();
        }

        private FilterForm filterForm;
    }
}
