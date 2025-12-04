namespace EasyEPlanner.mpk.View
{
    partial class MpkSaverForm
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
            this.NewContainerBttn = new System.Windows.Forms.RadioButton();
            this.UpdateContainerBttn = new System.Windows.Forms.RadioButton();
            this.MainContainerNameTextBox = new System.Windows.Forms.TextBox();
            this.MpkDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PathReviewBttn = new System.Windows.Forms.Button();
            this.ExportBttn = new System.Windows.Forms.Button();
            this.CancelBttn = new System.Windows.Forms.Button();
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.MainTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NewContainerBttn
            // 
            this.NewContainerBttn.AutoSize = true;
            this.MainTableLayoutPanel.SetColumnSpan(this.NewContainerBttn, 3);
            this.NewContainerBttn.Location = new System.Drawing.Point(30, 143);
            this.NewContainerBttn.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.NewContainerBttn.Name = "NewContainerBttn";
            this.NewContainerBttn.Size = new System.Drawing.Size(238, 17);
            this.NewContainerBttn.TabIndex = 0;
            this.NewContainerBttn.TabStop = true;
            this.NewContainerBttn.Text = "Создать новый контейнер (перезаписать)";
            this.NewContainerBttn.UseVisualStyleBackColor = true;
            // 
            // UpdateContainerBttn
            // 
            this.UpdateContainerBttn.AutoSize = true;
            this.MainTableLayoutPanel.SetColumnSpan(this.UpdateContainerBttn, 3);
            this.UpdateContainerBttn.Enabled = false;
            this.UpdateContainerBttn.Location = new System.Drawing.Point(30, 173);
            this.UpdateContainerBttn.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.UpdateContainerBttn.Name = "UpdateContainerBttn";
            this.UpdateContainerBttn.Size = new System.Drawing.Size(210, 17);
            this.UpdateContainerBttn.TabIndex = 1;
            this.UpdateContainerBttn.TabStop = true;
            this.UpdateContainerBttn.Text = "Обновить существующий контейнер";
            this.UpdateContainerBttn.UseVisualStyleBackColor = true;
            // 
            // MainContainerNameTextBox
            // 
            this.MainTableLayoutPanel.SetColumnSpan(this.MainContainerNameTextBox, 2);
            this.MainContainerNameTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MainContainerNameTextBox.Location = new System.Drawing.Point(5, 97);
            this.MainContainerNameTextBox.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.MainContainerNameTextBox.Name = "MainContainerNameTextBox";
            this.MainContainerNameTextBox.Size = new System.Drawing.Size(296, 20);
            this.MainContainerNameTextBox.TabIndex = 2;
            // 
            // MpkDirectoryTextBox
            // 
            this.MainTableLayoutPanel.SetColumnSpan(this.MpkDirectoryTextBox, 2);
            this.MpkDirectoryTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MpkDirectoryTextBox.Location = new System.Drawing.Point(5, 37);
            this.MpkDirectoryTextBox.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.MpkDirectoryTextBox.Name = "MpkDirectoryTextBox";
            this.MpkDirectoryTextBox.Size = new System.Drawing.Size(296, 20);
            this.MpkDirectoryTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.MainTableLayoutPanel.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Каталог для сохранения";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.MainTableLayoutPanel.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(3, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(298, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Название контейнера";
            // 
            // PathReviewBttn
            // 
            this.PathReviewBttn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PathReviewBttn.Location = new System.Drawing.Point(307, 35);
            this.PathReviewBttn.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.PathReviewBttn.Name = "PathReviewBttn";
            this.PathReviewBttn.Size = new System.Drawing.Size(94, 23);
            this.PathReviewBttn.TabIndex = 6;
            this.PathReviewBttn.Text = "Обзор";
            this.PathReviewBttn.UseVisualStyleBackColor = true;
            this.PathReviewBttn.Click += new System.EventHandler(this.PathReviewBttn_Click);
            // 
            // ExportBttn
            // 
            this.ExportBttn.Dock = System.Windows.Forms.DockStyle.Right;
            this.ExportBttn.Location = new System.Drawing.Point(191, 224);
            this.ExportBttn.Name = "ExportBttn";
            this.ExportBttn.Size = new System.Drawing.Size(110, 24);
            this.ExportBttn.TabIndex = 7;
            this.ExportBttn.Text = "Экспортировать";
            this.ExportBttn.UseVisualStyleBackColor = true;
            this.ExportBttn.Click += new System.EventHandler(this.ExportBttn_Click);
            // 
            // CancelBttn
            // 
            this.CancelBttn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.CancelBttn.Location = new System.Drawing.Point(307, 225);
            this.CancelBttn.Name = "CancelBttn";
            this.CancelBttn.Size = new System.Drawing.Size(94, 23);
            this.CancelBttn.TabIndex = 8;
            this.CancelBttn.Text = "Отменить";
            this.CancelBttn.UseVisualStyleBackColor = true;
            this.CancelBttn.Click += new System.EventHandler(this.CancelBttn_Click);
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.MainTableLayoutPanel.Controls.Add(this.ExportBttn, 1, 7);
            this.MainTableLayoutPanel.Controls.Add(this.UpdateContainerBttn, 0, 6);
            this.MainTableLayoutPanel.Controls.Add(this.MainContainerNameTextBox, 0, 3);
            this.MainTableLayoutPanel.Controls.Add(this.label2, 0, 2);
            this.MainTableLayoutPanel.Controls.Add(this.PathReviewBttn, 2, 1);
            this.MainTableLayoutPanel.Controls.Add(this.CancelBttn, 2, 7);
            this.MainTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.MpkDirectoryTextBox, 0, 1);
            this.MainTableLayoutPanel.Controls.Add(this.NewContainerBttn, 0, 5);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(5);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 8;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(404, 251);
            this.MainTableLayoutPanel.TabIndex = 9;
            // 
            // MpkSaverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 251);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.MinimumSize = new System.Drawing.Size(420, 290);
            this.Name = "MpkSaverForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Экспорт mpk";
            this.Load += new System.EventHandler(this.MpkSaverForm_Load);
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MainTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton NewContainerBttn;
        private System.Windows.Forms.RadioButton UpdateContainerBttn;
        private System.Windows.Forms.TextBox MainContainerNameTextBox;
        private System.Windows.Forms.TextBox MpkDirectoryTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button PathReviewBttn;
        private System.Windows.Forms.Button ExportBttn;
        private System.Windows.Forms.Button CancelBttn;
        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
    }
}