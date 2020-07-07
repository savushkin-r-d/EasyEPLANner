using System;
using System.Windows.Forms;

namespace InterprojectExchange
{
    /// <summary>
    /// Форма выбора типа устройств при невозможности определения
    /// </summary>
    public partial class UnknownDevTypeForm : Form
    {
        public UnknownDevTypeForm()
        {
            InitializeComponent();
            checkedButton = AIBtn;
        }

        private RadioButton checkedButton;

        /// <summary>
        /// Имя группы для добавления в неё устройств
        /// </summary>
        public string GroupForAddingDeviceName { get; set; } = null;

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void UnknownDevTypeForm_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            Dispose();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            GroupForAddingDeviceName = checkedButton.Text;
            Hide();
        }

        private void AIBtn_CheckedChanged(object sender, EventArgs e)
        {
            if(AIBtn.Checked == true)
            {
                checkedButton = AIBtn;
                AOBtn.Checked = false;
                DIBtn.Checked = false;
                DOBtn.Checked = false;
            }
        }

        private void AOBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (AOBtn.Checked == true)
            {
                checkedButton = AOBtn;
                AIBtn.Checked = false;
                DIBtn.Checked = false;
                DOBtn.Checked = false;
            }
        }

        private void DIBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (DIBtn.Checked == true)
            {
                checkedButton = DIBtn;
                AIBtn.Checked = false;
                AOBtn.Checked = false;
                DOBtn.Checked = false;
            }
        }

        private void DOBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (DOBtn.Checked == true)
            {
                checkedButton = DOBtn;
                AIBtn.Checked = false;
                AOBtn.Checked = false;
                DIBtn.Checked = false;
            }
        }
    }
}
