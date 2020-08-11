using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EasyEPlanner;
using PInvoke;
using BrightIdeasSoftware;
using System.Collections;
using System.Diagnostics;

namespace NewEditor
{
    public partial class NewEditorControl : UserControl
    {
        public NewEditorControl()
        {
            InitializeComponent();
            InitObjectListView();

            dialogCallbackDelegate = new PI.HookProc(DlgWndHookCallbackFunction);
            mainWndKeyboardCallbackDelegate =
                new PI.LowLevelKeyboardProc(GlobalHookKeyboardCallbackFunction);

            // Фильтр
            editorTView.ModelFilter = new ModelFilter(delegate (object obj) 
            {
                return ((ITreeViewItem)obj).IsFilled;
            });

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
                    if (displayingObject is TreeViewItem)
                    {
                        ITreeViewItem obj = displayingObject as ITreeViewItem;
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
                    ITreeViewItem objTreeViewItem = expandingObject as ITreeViewItem;
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
                    ITreeViewItem objTreeViewItem = obj as ITreeViewItem;
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
            if (editorTView.SelectedObject != null)
            {
                if (editorTView.SelectedObject is ITreeViewItem)
                {
                    return editorTView.SelectedObject as ITreeViewItem;
                }
            }

            return null;
        }

        public bool wasInit { get; set; }
        private bool IsCellEditing;
        public List<ITreeViewItem> treeViewItemsList;

        // Редакторы для editorTView
        ComboBox comboBoxCellEditor;
        TextBox textBoxCellEditor;

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
            //data.AddParent(null);
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

        public static void CheckShown() // Проверка текущего состояния окон
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

        // Сохранение конфигурации окон
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
                    c == (uint)Keys.X) &&
                    PI.GetKeyState((int)PI.VIRTUAL_KEY.VK_CONTROL) != -0))
                {
                    if (IsCellEditing || editorTView.Focused)
                    {
                        return (IntPtr) 1;
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
                            return (IntPtr) 1;
                        }
                        break;

                    case PI.VIRTUAL_KEY.VK_RETURN:                       //Enter
                        if (IsCellEditing)
                        {
                            editorTView.FinishCellEdit();
                            return (IntPtr) 1;
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

                                return (IntPtr) 1;
                            }

                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.C, 0);

                                return (IntPtr) 1;
                            }

                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.C, 0);

                                return (IntPtr) 1;
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
                                return (IntPtr) 1;
                            }

                            if (editorTView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.V, 0);
                                return (IntPtr) 1;
                            }
                        }
                        break;

                    case (uint)Keys.Delete:                             //Delete
                        if (IsCellEditing || editorTView.Focused)
                        {
                            PI.SendMessage(PI.GetFocus(), (int)PI.WM.KEYDOWN,
                                (int)PI.VIRTUAL_KEY.VK_DELETE, 0);
                            return (IntPtr) 1;
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
                        return (IntPtr) 1;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        /// <summary>
        /// Функция для обработки завершения работы окна редактора.
        /// </summary>
        public void CloseEditor()
        {
            cancelChanges = true;
            editorTView.FinishCellEdit();
            editorTView.ClearObjects();

            PI.UnhookWindowsHookEx(dialogHookPtr);
            PI.UnhookWindowsHookEx(globalKeyboardHookPtr);

            globalKeyboardHookPtr = IntPtr.Zero;

            PI.SetParent(editorTView.Handle, this.Handle);
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
                        IntPtr dialogPtr = PI.GetParent(editorTView.Handle);

                        PI.RECT rctDialog;
                        PI.RECT rctPanel;
                        PI.GetWindowRect(dialogPtr, out rctDialog);
                        PI.GetWindowRect(panelPtr, out rctPanel);

                        int w = rctDialog.Right - rctDialog.Left;
                        int h = rctDialog.Bottom - rctDialog.Top;

                        toolStrip.Location = new Point(0, 0);
                        editorTView.Location = new Point(0, toolStrip.Height);

                        toolStrip.Width = w;
                        editorTView.Width = w;
                        editorTView.Height = h - toolStrip.Height;
                        break;
                }
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        public static bool editIsShown = false; //Показано ли окно.
        public bool IsShown = false; // Костыль, без этого не работает IsShown()
        public static IntPtr wndEditVisiblePtr; // Дескриптор редактора.

        /// <summary>
        /// Инициализация формы данными для редактирования.
        ///
        /// Так как данная форма отображается как внутреннее окно, то алгоритм
        /// следующий:
        /// 1 Поиск окна "Пространство листа" (Страница -> Навигатор комментариев).
        /// 1.1 Поиск плавающего представления: через FindWindowByCaption ,
        /// потом для поиска панели и диалога DlgItemId (0xE81F - базовая панель,
        /// 0x3458 - диалог). Если окно найдено, то переходим к 4, иначе к 1.1.1.
        /// 1.1.1 Поиск плавающего представления: иногда не отображается заголовок
        /// из-за чего невозможно сразу определить окно, тогда проверяются потомки окон,
        /// которые могут содержать заголовок родительского окна. Если не найдены заголовки,
        /// то переходим к 1.2 (значит плавающего представления нет).
        /// 1.2 Поиск закрепленного представления: через GetDlgItem для всех дочерних
        /// окон (GetChildWindows) приложения Eplan по DlgItemId (0x3458 - диалог).
        /// Если окно найдено, то переходим к 4, иначе к 2.
        /// 2 Симулируем нажатие пункта меню (Страница -> Навигатор комментариев)
        /// для его отображения.
        /// 3 Повторяем поиск окна (1.1, 1.1.1 и 1.2). Если окно не найдено выводим
        /// сообщение об ошибке, завершаем редактирование, иначе к 4.
        /// 4 Скрываем панель с элементами управления Eplan'а
        /// (GetDlgItem, 0xBC2 - родительская панель, ShowWindow).
        /// 5. Переносим на найденное окно свои элементы (SetParent) и подгоняем
        /// из размеры и позицию.
        /// 6. Устанавливаем свой хук для найденного окна (для изменения размеров
        /// своих элементов, сохранения изменений при закрытии и отключения хука).
        /// </summary>

        public void ShowDlg()
        {
            Process oCurrent = Process.GetCurrentProcess();

            // Идентификатор команды вызова окна "Навигатор комментариев"
            const int wndWmCommand = 35381; 

            if (editIsShown == true && IsShown == true)
            {
                if (PI.IsWindowVisible(wndEditVisiblePtr) == false)
                {
                    PI.SendMessage(oCurrent.MainWindowHandle,
                            (uint)PI.WM.COMMAND, wndWmCommand, 0);
                    return;
                }
                return;
            }

            wasShown = true;

            string windowName = "Комментарий";
            IntPtr res = PI.FindWindowByCaption(
                IntPtr.Zero, windowName);            //1.1;

            if (res != IntPtr.Zero)
            {
                var resList = PI.GetChildWindows(res);
                if (resList.Count > 0)
                {
                    dialogHandle = resList[0];
                    wndEditVisiblePtr = dialogHandle; // Сохраняем дескриптор окна.
                }
            }
            else
            {
                StringBuilder stringBuffer = new StringBuilder(200);                //1.1.1

                List<IntPtr> mainWindowChilds = PI.GetChildWindows(PI.GetDesktopWindow());
                foreach (IntPtr mainWindowChild in mainWindowChilds)
                {
                    PI.GetWindowText(mainWindowChild, stringBuffer, stringBuffer.Capacity);
                    if (stringBuffer.ToString().Contains(windowName) == false &&
                        stringBuffer.ToString().Contains("EPLAN") == false)
                    {
                        List<IntPtr> windowChilds = PI.GetChildWindows(mainWindowChild);
                        foreach (IntPtr windowChild in windowChilds)
                        {
                            PI.GetWindowText(windowChild, stringBuffer, stringBuffer.Capacity);
                            if (stringBuffer.ToString().Contains(windowName) == true)
                            {
                                if (PI.IsWindowVisible(windowChild) == true)
                                {
                                    // Если нашел в потомке название, беру родительское окно и работаю с ним
                                    var resList = PI.GetChildWindows(mainWindowChild);
                                    if (resList.Count > 0)
                                    {
                                        dialogHandle = resList[0];
                                        res = dialogHandle;
                                        wndEditVisiblePtr = dialogHandle; // Сохраняем дескриптор окна.
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                List<IntPtr> resW = PI.GetChildWindows(oCurrent.MainWindowHandle);    //1.2
                foreach (IntPtr panel in resW)
                {
                    PI.GetWindowText(panel, stringBuffer, stringBuffer.Capacity);
                    if (stringBuffer.ToString().Contains(windowName) == true)
                    {
                        if (PI.IsWindowVisible(panel) == true)
                        {
                            var resList = PI.GetChildWindows(panel);
                            if (resList.Count > 0)
                            {
                                dialogHandle = resList[0];

                                res = dialogHandle;
                                wndEditVisiblePtr = dialogHandle; // Сохраняем дескриптор окна.
                                break;
                            }
                        }
                    }
                }

                if (res == IntPtr.Zero)
                {
                    PI.SendMessage(oCurrent.MainWindowHandle,
                        (uint)PI.WM.COMMAND, wndWmCommand, 0);     //2

                    res = PI.FindWindowByCaption(
                    IntPtr.Zero, windowName);                      //3

                    if (res != IntPtr.Zero)
                    {

                        var resList = PI.GetChildWindows(res);
                        if (resList.Count > 0)
                        {
                            dialogHandle = resList[0];
                            wndEditVisiblePtr = dialogHandle; // Сохраняем дескриптор окна.
                        }
                    }
                    else
                    {
                        mainWindowChilds = PI.GetChildWindows(PI.GetDesktopWindow());
                        foreach (IntPtr mainWindowChild in mainWindowChilds)
                        {
                            PI.GetWindowText(mainWindowChild, stringBuffer, stringBuffer.Capacity);
                            if (stringBuffer.ToString().Contains(windowName) == false &&
                        stringBuffer.ToString().Contains("EPLAN") == false)
                            {
                                List<IntPtr> windowChilds = PI.GetChildWindows(mainWindowChild);
                                foreach (IntPtr windowChild in windowChilds)
                                {
                                    PI.GetWindowText(windowChild, stringBuffer, stringBuffer.Capacity);
                                    if (stringBuffer.ToString().Contains(windowName) == true)
                                    {
                                        // Если нашел в потомке название, беру родительское окно и работаю с ним
                                        var resList = PI.GetChildWindows(mainWindowChild);
                                        if (resList.Count > 0)
                                        {
                                            dialogHandle = resList[0];
                                            wndEditVisiblePtr = dialogHandle; // Сохраняем дескриптор окна.
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        resW = PI.GetChildWindows(oCurrent.MainWindowHandle);
                        foreach (IntPtr panel in resW)
                        {

                            PI.GetWindowText(panel, stringBuffer, stringBuffer.Capacity);
                            if (stringBuffer.ToString().Contains(windowName) == true)
                            {
                                var resList = PI.GetChildWindows(panel);
                                if (resList.Count > 0)
                                {
                                    dialogHandle = resList[0];

                                    wndEditVisiblePtr = dialogHandle; // Сохраняем дескриптор окна.
                                    break;
                                }
                            }
                        }

                        if (dialogHandle == IntPtr.Zero)
                        {
                            MessageBox.Show("Не удалось найти окно!");
                            return;
                        }
                    }
                }
            }

            var panelList = PI.GetChildWindows(dialogHandle);       //4
            if (panelList.Count > 0)
            {
                panelPtr = panelList[0];
            }

            if (panelPtr == IntPtr.Zero)
            {
                MessageBox.Show("Не удалось скрыть окно!");
                return;
            }

            // Проверка, скрыт ли элемент управления с редактором.
            if (PI.IsWindowVisible(dialogHandle) == false)
            {
                PI.ShowWindow(dialogHandle, 1);
            }
            PI.ShowWindow(panelPtr, 0);

            this.Controls.Clear();

            PI.SetParent(editorTView.Handle, dialogHandle);         //5
            PI.SetParent(toolStrip.Handle, dialogHandle);

            IntPtr dialogPtr = PI.GetParent(editorTView.Handle);

            PI.RECT rctDialog;
            PI.RECT rctPanel;
            PI.GetWindowRect(dialogPtr, out rctDialog);
            PI.GetWindowRect(panelPtr, out rctPanel);

            int w = rctDialog.Right - rctDialog.Left;
            int h = rctDialog.Bottom - rctDialog.Top;

            toolStrip.Location = new Point(0, 0);
            editorTView.Location = new Point(0, toolStrip.Height);

            toolStrip.Width = w;
            editorTView.Width = w;
            editorTView.Height = h - toolStrip.Height;

            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);        //6
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);

            globalKeyboardHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_KEYBOARD_LL,
                mainWndKeyboardCallbackDelegate, IntPtr.Zero, 0);

            if (globalKeyboardHookPtr == IntPtr.Zero)
            {
                MessageBox.Show("Ошибка! Не удалось переназначить клавиши!");
            }

            editIsShown = true;
            IsShown = true;

            DisableNeededObjects(treeViewItemsList.ToArray());
        }

        public uint pidMain = 0;

        /// <summary>
        /// Обработка изменений при редактировании.
        /// </summary>
        private void OnModify()
        {
            //Отображение (подсветка) участвующих в операции устройств на карте
            //Eplan'a.
            if (drawDev_toolStripButton.Checked)
            {
                ITreeViewItem item = GetActiveItem();

                ProjectManager.GetInstance().RemoveHighLighting();
                if (item != null && item.IsDrawOnEplanPage)
                {
                    //ProjectManager.GetInstance().SetHighLighting(
                    //    item.GetObjectToDrawOnEplanPage());
                }
            }
        }

        // Хранение скопированного объекта.
        private object copyItem = null;

        private bool noOnChange = default;

        /// <summary>
        /// Обработка нажатий клавиш клавиатуры.
        /// </summary>
        private void editorTView_KeyDown(object sender, KeyEventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item == null)
            {
                MessageBox.Show("Действие недоступно. Элемент дерева не " +
                    "определен.", "Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (Editable)
            {
                // Перемещение элемента вверх.
                if (e.KeyCode == Keys.Up && e.Shift == true)
                {
                    MoveUpItem(item);
                    return;
                }

                //Перемещение элемента вниз.
                if (e.KeyCode == Keys.Down && e.Shift == true)
                {
                    MoveDownItem(item);
                    return;
                }

                // Копирование элемента.
                if (e.KeyCode == Keys.C && e.Control == true)
                {
                    CopyItem(item);
                    return;
                }

                // Вставка скопированного ранее элемента.
                if (e.KeyCode == Keys.V && e.Control == true)
                {
                    PasteItem(item);
                    return;
                }

                // Замена элемента.
                if (e.KeyCode == Keys.B && e.Control == true)
                {
                    ReplaceItem(item);
                    return;
                }

                // Вставка нового элемента.
                if (e.KeyCode == Keys.Insert)
                {
                    CreateItem(item);
                    return;
                }

                // Удаление существующего элемента.
                if (e.KeyCode == Keys.Delete)
                {
                    DeleteItem(item);
                    return;
                }
            }
            
            // Окно справки по элементам
            if (e.KeyCode == Keys.F1)
            {
                var itemHelper = item as IHelperItem;
                string link = itemHelper.GetLinkToHelpPage();
                if (link == null)
                {
                    link = ProjectManager.GetInstance()
                        .GetOstisHelpSystemMainPageLink();
                    if(link == null)
                    {
                        MessageBox.Show("Ошибка поиска адреса системы помощи");
                        return;
                    }
                }
                Process.Start(link);

                return;
            }

            return;
        }

        /// <summary>
        /// Скопировать элемент
        /// </summary>
        /// <param name="item">Копируемый элемент</param>
        private void CopyItem(ITreeViewItem item)
        {
            if (item.IsCopyable)
            {
                copyItem = item;
            }
        }

        /// <summary>
        /// Создать элемент (в выделенной точке)
        /// </summary>
        /// <param name="item">Элемент в котором создается новый элемент</param>
        private void CreateItem(ITreeViewItem item)
        {
            if (item.IsInsertable == true)
            {
                ITreeViewItem newItem = item.Insert();
                if(newItem != null)
                {
                    newItem.AddParent(item);
                    editorTView.RefreshObjects(item.Items);
                    editorTView.RefreshObject(item);
                    if (item.NeedRebuildParent && item.Parent != null)
                    {
                        editorTView.RefreshObject(item.Parent);
                    }

                    OnModify();
                }
            }
        }

        /// <summary>
        /// Удалить элемент
        /// </summary>
        /// <param name="item">Удаляемый элемент</param>
        private void DeleteItem(ITreeViewItem item)
        {
            if (item.IsDeletable == true)
            {
                DialogResult showWarningResult;
                if (item.ShowWarningBeforeDelete)
                {
                    string message = "Вы действительно хотите удалить " +
                        "выделенный элемент?";
                    showWarningResult = MessageBox.Show(message, "Внимание", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (showWarningResult == DialogResult.No)
                    {
                        return;
                    }
                }

                ITreeViewItem parent = item.Parent;
                bool isDelete = parent.Delete(item);
                if (isDelete)
                {
                    if (item.NeedRebuildMainObject)
                    {
                        var mainObject = GetParentBranch(item);
                        editorTView.RefreshObjects(mainObject.Items);
                    }
                    editorTView.RefreshObjects(parent.Items);
                    editorTView.RefreshObject(parent);
                }
                else
                {
                    editorTView.RefreshObject(item);
                    if (item.Items != null)
                    {
                        editorTView.RefreshObjects(item.Items);
                    }
                }
                OnModify();
            }
        }

        /// <summary>
        /// Вставить элемент (Ctrl + V)
        /// </summary>
        /// <param name="item">Вставляемый элемент</param>
        private void PasteItem(ITreeViewItem item)
        {
            if (item.IsInsertableCopy && copyItem != null)
            {
                ITreeViewItem newItem = item.InsertCopy(copyItem);
                if (newItem != null)
                {
                    newItem.AddParent(item);
                    OnModify();
                    editorTView.RefreshObjects(item.Items);

                    if(item.NeedRebuildParent && item.Parent != null)
                    {
                        editorTView.RefreshObject(item.Parent);
                    }

                    DisableNeededObjects(new ITreeViewItem[] { newItem });
                }
            }
        }

        /// <summary>
        /// Заменить элемент (Ctrl + B)
        /// </summary>
        /// <param name="item">Заменяемый элемент</param>
        private void ReplaceItem(ITreeViewItem item)
        {
            if (copyItem != null && item.IsReplaceable)
            {
                ITreeViewItem parent = item.Parent;
                if(parent != null)
                {
                    ITreeViewItem newItem = parent.Replace(item, copyItem);
                    if (newItem != null)
                    {
                        newItem.AddParent(parent);
                        editorTView.RefreshObjects(parent.Items);
                        if (item.NeedRebuildMainObject)
                        {
                            var mainObject = GetParentBranch(item);
                            editorTView.RefreshObjects(mainObject.Items);
                        }

                        if (item.NeedRebuildParent)
                        {
                            editorTView.RefreshObject(parent);
                        }

                        DisableNeededObjects(new ITreeViewItem[] { newItem });
                    }
                    OnModify();
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
                    item.AddParent(itemParent);
                    editorTView.RefreshObjects(itemParent.Items);
                    editorTView.SelectedIndex--;
                }
                OnModify();
            }
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
                    item.AddParent(itemParent);
                    editorTView.RefreshObjects(itemParent.Items);
                    editorTView.SelectedIndex++;
                }
                OnModify();
            }
        }

        /// <summary>
        /// Обработка нажатий кнопок уровня.
        /// </summary>
        private void toolStripButton_Click(object sender, EventArgs e)
        {
            editorTView.BeginUpdate();
            int level = Convert.ToInt32((sender as ToolStripButton).Tag);
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
            bool isModified = (editorTView.SelectedObject as ITreeViewItem).SetNewValue(newVal);

            if (isModified)
            {
                ITreeViewItem item = editorTView.SelectedObject as ITreeViewItem;
                noOnChange = true;
                editorTView.RefreshObject(item);
                noOnChange = false;
                OnModify();
            }
        }

        /// <summary>
        /// Функция установки нового значения ограничений в редакторе.
        /// Ограничения передаются в виде словаря.
        /// </summary>
        /// <param name="dict">Словарь ограничений</param>
        internal void SetNewVal(SortedDictionary<int, List<int>> dict)
        {
            bool isModified = (editorTView.SelectedObject as ITreeViewItem)
                .SetNewValue(dict);

            if (isModified)
            {
                ITreeViewItem item = editorTView.SelectedObject as ITreeViewItem;
                noOnChange = true;
                editorTView.RefreshObject(item.Parent);
                noOnChange = false;
                OnModify();
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
                        //ProjectManager.GetInstance().SetHighLighting(
                        //    item.GetObjectToDrawOnEplanPage());
                    }
                }
            }
        }


        bool Editable = false;

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

                DFrm.CheckShown();
                if (DFrm.GetInstance().IsVisible())
                {
                    DFrm.GetInstance().ShowDevices(
                        Device.DeviceManager.GetInstance(),
                        null, null, true, false, "", null);
                }

                ModeFrm.CheckShown();
                if (ModeFrm.GetInstance().isVisible())
                {
                    ModeFrm.GetInstance().ShowModes(
                       TechObject.TechObjectManager.GetInstance(),
                       false, false, null, null, null, true);
                }

                EProjectManager.GetInstance().StopEditModes();
            }
            else
            {
                if(hideEmptyItemsBtn.Checked)
                {
                    hideEmptyItemsBtn.Checked = false;
                }

                edit_toolStripButton.Checked = true;
                Editable = true;

                int columnsCount = editorTView.Columns.Count;
                for (int i = 0; i < columnsCount; i++)
                {
                    OLVColumn column = editorTView.GetColumn(i);
                    column.IsEditable = Editable;
                }

                DFrm.CheckShown();
                if (DFrm.GetInstance().IsVisible())
                {
                    ITreeViewItem item = GetActiveItem();
                    if (item != null)
                    {
                        Device.DeviceType[] devTypes;
                        Device.DeviceSubType[] devSubTypes;
                        item.GetDevTypes(out devTypes, out devSubTypes);

                        DFrm.GetInstance().ShowDevices(
                            Device.DeviceManager.GetInstance(),
                            devTypes, devSubTypes, false, true,
                            " " + item.EditText[1] + " ", SetNewVal);
                    }
                    else
                    {
                        DFrm.GetInstance().ShowNoDevices();
                    }
                }

                ModeFrm.CheckShown();
                if (ModeFrm.GetInstance().isVisible())
                {
                    ITreeViewItem item = GetActiveItem();

                    ITreeViewItem parentItem = null;
                    if (item != null)
                    {
                        parentItem = GetParentBranch(item);
                    }

                    if (item != null)
                    {
                        if (parentItem != null)
                        {
                            //ModeFrm.GetInstance().ShowModes(
                            //    TechObject.TechObjectManager.GetInstance(), 
                            //    true, item.IsLocalRestrictionUse, parentItem, 
                            //    item, SetNewVal, true);
                            //ModeFrm.GetInstance()
                            //    .SelectDevices(item, SetNewVal);
                        }
                        else
                        {
                            ModeFrm.GetInstance().ShowNoModes();
                        }

                    }
                    else
                    {
                        ModeFrm.GetInstance().ShowNoModes();
                    }
                }

                //Редактирование устройств (запуск).
                EProjectManager.GetInstance().StartEditModesWithDelay();
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
        /// Установка необходимых шрифтов.
        /// </summary>
        private void editorTView_FormatCell(object sender, FormatCellEventArgs e)
        {
            foreach (ITreeViewItem tObjectMan in treeViewItemsList)
            {
                foreach (ITreeViewItem tObject in tObjectMan.Items)
                {
                    if (e.Item.RowObject == tObject || 
                        e.Item.RowObject == tObjectMan)
                    {
                        e.Item.Font = new Font("Microsoft Sans Serif", 8, 
                            FontStyle.Bold);
                    }
                }
            }

            var boolProperty = e.Model as TechObject.ActiveBoolParameter;
            if (boolProperty != null && 
                e.ColumnIndex == 1 &&
                boolProperty.DefaultValue != boolProperty.Value)
            {
                e.SubItem.Font = new Font("Microsoft Sans Serif", 8, 
                    FontStyle.Bold);
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
            if (e.Column.Index == 1 && item.ContainsBaseObject)
            {
                InitComboBoxCellEditor(item.BaseObjectsList);
                comboBoxCellEditor.Text = e.Value.ToString();
                comboBoxCellEditor.Bounds = e.CellBounds;
                e.Control = comboBoxCellEditor;
                comboBoxCellEditor.Focus();
                editorTView.Freeze();
            }
            else if(e.Column.Index == 1 && item.IsBoolParameter)
            {
                item.SetNewValue(e.Value.ToString());
                IsCellEditing = false;
                e.Cancel = true;
                var parentItems = item.Parent.Items;
                DisableNeededObjects(parentItems);
                editorTView.RefreshObjects(parentItems);
                return;
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
            foreach(var item in items)
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
                else
                {
                    if (item.NeedDisable)
                    {
                        editorTView.DisableObject(item);
                        item.Disabled = true;
                    }
                    else
                    {
                        editorTView.EnableObject(item);
                        item.Disabled = false;
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
            var selectedItem = editorTView.SelectedObject as ITreeViewItem;

            //При нажатии Esc отменяются все изменения.
            if (cancelChanges || selectedItem == null)
            {
                e.Cancel = true;
                cancelChanges = false;
                editorTView.Unfreeze();
                return;
            }

            // Если редактируются базовые операции/объекты/шаги
            if (e.Column.Index == 1 && selectedItem.ContainsBaseObject)
            {       
                e.NewValue = comboBoxCellEditor.Text;
                editorTView.Controls.Remove(comboBoxCellEditor);
                // true (IsExtraBool) - флаг работы с "экстра" полями
                isModified = selectedItem.SetNewValue(e.NewValue.ToString(), 
                    true);
            }
            else
            {
                editorTView.Controls.Remove(textBoxCellEditor);
                isModified = selectedItem.SetNewValue(e.NewValue.ToString());
            }

            if (isModified)
            {
                //Обновляем также и узел родителя при его наличии.
                if (selectedItem.NeedRebuildParent)
                {
                    editorTView.RefreshObjects(selectedItem.Parent.Items);
                    DisableNeededObjects(selectedItem.Parent.Items);
                }
                else if (selectedItem.NeedRebuildMainObject)
                {
                    var mainObject = GetParentBranch(selectedItem);
                    editorTView.RefreshObjects(mainObject.Items);
                }

                editorTView.RefreshObject(selectedItem);
                OnModify();
            }

            e.Cancel = true;
            editorTView.Unfreeze();
        }

        /// <summary>
        /// Обработка изменения выбранной строки.
        /// </summary>
        private void editorTView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ITreeViewItem item = editorTView.SelectedObject as ITreeViewItem;
            if (item == null || noOnChange)
            {
                return;
            }

            editorTView.BeginUpdate();

            if (edit_toolStripButton.Checked)
            {
                DFrm.CheckShown();
                //Обновление списка устройств при его наличии.
                if (DFrm.GetInstance().IsVisible() == true)
                {
                    if (item.IsUseDevList == false)
                    {
                        DFrm.CheckShown();
                        if (DFrm.GetInstance().IsVisible())
                        {
                            DFrm.GetInstance().ShowNoDevices();
                        }
                    }
                    else
                    {
                        DFrm.CheckShown();
                        if (DFrm.GetInstance().IsVisible())
                        {
                            Device.DeviceType[] devTypes;
                            Device.DeviceSubType[] devSubTypes;
                            item.GetDevTypes(out devTypes, out devSubTypes);

                            DFrm.GetInstance().ShowDevices(
                                Device.DeviceManager.GetInstance(),
                                devTypes, devSubTypes, false, true,
                                item.EditText[1], SetNewVal);

                            DFrm.GetInstance().SelectDevices(item.EditText[1],
                                SetNewVal);

                            editorTView.RefreshObjects(treeViewItemsList);
                            OnModify();
                        }
                    }
                }

                ModeFrm.CheckShown();
                if (ModeFrm.GetInstance().isVisible() == true)
                {
                    if (item.IsUseRestriction == false)
                    {
                        ModeFrm.CheckShown();
                        if (ModeFrm.GetInstance().isVisible())
                        {
                            ModeFrm.GetInstance().ShowNoModes();
                        }
                    }
                    else
                    {
                        ITreeViewItem parentItem = GetParentBranch(item);
                        //ModeFrm.GetInstance().ShowModes(
                        //    TechObject.TechObjectManager.GetInstance(), true,
                        //    item.IsLocalRestrictionUse, parentItem,
                        //    item, SetNewVal, true);
                        //ModeFrm.GetInstance().SelectDevices(item, SetNewVal);
                        OnModify();
                    }
                }
            }

            //Отображение (подсветка) участвующих в режиме устройств на карте
            //Eplan'a.
            if (drawDev_toolStripButton.Checked)
            {
                ProjectManager.GetInstance().RemoveHighLighting();
                if (item.IsDrawOnEplanPage)
                {
                    //TODO: Починить метод.
                    //ProjectManager.GetInstance().SetHighLighting(
                    //    item.GetObjectToDrawOnEplanPage());
                }
            }
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
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable == true)
            {
                DeleteItem(item);
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable == true)
            {
                CopyItem(item);
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

        private void hideEmptyItemsBtn_CheckStateChanged(object sender, EventArgs e)
        {
            if (edit_toolStripButton.Checked && hideEmptyItemsBtn.Checked)
            {
                hideEmptyItemsBtn.Checked = false;
                editorTView.UseFiltering = false;
                MessageBox.Show("Режим сокрытия пустых элементов можно " +
                    "включить только при отключенном режиме редактирования.",
                    "Информация", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (hideEmptyItemsBtn.Checked)
            {
                editorTView.UseFiltering = true;
            }
            else
            {
                editorTView.UseFiltering = false;
            }
        }
    }
}