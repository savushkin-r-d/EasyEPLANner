///@file ExcelRepoter.cs
///@brief Классы, реализующие минимальную функциональность, необходимую для 
///экспорта описания проекта в Excel.
///
/// @author  Иванюк Дмитрий Сергеевич.
///
/// @par Текущая версия:
/// @$Rev: --- $.\n
/// @$Author: sedr $.\n
/// @$Date:: 2019-10-21#$.

using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace EasyEPlanner
{
    class ExcelRepoter
    {
        /// <summary>
        /// Создание и сохранение Excel файла с параметрами проекта 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static int ExportTechDevs(string fileName)
        {
            Excel._Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            List<int> ID = new List<int>();
            GetExcelProcess(ID);

            try
            {
                xlApp = new Excel.Application();
                //xlApp.Visible = false;
                //xlApp.UserControl = true;
                xlWorkBook = xlApp.Workbooks.Add();

                string prjName = fileName.Remove(fileName.IndexOf(".xlsx"));
                prjName = prjName.Substring(prjName.LastIndexOf("\\") + 1);

                CreateModulesPage(prjName, ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(5);

                CreateInformDevicePage(ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(20);

                CreateTotalDevicePage(ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(35);

                CreateDeviceConnectionPage(ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(50);

                CreateObjectParamsPage(ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(65);

                CreateObjectDevicesPage(ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(80);

                CreateObjectsPageWithoutActions(ref xlWorkSheet, ref xlApp);

                ProjectManager.GetInstance().SetLogProgress(85);

                xlWorkSheet = xlApp.Sheets[1] as Excel.Worksheet;
                xlWorkSheet.Select();

                xlWorkBook.SaveAs(fileName);
            }
            finally
            {
                //xlWorkBook.Close(false);
                //xlApp.Quit();

                xlWorkBook = null;
                xlWorkSheet = null;
                xlApp = null;
                KillExcelProcess(ID);
                //GC.Collect();

                //Process.Start(fileName);
            }

            return 0;
        }

        private static void KillExcelProcess(List<int> ID)
        {
            Process[] ps2 = Process.GetProcessesByName("EXCEL");
            if (ps2 != null)
            {
                if (ID.Count == 0)
                {
                    foreach (Process excelProc in ps2)
                    {
                        excelProc.Kill();

                    }
                }
                else
                {
                    foreach (Process excelProc in ps2)
                    {
                        if (!ID.Contains(excelProc.Id))
                        {
                            excelProc.Kill();

                        }
                    }
                }
            }
        }

        private static void GetExcelProcess(List<int> ID)
        {
            Process[] ps2 = Process.GetProcessesByName("EXCEL");
            if (ps2 != null)
            {
                foreach (Process excelProc in ps2)
                {
                    ID.Add(excelProc.Id);
                }
            }
        }

        /// <summary>
        /// Создание страницы с модулями IO
        /// </summary>
        private static void CreateModulesPage(string prjName, ref Excel.Worksheet xlWorkSheet,
            ref Excel._Application xlApp)
        {
            xlWorkSheet = xlApp.ActiveSheet as Excel.Worksheet;

            xlWorkSheet.Name = "Модули ввода-вывода";

            xlWorkSheet.Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            Dictionary<string, int> modulesCount = new Dictionary<string, int>();

            Dictionary<string, System.Drawing.Color> modulesColor = new Dictionary<string, System.Drawing.Color>();
            Dictionary<string, object[,]> asInterfaceConnection = new Dictionary<string, object[,]>();

            object[,] res = IO.IOManager.GetInstance().SaveAsConnectionArray(prjName, modulesCount, modulesColor, asInterfaceConnection);

            string endPos = "D" + (res.GetLength(0) + 0);
            xlWorkSheet.Range["A1", endPos].Value2 = res;
            int finalRows = res.GetLength(0) + 2;

            //Форматирование страницы
            xlApp.ScreenUpdating = false;
            xlApp.DisplayAlerts = false;
            xlWorkSheet.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            Excel.Range rangeCurrent = xlWorkSheet.Range["A1", "A1"];
            Excel.Range rangeStart = rangeCurrent;
            int totalCountRows = xlWorkSheet.UsedRange.Rows.Count;
            int i = 1;
            string arr2 = rangeCurrent.Text as string;
            do
            {
                rangeCurrent = rangeCurrent.MergeArea.Offset[1, 0];
                string arr1 = rangeStart.Text as string;
                arr2 = rangeCurrent.Text as string;
                if (arr1 != arr2)
                {
                    xlWorkSheet.Range[rangeStart, rangeCurrent.Offset[-1, 0]].Merge();
                    Excel.Range moduleNameRange = rangeStart.Offset[0, 1];
                    string moduleName = moduleNameRange.Text as string;

                    if (modulesColor.ContainsKey(moduleName))
                    {
                        moduleNameRange.Interior.Color = modulesColor[moduleName];
                    }

                    int moduleIdx;
                    if (Int32.TryParse(arr1, out moduleIdx))
                    {
                        xlWorkSheet.Range[rangeStart.Offset[0, 1], rangeCurrent.Offset[-1, 1]].Merge();
                        xlWorkSheet.Range[rangeStart, rangeCurrent.Offset[-1, 3]].
                            BorderAround(Type.Missing, Excel.XlBorderWeight.xlThick);
                    }
                    else
                    {
                        xlWorkSheet.Range[rangeStart, rangeCurrent.Offset[-1, 3]].Borders.LineStyle =
                            Excel.XlLineStyle.xlLineStyleNone;

                    }
                    rangeStart = rangeCurrent;
                }
                i++;
            }
            while (i <= totalCountRows);

            xlWorkSheet.UsedRange.WrapText = false;

            // Форматирование по ширине содержимого.
            xlWorkSheet.Cells.EntireColumn.AutoFit();
            xlWorkSheet.Cells.EntireColumn.WrapText = true;

            Excel.Range column = xlWorkSheet.Range["B2", "B" + finalRows.ToString()];
            column.Orientation = 90;
            // 6.43 - 50 пикселей
            column.ColumnWidth = 6.43;
            column.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


            column = xlWorkSheet.Range["A2", "A" + finalRows.ToString()];
            //26.43 - 190 пикселей
            column.ColumnWidth = 26.43;
            column.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


            column = xlWorkSheet.Range["C2", "C" + finalRows.ToString()];
            // 2.14 - 20 пикселей
            column.ColumnWidth = 6.43;
            column.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            int totalStart = totalCountRows + 3;
            int totalEnd = totalStart;
            int idx = 0;
            int total = 0;
            //Выделение отдельным блоком модулей AS-interface
            if (asInterfaceConnection.Count != 0)
            {
                object[,] ASInterface = new object[asInterfaceConnection.Count * 130, 4];
                idx = 0;
                ASInterface[idx, 0] = "AS-interface/IO-Link";
                idx++;
                foreach (string key in asInterfaceConnection.Keys)
                {

                    ASInterface[idx, 0] = key;
                    idx++;
                    int startColumn = 2;
                    object[,] connections = asInterfaceConnection[key];
                    int rows = connections.GetLength(0);
                    int cols = connections.GetLength(1);
                    for (int ii = 0; ii < rows; ii++)
                    {
                        bool notNull = true;
                        for (int jj = 0; jj < cols; jj++)
                        {
                            if (connections[ii, jj] != null)
                            {
                                ASInterface[idx, startColumn + jj] = connections[ii, jj];
                            }
                            else
                            {
                                notNull = false;
                            }
                        }
                        if (notNull)
                        {
                            idx++;
                        }
                    }

                }
                totalEnd = totalStart + idx;
                xlWorkSheet.Range["A" + totalStart.ToString(), "D" + totalEnd.ToString()].Value2 = ASInterface;
                totalStart = totalEnd + 2;
            }

            //Создание сводной таблицы используемых модулей на основании словаря

            object[,] modulesTotal = new object[modulesCount.Count + 1, 2];
            idx = 0;
            total = 0;

            // Заполнение таблицы
            foreach (string key in modulesCount.Keys)
            {
                modulesTotal[idx, 0] = key;
                modulesTotal[idx, 1] = modulesCount[key];
                total += modulesCount[key];
                idx++;
            }
            modulesTotal[idx, 0] = "Всего:";
            modulesTotal[idx, 1] = total;

            //Форматирование таблицы

            totalEnd = totalStart + modulesCount.Count;

            rangeCurrent = xlWorkSheet.Range["A" + totalStart.ToString(), "B" + totalEnd.ToString()];
            rangeCurrent.Value2 = modulesTotal;
            rangeCurrent.Orientation = 0;
            rangeCurrent.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            rangeCurrent.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            rangeCurrent.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


            rangeCurrent = xlWorkSheet.Range["A" + totalStart.ToString(), "A" + totalStart.ToString()];
            rangeStart = rangeCurrent;

            // Окрас ячеек
            i = totalStart;
            arr2 = rangeCurrent.Text as string;
            do
            {
                rangeCurrent = rangeCurrent.MergeArea.Offset[1, 0];
                string arr1 = rangeStart.Text as string;
                arr2 = rangeCurrent.Text as string;
                if (arr1 != arr2)
                {
                    Excel.Range moduleNameRange = rangeStart.Offset[0, 0];
                    string moduleName = moduleNameRange.Text as string;

                    if (modulesColor.ContainsKey(moduleName))
                    {
                        moduleNameRange.Interior.Color = modulesColor[moduleName];
                    }

                    rangeStart = rangeCurrent;
                }
                i++;
            }
            while (i <= totalEnd);

            rangeCurrent = null;
            rangeStart = null;

        }

        /// <summary>
        /// Создание страницы с устройствами для операций и шагов техобъектов
        /// </summary>
        private static void CreateObjectDevicesPage(ref Excel.Worksheet xlWorkSheet,
                ref Excel._Application xlApp)
        {
            xlWorkSheet = xlApp.Sheets.Add(Type.Missing, xlWorkSheet) as Excel.Worksheet;

            xlWorkSheet.Name = "Операции и устройства";

            Excel.Range excelCells = xlWorkSheet.get_Range("A1", "C1").Cells;
            // Производим объединение
            excelCells.Merge(System.Reflection.Missing.Value);
            excelCells.Value = "Технологические объекты";
            xlWorkSheet.Range["D1", "L1"].Value2 =
                new string[] { "Вкл.устройства", "Выкл. устройства",
                "Верхние седла", "Нижние седла" , "Сигналы для включения", "Мойка (DI)",
                "Мойка (DO)", "Мойка (Устройства)", "Группы DI-->DO"};
            xlWorkSheet.Range["A1", "L1"].EntireColumn.AutoFit();

            //Заполнение страницы данными
            TreeView tree = TechObject.TechObjectManager.GetInstance().SaveDevicesAsTree();
            int row = 2;
            WriteTreeNode(ref xlWorkSheet, tree.Nodes, ref row);

            //Форматирование страницы
            xlApp.ActiveWindow.SplitRow = 1;
            xlApp.ActiveWindow.FreezePanes = true;
            row = xlWorkSheet.UsedRange.Rows.Count;
            xlWorkSheet.Range["A1", "C" + row.ToString()].EntireColumn.AutoFit();
            // установка переноса текста в ячейке.
            xlWorkSheet.UsedRange.WrapText = true;
            xlWorkSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Создание страницы с параметрами техобъектов проекта
        /// </summary>
        private static void CreateObjectParamsPage(ref Excel.Worksheet xlWorkSheet,
            ref Excel._Application xlApp)
        {
            // Добавление листа в книгу.
            xlWorkSheet = xlApp.Sheets.Add(Type.Missing, xlWorkSheet) as Excel.Worksheet;
            xlWorkSheet.Name = "Параметры объектов";

            // Настройка имен столбцов.
            xlWorkSheet.Range["A1", "A1"].Value2 = new string[] { "Технологический объект" };
            Excel.Range excelCells = xlWorkSheet.get_Range("B1", "C1").Cells;
            excelCells.Merge(System.Reflection.Missing.Value);
            excelCells.Value = "Параметры";
            xlWorkSheet.Range["D1", "G1"].Value2 = new string[] { "Значение", 
                "Размерность", "Операция", "Lua имя"};
            
            // Получить и записать данные
            TreeView tree = TechObject.TechObjectManager.GetInstance().SaveParamsAsTree();
            int row = 2;
            WriteTreeNode(ref xlWorkSheet, tree.Nodes, ref row);

            // Форматирование страницы.
            xlApp.ActiveWindow.SplitRow = 1;
            xlApp.ActiveWindow.FreezePanes = true;
            row = xlWorkSheet.UsedRange.Rows.Count;
            xlWorkSheet.Range["A1", "G" + row.ToString()].EntireColumn.AutoFit();

            // Установка переноса текста в ячейке.
            xlWorkSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Создание страницы с описанием устройств
        /// </summary>
        private static void CreateInformDevicePage(ref Excel.Worksheet xlWorkSheet,
            ref Excel._Application xlApp)
        {
            xlWorkSheet = xlApp.Sheets.Add(Type.Missing,
                   xlWorkSheet) as Excel.Worksheet;

            xlWorkSheet.Name = "Техустройства";
            xlWorkSheet.Range["A1", "D1"].Value2 =
                new string[] { "название", "описание", "тип", "подтип" };
            object[,] res = Device.DeviceManager.GetInstance().SaveAsArray();
            string endPos = "Q" + (res.GetLength(0) + 1);
            xlWorkSheet.Range["A2", endPos].Value2 = res;
            // Форматирование по ширине содержимого.
            xlWorkSheet.Cells.EntireColumn.AutoFit();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateTotalDevicePage(ref Excel.Worksheet xlWorkSheet,
            ref Excel._Application xlApp)
        {
            xlWorkSheet = xlApp.Sheets.Add(Type.Missing,
                            xlWorkSheet) as Excel.Worksheet;
            xlWorkSheet.Name = "Сводная таблица устройств";
            object[,] res = Device.DeviceManager.GetInstance().SaveSummaryAsArray();
            string endPos = "Q" + res.GetLength(0);
            xlWorkSheet.Range["A1", endPos].Value2 = res;
            xlWorkSheet.Cells.EntireColumn.AutoFit();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateDeviceConnectionPage(ref Excel.Worksheet xlWorkSheet,
            ref Excel._Application xlApp)
        {
            xlWorkSheet = xlApp.Sheets.Add(Type.Missing,
                            xlWorkSheet) as Excel.Worksheet;
            xlWorkSheet.Name = "Подключение устройств";
            TreeView tree = Device.DeviceManager.GetInstance().SaveConnectionAsTree();
            int row = 1;
            WriteTreeNode(ref xlWorkSheet, tree.Nodes, ref row);
            xlWorkSheet.Cells.EntireColumn.AutoFit();
            xlWorkSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Создание страницы с информацией об объектах (слепок редактора).
        /// </summary>
        /// <param name="xlWorkSheet"></param>
        /// <param name="xlApp"></param>
        private static void CreateObjectsPageWithoutActions(
            ref Excel.Worksheet xlWorkSheet, ref Excel._Application xlApp)
        {
            xlWorkSheet = xlApp.Sheets.Add(Type.Missing, xlWorkSheet) as 
                Excel.Worksheet;
            xlWorkSheet.Name = "Технологические объекты";
            TreeView tree = TechObject.TechObjectManager.GetInstance()
                .SaveObjectsWithoutActionsAsTree();
            int row = 1;
            WriteTreeNode(ref xlWorkSheet, tree.Nodes, ref row);
            xlWorkSheet.Cells.EntireColumn.AutoFit();
            xlWorkSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Запись узла дерева в Excel таблицу
        /// </summary>
        private static void WriteTreeNode(ref Excel.Worksheet xlWorkSheet,
            TreeNodeCollection Nodes, ref int row)
        {
            foreach (TreeNode node in Nodes)
            {
                int firstGroupRow = row + 1;
                if (node.Tag is string[])
                {
                    string[] values = node.Tag as string[];
                    string firstCellAddress = ParseColNum(node.Level) + 
                        row.ToString();
                    string secondCellAddress = ParseColNum(
                        node.Level + values.Length - 1) + row.ToString();
                    xlWorkSheet.Range[firstCellAddress, secondCellAddress]
                        .Value2 = values;
                }
                else
                {
                    string[] srt = new string[] { node.Text };

                    string cellAddress = ParseColNum(node.Level) + 
                        row.ToString();

                    xlWorkSheet.Range[cellAddress, cellAddress].Value2 = srt;

                }
                row++;
                WriteTreeNode(ref xlWorkSheet, node.Nodes, ref row);
                if (firstGroupRow != row)
                {
                    (xlWorkSheet.Rows[string.Format("{0}:{1}", firstGroupRow, 
                                row - 1), System.Reflection.Missing.Value]
                                as Excel.Range).Group();
                }
            }
        }

        /// <summary>
        /// перевод порядкового номера столбца в его буквенный эквивалент.
        /// </summary>
        /// <param name="colNum">номер столбца</param>
        private static string ParseColNum(int colNum)
        {
            var sb = new System.Text.StringBuilder();
            if (colNum <= 0) return "A";

            while (colNum != 0)
            {
                sb.Append((char)('A' + (colNum % 26)));
                colNum /= 26;
            }
            return sb.ToString();
        }
    }
}