using System;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EasyEPlanner;
using PInvoke;
using BrightIdeasSoftware;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Threading;
using static System.Windows.Forms.LinkLabel;
using TechObject;

namespace Editor
{
    [ExcludeFromCodeCoverage]
    public partial class NewEditorControl : UserControl
    {
        public NewEditorControl()
        {
            InitializeComponent();
            InitObjectListView();
            SetUpToolsHideFromConfig();

            dialogCallbackDelegate = new PI.HookProc(DlgWndHookCallbackFunction);
            mainWndKeyboardCallbackDelegate =
                new PI.LowLevelKeyboardProc(GlobalHookKeyboardCallbackFunction);

            //Фильтр
            editorTView.ModelFilter = new ModelFilter((obj)
                => (obj as ITreeViewItem).Filter(searchText, hideEmptyItemsBtn.Checked));

            wasInit = false;
        }

        /// <summary>
        /// Инициализация ObjectListView
        /// </summary>
        private void InitObjectListView()
        {
            // Настройка цвета отключенного компонента в дереве
            var disabletItemStyle = new SimpleItemStyle();
            disabletItemStyle.ForeColor = Color.Gray;
            editorTView.DisabledItemStyle = disabletItemStyle;

            // Текст подсветки чередующихся строк
            editorTView.AlternateRowBackColor = Color.FromArgb(250, 250, 250);

            // Получение текста для отображения
            editorTView.CellToolTipGetter =
                delegate (OLVColumn column, object displayingObject)
                {
                    if (displayingObject is ITreeViewItem)
                    {
                        var obj = displayingObject as ITreeViewItem;
                        switch (column.Index)
                        {
                            case 0:
                                return obj.DisplayText[0];
                            case 1:
                                return obj.DisplayText[1];
                        }
                    }

                    return null;
                };

            // Делегат для Expand
            editorTView.CanExpandGetter =
                delegate (object expandingObject)
                {
                    var objTreeViewItem = expandingObject as ITreeViewItem;
                    if (objTreeViewItem != null)
                    {
                        if (objTreeViewItem.Items != null)
                        {
                            return objTreeViewItem.Items.Length > 0;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                };

            // делегат получения отрисовщика для ячейки
            editorTView.CellRendererGetter =
                (object rowObject, OLVColumn column) =>
                {
                    return (rowObject as ITreeViewItem)?.CellRenderer.ElementAtOrDefault(column.Index);
                };

            // Делегат получения дочерних элементов
            editorTView.ChildrenGetter = obj => (obj as ITreeViewItem).Items;

            // Настройка и добавление колонок
            var firstColumn = new OLVColumn("Название", "DisplayText[0]");
            firstColumn.IsEditable = false;
            firstColumn.AspectGetter = obj => (obj as ITreeViewItem).DisplayText[0];
            firstColumn.Sortable = false;
            firstColumn.ImageGetter =
                delegate (object obj)
                {
                    var objTreeViewItem = obj as ITreeViewItem;
                    int countOfImages = editorTView.SmallImageList.Images.Count;
                    if ((int)objTreeViewItem.ImageIndex < countOfImages)
                    {
                        return (int)objTreeViewItem.ImageIndex;
                    }
                    else
                    {
                        return null;
                    }
                };

            var secondColumn = new OLVColumn("Описание", "DisplayText[1]");
            secondColumn.IsEditable = false;
            secondColumn.AspectGetter = obj => (obj as ITreeViewItem).DisplayText[1];
            secondColumn.Sortable = false;

            editorTView.Columns.Add(firstColumn);
            editorTView.Columns.Add(secondColumn);
        }

        /// <summary>
        /// Получение активного элемента дерева.
        /// </summary>
        /// <returns>Активный элемент дерева.</returns>
        public ITreeViewItem GetActiveItem()
        {
            try
            {
                return editorTView.SelectedObjects.Cast<ITreeViewItem>().SingleOrDefault();
            }
            catch { return null; }
        }

        /// <summary>
        /// Получение активных элементов дерева.
        /// </summary>
        /// <returns>Активный элемент дерева.</returns>
        public List<ITreeViewItem> GetActiveItems()
        {
            return editorTView.SelectedObjects.Cast<ITreeViewItem>().ToList();
        }

        /// <summary>
        /// Инициализация ComboBox редактора
        /// </summary>
        private void InitComboBoxCellEditor(List<string> data)
        {
            comboBoxCellEditor = new ComboBox();
            comboBoxCellEditor.Items.AddRange(data.ToArray());
            comboBoxCellEditor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCellEditor.Sorted = true;
            comboBoxCellEditor.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBoxCellEditor.AutoCompleteMode = AutoCompleteMode.Append;
            comboBoxCellEditor.Enabled = true;
            comboBoxCellEditor.Visible = true;
            comboBoxCellEditor.LostFocus += editorTView_LostFocus;
            editorTView.Controls.Add(comboBoxCellEditor);
        }

        /// <summary>
        /// Инициализация TextBox редактора
        /// </summary>
        private void InitTextBoxCellEditor()
        {
            textBoxCellEditor = new TextBox();
            textBoxCellEditor.Enabled = true;
            textBoxCellEditor.Visible = true;
            textBoxCellEditor.LostFocus += editorTView_LostFocus;
            editorTView.Controls.Add(textBoxCellEditor);
        }

        /// <summary>
        /// Обновление формы с данными для редактирования.
        /// </summary>
        public void RefreshTree()
        {
            if (editorTView.Items.Count == 0)
            {
                editorTView.BeginUpdate();
                editorTView.ClearObjects();
                editorTView.EndUpdate();
            }
            else
            {
                editorTView.RefreshObjects(treeViewItemsList);
            }
        }

        /// <summary>
        /// Инициализация формы данными для редактирования.
        /// </summary>
        /// <param name="data"> Данные для отображения.</param>
        public void Init(ITreeViewItem data)
        {
            editorTView.BeginUpdate();
            treeViewItemsList = new List<ITreeViewItem>();
            treeViewItemsList.Add(data);
            editorTView.Roots = treeViewItemsList;
            editorTView.Columns[0]
                .AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            editorTView.Columns[0].Width = 200;
            editorTView.Columns[1].Width = editorTView.Width -
                editorTView.Columns[0].Width - deltaWidth;
            if (treeViewItemsList.Count > 0)
            {
                editorTView.Expand(data);
                editorTView.SelectedIndex = 0;
                editorTView.SelectedItem.EnsureVisible();
            }
            editorTView.EndUpdate();

            wasInit = true;
        }

        /// <summary>
        /// Проверка текущего состояния окон
        /// </summary>
        public static void CheckShown()
        {
            if (PI.IsWindowVisible(wndEditVisiblePtr) == true)
            {
                editIsShown = true;
            }
            else
            {
                editIsShown = false;
            }
        }

        /// <summary>
        /// Сохранение конфигурации окон
        /// </summary>
        public static void SaveCfg()
        {
            string path = Environment.GetFolderPath(
                            Environment.SpecialFolder.ApplicationData);

            IniFile ini = new IniFile(path + @"\Eplan\eplan.cfg");
            if (editIsShown)
            {
                ini.WriteString("main", "show_obj_window", "true");
            }
            else
            {
                ini.WriteString("main", "show_obj_window", "false");
            }
        }

        // Для ручного управления конфигурацией (через код)
        public static void SaveCfg(bool wndState)
        {
            string path = Environment.GetFolderPath(
                            Environment.SpecialFolder.ApplicationData);

            IniFile ini = new IniFile(path + @"\Eplan\eplan.cfg");

            if (wndState)
            {
                ini.WriteString("main", "show_obj_window", "true");
            }
            else
            {
                ini.WriteString("main", "show_obj_window", "false");
            }
        }

        private PI.HookProc dialogCallbackDelegate = null;
        private IntPtr dialogHookPtr = IntPtr.Zero;
        private IntPtr dialogHandle = IntPtr.Zero;

        private IntPtr panelPtr = IntPtr.Zero;

        private PI.LowLevelKeyboardProc mainWndKeyboardCallbackDelegate = null;

        private IntPtr globalKeyboardHookPtr = IntPtr.Zero;

        public bool isEplanClosing = false;

        public bool wasShown;
        private bool cancelChanges = false;

        //Ширина, которую нужно учитывать при форматировании колонок.
        private const int deltaWidth = 5;

        private IntPtr GlobalHookKeyboardCallbackFunction(int code,
            PI.WM wParam, PI.KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0 || editorTView == null)
            {
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            if (!(IsCellEditing || editorTView.Focused))
            {
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            uint c = (uint)lParam.vkCode;
            //Отпускание клавиш - если активно окно редактора, то не пускаем
            //дальше.
            if (wParam == PI.WM.KEYUP || wParam == PI.WM.CHAR)
            {
                if (c == PI.VIRTUAL_KEY.VK_DELETE ||
                    ((c == (uint)Keys.C ||
                    c == (uint)Keys.V ||
                    c == (uint)Keys.A ||
                    c == (uint)Keys.X ||
                    c == (uint)Keys.B) &&
                    PI.GetKeyState((int)PI.VIRTUAL_KEY.VK_CONTROL) != -0))
                {
                    if (IsCellEditing || editorTView.Focused)
                    {
                        return (IntPtr)1;
                    }
                }
            }

            const short SHIFTED = 0x80;

            //Нажатие клавиш - если активно окно редактора, то обрабатываем и
            //не пускаем дальше.
            if (wParam == PI.WM.KEYDOWN)
            {
                switch (c)
                {
                    case PI.VIRTUAL_KEY.VK_ESCAPE:                       //Esc
                        if (IsCellEditing)
                        {
                            cancelChanges = true;
                            editorTView.FinishCellEdit();
                            return (IntPtr)1;
                        }

                        if (editorTView.Focused)
                        {
                            PI.SendMessage(PI.GetFocus(),
                                (int)PI.WM.KEYDOWN,
                                (int)Keys.Escape, 0);

                            return (IntPtr)1;
                        }

                        break;

                    case PI.VIRTUAL_KEY.VK_RETURN:                       //Enter
                        if (IsCellEditing)
                        {
                            editorTView.FinishCellEdit();
                            return (IntPtr)1;
                        }
                        break;

                    case (uint)Keys.C:                                   //C
                        short ctrlState = PI.GetKeyState(
                            (int)PI.VIRTUAL_KEY.VK_CONTROL);
                        if ((ctrlState & SHIFTED) > 0)
                        {
                            if (IsCellEditing)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.COPY, 0, 0);

                                return (IntPtr)1;
                            }

                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.C, 0);

                                return (IntPtr)1;
                            }
                        }
                        break;

                    case (uint)Keys.X:                                  //X
                        ctrlState = PI.GetKeyState((int)
                            PI.VIRTUAL_KEY.VK_CONTROL);
                        if ((ctrlState & SHIFTED) > 0)
                        {
                            if (IsCellEditing)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.CUT, 0, 0);
                                return (IntPtr)1;
                            }

                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.X, 0);

                                return (IntPtr)1;
                            }
                        }
                        break;

                    case (uint)Keys.V:                                   //V
                        ctrlState = PI.GetKeyState(
                            (int)PI.VIRTUAL_KEY.VK_CONTROL);
                        if ((ctrlState & SHIFTED) > 0)
                        {
                            if (IsCellEditing)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.PASTE, 0, 0);
                                return (IntPtr)1;
                            }

                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.V, 0);
                                return (IntPtr)1;
                            }
                        }
                        break;

                    case (uint)Keys.B:                                   //B
                        ctrlState = PI.GetKeyState(
                            (int)PI.VIRTUAL_KEY.VK_CONTROL);
                        if ((ctrlState & SHIFTED) > 0)
                        {
                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.B, 0);
                                return (IntPtr)1;
                            }
                        }
                        break;

                    case (uint)Keys.Delete:                             //Delete
                        if (IsCellEditing || editorTView.Focused)
                        {
                            PI.SendMessage(PI.GetFocus(), (int)PI.WM.KEYDOWN,
                                (int)PI.VIRTUAL_KEY.VK_DELETE, 0);
                            return (IntPtr)1;
                        }

                        break;

                    case (uint)Keys.Insert:                             //Insert
                    case PI.VIRTUAL_KEY.VK_UP:                          //Up
                    case PI.VIRTUAL_KEY.VK_DOWN:                        //Down
                    case PI.VIRTUAL_KEY.VK_LEFT:                        //Left
                    case PI.VIRTUAL_KEY.VK_RIGHT:                       //Right
                    case PI.VIRTUAL_KEY.VK_F1:
                        PI.SendMessage(PI.GetFocus(),
                            (int)PI.WM.KEYDOWN, (int)c, 0);
                        return (IntPtr)1;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        /// <summary>
        /// Функция для обработки завершения работы окна редактора.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void CloseEditor()
        {
            cancelChanges = true;
            editorTView.FinishCellEdit();
            editorTView.ClearObjects();

            if (Editable)
            {
                edit_toolStripButton_Click(this, new EventArgs());
            }

            PI.UnhookWindowsHookEx(dialogHookPtr);
            PI.UnhookWindowsHookEx(globalKeyboardHookPtr);

            globalKeyboardHookPtr = IntPtr.Zero;

            PI.SetParent(editorTView.Handle, this.Handle);
            PI.SetParent(tableLayoutPanel.Handle, this.Handle);
            PI.SetParent(toolSettingsStrip.Handle, this.Handle);
            PI.SetParent(toolStrip.Handle, this.Handle);

            drawDev_toolStripButton.Checked = false;

            bool isClosingProject = true;
            ProjectManager.GetInstance().RemoveHighLighting(isClosingProject);

            System.Threading.Thread.Sleep(1);

            IsShown = false;
            editIsShown = false;
            wasInit = false;
        }

        private IntPtr DlgWndHookCallbackFunction(int code, IntPtr wParam, IntPtr lParam)
        {
            PI.CWPSTRUCT msg =
                (PI.CWPSTRUCT)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(PI.CWPSTRUCT));

            if (msg.hwnd == panelPtr)
            {
                switch (msg.message)
                {
                    case (int)PI.WM.MOVE:
                    case (int)PI.WM.SIZE:
                        ChangeUISize();
                        break;
                }
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        /// <summary>
        /// Изменить размер UI.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void ChangeUISize()
        {
            IntPtr dialogPtr = PI.GetParent(editorTView.Handle);

            PI.RECT rctDialog;
            PI.GetWindowRect(dialogPtr, out rctDialog);

            int w = rctDialog.Right - rctDialog.Left;
            int h = rctDialog.Bottom - rctDialog.Top;

            toolStrip.Location = new Point(0, 0);
            editorTView.Location = new Point(0, toolStrip.Height);

            tableLayoutPanel.Width = w;
            editorTView.Width = w;
            editorTView.Height = h - toolStrip.Height;

            tableLayoutPanelSearchBox.Refresh();
        }

        public static bool editIsShown = false; //Показано ли окно.
        public bool IsShown = false; // Костыль, без этого не работает IsShown()
        public static IntPtr wndEditVisiblePtr; // Дескриптор редактора.

        /// <summary>
        /// Показать диалог (окно с редактором).
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void ShowDlg()
        {
            Process currentProcess = Process.GetCurrentProcess();

            // Идентификатор команды вызова окна "Навигатор комментариев"
            const int wndWmCommand = 35381;
            string windowName = "Комментарий";

            if (editIsShown == true && IsShown == true)
            {
                StaticHelper.GUIHelper.ShowHiddenWindow(currentProcess,
                    wndEditVisiblePtr, wndWmCommand);
                return;
            }

            wasShown = true;

            StaticHelper.GUIHelper.SearchWindowDescriptor(currentProcess,
                windowName, wndWmCommand, ref dialogHandle,
                ref wndEditVisiblePtr);
            if (wndEditVisiblePtr != IntPtr.Zero)
            {
                StaticHelper.GUIHelper.ShowHiddenWindow(currentProcess,
                    wndEditVisiblePtr, wndWmCommand);
                StaticHelper.GUIHelper.ChangeWindowMainPanels(ref dialogHandle,
                    ref panelPtr);

                Controls.Clear();

                // Переносим на найденное окно свои элементы (SetParent) и
                // подгоняем их размеры и позицию.
                PI.SetParent(editorTView.Handle, dialogHandle);
                PI.SetParent(tableLayoutPanel.Handle, dialogHandle);
                PI.SetParent(toolSettingsStrip.Handle, dialogHandle);
                PI.SetParent(toolStrip.Handle, dialogHandle);
                ChangeUISize();

                // Устанавливаем свой хук для найденного окна
                // (для изменения размеров своих элементов, сохранения
                // изменений при закрытии и отключения хука).
                SetUpHook();

                editIsShown = true;
                IsShown = true;
                cancelChanges = false;

                DisableNeededObjects(treeViewItemsList.ToArray());
            }
        }

        /// <summary>
        /// Устанавливаем хук для найденного окна.
        /// </summary>
        private void SetUpHook()
        {
            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);
            globalKeyboardHookPtr = PI.SetWindowsHookEx(
                PI.HookType.WH_KEYBOARD_LL, mainWndKeyboardCallbackDelegate,
                IntPtr.Zero, 0);

            if (globalKeyboardHookPtr == IntPtr.Zero)
            {
                MessageBox.Show("Ошибка! Не удалось переназначить клавиши!");
            }
        }

        public uint pidMain = 0;

        /// <summary>
        /// Обработка изменений при редактировании.
        /// </summary>
        private void HiglihtItems()
        {
            var drawInfoList = new List<DrawInfo>();

            if (drawDev_toolStripButton.Checked)
            {
                //Отображение (подсветка) участвующих в режиме устройств на карте
                //Eplan'a.
                foreach (var itemDraw in GetActiveItems())
                {
                    if (drawDev_toolStripButton.Checked is false)
                        continue;
                        
                    ProjectManager.GetInstance().RemoveHighLighting();
                    if (itemDraw.IsDrawOnEplanPage)
                        drawInfoList.AddRange(itemDraw.GetObjectToDrawOnEplanPage());
                }

                ProjectManager.GetInstance().RemoveHighLighting();
                ProjectManager.GetInstance().SetHighLighting(drawInfoList);
            }
        }

        /// <summary>
        /// Скопированные объекты
        /// </summary>
        private object[] copyItems = null;

        private bool noOnChange = default;

        /// <summary>
        /// Обработка нажатий клавиш клавиатуры.
        /// </summary>
        private void editorTView_KeyDown(object sender, KeyEventArgs e)
        {
            var items = GetActiveItems();
            var item = GetActiveItem();

            bool singleSelection = item != null;

            switch (e.KeyCode)
            {
                // Перемещение элемента вверх
                case Keys.Up when e.Control && singleSelection:
                    if (Editable)
                        MoveUpItem(item);
                    break;

                // Перемещение элемента вниз
                case Keys.Down when e.Control && singleSelection:
                    if (Editable)
                        MoveDownItem(item);
                    break;

                // Выделение элементов
                case Keys.Up when e.Shift:
                    if (items.FirstOrDefault() == editorTView.FocusedObject && items.Count > 1)
                    {
                        editorTView.SelectObjects(items.Take(items.Count - 1).ToList());
                    }
                    else
                    {
                        editorTView.SelectObject(items.FirstOrDefault());
                        editorTView.SelectedIndex--;
                        editorTView.SelectObjects(new[] { editorTView.SelectedObject as ITreeViewItem }.Concat(items).ToList());
                    }
                    break;

                // Выделение элементов
                case Keys.Down when e.Shift:
                    if (items.LastOrDefault() == editorTView.FocusedObject && items.Count > 1)
                    {
                        editorTView.SelectObjects(items.Skip(1).ToList());
                    }
                    else
                    {
                        editorTView.SelectObject(items.LastOrDefault());
                        editorTView.SelectedIndex++;
                        editorTView.SelectObjects(items.Concat(new[] { editorTView.SelectedObject as ITreeViewItem }).ToArray());
                    }
                    break;

                // Переход к предыдущему элементу
                case Keys.Up:
                    if (singleSelection is false)
                        editorTView.SelectObject(items.FirstOrDefault());
                    if (items.Count == 0)
                        editorTView.SelectedIndex = editorTView.Items.Count - 1;
                    else
                        editorTView.SelectedIndex--;
                    editorTView.FocusedObject = editorTView.SelectedObject;
                    break;
                
                //// Переходу к следующему элементу
                case Keys.Down:
                    if (singleSelection is false)
                        editorTView.SelectObject(items.LastOrDefault());
                    if (items.Count == 0)
                        editorTView.SelectedIndex = 0;
                    else
                        editorTView.SelectedIndex++;
                    editorTView.FocusedObject = editorTView.SelectedObject;
                    break;

                // Копирование
                case Keys.C when e.Control && Editable:
                    CopyItem(items);
                    break;

                // Вставка
                case Keys.V when e.Control && Editable && singleSelection:
                    PasteItem(item);
                    break;

                // Вырезать
                case Keys.X when e.Control && Editable:
                    CutItem(items);
                    break;

                // Замена
                case Keys.B when e.Control && Editable && singleSelection:
                    ReplaceItem(item);
                    break;

                // Создание нового элемента
                case Keys.Insert when Editable && singleSelection:
                    CreateItem(item);
                    break;

                // Удаление
                case Keys.Delete when Editable:
                    DeleteItems(items);
                    break;

                // Отмена вырезки
                case Keys.Escape when Editable:
                    CancelCut(copyItems);
                    break;

                // Окно справки по элементам
                case Keys.F1 when singleSelection:
                    if (item is IHelperItem itemHelper)
                    {
                        string link = itemHelper.GetLinkToHelpPage();
                        if (link == null)
                        {
                            link = ProjectManager.GetInstance().GetOstisHelpSystemMainPageLink();
                            if (link == null)
                            {
                                MessageBox.Show("Ошибка поиска адреса системы помощи");
                                return;
                            }
                        }
                        Process.Start(link);
                    }
                    break;

                default:
                    return; // exit withot handled
            }

            e.Handled = true;
            return;
        }

        /// <summary>
        /// Скопировать элемент
        /// </summary>
        /// <param name="item">Копируемый элемент</param>
        private void CopyItem(List<ITreeViewItem> items)
        {
            CancelCut(copyItems);

            var copies = new List<object>();
            foreach (var item in items)
            {
                copies.Add(item.Copy());
            }

            copyItems = copies.ToArray();
        }

        /// <summary>
        /// Создать элемент (в выделенной точке)
        /// </summary>
        /// <param name="item">Элемент в котором создается новый элемент</param>
        [ExcludeFromCodeCoverage]
        private void CreateItem(ITreeViewItem item)
        {
            if (item is TechObject.TechObject)
                editorTView.Collapse(item);

            if (item.IsInsertable == true)
            {
                ITreeViewItem newItem = item.Insert();
                if (newItem != null)
                {
                    editorTView.RefreshObjects(item.Items);
                    editorTView.RefreshObject(item);
                    if (item.NeedRebuildParent && item.Parent != null)
                    {
                        editorTView.RefreshObject(item.Parent);
                    }

                    HiglihtItems();
                }
            }
        }

        /// <summary>
        /// Удалить элемент
        /// </summary>
        /// <param name="item">Удаляемый элемент</param>
        private void DeleteItems(List<ITreeViewItem> items)
        {
            bool permissionToDelete = false;
            bool isDelete = false;
            bool itemsDeleted = false;

            editorTView.SelectObject(items.FirstOrDefault(), true);

            foreach (var item in items)
            {
                if (item.IsDeletable is false)
                    continue;
                
                DialogResult showWarningResult;
                if (item.ShowWarningBeforeDelete && permissionToDelete is false)
                {
                    string message = "Вы действительно хотите удалить " +
                        "выделенный элемент?";
                    showWarningResult = MessageBox.Show(message, "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (showWarningResult == DialogResult.No)
                    {
                        return;
                    }

                    permissionToDelete = true;
                }

                ITreeViewItem parent = item.Parent;
                isDelete = parent.Delete(item);
                itemsDeleted |= isDelete;

                if (isDelete is false)
                {
                    editorTView.RefreshObject(item);
                    if (item.Items != null)
                    {
                        editorTView.RefreshObjects(item.Items);
                    }
                    
                }
            }
            if (itemsDeleted)
            {
                RefreshTree();
            }

            HiglihtItems();
        }

        /// <summary>
        /// Вставить элемент (Ctrl + V)
        /// </summary>
        /// <param name="item">Вставляемый элемент</param>
        private void PasteItem(ITreeViewItem item)
        {
            if (item.IsInsertableCopy is false || (!copyItems?.Any() ?? true))
                return;

            bool itemsHasMarkToCut = false;

            foreach (var copyItem in copyItems)
            {
                ITreeViewItem newItem = item.InsertCopy(copyItem);
                if (newItem != null)
                {
                    var copiedItem = copyItem as ITreeViewItem;
                    if (copiedItem != null && copiedItem.MarkToCut)
                    {
                        copiedItem.MarkToCut = false;
                        itemsHasMarkToCut = true;
                    }

                    HiglihtItems();
                    RefreshTree();
                    DisableNeededObjects(newItem.Parent.Items);
                }
            }

            if (itemsHasMarkToCut)
                copyItems = null;
        }

        /// <summary>
        /// Заменить элемент (Ctrl + B)
        /// </summary>
        /// <param name="item">Заменяемый элемент</param>
        private void ReplaceItem(ITreeViewItem item)
        {
            var copiedItems = copyItems.Cast<ITreeViewItem>();
            ITreeViewItem copiedItem = null;
            try
            {
                copiedItem = copiedItems.SingleOrDefault();
            }
            catch
            {
                return;
            }

            bool copiedItemIsCorrect =
                copiedItem != null && !copiedItem.MarkToCut;
            if (copiedItemIsCorrect && item.IsReplaceable)
            {
                ITreeViewItem parent = item.Parent;
                if (parent != null)
                {
                    ITreeViewItem newItem = parent.Replace(item, copiedItem);
                    if (newItem != null)
                    {
                        RefreshTree();
                        DisableNeededObjects(treeViewItemsList.ToArray());
                    }
                    HiglihtItems();
                }
            }
        }

        /// <summary>
        /// Передвинуть элемент вверх (Shift + KeyUp)
        /// </summary>
        /// <param name="item">Передвигаемый элемент</param>
        private void MoveUpItem(ITreeViewItem item)
        {
            if (item.IsMoveable)
            {
                ITreeViewItem itemParent = item.Parent;
                ITreeViewItem isMove = itemParent.MoveUp(item);
                if (isMove != null) // Если перемещенный объект не null
                {
                    editorTView.RefreshObjects(itemParent.Items);
                }
                HiglihtItems();
            }
            editorTView.SelectObject(item, true);
        }

        /// <summary>
        /// Передвинуть элемент вниз (Shift + KeyDown)
        /// </summary>
        /// <param name="item">Передвигаемый элемент</param>
        private void MoveDownItem(ITreeViewItem item)
        {
            if (item.IsMoveable)
            {
                ITreeViewItem itemParent = item.Parent;
                ITreeViewItem isMove = itemParent.MoveDown(item);
                if (isMove != null) // Если перемещенный объект не null
                {       
                    editorTView.RefreshObjects(itemParent.Items);
                }
                HiglihtItems();
            }
            editorTView.SelectObject(item, true);
        }

        /// <summary>
        /// Вырезать элемент (Ctrl + X)
        /// </summary>
        /// <param name="item"></param>
        private void CutItem(List<ITreeViewItem> items)
        {
            if (items is null || items.Count <= 0)
                return;

            var parent = items.FirstOrDefault()?.Parent;
            if (items.TrueForAll(item => item.Parent == parent) is false)
                return;

            if (parent.IsCuttable is false)
                return;

            CancelCut(copyItems);

            var cutted = new List<ITreeViewItem>();
            foreach (var item in items)
            {
                if (item.IsMainObject)
                {
                    cutted.Add(item);
                    item.MarkToCut = true;
                    editorTView.RefreshObject(item);
                }
            }

            copyItems = cutted.ToArray();
        }

        /// <summary>
        /// Отменить вырезку объектов.
        /// </summary>
        /// <param name="item"></param>
        private void CancelCut(object[] objects)
        {
            var items = objects?.Cast<ITreeViewItem>().ToList();

            if (items is null)
                return;

            foreach (var item in items)
            {
                if (item is null)
                    continue;

                item.MarkToCut = false;
            }

            editorTView.RefreshObjects(items);
            copyItems = null;
        }

        /// <summary>
        /// Обработка нажатий кнопок уровня.
        /// </summary>
        private void toolStripButton_Click(object sender, EventArgs e)
        {
            editorTView.BeginUpdate();
            int level = Convert.ToInt32((sender as ToolStripMenuItem).Tag);
            editorTView.SelectedIndex = 0;
            editorTView.CollapseAll();
            ExpandToLevel(level, editorTView.Objects);
            editorTView.EnsureModelVisible(editorTView.SelectedObject);
            editorTView.EndUpdate();
        }

        /// <summary>
        /// Функции установки нового значения в виде строки в редакторе.
        /// </summary>
        /// <param name="newVal">Новое значение</param>
        internal void SetNewVal(string newVal)
        {
            bool isModified = (editorTView.SelectedObject as ITreeViewItem)
                .SetNewValue(newVal);

            if (isModified)
            {
                ITreeViewItem item = editorTView.SelectedObject as ITreeViewItem;
                noOnChange = true;
                editorTView.RefreshObject(item);
                noOnChange = false;
                HiglihtItems();
            }
        }

        /// <summary>
        /// Функция установки нового значения ограничений в редакторе.
        /// Ограничения передаются в виде словаря.
        /// </summary>
        /// <param name="dict">Словарь ограничений</param>
        internal void SetNewVal(IDictionary<int, List<int>> dict)
        {
            bool isModified = (editorTView.SelectedObject as ITreeViewItem)
                .SetNewValue(dict);

            if (isModified)
            {
                ITreeViewItem item = editorTView.SelectedObject as ITreeViewItem;
                noOnChange = true;
                editorTView.RefreshObject(item.Parent);
                DisableNeededObjects(item.Parent.Items);
                noOnChange = false;
                HiglihtItems();
            }
        }

        /// <summary>
        /// Подсветка устройств на схеме
        /// </summary>
        private void DrawDev_toolStripButton_Click(object sender, EventArgs e)
        {
            ProjectManager.GetInstance().RemoveHighLighting();

            if (drawDev_toolStripButton.Checked)
            {
                drawDev_toolStripButton.Checked = false;
            }
            else
            {
                drawDev_toolStripButton.Checked = true;
                ITreeViewItem item = GetActiveItem();
                if (item != null)
                {
                    if (item.IsDrawOnEplanPage)
                    {
                        ProjectManager.GetInstance().SetHighLighting(
                            item.GetObjectToDrawOnEplanPage());
                    }
                }
            }
        }


        public bool Editable { get; private set; } = false;

        public bool GenericGroupCreatable => false;

        /// <summary>
        /// Переход в режим редактирования.
        /// </summary>
        private void edit_toolStripButton_Click(object sender, EventArgs e)
        {
            if (edit_toolStripButton.Checked)
            {
                edit_toolStripButton.Checked = false;
                Editable = false;

                int columnsCount = editorTView.Columns.Count;
                for (int i = 0; i < columnsCount; i++)
                {
                    OLVColumn column = editorTView.GetColumn(i);
                    column.IsEditable = Editable;
                }

                EProjectManager.GetInstance().StopEditModes();

                DFrm.CheckShown();
                if (DFrm.GetInstance().IsVisible())
                {
                    ITreeViewItem item = null;
                    DFrm.OnSetNewValue onSetNewValue = null;
                    DFrm.GetInstance().ShowDisplayObjects(item, onSetNewValue);
                }

                ModeFrm.CheckShown();
                if (ModeFrm.GetInstance().IsVisible())
                {
                    ModeFrm.GetInstance().ShowModes(
                       TechObject.TechObjectManager.GetInstance(),
                       false, false, null, null, null, true);
                }
            }
            else
            {
                edit_toolStripButton.Checked = true;
                Editable = true;

                int columnsCount = editorTView.Columns.Count;
                for (int i = 0; i < columnsCount; i++)
                {
                    OLVColumn column = editorTView.GetColumn(i);
                    column.IsEditable = Editable;
                }

                //Редактирование устройств (запуск).
                EProjectManager.GetInstance().StartEditModesWithDelay();

                DFrm.CheckShown();
                if (DFrm.GetInstance().IsVisible())
                {
                    ITreeViewItem item = GetActiveItem();
                    if (item != null)
                    {
                        DFrm.GetInstance().ShowDisplayObjects(item, SetNewVal);
                    }
                    else
                    {
                        DFrm.GetInstance().ShowNoDevices();
                    }
                }

                ModeFrm.CheckShown();
                if (ModeFrm.GetInstance().IsVisible())
                {
                    ITreeViewItem item = GetActiveItem();

                    ITreeViewItem parentItem = null;
                    if (item != null)
                    {
                        parentItem = GetParentBranch(item);
                    }

                    if (item != null && parentItem != null)
                    {
                        ModeFrm.GetInstance().ShowModes(
                            TechObject.TechObjectManager.GetInstance(),
                            true, item.IsLocalRestrictionUse, parentItem,
                            item, SetNewVal, true);
                        ModeFrm.GetInstance().SelectDevices(item,
                            SetNewVal);
                    }
                    else
                    {
                        ModeFrm.GetInstance().ShowNoModes();
                    }
                }
            }
        }

        /// <summary>
        /// Получение родительского элемента ITreeViewItem.
        /// Родительский элемент - объект TechObjectManager.TechObject
        /// </summary>
        /// <param name="item">Элемент для поиска</param>
        /// <returns></returns>
        public ITreeViewItem GetParentBranch(ITreeViewItem item)
        {
            ITreeViewItem needItem = null;
            if (item == null)
            {
                return needItem;
            }

            if (item.IsMainObject)
            {
                return item;
            }
            else
            {
                if (item.Parent != null)
                {
                    needItem = GetParentBranch(item.Parent);
                }
            }

            return needItem;
        }

        /// <summary>
        /// Обновление дерева
        /// </summary>
        private void refresh_toolStripButton_Click(object sender, EventArgs e)
        {
            bool saveDescrSilentMode = false;
            EProjectManager.GetInstance().SyncAndSave(saveDescrSilentMode);

            DFrm.GetInstance().RefreshTree();
            editorTView.RefreshObjects(treeViewItemsList);
        }

        /// <summary>
        /// Захват хуков при наведении курсора мыши на редактор
        /// </summary>
        private void editorTView_MouseEnter(object sender, EventArgs e)
        {
            globalKeyboardHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_KEYBOARD_LL,
                mainWndKeyboardCallbackDelegate, IntPtr.Zero, 0);
        }

        /// <summary>
        /// Освобождение хуков после того как курсор мыши уведен
        /// с окна редактора.
        /// </summary>
        private void editorTView_MouseLeave(object sender, EventArgs e)
        {
            if (globalKeyboardHookPtr != IntPtr.Zero)
            {
                PI.UnhookWindowsHookEx(globalKeyboardHookPtr);
                globalKeyboardHookPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Развертывание дерева до определенного уровня
        /// </summary>       
        /// <param name="level">Уровень развертывания дерева</param>
        private void ExpandToLevel(int level, IEnumerable items)
        {
            foreach (ITreeViewItem item in items)
            {
                if (item.Items != null)
                {
                    int lev = level;
                    ExpandToLevel(--lev, item.Items);
                }

                if (level > 0)
                {
                    if (editorTView.IsExpanded(item) == false)
                    {
                        editorTView.Expand(item);
                    }
                }
                else
                {
                    if (editorTView.IsExpanded(item) == true)
                    {
                        editorTView.Collapse(item);
                    }
                }
            }
        }

        /// <summary>
        /// Жирный шрифт, 8 пикселей.
        /// </summary>
        private Font boldFont8px = new Font("Microsoft Sans Serif", 8,
            FontStyle.Bold);

        /// <summary>
        /// Перечеркнутый шрифт, 8 пикселей.
        /// </summary>
        private Font strikeoutBoldFont8px = new Font("Microsoft Sans Serif", 9,
            FontStyle.Strikeout | FontStyle.Bold);

        /// <summary>
        /// Форматирование строк в редакторе.
        /// </summary>
        private void editorTView_FormatCell(object sender,
            FormatCellEventArgs e)
        {
            FormatItemsToBold(e, treeViewItemsList.ToArray());
            FormatActiveBoolParameters(e);
            FormatCuttedItems(e);
        }

        /// <summary>
        /// Форматирование элементов дерева в жирный
        /// </summary>
        /// <param name="level">Уровень вложенности</param>
        /// <param name="e">Аргументы события форматирования</param>
        /// <param name="mainItems">Элементы-потомки</param>
        private void FormatItemsToBold(FormatCellEventArgs e,
            ITreeViewItem[] mainItems)
        {
            foreach (var techObjManager in mainItems)
            {
                foreach (var s88Obj in techObjManager.Items)
                {
                    foreach (var baseObj in s88Obj.Items)
                    {
                        if ((e.Item.RowObject == techObjManager &&
                            !techObjManager.IsMainObject) ||
                            (e.Item.RowObject == s88Obj &&
                            !s88Obj.IsMainObject) ||
                            (e.Item.RowObject == baseObj &&
                            !baseObj.IsMainObject))
                        {
                            e.Item.Font = boldFont8px;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Форматирование булевских свойств в редакторе.
        /// </summary>
        /// <param name="e">Аргументы события форматирования</param>
        private void FormatActiveBoolParameters(FormatCellEventArgs e)
        {
            var boolProperty = e.Model as TechObject.ActiveBoolParameter;
            if (boolProperty != null && e.ColumnIndex == 1 &&
                boolProperty.DefaultValue != boolProperty.Value)
            {
                e.SubItem.Font = boldFont8px;
            }
        }

        /// <summary>
        /// Форматирование элемента помеченного для вырезки;
        /// </summary>
        /// <param name="e">Аргументы события форматирования</param>
        private void FormatCuttedItems(FormatCellEventArgs e)
        {
            var item = e.Model as ITreeViewItem;
            if (item.MarkToCut)
            {
                e.Item.Font = strikeoutBoldFont8px;
            }
        }

        /// <summary>
        /// Обработка начала редактирования ячеек. Установка текста 
        /// редактирования.
        /// </summary>
        private void editorTView_CellEditStarting(object sender,
            CellEditEventArgs e)
        {
            IsCellEditing = true;
            ITreeViewItem item = editorTView.SelectedObject as ITreeViewItem;

            if (item == null ||
                !item.IsEditable ||
                item.EditablePart[e.Column.Index] != e.Column.Index ||
                (e.Column.Index == 1 &&
                item.ContainsBaseObject &&
                item.BaseObjectsList.Count == 0))
            {
                IsCellEditing = false;
                e.Cancel = true;
                return;
            }

            // Проверяем колонку, и какой объект редактируется и вызываем
            // соответствующий редактор для ячейки.
            if (e.Column.Index == 1 &&
                (item.ContainsBaseObject || item.IsBoolParameter))
            {
                InitComboBoxCellEditor(item.BaseObjectsList);
                comboBoxCellEditor.Text = e.Value.ToString();
                comboBoxCellEditor.Bounds = e.CellBounds;
                e.Control = comboBoxCellEditor;
                comboBoxCellEditor.Focus();
                editorTView.Freeze();
            }
            else
            {
                InitTextBoxCellEditor();
                textBoxCellEditor.Text = item.EditText[e.Column.Index];
                textBoxCellEditor.Bounds = e.CellBounds;
                e.Control = textBoxCellEditor;
                textBoxCellEditor.Focus();
                editorTView.Freeze();
            }
        }

        /// <summary>
        /// Отключить нужные объекты
        /// </summary>
        /// <param name="items">Объекты для проверки и отключения</param>
        private void DisableNeededObjects(ITreeViewItem[] items)
        {
            foreach (var item in items)
            {
                if (item.IsFilled == false && hideEmptyItemsBtn.Checked)
                {
                    return;
                }

                if (item.Items != null)
                {
                    if (item.Items.Length != 0)
                    {
                        DisableNeededObjects(item.Items);
                    }
                }

                if (item.NeedDisable)
                {
                    if (!item.Disabled)
                    {
                        try
                        {
                            editorTView.DisableObject(item);
                            item.Disabled = true;
                        }
                        catch
                        {
                            item.Disabled = true;
                        }
                    }
                }
                else
                {
                    if (item.Disabled)
                    {
                        try
                        {
                            editorTView.EnableObject(item);
                            item.Disabled = false;
                        }
                        catch
                        {
                            item.Disabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обработка полученных данных после редактирования.
        /// </summary>
        private void editorTView_CellEditFinishing(object sender,
            CellEditEventArgs e)
        {
            IsCellEditing = false;
            bool isModified = false;
            editorTView.LabelEdit = false;
            var item = editorTView.SelectedObject as ITreeViewItem;

            ////При нажатии Esc отменяются все изменения.
            if (cancelChanges || item == null)
            {
                e.Cancel = true;
                cancelChanges = false;
                editorTView.Unfreeze();
                return;
            }

            // Если редактируются базовые операции/объекты/шаги
            if (e.Column.Index == 1 &&
                (item.ContainsBaseObject || item.IsBoolParameter))
            {
                e.NewValue = comboBoxCellEditor.Text;
                editorTView.Controls.Remove(comboBoxCellEditor);
                // true (IsExtraBool) - флаг работы с "экстра" полями
                isModified = item.SetNewValue(e.NewValue.ToString(),
                    true);
            }
            else
            {
                editorTView.Controls.Remove(textBoxCellEditor);
                isModified = item.SetNewValue(e.NewValue.ToString());
            }

            if (isModified)
            {
                RefreshTree();
                //Обновляем также и узел родителя при его наличии.
                if (item.NeedRebuildParent)
                {
                    DisableNeededObjects(item.Parent.Items);
                }
                HiglihtItems();
            }

            e.Cancel = true;
            editorTView.Unfreeze();
        }

        /// <summary>
        /// Обработка изменения выбранной строки.
        /// </summary>
        private void editorTView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (GetActiveItems().Count <= 0 || noOnChange)
                return;

            editorTView.BeginUpdate();

            var item = GetActiveItem();

            DFrm.CheckShown();
            if (edit_toolStripButton.Checked && DFrm.GetInstance().IsVisible() == true)
            {
                //Обновление списка устройств при его наличии.
                if (item is null || item.IsUseDevList == false)
                    DFrm.GetInstance().ShowNoDevices();
                else
                    DFrm.GetInstance().ShowDisplayObjects(item, SetNewVal, (e.Item is IAction) == false);
            }

            ModeFrm.CheckShown();
            if (edit_toolStripButton.Checked && ModeFrm.GetInstance().IsVisible() == true && item != null)
            {
                ITreeViewItem parentItem = GetParentBranch(item);
                ModeFrm.GetInstance().ShowModes(
                    TechObject.TechObjectManager.GetInstance(), true,
                    item.IsLocalRestrictionUse, parentItem,
                    item, SetNewVal, true);
                ModeFrm.GetInstance().SelectDevices(item, SetNewVal);
            }

            HiglihtItems();

            editorTView.EndUpdate();
            editorTView.Focus();
        }

        /// <summary>
        /// Изменение ширины колонки и пересчет ширины другой колонки
        /// </summary>
        private void editorTView_ColumnWidthChanging(object sender,
            ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                editorTView.Columns[1].Width = editorTView.Width -
                    deltaWidth - editorTView.Columns[0].Width;
            }
        }

        /// <summary>
        /// Обработка потери фокуса редактором во время редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editorTView_LostFocus(object sender, EventArgs e)
        {
            if (IsCellEditing)
            {
                cancelChanges = true;
                editorTView.FinishCellEdit();
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable == true)
            {
                CreateItem(item);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var items = GetActiveItems();
            if (items.Count > 0 && Editable == true)
            {
                DeleteItems(items);
            }
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            if (GetActiveItems().Count > 0 && Editable == true)
            {
                CutItem(GetActiveItems());
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            List<ITreeViewItem> items = GetActiveItems();
            if (items?.Count > 0 && Editable == true)
            {
                CopyItem(items);
            }
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable == true)
            {
                PasteItem(item);
            }
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable == true)
            {
                ReplaceItem(item);
            }
        }

        /// <summary>
        /// Форма импорта технологических объектов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importButton_Click(object sender, EventArgs e)
        {
            var importForm = new TechObjectsImportForm();
            importForm.ShowDialog();
        }

        /// <summary>
        /// Форма экспорта технологических объектов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            var exportForm = new TechObjectsExportForm();
            exportForm.ShowDialog();
        }

        private void editorTView_Expanded(object sender,
            TreeBranchExpandedEventArgs e)
        {
            editorTView.Columns[0].AutoResize(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void editorTView_Collapsed(object sender,
            TreeBranchCollapsedEventArgs e)
        {
            editorTView.Columns[0].AutoResize(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void hideEmptyItemsBtn_CheckStateChanged(object sender,
            EventArgs e)
        {
            UpdateModelFilter();
        }

        private void changeBasesObjBtn_Click(object sender, EventArgs e)
        {
            if (!Editable)
            {
                return;
            }

            ITreeViewItem activeItem = GetActiveItem();
            if (activeItem != null && activeItem.IsMainObject)
            {
                ITreeViewItem mainObjectParent = activeItem.Parent;
                var baseObjChanger = mainObjectParent as IBaseObjChangeable;
                if (baseObjChanger == null)
                {
                    return;
                }

                string messageForUser = $"Сбросить базовый объект " +
                    $"\"{activeItem.DisplayText[0]}\"?";
                DialogResult result = MessageBox.Show(messageForUser,
                    "Внимание", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    baseObjChanger.ChangeBaseObj(activeItem);
                    editorTView.RefreshObjects(treeViewItemsList);
                }
            }
        }

        private void toolSettingItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var toolMenuItem = menuItem?.Tag as ToolStripItem;
            var searchItem = menuItem?.Tag as Control;

            if (toolMenuItem != null)
            {
                toolMenuItem.Visible = menuItem.Checked;
            }
            else if (searchItem != null)
            {
                searchItem.Visible = menuItem.Checked;
            }

            SaveToolsHideToConfig();
            toolSettingDropDownButton.ShowDropDown();
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable is true)
            {
                MoveUpItem(item);
            }
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable is true)
            {
                MoveDownItem(item);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var items = GetActiveItems();

            bool singleSelection = items.Count == 1;

            var item = items.FirstOrDefault();
            
            if (item is null) return;

            // Возможность создания и удаления объекта
            contextMenuStrip.Items[nameof(createToolStripMenuItem)]
                .Enabled = Editable && item.IsInsertable && singleSelection;
            contextMenuStrip.Items[nameof(deleteToolStripMenuItem)]
                .Enabled = Editable && item.IsDeletable;

            // Возможность копирования и вырезки объекта
            contextMenuStrip.Items[nameof(copyToolStripMenuItem)]
                .Enabled = Editable && item.IsCopyable;
            contextMenuStrip.Items[nameof(cutToolStripMenuItem)]
                .Enabled = Editable && (item.Parent?.IsCuttable ?? false);

            // Возможность вставки и замены скопированного элемента
            contextMenuStrip.Items[nameof(pasteToolStripMenuItem)]
                .Enabled = Editable && item.IsInsertableCopy 
                && copyItems != null && singleSelection;
            contextMenuStrip.Items[nameof(replaceToolStripMenuItem)]
                .Enabled = Editable && item.IsReplaceable && copyItems != null 
                && (copyItems.Count() == 1)
                && (copyItems.SingleOrDefault() as ITreeViewItem)?.MarkToCut is false 
                && (copyItems.SingleOrDefault()?.GetType() == item.GetType()) 
                && singleSelection;

            // Возможность перемещения объектов
            contextMenuStrip.Items[nameof(moveUpToolStripMenuItem)]
                .Enabled = Editable && (item.Parent?.CanMoveUp(item) ?? false) 
                && singleSelection;
            contextMenuStrip.Items[nameof(moveDownToolStripMenuItem)]
                .Enabled = Editable && (item.Parent?.CanMoveDown(item) ?? false) 
                && singleSelection;

            // toolTip показывает скопированный или вырезанный элемент
            var copies = copyItems?.Cast<ITreeViewItem>().ToList();
            contextMenuStrip.Items[nameof(pasteToolStripMenuItem)]
                .ToolTipText = string.Join("\n", copies?.Select(copy => (copy is null) ? null : $"{copy.DisplayText[0]} | {copy.DisplayText[1]}") ?? new List<string>());
            contextMenuStrip.Items[nameof(replaceToolStripMenuItem)]
                .ToolTipText = contextMenuStrip.Items[nameof(pasteToolStripMenuItem)].ToolTipText;
        }

        private void SetUpToolsHideFromConfig()
        {
            var hidenTools = AppConfiguarationManager.GetAppSetting("hidenTools");

            foreach (string tool in hidenTools.Split(';'))
            {
                var menuItem = (toolSettingDropDownButton.DropDownItems[$"settingMenuItem_{tool}"] as ToolStripMenuItem);
                if (menuItem is null)
                    continue;

                menuItem.Checked = false;

                var toolStripButton = menuItem.Tag as ToolStripItem;
                var searchItem = menuItem.Tag as Control;

                if (toolStripButton != null)
                {
                    toolStripButton.Visible = false;
                }
                if (searchItem != null)
                {
                    searchItem.Visible = false;
                }
            }

        }

        private void SaveToolsHideToConfig()
        {
            var result = new StringBuilder();
            foreach (ToolStripMenuItem menuItem in toolSettingDropDownButton.DropDownItems)
            {
                if (menuItem.Checked is false)
                {
                    result.Append($"{menuItem.Name.Split('_').LastOrDefault()};");
                }
            }

            AppConfiguarationManager.SetAppSetting("hidenTools",
                result.ToString().TrimEnd(';'));
        }

        private void tableLayoutPanelSearchBox_Paint(object sender, PaintEventArgs e)
        {
            var rect = e.ClipRectangle;
            rect.Inflate(-1, -1);

            e.Graphics.Clear(Color.White);
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), rect);
        }

        private void tableLayoutPanelSearchBox_MouseClick(object sender, MouseEventArgs e)
        {
            textBox_search.Focus();
        }

        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            if (textBox_search.Text == "Поиск..." || textBox_search.Text == string.Empty)
            {
                formatNumericUpDown_SearchSelectedItem.Value = 0;
                searchText = string.Empty;

                UpdateModelFilter();
                return;
            }

            if (textBoxSearchTypingTimer is null)
            {
                textBoxSearchTypingTimer = new System.Windows.Forms.Timer()
                {
                    Interval = 300,

                };
                textBoxSearchTypingTimer.Tick += TextBoxSearchTypingTimer_Tick;
            }

            textBoxSearchTypingTimer.Stop();
            textBoxSearchTypingTimer.Tag = textBox_search.Text;
            textBoxSearchTypingTimer.Start();
        }

        private void TextBoxSearchTypingTimer_Tick(object sender, EventArgs e)
        {
            if (textBoxSearchTypingTimer is null)
                return;

            var search = textBoxSearchTypingTimer.Tag.ToString();

            searchText = search;

            UpdateModelFilter();


            textBoxSearchTypingTimer.Stop();
        }

        private void textBox_search_Enter(object sender, EventArgs e)
        {
            if (textBox_search.Text == "Поиск...")
            {
                textBox_search.ForeColor = Color.Black;
                textBox_search.Text = string.Empty;
            }
        }

        private void textBox_search_Leave(object sender, EventArgs e)
        {
            if (textBox_search.Text == string.Empty)
            {
                textBox_search.ForeColor = Color.Gray;
                textBox_search.Text = "Поиск...";
            }
        }

        private void UpdateModelFilter()
        {
            editorTView.UseFiltering = false;

            FoundTreeViewItemsList.Clear();
            formatNumericUpDown_SearchSelectedItem.Value = 0;
            treeViewItemsList.ForEach(item => item.ResetFilter());

            if (hideEmptyItemsBtn.Checked || searchText != string.Empty)
            {
                editorTView.UseFiltering = true;
                formatNumericUpDown_SearchSelectedItem.Maximum = FoundTreeViewItemsList.Count;
            }
        }

        private void formatNumericUpDown_SearchSelectedItem_ValueChanged(object sender, EventArgs e)
        {
            var item = FoundTreeViewItemsList?.ElementAtOrDefault((int)formatNumericUpDown_SearchSelectedItem.Value - 1);

            if (item != null)
            {
                RecursiveExpand(item.Parent);
                if (editorTView.CanExpand(item))
                    editorTView.Expand(item);
                editorTView.SelectObject(item, true);
                editorTView.EnsureModelVisible(item);
            }
        }

        private void RecursiveExpand(ITreeViewItem parent)
        {
            if (parent is null)
                return;

            RecursiveExpand(parent.Parent);

            if (editorTView.CanExpand(parent))
                editorTView.Expand(parent);
        }

        /// <summary>
        /// Быстрое выделение нескольких элементов
        /// </summary>
        private void editorTView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (editorTView.MouseMoveHitTest.Item is null)
                return;

            var item = editorTView.MouseMoveHitTest.Item.RowObject as ITreeViewItem;
            if (item is null)
                return;

            editorTView.BeginUpdate();

            if (ModifierKeys == Keys.Control && e.Button == MouseButtons.Left)
            {
                SelectedItems = item.QuickMultiSelect();
                editorTView.SelectObjects(SelectedItems);
            }

            editorTView.EndUpdate();
        }

        public bool wasInit { get; set; }
        private bool IsCellEditing;

        public List<ITreeViewItem> treeViewItemsList;
        public static List<ITreeViewItem> FoundTreeViewItemsList { get; set; } = new List<ITreeViewItem>();

        public List<ITreeViewItem> SelectedItems { get; private set; } = new List<ITreeViewItem>();

        private System.Windows.Forms.Timer textBoxSearchTypingTimer;
        private string searchText = "";

        // Редакторы для editorTView
        ComboBox comboBoxCellEditor;
        TextBox textBoxCellEditor;
    }
}