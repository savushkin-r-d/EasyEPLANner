namespace Recipe
{
    partial class RecipeFrm
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
            this.components = new System.ComponentModel.Container();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addRecipeBtn = new System.Windows.Forms.ToolStripButton();
            this.delRecipeBtn = new System.Windows.Forms.ToolStripButton();
            this.copyRecipeBtn = new System.Windows.Forms.ToolStripButton();
            this.pasteRecipeBtn = new System.Windows.Forms.ToolStripButton();
            this.replaceRecipeBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editorRView = new BrightIdeasSoftware.TreeListView();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorRView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripSeparator1,
            this.addRecipeBtn,
            this.delRecipeBtn,
            this.copyRecipeBtn,
            this.pasteRecipeBtn,
            this.replaceRecipeBtn,
            this.toolStripSeparator2});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(170, 27);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::EasyEPlanner.Properties.Resources.plus1;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton1.Tag = "0";
            this.toolStripButton1.ToolTipText = "Первый уровень";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::EasyEPlanner.Properties.Resources.plus2;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton2.Tag = "1";
            this.toolStripButton2.Text = "Второй уровень";
            this.toolStripButton2.ToolTipText = "Второй уровень";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::EasyEPlanner.Properties.Resources.plus3;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton3.Tag = "2";
            this.toolStripButton3.Text = "Третий уровень";
            this.toolStripButton3.ToolTipText = "Третий уровень";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // addRecipeBtn
            // 
            this.addRecipeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addRecipeBtn.Image = global::EasyEPlanner.Properties.Resources.create;
            this.addRecipeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addRecipeBtn.Name = "addRecipeBtn";
            this.addRecipeBtn.Size = new System.Drawing.Size(29, 24);
            this.addRecipeBtn.Text = "Добавить";
            this.addRecipeBtn.Click += new System.EventHandler(this.addRecipeButton_Click);
            // 
            // delRecipeBtn
            // 
            this.delRecipeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.delRecipeBtn.Image = global::EasyEPlanner.Properties.Resources.delete;
            this.delRecipeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.delRecipeBtn.Name = "delRecipeBtn";
            this.delRecipeBtn.Size = new System.Drawing.Size(29, 24);
            this.delRecipeBtn.Text = "Удалить";
            this.delRecipeBtn.Click += new System.EventHandler(this.delRecipeButton_Click);
            // 
            // copyRecipeBtn
            // 
            this.copyRecipeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyRecipeBtn.Image = global::EasyEPlanner.Properties.Resources.copy;
            this.copyRecipeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyRecipeBtn.Name = "copyRecipeBtn";
            this.copyRecipeBtn.Size = new System.Drawing.Size(29, 24);
            this.copyRecipeBtn.Text = "Копировать";
            this.copyRecipeBtn.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // pasteRecipeBtn
            // 
            this.pasteRecipeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteRecipeBtn.Image = global::EasyEPlanner.Properties.Resources.paste;
            this.pasteRecipeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteRecipeBtn.Name = "pasteRecipeBtn";
            this.pasteRecipeBtn.Size = new System.Drawing.Size(29, 24);
            this.pasteRecipeBtn.Text = "Вставить";
            this.pasteRecipeBtn.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // replaceRecipeBtn
            // 
            this.replaceRecipeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.replaceRecipeBtn.Image = global::EasyEPlanner.Properties.Resources.replace;
            this.replaceRecipeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceRecipeBtn.Name = "replaceRecipeBtn";
            this.replaceRecipeBtn.Size = new System.Drawing.Size(29, 24);
            this.replaceRecipeBtn.Text = "Заменить";
            this.replaceRecipeBtn.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // editorRView
            // 
            this.editorRView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.editorRView.CellEditUseWholeCell = false;
            this.editorRView.CopySelectionOnControlCUsesDragSource = false;
            this.editorRView.FullRowSelect = true;
            this.editorRView.GridLines = true;
            this.editorRView.HideSelection = false;
            this.editorRView.LabelWrap = false;
            this.editorRView.Location = new System.Drawing.Point(3, 32);
            this.editorRView.MinimumSize = new System.Drawing.Size(405, 989);
            this.editorRView.MultiSelect = false;
            this.editorRView.Name = "editorRView";
            this.editorRView.OwnerDrawnHeader = true;
            this.editorRView.PersistentCheckBoxes = false;
            this.editorRView.RowHeight = 20;
            this.editorRView.SelectAllOnControlA = false;
            this.editorRView.SelectColumnsMenuStaysOpen = false;
            this.editorRView.SelectColumnsOnRightClick = false;
            this.editorRView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.editorRView.ShowFilterMenuOnRightClick = false;
            this.editorRView.ShowGroups = false;
            this.editorRView.ShowImagesOnSubItems = true;
            this.editorRView.ShowSortIndicators = false;
            this.editorRView.Size = new System.Drawing.Size(500, 989);
            this.editorRView.TabIndex = 4;
            this.editorRView.TriggerCellOverEventsWhenOverHeader = false;
            this.editorRView.UseAlternatingBackColors = true;
            this.editorRView.UseCellFormatEvents = true;
            this.editorRView.UseCompatibleStateImageBehavior = false;
            this.editorRView.UseHotControls = false;
            this.editorRView.View = System.Windows.Forms.View.Details;
            this.editorRView.VirtualMode = true;
            this.editorRView.Expanded += new System.EventHandler<BrightIdeasSoftware.TreeBranchExpandedEventArgs>(this.editorRView_Expanded);
            this.editorRView.Collapsed += new System.EventHandler<BrightIdeasSoftware.TreeBranchCollapsedEventArgs>(this.editorRView_Collapsed);
            this.editorRView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.editorRView_CellEditFinishing);
            this.editorRView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.editorRView_CellEditStarting);
            this.editorRView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.editorRView_ColumnWidthChanging);
            this.editorRView.SelectedIndexChanged += new System.EventHandler(this.editorRView_SelectedIndexChanged);
            this.editorRView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editorRView_KeyDown);
            this.editorRView.MouseEnter += new System.EventHandler(this.editorRView_MouseEnter);
            this.editorRView.MouseLeave += new System.EventHandler(this.editorRView_MouseLeave);
            // 
            // RecipeFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 1022);
            this.Controls.Add(this.editorRView);
            this.Controls.Add(this.toolStrip);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "RecipeFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Рецепты";
            this.TopMost = true;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorRView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton addRecipeBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public BrightIdeasSoftware.TreeListView editorRView;
        private System.Windows.Forms.ToolStripButton delRecipeBtn;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton copyRecipeBtn;
        private System.Windows.Forms.ToolStripButton pasteRecipeBtn;
        private System.Windows.Forms.ToolStripButton replaceRecipeBtn;
    }
}