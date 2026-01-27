namespace EasyEPlanner
    {
    partial class LogFrm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
            {
            if( disposing && ( components != null ) )
                {
                components.Dispose();
                }
            base.Dispose( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ExportButton = new System.Windows.Forms.Button();
            this.MainTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTableLayoutPanel.SetColumnSpan(this.progressBar, 3);
            this.progressBar.Location = new System.Drawing.Point(3, 355);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(778, 23);
            this.progressBar.TabIndex = 0;
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTableLayoutPanel.SetColumnSpan(this.richTextBox, 3);
            this.richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox.Location = new System.Drawing.Point(3, 3);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(778, 345);
            this.richTextBox.TabIndex = 1;
            this.richTextBox.Text = "";
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OkButton.Enabled = false;
            this.OkButton.Location = new System.Drawing.Point(687, 385);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(94, 23);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "Ок";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.MainTableLayoutPanel.Controls.Add(this.richTextBox, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.progressBar, 0, 1);
            this.MainTableLayoutPanel.Controls.Add(this.OkButton, 2, 2);
            this.MainTableLayoutPanel.Controls.Add(this.ExportButton, 1, 2);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 3;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(784, 411);
            this.MainTableLayoutPanel.TabIndex = 3;
            // 
            // ExportButton
            // 
            this.ExportButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExportButton.Location = new System.Drawing.Point(587, 385);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(94, 23);
            this.ExportButton.TabIndex = 3;
            this.ExportButton.Text = "Выгрузить лог";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // LogFrm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.OkButton;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.ControlBox = false;
            this.Controls.Add(this.MainTableLayoutPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "LogFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Log";
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

            }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.Button ExportButton;
    }
    }