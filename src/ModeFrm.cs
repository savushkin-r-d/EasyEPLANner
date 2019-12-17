using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PInvoke;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

using Editor;

namespace EasyEPlanner
{
    public partial class ModeFrm : Form
    {
        private ModeFrm()
        {
            InitializeComponent();

            InitTreeViewComponents();

            modesTreeViewAdv.Columns.Add(treeColumn1);
            modesTreeViewAdv.NodeControls.Add(nodeTextBox);

            // Событие рисования узлов дерева
            nodeTextBox.DrawText += new EventHandler<DrawTextEventArgs>(modesTreeViewAdv_DrawNode);

            dialogCallbackDelegate = new PI.HookProc(DlgWndHookCallbackFunction);
        }

        private static ModeFrm mFrm = null;

        ///<summary>
        ///Инициализация графических компонентов формы
        ///</summary>
        private void InitTreeViewComponents()
        {
            modesTreeViewAdv.FullRowSelect = true;
            modesTreeViewAdv.FullRowSelectActiveColor = Color.FromArgb(192, 192, 255);
            modesTreeViewAdv.FullRowSelectInactiveColor = Color.FromArgb(192, 255, 192);
            modesTreeViewAdv.GridLineStyle = GridLineStyle.Horizontal;
            modesTreeViewAdv.UseColumns = true;
            modesTreeViewAdv.ShowLines = true;
            modesTreeViewAdv.ShowPlusMinus = true;
            modesTreeViewAdv.RowHeight = 20;

            treeColumn1.Sortable = false;
            treeColumn1.Header = "Операции";
            treeColumn1.Width = 300;

            nodeCheckBox.DataPropertyName = "CheckState";
            nodeCheckBox.VerticalAlign = VerticalAlignment.Center;
            nodeCheckBox.ParentColumn = treeColumn1;
            nodeCheckBox.EditEnabled = true;

            nodeTextBox.DataPropertyName = "Text";
            nodeTextBox.VerticalAlign = VerticalAlignment.Center;
            nodeTextBox.TrimMultiLine = true;
            nodeTextBox.ParentColumn = treeColumn1;
        }

        ///<summary>
        ///Компоненты TreeView для инициализации
        ///</summary>
        TreeColumn treeColumn1 = new TreeColumn();
        NodeCheckBox nodeCheckBox = new NodeCheckBox();
        NodeTextBox nodeTextBox = new NodeTextBox();

        public static ModeFrm GetInstance()
        {
            if (mFrm == null)
            {
                mFrm = new ModeFrm();
            }

            return mFrm;
        }

        public bool isVisible()
        {
            return modeIsShown;
        }

        public static void CheckShown()
        {
            if (PI.IsWindowVisible(wndModeVisibilePtr) == true)
            {
                modeIsShown = true;
            }
            else
            {
                modeIsShown = false;
            }
        }

        public static void SaveCfg(bool wndState)
        {
            string path = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
            IniFile ini = new PInvoke.IniFile(
                path + @"\Eplan\eplan.cfg");

            if (wndState == true)
            {
                ini.WriteString("main", "show_oper_window", "true");
            }
            else
            {
                ini.WriteString("main", "show_oper_window", "false");
            }
        }

        static string caption = "Операции\0";
        static byte[] newCapt = Encoding.GetEncoding(1251).GetBytes(caption);

        private PI.HookProc dialogCallbackDelegate = null;
        private IntPtr dialogHookPtr = IntPtr.Zero;

        private IntPtr dialogHandle = IntPtr.Zero;
        private IntPtr wndHandle = IntPtr.Zero;
        private IntPtr panelPtr = IntPtr.Zero;

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
                        IntPtr dialogPtr = PI.GetParent(modesTreeViewAdv.Handle);

                        PI.RECT rctDialog;
                        PI.RECT rctPanel;
                        PI.GetWindowRect(dialogPtr, out rctDialog);
                        PI.GetWindowRect(panelPtr, out rctPanel);

                        int w = rctDialog.Right - rctDialog.Left;
                        int h = rctDialog.Bottom - rctDialog.Top;

                        toolStrip.Location =
                            new Point(0, 0);
                        modesTreeViewAdv.Location =
                            new Point(0, 0 + toolStrip.Height);

                        toolStrip.Width = w;
                        modesTreeViewAdv.Width = w;
                        modesTreeViewAdv.Height = h - toolStrip.Height;
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
                            System.Runtime.InteropServices.Marshal.PtrToStructure(
                            lParam, typeof(PI.WINDOWPOS));

                        break;

                    case (int)PI.WM.DESTROY:
                        PI.UnhookWindowsHookEx(dialogHookPtr);
                        dialogHookPtr = IntPtr.Zero;
                        dialogHandle = IntPtr.Zero;

                        PI.SetParent(modesTreeViewAdv.Handle, this.Handle);
                        PI.SetParent(toolStrip.Handle, this.Handle);
                        this.Controls.Add(modesTreeViewAdv);
                        this.Controls.Add(toolStrip);
                        modesTreeViewAdv.Hide();
                        System.Threading.Thread.Sleep(1);
                        modeIsShown = false;
                        break;

                    case (int)PI.WM.GETTEXT:
                        System.Runtime.InteropServices.Marshal.Copy(
                            newCapt, 0, lParam, newCapt.Length);
                        return (IntPtr)newCapt.Length;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        public static IntPtr wndModeVisibilePtr; // Дескриптор окна
        public static bool modeIsShown = false; // Показано ли окно.

        /// <summary>
        /// Инициализация формы данными для редактирования.
        ///
        /// Так как данная форма отображается как внутреннее окно, то алгоритм
        /// следующий:
        /// 1 Поиск окна "Основные данные изделия" (меню Сервисные программы -> 
        /// Изделие -> Навигатор основных данных изделий).
        /// 1.1 Поиск плавающего представления: через FindWindowByCaption,         
        /// потом для поиска панели и диалога DlgItemId (0xE81F - базовая панель,
        /// 0x32С8 - диалог). Если окно найдено, то переходим к 4,  иначе к 1.1.1.
        /// 1.1.1 Поиск плавающего представления: иногда не отображается заголовок
        /// из-за чего невозможно сразу определить окно, тогда проверяются потомки окон,
        /// которые могут содержать заголовок родительского окна. Если не найдены заголовки,
        /// то переходим к 1.2 (значит плавающего представления нет).
        /// 1.2 Поиск закрепленного представления: через GetDlgItem для всех дочерних
        /// окон (GetChildWindows) приложения Eplan по DlgItemId (0x32C8 - диалог).
        /// Если окно найдено, то переходим к 4, иначе к 2.
        /// 2 Симулируем нажатие пункта меню (Сервисные программы -> 
        /// Изделие -> Навигатор основных данных изделий - 35357)
        /// для его отображения.
        /// 3 Повторяем поиск окна (1.1, 1.1.1 и 1.2). Если окно не найдено выводим
        /// сообщение об ошибке, завершаем редактирование, иначе к 4.
        /// 4 Скрываем панель с элементами управления Eplan'а
        /// (GetDlgItem, 0x3E6 - родительская панель, ShowWindow).
        /// 5. Переносим на найденное окно свои элементы (SetParent) и подгоняем
        /// из размеры и позицию.
        /// 6. Устанавливаем свой хук для найденного окна (для изменения размеров
        /// своих элементов, сохранения изменений при закрытии и отключения хука).
        /// </summary>

        public void ShowDlg()
        {
            System.Diagnostics.Process oCurrent =
                System.Diagnostics.Process.GetCurrentProcess();

            const int wndWmCommand = 35093; // Идентификатор команды вызова окна "Штекеры"

            if (modeIsShown == true)
            {
                if (PI.IsWindowVisible(wndModeVisibilePtr) == false)
                {
                    PI.SendMessage(oCurrent.MainWindowHandle,
                        (uint)PI.WM.COMMAND, wndWmCommand, 0);
                    return;
                }
                return;
            }

            string windowName = "Штекеры";

            bool isDocked = false;

            IntPtr res = PI.FindWindowByCaption(
                IntPtr.Zero, windowName);                  //1.1

            if (res != IntPtr.Zero)
            {
                var resList = PI.GetChildWindows(res);
                if (resList.Count > 0)
                {
                    wndHandle = PI.GetParent(resList[0]);
                    dialogHandle = resList[0];
                    wndModeVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
                }
            }
            else
            {
                StringBuilder stringBuffer = new StringBuilder(200);        //1.1.1

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
                                        wndModeVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                List<IntPtr> resW = PI.GetChildWindows(oCurrent.MainWindowHandle);        //1.2
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
                                isDocked = true;
                                res = dialogHandle;
                                wndModeVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
                                break;
                            }
                        }

                    }
                }

                if (res == IntPtr.Zero)
                {
                    PI.SendMessage(oCurrent.MainWindowHandle,
                        (uint)PI.WM.COMMAND, wndWmCommand, 0);                //2

                    res = PI.FindWindowByCaption(
                        IntPtr.Zero, windowName);          //3

                    if (res != IntPtr.Zero)
                    {
                        var resList = PI.GetChildWindows(res);
                        if (resList.Count > 0)
                        {
                            dialogHandle = resList[0];
                            wndHandle = PI.GetParent(resList[0]);
                            wndModeVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                                            wndModeVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                                    isDocked = true;
                                    wndModeVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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

            var panelList = PI.GetChildWindows(dialogHandle);           //4
            if (panelList.Count > 0)
            {
                panelPtr = panelList[0];
            }

            if (panelPtr == IntPtr.Zero)
            {
                MessageBox.Show("Не удалось скрыть окно!");
                return;
            }

            PI.ShowWindow(panelPtr, 0);
            this.Controls.Clear();

            modesTreeViewAdv.Show();
            PI.SetParent(modesTreeViewAdv.Handle, dialogHandle);         //5 
            PI.SetParent(toolStrip.Handle, dialogHandle);

            int dy = 0;
            if (isDocked)
            {
                dy = 2;
                //TODO: Доработать открытие формы в окне.
            }

            PI.RECT dialogRect;
            PI.GetWindowRect(dialogHandle, out dialogRect);

            toolStrip.Location = new Point(0, dy);
            modesTreeViewAdv.Location = new Point(0, dy + toolStrip.Height);

            int w = dialogRect.Right - dialogRect.Left;
            int h = dialogRect.Bottom - dialogRect.Top - toolStrip.Height - dy;

            modesTreeViewAdv.Width = w;
            modesTreeViewAdv.Height = h;
            toolStrip.Width = w;

            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);        //6
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);

            PI.SetWindowText(dialogHandle, caption);
            PI.SetWindowText(wndHandle, caption);

            modeIsShown = true;
        }

        public delegate void OnSetNewValue(SortedDictionary<int, List<int>> dict);
        public OnSetNewValue functionAfterCheck = null;

        /// <summary>
        /// Выбор операции на дереве, которые входят в ограничения.
        /// </summary> 
        /// <param name="checkedDev">Объект, в котором хранятся ограничения.</param>
        /// <param name="fn">Делегат, который вызывается при последующих изменениях операций в ограничениях.</param>
        public void SelectDevices(ITreeViewItem checkedDev, OnSetNewValue fn)
        {
            modesTreeViewAdv.BeginUpdate();

            foreach (TreeNodeAdv boxedNode in modesTreeViewAdv.AllNodes)
            {
                Node node = boxedNode.Tag as Node;
                node.CheckState = CheckState.Unchecked;
            }

            nodeCheckBox.CheckStateChanged -= new EventHandler<TreePathEventArgs>(treeItem_AfterCheck);

            if (fn != null)
            {
                functionAfterCheck = fn;
            }

            TreeModel treeModel = modesTreeViewAdv.Model as TreeModel;
            List<Node> nodes = treeModel.Nodes.ToList();
            selectedDevices(nodes, checkedDev);

            nodeCheckBox.CheckStateChanged += new EventHandler<TreePathEventArgs>(treeItem_AfterCheck);

            modesTreeViewAdv.EndUpdate();
        }

        /// <summary>
        /// Переключение состояния чекбоксов операции
        /// </summary>
        /// <param name="nodes">узлы</param>
        /// <param name="checkedMode">выбранные (отмеченные) операции</param>
        private void selectedDevices(List<Node> nodes, ITreeViewItem checkedMode)
        {
            foreach (Node subNode in nodes)
            {
                if (subNode.Tag.ToString() == "TechObject.Mode")
                {
                    Node parentNode = subNode.Parent;
                    string checkStr = "{ " + (parentNode.Parent.Nodes.IndexOf(parentNode) + 1).ToString() + ", " +
                            (nodes.IndexOf(subNode) + 1).ToString() + " }";

                    TechObject.Restriction restriction = checkedMode as TechObject.Restriction;

                    // Если не ограничения, то выходим из функции
                    if (restriction == null)
                    {
                        return;
                    }

                    if (restriction.RestrictDictionary != null)
                    {
                        if (restriction.RestrictDictionary.ContainsKey(parentNode.Parent.Nodes.IndexOf(parentNode) + 1))
                        {
                            if (restriction.RestrictDictionary[
                                parentNode.Parent.Nodes.IndexOf(parentNode) + 1].Contains(
                                nodes.IndexOf(subNode) + 1))
                            {
                                subNode.CheckState = CheckState.Checked;

                                // Выставляем состояние родителя
                                RecursiveCheckParent(subNode.Parent);

                                // Выставляем состояние узла
                                RecursiveCheck(subNode);
                            }
                        }
                    }

                }
                selectedDevices(subNode.Nodes.ToList(), checkedMode);
            }
        }

        /// <summary>
        /// Отключение видимости всех операций
        /// </summary>
        /// <returns></returns>
        public bool ShowNoModes()
        {
            modesTreeViewAdv.BeginUpdate();

            TreeModel model = new TreeModel();
            model.Nodes.Clear();

            Node root = new Node("Операции проекта");
            model.Nodes.Add(root);
            modesTreeViewAdv.Model = model;

            modesTreeViewAdv.EndUpdate();

            return true;
        }

        /// <summary>
        /// Отключиние видимости операций в дереве
        /// </summary>
        static class OnHideOperationTree
        {
            public static void Execute(TreeNodeAdv treeNode)
            {
                int correctionForTreeLevelWithNewControl = 1;

                Node node = treeNode.Tag as Node;
                if (treeNode.Level == 1 + correctionForTreeLevelWithNewControl && treeNode.Children.Count < 1)
                {
                    treeNode.IsHidden = true;
                    return;
                }

                if (treeNode.Children.Count > 0)
                {
                    bool isHidden = true;
                    List<TreeNodeAdv> childs = treeNode.Children.ToList();
                    foreach (TreeNodeAdv childTree in childs)
                    {
                        Node childNode = childTree.Tag as Node;
                        if (childNode.IsHidden == false)
                        {
                            isHidden = false;
                            Execute(childTree);
                        }

                        treeNode.IsHidden = isHidden;
                    }
                }

                if (node.IsHidden == true)
                {
                    node.IsHidden = false; // Костыль, иначе не скрывает.
                    treeNode.IsHidden = true;
                }
            }
        }

        /// <summary>
        /// Обновление дерева на основе текущих устройств проекта.
        /// </summary>
        /// 
        /// <param name="techManager">Менеджер техустройств проекта.</param>
        /// <param name="checkedMode">Выбранные операиции.</param>
        private void Refresh(TechObject.TechObjectManager techManager,
            ITreeViewItem checkedMode, bool showOneNode, ITreeViewItem item)
        {
            modesTreeViewAdv.BeginUpdate();

            modesTreeViewAdv.Model = null;
            modesTreeViewAdv.Refresh();
            TreeModel treeModel = new TreeModel();

            Node root = new Node(techManager.DisplayText[0]);
            root.Tag = techManager.GetType().FullName;
            treeModel.Nodes.Add(root);

            int toNum = 0;
            int modeNum = 0;
            TechObject.TechObject mainTO = item as TechObject.TechObject;
            //Заполняем узлы дерева устройствами.
            foreach (TechObject.TechObject to in techManager.Objects)
            {
                toNum++;

                Node parentNode = new Node(to.DisplayText[0]);
                parentNode.Tag = to.GetType().FullName;
                root.Nodes.Add(parentNode);

                List<TechObject.Mode> modes = to.GetModesManager.GetModes;

                foreach (TechObject.Mode mode in modes)
                {
                    modeNum++;

                    Node childNode = new Node(mode.DisplayText[0]);
                    childNode.Tag = mode.GetType().FullName;
                    parentNode.Nodes.Add(childNode);

                    string checkedStr;
                    if (checkedMode != null)
                    {
                        checkedStr = checkedMode.EditText[1];
                        TechObject.Restriction restrict = checkedMode as TechObject.Restriction;

                        if (restrict != null)
                        {
                            if (restrict.RestrictDictionary != null)
                            {
                                if (restrict.RestrictDictionary.ContainsKey(toNum))
                                {
                                    if (restrict.RestrictDictionary[toNum].Contains(modeNum))
                                    {
                                        childNode.CheckState = CheckState.Checked;
                                    }
                                    else
                                    {
                                        childNode.CheckState = CheckState.Unchecked;
                                    }
                                }

                            }
                        }



                    }
                    else
                    {
                        checkedStr = "";
                        childNode.CheckState = CheckState.Unchecked;
                    }

                }

                if (showOneNode == true)
                {
                    if (to != mainTO)
                    {
                        parentNode.IsHidden = true;
                        foreach (Node child in parentNode.Nodes)
                        {
                            child.IsHidden = true;
                        }
                    }
                }
                else
                {
                    if (to == mainTO)
                    {
                        parentNode.IsHidden = true;
                        foreach (Node child in parentNode.Nodes)
                        {
                            child.IsHidden = true;
                        }
                    }
                }

            }

            modesTreeViewAdv.Model = treeModel;
            //Обновляем названия строк (добавляем количество объектов).

            List<TreeNodeAdv> nodes = modesTreeViewAdv.AllNodes.ToList();
            TreeNodeAdv treeNode = nodes[0];
            OnHideOperationTree.Execute(treeNode);

            modesTreeViewAdv.ExpandAll();
            modesTreeViewAdv.Refresh();
            modesTreeViewAdv.EndUpdate();
        }

        /// <summary>
        /// Построение дерева на основе определенных операций проекта.
        /// </summary>   
        public bool ShowModes(TechObject.TechObjectManager techManager,
           bool showCheckBoxes, bool showOneNode, ITreeViewItem item, ITreeViewItem checkedMode,
           OnSetNewValue fn, bool isRebuiltTree = false)
        {

            if (fn != null)
            {
                functionAfterCheck = fn;
            }

            if (showCheckBoxes)
            {
                modesTreeViewAdv.NodeControls.Insert(0, nodeCheckBox);
            }
            else
            {
                modesTreeViewAdv.NodeControls.Remove(nodeCheckBox);
            }


            //Проверяем на изменение типов отображаемых операций.
            if (isRebuiltTree == false)
            {
                ShowDlg();
                return true;
            }

            Refresh(techManager, checkedMode, showOneNode, item);

            ShowDlg();
            return true;
        }

        /// <summary>
        /// Выбор устройств, участвующих в операции.
        /// </summary>         
        static class OnCheckOperationTree
        {
            static string res = "";
            static SortedDictionary<int, List<int>> resDict = new SortedDictionary<int, List<int>>();

            public static void ResetResStr()
            {
                res = "";
            }

            public static void ResetResDict()
            {
                resDict.Clear();
            }

            public static string GetResStr()
            {
                return res;
            }

            public static SortedDictionary<int, List<int>> GetResDict()
            {
                return resDict;
            }

            public static void Execute(TreeNodeAdv treeNode)
            {
                Node node = treeNode.Tag as Node;
                if (node.CheckState == CheckState.Checked && node.Tag.ToString() == "TechObject.Mode")
                {
                    Node parentNode = node.Parent;
                    string idx = "{ " + (parentNode.Parent.Nodes.IndexOf(parentNode) + 1).ToString();
                    res += idx + ", " + (parentNode.Nodes.IndexOf(node) + 1).ToString() + " } ";

                    //*************************************************************//
                    if (resDict.ContainsKey(parentNode.Parent.Nodes.IndexOf(parentNode) + 1))
                    {
                        resDict[parentNode.Parent.Nodes.IndexOf(parentNode) + 1].Add(parentNode.Nodes.IndexOf(node) + 1);
                    }
                    else
                    {
                        List<int> modeList = new List<int>();
                        modeList.Add(parentNode.Nodes.IndexOf(node) + 1);
                        resDict.Add(parentNode.Parent.Nodes.IndexOf(parentNode) + 1, modeList);
                    }
                    //*************************************************************//          
                }

                if (treeNode.Children.Count > 0)
                {
                    List<TreeNodeAdv> childs = treeNode.Children.ToList();
                    foreach (TreeNodeAdv child in childs)
                    {
                        Execute(child);
                    }
                }
            }
        }

        /// <summary>
        /// Обработка выбора операции для добавления в ограничение.
        /// После выбора операции мы передаем новый словарь с ограничениями
        /// через делегат functionAfterCheck.
        /// </summary>        
        private void treeItem_AfterCheck(object sender, TreePathEventArgs e)
        {
            if (functionAfterCheck != null)
            {
                OnCheckOperationTree.ResetResStr();
                OnCheckOperationTree.ResetResDict();

                string res = "";
                SortedDictionary<int, List<int>> resDict = new SortedDictionary<int, List<int>>();

                treeItem_ChangeCheckBoxState(sender, e);

                List<TreeNodeAdv> treeNodes = modesTreeViewAdv.AllNodes.ToList();
                TreeNodeAdv treeNode = treeNodes[0];
                OnCheckOperationTree.Execute(treeNode);

                res = OnCheckOperationTree.GetResStr();
                resDict = OnCheckOperationTree.GetResDict();

                modesTreeViewAdv.Refresh();

                functionAfterCheck(resDict);
            }
        }

        /// <summary>
        /// Функция обновления состояний чекбоксов
        /// </summary>
        /// <param name="sender">Объект, который вызвал функцию</param>
        /// <param name="e">Контекст переданный вызывающим кодом</param>
        private void treeItem_ChangeCheckBoxState(object sender, TreePathEventArgs e)
        {
            // Нажатый узел дерева
            object nodeObject = e.Path.LastNode;
            Node checkedNode = nodeObject as Node;

            // Выставляем состояние родителя
            RecursiveCheckParent(checkedNode.Parent);

            // Выставляем состояние узла
            RecursiveCheck(checkedNode);
        }

        /// <summary>
        /// Функция установки состояния
        /// отображения узла
        /// </summary>
        /// <param name="node">Выбранный узел</param>
        private void RecursiveCheck(Node node)
        {
            // Если есть потомки
            if (node.Nodes.Count > 0)
            {
                List<Node> childNodes = node.Nodes.ToList();

                foreach (Node child in childNodes)
                {
                    if (child.IsHidden != true)
                    {
                        // Такое же состояние child'у, как и у node
                        child.CheckState = node.CheckState;
                        RecursiveCheck(child);
                    }
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Установка состояния отображения
        /// для родительского узла выбранного элемента
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        private void RecursiveCheckParent(Node parentNode)
        {
            // 0 - корень (но не Root)
            if (parentNode.Index > -1)
            {
                int countOfCheckedNodes = 0;
                int countOfIndeterminateNodes = 0;
                int countOfNodes = parentNode.Nodes.Count;
                foreach (Node node in parentNode.Nodes)
                {
                    if (node.CheckState == CheckState.Checked)
                    {
                        countOfCheckedNodes++;
                    }

                    if (node.CheckState == CheckState.Indeterminate)
                    {
                        countOfIndeterminateNodes++;
                    }

                    // Т.к учитывает скрытые в nodes.count
                    if (node.IsHidden == true)
                    {
                        countOfNodes--;
                    }
                }

                if (parentNode.CheckState != CheckState.Indeterminate)
                {
                    parentNode.CheckState = CheckState.Indeterminate;
                }

                if (countOfCheckedNodes == countOfNodes)
                {
                    parentNode.CheckState = CheckState.Checked;
                }

                if (countOfCheckedNodes == 0 && countOfIndeterminateNodes == 0)
                {
                    parentNode.CheckState = CheckState.Unchecked;
                }

                RecursiveCheckParent(parentNode.Parent);
            }
        }

        /// <summary>
        /// Обработка нажатий на кнопки уровней.
        /// </summary>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            modesTreeViewAdv.BeginUpdate();

            int level = Convert.ToInt32((sender as ToolStripButton).Tag);
            modesTreeViewAdv.CollapseAll();
            if (level >= 0)
            {
                ExpandToLevel(level);
            }

            modesTreeViewAdv.EndUpdate();
        }

        /// <summary>
        /// Развертывание дерева до определенного уровня
        /// </summary>       
        /// <param name="level">Уровень развертывания дерева</param>
        private void ExpandToLevel(int level)
        {
            foreach (TreeNodeAdv node in modesTreeViewAdv.AllNodes)
            {
                if (node.Level <= level)
                {
                    node.Expand();
                }
            }
        }

        /// <summary>
        /// Событие отрисовки дерева
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Контекст переданный событием</param>
        private void modesTreeViewAdv_DrawNode(object sender, DrawTextEventArgs e)
        {
            e.TextColor = Color.Black;
        }
    }
}
