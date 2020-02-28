using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, отвечающий за генерацию Excel документов
    /// </summary>
    class ExcelRepoter
    {
        /// <summary>
        /// Создание и сохранение Excel файла с параметрами проекта 
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="autoSave">Вызвано автосохранением</param>
        /// <returns></returns>
        public static int ExportTechDevs(string fileName, bool autoSave = false)
        {
            Excel._Application app = null;
            Excel.Workbook workBook = null;
            Excel.Worksheet workSheet = null;

            List<int> ID = new List<int>();
            GetExcelProcess(ID);

            try
            {
                app = new Excel.Application();
                app.Visible = false;
                app.UserControl = true;
                workBook = app.Workbooks.Add();

                string prjName = fileName.Remove(fileName.IndexOf(".xlsx"));
                prjName = prjName.Substring(prjName.LastIndexOf("\\") + 1);

                if (!autoSave)
                {
                    GenerateFullExcelFile(prjName, ref workSheet, ref app);
                }
                else
                {
                    GenerateExcelAutoReport(ref workSheet, ref app);
                }

                workSheet = app.Sheets[1] as Excel.Worksheet;
                workSheet.Select();

                SaveExcelFile(autoSave, workBook, fileName);
            }
            finally
            {
                workBook.Close(false);
                app.Quit();

                workBook = null;
                workSheet = null;
                app = null;        
                
                KillExcelProcess(ID);
                GC.Collect();

                if (autoSave == false)
                {
                    Process.Start(fileName);
                }
            }
            return 0;
        }

        /// <summary>
        /// Генерировать полный Excel файл
        /// </summary>
        private static void GenerateFullExcelFile(string prjName, 
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            CreateModulesPage(prjName, ref workSheet, ref app);
            ProjectManager.GetInstance().SetLogProgress(5);

            CreateInformDevicePage(ref workSheet, ref app);
            ProjectManager.GetInstance().SetLogProgress(20);

            CreateTotalDevicePage(ref workSheet, ref app);
            ProjectManager.GetInstance().SetLogProgress(35);

            CreateDeviceConnectionPage(ref workSheet, ref app);
            ProjectManager.GetInstance().SetLogProgress(50);

            CreateObjectParamsPage(ref workSheet, ref app);
            ProjectManager.GetInstance().SetLogProgress(65);

            CreateObjectDevicesPage(ref workSheet, ref app);
            ProjectManager.GetInstance().SetLogProgress(80);
        }

        /// <summary>
        /// Генерировать отчет по технологическим объектом (для SCADA).
        /// </summary>
        private static void GenerateExcelAutoReport(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            CreateObjectsPageWithoutActions(ref workSheet, ref app);
        }

        /// <summary>
        /// Сохранить Excel файл
        /// </summary>
        /// <param name="autoSave">Автосохранение</param>
        /// <param name="workBook">Книга</param>
        /// <param name="fileName">Имя файла</param>
        private static void SaveExcelFile(bool autoSave, 
            Excel.Workbook workBook, string fileName)
        {
            object miss = Type.Missing;
            if (autoSave)
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                workBook.SaveAs(fileName, miss, miss, "Read", true);
            }
            else
            {
                workBook.SaveAs(fileName);
            }
        }

        /// <summary>
        /// Уничтожить процесс Excel в системе
        /// </summary>
        /// <param name="ID">Уникальный номер процесса</param>
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

        /// <summary>
        /// Получить номер процесса Excel в системе
        /// </summary>
        /// <param name="ID">Список всех процессов системы</param>
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
        private static void CreateModulesPage(string prjName, 
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            workSheet = app.ActiveSheet as Excel.Worksheet;
            workSheet.Name = "Модули ввода-вывода";
            workSheet.Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            var modulesCount = new Dictionary<string, int>();
            var modulesColor = new Dictionary<string, System.Drawing.Color>();
            var asInterfaceConnection = new Dictionary<string, object[,]>();

            object[,] res = ExcelDataCollector.SaveIOAsConnectionArray(prjName, 
                modulesCount, modulesColor, asInterfaceConnection);

            string endPos = "D" + (res.GetLength(0) + 0);
            workSheet.Range["A1", endPos].Value2 = res;
            int finalRows = res.GetLength(0) + 2;

            //Форматирование страницы
            app.ScreenUpdating = false;
            app.DisplayAlerts = false;
            workSheet.UsedRange.Borders.LineStyle = Excel.XlLineStyle
                .xlContinuous;
            workSheet.UsedRange.WrapText = false;

            Excel.Range rangeCurrent = workSheet.Range["A1", "A1"];
            Excel.Range rangeStart = rangeCurrent;
            int totalCountRows = workSheet.UsedRange.Rows.Count;
            int i = 1;
            string arr2 = rangeCurrent.Text as string;
            do
            {
                rangeCurrent = rangeCurrent.MergeArea.Offset[1, 0];
                string arr1 = rangeStart.Text as string;
                arr2 = rangeCurrent.Text as string;
                if (arr1 != arr2)
                {
                    workSheet.Range[rangeStart, rangeCurrent.Offset[-1, 0]]
                        .Merge();
                    Excel.Range moduleNameRange = rangeStart.Offset[0, 1];
                    string moduleName = moduleNameRange.Text as string;

                    if (modulesColor.ContainsKey(moduleName))
                    {
                        moduleNameRange.Interior.Color = modulesColor[
                            moduleName];
                    }

                    int moduleIdx;
                    if (Int32.TryParse(arr1, out moduleIdx))
                    {
                        workSheet.Range[rangeStart.Offset[0, 1], 
                            rangeCurrent.Offset[-1, 1]].Merge();
                        workSheet.Range[rangeStart, 
                            rangeCurrent.Offset[-1, 3]].BorderAround(
                            Type.Missing, Excel.XlBorderWeight.xlThick);
                    }
                    else
                    {
                        workSheet.Range[rangeStart, 
                            rangeCurrent.Offset[-1, 3]].Borders.LineStyle =
                            Excel.XlLineStyle.xlLineStyleNone;
                    }
                    rangeStart = rangeCurrent;
                }
                i++;
            }
            while (i <= totalCountRows);

            // Форматирование по ширине содержимого.
            workSheet.Cells.EntireColumn.AutoFit();
            workSheet.Cells.EntireColumn.WrapText = true;

            Excel.Range column = workSheet.Range["B2", "B" + 
                finalRows.ToString()];
            column.Orientation = 90;
            
            // 6.43 - 50 пикселей
            column.ColumnWidth = 6.43;
            column.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            column = workSheet.Range["A2", "A" + finalRows.ToString()];

            //26.43 - 190 пикселей
            column.ColumnWidth = 26.43;
            column.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            column = workSheet.Range["C2", "C" + finalRows.ToString()];

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
                var ASInterface = new object[
                    asInterfaceConnection.Count * 130, 4];
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
                                ASInterface[idx, startColumn + jj] = 
                                    connections[ii, jj];
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
                workSheet.Range["A" + totalStart.ToString(), "D" + 
                    totalEnd.ToString()].Value2 = ASInterface;
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

            rangeCurrent = workSheet.Range["A" + totalStart.ToString(), "B" + 
                totalEnd.ToString()];
            rangeCurrent.Value2 = modulesTotal;
            rangeCurrent.Orientation = 0;
            rangeCurrent.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            rangeCurrent.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            rangeCurrent.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            rangeCurrent = workSheet.Range["A" + totalStart.ToString(), "A" + 
                totalStart.ToString()];
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
                        moduleNameRange.Interior.Color = modulesColor[
                            moduleName];
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
        private static void CreateObjectDevicesPage(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            workSheet = app.Sheets.Add(Type.Missing, workSheet) as 
                Excel.Worksheet;
            workSheet.Name = "Операции и устройства";
            Excel.Range excelCells = workSheet.get_Range("A1", "C1").Cells;
            
            // Производим объединение
            excelCells.Merge(System.Reflection.Missing.Value);
            excelCells.Value = "Технологические объекты";
            workSheet.Range["D1", "L1"].Value2 = new string[] 
            { 
                "Вкл.устройства", 
                "Выкл. устройства",
                "Верхние седла", 
                "Нижние седла" , 
                "Сигналы для включения", 
                "Мойка (DI)",
                "Мойка (DO)", 
                "Мойка (Устройства)", 
                "Группы DI-->DO"
            };
            workSheet.Range["A1", "L1"].EntireColumn.AutoFit();

            //Заполнение страницы данными
            TreeView tree = ExcelDataCollector
                .SaveTechObjectOperationsAndActionsAsTree();
            int row = 2;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);

            //Форматирование страницы
            app.ActiveWindow.SplitRow = 1;
            app.ActiveWindow.FreezePanes = true;
            row = workSheet.UsedRange.Rows.Count;
            workSheet.Range["A1", "C" + row.ToString()].EntireColumn.AutoFit();
            
            // установка переноса текста в ячейке.
            workSheet.UsedRange.WrapText = true;
            workSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Создание страницы с параметрами техобъектов проекта
        /// </summary>
        private static void CreateObjectParamsPage(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            // Добавление листа в книгу.
            workSheet = app.Sheets.Add(Type.Missing, workSheet) as 
                Excel.Worksheet;
            workSheet.Name = "Параметры объектов";

            // Настройка имен столбцов.
            workSheet.Range["A1", "A1"].Value2 = new string[] 
            { 
                "Технологический объект" 
            };
            Excel.Range excelCells = workSheet.get_Range("B1", "C1").Cells;
            excelCells.Merge(System.Reflection.Missing.Value);
            excelCells.Value = "Параметры";
            workSheet.Range["D1", "G1"].Value2 = new string[] 
            { 
                "Значение", 
                "Размерность", 
                "Операция", 
                "Lua имя"
            };
            
            // Получить и записать данные
            TreeView tree = ExcelDataCollector.SaveParamsAsTree();
            int row = 2;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);

            // Форматирование страницы.
            app.ActiveWindow.SplitRow = 1;
            app.ActiveWindow.FreezePanes = true;
            row = workSheet.UsedRange.Rows.Count;
            workSheet.Range["A1", "G" + row.ToString()].EntireColumn.AutoFit();

            // Установка переноса текста в ячейке.
            workSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Создание страницы с описанием устройств
        /// </summary>
        private static void CreateInformDevicePage(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            workSheet = app.Sheets.Add(Type.Missing, workSheet) as 
                Excel.Worksheet;
            workSheet.Name = "Техустройства";
            workSheet.Range["A1", "D1"].Value2 = new string[] 
            { 
                "Название", 
                "Описание", 
                "Тип", 
                "Подтип" 
            };
            object[,] res = ExcelDataCollector.SaveDevicesInformationAsArray();
            string endPos = "Q" + (res.GetLength(0) + 1);
            workSheet.Range["A2", endPos].Value2 = res;
            
            // Форматирование по ширине содержимого.
            workSheet.Cells.EntireColumn.AutoFit();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateTotalDevicePage(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            workSheet = app.Sheets.Add(Type.Missing, workSheet) as 
                Excel.Worksheet;
            workSheet.Name = "Сводная таблица устройств";
            object[,] res = ExcelDataCollector.SaveDevicesSummaryAsArray();
            string endPos = "Q" + res.GetLength(0);
            workSheet.Range["A1", endPos].Value2 = res;
            workSheet.Cells.EntireColumn.AutoFit();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateDeviceConnectionPage(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            workSheet = app.Sheets.Add(Type.Missing, workSheet) as 
                Excel.Worksheet;
            workSheet.Name = "Подключение устройств";
            TreeView tree = ExcelDataCollector.SaveDeviceConnectionAsTree();
            int row = 1;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);
            workSheet.Cells.EntireColumn.AutoFit();
            workSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
        }

        /// <summary>
        /// Создание страницы с информацией об объектах (слепок редактора).
        /// </summary>
        /// <param name="workSheet"></param>
        /// <param name="app"></param>
        private static void CreateObjectsPageWithoutActions(
            ref Excel.Worksheet workSheet, ref Excel._Application app)
        {
            const int widthColumnA = 40;
            const int widthColumnC = 55;
            const int widthColumnE = 45;
            const int MaxNodeLevel = 5;

            workSheet = app.ActiveSheet as Excel.Worksheet;
            workSheet.Name = "Технологические объекты";
            TreeView tree = ExcelDataCollector
                .SaveObjectsWithoutActionsAsTree();
            int row = 1;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);
            workSheet.Cells.EntireColumn.AutoFit();
            workSheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;
            workSheet.Range["A1", "A" + row.ToString()].Columns
                .ColumnWidth = widthColumnA;
            workSheet.Range["C1", "C" + row.ToString()].Columns
                .ColumnWidth = widthColumnC;
            workSheet.Range["E1", "E" + row.ToString()].Columns
                .ColumnWidth = widthColumnE;
            for (int i = MaxNodeLevel; i > 0; i--)
            {
                workSheet.Outline.ShowLevels(i, 0);
            }
        }

        /// <summary>
        /// Запись узла дерева в Excel таблицу
        /// </summary>
        private static void WriteTreeNode(ref Excel.Worksheet workSheet,
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
                    workSheet.Range[firstCellAddress, secondCellAddress]
                        .Value2 = values;
                }
                else
                {
                    string[] srt = new string[] { node.Text };
                    string cellAddress = ParseColNum(node.Level) + 
                        row.ToString();
                    workSheet.Range[cellAddress, cellAddress].Value2 = srt;

                }
                row++;
                
                WriteTreeNode(ref workSheet, node.Nodes, ref row);
                if (firstGroupRow != row)
                {
                    (workSheet.Rows[string.Format("{0}:{1}", firstGroupRow, 
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

        /// <summary>
        /// Экспорт информации о проекта для SCADA системы
        /// </summary>
        /// <param name="project">Проект, для которого осуществляется
        /// экспорт</param>
        public static void AutomaticExportExcelForSCADA(
            Eplan.EplApi.DataModel.Project project)
        {
            string path = project.ProjectDirectoryPath + @"\DOC\" +
                    project.ProjectName + " auto report.xlsx";
            ExcelRepoter.ExportTechDevs(path, true);
        }
    }
}