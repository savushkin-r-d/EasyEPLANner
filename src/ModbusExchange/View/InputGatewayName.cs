using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange.View
{
    public partial class InputGatewayName : Form
    {
        public string GatewayName { get; set; }

        public InputGatewayName()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            if (NameTB.Text == string.Empty)
                return;

            GatewayName = NameTB.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
