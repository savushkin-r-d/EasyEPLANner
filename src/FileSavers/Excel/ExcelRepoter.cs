using Spire.Xls;
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
            Workbook workBook = new Workbook();
            try
            {
                string prjName = fileName.Remove(fileName.IndexOf(".xlsx"));
                prjName = prjName.Substring(prjName.LastIndexOf("\\") + 1);

                if (!autoSave)
                {
                    GenerateFullExcelFile(prjName, ref workBook);
                }
                else
                {
                    GenerateExcelAutoReport(ref workBook);
                }

                SaveExcelFile(autoSave, workBook, fileName);
            }
            finally
            {         
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
            ref Workbook workBook)
        {
            CreateModulesPage(prjName, ref workBook);
            Logs.SetProgress(5);

            CreateInformDevicePage(ref workBook);
            Logs.SetProgress(20);

            CreateTotalDevicePage(ref workBook);
            Logs.SetProgress(35);

            CreateDeviceConnectionPage(ref workBook);
            Logs.SetProgress(50);

            CreateObjectParamsPage(ref workBook);
            Logs.SetProgress(65);

            CreateObjectDevicesPage(ref workBook);
            Logs.SetProgress(80);
        }

        /// <summary>
        /// Генерировать отчет по технологическим объектом (для SCADA).
        /// </summary>
        private static void GenerateExcelAutoReport(
            ref Workbook workBook)
        {
            CreateObjectsPageWithoutActions(ref workBook);
        }

        /// <summary>
        /// Сохранить Excel файл
        /// </summary>
        /// <param name="autoSave">Автосохранение</param>
        /// <param name="workBook">Книга</param>
        /// <param name="fileName">Имя файла</param>
        private static void SaveExcelFile(bool autoSave, 
            Workbook workBook, string fileName)
        {
            if (autoSave)
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                workBook.Protect("77777777");
                workBook.SaveToFile(fileName);
            }
            else
            {
                workBook.SaveToFile(fileName);
            }
        }

        /// <summary>
        /// Создание страницы с модулями IO
        /// </summary>
        private static void CreateModulesPage(string prjName,
            ref Workbook workBook)
        {
            string sheetName = "Модули ввода-вывода";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);

            workSheet.Range.Style.VerticalAlignment = VerticalAlignType
                .Center;

            var modulesCount = new Dictionary<string, int>();
            var modulesColor = new Dictionary<string, System.Drawing.Color>();
            var asInterfaceConnection = new Dictionary<string, object[,]>();

            object[,] res = ExcelDataCollector.SaveIOAsConnectionArray(prjName,
                modulesCount, modulesColor, asInterfaceConnection);

            string endPos = "D" + (res.GetLength(0) + 0);
            workSheet.Range[$"A1:{endPos}"].Value2 = res;
            int finalRows = res.GetLength(0) + 2;

            //Форматирование страницы
            workSheet.Range.BorderInside(LineStyleType.Thin);
            workSheet.Range.BorderAround(LineStyleType.Medium);
            workSheet.Range.IsWrapText = false;

            CellRange rangeCurrent = workSheet.Range["A1:A1"];
            CellRange rangeStart = rangeCurrent;
            int totalCountRows = workSheet.Range.Rows.Length;
            int i = 1;
            string arr2 = rangeCurrent.Text as string;
            do
            {
                int startColumn = rangeCurrent.MergeArea.Column;
                int startRow = rangeCurrent.MergeArea.Row;
                rangeCurrent.MergeArea.UpdateRange(startRow + 1, startColumn, 
                    startRow + 1, startColumn);
                string arr1 = rangeStart.Text as string;
                arr2 = rangeCurrent.Text as string;
                if (arr1 != arr2)
                {
                    workSheet.Range[rangeStart.Row, rangeStart.Column, rangeCurrent.Row, rangeCurrent.Column].Merge(); //Offset[-1, 0]
                    CellRange moduleNameRange = rangeStart;
                    moduleNameRange.UpdateRange(rangeStart.Row, rangeStart.Column + 1, rangeStart.Row, rangeStart.Column + 1);//Offset[0, 1];
                    string moduleName = moduleNameRange.Text as string;

                    if (modulesColor.ContainsKey(moduleName))
                    {
                        moduleNameRange.Style.Color = modulesColor[moduleName];
                    }

                    if (Int32.TryParse(arr1, out _))
                    {
                        workSheet.Range[rangeStart.Row, rangeStart.Column - 1, rangeCurrent.Row - 1, rangeCurrent.Column + 1].Merge(); //Offset[0, 1], Offset[-1, 1]
                        workSheet.Range[rangeStart.Row, rangeStart.Column, rangeCurrent.Row - 1, rangeCurrent.Column + 3]                //Offset[-1, 3]
                            .BorderAround(LineStyleType.Thick);
                    }
                    else
                    {
                        workSheet.Range[rangeStart.Row, rangeStart.Column, rangeCurrent.Row - 1, rangeCurrent.Column + 3] //Offset[-1, 3]
                            .Borders.LineStyle = LineStyleType.None;
                    }
                    rangeStart = rangeCurrent;
                }
                i++;
            }
            while (i <= totalCountRows);

            // Форматирование по ширине содержимого.
            workSheet.Range.EntireColumn.AutoFitColumns();
            workSheet.Range.EntireColumn.AutoFitRows();
            workSheet.Range.EntireColumn.IsWrapText = true;

            CellRange column = workSheet.Range[$"B2:B{finalRows}"];
            column.Style.Rotation = 90;

            // 6.43 - 50 пикселей
            column.ColumnWidth = 6.43;
            column.HorizontalAlignment = HorizontalAlignType.Center;
            column = workSheet.Range[$"A2:A{finalRows}"];

            //26.43 - 190 пикселей
            column.ColumnWidth = 26.43;
            column.HorizontalAlignment = HorizontalAlignType.Center;
            column = workSheet.Range[$"C2:C{finalRows}"];

            // 2.14 - 20 пикселей
            column.ColumnWidth = 6.43;
            column.HorizontalAlignment = HorizontalAlignType.Center;

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
                workSheet.Range[$"A{totalStart}:D{totalEnd}"].Value2 =
                    ASInterface;
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

            rangeCurrent = workSheet.Range[$"A{totalStart}:B{totalEnd}"];
            rangeCurrent.Value2 = modulesTotal;
            rangeCurrent.Style.Rotation = 0;
            rangeCurrent.VerticalAlignment = VerticalAlignType.Center;
            rangeCurrent.HorizontalAlignment = HorizontalAlignType.Right;
            rangeCurrent.BorderInside(LineStyleType.Thin);
            rangeCurrent.BorderAround(LineStyleType.Medium);


            rangeCurrent = workSheet.Range[$"A{totalStart}:A{totalStart}"];
            rangeStart = rangeCurrent;

            // Окрас ячеек
            i = totalStart;
            arr2 = rangeCurrent.Text as string;
            do
            {
                int startColumn = rangeCurrent.MergeArea.Column;
                int startRow = rangeCurrent.MergeArea.Row;
                rangeCurrent.MergeArea.UpdateRange(startRow, startColumn, 
                    startRow + 1, startColumn);
                string arr1 = rangeStart.Text as string;
                arr2 = rangeCurrent.Text as string;
                if (arr1 != arr2)
                {
                    CellRange moduleNameRange = rangeStart;
                    string moduleName = moduleNameRange.Text as string;

                    if (modulesColor.ContainsKey(moduleName))
                    {
                        moduleNameRange.Style.Color = modulesColor[moduleName];
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
        private static void CreateObjectDevicesPage(ref Workbook workBook)
        {
            string sheetName = "Операции и устройства";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            CellRange excelCells = workSheet.Range["A1:C1"];
            
            // Производим объединение
            excelCells.Merge();
            excelCells.Value = "Технологические объекты";
            workSheet.Range["D1:L1"].Value2 = new string[] 
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
            workSheet.Range["A1:L1"].EntireColumn.AutoFitColumns();
            workSheet.Range["A1:L1"].EntireColumn.AutoFitRows();

            //Заполнение страницы данными
            TreeView tree = ExcelDataCollector
                .SaveTechObjectOperationsAndActionsAsTree();
            int row = 2;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);

            //Форматирование страницы
            workSheet.FreezePanes(2, 1);
            row = workSheet.Range.Rows.Length;
            workSheet.Range[$"A1:C{row}"].EntireColumn.AutoFitColumns();
            workSheet.Range[$"A1:C{row}"].EntireColumn.AutoFitRows();

            // установка переноса текста в ячейке.
            workSheet.Range.IsWrapText = true;
            workSheet.PageSetup.IsSummaryColumnRight = true;
        }

        /// <summary>
        /// Создание страницы с параметрами техобъектов проекта
        /// </summary>
        private static void CreateObjectParamsPage(ref Workbook workBook)
        {
            string sheetName = "Параметры объектов";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);

            // Настройка имен столбцов.
            workSheet.Range["A1:A1"].Value2 = new string[]
            {
                "Технологический объект"
            };
            CellRange excelCells = workSheet.Range["B1:C1"];
            excelCells.Merge();
            excelCells.Value = "Параметры";
            workSheet.Range["D1:G1"].Value2 = new string[] 
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
            workSheet.FreezePanes(2, 1);
            row = workSheet.Range.Rows.Length;
            workSheet.Range[$"A1:G{row}"].EntireColumn.AutoFitColumns();
            workSheet.Range[$"A1:G{row}"].EntireColumn.AutoFitRows();
            workSheet.PageSetup.IsSummaryColumnRight = true;
        }

        /// <summary>
        /// Создание страницы с описанием устройств
        /// </summary>
        private static void CreateInformDevicePage(ref Workbook workBook)
        {
            string sheetName = "Техустройства";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            workSheet.Range["A1:D1"].Value2 = new string[] 
            { 
                "Название", 
                "Описание", 
                "Тип", 
                "Подтип" 
            };
            object[,] res = ExcelDataCollector.SaveDevicesInformationAsArray();
            string endPos = "Q" + (res.GetLength(0) + 1);
            workSheet.Range[$"A2:{endPos}"].Value2 = res;

            // Форматирование по ширине содержимого.
            workSheet.Range.EntireColumn.AutoFitColumns();
            workSheet.Range.EntireColumn.AutoFitRows();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateTotalDevicePage(ref Workbook workBook)
        {
            string sheetName = "Сводная таблица устройств";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            object[,] res = ExcelDataCollector.SaveDevicesSummaryAsArray();
            string endPos = "Q" + res.GetLength(0);
            workSheet.Range[$"A1:{endPos}"].Value2 = res;
            workSheet.Range.EntireColumn.AutoFitRows();
            workSheet.Range.EntireColumn.AutoFitColumns();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateDeviceConnectionPage(ref Workbook workBook)
        {
            string sheetName = "Подключение устройств";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            TreeView tree = ExcelDataCollector.SaveDeviceConnectionAsTree();
            int row = 1;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);
            workSheet.Range.EntireColumn.AutoFitColumns();
            workSheet.Range.EntireColumn.AutoFitRows();
            workSheet.PageSetup.IsSummaryColumnRight = true;
        }

        /// <summary>
        /// Создание страницы с информацией об объектах (слепок редактора).
        /// </summary>
        /// <param name="workSheet"></param>
        /// <param name="app"></param>
        private static void CreateObjectsPageWithoutActions(
            ref Workbook workbook)
        {
            string sheetName = "Технологические объекты";
            Worksheet workSheet = workbook.Worksheets.Add(sheetName);

            const int widthColumnA = 40;
            const int widthColumnC = 55;
            const int widthColumnE = 45;

            TreeView tree = ExcelDataCollector
                .SaveObjectsWithoutActionsAsTree();
            int row = 1;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);
            workSheet.Range.EntireColumn.AutoFitColumns();
            workSheet.PageSetup.IsSummaryColumnRight = true;
            workSheet.Range[$"A1:A{row}"].ColumnWidth = widthColumnA;
            workSheet.Range[$"C1:C{row}"].ColumnWidth = widthColumnC;
            workSheet.Range[$"E1:E{row}"].ColumnWidth = widthColumnE;

            workSheet.Range.CollapseGroup(GroupByType.ByRows);
        }

        /// <summary>
        /// Запись узла дерева в Excel таблицу
        /// </summary>
        private static void WriteTreeNode(ref Worksheet workSheet,
            TreeNodeCollection Nodes, ref int row)
        {
            bool isCollapsed = true;
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
                    workSheet.Range[$"{firstCellAddress}:{secondCellAddress}"]
                        .Value2 = values;
                }
                else
                {
                    string[] srt = new string[] { node.Text };
                    string cellAddress = ParseColNum(node.Level) + 
                        row.ToString();
                    workSheet.Range[$"{cellAddress}:{cellAddress}"].Value2 = srt;

                }
                row++;
                
                WriteTreeNode(ref workSheet, node.Nodes, ref row);
                if (firstGroupRow != row)
                {
                    workSheet.Range[firstGroupRow, row - 1]
                       .GroupByRows(isCollapsed);
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
            ExportTechDevs(path, true);
        }
    }
}