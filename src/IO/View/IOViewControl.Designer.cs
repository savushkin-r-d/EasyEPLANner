using BrightIdeasSoftware;

namespace IO.View
{
    partial class IOViewControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IOViewControl));
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.StructPLC = new BrightIdeasSoftware.TreeListView();
            this.ViewItemImageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Expand = new System.Windows.Forms.ToolStripDropDownButton();
            this.Expand_1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_2 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_3 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.MainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StructPLC)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 1;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Controls.Add(this.StructPLC, 0, 1);
            this.MainTableLayoutPanel.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 2;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(597, 521);
            this.MainTableLayoutPanel.TabIndex = 1;
            // 
            // StructPLC
            // 
            this.StructPLC.AlternateRowBackColor = System.Drawing.Color.White;
            this.StructPLC.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.StructPLC.CellEditUseWholeCell = false;
            this.StructPLC.CopySelectionOnControlCUsesDragSource = false;
            this.StructPLC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StructPLC.FullRowSelect = true;
            this.StructPLC.GridLines = true;
            this.StructPLC.HideSelection = false;
            this.StructPLC.LabelWrap = false;
            this.StructPLC.LargeImageList = this.ViewItemImageList;
            this.StructPLC.Location = new System.Drawing.Point(0, 30);
            this.StructPLC.Margin = new System.Windows.Forms.Padding(0);
            this.StructPLC.MultiSelect = false;
            this.StructPLC.Name = "StructPLC";
            this.StructPLC.OwnerDrawnHeader = true;
            this.StructPLC.PersistentCheckBoxes = false;
            this.StructPLC.RowHeight = 20;
            this.StructPLC.SelectAllOnControlA = false;
            this.StructPLC.SelectColumnsMenuStaysOpen = false;
            this.StructPLC.SelectColumnsOnRightClick = false;
            this.StructPLC.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.StructPLC.ShowFilterMenuOnRightClick = false;
            this.StructPLC.ShowGroups = false;
            this.StructPLC.ShowImagesOnSubItems = true;
            this.StructPLC.ShowSortIndicators = false;
            this.StructPLC.Size = new System.Drawing.Size(597, 491);
            this.StructPLC.SmallImageList = this.ViewItemImageList;
            this.StructPLC.TabIndex = 0;
            this.StructPLC.TriggerCellOverEventsWhenOverHeader = false;
            this.StructPLC.UseAlternatingBackColors = true;
            this.StructPLC.UseCellFormatEvents = true;
            this.StructPLC.UseCompatibleStateImageBehavior = false;
            this.StructPLC.UseHotControls = false;
            this.StructPLC.View = System.Windows.Forms.View.Details;
            this.StructPLC.VirtualMode = true;
            this.StructPLC.Expanded += new System.EventHandler<BrightIdeasSoftware.TreeBranchExpandedEventArgs>(this.ItemExpanded);
            this.StructPLC.Collapsed += new System.EventHandler<BrightIdeasSoftware.TreeBranchCollapsedEventArgs>(this.ItemCollapsed);
            this.StructPLC.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.CellEditFinishing);
            this.StructPLC.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.CellEditStarting);
            this.StructPLC.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.StructPLC_FormatCell);
            this.StructPLC.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.SelectionChanged);
            this.StructPLC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownHandler);
            this.StructPLC.MouseEnter += new System.EventHandler(this.StructPLC_MouseEnter);
            this.StructPLC.MouseLeave += new System.EventHandler(this.StructPLC_MouseLeave);
            // 
            // ViewItemImageList
            // 
            this.ViewItemImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ViewItemImageList.ImageStream")));
            this.ViewItemImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ViewItemImageList.Images.SetKeyName(0, "cab.png");
            this.ViewItemImageList.Images.SetKeyName(1, "node.png");
            this.ViewItemImageList.Images.SetKeyName(2, "module_black.png");
            this.ViewItemImageList.Images.SetKeyName(3, "module_blue.png");
            this.ViewItemImageList.Images.SetKeyName(4, "module_gray.png");
            this.ViewItemImageList.Images.SetKeyName(5, "module_green.png");
            this.ViewItemImageList.Images.SetKeyName(6, "module_lime.png");
            this.ViewItemImageList.Images.SetKeyName(7, "module_orange.png");
            this.ViewItemImageList.Images.SetKeyName(8, "module_red.png");
            this.ViewItemImageList.Images.SetKeyName(9, "module_violet.png");
            this.ViewItemImageList.Images.SetKeyName(10, "module_yellow.png");
            this.ViewItemImageList.Images.SetKeyName(11, "clamp.png");
            this.ViewItemImageList.Images.SetKeyName(12, "cable.png");
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(597, 30);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Expand,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(597, 30);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Expand
            // 
            this.Expand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Expand.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Expand_1,
            this.Expand_2,
            this.Expand_3,
            this.Expand_4});
            this.Expand.Image = ((System.Drawing.Image)(resources.GetObject("Expand.Image")));
            this.Expand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Expand.Name = "Expand";
            this.Expand.Size = new System.Drawing.Size(29, 27);
            this.Expand.Text = "toolStripDropDownButton1";
            // 
            // Expand_1
            // 
            this.Expand_1.Image = ((System.Drawing.Image)(resources.GetObject("Expand_1.Image")));
            this.Expand_1.Name = "Expand_1";
            this.Expand_1.Size = new System.Drawing.Size(183, 22);
            this.Expand_1.Tag = "1";
            this.Expand_1.Text = "Шкафы управления";
            this.Expand_1.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_2
            // 
            this.Expand_2.Image = ((System.Drawing.Image)(resources.GetObject("Expand_2.Image")));
            this.Expand_2.Name = "Expand_2";
            this.Expand_2.Size = new System.Drawing.Size(183, 22);
            this.Expand_2.Tag = "2";
            this.Expand_2.Text = "Узлы";
            this.Expand_2.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_3
            // 
            this.Expand_3.Image = ((System.Drawing.Image)(resources.GetObject("Expand_3.Image")));
            this.Expand_3.Name = "Expand_3";
            this.Expand_3.Size = new System.Drawing.Size(183, 22);
            this.Expand_3.Tag = "3";
            this.Expand_3.Text = "Модули";
            this.Expand_3.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_4
            // 
            this.Expand_4.Image = ((System.Drawing.Image)(resources.GetObject("Expand_4.Image")));
            this.Expand_4.Name = "Expand_4";
            this.Expand_4.Size = new System.Drawing.Size(183, 22);
            this.Expand_4.Tag = "4";
            this.Expand_4.Text = "Клеммы";
            this.Expand_4.Click += new System.EventHandler(this.Expand_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 27);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // IOViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 521);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Name = "IOViewControl";
            this.Text = "IOViewControl";
            this.MainTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StructPLC)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private TreeListView StructPLC;
        private System.Windows.Forms.ImageList ViewItemImageList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton Expand;
        private System.Windows.Forms.ToolStripMenuItem Expand_1;
        private System.Windows.Forms.ToolStripMenuItem Expand_2;
        private System.Windows.Forms.ToolStripMenuItem Expand_3;
        private System.Windows.Forms.ToolStripMenuItem Expand_4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}