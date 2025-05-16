namespace EasyEPlanner.ModbusExchange.View
{
    partial class ModbusExchangeView
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.SignalsList = new System.Windows.Forms.ListView();
            this.SignalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SignalDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Exchange = new BrightIdeasSoftware.TreeListView();
            this.ImportGatewayStructBttn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SignalsSerch = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.AddGatewayBttn = new System.Windows.Forms.Button();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.GatewaySelectionCB = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.SaveBttn = new System.Windows.Forms.Button();
            this.CancelBttn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Exchange)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 661);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.tableLayoutPanel2.Controls.Add(this.SignalsList, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.Exchange, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.ImportGatewayStructBttn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.SignalsSerch, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 93);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(598, 525);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // SignalsList
            // 
            this.SignalsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SignalName,
            this.SignalDescription});
            this.tableLayoutPanel2.SetColumnSpan(this.SignalsList, 2);
            this.SignalsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignalsList.FullRowSelect = true;
            this.SignalsList.HideSelection = false;
            this.SignalsList.Location = new System.Drawing.Point(3, 33);
            this.SignalsList.Name = "SignalsList";
            this.SignalsList.Size = new System.Drawing.Size(285, 489);
            this.SignalsList.TabIndex = 0;
            this.SignalsList.UseCompatibleStateImageBehavior = false;
            this.SignalsList.View = System.Windows.Forms.View.Details;
            this.SignalsList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.SignalsList_ItemDrag);
            this.SignalsList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.Exchange_ItemSelectionChanged);
            // 
            // SignalName
            // 
            this.SignalName.Text = "Название";
            this.SignalName.Width = 72;
            // 
            // SignalDescription
            // 
            this.SignalDescription.Text = "Описание";
            this.SignalDescription.Width = 172;
            // 
            // Exchange
            // 
            this.Exchange.AllowDrop = true;
            this.Exchange.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.Exchange.CellEditUseWholeCell = false;
            this.tableLayoutPanel2.SetColumnSpan(this.Exchange, 2);
            this.Exchange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Exchange.FullRowSelect = true;
            this.Exchange.HideSelection = false;
            this.Exchange.Location = new System.Drawing.Point(294, 33);
            this.Exchange.Name = "Exchange";
            this.Exchange.ShowGroups = false;
            this.Exchange.Size = new System.Drawing.Size(301, 489);
            this.Exchange.TabIndex = 1;
            this.Exchange.UseCompatibleStateImageBehavior = false;
            this.Exchange.View = System.Windows.Forms.View.Details;
            this.Exchange.VirtualMode = true;
            this.Exchange.Expanded += new System.EventHandler<BrightIdeasSoftware.TreeBranchExpandedEventArgs>(this.Exchange_Expanded);
            this.Exchange.Collapsed += new System.EventHandler<BrightIdeasSoftware.TreeBranchCollapsedEventArgs>(this.Exchange_Collapsed);
            this.Exchange.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.Exchange_CellEditFinishing);
            this.Exchange.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.Exchange_CellEditStarting);
            this.Exchange.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.Exchange_ItemSelectionChanged);
            this.Exchange.DragDrop += new System.Windows.Forms.DragEventHandler(this.Exchange_DragDrop);
            this.Exchange.DragOver += new System.Windows.Forms.DragEventHandler(this.Exchange_DragOver);
            this.Exchange.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Exchange_KeyDown);
            // 
            // ImportGatewayStructBttn
            // 
            this.ImportGatewayStructBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.ImportGatewayStructBttn.Location = new System.Drawing.Point(448, 3);
            this.ImportGatewayStructBttn.Name = "ImportGatewayStructBttn";
            this.ImportGatewayStructBttn.Size = new System.Drawing.Size(147, 23);
            this.ImportGatewayStructBttn.TabIndex = 2;
            this.ImportGatewayStructBttn.Text = "Импортировать модель";
            this.ImportGatewayStructBttn.UseVisualStyleBackColor = true;
            this.ImportGatewayStructBttn.Click += new System.EventHandler(this.ImportGatewayStructBttn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Поиск";
            // 
            // SignalsSerch
            // 
            this.SignalsSerch.Dock = System.Windows.Forms.DockStyle.Top;
            this.SignalsSerch.Location = new System.Drawing.Point(63, 3);
            this.SignalsSerch.Name = "SignalsSerch";
            this.SignalsSerch.Size = new System.Drawing.Size(225, 20);
            this.SignalsSerch.TabIndex = 4;
            this.SignalsSerch.TextChanged += new System.EventHandler(this.SignalsSerch_TextChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.Controls.Add(this.AddGatewayBttn, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.IPTextBox, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.PortTextBox, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.GatewaySelectionCB, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(598, 84);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // AddGatewayBttn
            // 
            this.AddGatewayBttn.Location = new System.Drawing.Point(568, 3);
            this.AddGatewayBttn.Name = "AddGatewayBttn";
            this.AddGatewayBttn.Size = new System.Drawing.Size(25, 22);
            this.AddGatewayBttn.TabIndex = 1;
            this.AddGatewayBttn.Text = "+";
            this.AddGatewayBttn.UseVisualStyleBackColor = true;
            this.AddGatewayBttn.Click += new System.EventHandler(this.AddGatewayBttn_Click);
            // 
            // IPTextBox
            // 
            this.IPTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.IPTextBox.Location = new System.Drawing.Point(368, 31);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(194, 20);
            this.IPTextBox.TabIndex = 2;
            this.IPTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.IPTextBox.Leave += new System.EventHandler(this.IP_TextBox_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(303, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "IP-адрес";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(303, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Порт";
            // 
            // PortTextBox
            // 
            this.PortTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PortTextBox.Location = new System.Drawing.Point(368, 59);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(194, 20);
            this.PortTextBox.TabIndex = 3;
            this.PortTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.PortTextBox.Leave += new System.EventHandler(this.Port_TextBox_Leave);
            // 
            // GatewaySelectionCB
            // 
            this.GatewaySelectionCB.Dock = System.Windows.Forms.DockStyle.Top;
            this.GatewaySelectionCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GatewaySelectionCB.FormattingEnabled = true;
            this.GatewaySelectionCB.Location = new System.Drawing.Point(368, 3);
            this.GatewaySelectionCB.Name = "GatewaySelectionCB";
            this.GatewaySelectionCB.Size = new System.Drawing.Size(194, 21);
            this.GatewaySelectionCB.TabIndex = 0;
            this.GatewaySelectionCB.SelectedIndexChanged += new System.EventHandler(this.GatewaySelectionCB_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(303, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Шлюз";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel4.Controls.Add(this.SaveBttn, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.CancelBttn, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 624);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(598, 34);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // SaveBttn
            // 
            this.SaveBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.SaveBttn.Location = new System.Drawing.Point(301, 3);
            this.SaveBttn.Name = "SaveBttn";
            this.SaveBttn.Size = new System.Drawing.Size(144, 23);
            this.SaveBttn.TabIndex = 0;
            this.SaveBttn.Text = "Сохранить";
            this.SaveBttn.UseVisualStyleBackColor = true;
            this.SaveBttn.Click += new System.EventHandler(this.SaveBttn_Click);
            // 
            // CancelBttn
            // 
            this.CancelBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.CancelBttn.Location = new System.Drawing.Point(451, 3);
            this.CancelBttn.Name = "CancelBttn";
            this.CancelBttn.Size = new System.Drawing.Size(144, 23);
            this.CancelBttn.TabIndex = 1;
            this.CancelBttn.Text = "Отмена";
            this.CancelBttn.UseVisualStyleBackColor = true;
            this.CancelBttn.Click += new System.EventHandler(this.CancelBttn_Click);
            // 
            // ModbusExchangeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 661);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(620, 700);
            this.Name = "ModbusExchangeView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Обмен сигналами Modbus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModbusExchangeView_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Exchange)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListView SignalsList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private BrightIdeasSoftware.TreeListView Exchange;
        private System.Windows.Forms.Button ImportGatewayStructBttn;
        private System.Windows.Forms.ComboBox GatewaySelectionCB;
        private System.Windows.Forms.Button AddGatewayBttn;
        private System.Windows.Forms.ColumnHeader SignalName;
        private System.Windows.Forms.ColumnHeader SignalDescription;
        private System.Windows.Forms.Button SaveBttn;
        private System.Windows.Forms.Button CancelBttn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SignalsSerch;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}