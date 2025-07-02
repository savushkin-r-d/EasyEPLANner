namespace Editor
{
    partial class TechObjectsExportForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.exportButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.selectAllObjectsLink = new System.Windows.Forms.LinkLabel();
            this.clearSelectedObjectsLink = new System.Windows.Forms.LinkLabel();
            this.exportingObjectsTree = new Aga.Controls.Tree.TreeViewAdv();
            this.MainTLP = new System.Windows.Forms.TableLayoutPanel();
            this.SelectorTLP = new System.Windows.Forms.TableLayoutPanel();
            this.DialogTLP = new System.Windows.Forms.TableLayoutPanel();
            this.MainTLP.SuspendLayout();
            this.SelectorTLP.SuspendLayout();
            this.DialogTLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(378, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Выберите объекты для экспорта:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exportButton
            // 
            this.exportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportButton.Location = new System.Drawing.Point(147, 13);
            this.exportButton.Margin = new System.Windows.Forms.Padding(3, 13, 3, 13);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(114, 24);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Экспортировать";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelButton.Location = new System.Drawing.Point(267, 13);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 13, 3, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(114, 24);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Выбрать:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectAllObjectsLink
            // 
            this.selectAllObjectsLink.ActiveLinkColor = System.Drawing.Color.Black;
            this.selectAllObjectsLink.AutoSize = true;
            this.selectAllObjectsLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectAllObjectsLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.selectAllObjectsLink.LinkColor = System.Drawing.Color.Black;
            this.selectAllObjectsLink.Location = new System.Drawing.Point(63, 0);
            this.selectAllObjectsLink.Name = "selectAllObjectsLink";
            this.selectAllObjectsLink.Size = new System.Drawing.Size(34, 25);
            this.selectAllObjectsLink.TabIndex = 4;
            this.selectAllObjectsLink.TabStop = true;
            this.selectAllObjectsLink.Text = "Всё";
            this.selectAllObjectsLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectAllObjectsLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.selectAllObjectsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.selectAllObjects_LinkClicked);
            // 
            // clearSelectedObjectsLink
            // 
            this.clearSelectedObjectsLink.ActiveLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjectsLink.AutoSize = true;
            this.clearSelectedObjectsLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearSelectedObjectsLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.clearSelectedObjectsLink.LinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjectsLink.Location = new System.Drawing.Point(103, 0);
            this.clearSelectedObjectsLink.Name = "clearSelectedObjectsLink";
            this.clearSelectedObjectsLink.Size = new System.Drawing.Size(278, 25);
            this.clearSelectedObjectsLink.TabIndex = 5;
            this.clearSelectedObjectsLink.TabStop = true;
            this.clearSelectedObjectsLink.Text = "Очистить";
            this.clearSelectedObjectsLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearSelectedObjectsLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjectsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clearSelectedObjects_LinkClicked);
            // 
            // exportingObjectsTree
            // 
            this.exportingObjectsTree.BackColor = System.Drawing.SystemColors.Window;
            this.exportingObjectsTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exportingObjectsTree.DefaultToolTipProvider = null;
            this.exportingObjectsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportingObjectsTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.exportingObjectsTree.FullRowSelect = true;
            this.exportingObjectsTree.FullRowSelectActiveColor = System.Drawing.Color.Empty;
            this.exportingObjectsTree.FullRowSelectInactiveColor = System.Drawing.Color.Empty;
            this.exportingObjectsTree.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.exportingObjectsTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.exportingObjectsTree.Location = new System.Drawing.Point(3, 53);
            this.exportingObjectsTree.Model = null;
            this.exportingObjectsTree.Name = "exportingObjectsTree";
            this.exportingObjectsTree.NodeFilter = null;
            this.exportingObjectsTree.SelectedNode = null;
            this.exportingObjectsTree.ShowNodeToolTips = true;
            this.exportingObjectsTree.Size = new System.Drawing.Size(378, 335);
            this.exportingObjectsTree.TabIndex = 8;
            this.exportingObjectsTree.Text = "exportingDevices";
            this.exportingObjectsTree.UseColumns = true;
            // 
            // MainTLP
            // 
            this.MainTLP.ColumnCount = 1;
            this.MainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTLP.Controls.Add(this.exportingObjectsTree, 0, 2);
            this.MainTLP.Controls.Add(this.label1, 0, 0);
            this.MainTLP.Controls.Add(this.SelectorTLP, 0, 1);
            this.MainTLP.Controls.Add(this.DialogTLP, 0, 3);
            this.MainTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTLP.Location = new System.Drawing.Point(0, 0);
            this.MainTLP.Margin = new System.Windows.Forms.Padding(0);
            this.MainTLP.Name = "MainTLP";
            this.MainTLP.RowCount = 4;
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainTLP.Size = new System.Drawing.Size(384, 441);
            this.MainTLP.TabIndex = 9;
            // 
            // SelectorTLP
            // 
            this.SelectorTLP.ColumnCount = 3;
            this.SelectorTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.SelectorTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.SelectorTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SelectorTLP.Controls.Add(this.label2, 0, 0);
            this.SelectorTLP.Controls.Add(this.clearSelectedObjectsLink, 2, 0);
            this.SelectorTLP.Controls.Add(this.selectAllObjectsLink, 1, 0);
            this.SelectorTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectorTLP.Location = new System.Drawing.Point(0, 25);
            this.SelectorTLP.Margin = new System.Windows.Forms.Padding(0);
            this.SelectorTLP.Name = "SelectorTLP";
            this.SelectorTLP.RowCount = 1;
            this.SelectorTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SelectorTLP.Size = new System.Drawing.Size(384, 25);
            this.SelectorTLP.TabIndex = 9;
            // 
            // DialogTLP
            // 
            this.DialogTLP.ColumnCount = 3;
            this.DialogTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DialogTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.DialogTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.DialogTLP.Controls.Add(this.exportButton, 1, 0);
            this.DialogTLP.Controls.Add(this.cancelButton, 2, 0);
            this.DialogTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DialogTLP.Location = new System.Drawing.Point(0, 391);
            this.DialogTLP.Margin = new System.Windows.Forms.Padding(0);
            this.DialogTLP.Name = "DialogTLP";
            this.DialogTLP.RowCount = 1;
            this.DialogTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DialogTLP.Size = new System.Drawing.Size(384, 50);
            this.DialogTLP.TabIndex = 10;
            // 
            // TechObjectsExportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(384, 441);
            this.Controls.Add(this.MainTLP);
            this.MinimumSize = new System.Drawing.Size(400, 480);
            this.Name = "TechObjectsExportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Экспорт объектов";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportObjectsForm_FormClosed);
            this.Load += new System.EventHandler(this.ExportObjectsForm_Load);
            this.MainTLP.ResumeLayout(false);
            this.MainTLP.PerformLayout();
            this.SelectorTLP.ResumeLayout(false);
            this.SelectorTLP.PerformLayout();
            this.DialogTLP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel selectAllObjectsLink;
        private System.Windows.Forms.LinkLabel clearSelectedObjectsLink;
        private Aga.Controls.Tree.TreeViewAdv exportingObjectsTree;
        private System.Windows.Forms.TableLayoutPanel MainTLP;
        private System.Windows.Forms.TableLayoutPanel SelectorTLP;
        private System.Windows.Forms.TableLayoutPanel DialogTLP;
    }
}