using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StaticHelper
{
    /// <summary>
    /// Обработчик форм. Повторяющихся действий и др.
    /// </summary>
    public static class GUIHelper
    {
        #region Настройка TreeViewAdv
        public delegate void DrawEventHandler(object sender,
            DrawTextEventArgs e);

        public delegate void CheckStateChangedEventHandler(object sender,
            TreePathEventArgs e);

        /// <summary>
        /// Предварительная настройка treeViewAdv
        /// </summary>
        /// <param name="customizingView">Дерево для настройки</param>
        private static void SetUpCustomizingView(TreeViewAdv customizingView)
        {
            customizingView.FullRowSelect = true;
            customizingView.FullRowSelectActiveColor = Color
                .FromArgb(192, 192, 255);
            customizingView.FullRowSelectInactiveColor = Color
                .FromArgb(192, 255, 192);
            customizingView.GridLineStyle = GridLineStyle.Horizontal;
            customizingView.UseColumns = true;
            customizingView.ShowLines = true;
            customizingView.ShowPlusMinus = true;
            customizingView.RowHeight = 20;
        }

        /// <summary>
        /// Инициализация дерева TreeViewAdv из Aga библиотеки.
        /// </summary>
        /// <param name="customizingView">Дерево для настройки</param>
        /// <param name="columnName">Имя добавляемой колонки</param>
        /// <param name="checkStateChangedDelegate">Делегат для обработки
        /// изменения состояния чекбокса</param>
        /// <param name="customizingCheckBox">Чекбокс для настройки</param>
        /// <param name="drawNodeDelegate">Делегат для отрисовки текстовых
        /// элементов</param>
        public static void SetUpAdvTreeView(TreeViewAdv customizingView,
            string columnName, DrawEventHandler drawNodeDelegate,
            NodeCheckBox customizingCheckBox,
            CheckStateChangedEventHandler checkStateChangedDelegate = null)
        {
            SetUpCustomizingView(customizingView);

            TreeColumn column = SetUpColumn(customizingView, columnName);
            
            SetUpNodeCheckBox(customizingView, column, customizingCheckBox,
                checkStateChangedDelegate);
            SetUpNodeTextBox(customizingView, column, drawNodeDelegate);
        }

        public static void SetUpAdvTreeView(TreeViewAdv customizingView,
            string column1Name, string column2Name,
            DrawEventHandler drawNodeDelegate,
            DrawEventHandler drawNodeEditDelegate,
            EventHandler<LabelEventArgs> editNodeEdidDelegate,
            NodeCheckBox customizingCheckBox,
            CheckStateChangedEventHandler checkStateChangedDelegate = null)
        {
            SetUpCustomizingView(customizingView);

            TreeColumn column = SetUpColumn(customizingView, column1Name);
            TreeColumn column2 = SetUpColumn(customizingView, column2Name);

            SetUpNodeCheckBox(customizingView, column, customizingCheckBox,
                checkStateChangedDelegate);
            SetUpNodeTextBox(customizingView, column, drawNodeDelegate);
            SetUpNodeEditText(customizingView, column2, drawNodeEditDelegate,
                editNodeEdidDelegate);
        }


        /// <summary>
        /// Инициализация колонки для дерева из Aga библиотеки.
        /// </summary>
        /// <param name="view">Дерево</param>
        /// <param name="name">Имя колонки</param>
        private static TreeColumn SetUpColumn(TreeViewAdv view, string name)
        {
            var column = new TreeColumn();
            column.Sortable = false;
            column.Header = name;
            column.Width = 300;
            column.MinColumnWidth = 100;

            view.Columns.Add(column);

            return column;
        }

        /// <summary>
        /// Инициализация узла чекбоксов
        /// </summary>
        /// <param name="customizingView">Дерево</param>
        /// <param name="column">Колонка</param>
        /// <param name="customizingCheckBox">Чекбокс</param>
        /// <param name="checkStateChanged">Делегат для обработки изменения
        /// состояния чекбокса</param>
        private static void SetUpNodeCheckBox(TreeViewAdv customizingView,
            TreeColumn column, NodeCheckBox customizingCheckBox,
            CheckStateChangedEventHandler checkStateChanged = null)
        {
            customizingCheckBox.DataPropertyName = "CheckState";
            customizingCheckBox.VerticalAlign = VerticalAlignment.Center;
            customizingCheckBox.ParentColumn = column;
            customizingCheckBox.EditEnabled = true;
            if(checkStateChanged != null)
            {
                customizingCheckBox.CheckStateChanged +=
                    new EventHandler<TreePathEventArgs>(checkStateChanged);
            }

            customizingView.NodeControls.Add(customizingCheckBox);
        }

        /// <summary>
        /// Инициализация узла отображения текста
        /// </summary>
        /// <param name="customizingView">Дерево</param>
        /// <param name="column">Колонка</param>
        /// <param name="drawNodeDelegate">Метод отрисовки</param>
        private static void SetUpNodeTextBox(TreeViewAdv customizingView,
            TreeColumn column, DrawEventHandler drawNodeDelegate)
        {
            var nodeTextBox = new NodeTextBox();
            nodeTextBox.DataPropertyName = "Text";
            nodeTextBox.VerticalAlign = VerticalAlignment.Center;
            nodeTextBox.TrimMultiLine = true;
            nodeTextBox.ParentColumn = column;
            nodeTextBox.DrawText +=
                new EventHandler<DrawTextEventArgs>(drawNodeDelegate);

            customizingView.NodeControls.Add(nodeTextBox);
        }

        /// <summary>
        /// Инициализация узла для отображения и редоктирования текста
        /// </summary>
        /// <param name="customizingView">Дерево</param>
        /// <param name="column">Колонка</param>
        /// <param name="drawNodeDelegate">Метод отрисовки</param>
        private static void SetUpNodeEditText(TreeViewAdv customizingView,
            TreeColumn column, DrawEventHandler drawNodeDelegate,
            EventHandler<LabelEventArgs> editNodeEdidDelegate)
        {
            var nodeTextBox = new NodeTextBox();
            nodeTextBox.DataPropertyName = "Value";
            nodeTextBox.VerticalAlign = VerticalAlignment.Center;
            nodeTextBox.TrimMultiLine = true;
            nodeTextBox.EditEnabled = true;
            nodeTextBox.EditOnClick = true;
            nodeTextBox.ParentColumn = column;
            nodeTextBox.DrawText +=
                new EventHandler<DrawTextEventArgs>(drawNodeDelegate);
            nodeTextBox.LabelChanged += editNodeEdidDelegate;

            customizingView.NodeControls.Add(nodeTextBox);
        }
        #endregion

        #region Настройка состояний CheckState для TreeViewAdv чекбоксов
        /// <summary>
        /// Функция настройки CheckState для узлов
        /// </summary>
        /// <param name="node"></param>
        public static void CheckCheckState(Node node)
        {
            RecursiveCheck(node);
            RecursiveCheckParent(node.Parent);
        }

        /// <summary>
        /// Функция установки состояния
        /// отображения узла
        /// </summary>
        /// <param name="node">Выбранный узел</param>
        private static void RecursiveCheck(Node node)
        {
            if (node.Nodes.Count > 0)
            {
                List<Node> childNodes = node.Nodes.ToList();

                foreach (Node child in childNodes)
                {
                    if (child.IsHidden != true)
                    {
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
        private static void RecursiveCheckParent(Node parentNode)
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
        #endregion

        #region Поиск дескриптора окна и настройка
        /// <summary>
        /// Поиск дескриптора окна
        /// </summary>
        /// <param name="currentProcess">Текущий процесс</param>
        /// <param name="windowName">Имя окна</param>
        /// <param name="wndWmCommand">WM-команда окна</param>
        /// <param name="dialogHandle">handle диалога</param>
        /// <param name="wndVisiblePtr">Дескриптор найденного окна</param>
        /// <returns>True - успешно, False - с ошибкой</returns>
        /// <param name="isSecondUse">Второй ли запуск функции (рекурсивный)
        /// </param>
        public static bool SearchWindowDescriptor(Process currentProcess,
            string windowName, int wndWmCommand, ref IntPtr dialogHandle,
            ref IntPtr wndVisiblePtr, bool isSecondUse = false)
        {
            // Поиск плавающего представления окна через функцию
            IntPtr res = PI.FindWindowByCaption(IntPtr.Zero, windowName);
            if (res != IntPtr.Zero)
            {
                var resList = PI.GetChildWindows(res);
                if (resList.Count > 0)
                {
                    dialogHandle = resList[0];
                    wndVisiblePtr = dialogHandle;
                }
            }
            else
            {
                const int bufferSize = 200;
                var stringBuffer = new StringBuilder(bufferSize);

                // Поиск плавающего представления окна по всем окнам системы
                SearchFloatWindow(stringBuffer, windowName, ref dialogHandle,
                    ref wndVisiblePtr);

                if(dialogHandle == IntPtr.Zero)
                {
                    // Поиск закрепленного представления окна
                    SearchStaticWindow(currentProcess, stringBuffer, windowName,
                        ref dialogHandle, ref wndVisiblePtr);
                }

                if (dialogHandle == IntPtr.Zero)
                {
                    PI.SendMessage(currentProcess.MainWindowHandle,
                        (uint)PI.WM.COMMAND, wndWmCommand, 0);

                    if (isSecondUse == true && dialogHandle == IntPtr.Zero)
                    {
                        MessageBox.Show("Не удалось найти окно!");
                        return false;
                    }

                    // Повторный поиск
                    SearchWindowDescriptor(currentProcess, windowName,
                        wndWmCommand, ref dialogHandle, ref wndVisiblePtr,
                        true);
                }
            }

            return true;
        }

        /// <summary>
        /// Поиск закрепленного представления окна в процессе.
        /// </summary>
        /// <param name="currentProcess">Текущий процесс</param>
        /// <param name="stringBuffer">Буфер</param>
        /// <param name="windowName">Имя окна</param>
        /// <param name="dialogHandle">Handle диалога выходной</param>
        /// <param name="wndVisiblePtr">Найденный дескриптор окна</param>
        private static void SearchStaticWindow(Process currentProcess,
            StringBuilder stringBuffer, string windowName,
            ref IntPtr dialogHandle, ref IntPtr wndVisiblePtr)
        {
            List<IntPtr> resW = PI.GetChildWindows(currentProcess.MainWindowHandle);
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
                            wndVisiblePtr = dialogHandle;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Поиск плавающего представления окна по всем окнам
        /// </summary>
        /// <param name="stringBuffer">Буфер</param>
        /// <param name="windowName">Имя окна</param>
        /// <param name="dialogHandle">Дескриптор диалога, выходное значение
        /// </param>
        /// <param name="res">Найденный дескриптор для сверки</param>
        /// <param name="wndVisiblePtr">Дескриптор окна</param>
        private static void SearchFloatWindow(StringBuilder stringBuffer,
            string windowName, ref IntPtr dialogHandle, ref IntPtr wndVisiblePtr)
        {
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
                            // Если нашел в потомке название, беру родительское окно и работаю с ним
                            var resList = PI.GetChildWindows(mainWindowChild);
                            if (resList.Count > 0)
                            {
                                dialogHandle = resList[0];
                                wndVisiblePtr = dialogHandle;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Заменить панели заданного окна.
        /// </summary>
        /// <param name="dialogHandle">Дескриптор нового диалога</param>
        /// <param name="panelPtr">Дескриптор старого диалога</param>
        public static void ChangeWindowMainPanels(ref IntPtr dialogHandle,
            ref IntPtr panelPtr)
        {
            var panelList = PI.GetChildWindows(dialogHandle);
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
        }

        /// <summary>
        /// Показать скрытое окно
        /// </summary>
        /// <param name="process">Процесс</param>
        /// <param name="windowHandle">Дескриптор окна</param>
        /// <param name="windowWmCommand">Команда для открытия окна</param>
        public static void ShowHiddenWindow(Process process,
            IntPtr windowHandle, int windowWmCommand)
        {
            if (PI.IsWindowVisible(windowHandle) == false)
            {
                PI.SendMessage(process.MainWindowHandle,
                    (uint)PI.WM.COMMAND, windowWmCommand, 0);
                return;
            }
        }
        #endregion
    }
}
