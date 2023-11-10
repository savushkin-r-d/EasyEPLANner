using System.Windows.Forms;

namespace Editor
    {
    partial class NewEditorControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewEditorControl));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.expandDropDownList = new System.Windows.Forms.ToolStripDropDownButton();
            this.expandButton1 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandButton2 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandButton3 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandButton4 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandButton5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator_1 = new System.Windows.Forms.ToolStripSeparator();
            this.drawDev_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.edit_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refresh_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.insertButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.cutButton = new System.Windows.Forms.ToolStripButton();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.replaceButton = new System.Windows.Forms.ToolStripButton();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.importButton = new System.Windows.Forms.ToolStripButton();
            this.exportButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.changeBasesObjBtn = new System.Windows.Forms.ToolStripButton();
            this.hideEmptyItemsBtn = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.editorTView = new BrightIdeasSoftware.TreeListView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolSettingsStrip = new System.Windows.Forms.ToolStrip();
            this.toolSettingDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.settingMenuItem_expand = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_drawDev = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_refresh = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_insert = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_delete = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_cut = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_copy = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_paste = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_replace = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_moveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_moveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_import = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_export = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_changeBaseObj = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_hideEmptyItems = new System.Windows.Forms.ToolStripMenuItem();
            this.settingMenuItem_search = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanelSearchBox = new System.Windows.Forms.TableLayoutPanel();
            this.formatNumericUpDown_SearchSelectedItem = new EditorControls.FormatNumericUpDown();
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorTView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.toolSettingsStrip.SuspendLayout();
            this.tableLayoutPanelSearchBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formatNumericUpDown_SearchSelectedItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandDropDownList,
            this.toolStripSeparator_1,
            this.drawDev_toolStripButton,
            this.edit_toolStripButton,
            this.toolStripSeparator1,
            this.refresh_toolStripButton,
            this.toolStripSeparator2,
            this.insertButton,
            this.deleteButton,
            this.cutButton,
            this.copyButton,
            this.pasteButton,
            this.replaceButton,
            this.moveUpButton,
            this.moveDownButton,
            this.toolStripSeparator3,
            this.importButton,
            this.exportButton,
            this.toolStripSeparator4,
            this.changeBasesObjBtn,
            this.hideEmptyItemsBtn});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(719, 29);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // expandDropDownList
            // 
            this.expandDropDownList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.expandDropDownList.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandButton1,
            this.expandButton2,
            this.expandButton3,
            this.expandButton4,
            this.expandButton5});
            this.expandDropDownList.Image = global::EasyEPlanner.Properties.Resources.expand;
            this.expandDropDownList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.expandDropDownList.Name = "expandDropDownList";
            this.expandDropDownList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.expandDropDownList.Size = new System.Drawing.Size(33, 26);
            this.expandDropDownList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.expandDropDownList.ToolTipText = "Выбрать уровень развертки";
            // 
            // expandButton1
            // 
            this.expandButton1.Image = global::EasyEPlanner.Properties.Resources.expand1;
            this.expandButton1.Name = "expandButton1";
            this.expandButton1.ShortcutKeyDisplayString = "";
            this.expandButton1.ShowShortcutKeys = false;
            this.expandButton1.Size = new System.Drawing.Size(125, 22);
            this.expandButton1.Tag = "1";
            this.expandButton1.Text = " Уровень 1";
            this.expandButton1.ToolTipText = "Уровень 1";
            this.expandButton1.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // expandButton2
            // 
            this.expandButton2.Image = global::EasyEPlanner.Properties.Resources.expand2;
            this.expandButton2.Name = "expandButton2";
            this.expandButton2.ShowShortcutKeys = false;
            this.expandButton2.Size = new System.Drawing.Size(125, 22);
            this.expandButton2.Tag = "2";
            this.expandButton2.Text = "Уровень 2";
            this.expandButton2.ToolTipText = "Уровень 2";
            this.expandButton2.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // expandButton3
            // 
            this.expandButton3.Image = global::EasyEPlanner.Properties.Resources.expand3;
            this.expandButton3.Name = "expandButton3";
            this.expandButton3.ShowShortcutKeys = false;
            this.expandButton3.Size = new System.Drawing.Size(125, 22);
            this.expandButton3.Tag = "3";
            this.expandButton3.Text = "Уровень 3";
            this.expandButton3.ToolTipText = "Уровень 3";
            this.expandButton3.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // expandButton4
            // 
            this.expandButton4.Image = global::EasyEPlanner.Properties.Resources.expand4;
            this.expandButton4.Name = "expandButton4";
            this.expandButton4.ShowShortcutKeys = false;
            this.expandButton4.Size = new System.Drawing.Size(125, 22);
            this.expandButton4.Tag = "4";
            this.expandButton4.Text = "Уровень 4";
            this.expandButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.expandButton4.ToolTipText = "Уровень 4";
            this.expandButton4.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // expandButton5
            // 
            this.expandButton5.Image = global::EasyEPlanner.Properties.Resources.expand5;
            this.expandButton5.Name = "expandButton5";
            this.expandButton5.ShowShortcutKeys = false;
            this.expandButton5.Size = new System.Drawing.Size(125, 22);
            this.expandButton5.Tag = "5";
            this.expandButton5.Text = "Уровень 5";
            this.expandButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.expandButton5.ToolTipText = "Уровень 5";
            this.expandButton5.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator_1
            // 
            this.toolStripSeparator_1.Name = "toolStripSeparator_1";
            this.toolStripSeparator_1.Size = new System.Drawing.Size(6, 29);
            // 
            // drawDev_toolStripButton
            // 
            this.drawDev_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.drawDev_toolStripButton.Image = global::EasyEPlanner.Properties.Resources.highlight;
            this.drawDev_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.drawDev_toolStripButton.Name = "drawDev_toolStripButton";
            this.drawDev_toolStripButton.Size = new System.Drawing.Size(24, 26);
            this.drawDev_toolStripButton.ToolTipText = "Подсветка устройств";
            this.drawDev_toolStripButton.Click += new System.EventHandler(this.DrawDev_toolStripButton_Click);
            // 
            // edit_toolStripButton
            // 
            this.edit_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.edit_toolStripButton.Image = global::EasyEPlanner.Properties.Resources.edit;
            this.edit_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.edit_toolStripButton.Name = "edit_toolStripButton";
            this.edit_toolStripButton.Size = new System.Drawing.Size(24, 26);
            this.edit_toolStripButton.Text = "Редактирование устройств";
            this.edit_toolStripButton.ToolTipText = "Редактирование";
            this.edit_toolStripButton.Click += new System.EventHandler(this.edit_toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // refresh_toolStripButton
            // 
            this.refresh_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refresh_toolStripButton.Image = global::EasyEPlanner.Properties.Resources.refresh;
            this.refresh_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refresh_toolStripButton.Name = "refresh_toolStripButton";
            this.refresh_toolStripButton.Size = new System.Drawing.Size(24, 26);
            this.refresh_toolStripButton.Text = "Синхронизация и сохранение";
            this.refresh_toolStripButton.Click += new System.EventHandler(this.refresh_toolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 29);
            // 
            // insertButton
            // 
            this.insertButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.insertButton.Image = global::EasyEPlanner.Properties.Resources.create;
            this.insertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(24, 26);
            this.insertButton.Text = "insertButton";
            this.insertButton.ToolTipText = "Создать (Insert)";
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::EasyEPlanner.Properties.Resources.delete;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(24, 26);
            this.deleteButton.Text = "deleteButton";
            this.deleteButton.ToolTipText = "Удалить (Delete)";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cutButton
            // 
            this.cutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutButton.Image = global::EasyEPlanner.Properties.Resources.cut;
            this.cutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutButton.Name = "cutButton";
            this.cutButton.Size = new System.Drawing.Size(24, 26);
            this.cutButton.Text = "copyButton";
            this.cutButton.ToolTipText = "Вырезать (Ctrl + X)";
            this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Image = global::EasyEPlanner.Properties.Resources.copy;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(24, 26);
            this.copyButton.Text = "copyButton";
            this.copyButton.ToolTipText = "Копировать (Ctrl + C)";
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteButton.Image = global::EasyEPlanner.Properties.Resources.paste;
            this.pasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(24, 26);
            this.pasteButton.Text = "pasteButton";
            this.pasteButton.ToolTipText = "Вставить (Ctrl + V)";
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // replaceButton
            // 
            this.replaceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.replaceButton.Image = global::EasyEPlanner.Properties.Resources.replace;
            this.replaceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(24, 26);
            this.replaceButton.Text = "replaceButton";
            this.replaceButton.ToolTipText = "Заменить (Ctrl + B)";
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpButton.Image = global::EasyEPlanner.Properties.Resources.moveup;
            this.moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(24, 26);
            this.moveUpButton.Text = "toolStripButton2";
            this.moveUpButton.ToolTipText = "Переместить вверх (Ctrl + Up)";
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveDownButton
            // 
            this.moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownButton.Image = global::EasyEPlanner.Properties.Resources.movedown;
            this.moveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(24, 26);
            this.moveDownButton.Text = "toolStripButton1";
            this.moveDownButton.ToolTipText = "Переместить вниз (Ctrl + Down)";
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 29);
            // 
            // importButton
            // 
            this.importButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.importButton.Image = ((System.Drawing.Image)(resources.GetObject("importButton.Image")));
            this.importButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(24, 26);
            this.importButton.Text = "importButton";
            this.importButton.ToolTipText = "Импорт объектов";
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportButton.Image = global::EasyEPlanner.Properties.Resources.export;
            this.exportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(24, 26);
            this.exportButton.Text = "exportButton";
            this.exportButton.ToolTipText = "Экспорт объектов";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 29);
            // 
            // changeBasesObjBtn
            // 
            this.changeBasesObjBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.changeBasesObjBtn.Image = global::EasyEPlanner.Properties.Resources.changeObj;
            this.changeBasesObjBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.changeBasesObjBtn.Name = "changeBasesObjBtn";
            this.changeBasesObjBtn.Size = new System.Drawing.Size(24, 26);
            this.changeBasesObjBtn.Text = "changeBaseObj";
            this.changeBasesObjBtn.ToolTipText = "Изменить базовый объект";
            this.changeBasesObjBtn.Click += new System.EventHandler(this.changeBasesObjBtn_Click);
            // 
            // hideEmptyItemsBtn
            // 
            this.hideEmptyItemsBtn.CheckOnClick = true;
            this.hideEmptyItemsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideEmptyItemsBtn.Image = global::EasyEPlanner.Properties.Resources.hideEmptyItems;
            this.hideEmptyItemsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideEmptyItemsBtn.Name = "hideEmptyItemsBtn";
            this.hideEmptyItemsBtn.Size = new System.Drawing.Size(24, 26);
            this.hideEmptyItemsBtn.Text = "hideEmptyItems";
            this.hideEmptyItemsBtn.ToolTipText = "Скрыть пустые элементы";
            this.hideEmptyItemsBtn.CheckStateChanged += new System.EventHandler(this.hideEmptyItemsBtn_CheckStateChanged);
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
            this.imageList1.Images.SetKeyName(13, "equipment.ico");
            this.imageList1.Images.SetKeyName(14, "generic.png");
            this.imageList1.Images.SetKeyName(15, "group.png");
            // 
            // editorTView
            // 
            this.editorTView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.editorTView.CellEditUseWholeCell = false;
            this.editorTView.ContextMenuStrip = this.contextMenuStrip;
            this.editorTView.CopySelectionOnControlCUsesDragSource = false;
            this.editorTView.FullRowSelect = true;
            this.editorTView.GridLines = true;
            this.editorTView.HideSelection = false;
            this.editorTView.LabelWrap = false;
            this.editorTView.LargeImageList = this.imageList1;
            this.editorTView.Location = new System.Drawing.Point(3, 28);
            this.editorTView.MinimumSize = new System.Drawing.Size(200, 250);
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
            this.editorTView.Size = new System.Drawing.Size(996, 469);
            this.editorTView.SmallImageList = this.imageList1;
            this.editorTView.TabIndex = 4;
            this.editorTView.TriggerCellOverEventsWhenOverHeader = false;
            this.editorTView.UseAlternatingBackColors = true;
            this.editorTView.UseCellFormatEvents = true;
            this.editorTView.UseCompatibleStateImageBehavior = false;
            this.editorTView.UseHotControls = false;
            this.editorTView.View = System.Windows.Forms.View.Details;
            this.editorTView.VirtualMode = true;
            this.editorTView.Expanded += new System.EventHandler<BrightIdeasSoftware.TreeBranchExpandedEventArgs>(this.editorTView_Expanded);
            this.editorTView.Collapsed += new System.EventHandler<BrightIdeasSoftware.TreeBranchCollapsedEventArgs>(this.editorTView_Collapsed);
            this.editorTView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.editorTView_CellEditFinishing);
            this.editorTView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.editorTView_CellEditStarting);
            this.editorTView.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.editorTView_FormatCell);
            this.editorTView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.editorTView_ColumnWidthChanging);
            this.editorTView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.editorTView_ItemSelectionChanged);
            this.editorTView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editorTView_KeyDown);
            this.editorTView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.editorTView_MouseDoubleClick);
            this.editorTView.MouseEnter += new System.EventHandler(this.editorTView_MouseEnter);
            this.editorTView.MouseLeave += new System.EventHandler(this.editorTView_MouseLeave);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator5,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.toolStripSeparator6,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(226, 192);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.create;
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.createToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.createToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.createToolStripMenuItem.Text = "Создать";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.deleteToolStripMenuItem.Text = "Удалить";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(227, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.cut;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.cutToolStripMenuItem.Text = "Вырезать";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.copyToolStripMenuItem.Text = "Копировать";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.paste;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.pasteToolStripMenuItem.Text = "Вставить";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.replace;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.replaceToolStripMenuItem.Text = "Заменить";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(227, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.moveup;
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl + ↑";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.moveUpToolStripMenuItem.Text = "Переместить вверх";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Image = global::EasyEPlanner.Properties.Resources.movedown;
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl + ↓";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.moveDownToolStripMenuItem.Text = "Переместить вниз";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.toolStrip, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.toolSettingsStrip, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanelSearchBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(999, 29);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // toolSettingsStrip
            // 
            this.toolSettingsStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolSettingsStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolSettingsStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSettingDropDownButton});
            this.toolSettingsStrip.Location = new System.Drawing.Point(976, 0);
            this.toolSettingsStrip.Name = "toolSettingsStrip";
            this.toolSettingsStrip.ShowItemToolTips = false;
            this.toolSettingsStrip.Size = new System.Drawing.Size(23, 29);
            this.toolSettingsStrip.TabIndex = 3;
            this.toolSettingsStrip.Text = "toolStrip1";
            // 
            // toolSettingDropDownButton
            // 
            this.toolSettingDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSettingDropDownButton.DropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowLeft;
            this.toolSettingDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingMenuItem_expand,
            this.settingMenuItem_drawDev,
            this.settingMenuItem_edit,
            this.settingMenuItem_refresh,
            this.settingMenuItem_insert,
            this.settingMenuItem_delete,
            this.settingMenuItem_cut,
            this.settingMenuItem_copy,
            this.settingMenuItem_paste,
            this.settingMenuItem_replace,
            this.settingMenuItem_moveUp,
            this.settingMenuItem_moveDown,
            this.settingMenuItem_import,
            this.settingMenuItem_export,
            this.settingMenuItem_changeBaseObj,
            this.settingMenuItem_hideEmptyItems,
            this.settingMenuItem_search});
            this.toolSettingDropDownButton.Image = global::EasyEPlanner.Properties.Resources.toolSettings;
            this.toolSettingDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSettingDropDownButton.Name = "toolSettingDropDownButton";
            this.toolSettingDropDownButton.ShowDropDownArrow = false;
            this.toolSettingDropDownButton.Size = new System.Drawing.Size(20, 26);
            this.toolSettingDropDownButton.Text = "toolStripDropDownButton1";
            this.toolSettingDropDownButton.ToolTipText = "Настроить инструменты";
            // 
            // settingMenuItem_expand
            // 
            this.settingMenuItem_expand.Checked = true;
            this.settingMenuItem_expand.CheckOnClick = true;
            this.settingMenuItem_expand.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_expand.Image = global::EasyEPlanner.Properties.Resources.expand;
            this.settingMenuItem_expand.Name = "settingMenuItem_expand";
            this.settingMenuItem_expand.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_expand.Tag = this.expandDropDownList;
            this.settingMenuItem_expand.Text = "Уровень развертки";
            this.settingMenuItem_expand.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_drawDev
            // 
            this.settingMenuItem_drawDev.Checked = true;
            this.settingMenuItem_drawDev.CheckOnClick = true;
            this.settingMenuItem_drawDev.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_drawDev.Image = global::EasyEPlanner.Properties.Resources.highlight;
            this.settingMenuItem_drawDev.Name = "settingMenuItem_drawDev";
            this.settingMenuItem_drawDev.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_drawDev.Tag = this.drawDev_toolStripButton;
            this.settingMenuItem_drawDev.Text = "Подсветка устройств";
            this.settingMenuItem_drawDev.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_edit
            // 
            this.settingMenuItem_edit.Checked = true;
            this.settingMenuItem_edit.CheckOnClick = true;
            this.settingMenuItem_edit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_edit.Image = global::EasyEPlanner.Properties.Resources.edit;
            this.settingMenuItem_edit.Name = "settingMenuItem_edit";
            this.settingMenuItem_edit.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_edit.Tag = this.edit_toolStripButton;
            this.settingMenuItem_edit.Text = "Режим редактирования";
            this.settingMenuItem_edit.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_refresh
            // 
            this.settingMenuItem_refresh.Checked = true;
            this.settingMenuItem_refresh.CheckOnClick = true;
            this.settingMenuItem_refresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_refresh.Image = global::EasyEPlanner.Properties.Resources.refresh;
            this.settingMenuItem_refresh.Name = "settingMenuItem_refresh";
            this.settingMenuItem_refresh.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_refresh.Tag = this.refresh_toolStripButton;
            this.settingMenuItem_refresh.Text = "Синхронизация и сохранение";
            this.settingMenuItem_refresh.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_insert
            // 
            this.settingMenuItem_insert.Checked = true;
            this.settingMenuItem_insert.CheckOnClick = true;
            this.settingMenuItem_insert.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_insert.Image = global::EasyEPlanner.Properties.Resources.create;
            this.settingMenuItem_insert.Name = "settingMenuItem_insert";
            this.settingMenuItem_insert.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_insert.Tag = this.insertButton;
            this.settingMenuItem_insert.Text = "Создать";
            this.settingMenuItem_insert.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_delete
            // 
            this.settingMenuItem_delete.Checked = true;
            this.settingMenuItem_delete.CheckOnClick = true;
            this.settingMenuItem_delete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_delete.Image = global::EasyEPlanner.Properties.Resources.delete;
            this.settingMenuItem_delete.Name = "settingMenuItem_delete";
            this.settingMenuItem_delete.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_delete.Tag = this.deleteButton;
            this.settingMenuItem_delete.Text = "Удалить";
            this.settingMenuItem_delete.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_cut
            // 
            this.settingMenuItem_cut.Checked = true;
            this.settingMenuItem_cut.CheckOnClick = true;
            this.settingMenuItem_cut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_cut.Image = global::EasyEPlanner.Properties.Resources.cut;
            this.settingMenuItem_cut.Name = "settingMenuItem_cut";
            this.settingMenuItem_cut.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_cut.Tag = this.cutButton;
            this.settingMenuItem_cut.Text = "Вырезать";
            this.settingMenuItem_cut.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_copy
            // 
            this.settingMenuItem_copy.Checked = true;
            this.settingMenuItem_copy.CheckOnClick = true;
            this.settingMenuItem_copy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_copy.Image = global::EasyEPlanner.Properties.Resources.copy;
            this.settingMenuItem_copy.Name = "settingMenuItem_copy";
            this.settingMenuItem_copy.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_copy.Tag = this.copyButton;
            this.settingMenuItem_copy.Text = "Копировать";
            this.settingMenuItem_copy.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_paste
            // 
            this.settingMenuItem_paste.Checked = true;
            this.settingMenuItem_paste.CheckOnClick = true;
            this.settingMenuItem_paste.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_paste.Image = global::EasyEPlanner.Properties.Resources.paste;
            this.settingMenuItem_paste.Name = "settingMenuItem_paste";
            this.settingMenuItem_paste.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_paste.Tag = this.pasteButton;
            this.settingMenuItem_paste.Text = "Вставить";
            this.settingMenuItem_paste.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_replace
            // 
            this.settingMenuItem_replace.Checked = true;
            this.settingMenuItem_replace.CheckOnClick = true;
            this.settingMenuItem_replace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_replace.Image = global::EasyEPlanner.Properties.Resources.replace;
            this.settingMenuItem_replace.Name = "settingMenuItem_replace";
            this.settingMenuItem_replace.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_replace.Tag = this.replaceButton;
            this.settingMenuItem_replace.Text = "Заменить";
            this.settingMenuItem_replace.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_moveUp
            // 
            this.settingMenuItem_moveUp.Checked = true;
            this.settingMenuItem_moveUp.CheckOnClick = true;
            this.settingMenuItem_moveUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_moveUp.Image = global::EasyEPlanner.Properties.Resources.moveup;
            this.settingMenuItem_moveUp.Name = "settingMenuItem_moveUp";
            this.settingMenuItem_moveUp.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_moveUp.Tag = this.moveUpButton;
            this.settingMenuItem_moveUp.Text = "Переместить вверх";
            this.settingMenuItem_moveUp.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_moveDown
            // 
            this.settingMenuItem_moveDown.Checked = true;
            this.settingMenuItem_moveDown.CheckOnClick = true;
            this.settingMenuItem_moveDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_moveDown.Image = global::EasyEPlanner.Properties.Resources.movedown;
            this.settingMenuItem_moveDown.Name = "settingMenuItem_moveDown";
            this.settingMenuItem_moveDown.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_moveDown.Tag = this.moveDownButton;
            this.settingMenuItem_moveDown.Text = "Переместить вниз";
            this.settingMenuItem_moveDown.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_import
            // 
            this.settingMenuItem_import.Checked = true;
            this.settingMenuItem_import.CheckOnClick = true;
            this.settingMenuItem_import.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_import.Image = global::EasyEPlanner.Properties.Resources.import;
            this.settingMenuItem_import.Name = "settingMenuItem_import";
            this.settingMenuItem_import.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_import.Tag = this.importButton;
            this.settingMenuItem_import.Text = "Импорт объектов";
            this.settingMenuItem_import.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_export
            // 
            this.settingMenuItem_export.Checked = true;
            this.settingMenuItem_export.CheckOnClick = true;
            this.settingMenuItem_export.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_export.Image = global::EasyEPlanner.Properties.Resources.export;
            this.settingMenuItem_export.Name = "settingMenuItem_export";
            this.settingMenuItem_export.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_export.Tag = this.exportButton;
            this.settingMenuItem_export.Text = "Экспорт объектов";
            this.settingMenuItem_export.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_changeBaseObj
            // 
            this.settingMenuItem_changeBaseObj.Checked = true;
            this.settingMenuItem_changeBaseObj.CheckOnClick = true;
            this.settingMenuItem_changeBaseObj.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_changeBaseObj.Image = global::EasyEPlanner.Properties.Resources.changeObj;
            this.settingMenuItem_changeBaseObj.Name = "settingMenuItem_changeBaseObj";
            this.settingMenuItem_changeBaseObj.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_changeBaseObj.Tag = this.changeBasesObjBtn;
            this.settingMenuItem_changeBaseObj.Text = "Изменить базовый объект";
            this.settingMenuItem_changeBaseObj.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_hideEmptyItems
            // 
            this.settingMenuItem_hideEmptyItems.Checked = true;
            this.settingMenuItem_hideEmptyItems.CheckOnClick = true;
            this.settingMenuItem_hideEmptyItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_hideEmptyItems.Image = global::EasyEPlanner.Properties.Resources.hideEmptyItems;
            this.settingMenuItem_hideEmptyItems.Name = "settingMenuItem_hideEmptyItems";
            this.settingMenuItem_hideEmptyItems.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_hideEmptyItems.Tag = this.hideEmptyItemsBtn;
            this.settingMenuItem_hideEmptyItems.Text = "Скрыть пустые элементы";
            this.settingMenuItem_hideEmptyItems.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // settingMenuItem_search
            // 
            this.settingMenuItem_search.Checked = true;
            this.settingMenuItem_search.CheckOnClick = true;
            this.settingMenuItem_search.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingMenuItem_search.Image = global::EasyEPlanner.Properties.Resources.search;
            this.settingMenuItem_search.Name = "settingMenuItem_search";
            this.settingMenuItem_search.Size = new System.Drawing.Size(237, 22);
            this.settingMenuItem_search.Tag = this.tableLayoutPanelSearchBox;
            this.settingMenuItem_search.Text = "Поиск";
            this.settingMenuItem_search.Click += new System.EventHandler(this.toolSettingItem_Click);
            // 
            // tableLayoutPanelSearchBox
            // 
            this.tableLayoutPanelSearchBox.AutoSize = true;
            this.tableLayoutPanelSearchBox.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanelSearchBox.ColumnCount = 3;
            this.tableLayoutPanelSearchBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanelSearchBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSearchBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelSearchBox.Controls.Add(this.formatNumericUpDown_SearchSelectedItem, 2, 0);
            this.tableLayoutPanelSearchBox.Controls.Add(this.textBox_search, 1, 0);
            this.tableLayoutPanelSearchBox.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelSearchBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tableLayoutPanelSearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSearchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanelSearchBox.Location = new System.Drawing.Point(722, 3);
            this.tableLayoutPanelSearchBox.Name = "tableLayoutPanelSearchBox";
            this.tableLayoutPanelSearchBox.RowCount = 1;
            this.tableLayoutPanelSearchBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSearchBox.Size = new System.Drawing.Size(251, 23);
            this.tableLayoutPanelSearchBox.TabIndex = 4;
            this.tableLayoutPanelSearchBox.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanelSearchBox_Paint);
            this.tableLayoutPanelSearchBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanelSearchBox_MouseClick);
            // 
            // formatNumericUpDown_SearchSelectedItem
            // 
            this.formatNumericUpDown_SearchSelectedItem.BackColor = System.Drawing.Color.White;
            this.formatNumericUpDown_SearchSelectedItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.formatNumericUpDown_SearchSelectedItem.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.formatNumericUpDown_SearchSelectedItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatNumericUpDown_SearchSelectedItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formatNumericUpDown_SearchSelectedItem.Location = new System.Drawing.Point(182, 5);
            this.formatNumericUpDown_SearchSelectedItem.Margin = new System.Windows.Forms.Padding(1, 5, 1, 5);
            this.formatNumericUpDown_SearchSelectedItem.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.formatNumericUpDown_SearchSelectedItem.Name = "formatNumericUpDown_SearchSelectedItem";
            this.formatNumericUpDown_SearchSelectedItem.ReadOnly = true;
            this.formatNumericUpDown_SearchSelectedItem.Size = new System.Drawing.Size(68, 16);
            this.formatNumericUpDown_SearchSelectedItem.TabIndex = 6;
            this.formatNumericUpDown_SearchSelectedItem.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.formatNumericUpDown_SearchSelectedItem.ValueChanged += new System.EventHandler(this.formatNumericUpDown_SearchSelectedItem_ValueChanged);
            // 
            // textBox_search
            // 
            this.textBox_search.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_search.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox_search.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_search.ForeColor = System.Drawing.Color.Gray;
            this.textBox_search.Location = new System.Drawing.Point(20, 5);
            this.textBox_search.Margin = new System.Windows.Forms.Padding(1, 5, 1, 5);
            this.textBox_search.MaximumSize = new System.Drawing.Size(160, 0);
            this.textBox_search.MinimumSize = new System.Drawing.Size(160, 0);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(160, 13);
            this.textBox_search.TabIndex = 0;
            this.textBox_search.Text = "Поиск...";
            this.textBox_search.TextChanged += new System.EventHandler(this.textBox_search_TextChanged);
            this.textBox_search.GotFocus += new System.EventHandler(this.textBox_search_Enter);
            this.textBox_search.LostFocus += new System.EventHandler(this.textBox_search_Leave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.ErrorImage = global::EasyEPlanner.Properties.Resources.hideEmptyItems;
            this.pictureBox1.Image = global::EasyEPlanner.Properties.Resources.search;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(3, 5);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 5, 2, 4);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(14, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 14);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanelSearchBox_MouseClick);
            // 
            // NewEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.editorTView);
            this.Name = "NewEditorControl";
            this.Size = new System.Drawing.Size(999, 500);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorTView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.toolSettingsStrip.ResumeLayout(false);
            this.toolSettingsStrip.PerformLayout();
            this.tableLayoutPanelSearchBox.ResumeLayout(false);
            this.tableLayoutPanelSearchBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formatNumericUpDown_SearchSelectedItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

            }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator_1;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton importButton;
        private System.Windows.Forms.ToolStripButton exportButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton hideEmptyItemsBtn;
        private System.Windows.Forms.ToolStripButton changeBasesObjBtn;
        private System.Windows.Forms.ToolStripButton cutButton;
        private System.Windows.Forms.ToolStripDropDownButton expandDropDownList;
        private System.Windows.Forms.ToolStripMenuItem expandButton2;
        private System.Windows.Forms.ToolStripMenuItem expandButton3;
        private System.Windows.Forms.ToolStripMenuItem expandButton4;
        private System.Windows.Forms.ToolStripMenuItem expandButton5;
        private System.Windows.Forms.ToolStripMenuItem expandButton1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ToolStrip toolSettingsStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolSettingDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_expand;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_drawDev;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_edit;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_refresh;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_insert;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_delete;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_cut;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_copy;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_paste;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_replace;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_import;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_export;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_changeBaseObj;
        private System.Windows.Forms.ToolStripMenuItem settingMenuItem_hideEmptyItems;
        private ToolStripButton moveDownButton;
        private ToolStripButton moveUpButton;
        private ToolStripMenuItem settingMenuItem_moveUp;
        private ToolStripMenuItem settingMenuItem_moveDown;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem replaceToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private TableLayoutPanel tableLayoutPanelSearchBox;
        public TextBox textBox_search;
        private PictureBox pictureBox1;
        private ToolStripMenuItem settingMenuItem_search;
        private EditorControls.FormatNumericUpDown formatNumericUpDown_SearchSelectedItem;
    }
    }
