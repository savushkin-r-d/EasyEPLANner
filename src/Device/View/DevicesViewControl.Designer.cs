using BrightIdeasSoftware;

namespace EasyEPlanner.Devices.View
{
    public partial class DevicesViewControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolbarPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Expand = new System.Windows.Forms.ToolStripDropDownButton();
            this.Expand_1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_2 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_3 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_4 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_5 = new System.Windows.Forms.ToolStripMenuItem();
            this.Expand_6 = new System.Windows.Forms.ToolStripMenuItem();
            this.syncButton = new System.Windows.Forms.ToolStripButton();
            this.groupingToggleButton = new System.Windows.Forms.ToolStripButton();
            this.searchButtonToolStrip = new System.Windows.Forms.ToolStrip();
            this.searchTSButton = new System.Windows.Forms.ToolStripButton();
            this.searchBoxTLP = new System.Windows.Forms.TableLayoutPanel();
            this.searchPictureBox = new System.Windows.Forms.PictureBox();
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.searchIteratorPanel = new System.Windows.Forms.Panel();
            this.devicesTree = new BrightIdeasSoftware.TreeListView();
            this.ViewItemImageList = new System.Windows.Forms.ImageList(this.components);
            this.MainTableLayoutPanel.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.searchButtonToolStrip.SuspendLayout();
            this.searchBoxTLP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.devicesTree)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 1;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Controls.Add(this.toolbarPanel, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.devicesTree, 0, 1);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 2;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(800, 600);
            this.MainTableLayoutPanel.TabIndex = 0;
            // 
            // toolbarPanel
            // 
            this.toolbarPanel.ColumnCount = 3;
            this.toolbarPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.toolbarPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.toolbarPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.toolbarPanel.Controls.Add(this.toolStrip1, 0, 0);
            this.toolbarPanel.Controls.Add(this.searchButtonToolStrip, 1, 0);
            this.toolbarPanel.Controls.Add(this.searchBoxTLP, 2, 0);
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarPanel.Location = new System.Drawing.Point(0, 0);
            this.toolbarPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.RowCount = 1;
            this.toolbarPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.toolbarPanel.Size = new System.Drawing.Size(800, 30);
            this.toolbarPanel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Expand,
            this.syncButton,
            this.groupingToggleButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(543, 30);
            this.toolStrip1.TabIndex = 0;
            // 
            // Expand
            // 
            this.Expand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Expand.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Expand_1,
            this.Expand_2,
            this.Expand_3,
            this.Expand_4,
            this.Expand_5,
            this.Expand_6});
            this.Expand.Image = global::EasyEPlanner.Properties.Resources.expand;
            this.Expand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Expand.Name = "Expand";
            this.Expand.Size = new System.Drawing.Size(29, 27);
            this.Expand.Text = "Развернуть";
            this.Expand.ToolTipText = "Выбрать уровень развертки";
            // 
            // Expand_1
            // 
            this.Expand_1.Image = global::EasyEPlanner.Properties.Resources.expand1;
            this.Expand_1.Name = "Expand_1";
            this.Expand_1.Size = new System.Drawing.Size(146, 22);
            this.Expand_1.Tag = "1";
            this.Expand_1.Text = "Уровень 1";
            this.Expand_1.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_2
            // 
            this.Expand_2.Image = global::EasyEPlanner.Properties.Resources.expand2;
            this.Expand_2.Name = "Expand_2";
            this.Expand_2.Size = new System.Drawing.Size(146, 22);
            this.Expand_2.Tag = "2";
            this.Expand_2.Text = "Уровень 2";
            this.Expand_2.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_3
            // 
            this.Expand_3.Image = global::EasyEPlanner.Properties.Resources.expand3;
            this.Expand_3.Name = "Expand_3";
            this.Expand_3.Size = new System.Drawing.Size(146, 22);
            this.Expand_3.Tag = "3";
            this.Expand_3.Text = "Уровень 3";
            this.Expand_3.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_4
            // 
            this.Expand_4.Image = global::EasyEPlanner.Properties.Resources.expand4;
            this.Expand_4.Name = "Expand_4";
            this.Expand_4.Size = new System.Drawing.Size(146, 22);
            this.Expand_4.Tag = "4";
            this.Expand_4.Text = "Уровень 4";
            this.Expand_4.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_5
            // 
            this.Expand_5.Image = global::EasyEPlanner.Properties.Resources.expand5;
            this.Expand_5.Name = "Expand_5";
            this.Expand_5.Size = new System.Drawing.Size(146, 22);
            this.Expand_5.Tag = "5";
            this.Expand_5.Text = "Уровень 5";
            this.Expand_5.Click += new System.EventHandler(this.Expand_Click);
            // 
            // Expand_6
            // 
            this.Expand_6.Name = "Expand_6";
            this.Expand_6.Size = new System.Drawing.Size(146, 22);
            this.Expand_6.Tag = "0";
            this.Expand_6.Text = "Свернуть всё";
            this.Expand_6.Click += new System.EventHandler(this.Expand_Click);
            // 
            // syncButton
            // 
            this.syncButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.syncButton.Image = global::EasyEPlanner.Properties.Resources.refresh;
            this.syncButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(23, 27);
            this.syncButton.Text = "Обновить";
            this.syncButton.ToolTipText = "Обновить";
            this.syncButton.Click += new System.EventHandler(this.SyncButton_Click);
            // 
            // groupingToggleButton
            // 
            this.groupingToggleButton.CheckOnClick = true;
            this.groupingToggleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.groupingToggleButton.Image = global::EasyEPlanner.Properties.Resources.devicesGroupingObjectType;
            this.groupingToggleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.groupingToggleButton.Name = "groupingToggleButton";
            this.groupingToggleButton.Size = new System.Drawing.Size(23, 27);
            this.groupingToggleButton.Click += new System.EventHandler(this.GroupingToggleButton_Click);
            // 
            // searchButtonToolStrip
            // 
            this.searchButtonToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchButtonToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.searchButtonToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchTSButton});
            this.searchButtonToolStrip.Location = new System.Drawing.Point(543, 0);
            this.searchButtonToolStrip.Name = "searchButtonToolStrip";
            this.searchButtonToolStrip.Size = new System.Drawing.Size(57, 30);
            this.searchButtonToolStrip.TabIndex = 2;
            // 
            // searchTSButton
            // 
            this.searchTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchTSButton.Image = global::EasyEPlanner.Properties.Resources.search;
            this.searchTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchTSButton.Name = "searchTSButton";
            this.searchTSButton.Size = new System.Drawing.Size(23, 27);
            this.searchTSButton.ToolTipText = "Поиск";
            this.searchTSButton.Click += new System.EventHandler(this.SearchTSButton_Click);
            // 
            // searchBoxTLP
            // 
            this.searchBoxTLP.AutoSize = true;
            this.searchBoxTLP.BackColor = System.Drawing.SystemColors.Window;
            this.searchBoxTLP.ColumnCount = 3;
            this.searchBoxTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.searchBoxTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchBoxTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.searchBoxTLP.Controls.Add(this.searchPictureBox, 0, 0);
            this.searchBoxTLP.Controls.Add(this.textBox_search, 1, 0);
            this.searchBoxTLP.Controls.Add(this.searchIteratorPanel, 2, 0);
            this.searchBoxTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchBoxTLP.Location = new System.Drawing.Point(602, 2);
            this.searchBoxTLP.Margin = new System.Windows.Forms.Padding(2);
            this.searchBoxTLP.Name = "searchBoxTLP";
            this.searchBoxTLP.RowCount = 1;
            this.searchBoxTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchBoxTLP.Size = new System.Drawing.Size(196, 26);
            this.searchBoxTLP.TabIndex = 1;
            this.searchBoxTLP.Visible = false;
            // 
            // searchPictureBox
            // 
            this.searchPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPictureBox.Image = global::EasyEPlanner.Properties.Resources.search;
            this.searchPictureBox.Location = new System.Drawing.Point(3, 5);
            this.searchPictureBox.Margin = new System.Windows.Forms.Padding(3, 5, 2, 4);
            this.searchPictureBox.MaximumSize = new System.Drawing.Size(14, 14);
            this.searchPictureBox.Name = "searchPictureBox";
            this.searchPictureBox.Size = new System.Drawing.Size(14, 14);
            this.searchPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.searchPictureBox.TabIndex = 0;
            this.searchPictureBox.TabStop = false;
            // 
            // textBox_search
            // 
            this.textBox_search.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_search.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_search.ForeColor = System.Drawing.Color.Gray;
            this.textBox_search.Location = new System.Drawing.Point(20, 7);
            this.textBox_search.Margin = new System.Windows.Forms.Padding(1, 7, 1, 5);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(105, 13);
            this.textBox_search.TabIndex = 0;
            this.textBox_search.Text = "Поиск...";
            this.textBox_search.TextChanged += new System.EventHandler(this.TextBox_search_TextChanged);
            this.textBox_search.Enter += new System.EventHandler(this.TextBox_search_Enter);
            this.textBox_search.Leave += new System.EventHandler(this.TextBox_search_Leave);
            // 
            // searchIteratorPanel
            // 
            this.searchIteratorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchIteratorPanel.Location = new System.Drawing.Point(129, 3);
            this.searchIteratorPanel.Margin = new System.Windows.Forms.Padding(3, 3, 1, 1);
            this.searchIteratorPanel.Name = "searchIteratorPanel";
            this.searchIteratorPanel.Size = new System.Drawing.Size(66, 22);
            this.searchIteratorPanel.TabIndex = 1;
            // 
            // devicesTree
            // 
            this.devicesTree.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.devicesTree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.devicesTree.CellEditUseWholeCell = false;
            this.devicesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicesTree.FullRowSelect = true;
            this.devicesTree.GridLines = true;
            this.devicesTree.HideSelection = false;
            this.devicesTree.LargeImageList = this.ViewItemImageList;
            this.devicesTree.Location = new System.Drawing.Point(0, 30);
            this.devicesTree.Margin = new System.Windows.Forms.Padding(0);
            this.devicesTree.Name = "devicesTree";
            this.devicesTree.RowHeight = 20;
            this.devicesTree.ShowGroups = false;
            this.devicesTree.Size = new System.Drawing.Size(800, 570);
            this.devicesTree.SmallImageList = this.ViewItemImageList;
            this.devicesTree.TabIndex = 1;
            this.devicesTree.UseAlternatingBackColors = true;
            this.devicesTree.UseCellFormatEvents = true;
            this.devicesTree.UseCompatibleStateImageBehavior = false;
            this.devicesTree.View = System.Windows.Forms.View.Details;
            this.devicesTree.VirtualMode = true;
            this.devicesTree.Expanded += new System.EventHandler<BrightIdeasSoftware.TreeBranchExpandedEventArgs>(this.ItemExpanded);
            this.devicesTree.Collapsed += new System.EventHandler<BrightIdeasSoftware.TreeBranchCollapsedEventArgs>(this.ItemCollapsed);
            this.devicesTree.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.CellEditFinishing);
            this.devicesTree.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.CellEditStarting);
            this.devicesTree.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.DevicesTree_FormatCell);
            this.devicesTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DevicesTree_KeyDown);
            this.devicesTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DevicesTree_MouseDown);
            // 
            // ViewItemImageList
            // 
            this.ViewItemImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ViewItemImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.ViewItemImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // DevicesViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Name = "DevicesViewControl";
            this.Text = "Устройства (new)";
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.searchButtonToolStrip.ResumeLayout(false);
            this.searchButtonToolStrip.PerformLayout();
            this.searchBoxTLP.ResumeLayout(false);
            this.searchBoxTLP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.devicesTree)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private TreeListView devicesTree;
        private System.Windows.Forms.ImageList ViewItemImageList;
        private System.Windows.Forms.TableLayoutPanel toolbarPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton Expand;
        private System.Windows.Forms.ToolStripMenuItem Expand_1;
        private System.Windows.Forms.ToolStripMenuItem Expand_2;
        private System.Windows.Forms.ToolStripMenuItem Expand_3;
        private System.Windows.Forms.ToolStripMenuItem Expand_4;
        private System.Windows.Forms.ToolStripMenuItem Expand_5;
        private System.Windows.Forms.ToolStripMenuItem Expand_6;
        private System.Windows.Forms.ToolStripButton syncButton;
        private System.Windows.Forms.ToolStripButton groupingToggleButton;
        private System.Windows.Forms.ToolStrip searchButtonToolStrip;
        private System.Windows.Forms.ToolStripButton searchTSButton;
        private System.Windows.Forms.TableLayoutPanel searchBoxTLP;
        private System.Windows.Forms.PictureBox searchPictureBox;
        private System.Windows.Forms.TextBox textBox_search;
        private System.Windows.Forms.Panel searchIteratorPanel;
    }
}
