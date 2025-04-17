namespace EasyEPlanner.ModbusExchange.View
{
    partial class InputGatewayName
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.NameTB = new System.Windows.Forms.TextBox();
            this.Apply = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.NameTB, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Apply, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.Cancel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(315, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // NameTB
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.NameTB, 3);
            this.NameTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NameTB.Location = new System.Drawing.Point(20, 36);
            this.NameTB.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
            this.NameTB.Name = "NameTB";
            this.NameTB.Size = new System.Drawing.Size(275, 20);
            this.NameTB.TabIndex = 0;
            // 
            // Apply
            // 
            this.Apply.Dock = System.Windows.Forms.DockStyle.Top;
            this.Apply.Location = new System.Drawing.Point(108, 69);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(99, 23);
            this.Apply.TabIndex = 1;
            this.Apply.Text = "Создать";
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // Cancel
            // 
            this.Cancel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Cancel.Location = new System.Drawing.Point(213, 69);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(99, 23);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Отменить";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(20, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 27);
            this.label1.TabIndex = 3;
            this.label1.Text = "Введите название:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // InputGatewayName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 100);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "InputGatewayName";
            this.Text = "InputGatewayName";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox NameTB;
        private System.Windows.Forms.Button Apply;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label1;
    }
}