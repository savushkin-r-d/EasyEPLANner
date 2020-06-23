namespace InterprojectExchange
{
    partial class InterprojectExchangeForm
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
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("AO", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("AI", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup13 = new System.Windows.Forms.ListViewGroup("DO", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup14 = new System.Windows.Forms.ListViewGroup("DI", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup15 = new System.Windows.Forms.ListViewGroup("Остальные", System.Windows.Forms.HorizontalAlignment.Left);
            this.currProjNameTextBox = new System.Windows.Forms.TextBox();
            this.advProjNameComboBox = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.addAdvProjButton = new System.Windows.Forms.Button();
            this.delAdvProjButton = new System.Windows.Forms.Button();
            this.advancedProjSignalsList = new System.Windows.Forms.ListView();
            this.AdvSignal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AdvDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.currentProjSignalsList = new System.Windows.Forms.ListView();
            this.CurrDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CurrSignal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.filterButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.currProjSearchBox = new System.Windows.Forms.TextBox();
            this.advProjSearchBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bindedSignalsList = new System.Windows.Forms.ListView();
            this.currentProj = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.advProj = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.pacSetUpBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // currProjNameTextBox
            // 
            this.currProjNameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.currProjNameTextBox.Location = new System.Drawing.Point(111, 12);
            this.currProjNameTextBox.Name = "currProjNameTextBox";
            this.currProjNameTextBox.ReadOnly = true;
            this.currProjNameTextBox.Size = new System.Drawing.Size(251, 20);
            this.currProjNameTextBox.TabIndex = 1;
            // 
            // advProjNameComboBox
            // 
            this.advProjNameComboBox.BackColor = System.Drawing.Color.White;
            this.advProjNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.advProjNameComboBox.FormattingEnabled = true;
            this.advProjNameComboBox.Location = new System.Drawing.Point(835, 11);
            this.advProjNameComboBox.Name = "advProjNameComboBox";
            this.advProjNameComboBox.Size = new System.Drawing.Size(194, 21);
            this.advProjNameComboBox.TabIndex = 5;
            this.advProjNameComboBox.SelectedValueChanged += new System.EventHandler(this.advProjNameComboBox_SelectedItemChanged);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(1004, 435);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Отмена";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(923, 435);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // addAdvProjButton
            // 
            this.addAdvProjButton.Location = new System.Drawing.Point(1035, 10);
            this.addAdvProjButton.Name = "addAdvProjButton";
            this.addAdvProjButton.Size = new System.Drawing.Size(19, 23);
            this.addAdvProjButton.TabIndex = 8;
            this.addAdvProjButton.Text = "+";
            this.addAdvProjButton.UseVisualStyleBackColor = true;
            this.addAdvProjButton.Click += new System.EventHandler(this.addAdvProjButton_Click);
            // 
            // delAdvProjButton
            // 
            this.delAdvProjButton.Location = new System.Drawing.Point(1060, 10);
            this.delAdvProjButton.Name = "delAdvProjButton";
            this.delAdvProjButton.Size = new System.Drawing.Size(19, 23);
            this.delAdvProjButton.TabIndex = 9;
            this.delAdvProjButton.Text = "-";
            this.delAdvProjButton.UseVisualStyleBackColor = true;
            this.delAdvProjButton.Click += new System.EventHandler(this.delAdvProjButton_Click);
            // 
            // advancedProjSignalsList
            // 
            this.advancedProjSignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AdvSignal,
            this.AdvDescription});
            this.advancedProjSignalsList.FullRowSelect = true;
            this.advancedProjSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.advancedProjSignalsList.HideSelection = false;
            this.advancedProjSignalsList.LabelWrap = false;
            this.advancedProjSignalsList.Location = new System.Drawing.Point(729, 60);
            this.advancedProjSignalsList.MultiSelect = false;
            this.advancedProjSignalsList.Name = "advancedProjSignalsList";
            this.advancedProjSignalsList.ShowGroups = false;
            this.advancedProjSignalsList.ShowItemToolTips = true;
            this.advancedProjSignalsList.Size = new System.Drawing.Size(350, 369);
            this.advancedProjSignalsList.TabIndex = 11;
            this.advancedProjSignalsList.UseCompatibleStateImageBehavior = false;
            this.advancedProjSignalsList.View = System.Windows.Forms.View.Details;
            this.advancedProjSignalsList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.advancedProjSignalsList_ItemSelectionChanged);
            this.advancedProjSignalsList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.advancedProjSignalsList_KeyPress);
            // 
            // AdvSignal
            // 
            this.AdvSignal.Text = "Сигнал";
            this.AdvSignal.Width = 155;
            // 
            // AdvDescription
            // 
            this.AdvDescription.Text = "Описание";
            this.AdvDescription.Width = 165;
            // 
            // currentProjSignalsList
            // 
            this.currentProjSignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CurrDescription,
            this.CurrSignal});
            this.currentProjSignalsList.FullRowSelect = true;
            this.currentProjSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.currentProjSignalsList.HideSelection = false;
            this.currentProjSignalsList.LabelWrap = false;
            this.currentProjSignalsList.Location = new System.Drawing.Point(12, 60);
            this.currentProjSignalsList.MultiSelect = false;
            this.currentProjSignalsList.Name = "currentProjSignalsList";
            this.currentProjSignalsList.ShowGroups = false;
            this.currentProjSignalsList.ShowItemToolTips = true;
            this.currentProjSignalsList.Size = new System.Drawing.Size(350, 369);
            this.currentProjSignalsList.TabIndex = 12;
            this.currentProjSignalsList.UseCompatibleStateImageBehavior = false;
            this.currentProjSignalsList.View = System.Windows.Forms.View.Details;
            this.currentProjSignalsList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.currentProjSignalsList_ItemSelectionChanged);
            this.currentProjSignalsList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.currentProjSignalsList_KeyPress);
            // 
            // CurrDescription
            // 
            this.CurrDescription.Text = "Описание";
            this.CurrDescription.Width = 155;
            // 
            // CurrSignal
            // 
            this.CurrSignal.Text = "Сигнал";
            this.CurrSignal.Width = 165;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(726, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Связуемый проект:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Текущий проект:";
            // 
            // filterButton
            // 
            this.filterButton.Location = new System.Drawing.Point(12, 435);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(85, 23);
            this.filterButton.TabIndex = 15;
            this.filterButton.Text = "Фильтр";
            this.filterButton.UseVisualStyleBackColor = true;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Поиск:";
            // 
            // currProjSearchBox
            // 
            this.currProjSearchBox.BackColor = System.Drawing.SystemColors.Window;
            this.currProjSearchBox.Location = new System.Drawing.Point(111, 37);
            this.currProjSearchBox.Name = "currProjSearchBox";
            this.currProjSearchBox.Size = new System.Drawing.Size(251, 20);
            this.currProjSearchBox.TabIndex = 17;
            this.currProjSearchBox.TextChanged += new System.EventHandler(this.currProjSearchBox_TextChanged);
            // 
            // advProjSearchBox
            // 
            this.advProjSearchBox.BackColor = System.Drawing.SystemColors.Window;
            this.advProjSearchBox.Location = new System.Drawing.Point(828, 37);
            this.advProjSearchBox.Name = "advProjSearchBox";
            this.advProjSearchBox.Size = new System.Drawing.Size(251, 20);
            this.advProjSearchBox.TabIndex = 19;
            this.advProjSearchBox.TextChanged += new System.EventHandler(this.advProjSearchBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(726, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Поиск:";
            // 
            // bindedSignalsList
            // 
            this.bindedSignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.currentProj,
            this.advProj});
            this.bindedSignalsList.FullRowSelect = true;
            listViewGroup11.Header = "AO";
            listViewGroup11.Name = "AO";
            listViewGroup12.Header = "AI";
            listViewGroup12.Name = "AI";
            listViewGroup13.Header = "DO";
            listViewGroup13.Name = "DO";
            listViewGroup14.Header = "DI";
            listViewGroup14.Name = "DI";
            listViewGroup15.Header = "Остальные";
            listViewGroup15.Name = "Other";
            this.bindedSignalsList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup11,
            listViewGroup12,
            listViewGroup13,
            listViewGroup14,
            listViewGroup15});
            this.bindedSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.bindedSignalsList.HideSelection = false;
            this.bindedSignalsList.Location = new System.Drawing.Point(369, 38);
            this.bindedSignalsList.MultiSelect = false;
            this.bindedSignalsList.Name = "bindedSignalsList";
            this.bindedSignalsList.ShowGroups = false;
            this.bindedSignalsList.ShowItemToolTips = true;
            this.bindedSignalsList.Size = new System.Drawing.Size(354, 391);
            this.bindedSignalsList.TabIndex = 100;
            this.bindedSignalsList.UseCompatibleStateImageBehavior = false;
            this.bindedSignalsList.View = System.Windows.Forms.View.Details;
            this.bindedSignalsList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.bindedSignalsList_ItemSelectionChanged);
            this.bindedSignalsList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bindedSignalsList_KeyDown);
            this.bindedSignalsList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.bindedSignalsList_MouseClick);
            this.bindedSignalsList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bindedSignalsList_MouseDown);
            // 
            // currentProj
            // 
            this.currentProj.Text = "Текущий проект";
            this.currentProj.Width = 160;
            // 
            // advProj
            // 
            this.advProj.Text = "Связуемый проект";
            this.advProj.Width = 160;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(369, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 101;
            this.label5.Text = "Режим:";
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "Источник >> Приемник",
            "Приемник >> Источник"});
            this.modeComboBox.Location = new System.Drawing.Point(419, 12);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(148, 21);
            this.modeComboBox.TabIndex = 103;
            this.modeComboBox.SelectedValueChanged += new System.EventHandler(this.modeComboBox_SelectedValueChanged);
            // 
            // pacSetUpBtn
            // 
            this.pacSetUpBtn.Location = new System.Drawing.Point(615, 11);
            this.pacSetUpBtn.Name = "pacSetUpBtn";
            this.pacSetUpBtn.Size = new System.Drawing.Size(105, 22);
            this.pacSetUpBtn.TabIndex = 104;
            this.pacSetUpBtn.Text = "Настройка PAC";
            this.pacSetUpBtn.UseVisualStyleBackColor = true;
            this.pacSetUpBtn.Click += new System.EventHandler(this.pacSetUpBtn_Click);
            // 
            // InterprojectExchangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 465);
            this.Controls.Add(this.pacSetUpBtn);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.bindedSignalsList);
            this.Controls.Add(this.advProjSearchBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.currProjSearchBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.filterButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.currentProjSignalsList);
            this.Controls.Add(this.advancedProjSignalsList);
            this.Controls.Add(this.delAdvProjButton);
            this.Controls.Add(this.addAdvProjButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.advProjNameComboBox);
            this.Controls.Add(this.currProjNameTextBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1107, 504);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1107, 504);
            this.Name = "InterprojectExchangeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка межконтроллерного обмена сигналами";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InterprojectExchangeForm_FormClosed);
            this.Load += new System.EventHandler(this.InterprojectExchangeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox currProjNameTextBox;
        private System.Windows.Forms.ComboBox advProjNameComboBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button addAdvProjButton;
        private System.Windows.Forms.Button delAdvProjButton;
        private System.Windows.Forms.ListView advancedProjSignalsList;
        private System.Windows.Forms.ColumnHeader AdvSignal;
        private System.Windows.Forms.ColumnHeader AdvDescription;
        private System.Windows.Forms.ListView currentProjSignalsList;
        private System.Windows.Forms.ColumnHeader CurrSignal;
        private System.Windows.Forms.ColumnHeader CurrDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox currProjSearchBox;
        private System.Windows.Forms.TextBox advProjSearchBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView bindedSignalsList;
        private System.Windows.Forms.ColumnHeader currentProj;
        private System.Windows.Forms.ColumnHeader advProj;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Button pacSetUpBtn;
    }
}