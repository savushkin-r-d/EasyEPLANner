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
    public partial class InterprojectExchangeForm : Form
    {
        public InterprojectExchangeForm()
        {
            InitializeComponent();
        }

        private void InterprojectExchangeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InterprojectExchangeForm_Load(object sender, EventArgs e)
        {
            //init mock
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 1", "Сигнал 1" }));
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 2", "Сигнал 2" }));
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 3", "Сигнал 3" }));
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 4", "Сигнал 4" }));
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 5", "Сигнал 5" }));
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 6", "Сигнал 6" }));
            currentProjSignalsList.Items.Add(new ListViewItem(new string[] { "Примечание 7", "Сигнал 7" }));

            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 1", "Примечание 1" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 2", "Примечание 2" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 3", "Примечание 3" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 4", "Примечание 4" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 5", "Примечание 5" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 6", "Примечание 6" }));
            advancedProjSignalsList.Items.Add(new ListViewItem(new string[] { "Сигнал 7", "Примечание 7" }));
        }
    }
}
