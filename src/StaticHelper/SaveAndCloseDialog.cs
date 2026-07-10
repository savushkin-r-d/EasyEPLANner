using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StaticHelper
{
    [ExcludeFromCodeCoverage]
    public partial class SaveAndCloseDialog : Form
    {
        public SaveAndCloseDialog()
        {
            InitializeComponent();  
        }

        private void Save_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void DontSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public static DialogResult ShowDialog(Form owner = null)
        {
            return new SaveAndCloseDialog() { Text = owner?.Text }
                .ShowDialog(owner as IWin32Window);
        }
    }
}
