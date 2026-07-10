namespace Editor
{
    partial class TechObjectsImportForm
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
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.overviewButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.selectedAllObjects = new System.Windows.Forms.LinkLabel();
            this.clearSelectedObjects = new System.Windows.Forms.LinkLabel();
            this.importingObjectsTree = new Aga.Controls.Tree.TreeViewAdv();
            this.MainTLP = new System.Windows.Forms.TableLayoutPanel();
            this.SelectorTLP = new System.Windows.Forms.TableLayoutPanel();
            this.FileOverviewTLP = new System.Windows.Forms.TableLayoutPanel();
            this.DialogTLP = new System.Windows.Forms.TableLayoutPanel();
            this.MainTLP.SuspendLayout();
            this.SelectorTLP.SuspendLayout();
            this.FileOverviewTLP.SuspendLayout();
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
            this.label1.Text = "Выберите файл для импорта:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePathTextBox.Location = new System.Drawing.Point(3, 3);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.ReadOnly = true;
            this.filePathTextBox.Size = new System.Drawing.Size(303, 20);
            this.filePathTextBox.TabIndex = 99;
            this.filePathTextBox.TabStop = false;
            // 
            // overviewButton
            // 
            this.overviewButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.overviewButton.Location = new System.Drawing.Point(312, 3);
            this.overviewButton.Name = "overviewButton";
            this.overviewButton.Size = new System.Drawing.Size(69, 21);
            this.overviewButton.TabIndex = 3;
            this.overviewButton.Text = "Обзор";
            this.overviewButton.UseVisualStyleBackColor = true;
            this.overviewButton.Click += new System.EventHandler(this.overviewButton_Click);
            // 
            // importButton
            // 
            this.importButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importButton.Location = new System.Drawing.Point(147, 13);
            this.importButton.Margin = new System.Windows.Forms.Padding(3, 13, 3, 13);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(114, 24);
            this.importButton.TabIndex = 2;
            this.importButton.Text = "Импортировать";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
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
            // selectedAllObjects
            // 
            this.selectedAllObjects.ActiveLinkColor = System.Drawing.Color.Black;
            this.selectedAllObjects.AutoSize = true;
            this.selectedAllObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedAllObjects.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.selectedAllObjects.LinkColor = System.Drawing.Color.Black;
            this.selectedAllObjects.Location = new System.Drawing.Point(63, 0);
            this.selectedAllObjects.Name = "selectedAllObjects";
            this.selectedAllObjects.Size = new System.Drawing.Size(34, 25);
            this.selectedAllObjects.TabIndex = 4;
            this.selectedAllObjects.TabStop = true;
            this.selectedAllObjects.Text = "Всё";
            this.selectedAllObjects.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectedAllObjects.VisitedLinkColor = System.Drawing.Color.Black;
            this.selectedAllObjects.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.selectedAllObjects_LinkClicked);
            // 
            // clearSelectedObjects
            // 
            this.clearSelectedObjects.ActiveLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjects.AutoSize = true;
            this.clearSelectedObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearSelectedObjects.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.clearSelectedObjects.LinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjects.Location = new System.Drawing.Point(103, 0);
            this.clearSelectedObjects.Name = "clearSelectedObjects";
            this.clearSelectedObjects.Size = new System.Drawing.Size(278, 25);
            this.clearSelectedObjects.TabIndex = 5;
            this.clearSelectedObjects.TabStop = true;
            this.clearSelectedObjects.Text = "Очистить";
            this.clearSelectedObjects.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearSelectedObjects.VisitedLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjects.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clearSelectedObjects_LinkClicked);
            // 
            // importingObjectsTree
            // 
            this.importingObjectsTree.BackColor = System.Drawing.SystemColors.Window;
            this.importingObjectsTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.importingObjectsTree.DefaultToolTipProvider = null;
            this.importingObjectsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importingObjectsTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.importingObjectsTree.FullRowSelect = true;
            this.importingObjectsTree.FullRowSelectActiveColor = System.Drawing.Color.Empty;
            this.importingObjectsTree.FullRowSelectInactiveColor = System.Drawing.Color.Empty;
            this.importingObjectsTree.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.importingObjectsTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.importingObjectsTree.Location = new System.Drawing.Point(3, 83);
            this.importingObjectsTree.Model = null;
            this.importingObjectsTree.Name = "importingObjectsTree";
            this.importingObjectsTree.NodeFilter = null;
            this.importingObjectsTree.SelectedNode = null;
            this.importingObjectsTree.ShowNodeToolTips = true;
            this.importingObjectsTree.Size = new System.Drawing.Size(378, 305);
            this.importingObjectsTree.TabIndex = 100;
            this.importingObjectsTree.Text = "exportingDevices";
            this.importingObjectsTree.UseColumns = true;
            // 
            // MainTLP
            // 
            this.MainTLP.ColumnCount = 1;
            this.MainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTLP.Controls.Add(this.label1, 0, 0);
            this.MainTLP.Controls.Add(this.importingObjectsTree, 0, 3);
            this.MainTLP.Controls.Add(this.SelectorTLP, 0, 2);
            this.MainTLP.Controls.Add(this.FileOverviewTLP, 0, 1);
            this.MainTLP.Controls.Add(this.DialogTLP, 0, 4);
            this.MainTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTLP.Location = new System.Drawing.Point(0, 0);
            this.MainTLP.Margin = new System.Windows.Forms.Padding(0);
            this.MainTLP.Name = "MainTLP";
            this.MainTLP.RowCount = 5;
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainTLP.Size = new System.Drawing.Size(384, 441);
            this.MainTLP.TabIndex = 101;
            // 
            // SelectorTLP
            // 
            this.SelectorTLP.ColumnCount = 3;
            this.SelectorTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.SelectorTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.SelectorTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.SelectorTLP.Controls.Add(this.label2, 0, 0);
            this.SelectorTLP.Controls.Add(this.selectedAllObjects, 1, 0);
            this.SelectorTLP.Controls.Add(this.clearSelectedObjects, 2, 0);
            this.SelectorTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectorTLP.Location = new System.Drawing.Point(0, 55);
            this.SelectorTLP.Margin = new System.Windows.Forms.Padding(0);
            this.SelectorTLP.Name = "SelectorTLP";
            this.SelectorTLP.RowCount = 1;
            this.SelectorTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SelectorTLP.Size = new System.Drawing.Size(384, 25);
            this.SelectorTLP.TabIndex = 101;
            // 
            // FileOverviewTLP
            // 
            this.FileOverviewTLP.ColumnCount = 2;
            this.FileOverviewTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FileOverviewTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.FileOverviewTLP.Controls.Add(this.filePathTextBox, 0, 0);
            this.FileOverviewTLP.Controls.Add(this.overviewButton, 1, 0);
            this.FileOverviewTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileOverviewTLP.Location = new System.Drawing.Point(0, 25);
            this.FileOverviewTLP.Margin = new System.Windows.Forms.Padding(0);
            this.FileOverviewTLP.Name = "FileOverviewTLP";
            this.FileOverviewTLP.RowCount = 1;
            this.FileOverviewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FileOverviewTLP.Size = new System.Drawing.Size(384, 30);
            this.FileOverviewTLP.TabIndex = 102;
            // 
            // DialogTLP
            // 
            this.DialogTLP.ColumnCount = 3;
            this.DialogTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DialogTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.DialogTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.DialogTLP.Controls.Add(this.cancelButton, 2, 0);
            this.DialogTLP.Controls.Add(this.importButton, 1, 0);
            this.DialogTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DialogTLP.Location = new System.Drawing.Point(0, 391);
            this.DialogTLP.Margin = new System.Windows.Forms.Padding(0);
            this.DialogTLP.Name = "DialogTLP";
            this.DialogTLP.RowCount = 1;
            this.DialogTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DialogTLP.Size = new System.Drawing.Size(384, 50);
            this.DialogTLP.TabIndex = 103;
            // 
            // TechObjectsImportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(384, 441);
            this.Controls.Add(this.MainTLP);
            this.MinimumSize = new System.Drawing.Size(400, 480);
            this.Name = "TechObjectsImportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Импорт объектов";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportObjectsForm_FormClosed);
            this.MainTLP.ResumeLayout(false);
            this.MainTLP.PerformLayout();
            this.SelectorTLP.ResumeLayout(false);
            this.SelectorTLP.PerformLayout();
            this.FileOverviewTLP.ResumeLayout(false);
            this.FileOverviewTLP.PerformLayout();
            this.DialogTLP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Button overviewButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel selectedAllObjects;
        private System.Windows.Forms.LinkLabel clearSelectedObjects;
        private Aga.Controls.Tree.TreeViewAdv importingObjectsTree;
        private System.Windows.Forms.TableLayoutPanel MainTLP;
        private System.Windows.Forms.TableLayoutPanel SelectorTLP;
        private System.Windows.Forms.TableLayoutPanel FileOverviewTLP;
        private System.Windows.Forms.TableLayoutPanel DialogTLP;
    }
}