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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("AO", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("AI", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("DO", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("DI", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Остальные", System.Windows.Forms.HorizontalAlignment.Left);
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
            this.mainTLP = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.mainTLP.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // currProjNameTextBox
            // 
            this.currProjNameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.currProjNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currProjNameTextBox.Location = new System.Drawing.Point(123, 3);
            this.currProjNameTextBox.Name = "currProjNameTextBox";
            this.currProjNameTextBox.ReadOnly = true;
            this.currProjNameTextBox.Size = new System.Drawing.Size(237, 20);
            this.currProjNameTextBox.TabIndex = 1;
            // 
            // advProjNameComboBox
            // 
            this.advProjNameComboBox.BackColor = System.Drawing.Color.White;
            this.advProjNameComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advProjNameComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.advProjNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.advProjNameComboBox.FormattingEnabled = true;
            this.advProjNameComboBox.Location = new System.Drawing.Point(123, 3);
            this.advProjNameComboBox.Name = "advProjNameComboBox";
            this.advProjNameComboBox.Size = new System.Drawing.Size(179, 21);
            this.advProjNameComboBox.Sorted = true;
            this.advProjNameComboBox.TabIndex = 5;
            this.advProjNameComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.advProjNameComboBox_DrawItem);
            this.advProjNameComboBox.SelectedValueChanged += new System.EventHandler(this.advProjNameComboBox_SelectedItemChanged);
            // 
            // closeButton
            // 
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.closeButton.Location = new System.Drawing.Point(293, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(69, 24);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Отмена";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveButton.Location = new System.Drawing.Point(218, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(69, 24);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // addAdvProjButton
            // 
            this.addAdvProjButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addAdvProjButton.Location = new System.Drawing.Point(308, 3);
            this.addAdvProjButton.Name = "addAdvProjButton";
            this.addAdvProjButton.Size = new System.Drawing.Size(24, 21);
            this.addAdvProjButton.TabIndex = 8;
            this.addAdvProjButton.Text = "+";
            this.addAdvProjButton.UseVisualStyleBackColor = true;
            this.addAdvProjButton.Click += new System.EventHandler(this.addAdvProjButton_Click);
            // 
            // delAdvProjButton
            // 
            this.delAdvProjButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.delAdvProjButton.Location = new System.Drawing.Point(338, 3);
            this.delAdvProjButton.Name = "delAdvProjButton";
            this.delAdvProjButton.Size = new System.Drawing.Size(24, 21);
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
            this.tableLayoutPanel3.SetColumnSpan(this.advancedProjSignalsList, 2);
            this.advancedProjSignalsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advancedProjSignalsList.FullRowSelect = true;
            this.advancedProjSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.advancedProjSignalsList.HideSelection = false;
            this.advancedProjSignalsList.LabelWrap = false;
            this.advancedProjSignalsList.Location = new System.Drawing.Point(3, 29);
            this.advancedProjSignalsList.MultiSelect = false;
            this.advancedProjSignalsList.Name = "advancedProjSignalsList";
            this.advancedProjSignalsList.ShowGroups = false;
            this.advancedProjSignalsList.ShowItemToolTips = true;
            this.advancedProjSignalsList.Size = new System.Drawing.Size(359, 376);
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
            this.tableLayoutPanel2.SetColumnSpan(this.currentProjSignalsList, 2);
            this.currentProjSignalsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentProjSignalsList.FullRowSelect = true;
            this.currentProjSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.currentProjSignalsList.HideSelection = false;
            this.currentProjSignalsList.LabelWrap = false;
            this.currentProjSignalsList.Location = new System.Drawing.Point(3, 29);
            this.currentProjSignalsList.MultiSelect = false;
            this.currentProjSignalsList.Name = "currentProjSignalsList";
            this.currentProjSignalsList.ShowGroups = false;
            this.currentProjSignalsList.ShowItemToolTips = true;
            this.currentProjSignalsList.Size = new System.Drawing.Size(357, 376);
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
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 18);
            this.label1.TabIndex = 13;
            this.label1.Text = "Связуемый проект:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 18);
            this.label2.TabIndex = 14;
            this.label2.Text = "Открытый проект:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // filterButton
            // 
            this.filterButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.filterButton.Location = new System.Drawing.Point(3, 438);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(85, 24);
            this.filterButton.TabIndex = 15;
            this.filterButton.Text = "Фильтр";
            this.filterButton.UseVisualStyleBackColor = true;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 17);
            this.label3.TabIndex = 16;
            this.label3.Text = "Поиск:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // currProjSearchBox
            // 
            this.currProjSearchBox.BackColor = System.Drawing.SystemColors.Window;
            this.currProjSearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currProjSearchBox.Location = new System.Drawing.Point(123, 3);
            this.currProjSearchBox.Name = "currProjSearchBox";
            this.currProjSearchBox.Size = new System.Drawing.Size(237, 20);
            this.currProjSearchBox.TabIndex = 17;
            this.currProjSearchBox.TextChanged += new System.EventHandler(this.currProjSearchBox_TextChanged);
            // 
            // advProjSearchBox
            // 
            this.advProjSearchBox.BackColor = System.Drawing.SystemColors.Window;
            this.advProjSearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advProjSearchBox.Location = new System.Drawing.Point(123, 3);
            this.advProjSearchBox.Name = "advProjSearchBox";
            this.advProjSearchBox.Size = new System.Drawing.Size(239, 20);
            this.advProjSearchBox.TabIndex = 19;
            this.advProjSearchBox.TextChanged += new System.EventHandler(this.advProjSearchBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Поиск:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bindedSignalsList
            // 
            this.bindedSignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.currentProj,
            this.advProj});
            this.bindedSignalsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bindedSignalsList.FullRowSelect = true;
            listViewGroup1.Header = "AO";
            listViewGroup1.Name = "AO";
            listViewGroup2.Header = "AI";
            listViewGroup2.Name = "AI";
            listViewGroup3.Header = "DO";
            listViewGroup3.Name = "DO";
            listViewGroup4.Header = "DI";
            listViewGroup4.Name = "DI";
            listViewGroup5.Header = "Остальные";
            listViewGroup5.Name = "Other";
            this.bindedSignalsList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5});
            this.bindedSignalsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.bindedSignalsList.HideSelection = false;
            this.bindedSignalsList.Location = new System.Drawing.Point(366, 30);
            this.bindedSignalsList.MultiSelect = false;
            this.bindedSignalsList.Name = "bindedSignalsList";
            this.bindedSignalsList.ShowGroups = false;
            this.bindedSignalsList.ShowItemToolTips = true;
            this.bindedSignalsList.Size = new System.Drawing.Size(357, 402);
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
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 18);
            this.label5.TabIndex = 101;
            this.label5.Text = "Режим:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // modeComboBox
            // 
            this.modeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "Источник >> Приемник",
            "Приемник >> Источник"});
            this.modeComboBox.Location = new System.Drawing.Point(63, 3);
            this.modeComboBox.MaxDropDownItems = 2;
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(154, 21);
            this.modeComboBox.TabIndex = 103;
            this.modeComboBox.SelectedValueChanged += new System.EventHandler(this.modeComboBox_SelectedValueChanged);
            // 
            // pacSetUpBtn
            // 
            this.pacSetUpBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pacSetUpBtn.Location = new System.Drawing.Point(246, 3);
            this.pacSetUpBtn.Name = "pacSetUpBtn";
            this.pacSetUpBtn.Size = new System.Drawing.Size(114, 21);
            this.pacSetUpBtn.TabIndex = 104;
            this.pacSetUpBtn.Text = "Настройка PAC";
            this.pacSetUpBtn.UseVisualStyleBackColor = true;
            this.pacSetUpBtn.Click += new System.EventHandler(this.pacSetUpBtn_Click);
            // 
            // mainTLP
            // 
            this.mainTLP.ColumnCount = 3;
            this.mainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.mainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.mainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.mainTLP.Controls.Add(this.tableLayoutPanel1, 2, 2);
            this.mainTLP.Controls.Add(this.bindedSignalsList, 1, 1);
            this.mainTLP.Controls.Add(this.filterButton, 0, 2);
            this.mainTLP.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.mainTLP.Controls.Add(this.tableLayoutPanel3, 2, 1);
            this.mainTLP.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.mainTLP.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.mainTLP.Controls.Add(this.tableLayoutPanel6, 2, 0);
            this.mainTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTLP.Location = new System.Drawing.Point(0, 0);
            this.mainTLP.Name = "mainTLP";
            this.mainTLP.RowCount = 3;
            this.mainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.mainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.mainTLP.Size = new System.Drawing.Size(1091, 465);
            this.mainTLP.TabIndex = 105;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.saveButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.closeButton, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(726, 435);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(365, 30);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.currProjSearchBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.currentProjSignalsList, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 27);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(363, 408);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.advProjSearchBox, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.advancedProjSignalsList, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(726, 27);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(365, 408);
            this.tableLayoutPanel3.TabIndex = 17;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.pacSetUpBtn, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.modeComboBox, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(363, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(363, 27);
            this.tableLayoutPanel4.TabIndex = 18;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.currProjNameTextBox, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(363, 27);
            this.tableLayoutPanel5.TabIndex = 19;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 4;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.advProjNameComboBox, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.addAdvProjButton, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.delAdvProjButton, 3, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(726, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(365, 27);
            this.tableLayoutPanel6.TabIndex = 20;
            // 
            // InterprojectExchangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 465);
            this.Controls.Add(this.mainTLP);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1107, 504);
            this.Name = "InterprojectExchangeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка межконтроллерного обмена сигналами";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InterprojectExchangeForm_FormClosed);
            this.Load += new System.EventHandler(this.InterprojectExchangeForm_Load);
            this.mainTLP.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.TableLayoutPanel mainTLP;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    }
}