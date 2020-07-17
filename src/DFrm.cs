﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PInvoke;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace EasyEPlanner
{
    public partial class DFrm : Form
    {
        private static DFrm frm = null;

        public static DFrm GetInstance()
        {
            if (frm == null)
            {
                frm = new DFrm();
            }

            return frm;
        }

        public bool IsVisible()
        {
            return deviceIsShown;
        }

        public static void CheckShown()
        {
            if (PI.IsWindowVisible(wndDevVisibilePtr) == true)
            {
                deviceIsShown = true;
            }
            else
            {
                deviceIsShown = false;
            }
        }

        public static void SaveCfg(bool wndState)
        {
            string path = Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData);
            var ini = new IniFile(path + @"\Eplan\eplan.cfg");
            if (wndState == true)
            {
                ini.WriteString("main", "show_dev_window", "true");
            }
            else
            {
                ini.WriteString("main", "show_dev_window", "false");
            }
        }

        static string caption = "Устройства\0";
        static byte[] newCapt = Encoding.GetEncoding(1251).GetBytes(caption);

        private PI.HookProc dialogCallbackDelegate = null;
        private IntPtr dialogHookPtr = IntPtr.Zero;

        private IntPtr dialogHandle = IntPtr.Zero;
        private IntPtr wndHandle = IntPtr.Zero;
        private IntPtr panelPtr = IntPtr.Zero;

        /// <summary>
        /// Функция для обработки завершения работы окна устройств.
        /// </summary>
        public void CloseEditor()
        {
            PI.UnhookWindowsHookEx(dialogHookPtr);

            PI.SetParent(devicesTreeViewAdv.Handle, this.Handle);
            PI.SetParent(toolStrip.Handle, this.Handle);

            System.Threading.Thread.Sleep(1);
            deviceIsShown = false;
        }

        #region Dialog window hook
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
                        IntPtr dialogPtr = 
                            PI.GetParent(devicesTreeViewAdv.Handle);

                        PI.RECT rctDialog;
                        PI.RECT rctPanel;
                        PI.GetWindowRect(dialogPtr, out rctDialog);
                        PI.GetWindowRect(panelPtr, out rctPanel);

                        int w = rctDialog.Right - rctDialog.Left;
                        int h = rctDialog.Bottom - rctDialog.Top;

                        toolStrip.Location =
                            new Point(0, 0);
                        devicesTreeViewAdv.Location =
                            new Point(0, 0 + toolStrip.Height);
                        toolStrip.Width = w;
                        devicesTreeViewAdv.Width = w;
                        devicesTreeViewAdv.Height = h - toolStrip.Height;
                        devicesTreeViewAdv.AutoSizeColumn(nodeColumn);
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

                        PI.SetParent(devicesTreeViewAdv.Handle, this.Handle);
                        PI.SetParent(toolStrip.Handle, this.Handle);
                        this.Controls.Add(devicesTreeViewAdv);
                        this.Controls.Add(toolStrip);
                        devicesTreeViewAdv.Hide();
                        System.Threading.Thread.Sleep(1);
                        deviceIsShown = false;
                        break;

                    case (int)PI.WM.GETTEXT:
                        System.Runtime.InteropServices.Marshal.Copy(
                            newCapt, 0, lParam, newCapt.Length);
                        return (IntPtr)newCapt.Length;
                }
            }

            return PI.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
        #endregion

        #region ShowDialog
        public static IntPtr wndDevVisibilePtr; // Дескриптор окна устройств
        public static bool deviceIsShown = false; // Показано ли окно.

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

            const int wndWmCommand = 35357; // Идентификатор команды вызова окна "Основные данные изделия"

            if (deviceIsShown == true)
            {
                if (PI.IsWindowVisible(wndDevVisibilePtr) == false)
                {
                    PI.SendMessage(oCurrent.MainWindowHandle,
                        (uint)PI.WM.COMMAND, wndWmCommand, 0);
                    return;
                }
                return;
            }

            string windowName = "Основные данные изделия";

            IntPtr res = PI.FindWindowByCaption(
                IntPtr.Zero, windowName);                  //1.1
            if (res != IntPtr.Zero)
            {
                var resList = PI.GetChildWindows(res);
                if (resList.Count > 0)
                {
                    wndHandle = PI.GetParent(resList[0]);
                    dialogHandle = resList[0];
                    wndDevVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                                        wndDevVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                                wndDevVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                        IntPtr.Zero, windowName);                              //3

                    if (res != IntPtr.Zero)
                    {
                        var resList = PI.GetChildWindows(res);
                        if (resList.Count > 0)
                        {
                            dialogHandle = resList[0];
                            wndHandle = PI.GetParent(resList[0]);
                            wndDevVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                                            wndDevVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
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
                                    wndDevVisibilePtr = dialogHandle; // Сохраняем дескриптор окна.
                                    break;
                                }
                            }
                        }

                        if (dialogHandle == IntPtr.Zero)
                        {
                            MessageBox.Show(
                                "Не удалось найти окно!");
                            return;
                        }
                    }
                }
            }

            var panelList = PI.GetChildWindows(dialogHandle);               //4
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

            devicesTreeViewAdv.Show();
            PI.SetParent(devicesTreeViewAdv.Handle, dialogHandle);                //5 
            PI.SetParent(toolStrip.Handle, dialogHandle);

            IntPtr dialogPtr = PI.GetParent(devicesTreeViewAdv.Handle);

            PI.RECT rctDialog;
            PI.RECT rctPanel;
            PI.GetWindowRect(dialogPtr, out rctDialog);
            PI.GetWindowRect(panelPtr, out rctPanel);

            int w = rctDialog.Right - rctDialog.Left;
            int h = rctDialog.Bottom - rctDialog.Top;

            toolStrip.Location =
                new Point(0, 0);
            devicesTreeViewAdv.Location =
                new Point(0, 0 + toolStrip.Height);
            toolStrip.Width = w;
            devicesTreeViewAdv.Width = w;
            devicesTreeViewAdv.Height = h - toolStrip.Height;
            devicesTreeViewAdv.AutoSizeColumn(nodeColumn);

            uint pid = PI.GetWindowThreadProcessId(dialogHandle, IntPtr.Zero);        //6
            dialogHookPtr = PI.SetWindowsHookEx(PI.HookType.WH_CALLWNDPROC,
                dialogCallbackDelegate, IntPtr.Zero, pid);

            PI.SetWindowText(dialogHandle, caption);
            PI.SetWindowText(wndHandle, caption);

            deviceIsShown = true;
        }
        #endregion

        private DFrm()
        {
            InitializeComponent();

            InitTreeViewComponents();
            devicesTreeViewAdv.Columns.Add(nodeColumn);
            devicesTreeViewAdv.NodeControls.Add(nodeTextBox);

            // Событие рисования узлов дерева
            nodeTextBox.DrawText += new EventHandler<DrawTextEventArgs>(
                TreeViewAdv1_DrawNode);

            dialogCallbackDelegate = new PI.HookProc(
                DlgWndHookCallbackFunction);
        }

        ///<summary>
        ///Инициализация графических компонентов формы
        ///</summary>
        private void InitTreeViewComponents()
        {
            devicesTreeViewAdv.FullRowSelect = true;
            devicesTreeViewAdv.FullRowSelectActiveColor = Color
                .FromArgb(192, 192, 255);
            devicesTreeViewAdv.FullRowSelectInactiveColor = Color
                .FromArgb(192, 255, 192);
            devicesTreeViewAdv.GridLineStyle = GridLineStyle.Horizontal;
            devicesTreeViewAdv.UseColumns = true;
            devicesTreeViewAdv.ShowLines = true;
            devicesTreeViewAdv.ShowPlusMinus = true;
            devicesTreeViewAdv.RowHeight = 20;
            devicesTreeViewAdv.SelectionMode = TreeSelectionMode.Single;

            nodeColumn.Sortable = false;
            nodeColumn.Header = "Устройства";
            nodeColumn.Width = 300;

            nodeCheckBox.DataPropertyName = "CheckState";
            nodeCheckBox.VerticalAlign = VerticalAlignment.Center;
            nodeCheckBox.ParentColumn = nodeColumn;
            nodeCheckBox.EditEnabled = true;

            nodeTextBox.DataPropertyName = "Text";
            nodeTextBox.VerticalAlign = VerticalAlignment.Center;
            nodeTextBox.ParentColumn = nodeColumn;
        }

        ///<summary>
        ///Компоненты TreeView для инициализации
        ///</summary>
        TreeColumn nodeColumn = new TreeColumn();
        NodeCheckBox nodeCheckBox = new NodeCheckBox();
        NodeTextBox nodeTextBox = new NodeTextBox();
        // Коррекция уровней дерева для нового элемента управления
        private static int correctionValue = 1;

        public delegate void OnSetNewValue(string str);
        public OnSetNewValue functionAfterCheck = null;

        /// <summary>
        /// Выбор устройства на дереве, которые входят в шаг.
        /// </summary> 
        /// <param name="checkedDev">Строка с устройствами шага, 
        /// которых надо отметить.</param>
        /// <param name="fn">Делегат, который вызывается при 
        /// последующих изменениях устройств шага.</param>
        public void SelectDevices(string checkedDev, OnSetNewValue fn)
        {
            devicesTreeViewAdv.BeginUpdate();

            foreach (TreeNodeAdv boxedNode in devicesTreeViewAdv.AllNodes)
            {
                var node = boxedNode.Tag as Node;
                node.CheckState = CheckState.Unchecked;
            }

            checkedDev = ' ' + checkedDev + ' ';

            nodeCheckBox.CheckStateChanged -= 
                new EventHandler<TreePathEventArgs>(treeItem_AfterCheck);

            if (fn != null)
            {
                functionAfterCheck = fn;
            }

            var treeModel = devicesTreeViewAdv.Model as TreeModel;
            List<Node> nodes = treeModel.Nodes.ToList();
            SelectedDevices(nodes, checkedDev);

            nodeCheckBox.CheckStateChanged += 
                new EventHandler<TreePathEventArgs>(treeItem_AfterCheck);

            devicesTreeViewAdv.EndUpdate();
        }

        /// <summary>
        /// Рекурсивная функция отметки галочками узлов
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="checkedDev"></param>
        private void SelectedDevices(List<Node> nodes, string checkedDev)
        {
            foreach (Node subNode in nodes)
            {
                if (subNode.Tag is Device.Device)
                {
                    string devName = $" {(subNode.Tag as Device.Device).Name} ";
                    if (checkedDev.Contains(devName))
                    {
                        subNode.CheckState = CheckState.Checked;

                        // Выставляем состояние родителя
                        RecursiveCheckParent(subNode.Parent);

                        // Выставляем состояние узла
                        RecursiveCheck(subNode);
                    }
                }
                SelectedDevices(subNode.Nodes.ToList(), checkedDev);
            }
        }

        /// <summary>
        /// Скрыть все устройства
        /// </summary>
        /// <returns></returns>
        public bool ShowNoDevices()
        {
            devicesTreeViewAdv.BeginUpdate();

            var model = new TreeModel();
            model.Nodes.Clear();

            var root = new Node("Устройства проекта");
            model.Nodes.Add(root);
            devicesTreeViewAdv.Model = model;

            devicesTreeViewAdv.EndUpdate();
            devTypesLastSelected = null;
            devSubTypesLastSelected = null;

            return true;
        }

        private Device.DeviceType[] devTypesLastSelected = null;
        private Device.DeviceSubType[] devSubTypesLastSelected = null;
        private bool prevShowChannels = false;
        private bool prevShowCheckboxes = false;

        /// <summary>
        /// Обновления видимости дерева устройств
        /// </summary>
        static class RefreshOperationTree
        {
            public static void Execute(TreeNodeAdv treeNode)
            {
                var node = treeNode.Tag as Node;

                if(node.Tag is Device.IODevice)
                {
                    RefreshDevice(node, treeNode);
                }
                else if(node.Tag is Device.IODevice.IOChannel)
                {
                    RefreshChannel(node, treeNode);
                }
                else
                {
                    RefreshOthers(node, treeNode);
                }
            }

            /// <summary>
            /// Обновить устройства
            /// </summary>
            private static void RefreshDevice(Node node, TreeNodeAdv treeNode)
            {
                var dev = node.Tag as Device.IODevice;

                bool isDevHidden = true;
                foreach (Device.IODevice.IOChannel ch in dev.Channels)
                {
                    if (ch.IsEmpty())
                    {
                        //Показываем каналы.
                        isDevHidden = false;
                    }
                }

                node.IsHidden = isDevHidden;

                if (treeNode.Children.Count < 1)
                {
                    return;
                }
                else
                {
                    RefreshChildRecursive(treeNode);
                }

            }

            /// <summary>
            /// Обновить канал
            /// </summary>
            private static void RefreshChannel(Node node, TreeNodeAdv treeNode)
            {
                var chn = node.Tag as Device.IODevice.IOChannel;
                if (chn.IsEmpty())
                {
                    node.IsHidden = false;
                }
                else
                {
                    node.IsHidden = true;
                }

                if (treeNode.Children.Count < 1)
                {
                    return;
                }
                else
                {
                    RefreshChildRecursive(treeNode);
                }
            }

            /// <summary>
            /// Обновить остальные узлы
            /// </summary>
            private static void RefreshOthers(Node node, TreeNodeAdv treeNode)
            {
                if (treeNode.Children.Count < 1)
                {
                    node.IsHidden = false;
                }
                else
                {
                    RefreshChildRecursive(treeNode);
                }
            }

            /// <summary>
            /// Рекурсивный вызов обновления дочерних элементов
            /// </summary>
            private static void RefreshChildRecursive(TreeNodeAdv treeNode)
            {
                List<TreeNodeAdv> childs = treeNode.Children.ToList();
                foreach (TreeNodeAdv child in childs)
                {
                    Execute(child);
                }

                return;
            }
        }

        /// <summary>
        /// Скрытие пустых узлов
        /// </summary>
        static class OnHideOperationTree
        {
            public static void Execute(TreeNodeAdv treeNode, 
                bool isHiddenNodeFull = false)
            {
                var node = treeNode.Tag as Node;

                bool firstTreeLevel = treeNode.Level == 1 + correctionValue;
                bool notExistChildren = firstTreeLevel &&
                    treeNode.Children.Count < 1;
                bool existChildren = firstTreeLevel &&
                    treeNode.Children.Count >= 1;

                if (notExistChildren)
                {
                    treeNode.IsHidden = true;
                    return;
                }
                else if (existChildren)
                {
                    // Проверка, есть ли устройства, которые не отображаются
                    if (node.Text.Contains("(0)"))
                    {
                        treeNode.IsHidden = true;
                        List<TreeNodeAdv> childs = treeNode.Children.ToList();
                        foreach (TreeNodeAdv child in childs)
                        {
                            Execute(child, true);
                        }
                        return;
                    }
                }

                if (treeNode.Children.Count > 0)
                {
                    bool isHidden = true;
                    List<TreeNodeAdv> childs = treeNode.Children.ToList();
                    foreach (TreeNodeAdv child in childs)
                    {
                        if (isHiddenNodeFull == true)
                        {
                            Execute(child, true);
                        }

                        if (child.IsHidden == false)
                        {
                            isHidden = false;
                            Execute(child);
                        }

                        treeNode.IsHidden = isHidden;
                    }
                }
            }
        }

        #region Refresh
        /// <summary>
        /// Обновление дерева на основе текущих устройств проекта.
        /// </summary>
        /// <param name="deviceManager">Менеджер устройств проекта.</param>
        /// <param name="checkedDev">Выбранные устройства.</param>
        private void Refresh(Device.DeviceManager deviceManager,
            string checkedDev)
        {
            devicesTreeViewAdv.BeginUpdate();

            devicesTreeViewAdv.Model = null;
            devicesTreeViewAdv.Refresh();
            var treeModel = new TreeModel();

            string mainNodeName = "Устройства проекта";
            var root = new Node(mainNodeName);
            treeModel.Nodes.Add(root);

            // Подтипы, которые отдельно записываем в устройства
            var devicesSubTypesEnum = new string[]
            {
                "AI_VIRT",
                "AO_VIRT",
                "DI_VIRT",
                "DO_VIRT"
            };
            object[] devicesArray = GenerateArrayObjectsForFill(
                devicesSubTypesEnum);
            FillMainDevicesInNode(root, devicesArray);

            Dictionary<string, int> countDev = MakeDevicesCounterDictionary(
                root.Nodes);
            foreach (Device.IODevice dev in deviceManager.Devices)
            {
                string deviceDescription = GenerateDeviceDescription(dev);
                string devSubType = dev.GetDeviceSubTypeStr(dev.DeviceType,
                    dev.DeviceSubType);
                if (devicesSubTypesEnum.Contains(devSubType))
                {
                    FillSubTypeNode(dev, root, deviceDescription, countDev);
                }
                else
                {
                    FillTypeNode(dev, root, deviceDescription, countDev,
                        checkedDev);
                }
            }

            UpdateDevicesCountInHeaders(devicesArray, root, countDev);
            SortTreeView(treeModel);

            devicesTreeViewAdv.Model = treeModel;

            List<TreeNodeAdv> nodes = devicesTreeViewAdv.AllNodes.ToList();
            TreeNodeAdv treeNode = nodes[0];
            OnHideOperationTree.Execute(treeNode);

            devicesTreeViewAdv.ExpandAll();
            devicesTreeViewAdv.Refresh();
            devicesTreeViewAdv.EndUpdate();
        }

        /// <summary>
        /// Сгенерировать массив со список типов и подтипов для заполнения
        /// </summary>
        /// <param name="subTypes">Список подтипов добавляемых в узлы</param>
        /// <returns></returns>
        private object[] GenerateArrayObjectsForFill(string[] subTypes)
        {
            Array devicesTypesEnum = Enum.GetValues(typeof(Device.DeviceType));
            int publicLength = devicesTypesEnum.Length + subTypes.Length;
            var objectArray = new object[publicLength];
            
            int index = 0;       
            foreach(Device.DeviceType devType in devicesTypesEnum)
            {
                objectArray[index] = devType;
                index++;
            }
            
            foreach(var devSubType in subTypes)
            {
                objectArray[index] = devSubType;
                index++;
            }

            return objectArray;
        }

        /// <summary>
        /// Заполнить узел типами и подтипами устройств
        /// </summary>
        /// <param name="deviceEnum">Список типов и подтипов для добавления
        /// </param>
        /// <param name="root">Главный узел</param>
        private void FillMainDevicesInNode(Node root, object[] deviceEnum)
        {
            foreach (var devType in deviceEnum)
            {
                var r = new Node(devType.ToString());
                if (devType is Device.DeviceType)
                {
                    r.Tag = (Device.DeviceType)devType;
                }
                else
                {
                    var devSubType = (Device.DeviceSubType)Enum
                        .Parse(typeof(Device.DeviceSubType),
                        devType.ToString());
                    r.Tag = devSubType;
                }
                root.Nodes.Add(r);
            }
        }

        /// <summary>
        /// Генерация словаря с количеством устройств каждого типа
        /// </summary>
        /// <param name="nodes">Коллекция узлов с названиями типов и подтипов
        /// </param>
        /// <returns></returns>
        private Dictionary<string, int> MakeDevicesCounterDictionary(
            IEnumerable<Node> nodes)
        {
            var devicesText = new Dictionary<string, int>();
            foreach (var node in nodes)
            {
                int defaultValue = 0;
                devicesText.Add(node.Text, defaultValue);
            }

            return devicesText;
        }

        /// <summary>
        /// Заполнить узел для типа устройства
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <param name="root">Главный узел</param>
        /// <param name="deviceDescription">Описание устройства</param>
        /// <param name="countDev">Словарь для подсчета количества</param>
        /// <param name="checkedDev">Выбранные устройств в действии</param>
        private void FillTypeNode(Device.IODevice dev, Node root,
            string deviceDescription, Dictionary<string, int> countDev,
            string checkedDev)
        {
            Node devTypeNode = FindDevTypeNode(root, dev);
            if (devTypeNode == null)
            {
                return;
            }

            Node devObjectNode = MakeObjectNode(dev.ObjectName,
                dev.ObjectNumber, devTypeNode);
            Node devNode = MakeDeviceNode(devTypeNode, devObjectNode,
                dev, deviceDescription);
            ChangeDevicesCheckState(devNode, checkedDev, dev);
            bool isDevVisible = AddDevChannels(devNode, dev);
            HideIncorrectDeviceTypeSubType(devNode, isDevVisible, countDev, 
                dev);
        }

        /// <summary>
        /// Заполнить узел для подтипа устройства
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <param name="root">Главный узел</param>
        /// <param name="deviceDescription">Описание устройства</param>
        /// <param name="countDev">Словарь для подсчета количества</param>
        private void FillSubTypeNode(Device.IODevice dev, Node root, 
            string deviceDescription, Dictionary<string, int> countDev)
        {
            Node devSubTypeNode = FindDevSubTypeNode(root, dev);
            if (devSubTypeNode == null)
            {
                return;
            }
            Node subDevObjectNode = MakeObjectNode(dev.ObjectName,
                dev.ObjectNumber, devSubTypeNode);
            Node subDevNode = MakeDeviceNode(devSubTypeNode,
                subDevObjectNode, dev, deviceDescription);
            bool isDevVisible = noAssigmentBtn.Checked ? false : true;
            HideIncorrectDeviceTypeSubType(subDevNode, isDevVisible, countDev, 
                dev);
        }

        /// <summary>
        /// Поиск узла типа устройства
        /// </summary>
        /// <param name="root">главный узел</param>
        /// <param name="dev">Устройство</param>
        /// <returns></returns>
        private Node FindDevTypeNode(Node root, Device.IODevice dev)
        {
            foreach (Node node in root.Nodes)
            {
                if (node.Tag is Device.DeviceType && 
                    (Device.DeviceType)node.Tag == dev.DeviceType)
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Поиск узла подтипа устройства
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <param name="root">Главный узел</param>
        /// <returns></returns>
        private Node FindDevSubTypeNode(Node root, Device.IODevice dev)
        {
            foreach (Node node in root.Nodes)
            {
                string devSubType = dev.GetDeviceSubTypeStr(dev.DeviceType, 
                    dev.DeviceSubType);
                if (node.Tag is Device.DeviceSubType && 
                    node.Text == devSubType)
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Генерация описания устройства
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <returns></returns>
        private string GenerateDeviceDescription(Device.IODevice dev)
        {
            string result = "";
            if (dev.Description.Contains('\n'))
            {
                string[] devDescr = dev.Description.Split('\n');
                foreach (string str in devDescr)
                {
                    result += str + " ";
                }
            }
            else
            {
                result = dev.Description;
            }

            return result;
        }

        /// <summary>
        /// Сделать и заполнить узел объекта
        /// </summary>
        /// <param name="devTypeNode">Узел для добавления объекта</param>
        /// <param name="objectName">Имя объекта</param>
        /// <param name="objectNumber">Номер объекта</param>
        /// <returns></returns>
        private Node MakeObjectNode(string objectName, long objectNumber, 
            Node devTypeNode)
        {
            Node devObjectNode = null;
            string fullObjectName = objectName + objectNumber;
            if (objectName != "")
            {
                foreach (Node node in devTypeNode.Nodes)
                {
                    if ((node.Tag is string) && 
                        (string)node.Tag == fullObjectName)
                    {
                        return node;
                    }
                }

                if (devObjectNode == null)
                {
                    devObjectNode = new Node(fullObjectName);
                    devObjectNode.Tag = fullObjectName;
                    devTypeNode.Nodes.Add(devObjectNode);
                    return devObjectNode;
                }
            }

            return devObjectNode;
        }

        /// <summary>
        /// Сделать узел устройства
        /// </summary>
        /// <param name="devTypeNode">Узел типа устройства</param>
        /// <param name="dev">Устройство</param>
        /// <param name="deviceDescription">Описание устройства</param>
        /// <param name="devObjectNode">Узел объекта или null</param>
        /// <returns>Заполненный узел</returns>
        private Node MakeDeviceNode(Node devTypeNode, Node devObjectNode, 
            Device.IODevice dev, string deviceDescription)
        {
            Node devNode;
            if (dev.ObjectName != "")
            {
                string devName = dev.DeviceType + dev.DeviceNumber.ToString() + 
                    "\t  " + deviceDescription;
                devNode = new Node(devName);
            }
            else
            {
                devNode = new Node(dev.name + "\t  " + deviceDescription);
            }
            devNode.Tag = dev;

            if (devObjectNode != null)
            {
                devObjectNode.Nodes.Add(devNode);
            }
            else
            {
                devTypeNode.Nodes.Add(devNode);
            }

            return devNode;
        }

        /// <summary>
        /// Изменение состояния CheckBox
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <param name="checkedDev">Выбранные устройств в действии</param>
        /// <param name="devNode">Узел устройства</param>
        private void ChangeDevicesCheckState(Node devNode , string checkedDev, 
            Device.IODevice dev)
        {
            checkedDev = ' ' + checkedDev + ' ';
            if (checkedDev != "  " &&
                checkedDev.Contains(' ' + dev.Name + ' '))
            {
                devNode.CheckState = CheckState.Checked;
            }
            else
            {
                devNode.CheckState = CheckState.Unchecked;
            }
        }

        /// <summary>
        /// Отображение каналов устройств
        /// </summary>
        /// <param name="devNode">Узел устройства</param>
        /// <param name="dev">Устройство</param>
        /// <returns></returns>
        private bool AddDevChannels(Node devNode, Device.IODevice dev)
        {
            bool isDevVisible = false;
            if (prevShowChannels)
            {
                Node channelNode;
                //Показываем каналы.
                foreach (Device.IODevice.IOChannel ch in dev.Channels)
                {
                    if (!ch.IsEmpty())
                    {
                        channelNode = new Node(ch.Name + " " + ch.Comment +
                            $" (A{ch.FullModule}:" + ch.PhysicalClamp + ")");
                        channelNode.Tag = ch;
                        devNode.Nodes.Add(channelNode);

                        if (noAssigmentBtn.Checked)
                        {
                            channelNode.IsHidden = true;
                        }
                        else
                        {
                            isDevVisible = true;
                        }
                    }
                    else
                    {
                        channelNode = new Node(ch.Name + " " + ch.Comment);
                        channelNode.Tag = ch;
                        devNode.Nodes.Add(channelNode);

                        isDevVisible = true;
                    }
                }
            }

            return isDevVisible;
        }

        /// <summary>
        /// Скрываем устройства ненужных подтипов и типов
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <param name="countDev">Словарь для подсчета количества устройств
        /// </param>
        /// <param name="devTypeSubTypeNode">Узел типа или подтипа</param>
        /// <param name="isDevVisible">Видимость устройства</param>
        private void HideIncorrectDeviceTypeSubType(Node devTypeSubTypeNode,
            bool isDevVisible, Dictionary<string,int> countDev, 
            Device.IODevice dev)
        {
            if (devTypesLastSelected != null &&
                !devTypesLastSelected.Contains(dev.DeviceType))
            {
                devTypeSubTypeNode.IsHidden = true;
            }
            else
            {
                if (devSubTypesLastSelected != null &&
                    !devSubTypesLastSelected.Contains(dev.DeviceSubType))
                {
                    devTypeSubTypeNode.IsHidden = true;
                }
                else
                {
                    if (prevShowChannels && !isDevVisible)
                    {
                        devTypeSubTypeNode.IsHidden = true;
                    }
                    else
                    {
                        string subTypeName = dev.GetDeviceSubTypeStr(
                            dev.DeviceType, dev.DeviceSubType);
                        if(subTypeName != "" && 
                            countDev.ContainsKey(subTypeName))
                        {
                            countDev[subTypeName]++;
                        }
                        else
                        {
                            countDev[dev.DeviceType.ToString()]++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обновление количества устройств шапках их типов
        /// </summary>
        /// <param name="deviceEnum">Названия</param>
        /// <param name="root">Узел</param>
        /// <param name="countDev">Словарь с количеством по индексам</param>
        private void UpdateDevicesCountInHeaders(object[] deviceEnum, Node root, 
            Dictionary<string, int> countDev)
        {
            //Обновляем названия строк (добавляем количество устройств).
            int total = 0;
            foreach (var dev in deviceEnum)
            {
                Node node = root.Nodes.Where(x => x.Text == dev.ToString())
                    .FirstOrDefault();
                total += countDev[dev.ToString()];
                node.Text = dev.ToString() + 
                    " (" + countDev[dev.ToString()] + ")  ";
            }

            root.Text = "Устройства проекта (" + total + ")";
        }

        /// <summary>
        /// Сортировка модели дерева устройств
        /// </summary>
        private void SortTreeView(TreeModel treeModel)
        {
            // Сортировка узлов
            List<Node> rootNodes = treeModel.Nodes.ToList();
            // Сортируем узлы внутри каждого устройства Device
            foreach (Node node in rootNodes)
            {
                TreeSort(node.Nodes.ToList(), node);
            }
        }

        /// <summary>
        /// Сортировка дерева с помощью List.Sort
        /// </summary>
        /// <param name="nodes">Список Nodes в узле</param>
        /// <param name="parent">Родительский узел, где лежат Nodes</param>
        private void TreeSort(List<Node> nodes, Node parent)
        {
            // Если меньше или 1, то нет смысла сортировать
            if (nodes.Count > 1)
            {
                // Собственный Comparer для List
                nodes.Sort((x, y) =>
               {
                   int res = 0;
                   if (x.Tag is Device.IODevice.IOChannel &&
                       y.Tag is Device.IODevice.IOChannel)
                   {
                       var wx = x.Tag as Device.IODevice.IOChannel;
                       var wy = y.Tag as Device.IODevice.IOChannel;

                       res = Device.IODevice.IOChannel.Compare(wx, wy);
                       return res;
                   }

                   bool checkDevTypeSubType =
                   (x.Tag is Device.DeviceType || 
                   x.Tag is Device.DeviceSubType) &&
                   (y.Tag is Device.DeviceSubType || 
                   y.Tag is Device.DeviceSubType);
                   if (checkDevTypeSubType)
                   {
                       res = x.Text.CompareTo(y.Text);
                       return res;
                   }

                   if (x.Nodes.Count > 0 && x.Nodes[0].Tag is Device.Device &&
                       y.Nodes.Count > 0 && y.Nodes[0].Tag is Device.Device)
                   {
                       res = Device.Device.Compare(
                           x.Nodes[0].Tag as Device.Device,
                           y.Nodes[0].Tag as Device.Device);
                       return res;
                   }

                   if (x.Tag is Device.Device && y.Tag is Device.Device)
                   {
                       var xDev = x.Tag as Device.Device;
                       var yDev = y.Tag as Device.Device;

                       res = Device.Device.Compare(xDev, yDev);
                       return res;
                   }

                   res = x.Text.CompareTo(y.Text);
                   return res;
               });

                // Очищаем Nodes
                parent.Nodes.Clear();
                // Записываем отсортированные и сортируем сразу следующие
                foreach (Node node in nodes)
                {
                    parent.Nodes.Add(node);
                    TreeSort(node.Nodes.ToList(), node);
                }
            }
        }
        #endregion

        /// <summary>
        /// Построение дерева на основе определенных устройств проекта.
        /// </summary>
        /// <param name="deviceManager">Менеджер устройств проекта.</param>
        /// <param name="devTypes">Показывать данные типы устройств.</param>
        /// /// <param name="devSubTypes">Показывать данные подтипы устройств.
        /// </param>
        public bool ShowDevices(Device.DeviceManager deviceManager,
            Device.DeviceType[] devTypes, Device.DeviceSubType[] devSubTypes,
            bool showChannels, bool showCheckboxes, string checkedDev,
            OnSetNewValue fn, bool isRebuiltTree = false)
        {
            prevShowChannels = showChannels;
            prevShowCheckboxes = showCheckboxes;

            if (fn != null)
            {
                functionAfterCheck = fn;
            }

            if (showCheckboxes)
            {
                devicesTreeViewAdv.NodeControls.Insert(0, nodeCheckBox);
            }
            else
            {
                devicesTreeViewAdv.NodeControls.Remove(nodeCheckBox);
            }

            //Проверяем на изменение типов отображаемых устройств.
            if (Equals(devTypes, devTypesLastSelected) &&
                Equals(devSubTypes, devSubTypesLastSelected) &&
                isRebuiltTree == false)
            {
                ShowDlg();
                return true;
            }

            devTypesLastSelected = devTypes;
            devSubTypesLastSelected = devSubTypes;

            Refresh(deviceManager, checkedDev);

            ShowDlg();
            return true;
        }

        /// <summary>
        /// По двойному клику на узле вставляем в поле описания 
        /// функционального текста(EPlan) строку с информацией 
        /// о канале устройства.
        /// </summary>
        /// 
        private void treeView_DoubleClick(object sender, 
            TreeNodeAdvMouseEventArgs e)
        {
            var deviceBinder = new DeviceBinder(this);
            deviceBinder.Bind();
        }

        /// <summary>
        /// Выбор устройств, участвующих в операции.
        /// </summary>         
        static class OnCheckOperationTree
        {
            static string res = "";

            public static void ResetResStr()
            {
                res = "";
            }

            public static string GetResStr()
            {
                return res;
            }

            public static void Execute(TreeNodeAdv treeNode)
            {
                var node = treeNode.Tag as Node;

                if (treeNode.IsHidden == false)
                {
                    if (node.CheckState == CheckState.Checked && 
                        node.Tag is Device.Device)
                    {
                        res += (node.Tag as Device.Device).Name + " ";
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
        }

        /// <summary>
        /// Обработка выбора устройства для добавления его к шагу.
        /// После выбора устройства мы передаем новую строку с устройствами
        /// через делегат functionAfterCheck.
        /// </summary>        
        private void treeItem_AfterCheck(object sender, TreePathEventArgs e)
        {
            if (functionAfterCheck != null)
            {
                OnCheckOperationTree.ResetResStr();
                string res = "";

                treeItem_ChangeCheckBoxState(sender, e);

                List<TreeNodeAdv> treeNodes = devicesTreeViewAdv.AllNodes
                    .ToList();
                TreeNodeAdv treeNode = treeNodes[0];
                OnCheckOperationTree.Execute(treeNode);

                res = OnCheckOperationTree.GetResStr();

                devicesTreeViewAdv.Refresh();

                functionAfterCheck(res);
            }
        }

        /// <summary>
        /// Функция обновления состояний чекбоксов
        /// </summary>
        /// <param name="sender">Объект, который вызвал функцию</param>
        /// <param name="e">Контекст переданный вызывающим кодом</param>
        private void treeItem_ChangeCheckBoxState(object sender, 
            TreePathEventArgs e)
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
                        // Такое же состояние child, как и у node
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
            devicesTreeViewAdv.BeginUpdate();

            int level = Convert.ToInt32((sender as ToolStripButton).Tag) + 
                correctionValue;
            devicesTreeViewAdv.CollapseAll();
            ExpandToLevel(level);

            devicesTreeViewAdv.EndUpdate();
        }

        /// <summary>
        /// Развертывание дерева до определенного уровня
        /// </summary>       
        /// <param name="level">Уровень развертывания дерева</param>
        private void ExpandToLevel(int level)
        {
            foreach (TreeNodeAdv node in devicesTreeViewAdv.AllNodes)
            {
                if (node.Level <= level)
                {
                    node.Expand();
                }
            }
        }

        /// <summary>
        /// Соответствующие шрифты узлов дерева.
        /// </summary>
        private const string fName = "Microsoft Sans Serif";
        private readonly Font itemFontNoDevice = new Font(fName, 8);
        private readonly Font itemFontIsDevice = new Font(fName, 8, 
            FontStyle.Strikeout);
        private readonly Font itemFontType = new Font(fName, 8, FontStyle.Bold);

        private void TreeViewAdv1_DrawNode(object sender, DrawTextEventArgs e)
        {
            e.TextColor = Color.Black;
            if (e.Node.Level == 1 || e.Node.Level == 2)
            {
                e.Font = itemFontType;
                return;
            }
            Node currentNode = (Node)e.Node.Tag;
            if (currentNode.Tag is Device.IODevice.IOChannel)
            {
                e.Font = itemFontNoDevice;

                Device.IODevice.IOChannel ch =
                    currentNode.Tag as Device.IODevice.IOChannel;
                if (!ch.IsEmpty())
                {
                    e.Font = itemFontIsDevice;
                    e.Text = ch.Name + " " + ch.Comment +
                        $" (A{ch.FullModule}:" + ch.PhysicalClamp + ")";
                }
                else
                {
                    e.Text = ch.Name + " " + ch.Comment;
                }
            }
        }

        private void noAssigmentBtn_Click(object sender, EventArgs e)
        {
            if (prevShowChannels == true)
            {
                if (noAssigmentBtn.Checked)
                {
                    noAssigmentBtn.Checked = false;
                }
                else
                {
                    noAssigmentBtn.Checked = true;
                }

                ShowDevices(Device.DeviceManager.GetInstance(),
                    devTypesLastSelected, devSubTypesLastSelected, 
                    prevShowChannels, prevShowCheckboxes, "", null, true);
            }
        }

        private void synchBtn_Click(object sender, EventArgs e)
        {
            bool saveDescrSilentMode = false;
            EProjectManager.GetInstance().SyncAndSave(saveDescrSilentMode);

            Editor.Editor.GetInstance().EForm.RefreshTree();

            RefreshTree();
        }

        /// <summary>
        /// Обновление дерева на основе текущих устройств.
        /// </summary>
        public void RefreshTree()
        {
            Refresh(Device.DeviceManager.GetInstance(), "");
        }

        /// <summary>
        /// Обновление дерева после привязки устройства к каналу при
        /// двойном клике
        /// </summary>
        public void RefreshTreeAfterBinding()
        {
            if (noAssigmentBtn.Checked)
            {
                devicesTreeViewAdv.BeginUpdate();
                List<TreeNodeAdv> treeNodes = devicesTreeViewAdv.AllNodes
                    .ToList();
                TreeNodeAdv treeNode = treeNodes[0];
                RefreshOperationTree.Execute(treeNode);
                treeNodes.Clear();
                treeNodes = devicesTreeViewAdv.AllNodes.ToList();
                treeNode = treeNodes[0];
                OnHideOperationTree.Execute(treeNode);
                devicesTreeViewAdv.EndUpdate();
                devicesTreeViewAdv.Refresh();
            }
            else
            {
                devicesTreeViewAdv.Refresh();
            }
        }

        /// <summary>
        /// Последний выбранный элемент в форме
        /// </summary>
        /// <returns>Узел с информацией</returns>
        public TreeNodeAdv GetLastSelectedNode()
        {
            return devicesTreeViewAdv.SelectedNodes.Last();
        }
    }
}