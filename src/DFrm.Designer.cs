namespace EasyEPlanner
    {
    partial class DFrm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
            {
            if ( disposing && ( components != null ) )
                {
                components.Dispose();
                }
            base.Dispose( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.noAssigmentBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.synchBtn = new System.Windows.Forms.ToolStripButton();
            this.devicesTreeViewAdv = new Aga.Controls.Tree.TreeViewAdv();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSeparator1,
            this.noAssigmentBtn,
            this.toolStripSeparator2,
            this.synchBtn});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.Size = new System.Drawing.Size(304, 26);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::EasyEPlanner.Properties.Resources.plus1;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 23);
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
            this.toolStripButton2.Size = new System.Drawing.Size(23, 23);
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
            this.toolStripButton3.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton3.Tag = "2";
            this.toolStripButton3.Text = "Третий уровень";
            this.toolStripButton3.ToolTipText = "Третий уровень";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = global::EasyEPlanner.Properties.Resources.plus4;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton4.Tag = "3";
            this.toolStripButton4.Text = "Четвертый уровень";
            this.toolStripButton4.ToolTipText = "Четвертый уровень";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 26);
            // 
            // noAssigmentBtn
            // 
            this.noAssigmentBtn.Checked = true;
            this.noAssigmentBtn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noAssigmentBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.noAssigmentBtn.Image = global::EasyEPlanner.Properties.Resources.toCheck;
            this.noAssigmentBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.noAssigmentBtn.Name = "noAssigmentBtn";
            this.noAssigmentBtn.Size = new System.Drawing.Size(23, 23);
            this.noAssigmentBtn.Text = "Без привязки";
            this.noAssigmentBtn.Click += new System.EventHandler(this.noAssigmentBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 26);
            // 
            // synchBtn
            // 
            this.synchBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.synchBtn.Image = global::EasyEPlanner.Properties.Resources.refresh;
            this.synchBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.synchBtn.Name = "synchBtn";
            this.synchBtn.Size = new System.Drawing.Size(23, 23);
            this.synchBtn.Text = "Обновить";
            this.synchBtn.Click += new System.EventHandler(this.synchBtn_Click);
            // 
            // devicesTreeViewAdv
            // 
            this.devicesTreeViewAdv.BackColor = System.Drawing.SystemColors.Window;
            this.devicesTreeViewAdv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.devicesTreeViewAdv.DefaultToolTipProvider = null;
            this.devicesTreeViewAdv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicesTreeViewAdv.DragDropMarkColor = System.Drawing.Color.Black;
            this.devicesTreeViewAdv.FullRowSelect = true;
            this.devicesTreeViewAdv.FullRowSelectActiveColor = System.Drawing.Color.Empty;
            this.devicesTreeViewAdv.FullRowSelectInactiveColor = System.Drawing.Color.Empty;
            this.devicesTreeViewAdv.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.devicesTreeViewAdv.LineColor = System.Drawing.SystemColors.ControlDark;
            this.devicesTreeViewAdv.Location = new System.Drawing.Point(0, 26);
            this.devicesTreeViewAdv.Model = null;
            this.devicesTreeViewAdv.Name = "devicesTreeViewAdv";
            this.devicesTreeViewAdv.NodeFilter = null;
            this.devicesTreeViewAdv.SelectedNode = null;
            this.devicesTreeViewAdv.ShowNodeToolTips = true;
            this.devicesTreeViewAdv.Size = new System.Drawing.Size(304, 804);
            this.devicesTreeViewAdv.TabIndex = 5;
            this.devicesTreeViewAdv.Text = "devicesTreeViewAdv";
            this.devicesTreeViewAdv.UseColumns = true;
            this.devicesTreeViewAdv.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.treeView_DoubleClick);
            // 
            // DFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 830);
            this.Controls.Add(this.devicesTreeViewAdv);
            this.Controls.Add(this.toolStrip);
            this.MinimizeBox = false;
            this.Name = "DFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Устройства";
            this.TopMost = true;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

            }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton noAssigmentBtn;
        private System.Windows.Forms.ToolStripButton synchBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private Aga.Controls.Tree.TreeViewAdv devicesTreeViewAdv;
    }
    }