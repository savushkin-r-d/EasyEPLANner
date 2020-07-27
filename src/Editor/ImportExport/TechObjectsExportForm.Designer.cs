namespace EasyEPlanner
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
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.selectAllObjectsLink = new System.Windows.Forms.LinkLabel();
            this.clearSelectedObjectsLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Выберите объекты для экспорта:";
            // 
            // checkedListBox
            // 
            this.checkedListBox.CheckOnClick = true;
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Location = new System.Drawing.Point(12, 40);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(360, 349);
            this.checkedListBox.TabIndex = 3;
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton.Location = new System.Drawing.Point(146, 401);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(110, 28);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Экспортировать";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(262, 401);
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
            this.label2.Location = new System.Drawing.Point(12, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Выбрать:";
            // 
            // selectAllObjectsLink
            // 
            this.selectAllObjectsLink.ActiveLinkColor = System.Drawing.Color.Black;
            this.selectAllObjectsLink.AutoSize = true;
            this.selectAllObjectsLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.selectAllObjectsLink.LinkColor = System.Drawing.Color.Black;
            this.selectAllObjectsLink.Location = new System.Drawing.Point(71, 23);
            this.selectAllObjectsLink.Name = "selectAllObjectsLink";
            this.selectAllObjectsLink.Size = new System.Drawing.Size(26, 13);
            this.selectAllObjectsLink.TabIndex = 4;
            this.selectAllObjectsLink.TabStop = true;
            this.selectAllObjectsLink.Text = "Всё";
            this.selectAllObjectsLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.selectAllObjectsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.selectAllObjects_LinkClicked);
            // 
            // clearSelectedObjectsLink
            // 
            this.clearSelectedObjectsLink.ActiveLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjectsLink.AutoSize = true;
            this.clearSelectedObjectsLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.clearSelectedObjectsLink.LinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjectsLink.Location = new System.Drawing.Point(106, 23);
            this.clearSelectedObjectsLink.Name = "clearSelectedObjectsLink";
            this.clearSelectedObjectsLink.Size = new System.Drawing.Size(54, 13);
            this.clearSelectedObjectsLink.TabIndex = 5;
            this.clearSelectedObjectsLink.TabStop = true;
            this.clearSelectedObjectsLink.Text = "Очистить";
            this.clearSelectedObjectsLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.clearSelectedObjectsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.clearSelectedObjects_LinkClicked);
            // 
            // TechObjectsExportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(384, 441);
            this.Controls.Add(this.clearSelectedObjectsLink);
            this.Controls.Add(this.selectAllObjectsLink);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.checkedListBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 480);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 480);
            this.Name = "TechObjectsExportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Экспорт объектов";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportObjectsForm_FormClosed);
            this.Load += new System.EventHandler(this.ExportObjectsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel selectAllObjectsLink;
        private System.Windows.Forms.LinkLabel clearSelectedObjectsLink;
    }
}