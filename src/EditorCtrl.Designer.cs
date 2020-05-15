namespace Editor
    {
    partial class EditorCtrl
        {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose( bool disposing )
            {
            if ( disposing && ( components != null ) )
                {
                components.Dispose();
                }
            base.Dispose( disposing );
            }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
            {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorCtrl));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator_1 = new System.Windows.Forms.ToolStripSeparator();
            this.drawDev_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.edit_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refresh_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.insertButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.replaceButton = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.editorTView = new BrightIdeasSoftware.TreeListView();
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorTView)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_1});
            this.statusStrip.Location = new System.Drawing.Point(0, 478);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(506, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_1
            // 
            this.toolStripStatusLabel_1.Name = "toolStripStatusLabel_1";
            this.toolStripStatusLabel_1.Size = new System.Drawing.Size(112, 17);
            this.toolStripStatusLabel_1.Text = "toolStripStatusLabel";
            // 
            // toolStrip
            // 
            this.toolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_1,
            this.toolStripButton_2,
            this.toolStripButton_3,
            this.toolStripButton_4,
            this.toolStripButton_5,
            this.toolStripSeparator_1,
            this.drawDev_toolStripButton,
            this.edit_toolStripButton,
            this.toolStripSeparator1,
            this.refresh_toolStripButton,
            this.toolStripSeparator2,
            this.insertButton,
            this.deleteButton,
            this.copyButton,
            this.pasteButton,
            this.replaceButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(360, 27);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButton_1
            // 
            this.toolStripButton_1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_1.Image = global::EasyEPlanner.Properties.Resources.plus1;
            this.toolStripButton_1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_1.Name = "toolStripButton_1";
            this.toolStripButton_1.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton_1.Tag = "1";
            this.toolStripButton_1.Text = "toolStripButton1";
            this.toolStripButton_1.ToolTipText = "Уровень 1";
            this.toolStripButton_1.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton_2
            // 
            this.toolStripButton_2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_2.Image = global::EasyEPlanner.Properties.Resources.plus2;
            this.toolStripButton_2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_2.Name = "toolStripButton_2";
            this.toolStripButton_2.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton_2.Tag = "2";
            this.toolStripButton_2.Text = "toolStripButton2";
            this.toolStripButton_2.ToolTipText = "Уровень 2";
            this.toolStripButton_2.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton_3
            // 
            this.toolStripButton_3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_3.Image = global::EasyEPlanner.Properties.Resources.plus3;
            this.toolStripButton_3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_3.Name = "toolStripButton_3";
            this.toolStripButton_3.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton_3.Tag = "3";
            this.toolStripButton_3.Text = "toolStripButton3";
            this.toolStripButton_3.ToolTipText = "Уровень 3";
            this.toolStripButton_3.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton_4
            // 
            this.toolStripButton_4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_4.Image = global::EasyEPlanner.Properties.Resources.plus4;
            this.toolStripButton_4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_4.Name = "toolStripButton_4";
            this.toolStripButton_4.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton_4.Tag = "4";
            this.toolStripButton_4.Text = "toolStripButton4";
            this.toolStripButton_4.ToolTipText = "Уровень 4";
            this.toolStripButton_4.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton_5
            // 
            this.toolStripButton_5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_5.Image = global::EasyEPlanner.Properties.Resources.plus5;
            this.toolStripButton_5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_5.Name = "toolStripButton_5";
            this.toolStripButton_5.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton_5.Tag = "5";
            this.toolStripButton_5.Text = "toolStripButton5";
            this.toolStripButton_5.ToolTipText = "Уровень 5";
            this.toolStripButton_5.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator_1
            // 
            this.toolStripSeparator_1.Name = "toolStripSeparator_1";
            this.toolStripSeparator_1.Size = new System.Drawing.Size(6, 27);
            // 
            // drawDev_toolStripButton
            // 
            this.drawDev_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.drawDev_toolStripButton.Image = global::EasyEPlanner.Properties.Resources.highlight;
            this.drawDev_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.drawDev_toolStripButton.Name = "drawDev_toolStripButton";
            this.drawDev_toolStripButton.Size = new System.Drawing.Size(24, 24);
            this.drawDev_toolStripButton.ToolTipText = "Подсветка устройств";
            this.drawDev_toolStripButton.Click += new System.EventHandler(this.DrawDev_toolStripButton_Click);
            // 
            // edit_toolStripButton
            // 
            this.edit_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.edit_toolStripButton.Image = global::EasyEPlanner.Properties.Resources.edit;
            this.edit_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.edit_toolStripButton.Name = "edit_toolStripButton";
            this.edit_toolStripButton.Size = new System.Drawing.Size(24, 24);
            this.edit_toolStripButton.Text = "Редактирование устройств";
            this.edit_toolStripButton.Click += new System.EventHandler(this.edit_toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // refresh_toolStripButton
            // 
            this.refresh_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refresh_toolStripButton.Image = global::EasyEPlanner.Properties.Resources.refresh;
            this.refresh_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refresh_toolStripButton.Name = "refresh_toolStripButton";
            this.refresh_toolStripButton.Size = new System.Drawing.Size(24, 24);
            this.refresh_toolStripButton.Text = "Синхронизация и сохранение";
            this.refresh_toolStripButton.Click += new System.EventHandler(this.refresh_toolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // insertButton
            // 
            this.insertButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.insertButton.Image = global::EasyEPlanner.Properties.Resources.create;
            this.insertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(24, 24);
            this.insertButton.Text = "insertButton";
            this.insertButton.ToolTipText = "Создать";
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::EasyEPlanner.Properties.Resources.delete;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(24, 24);
            this.deleteButton.Text = "deleteButton";
            this.deleteButton.ToolTipText = "Удалить";
            // 
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Image = global::EasyEPlanner.Properties.Resources.copy;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(24, 24);
            this.copyButton.Text = "copyButton";
            this.copyButton.ToolTipText = "Копировать";
            // 
            // pasteButton
            // 
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteButton.Image = global::EasyEPlanner.Properties.Resources.paste;
            this.pasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(24, 24);
            this.pasteButton.Text = "pasteButton";
            this.pasteButton.ToolTipText = "Вставить (Ctrl + V)";
            // 
            // replaceButton
            // 
            this.replaceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.replaceButton.Image = global::EasyEPlanner.Properties.Resources.replace;
            this.replaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(24, 24);
            this.replaceButton.Text = "replaceButton";
            this.replaceButton.ToolTipText = "Заменить (Ctrl + B)";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "plant.ico");
            this.imageList1.Images.SetKeyName(1, "tank.ico");
            this.imageList1.Images.SetKeyName(2, "mode.ico");
            this.imageList1.Images.SetKeyName(3, "mode.ico");
            this.imageList1.Images.SetKeyName(4, "step.ico");
            this.imageList1.Images.SetKeyName(5, "on.ico");
            this.imageList1.Images.SetKeyName(6, "off.ico");
            this.imageList1.Images.SetKeyName(7, "reqiredFB.ico");
            this.imageList1.Images.SetKeyName(8, "upperSeats.ico");
            this.imageList1.Images.SetKeyName(9, "lowerSeats.ico");
            this.imageList1.Images.SetKeyName(10, "pairDIDO.ico");
            this.imageList1.Images.SetKeyName(11, "wash.ico");
            this.imageList1.Images.SetKeyName(12, "params.ico");
            this.imageList1.Images.SetKeyName(13, "timers.ico");
            // 
            // editorTView
            // 
            this.editorTView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.editorTView.CellEditUseWholeCell = false;
            this.editorTView.CopySelectionOnControlCUsesDragSource = false;
            this.editorTView.FullRowSelect = true;
            this.editorTView.GridLines = true;
            this.editorTView.HideSelection = false;
            this.editorTView.LabelWrap = false;
            this.editorTView.Location = new System.Drawing.Point(3, 28);
            this.editorTView.MinimumSize = new System.Drawing.Size(200, 250);
            this.editorTView.MultiSelect = false;
            this.editorTView.Name = "editorTView";
            this.editorTView.OwnerDrawnHeader = true;
            this.editorTView.PersistentCheckBoxes = false;
            this.editorTView.RowHeight = 20;
            this.editorTView.SelectAllOnControlA = false;
            this.editorTView.SelectColumnsMenuStaysOpen = false;
            this.editorTView.SelectColumnsOnRightClick = false;
            this.editorTView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.editorTView.ShowFilterMenuOnRightClick = false;
            this.editorTView.ShowGroups = false;
            this.editorTView.ShowImagesOnSubItems = true;
            this.editorTView.ShowSortIndicators = false;
            this.editorTView.Size = new System.Drawing.Size(500, 447);
            this.editorTView.SmallImageList = this.imageList1;
            this.editorTView.TabIndex = 4;
            this.editorTView.TriggerCellOverEventsWhenOverHeader = false;
            this.editorTView.UseAlternatingBackColors = true;
            this.editorTView.UseCellFormatEvents = true;
            this.editorTView.UseCompatibleStateImageBehavior = false;
            this.editorTView.UseHotControls = false;
            this.editorTView.View = System.Windows.Forms.View.Details;
            this.editorTView.VirtualMode = true;
            this.editorTView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.editorTView_CellEditFinishing);
            this.editorTView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.editorTView_CellEditStarting);
            this.editorTView.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.editorTView_FormatCell);
            this.editorTView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.editorTView_ColumnWidthChanging);
            this.editorTView.SelectedIndexChanged += new System.EventHandler(this.editorTView_SelectedIndexChanged);
            this.editorTView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editorTView_KeyDown);
            this.editorTView.MouseEnter += new System.EventHandler(this.editorTView_MouseEnter);
            this.editorTView.MouseLeave += new System.EventHandler(this.editorTView_MouseLeave);
            // 
            // EditorCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editorTView);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Name = "EditorCtrl";
            this.Size = new System.Drawing.Size(506, 500);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorTView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton_1;
        private System.Windows.Forms.ToolStripButton toolStripButton_2;
        private System.Windows.Forms.ToolStripButton toolStripButton_3;
        private System.Windows.Forms.ToolStripButton toolStripButton_4;
        private System.Windows.Forms.ToolStripButton toolStripButton_5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator_1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton drawDev_toolStripButton;
        private System.Windows.Forms.ToolStripButton edit_toolStripButton;
        private System.Windows.Forms.ToolStripButton refresh_toolStripButton;
        private System.Windows.Forms.ImageList imageList1;
        public BrightIdeasSoftware.TreeListView editorTView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton insertButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton replaceButton;
        private System.Windows.Forms.ToolStripButton pasteButton;
    }
    }
