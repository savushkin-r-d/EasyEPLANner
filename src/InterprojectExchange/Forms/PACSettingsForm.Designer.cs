namespace InterprojectExchange
{
    partial class PACSettingsForm
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
            this.projectsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataGroupBox = new System.Windows.Forms.GroupBox();
            this.disableGateBtn = new System.Windows.Forms.RadioButton();
            this.enableGateBtn = new System.Windows.Forms.RadioButton();
            this.disableEmulationBtn = new System.Windows.Forms.RadioButton();
            this.enableEmulationBtn = new System.Windows.Forms.RadioButton();
            this.stationNumberTextBox = new System.Windows.Forms.TextBox();
            this.stationNumberLabel = new System.Windows.Forms.Label();
            this.enableGateLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.timeoutTextBox = new System.Windows.Forms.TextBox();
            this.timeoutLabel = new System.Windows.Forms.Label();
            this.cycletimeTextBox = new System.Windows.Forms.TextBox();
            this.cycleTimeLabel = new System.Windows.Forms.Label();
            this.emulationEnabledLabel = new System.Windows.Forms.Label();
            this.emulatorIPTextBox = new System.Windows.Forms.TextBox();
            this.ipEmulatorLabel = new System.Windows.Forms.Label();
            this.ipAddressTextBox = new System.Windows.Forms.TextBox();
            this.ipAddrLabel = new System.Windows.Forms.Label();
            this.projNameTextBox = new System.Windows.Forms.TextBox();
            this.projectLabel = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.projLabel = new System.Windows.Forms.Label();
            this.dataGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectsListView
            // 
            this.projectsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.projectsListView.FullRowSelect = true;
            this.projectsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.projectsListView.HideSelection = false;
            this.projectsListView.Location = new System.Drawing.Point(12, 36);
            this.projectsListView.MultiSelect = false;
            this.projectsListView.Name = "projectsListView";
            this.projectsListView.ShowGroups = false;
            this.projectsListView.ShowItemToolTips = true;
            this.projectsListView.Size = new System.Drawing.Size(121, 235);
            this.projectsListView.TabIndex = 0;
            this.projectsListView.UseCompatibleStateImageBehavior = false;
            this.projectsListView.View = System.Windows.Forms.View.Details;
            this.projectsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.projectsListView_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 117;
            // 
            // dataGroupBox
            // 
            this.dataGroupBox.Controls.Add(this.disableGateBtn);
            this.dataGroupBox.Controls.Add(this.enableGateBtn);
            this.dataGroupBox.Controls.Add(this.disableEmulationBtn);
            this.dataGroupBox.Controls.Add(this.enableEmulationBtn);
            this.dataGroupBox.Controls.Add(this.stationNumberTextBox);
            this.dataGroupBox.Controls.Add(this.stationNumberLabel);
            this.dataGroupBox.Controls.Add(this.enableGateLabel);
            this.dataGroupBox.Controls.Add(this.portTextBox);
            this.dataGroupBox.Controls.Add(this.portLabel);
            this.dataGroupBox.Controls.Add(this.timeoutTextBox);
            this.dataGroupBox.Controls.Add(this.timeoutLabel);
            this.dataGroupBox.Controls.Add(this.cycletimeTextBox);
            this.dataGroupBox.Controls.Add(this.cycleTimeLabel);
            this.dataGroupBox.Controls.Add(this.emulationEnabledLabel);
            this.dataGroupBox.Controls.Add(this.emulatorIPTextBox);
            this.dataGroupBox.Controls.Add(this.ipEmulatorLabel);
            this.dataGroupBox.Controls.Add(this.ipAddressTextBox);
            this.dataGroupBox.Controls.Add(this.ipAddrLabel);
            this.dataGroupBox.Controls.Add(this.projNameTextBox);
            this.dataGroupBox.Controls.Add(this.projectLabel);
            this.dataGroupBox.Location = new System.Drawing.Point(139, 12);
            this.dataGroupBox.Name = "dataGroupBox";
            this.dataGroupBox.Size = new System.Drawing.Size(350, 259);
            this.dataGroupBox.TabIndex = 1;
            this.dataGroupBox.TabStop = false;
            this.dataGroupBox.Text = "Параметры";
            // 
            // disableGateBtn
            // 
            this.disableGateBtn.AutoCheck = false;
            this.disableGateBtn.AutoSize = true;
            this.disableGateBtn.Location = new System.Drawing.Point(171, 207);
            this.disableGateBtn.Name = "disableGateBtn";
            this.disableGateBtn.Size = new System.Drawing.Size(44, 17);
            this.disableGateBtn.TabIndex = 21;
            this.disableGateBtn.TabStop = true;
            this.disableGateBtn.Text = "Нет";
            this.disableGateBtn.UseVisualStyleBackColor = true;
            this.disableGateBtn.Click += new System.EventHandler(this.disableGateBtn_Click);
            // 
            // enableGateBtn
            // 
            this.enableGateBtn.AutoCheck = false;
            this.enableGateBtn.AutoSize = true;
            this.enableGateBtn.Location = new System.Drawing.Point(125, 207);
            this.enableGateBtn.Name = "enableGateBtn";
            this.enableGateBtn.Size = new System.Drawing.Size(40, 17);
            this.enableGateBtn.TabIndex = 20;
            this.enableGateBtn.TabStop = true;
            this.enableGateBtn.Text = "Да";
            this.enableGateBtn.UseVisualStyleBackColor = true;
            this.enableGateBtn.Click += new System.EventHandler(this.enableGateBtn_Click);
            // 
            // disableEmulationBtn
            // 
            this.disableEmulationBtn.AutoCheck = false;
            this.disableEmulationBtn.AutoSize = true;
            this.disableEmulationBtn.Location = new System.Drawing.Point(171, 103);
            this.disableEmulationBtn.Name = "disableEmulationBtn";
            this.disableEmulationBtn.Size = new System.Drawing.Size(44, 17);
            this.disableEmulationBtn.TabIndex = 19;
            this.disableEmulationBtn.TabStop = true;
            this.disableEmulationBtn.Text = "Нет";
            this.disableEmulationBtn.UseVisualStyleBackColor = true;
            this.disableEmulationBtn.Click += new System.EventHandler(this.disableEmulationBtn_Click);
            // 
            // enableEmulationBtn
            // 
            this.enableEmulationBtn.AutoCheck = false;
            this.enableEmulationBtn.AutoSize = true;
            this.enableEmulationBtn.Location = new System.Drawing.Point(125, 103);
            this.enableEmulationBtn.Name = "enableEmulationBtn";
            this.enableEmulationBtn.Size = new System.Drawing.Size(40, 17);
            this.enableEmulationBtn.TabIndex = 18;
            this.enableEmulationBtn.TabStop = true;
            this.enableEmulationBtn.Text = "Да";
            this.enableEmulationBtn.UseVisualStyleBackColor = true;
            this.enableEmulationBtn.Click += new System.EventHandler(this.enableEmulationBtn_Click);
            // 
            // stationNumberTextBox
            // 
            this.stationNumberTextBox.Location = new System.Drawing.Point(116, 232);
            this.stationNumberTextBox.MaxLength = 3;
            this.stationNumberTextBox.Name = "stationNumberTextBox";
            this.stationNumberTextBox.Size = new System.Drawing.Size(225, 20);
            this.stationNumberTextBox.TabIndex = 17;
            this.stationNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.stationNumberTextBox_KeyPress);
            // 
            // stationNumberLabel
            // 
            this.stationNumberLabel.AutoSize = true;
            this.stationNumberLabel.Location = new System.Drawing.Point(6, 235);
            this.stationNumberLabel.Name = "stationNumberLabel";
            this.stationNumberLabel.Size = new System.Drawing.Size(88, 13);
            this.stationNumberLabel.TabIndex = 16;
            this.stationNumberLabel.Text = "Номер станции:";
            // 
            // enableGateLabel
            // 
            this.enableGateLabel.AutoSize = true;
            this.enableGateLabel.Location = new System.Drawing.Point(6, 209);
            this.enableGateLabel.Name = "enableGateLabel";
            this.enableGateLabel.Size = new System.Drawing.Size(90, 13);
            this.enableGateLabel.TabIndex = 14;
            this.enableGateLabel.Text = "Включить шлюз:";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(116, 180);
            this.portTextBox.MaxLength = 5;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(225, 20);
            this.portTextBox.TabIndex = 13;
            this.portTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.portTextBox_KeyPress);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(6, 183);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(35, 13);
            this.portLabel.TabIndex = 12;
            this.portLabel.Text = "Порт:";
            // 
            // timeoutTextBox
            // 
            this.timeoutTextBox.Location = new System.Drawing.Point(116, 154);
            this.timeoutTextBox.MaxLength = 6;
            this.timeoutTextBox.Name = "timeoutTextBox";
            this.timeoutTextBox.Size = new System.Drawing.Size(225, 20);
            this.timeoutTextBox.TabIndex = 11;
            this.timeoutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.timeoutTextBox_KeyPress);
            // 
            // timeoutLabel
            // 
            this.timeoutLabel.AutoSize = true;
            this.timeoutLabel.Location = new System.Drawing.Point(6, 157);
            this.timeoutLabel.Name = "timeoutLabel";
            this.timeoutLabel.Size = new System.Drawing.Size(73, 13);
            this.timeoutLabel.TabIndex = 10;
            this.timeoutLabel.Text = "Таймаут, мс:";
            // 
            // cycletimeTextBox
            // 
            this.cycletimeTextBox.Location = new System.Drawing.Point(116, 128);
            this.cycletimeTextBox.MaxLength = 6;
            this.cycletimeTextBox.Name = "cycletimeTextBox";
            this.cycletimeTextBox.Size = new System.Drawing.Size(225, 20);
            this.cycletimeTextBox.TabIndex = 9;
            this.cycletimeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cycletimeTextBox_KeyPress);
            // 
            // cycleTimeLabel
            // 
            this.cycleTimeLabel.AutoSize = true;
            this.cycleTimeLabel.Location = new System.Drawing.Point(6, 131);
            this.cycleTimeLabel.Name = "cycleTimeLabel";
            this.cycleTimeLabel.Size = new System.Drawing.Size(96, 13);
            this.cycleTimeLabel.TabIndex = 8;
            this.cycleTimeLabel.Text = "Время цикла, мс:";
            // 
            // emulationEnabledLabel
            // 
            this.emulationEnabledLabel.AutoSize = true;
            this.emulationEnabledLabel.Location = new System.Drawing.Point(6, 105);
            this.emulationEnabledLabel.Name = "emulationEnabledLabel";
            this.emulationEnabledLabel.Size = new System.Drawing.Size(113, 13);
            this.emulationEnabledLabel.TabIndex = 6;
            this.emulationEnabledLabel.Text = "Включить эмуляцию:";
            // 
            // emulatorIPTextBox
            // 
            this.emulatorIPTextBox.Location = new System.Drawing.Point(116, 76);
            this.emulatorIPTextBox.Name = "emulatorIPTextBox";
            this.emulatorIPTextBox.Size = new System.Drawing.Size(225, 20);
            this.emulatorIPTextBox.TabIndex = 5;
            this.emulatorIPTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.emulatorIPTextBox_KeyPress);
            // 
            // ipEmulatorLabel
            // 
            this.ipEmulatorLabel.AutoSize = true;
            this.ipEmulatorLabel.Location = new System.Drawing.Point(6, 79);
            this.ipEmulatorLabel.Name = "ipEmulatorLabel";
            this.ipEmulatorLabel.Size = new System.Drawing.Size(111, 13);
            this.ipEmulatorLabel.TabIndex = 4;
            this.ipEmulatorLabel.Text = "IP-Адрес эмулятора:";
            // 
            // ipAddressTextBox
            // 
            this.ipAddressTextBox.Location = new System.Drawing.Point(116, 50);
            this.ipAddressTextBox.Name = "ipAddressTextBox";
            this.ipAddressTextBox.ReadOnly = true;
            this.ipAddressTextBox.Size = new System.Drawing.Size(225, 20);
            this.ipAddressTextBox.TabIndex = 3;
            // 
            // ipAddrLabel
            // 
            this.ipAddrLabel.AutoSize = true;
            this.ipAddrLabel.Location = new System.Drawing.Point(6, 53);
            this.ipAddrLabel.Name = "ipAddrLabel";
            this.ipAddrLabel.Size = new System.Drawing.Size(54, 13);
            this.ipAddrLabel.TabIndex = 2;
            this.ipAddrLabel.Text = "IP-Адрес:";
            // 
            // projNameTextBox
            // 
            this.projNameTextBox.Location = new System.Drawing.Point(116, 24);
            this.projNameTextBox.Name = "projNameTextBox";
            this.projNameTextBox.ReadOnly = true;
            this.projNameTextBox.Size = new System.Drawing.Size(225, 20);
            this.projNameTextBox.TabIndex = 1;
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(6, 27);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(47, 13);
            this.projectLabel.TabIndex = 0;
            this.projectLabel.Text = "Проект:";
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(414, 277);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "Отмена";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // acceptBtn
            // 
            this.acceptBtn.Location = new System.Drawing.Point(333, 277);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 23);
            this.acceptBtn.TabIndex = 3;
            this.acceptBtn.Text = "Применить";
            this.acceptBtn.UseVisualStyleBackColor = true;
            this.acceptBtn.Click += new System.EventHandler(this.acceptBtn_Click);
            // 
            // projLabel
            // 
            this.projLabel.AutoSize = true;
            this.projLabel.Location = new System.Drawing.Point(12, 17);
            this.projLabel.Name = "projLabel";
            this.projLabel.Size = new System.Drawing.Size(55, 13);
            this.projLabel.TabIndex = 6;
            this.projLabel.Text = "Проекты:";
            // 
            // PACSettingsForm
            // 
            this.AcceptButton = this.acceptBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(503, 305);
            this.ControlBox = false;
            this.Controls.Add(this.projLabel);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.dataGroupBox);
            this.Controls.Add(this.projectsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(519, 344);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(519, 344);
            this.Name = "PACSettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройка параметров PAC";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PACSettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.PACSettingsForm_Load);
            this.dataGroupBox.ResumeLayout(false);
            this.dataGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView projectsListView;
        private System.Windows.Forms.GroupBox dataGroupBox;
        private System.Windows.Forms.TextBox cycletimeTextBox;
        private System.Windows.Forms.Label cycleTimeLabel;
        private System.Windows.Forms.Label emulationEnabledLabel;
        private System.Windows.Forms.TextBox emulatorIPTextBox;
        private System.Windows.Forms.Label ipEmulatorLabel;
        private System.Windows.Forms.TextBox ipAddressTextBox;
        private System.Windows.Forms.Label ipAddrLabel;
        private System.Windows.Forms.TextBox projNameTextBox;
        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.TextBox stationNumberTextBox;
        private System.Windows.Forms.Label stationNumberLabel;
        private System.Windows.Forms.Label enableGateLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox timeoutTextBox;
        private System.Windows.Forms.Label timeoutLabel;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button acceptBtn;
        private System.Windows.Forms.RadioButton disableGateBtn;
        private System.Windows.Forms.RadioButton enableGateBtn;
        private System.Windows.Forms.RadioButton disableEmulationBtn;
        private System.Windows.Forms.RadioButton enableEmulationBtn;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label projLabel;
    }
}