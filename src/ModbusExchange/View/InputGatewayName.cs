using EasyEPlanner.ModbusExchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange.View
{
    [ExcludeFromCodeCoverage]
    public partial class InputGatewayName : Form
    {
        readonly IExchange exchange;

        public string GatewayName { get; set; }

        private static readonly string InvalidFileNameChars =
            Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));

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

            if (Regex.IsMatch(NameTextBox.Text,
                @$"[{InvalidFileNameChars}]|\.|\p{{IsCyrillic}}",
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

        private void InputGatewayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Return)
                Apply.PerformClick();

            if (e.KeyCode is Keys.Escape)
                Cancel.PerformClick();
        }
    }
}
