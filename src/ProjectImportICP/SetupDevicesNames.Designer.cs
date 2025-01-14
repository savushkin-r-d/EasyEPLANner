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
            this.renameDevicesOLV = new BrightIdeasSoftware.ObjectListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LoadRenameMapBttn = new System.Windows.Forms.Button();
            this.OkBttn = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.DevicesTabPage = new System.Windows.Forms.TabPage();
            this.parametersTabPage = new System.Windows.Forms.TabPage();
            this.defaultParametersOLV = new BrightIdeasSoftware.ObjectListView();
            ((System.ComponentModel.ISupportInitialize)(this.renameDevicesOLV)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.DevicesTabPage.SuspendLayout();
            this.parametersTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.defaultParametersOLV)).BeginInit();
            this.SuspendLayout();
            // 
            // renameDevicesOLV
            // 
            this.renameDevicesOLV.AccessibleName = "";
            this.renameDevicesOLV.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.renameDevicesOLV.CellEditUseWholeCell = false;
            this.renameDevicesOLV.Cursor = System.Windows.Forms.Cursors.Default;
            this.renameDevicesOLV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renameDevicesOLV.HideSelection = false;
            this.renameDevicesOLV.Location = new System.Drawing.Point(3, 3);
            this.renameDevicesOLV.Name = "renameDevicesOLV";
            this.renameDevicesOLV.Size = new System.Drawing.Size(478, 425);
            this.renameDevicesOLV.TabIndex = 0;
            this.renameDevicesOLV.UseCompatibleStateImageBehavior = false;
            this.renameDevicesOLV.View = System.Windows.Forms.View.Details;
            this.renameDevicesOLV.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.renameDevicesOLV_CellEditFinishing);
            this.renameDevicesOLV.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.renameDevicesOLV_CellEditStarting);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.Controls.Add(this.LoadRenameMapBttn, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.OkBttn, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(492, 495);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // LoadRenameMapBttn
            // 
            this.LoadRenameMapBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.LoadRenameMapBttn.Location = new System.Drawing.Point(3, 460);
            this.LoadRenameMapBttn.Name = "LoadRenameMapBttn";
            this.LoadRenameMapBttn.Size = new System.Drawing.Size(235, 23);
            this.LoadRenameMapBttn.TabIndex = 1;
            this.LoadRenameMapBttn.Text = "Использовать файл переименования";
            this.LoadRenameMapBttn.UseVisualStyleBackColor = true;
            this.LoadRenameMapBttn.Click += new System.EventHandler(this.LoadRenameMapBttn_Click);
            // 
            // OkBttn
            // 
            this.OkBttn.Dock = System.Windows.Forms.DockStyle.Top;
            this.OkBttn.Location = new System.Drawing.Point(404, 460);
            this.OkBttn.Name = "OkBttn";
            this.OkBttn.Size = new System.Drawing.Size(85, 23);
            this.OkBttn.TabIndex = 2;
            this.OkBttn.Text = "Применить";
            this.OkBttn.UseVisualStyleBackColor = true;
            this.OkBttn.Click += new System.EventHandler(this.OkBttn_Click);
            // 
            // tabControl
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl, 3);
            this.tabControl.Controls.Add(this.DevicesTabPage);
            this.tabControl.Controls.Add(this.parametersTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(492, 457);
            this.tabControl.TabIndex = 3;
            // 
            // DevicesTabPage
            // 
            this.DevicesTabPage.Controls.Add(this.renameDevicesOLV);
            this.DevicesTabPage.Location = new System.Drawing.Point(4, 22);
            this.DevicesTabPage.Name = "DevicesTabPage";
            this.DevicesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DevicesTabPage.Size = new System.Drawing.Size(484, 431);
            this.DevicesTabPage.TabIndex = 0;
            this.DevicesTabPage.Text = "Переименование устройств";
            this.DevicesTabPage.UseVisualStyleBackColor = true;
            // 
            // parametersTabPage
            // 
            this.parametersTabPage.Controls.Add(this.defaultParametersOLV);
            this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
            this.parametersTabPage.Name = "parametersTabPage";
            this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.parametersTabPage.Size = new System.Drawing.Size(484, 431);
            this.parametersTabPage.TabIndex = 1;
            this.parametersTabPage.Text = "Параметры по умолчанию";
            this.parametersTabPage.UseVisualStyleBackColor = true;
            // 
            // defaultParametersOLV
            // 
            this.defaultParametersOLV.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.defaultParametersOLV.CellEditUseWholeCell = false;
            this.defaultParametersOLV.Cursor = System.Windows.Forms.Cursors.Default;
            this.defaultParametersOLV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultParametersOLV.HideSelection = false;
            this.defaultParametersOLV.Location = new System.Drawing.Point(3, 3);
            this.defaultParametersOLV.Name = "defaultParametersOLV";
            this.defaultParametersOLV.Size = new System.Drawing.Size(478, 425);
            this.defaultParametersOLV.TabIndex = 0;
            this.defaultParametersOLV.UseCompatibleStateImageBehavior = false;
            this.defaultParametersOLV.View = System.Windows.Forms.View.Details;
            this.defaultParametersOLV.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.defaultParametersOLV_CellEditFinishing);
            this.defaultParametersOLV.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.defaultParametersOLV_CellEditStarting);
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
            ((System.ComponentModel.ISupportInitialize)(this.renameDevicesOLV)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.DevicesTabPage.ResumeLayout(false);
            this.parametersTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.defaultParametersOLV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView renameDevicesOLV;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button LoadRenameMapBttn;
        private System.Windows.Forms.Button OkBttn;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage DevicesTabPage;
        private System.Windows.Forms.TabPage parametersTabPage;
        private BrightIdeasSoftware.ObjectListView defaultParametersOLV;
    }
}