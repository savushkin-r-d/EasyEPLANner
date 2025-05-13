using EasyEPlanner.ModbusExchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange.View
{
    public partial class InputGatewayName : Form
    {
        readonly IExchange exchange;

        public string GatewayName { get; set; }

        public InputGatewayName(IExchange exchange)
        {
            this.exchange = exchange;
            InitializeComponent();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            if (NameTextBox.Text == string.Empty)
                return;

            if (exchange.Models.Select(m => m.Name).Contains(NameTextBox.Text))
                return;

            if (Regex.IsMatch(NameTextBox.Text, @"\p{IsCyrillic}",
                RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                return;

            GatewayName = NameTextBox.Text;

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
