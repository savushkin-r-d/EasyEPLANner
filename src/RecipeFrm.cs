using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PInvoke;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using BrightIdeasSoftware;
using EasyEPlanner;
using System.Collections;
using System.Diagnostics;


namespace Editor
{
    public partial class RecipeFrm : Form
    {
        private static RecipeFrm frm = null;

        private RecipeFrm()
        {
            InitializeComponent();

            //const string columnName = "Рецепты";
            InitObjectListView();


            dialogCallbackDelegate = new PI.HookProc(
                DlgWndHookCallbackFunction);

            mainWndKeyboardCallbackDelegate =
                new PI.LowLevelKeyboardProc(GlobalHookKeyboardCallbackFunction);
        }

        public static RecipeFrm GetInstance()
        {
            if (frm == null)
            {
                frm = new RecipeFrm();
            }

            return frm;
        }

        public void ShowRecipes(ITreeViewItem objectTree)
        {
            if (wasInit == false)
            {
                Init(objectTree);
                wasInit = true;
            }

            ShowDlg();
        }

        /// <summary>
        /// Инициализация формы данными для редактирования.
        /// </summary>
        /// <param name="data"> Данные для отображения.</param>
        public void Init(ITreeViewItem data)
        {
            editorRView.BeginUpdate();
            treeViewItemsList = new List<ITreeViewItem>();
            treeViewItemsList.Add(data);
            editorRView.Roots = treeViewItemsList;
            editorRView.Columns[0]
                .AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            editorRView.Columns[0].Width = 200;
            editorRView.Columns[1].Width = editorRView.Width -
                editorRView.Columns[0].Width - deltaWidth;
            if (treeViewItemsList.Count > 0)
            {
                editorRView.Expand(data);
                editorRView.SelectedIndex = 0;
                editorRView.SelectedItem.EnsureVisible();
            }
            editorRView.EndUpdate();

            wasInit = true;
        }

        /// <summary>
        /// Инициализация ObjectListView
        /// </summary>
        private void InitObjectListView()
        {
            // Настройка цвета отключенного компонента в дереве
            var disabletItemStyle = new SimpleItemStyle();
            disabletItemStyle.ForeColor = Color.Gray;
            editorRView.DisabledItemStyle = disabletItemStyle;

            // Текст подсветки чередующихся строк
            editorRView.AlternateRowBackColor = Color.FromArgb(250, 250, 250);

            // Получение текста для отображения
            editorRView.CellToolTipGetter =
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
            editorRView.CanExpandGetter =
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

            // Делегат получения дочерних элементов
            editorRView.ChildrenGetter = obj => (obj as ITreeViewItem).Items;

            // Настройка и добавление колонок
            var firstColumn = new OLVColumn("Название", "DisplayText[0]");
            firstColumn.IsEditable = true;//false;
            firstColumn.AspectGetter = obj => (obj as ITreeViewItem).DisplayText[0];
            firstColumn.Sortable = false;
            firstColumn.ImageGetter =
                delegate (object obj)
                {
                    var objTreeViewItem = obj as ITreeViewItem;
                    int countOfImages = editorRView.SmallImageList.Images.Count;
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
            secondColumn.IsEditable = true;//false;
            secondColumn.AspectGetter = obj => (obj as ITreeViewItem).DisplayText[1];
            secondColumn.Sortable = false;

            editorRView.Columns.Add(firstColumn);
            editorRView.Columns.Add(secondColumn);
        }

        /// <summary>
        /// Получение активного элемента дерева.
        /// </summary>
        /// <returns>Активный элемент дерева.</returns>
        public ITreeViewItem GetActiveItem()
        {
            if (editorRView.SelectedObject != null)
            {
                if (editorRView.SelectedObject is ITreeViewItem)
                {
                    return editorRView.SelectedObject as ITreeViewItem;
                }
            }

            return null;
        }

        public bool wasInit { get; set; }
        public List<ITreeViewItem> treeViewItemsList;


        private IntPtr DlgWndHookCallbackFunction(int code, IntPtr wParam,
            IntPtr lParam)
        {
            PI.CWPSTRUCT msg = (PI.CWPSTRUCT)System.Runtime.InteropServices
                .Marshal.PtrToStructure(lParam, typeof(PI.CWPSTRUCT));

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

            if (msg.hwnd == dialogHandle)
            {
                switch (msg.message)
                {
                    case (int)PI.WM.GETTEXTLENGTH:
                        return (IntPtr)newCapt.Length;

                    case (int)PI.WM.SETTEXT:
                        return IntPtr.Zero;

                    case (int)PI.WM.WINDOWPOSCHANGED:
                        PI.WINDOWPOS p = new PI.WINDOWPOS();
                        p = (PI.WINDOWPOS)
                            System.Runtime.InteropServices.Marshal
                            .PtrToStructure(lParam, typeof(PI.WINDOWPOS));

                        break;

                    case (int)PI.WM.DESTROY:
                        PI.UnhookWindowsHookEx(dialogHookPtr);
                        dialogHookPtr = IntPtr.Zero;
                        dialogHandle = IntPtr.Zero;

                        PI.SetParent(editorRView.Handle, this.Handle);
                        PI.SetParent(toolStrip.Handle, this.Handle);
                        this.Controls.Add(editorRView);
                        this.Controls.Add(toolStrip);
                        editorRView.Hide();
                        System.Threading.Thread.Sleep(1);
                        deviceIsShown = false;
                        isLoaded = false;
                        break;

                    case (int)PI.WM.GETTEXT:
                        System.Runtime.InteropServices.Marshal.Copy(
                            newCapt, 0, lParam, newCapt.Length);
                        return (IntPtr)newCapt.Length;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }


        /// <summary>
        /// Изменить размер UI.
        /// </summary>
        private void ChangeUISize()
        {
            IntPtr dialogPtr = PI.GetParent(editorRView.Handle);

            PI.RECT rctDialog;
            PI.GetWindowRect(dialogPtr, out rctDialog);

            int w = rctDialog.Right - rctDialog.Left;
            int h = rctDialog.Bottom - rctDialog.Top;

            toolStrip.Location = new Point(0, 0);
            editorRView.Location = new Point(0, toolStrip.Height);

            toolStrip.Width = w;
            editorRView.Width = w;
            editorRView.Height = h - toolStrip.Height;
        }

        /// <summary>
        /// Дескриптор окна устройств
        /// </summary>
        public static IntPtr wndDevVisibilePtr;

        /// <summary>
        /// Показано ли окно
        /// </summary>
        public static bool deviceIsShown = false;

        /// <summary>
        /// Загружено ли окно
        /// </summary>
        public static bool isLoaded = false;

        /// <summary>
        /// Показать окно в формой
        /// </summary>
        public void ShowDlg()
        {
            Process currentProcess = Process.GetCurrentProcess();

            // Идентификатор команды вызова окна "Навигатор комментариев"
            const int wndWmCommand = 35587;
            string windowName = "Шаблоны сегментов";

            if (editIsShown == true && IsShown == true)
            {
                StaticHelper.GUIHelper.ShowHiddenWindow(currentProcess,
                    wndDevVisibilePtr, wndWmCommand);
                return;
            }

            wasShown = true;

            StaticHelper.GUIHelper.SearchWindowDescriptor(currentProcess,
                windowName, wndWmCommand, ref dialogHandle,
                ref wndDevVisibilePtr);
            if (wndDevVisibilePtr != IntPtr.Zero)
            {
                StaticHelper.GUIHelper.ShowHiddenWindow(currentProcess,
                    wndDevVisibilePtr, wndWmCommand);
                StaticHelper.GUIHelper.ChangeWindowMainPanels(ref dialogHandle,
                    ref panelPtr);

                Controls.Clear();

                // Переносим на найденное окно свои элементы (SetParent) и
                // подгоняем их размеры и позицию.
                PI.SetParent(editorRView.Handle, dialogHandle);
                PI.SetParent(toolStrip.Handle, dialogHandle);
                ChangeUISize();

                // Устанавливаем свой хук для найденного окна
                // (для изменения размеров своих элементов, сохранения
                // изменений при закрытии и отключения хука).
                SetUpHook();

                editIsShown = true;
                IsShown = true;
                cancelChanges = false;
                Editable = true;
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

        private IntPtr GlobalHookKeyboardCallbackFunction(int code,
            PI.WM wParam, PI.KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0 || editorRView == null)
            {
                return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            if (!(IsCellEditing || editorRView.Focused))
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
                    if (IsCellEditing || editorRView.Focused)
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
                            if (cancelChanges == false)
                            {
                                cancelChanges = true;
                                editorRView.FinishCellEdit();
                                return (IntPtr)1;
                            }
                        }

                        if (editorRView.Focused)
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
                            editorRView.FinishCellEdit();
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

                            if (editorRView.Focused)
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

                            if (editorRView.Focused)
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

                            if (editorRView.Focused)
                            {
                                PI.SendMessage(PI.GetFocus(),
                                    (int)PI.WM.KEYDOWN,
                                    (int)Keys.V, 0);
                                return (IntPtr)1;
                            }
                        }
                        break;

                    case (uint)Keys.Delete:                             //Delete
                        if (IsCellEditing || editorRView.Focused)
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
        public void CloseEditor()
        {
            cancelChanges = true;
            editorRView.FinishCellEdit();
            editorRView.ClearObjects();

            PI.UnhookWindowsHookEx(dialogHookPtr);
            PI.UnhookWindowsHookEx(globalKeyboardHookPtr);

            globalKeyboardHookPtr = IntPtr.Zero;

            PI.SetParent(editorRView.Handle, this.Handle);
            PI.SetParent(toolStrip.Handle, this.Handle);

 
            bool isClosingProject = true;
            ProjectManager.GetInstance().RemoveHighLighting(isClosingProject);

            System.Threading.Thread.Sleep(1);

            IsShown = false;
            editIsShown = false;
            wasInit = false;

            isLoaded = false;
            Editable = false;
        }

        /// <summary>
        /// Проверка текущего состояния окон
        /// </summary>
        public static bool CheckShown()
        {
            if (PI.IsWindowVisible(wndDevVisibilePtr) == true)
            {
                editIsShown = true;
            }
            else
            {
                editIsShown = false;
            }
            return editIsShown;
        }

        public static bool editIsShown = false; //Показано ли окно.
        public bool IsShown = false; // Костыль, без этого не работает IsShown()
        private bool IsCellEditing = false;
        public bool wasShown;
        private bool cancelChanges = false;

        private PI.HookProc dialogCallbackDelegate = null;
        private PI.LowLevelKeyboardProc mainWndKeyboardCallbackDelegate = null;

        private IntPtr globalKeyboardHookPtr = IntPtr.Zero;

        private IntPtr dialogHookPtr = IntPtr.Zero;

        private IntPtr dialogHandle = IntPtr.Zero;
        private IntPtr wndHandle = IntPtr.Zero;
        private IntPtr panelPtr = IntPtr.Zero;

        static string caption = "Рецепты\0";
        static byte[] newCapt = EncodingDetector.Windows1251.GetBytes(caption);
        private int deltaWidth = 5;

        TextBox textBoxCellEditor;
        private bool Editable;

        /// <summary>
        /// Инициализация TextBox редактора
        /// </summary>
        private void InitTextBoxCellEditor()
        {
            textBoxCellEditor = new TextBox();
            textBoxCellEditor.Enabled = true;
            textBoxCellEditor.Visible = true;
            textBoxCellEditor.LostFocus += editorRView_LostFocus;
            editorRView.Controls.Add(textBoxCellEditor);
        }

        /// <summary>
        /// Обработка нажатий кнопок уровня.
        /// </summary>
        private void toolStripButton_Click(object sender, EventArgs e)
        {
            editorRView.BeginUpdate();
            int level = Convert.ToInt32((sender as ToolStripButton).Tag);
            editorRView.SelectedIndex = 0;
            editorRView.CollapseAll();
            ExpandToLevel(level, editorRView.Objects);
            editorRView.EnsureModelVisible(editorRView.SelectedObject);
            editorRView.EndUpdate();
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
                    if (editorRView.IsExpanded(item) == false)
                    {
                        editorRView.Expand(item);
                    }
                }
                else
                {
                    if (editorRView.IsExpanded(item) == true)
                    {
                        editorRView.Collapse(item);
                    }
                }
            }
        }

        private void editorRView_Expanded(object sender,
            TreeBranchExpandedEventArgs e)
        {
            editorRView.Columns[0].AutoResize(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void editorRView_Collapsed(object sender,
            TreeBranchCollapsedEventArgs e)
        {
            editorRView.Columns[0].AutoResize(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Изменение ширины колонки и пересчет ширины другой колонки
        /// </summary>
        private void editorRView_ColumnWidthChanging(object sender,
            ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                editorRView.Columns[1].Width = editorRView.Width -
                    deltaWidth - editorRView.Columns[0].Width;
            }
        }

        /// <summary>
        /// Обработка потери фокуса редактором во время редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editorRView_LostFocus(object sender, EventArgs e)
        {
            if (IsCellEditing)
            {
                cancelChanges = true;
                editorRView.FinishCellEdit();
            }
        }

        /// <summary>
        /// Обработка начала редактирования ячеек. Установка текста 
        /// редактирования.
        /// </summary>
        private void editorRView_CellEditStarting(object sender,
            CellEditEventArgs e)
        {
            IsCellEditing = true;
            ITreeViewItem item = editorRView.SelectedObject as ITreeViewItem;

            if (item == null ||
                !item.IsEditable ||
                item.EditablePart[e.Column.Index] != e.Column.Index)
            {
                IsCellEditing = false;
                e.Cancel = true;
                return;
            }

           
                InitTextBoxCellEditor();
                textBoxCellEditor.Text = item.EditText[e.Column.Index];
                textBoxCellEditor.Bounds = e.CellBounds;
                e.Control = textBoxCellEditor;
                textBoxCellEditor.Focus();
                editorRView.Freeze();
            
        }


        /// <summary>
        /// Обработка полученных данных после редактирования.
        /// </summary>
        private void editorRView_CellEditFinishing(object sender,
            CellEditEventArgs e)
        {
            IsCellEditing = false;
            IsCellEditing = false;
            bool isModified = false;
            editorRView.LabelEdit = false;
            var item = editorRView.SelectedObject as ITreeViewItem;

            ////При нажатии Esc отменяются все изменения.
            if (cancelChanges || item == null)
            {
                e.Cancel = true;
                cancelChanges = false;
                editorRView.Unfreeze();
                return;
            }

            
            editorRView.Controls.Remove(textBoxCellEditor);
            isModified = item.SetNewValue(e.NewValue.ToString());
           

            if (isModified)
            {
                RefreshTree();
                //Обновляем также и узел родителя при его наличии.
                if (item.NeedRebuildParent)
                {
                    DisableNeededObjects(item.Parent.Items);
                }
            }

            e.Cancel = true;
            editorRView.Unfreeze();
        }

        /// <summary>
        /// Обновление формы с данными для редактирования.
        /// </summary>
        public void RefreshTree()
        {
            if (editorRView.Items.Count == 0)
            {
                editorRView.BeginUpdate();
                editorRView.ClearObjects();
                editorRView.EndUpdate();
            }
            else
            {
                editorRView.RefreshObjects(treeViewItemsList);
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
                if (item.IsFilled == false )
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
                            editorRView.DisableObject(item);
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
                            editorRView.EnableObject(item);
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
        /// Обработка изменения выбранной строки.
        /// </summary>
        private void editorRView_SelectedIndexChanged(object sender, 
            EventArgs e)
        {
            ITreeViewItem item = editorRView.SelectedObject as ITreeViewItem;
            if (item == null || noOnChange)
            {
                return;
            }

            editorRView.BeginUpdate();

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
                        DFrm.GetInstance().ShowDisplayObjects(item, SetNewVal);
                        editorRView.RefreshObjects(treeViewItemsList);
                    }
                }
            }

            editorRView.EndUpdate();
            editorRView.Focus();
        }

        /// <summary>
        /// Захват хуков при наведении курсора мыши на редактор
        /// </summary>
        private void editorRView_MouseEnter(object sender, EventArgs e)
        {
            globalKeyboardHookPtr = PI.SetWindowsHookEx(
                PI.HookType.WH_KEYBOARD_LL, mainWndKeyboardCallbackDelegate,
                IntPtr.Zero, 0);
        }

        /// <summary>
        /// Освобождение хуков после того как курсор мыши уведен
        /// с окна редактора.
        /// </summary>
        private void editorRView_MouseLeave(object sender, EventArgs e)
        {
            if (globalKeyboardHookPtr != IntPtr.Zero)
            {
                PI.UnhookWindowsHookEx(globalKeyboardHookPtr);
                globalKeyboardHookPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Обработка нажатий клавиш клавиатуры.
        /// </summary>
        private void editorRView_KeyDown(object sender, KeyEventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item == null)
            {
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
                    var copiedItem = (copyItem as ITreeViewItem);
                    if (copiedItem != null && copiedItem.MarkToCut)
                    {
                        CancelCut(copiedItem);
                    }

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

                // Вырезка существующего элемента
                if (e.KeyCode == Keys.X && e.Control == true)
                {
                    CutItem(item);
                    return;
                }

                // Отмена вырезки существующего элемента
                if (e.KeyCode == Keys.Escape)
                {
                    CancelCut(copyItem as ITreeViewItem);
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
                    if (link == null)
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
        /// Функции установки нового значения в виде строки в редакторе.
        /// </summary>
        /// <param name="newVal">Новое значение</param>
        internal void SetNewVal(string newVal)
        {
            bool isModified = (editorRView.SelectedObject as ITreeViewItem)
                .SetNewValue(newVal);

            if (isModified)
            {
                ITreeViewItem item = 
                    editorRView.SelectedObject as ITreeViewItem;
                noOnChange = true;
                editorRView.RefreshObject(item);
                noOnChange = false;
                
            }
        }

        private void addRecipeButton_Click(object sender, EventArgs e)
        {
            ITreeViewItem item = GetActiveItem();
            if (item != null && Editable == true)
            {
                CreateItem(item);
            }
        }

        private void delRecipeButton_Click(object sender, EventArgs e)
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
        /// Хранение скопированного объекта.
        /// </summary>
        private object copyItem = null;
        private bool noOnChange = false;
        public static bool isOpened;

        /// <summary>
        /// Скопировать элемент
        /// </summary>
        /// <param name="item">Копируемый элемент</param>
        private void CopyItem(ITreeViewItem item)
        {
            copyItem = item.Copy();
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
                if (newItem != null)
                {
                    editorRView.RefreshObjects(item.Items);
                    editorRView.RefreshObject(item);
                    if (item.NeedRebuildParent && item.Parent != null)
                    {
                        editorRView.RefreshObject(item.Parent);
                    }
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
                if (parent != null)
                {
                    bool isDelete = parent.Delete(item);
                    if (isDelete)
                    {
                        RefreshTree();
                    }
                    else
                    {
                        editorRView.RefreshObject(item);
                        if (item.Items != null)
                        {
                            editorRView.RefreshObjects(item.Items);
                        }
                    }
                }

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
                    var copiedItem = copyItem as ITreeViewItem;
                    if (copiedItem != null && copiedItem.MarkToCut)
                    {
                        copiedItem.MarkToCut = false;
                        copyItem = null;
                    }


                    RefreshTree();
                    DisableNeededObjects(newItem.Parent.Items);
                }
            }
        }

        /// <summary>
        /// Заменить элемент (Ctrl + B)
        /// </summary>
        /// <param name="item">Заменяемый элемент</param>
        private void ReplaceItem(ITreeViewItem item)
        {
            var copiedItem = copyItem as ITreeViewItem;
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
                    editorRView.RefreshObjects(itemParent.Items);
                    editorRView.SelectedIndex--;
                }

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
                    editorRView.RefreshObjects(itemParent.Items);
                    editorRView.SelectedIndex++;
                }

            }
        }

        /// <summary>
        /// Вырезать элемент (Ctrl + X)
        /// </summary>
        /// <param name="item"></param>
        private void CutItem(ITreeViewItem item)
        {
            if (item.Parent.IsCuttable)
            {
                var copiedItem = copyItem as ITreeViewItem;
                if (copiedItem != null && copiedItem.MarkToCut)
                {
                    CancelCut(copiedItem);
                }

                if (item.IsMainObject)
                {
                    copyItem = item;
                    item.MarkToCut = true;
                    editorRView.RefreshObject(item);
                }
            }
        }

        /// <summary>
        /// Отменить вырезку объекта.
        /// </summary>
        /// <param name="item"></param>
        private void CancelCut(ITreeViewItem item)
        {
            if (item != null && item.MarkToCut)
            {
                DisableCutting(treeViewItemsList.ToArray());
                editorRView.RefreshObject(item);
                copyItem = null;
            }
        }

        /// <summary>
        /// Снять флаг, что объект вырезан.
        /// </summary>
        private void DisableCutting(ITreeViewItem[] items)
        {
            foreach (var item in items)
            {

                if (item.MarkToCut)
                {
                    item.MarkToCut = false;
                }

                if (item.Items != null && item.Items.Length != 0)
                {
                    DisableCutting(item.Items);
                }
            }
        }


    }
}