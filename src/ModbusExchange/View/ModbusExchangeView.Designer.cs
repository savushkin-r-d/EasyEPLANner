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
            this.listView1 = new System.Windows.Forms.ListView();
            this.Exchange = new BrightIdeasSoftware.TreeListView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.ImportGatewayStructBttn = new System.Windows.Forms.Button();
            this.GatewaySelectionCB = new System.Windows.Forms.ComboBox();
            this.AddGatewayBttn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Exchange)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.60317F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.39683F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(794, 471);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 343F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 137F));
            this.tableLayoutPanel2.Controls.Add(this.listView1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.Exchange, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.ImportGatewayStructBttn, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 102);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.04682F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.95318F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(788, 299);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // listView1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.listView1, 2);
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 44);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(302, 252);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // Exchange
            // 
            this.Exchange.CellEditUseWholeCell = false;
            this.tableLayoutPanel2.SetColumnSpan(this.Exchange, 2);
            this.Exchange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Exchange.HideSelection = false;
            this.Exchange.Location = new System.Drawing.Point(311, 44);
            this.Exchange.Name = "Exchange";
            this.Exchange.ShowGroups = false;
            this.Exchange.Size = new System.Drawing.Size(474, 252);
            this.Exchange.TabIndex = 1;
            this.Exchange.UseCompatibleStateImageBehavior = false;
            this.Exchange.View = System.Windows.Forms.View.Details;
            this.Exchange.VirtualMode = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 225F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tableLayoutPanel3.Controls.Add(this.GatewaySelectionCB, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.AddGatewayBttn, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.6129F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.3871F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(788, 93);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 206F));
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 407);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(788, 61);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // ImportGatewayStructBttn
            // 
            this.ImportGatewayStructBttn.Location = new System.Drawing.Point(654, 3);
            this.ImportGatewayStructBttn.Name = "ImportGatewayStructBttn";
            this.ImportGatewayStructBttn.Size = new System.Drawing.Size(75, 23);
            this.ImportGatewayStructBttn.TabIndex = 2;
            this.ImportGatewayStructBttn.Text = "button1";
            this.ImportGatewayStructBttn.UseVisualStyleBackColor = true;
            // 
            // GatewaySelectionCB
            // 
            this.GatewaySelectionCB.FormattingEnabled = true;
            this.GatewaySelectionCB.Location = new System.Drawing.Point(383, 3);
            this.GatewaySelectionCB.Name = "GatewaySelectionCB";
            this.GatewaySelectionCB.Size = new System.Drawing.Size(121, 21);
            this.GatewaySelectionCB.TabIndex = 0;
            // 
            // AddGatewayBttn
            // 
            this.AddGatewayBttn.Location = new System.Drawing.Point(608, 3);
            this.AddGatewayBttn.Name = "AddGatewayBttn";
            this.AddGatewayBttn.Size = new System.Drawing.Size(75, 23);
            this.AddGatewayBttn.TabIndex = 1;
            this.AddGatewayBttn.Text = "button2";
            this.AddGatewayBttn.UseVisualStyleBackColor = true;
            this.AddGatewayBttn.Click += new System.EventHandler(this.AddGatewayBttn_Click);
            // 
            // ModbusExchangeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 471);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ModbusExchangeView";
            this.Text = "ModbusExchangeView";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Exchange)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private BrightIdeasSoftware.TreeListView Exchange;
        private System.Windows.Forms.Button ImportGatewayStructBttn;
        private System.Windows.Forms.ComboBox GatewaySelectionCB;
        private System.Windows.Forms.Button AddGatewayBttn;
    }
}