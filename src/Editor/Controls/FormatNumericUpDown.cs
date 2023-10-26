using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorControls
{
    public partial class FormatNumericUpDown : NumericUpDown
    {
        protected override void UpdateEditText()
        {
            if ((int)this.Value == 0)
            {
                this.Text = string.Empty;
                return;
            }

            this.Text = $"{this.Value}/{(int)this.Maximum}";
        }

        public override void UpButton()
        {
            if (this.Value == 0) return;
            if (this.Value == 1)
                this.Value = Maximum;
            else
                base.DownButton(); 
        }

        public override void DownButton()
        {
            if (this.Value == Maximum && Maximum != 0)
                this.Value = 1;
            else
                base.UpButton();
        }
    }
}
