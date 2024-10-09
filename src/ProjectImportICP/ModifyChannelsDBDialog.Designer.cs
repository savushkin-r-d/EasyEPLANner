namespace EasyEPlanner.ProjectImportICP
{
    partial class ModifyChannelsDBDialog
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
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SrcChbasePathTextBox = new System.Windows.Forms.TextBox();
            this.DstChbasePathTextBox = new System.Windows.Forms.TextBox();
            this.SrcChbasePathBttn = new System.Windows.Forms.Button();
            this.ChbasePathBttn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CombineTagCmbBx = new System.Windows.Forms.ComboBox();
            this.FormatTagCmbBx = new System.Windows.Forms.ComboBox();
            this.ModifyBttn = new System.Windows.Forms.Button();
            this.CancelBttn = new System.Windows.Forms.Button();
            this.MainTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 2;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.MainTableLayoutPanel.Controls.Add(this.label3, 0, 4);
            this.MainTableLayoutPanel.Controls.Add(this.label4, 0, 5);
            this.MainTableLayoutPanel.Controls.Add(this.SrcChbasePathTextBox, 0, 1);
            this.MainTableLayoutPanel.Controls.Add(this.DstChbasePathTextBox, 0, 3);
            this.MainTableLayoutPanel.Controls.Add(this.SrcChbasePathBttn, 1, 1);
            this.MainTableLayoutPanel.Controls.Add(this.ChbasePathBttn, 1, 3);
            this.MainTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.label2, 0, 2);
            this.MainTableLayoutPanel.Controls.Add(this.CombineTagCmbBx, 1, 4);
            this.MainTableLayoutPanel.Controls.Add(this.FormatTagCmbBx, 1, 5);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 6;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(294, 190);
            this.MainTableLayoutPanel.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(5, 120);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Группировать теги в один подтип";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(5, 150);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(181, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Формат записи базы каналов ";
            // 
            // SrcChbasePathTextBox
            // 
            this.SrcChbasePathTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.SrcChbasePathTextBox.Location = new System.Drawing.Point(5, 30);
            this.SrcChbasePathTextBox.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.SrcChbasePathTextBox.Name = "SrcChbasePathTextBox";
            this.SrcChbasePathTextBox.Size = new System.Drawing.Size(181, 20);
            this.SrcChbasePathTextBox.TabIndex = 0;
            // 
            // DstChbasePathTextBox
            // 
            this.DstChbasePathTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.DstChbasePathTextBox.Location = new System.Drawing.Point(5, 90);
            this.DstChbasePathTextBox.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.DstChbasePathTextBox.Name = "DstChbasePathTextBox";
            this.DstChbasePathTextBox.Size = new System.Drawing.Size(181, 20);
            this.DstChbasePathTextBox.TabIndex = 1;
            this.DstChbasePathTextBox.Text = "P:\\Monitor\\chbase\\";
            this.DstChbasePathTextBox.TextChanged += new System.EventHandler(this.DstChbasePathTextBox_TextChanged);
            // 
            // SrcChbasePathBttn
            // 
            this.SrcChbasePathBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.SrcChbasePathBttn.Location = new System.Drawing.Point(196, 30);
            this.SrcChbasePathBttn.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.SrcChbasePathBttn.Name = "SrcChbasePathBttn";
            this.SrcChbasePathBttn.Size = new System.Drawing.Size(93, 23);
            this.SrcChbasePathBttn.TabIndex = 2;
            this.SrcChbasePathBttn.Text = "Обзор";
            this.SrcChbasePathBttn.UseVisualStyleBackColor = true;
            this.SrcChbasePathBttn.Click += new System.EventHandler(this.SrcChbasePathBttn_Click);
            // 
            // ChbasePathBttn
            // 
            this.ChbasePathBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.ChbasePathBttn.Location = new System.Drawing.Point(196, 90);
            this.ChbasePathBttn.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.ChbasePathBttn.Name = "ChbasePathBttn";
            this.ChbasePathBttn.Size = new System.Drawing.Size(93, 23);
            this.ChbasePathBttn.TabIndex = 3;
            this.ChbasePathBttn.Text = "Обзор";
            this.ChbasePathBttn.UseVisualStyleBackColor = true;
            this.ChbasePathBttn.Click += new System.EventHandler(this.ChbasePathBttn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.MainTableLayoutPanel.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(5, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Исходная база каналов (ICP-CON):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.MainTableLayoutPanel.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(5, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(284, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Новая база каналов:";
            // 
            // CombineTagCmbBx
            // 
            this.CombineTagCmbBx.Dock = System.Windows.Forms.DockStyle.Top;
            this.CombineTagCmbBx.FormattingEnabled = true;
            this.CombineTagCmbBx.Location = new System.Drawing.Point(196, 120);
            this.CombineTagCmbBx.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.CombineTagCmbBx.Name = "CombineTagCmbBx";
            this.CombineTagCmbBx.Size = new System.Drawing.Size(93, 21);
            this.CombineTagCmbBx.TabIndex = 6;
            // 
            // FormatTagCmbBx
            // 
            this.FormatTagCmbBx.Dock = System.Windows.Forms.DockStyle.Top;
            this.FormatTagCmbBx.FormattingEnabled = true;
            this.FormatTagCmbBx.Location = new System.Drawing.Point(196, 150);
            this.FormatTagCmbBx.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.FormatTagCmbBx.Name = "FormatTagCmbBx";
            this.FormatTagCmbBx.Size = new System.Drawing.Size(93, 21);
            this.FormatTagCmbBx.TabIndex = 7;
            // 
            // ModifyBttn
            // 
            this.ModifyBttn.Location = new System.Drawing.Point(103, 196);
            this.ModifyBttn.Name = "ModifyBttn";
            this.ModifyBttn.Size = new System.Drawing.Size(110, 23);
            this.ModifyBttn.TabIndex = 1;
            this.ModifyBttn.Text = "Модифицировать";
            this.ModifyBttn.UseVisualStyleBackColor = true;
            this.ModifyBttn.Click += new System.EventHandler(this.ModifyBttn_Click);
            // 
            // CancelBttn
            // 
            this.CancelBttn.Location = new System.Drawing.Point(219, 196);
            this.CancelBttn.Name = "CancelBttn";
            this.CancelBttn.Size = new System.Drawing.Size(70, 23);
            this.CancelBttn.TabIndex = 2;
            this.CancelBttn.Text = "Отмена";
            this.CancelBttn.UseVisualStyleBackColor = true;
            this.CancelBttn.Click += new System.EventHandler(this.CancelBttn_Click);
            // 
            // ModifyChannelsDBDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 231);
            this.Controls.Add(this.CancelBttn);
            this.Controls.Add(this.ModifyBttn);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModifyChannelsDBDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Модификация базы каналов";
            this.Load += new System.EventHandler(this.ModifyChannelsDBDialog_Load);
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MainTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.TextBox SrcChbasePathTextBox;
        private System.Windows.Forms.TextBox DstChbasePathTextBox;
        private System.Windows.Forms.Button SrcChbasePathBttn;
        private System.Windows.Forms.Button ChbasePathBttn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CombineTagCmbBx;
        private System.Windows.Forms.ComboBox FormatTagCmbBx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ModifyBttn;
        private System.Windows.Forms.Button CancelBttn;
    }
}