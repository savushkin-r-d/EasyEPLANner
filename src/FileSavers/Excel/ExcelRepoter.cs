using EplanDevice;
using Spire.Xls;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
            workBook.Worksheets.Clear();

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

            CreateDeviceArticlesPage(ref workBook);
            Logs.SetProgress(100);
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

                workBook.SetWriteProtectionPassword("1");
                workBook.SaveToFile(fileName, ExcelVersion.Version2010);
            }
            else
            {
                workBook.SaveToFile(fileName, ExcelVersion.Version2010);
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

            var modulesCount = new Dictionary<string, int>();
            var modulesColor = new Dictionary<string, System.Drawing.Color>();
            var asInterfaceConnection = new Dictionary<string, object[,]>();

            object[,] res = ExcelDataCollector.SaveIOAsConnectionArray(prjName,
                modulesCount, modulesColor, asInterfaceConnection);

            workSheet.InsertArray(res, 1, 1);
            int finalRows = res.GetLength(0) + 2;

            //Форматирование страницы
            workSheet.Range.BorderInside(LineStyleType.Thin);
            workSheet.Range.BorderAround(LineStyleType.Medium);
            workSheet.Range.Style.VerticalAlignment = VerticalAlignType
                .Center;
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            workSheet.Range.IsWrapText = false;

            CellRange rangeCurrent = workSheet.Range["A1:A1"];
            CellRange rangeStart = rangeCurrent;
            int totalCountRows = workSheet.Range.Rows.Length;
            int i = 1;
            string arr2 = rangeCurrent.Text as string;
            do
            {
                int startColumn = rangeCurrent.Column;
                int startRow = rangeCurrent.Row;
                rangeCurrent = workSheet.Range[startRow + 1, startColumn, 
                    startRow + 1, startColumn];
                string arr1 = rangeStart.Value as string;
                arr2 = rangeCurrent.Value as string;
                if (arr1 != arr2)
                {
                    workSheet.Range[rangeStart.Row, rangeStart.Column, 
                        rangeCurrent.Row - 1, rangeCurrent.Column].Merge();
                    CellRange moduleNameRange = workSheet.Range[rangeStart.Row, 
                        rangeStart.Column + 1, rangeStart.Row, 
                        rangeStart.Column + 1];

                    string moduleName = moduleNameRange.Value as string;

                    if (modulesColor.ContainsKey(moduleName))
                    {
                        moduleNameRange.Style.Color = modulesColor[moduleName];
                    }

                    if (Int32.TryParse(arr1, out _))
                    {
                        workSheet.Range[rangeStart.Row, rangeStart.Column + 1, 
                            rangeCurrent.Row - 1, rangeCurrent.Column + 1]
                            .Merge();
                        workSheet.Range[rangeStart.Row, rangeStart.Column, 
                            rangeCurrent.Row - 1, rangeCurrent.Column + 6]
                            .BorderAround(LineStyleType.Thick);
                    }
                    else
                    {
                        workSheet.Range[rangeStart.Row, rangeStart.Column, 
                            rangeCurrent.Row - 1, rangeCurrent.Column + 6]
                            .Borders.LineStyle = LineStyleType.None;
                    }
                    rangeStart = rangeCurrent;
                }
                i++;
            }
            while (i <= totalCountRows);

            // Форматирование по ширине содержимого.
            workSheet.Range.EntireColumn.AutoFitColumns();
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
                workSheet.InsertArray(ASInterface, totalStart, 1);

                column = workSheet.Range[$"A{totalStart}:A{totalEnd}"];
                column.HorizontalAlignment = HorizontalAlignType.Center;
                column = workSheet.Range[$"C{totalStart}:C{totalEnd}"];
                column.HorizontalAlignment = HorizontalAlignType.Center;

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
            workSheet.InsertArray(modulesTotal, totalStart, 1);
            rangeCurrent.Style.Rotation = 0;
            rangeCurrent.VerticalAlignment = VerticalAlignType.Center;
            rangeCurrent.HorizontalAlignment = HorizontalAlignType.Right;
            rangeCurrent.BorderInside(LineStyleType.Thin);
            rangeCurrent.BorderAround(LineStyleType.Medium);


            rangeCurrent = workSheet.Range[$"A{totalStart}:A{totalStart}"];
            rangeStart = rangeCurrent;

            workSheet.Range.EntireRow.AutoFitRows();

            // Окрас ячеек
            i = totalStart;
            arr2 = rangeCurrent.Text as string;
            do
            {
                int startColumn = rangeCurrent.Column;
                int startRow = rangeCurrent.Row;
                rangeCurrent = workSheet.Range[startRow + 1, startColumn, 
                    startRow + 1, startColumn];
                string arr1 = rangeStart.Value as string;
                arr2 = rangeCurrent.Value as string;
                if (arr1 != arr2)
                {
                    CellRange moduleNameRange = rangeStart;
                    string moduleName = moduleNameRange.Value as string;

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
            var header = new string[] 
            { 
                "Проверяемые Устройства",
                "Включать",
                "Включать с задержкой",
                "Включать реверс",
                "Выключать",
                "Выключать с задержкой",
                "Верхние седла" , 
                "Нижние седла",
                "Сигналы для включения",
                "Устройства", 
                "Группы DI-->DO",
                "Группы инвертированный DI-->DO",
                "Группы AI-->AO",
                "Сигналы для включения шага",
                "Переход к шагу по условию",
                "Время(параметр)",
                "Номер следующего шага",
            };
            workSheet.InsertArray(header, 1, 4, false);
            workSheet.Range["A1:L1"].EntireColumn.AutoFitColumns();

            //Заполнение страницы данными
            TreeView tree = ExcelDataCollector
                .SaveTechObjectOperationsAndActionsAsTree();
            int row = 2;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);

            //Форматирование страницы
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            workSheet.FreezePanes(2, 1);
            row = workSheet.Range.Rows.Length;
            workSheet.Range[$"A1:C{row}"].EntireColumn.AutoFitColumns();

            // установка переноса текста в ячейке.
            workSheet.Range.IsWrapText = true;
            workSheet.PageSetup.IsSummaryColumnRight = true;
            workSheet.PageSetup.IsSummaryRowBelow = false;

            workSheet.Range.EntireRow.AutoFitRows();
        }

        /// <summary>
        /// Создание страницы с параметрами техобъектов проекта
        /// </summary>
        private static void CreateObjectParamsPage(ref Workbook workBook)
        {
            string sheetName = "Параметры объектов";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);

            // Настройка имен столбцов.
            workSheet.Range["A1:A1"].Text = "Технологический объект";
            CellRange excelCells = workSheet.Range["B1:C1"];
            excelCells.Merge();
            excelCells.Value = "Параметры";
            var paramsHeader = new string[] 
            { 
                "Значение", 
                "Размерность", 
                "Операция", 
                "Lua имя"
            };
            workSheet.InsertArray(paramsHeader, 1, 4, false);
            
            // Получить и записать данные
            TreeView tree = ExcelDataCollector.SaveParamsAsTree();
            int row = 2;
            WriteTreeNode(ref workSheet, tree.Nodes, ref row);

            // Форматирование страницы.
            workSheet.FreezePanes(2, 1);
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            row = workSheet.Range.Rows.Length;
            workSheet.Range[$"A1:G{row}"].EntireColumn.AutoFitColumns();
            workSheet.PageSetup.IsSummaryColumnRight = true;
            workSheet.PageSetup.IsSummaryRowBelow = false;

            workSheet.Range.EntireRow.AutoFitRows();
        }

        /// <summary>
        /// Создание страницы с описанием устройств
        /// </summary>
        private static void CreateInformDevicePage(ref Workbook workBook)
        {
            string sheetName = "Техустройства";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            var deviceHeader = new string[] 
            { 
                "Название", 
                "Описание", 
                "Тип", 
                "Подтип" 
            };
            workSheet.InsertArray(deviceHeader, 1, 1, false);
            object[,] res = ExcelDataCollector.SaveDevicesInformationAsArray();
            string endPos = "Q" + (res.GetLength(0) + 1);
            workSheet.InsertArray(res, 2, 1);

            // Форматирование по ширине содержимого.
            foreach (var row in workSheet.Range.Rows)
            {
                var notNullCells = row.CellList.Where(x => x.Text != null);
                var multilineCells = notNullCells.Where(x => x.Text.Contains("\n"));
                if (multilineCells.Count() > 0)
                {
                    row.IsWrapText = true;
                }
            }
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            workSheet.Range.AutoFitColumns();

            workSheet.Range.EntireRow.AutoFitRows();
        }

        /// <summary>
        /// Создание страницы с итоговыми данными по устройствам
        /// </summary>
        private static void CreateTotalDevicePage(ref Workbook workBook)
        {
            string sheetName = "Сводная таблица устройств";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            workSheet.InsertArray(
                ["Тип", "Подтип", "Количество", "DI", "DO", "AI", "AO", "Всего каналов"],
                1, 1, false);
            workSheet.FreezePanes(2, 1);
            workSheet.Range[1, 1, 1, 8].Style.Font.IsBold = true;


            var devices = ExcelDataCollector.GetTypesCount();
            var rowIndex = 2;
            foreach (var typeIdx in Enumerable.Range(0, devices.Count))
            {
                var type = devices.ElementAt(typeIdx).Key;
                var subtypes = devices.ElementAt(typeIdx).Value;

                workSheet[rowIndex, 1].Value = type;
                var typeRange = workSheet.Range[rowIndex, 1, rowIndex + subtypes.Count - 1, 1];
                typeRange.Merge();
                typeRange.Style.VerticalAlignment = VerticalAlignType.Center;
                typeRange.Style.Font.IsBold = true;


                foreach (var subtypeIdx in Enumerable.Range(0, subtypes.Count))
                {
                    var subtype = subtypes.ElementAt(subtypeIdx).Key;
                    var count = subtypes.ElementAt(subtypeIdx).Value;

                    workSheet.InsertArray([subtype, count, ..ExcelDataCollector.GetChannelsCount(subtype)], rowIndex + subtypeIdx, 2, false);
                    workSheet[rowIndex + subtypeIdx, 8].Formula = $"=SUM(D{rowIndex + subtypeIdx}:G{rowIndex + subtypeIdx})*C{rowIndex + subtypeIdx}";
                }

                rowIndex += subtypes.Count;
            }

            workSheet[rowIndex, 1].Value = "Всего:";
            workSheet[rowIndex, 1].Style.HorizontalAlignment = HorizontalAlignType.Right;
            workSheet[rowIndex, 1].Style.Font.IsBold = true;
            
            workSheet[rowIndex, 2].Formula = $"=COUNTA(B2:B{rowIndex - 1})";
            workSheet[rowIndex, 3].Formula = $"=SUM(C2:C{rowIndex - 1})";
            workSheet[rowIndex, 3].Formula = $"=SUM(C2:C{rowIndex - 1})";
            workSheet[rowIndex, 4].Formula = $"=SUMPRODUCT(C2:C{rowIndex - 1},D2:D{rowIndex - 1})";
            workSheet[rowIndex, 5].Formula = $"=SUMPRODUCT(C2:C{rowIndex - 1},E2:E{rowIndex - 1})";
            workSheet[rowIndex, 6].Formula = $"=SUMPRODUCT(C2:C{rowIndex - 1},F2:F{rowIndex - 1})";
            workSheet[rowIndex, 7].Formula = $"=SUMPRODUCT(C2:C{rowIndex - 1},G2:G{rowIndex - 1})";
            workSheet[rowIndex, 8].Formula = $"=SUM(H2:H{rowIndex - 1})";;

            workSheet[rowIndex, 1, rowIndex, 8].Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thick;

            workBook.CalculateAllValue();

            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;           
            workSheet.Range.EntireColumn.AutoFitColumns();

            workSheet.Range.EntireRow.AutoFitRows();
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
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            workSheet.Range.EntireColumn.AutoFitColumns();
            workSheet.Range.EntireColumn.IsWrapText = true;
            workSheet.PageSetup.IsSummaryRowBelow = false;
            workSheet.PageSetup.IsSummaryColumnRight = true;

            workSheet.Range.EntireRow.AutoFitRows();
        }

        /// <summary>
        /// Создание страницы с изделиями устройств
        /// </summary>
        /// <param name="workBook"></param>
        private static void CreateDeviceArticlesPage(ref Workbook workBook)
        {
            string sheetName = "Изделия устройств";
            Worksheet workSheet = workBook.Worksheets.Add(sheetName);
            object[,] devicesWithArticles = ExcelDataCollector
                .SaveDevicesArticlesInfoAsArray();
            workSheet.InsertArray(devicesWithArticles, 1, 1);
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            workSheet.Range.EntireColumn.AutoFitColumns();

            workSheet.Range.EntireRow.AutoFitRows();
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
            WriteTreeNode(ref workSheet, tree.Nodes, ref row, true);
            workSheet.Range.Style.Font.FontName = "Calibri";
            workSheet.Range.Style.Font.Size = 11;
            workSheet.Range.EntireColumn.AutoFitColumns();
            workSheet.PageSetup.IsSummaryRowBelow = false;
            workSheet.PageSetup.IsSummaryColumnRight = true;
            workSheet.Range[$"A1:A{row}"].ColumnWidth = widthColumnA;
            workSheet.Range[$"C1:C{row}"].ColumnWidth = widthColumnC;
            workSheet.Range[$"E1:E{row}"].ColumnWidth = widthColumnE;

            workSheet.Range.EntireRow.AutoFitRows();
        }

        /// <summary>
        /// Запись узла дерева в Excel таблицу
        /// </summary>
        private static void WriteTreeNode(ref Worksheet workSheet,
            TreeNodeCollection Nodes, ref int row, bool collapse = false)
        {
            foreach (TreeNode node in Nodes)
            {
                int firstGroupRow = row + 1;
                if (node.Tag is string[])
                {
                    string[] values = node.Tag as string[];
                    int firstColumn = node.Level + 1;
                    workSheet.InsertArray(values, row, firstColumn, false);
                }
                else
                {
                    var srt = node.Text.ToString();
                    string cellAddr = ParseColNum(node.Level) + 
                        row.ToString();
                    workSheet.Range[$"{cellAddr}:{cellAddr}"].Value2 = srt;
                }
                row++;

                WriteTreeNode(ref workSheet, node.Nodes, ref row, collapse);
                if (firstGroupRow != row)
                {
                    workSheet.GroupByRows(firstGroupRow, row - 1, collapse);
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
        /// Экспорт информации о проекте для SCADA системы
        /// </summary>
        /// <param name="projectName">Название проекта</param>
        /// <param name="projectDirPath">Путь к папке проекта Eplan</param>
        public static void AutomaticExportExcelForSCADA(string projectDirPath,
            string projectName)
        {
            string path = projectDirPath + @"\DOC\" + projectName +
                " auto report.xlsx";
            ExportTechDevs(path, true);
        }
    }
}