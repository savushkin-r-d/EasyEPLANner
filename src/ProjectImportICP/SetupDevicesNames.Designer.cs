namespace EasyEPlanner.ProjectImportICP
{
    partial class SetupDevicesNames
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
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LoadRenameMapBttn = new System.Windows.Forms.Button();
            this.OkBttn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectListView
            // 
            this.objectListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.objectListView.CellEditUseWholeCell = false;
            this.tableLayoutPanel1.SetColumnSpan(this.objectListView, 3);
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView.HideSelection = false;
            this.objectListView.Location = new System.Drawing.Point(3, 3);
            this.objectListView.Name = "objectListView";
            this.objectListView.Size = new System.Drawing.Size(486, 451);
            this.objectListView.TabIndex = 0;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.View = System.Windows.Forms.View.Details;
            this.objectListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.objectListView_CellEditFinishing);
            this.objectListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.objectListView_CellEditStarting);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.objectListView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LoadRenameMapBttn, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.OkBttn, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(492, 495);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // LoadRenameMapBttn
            // 
            this.LoadRenameMapBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.LoadRenameMapBttn.Location = new System.Drawing.Point(3, 460);
            this.LoadRenameMapBttn.Name = "LoadRenameMapBttn";
            this.LoadRenameMapBttn.Size = new System.Drawing.Size(327, 23);
            this.LoadRenameMapBttn.TabIndex = 1;
            this.LoadRenameMapBttn.Text = "Применить файл переименования";
            this.LoadRenameMapBttn.UseVisualStyleBackColor = true;
            this.LoadRenameMapBttn.Click += new System.EventHandler(this.LoadRenameMapBttn_Click);
            // 
            // OkBttn
            // 
            this.OkBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.OkBttn.Location = new System.Drawing.Point(419, 460);
            this.OkBttn.Name = "OkBttn";
            this.OkBttn.Size = new System.Drawing.Size(70, 23);
            this.OkBttn.TabIndex = 2;
            this.OkBttn.Text = "OK";
            this.OkBttn.UseVisualStyleBackColor = true;
            this.OkBttn.Click += new System.EventHandler(this.OkBttn_Click);
            // 
            // SetupDevicesNames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 495);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SetupDevicesNames";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка замены имен устройств";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button LoadRenameMapBttn;
        private System.Windows.Forms.Button OkBttn;
    }
}