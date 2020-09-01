namespace NewEditor
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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Выберите файл для импорта:";
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathTextBox.Location = new System.Drawing.Point(12, 25);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.ReadOnly = true;
            this.filePathTextBox.Size = new System.Drawing.Size(287, 20);
            this.filePathTextBox.TabIndex = 99;
            this.filePathTextBox.TabStop = false;
            // 
            // overviewButton
            // 
            this.overviewButton.Location = new System.Drawing.Point(305, 24);
            this.overviewButton.Name = "overviewButton";
            this.overviewButton.Size = new System.Drawing.Size(67, 21);
            this.overviewButton.TabIndex = 3;
            this.overviewButton.Text = "Обзор";
            this.overviewButton.UseVisualStyleBackColor = true;
            this.overviewButton.Click += new System.EventHandler(this.overviewButton_Click);
            // 
            // importButton
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.importButton.Location = new System.Drawing.Point(146, 406);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(110, 28);
            this.importButton.TabIndex = 2;
            this.importButton.Text = "Импортировать";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(262, 406);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(110, 28);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Выбрать:";
            // 
            // selectedAllObjects
            // 
            this.selectedAllObjects.ActiveLinkColor = System.Drawing.Color.Black;
            this.selectedAllObjects.AutoSize = true;
            this.selectedAllObjects.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.selectedAllObjects.LinkColor = System.Drawing.Color.Black;
            this.selectedAllObjects.Location = new System.Drawing.Point(71, 49);
            this.selectedAllObjects.Name = "selectedAllObjects";
            this.selectedAllObjects.Size = new System.Drawing.Size(26, 13);
            this.selectedAllObjects.TabIndex = 4;
            this.selectedAllObjects.TabStop = true;
            this.selectedAllObjects.Text = "Всё";
            this.selectedAllObjects.VisitedLinkColor = System.Drawing.Color.Black;
            this.selectedAllObjects.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.selectedAllObjects_LinkClicked);
            // 
            // clearSelectedObjects
            // 
            this.clearSelectedObjects.ActiveLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjects.AutoSize = true;
            this.clearSelectedObjects.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.clearSelectedObjects.LinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjects.Location = new System.Drawing.Point(106, 49);
            this.clearSelectedObjects.Name = "clearSelectedObjects";
            this.clearSelectedObjects.Size = new System.Drawing.Size(54, 13);
            this.clearSelectedObjects.TabIndex = 5;
            this.clearSelectedObjects.TabStop = true;
            this.clearSelectedObjects.Text = "Очистить";
            this.clearSelectedObjects.VisitedLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjects.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clearSelectedObjects_LinkClicked);
            // 
            // importingObjectsTree
            // 
            this.importingObjectsTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importingObjectsTree.BackColor = System.Drawing.SystemColors.Window;
            this.importingObjectsTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.importingObjectsTree.DefaultToolTipProvider = null;
            this.importingObjectsTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.importingObjectsTree.FullRowSelect = true;
            this.importingObjectsTree.FullRowSelectActiveColor = System.Drawing.Color.Empty;
            this.importingObjectsTree.FullRowSelectInactiveColor = System.Drawing.Color.Empty;
            this.importingObjectsTree.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.importingObjectsTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.importingObjectsTree.Location = new System.Drawing.Point(12, 66);
            this.importingObjectsTree.Model = null;
            this.importingObjectsTree.Name = "importingObjectsTree";
            this.importingObjectsTree.NodeFilter = null;
            this.importingObjectsTree.SelectedNode = null;
            this.importingObjectsTree.ShowNodeToolTips = true;
            this.importingObjectsTree.Size = new System.Drawing.Size(360, 334);
            this.importingObjectsTree.TabIndex = 100;
            this.importingObjectsTree.Text = "exportingDevices";
            this.importingObjectsTree.UseColumns = true;
            // 
            // TechObjectsImportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(384, 441);
            this.Controls.Add(this.clearSelectedObjects);
            this.Controls.Add(this.selectedAllObjects);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.overviewButton);
            this.Controls.Add(this.filePathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.importingObjectsTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 480);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 480);
            this.Name = "TechObjectsImportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Импорт объектов";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportObjectsForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}