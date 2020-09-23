namespace EasyEPlanner
{
    partial class ModeFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.modesTreeViewAdv = new Aga.Controls.Tree.TreeViewAdv();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::EasyEPlanner.Properties.Resources.plus1;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Tag = "0";
            this.toolStripButton1.ToolTipText = "Первый уровень";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::EasyEPlanner.Properties.Resources.plus2;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Tag = "1";
            this.toolStripButton2.Text = "Второй уровень";
            this.toolStripButton2.ToolTipText = "Второй уровень";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::EasyEPlanner.Properties.Resources.plus3;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Tag = "2";
            this.toolStripButton3.Text = "Третий уровень";
            this.toolStripButton3.ToolTipText = "Третий уровень";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(158, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = global::EasyEPlanner.Properties.Resources.plus4;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Tag = "3";
            this.toolStripButton4.Text = "Четвертый уровень";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = global::EasyEPlanner.Properties.Resources.plus5;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Tag = "4";
            this.toolStripButton5.Text = "Пятый уровень";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // modesTreeViewAdv
            // 
            this.modesTreeViewAdv.BackColor = System.Drawing.SystemColors.Window;
            this.modesTreeViewAdv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.modesTreeViewAdv.DefaultToolTipProvider = null;
            this.modesTreeViewAdv.DragDropMarkColor = System.Drawing.Color.Black;
            this.modesTreeViewAdv.FullRowSelect = true;
            this.modesTreeViewAdv.FullRowSelectActiveColor = System.Drawing.Color.Empty;
            this.modesTreeViewAdv.FullRowSelectInactiveColor = System.Drawing.Color.Empty;
            this.modesTreeViewAdv.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.modesTreeViewAdv.LineColor = System.Drawing.SystemColors.ControlDark;
            this.modesTreeViewAdv.Location = new System.Drawing.Point(12, 29);
            this.modesTreeViewAdv.Model = null;
            this.modesTreeViewAdv.Name = "modesTreeViewAdv";
            this.modesTreeViewAdv.NodeFilter = null;
            this.modesTreeViewAdv.SelectedNode = null;
            this.modesTreeViewAdv.ShowNodeToolTips = true;
            this.modesTreeViewAdv.Size = new System.Drawing.Size(264, 789);
            this.modesTreeViewAdv.TabIndex = 6;
            this.modesTreeViewAdv.Text = "modesViewAdv";
            this.modesTreeViewAdv.UseColumns = true;
            // 
            // ModeFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 830);
            this.Controls.Add(this.modesTreeViewAdv);
            this.Controls.Add(this.toolStrip);
            this.MinimizeBox = false;
            this.Name = "ModeFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Операции";
            this.TopMost = true;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private Aga.Controls.Tree.TreeViewAdv modesTreeViewAdv;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
    }
}