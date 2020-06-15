namespace EasyEPlanner
{
    partial class FilterForm
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
            this.currProjGoupBox = new System.Windows.Forms.GroupBox();
            this.currDevLabel = new System.Windows.Forms.Label();
            this.currProjDevList = new System.Windows.Forms.CheckedListBox();
            this.bindedGridGroupBox = new System.Windows.Forms.GroupBox();
            this.groupAsPairsCheckBox = new System.Windows.Forms.CheckBox();
            this.advProjGroupBox = new System.Windows.Forms.GroupBox();
            this.advDevLabel = new System.Windows.Forms.Label();
            this.advProjDevList = new System.Windows.Forms.CheckedListBox();
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.currProjGoupBox.SuspendLayout();
            this.bindedGridGroupBox.SuspendLayout();
            this.advProjGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // currProjGoupBox
            // 
            this.currProjGoupBox.Controls.Add(this.currDevLabel);
            this.currProjGoupBox.Controls.Add(this.currProjDevList);
            this.currProjGoupBox.Location = new System.Drawing.Point(12, 12);
            this.currProjGoupBox.Name = "currProjGoupBox";
            this.currProjGoupBox.Size = new System.Drawing.Size(223, 375);
            this.currProjGoupBox.TabIndex = 0;
            this.currProjGoupBox.TabStop = false;
            this.currProjGoupBox.Text = "Текущий проект";
            // 
            // currDevLabel
            // 
            this.currDevLabel.AutoSize = true;
            this.currDevLabel.Location = new System.Drawing.Point(6, 16);
            this.currDevLabel.Name = "currDevLabel";
            this.currDevLabel.Size = new System.Drawing.Size(132, 13);
            this.currDevLabel.TabIndex = 6;
            this.currDevLabel.Text = "Отображать устройства:";
            // 
            // currProjDevList
            // 
            this.currProjDevList.FormattingEnabled = true;
            this.currProjDevList.Location = new System.Drawing.Point(6, 34);
            this.currProjDevList.Name = "currProjDevList";
            this.currProjDevList.Size = new System.Drawing.Size(211, 334);
            this.currProjDevList.TabIndex = 0;
            // 
            // bindedGridGroupBox
            // 
            this.bindedGridGroupBox.Controls.Add(this.groupAsPairsCheckBox);
            this.bindedGridGroupBox.Location = new System.Drawing.Point(241, 12);
            this.bindedGridGroupBox.Name = "bindedGridGroupBox";
            this.bindedGridGroupBox.Size = new System.Drawing.Size(223, 375);
            this.bindedGridGroupBox.TabIndex = 1;
            this.bindedGridGroupBox.TabStop = false;
            this.bindedGridGroupBox.Text = "Сводная таблица";
            // 
            // groupAsPairsCheckBox
            // 
            this.groupAsPairsCheckBox.AutoSize = true;
            this.groupAsPairsCheckBox.Location = new System.Drawing.Point(23, 36);
            this.groupAsPairsCheckBox.Name = "groupAsPairsCheckBox";
            this.groupAsPairsCheckBox.Size = new System.Drawing.Size(141, 17);
            this.groupAsPairsCheckBox.TabIndex = 5;
            this.groupAsPairsCheckBox.Text = "Группировка по парам";
            this.groupAsPairsCheckBox.UseVisualStyleBackColor = true;
            // 
            // advProjGroupBox
            // 
            this.advProjGroupBox.Controls.Add(this.advDevLabel);
            this.advProjGroupBox.Controls.Add(this.advProjDevList);
            this.advProjGroupBox.Location = new System.Drawing.Point(470, 12);
            this.advProjGroupBox.Name = "advProjGroupBox";
            this.advProjGroupBox.Size = new System.Drawing.Size(223, 375);
            this.advProjGroupBox.TabIndex = 2;
            this.advProjGroupBox.TabStop = false;
            this.advProjGroupBox.Text = "Связуемый проект";
            // 
            // advDevLabel
            // 
            this.advDevLabel.AutoSize = true;
            this.advDevLabel.Location = new System.Drawing.Point(10, 16);
            this.advDevLabel.Name = "advDevLabel";
            this.advDevLabel.Size = new System.Drawing.Size(132, 13);
            this.advDevLabel.TabIndex = 7;
            this.advDevLabel.Text = "Отображать устройства:";
            // 
            // advProjDevList
            // 
            this.advProjDevList.FormattingEnabled = true;
            this.advProjDevList.Location = new System.Drawing.Point(6, 34);
            this.advProjDevList.Name = "advProjDevList";
            this.advProjDevList.Size = new System.Drawing.Size(211, 334);
            this.advProjDevList.TabIndex = 1;
            // 
            // acceptButton
            // 
            this.acceptButton.Location = new System.Drawing.Point(537, 391);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(75, 23);
            this.acceptButton.TabIndex = 3;
            this.acceptButton.Text = "Применить";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(618, 391);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 421);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.advProjGroupBox);
            this.Controls.Add(this.bindedGridGroupBox);
            this.Controls.Add(this.currProjGoupBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(721, 460);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(721, 460);
            this.Name = "FilterForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка фильтрации сигналов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FilterForm_FormClosing);
            this.Load += new System.EventHandler(this.FilterForm_Load);
            this.currProjGoupBox.ResumeLayout(false);
            this.currProjGoupBox.PerformLayout();
            this.bindedGridGroupBox.ResumeLayout(false);
            this.bindedGridGroupBox.PerformLayout();
            this.advProjGroupBox.ResumeLayout(false);
            this.advProjGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox currProjGoupBox;
        private System.Windows.Forms.GroupBox bindedGridGroupBox;
        private System.Windows.Forms.GroupBox advProjGroupBox;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckedListBox currProjDevList;
        private System.Windows.Forms.CheckBox groupAsPairsCheckBox;
        private System.Windows.Forms.CheckedListBox advProjDevList;
        private System.Windows.Forms.Label currDevLabel;
        private System.Windows.Forms.Label advDevLabel;
    }
}