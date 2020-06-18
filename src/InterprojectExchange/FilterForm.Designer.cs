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
            this.bindedSignalsList = new System.Windows.Forms.GroupBox();
            this.groupAsPairsCheckBox = new System.Windows.Forms.CheckBox();
            this.advProjGroupBox = new System.Windows.Forms.GroupBox();
            this.advDevLabel = new System.Windows.Forms.Label();
            this.advProjDevList = new System.Windows.Forms.CheckedListBox();
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.currProjGoupBox.SuspendLayout();
            this.bindedSignalsList.SuspendLayout();
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
            this.currProjDevList.CheckOnClick = true;
            this.currProjDevList.FormattingEnabled = true;
            this.currProjDevList.Location = new System.Drawing.Point(6, 34);
            this.currProjDevList.Name = "currProjDevList";
            this.currProjDevList.Size = new System.Drawing.Size(211, 334);
            this.currProjDevList.TabIndex = 0;
            this.currProjDevList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.currProjDevList_ItemCheck);
            // 
            // bindedSignalsList
            // 
            this.bindedSignalsList.Controls.Add(this.groupAsPairsCheckBox);
            this.bindedSignalsList.Location = new System.Drawing.Point(241, 12);
            this.bindedSignalsList.Name = "bindedSignalsList";
            this.bindedSignalsList.Size = new System.Drawing.Size(223, 375);
            this.bindedSignalsList.TabIndex = 1;
            this.bindedSignalsList.TabStop = false;
            this.bindedSignalsList.Text = "Сводная таблица";
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
            this.groupAsPairsCheckBox.CheckStateChanged += new System.EventHandler(this.groupAsPairsCheckBox_CheckStateChanged);
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
            this.advProjDevList.CheckOnClick = true;
            this.advProjDevList.FormattingEnabled = true;
            this.advProjDevList.Location = new System.Drawing.Point(6, 34);
            this.advProjDevList.Name = "advProjDevList";
            this.advProjDevList.Size = new System.Drawing.Size(211, 334);
            this.advProjDevList.TabIndex = 1;
            this.advProjDevList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.advProjDevList_ItemCheck);
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
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(12, 391);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 6;
            this.clearButton.Text = "Очистить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 421);
            this.ControlBox = false;
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.advProjGroupBox);
            this.Controls.Add(this.bindedSignalsList);
            this.Controls.Add(this.currProjGoupBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(721, 460);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(721, 460);
            this.Name = "FilterForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка фильтрации сигналов";
            this.TopMost = true;
            this.currProjGoupBox.ResumeLayout(false);
            this.currProjGoupBox.PerformLayout();
            this.bindedSignalsList.ResumeLayout(false);
            this.bindedSignalsList.PerformLayout();
            this.advProjGroupBox.ResumeLayout(false);
            this.advProjGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox currProjGoupBox;
        private System.Windows.Forms.GroupBox advProjGroupBox;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label currDevLabel;
        private System.Windows.Forms.Label advDevLabel;
        private System.Windows.Forms.Button clearButton;
        public System.Windows.Forms.GroupBox bindedSignalsList;
        private System.Windows.Forms.CheckedListBox currProjDevList;
        private System.Windows.Forms.CheckBox groupAsPairsCheckBox;
        private System.Windows.Forms.CheckedListBox advProjDevList;
    }
}