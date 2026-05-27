using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using StaticHelper;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель узлов и модулей ввода-вывода.
    /// </summary>
    public class IOReader
    {
        IDeviceHelper deviceHelper;
        IProjectHelper projectHelper;

        public IOReader(IProjectHelper projectHelper, IDeviceHelper deviceHelper)
        {
            this.IOManager = IO.IOManager.GetInstance();
            this.isContainsNodes = false;
            this.isContainsA1 = false;
            this.IONameRegex = new Regex(IO.IOManager.IONamePattern);
            this.functionsForSearching = new List<Function>();
            this.projectHelper = projectHelper;
            this.deviceHelper = deviceHelper;
        }

        /// <summary>
        /// Чтение конфигурации узлов и модулей ввода-вывода.
        /// 
        /// Считывание информации о модулях из проекта.
        /// Информацию получаем на основе листа с шиной модулей ввода\вывода 
        /// IO. У изделий должны быть заполнены соответствующие свойства,
        /// они должны соответствующим образом именоваться.
        /// Алгоритм:
        /// 1. С помощью фильтра выбираем функциональные объекты  в пределах 
        /// открытого проекта со следующими ограничениями: категория PLCBox,
        /// главная функция, производитель изделия WAGO/ Phoenix.
        /// 2. Обрабатываем функциональные объекты со следующим именем:
        /// -Аxxx, где А - признак элемента IO, xxx - номер.
        /// Также учитываем контроллер Phoenix Contact с ОУ "А1", если он есть.
        /// 3. Вначале обрабатываем элементы с номером, кратным 100 - это узел 
        /// IO. Добавляем их в список узлов IO. Тип узла получаем из 
        /// изделия.
        /// 4. Обрабатываем остальные элементы - модули IO. Помещаем их в 
        /// списки согласно порядковому номеру (вставляем при этом пустые 
        /// элементы, если длина списка короче номера нового добавляемого 
        /// модуля).
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Read()
        {
            PrepareForReading();
            ReadNodes();
            
            if (!isContainsNodes)
            {
                Logs.AddMessage($"Не найден ни один узловой модуль " +
                    $"(A100, A200, ...).");
                return;
            }

            ReadModules();
            ReadDeletedModules();
        }

        /// <summary>
        /// Подготовка к чтению IO.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void PrepareForReading()
        {
            var project = (projectHelper as ProjectHelper).GetProject();
            var objectsFinder = new DMObjectsFinder(project);
            var functionsFilter = new FunctionsFilter();

            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;

            functionsFilter.SetFilteredPropertyList(properties);
            functionsFilter.Category = Function.Enums.Category.PLCBox;

            functionsForSearching = objectsFinder.GetFunctions(functionsFilter)
                .ToList();
            functionsForSearching.Sort(CompareFunctions);

            IOManager.Clear();
            isContainsNodes = false;
            isContainsA1 = CheckA1();
        }

        /// <summary>
        /// Функция проверки наличия узлового модуля
        /// типа А1, на котором находится управляющая
        /// программа (для Phoenix Contact)
        /// </summary>
        [ExcludeFromCodeCoverage]
        private bool CheckA1()
        {
            List<int> theNumbers = new List<int>();
            foreach (var function in functionsForSearching)
            {
                Match match = IONameRegex.Match(function.VisibleName);
                if (match.Success && !IsExtensionNode(match))
                {
                    int number = Convert.ToInt32(match.Groups["n"].Value);
                    theNumbers.Add(number);
                }
            }

            if (theNumbers.Contains(numberA1))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Чтение узлов ввода-вывода.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void ReadNodes()
        {
            foreach (var function in functionsForSearching)
            {
                bool needToSkip = NeedSkipNode(function);
                if (needToSkip)
                {
                    continue;
                }

                var match = IONameRegex.Match(function.VisibleName);
                if (IsExtensionNode(match))
                {
                    ReadExtensionNode(function, match);
                    continue;
                }

                isContainsNodes = true;
                int nodeNumber = Convert.ToInt32(match.Groups["n"].Value);
                string name = $"A{nodeNumber}";
                string ipAdress = GetIPAdressFromFunction(function);
                string type = GetNodeTypeFromFunction(function);
                string location = function.Properties
                    .DESIGNATION_FULLLOCATION_WITHPREFIX;
                string locationDescription = function.Properties.DESIGNATION_FULLLOCATION_DESCR.GetString();


                if (type != "")
                {
                    var node = IOManager.AddNode(nodeNumber / 100 + (isContainsA1 ? 1 : 0),
                        nodeNumber, type, ipAdress, name, location, locationDescription);

                    node.SetEplanFunction(new EplanFunction(function));
                }
                else
                {
                    Logs.AddMessage($"У модуля \"" +
                        $"{function.VisibleName}\" не задан параметр изделия" +
                        $" (номер типа изделия).");
                }
            }
        }

        /// <summary>
        /// Чтение модуля расширения узла.
        /// </summary>
        /// <param name="function">Функция модуля расширения.</param>
        /// <param name="match">Результат разбора имени функции.</param>
        [ExcludeFromCodeCoverage]
        private void ReadExtensionNode(Function function, Match match)
        {
            int nodeNumber = Convert.ToInt32(match.Groups["n"].Value);
            int extensionNumber = Convert.ToInt32(match.Groups["ext"].Value);
            string name = $"A{nodeNumber}.{extensionNumber}";
            string ipAdress = GetIPAdressFromFunction(function);
            string type = GetNodeTypeFromFunction(function);
            string location = function.Properties
                .DESIGNATION_FULLLOCATION_WITHPREFIX;
            string locationDescription = function.Properties.DESIGNATION_FULLLOCATION_DESCR.GetString();

            if (type == "")
            {
                Logs.AddMessage($"У модуля \"" +
                    $"{function.VisibleName}\" не задан параметр изделия" +
                    $" (номер типа изделия).");
                return;
            }

            int parentNodeIdx = GetNodeIdx(nodeNumber);
            try
            {
                var extensionNode = IOManager.AddExtensionNode(parentNodeIdx,
                    new IO.IOManager.ExtensionNodeInfo
                    {
                        ExtensionNumber = extensionNumber,
                        NodeNumber = nodeNumber,
                        Type = type,
                        IP = ipAdress,
                        Name = name,
                        Location = location,
                        LocationDescription = locationDescription
                    });

                if (extensionNode is null)
                {
                    Logs.AddMessage($"Для модуля расширения \"{function.VisibleName}\" - \"{type}\"" +
                        $", не найден узел \"A{nodeNumber}\".");
                    return;
                }

                extensionNode.SetEplanFunction(new EplanFunction(function));
            }
            catch (InvalidOperationException ex)
            {
                Logs.AddMessage(ex.Message);
            }
            catch (FormatException ex)
            {
                Logs.AddMessage(ex.Message);
            }
            catch (OverflowException ex)
            {
                Logs.AddMessage(ex.Message);
            }
        }

        /// <summary>
        /// Нужно ли пропустить узел ввода-вывода.
        /// </summary>
        /// <param name="function">Функция</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool NeedSkipNode(Function function)
        {            
            bool skip = NeedSkipFunction(function);
            if (skip)
            {
                return skip;
            }

            var match = IONameRegex.Match(function.VisibleName);
            int nodeNumber = Convert.ToInt32(match.Groups["n"].Value);
            if (IsExtensionNode(match))
            {
                return nodeNumber % 100 != 0 && nodeNumber != numberA1;
            }

            if (nodeNumber % 100 != 0 && nodeNumber != numberA1)
            {
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Является ли имя функции модулем расширения.
        /// </summary>
        /// <param name="match">Результат разбора имени функции.</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private static bool IsExtensionNode(Match match)
        {
            return match.Groups["ext"].Success &&
                !string.IsNullOrEmpty(match.Groups["ext"].Value);
        }

        /// <summary>
        /// Получить индекс родительского узла.
        /// </summary>
        /// <param name="nodeNumber">Физический номер узла.</param>
        /// <returns>Индекс родительского узла.</returns>
        [ExcludeFromCodeCoverage]
        private int GetNodeIdx(int nodeNumber)
        {
            if (nodeNumber == numberA1)
            {
                return isContainsA1 ? 0 : -1;
            }

            return nodeNumber / 100 + (isContainsA1 ? 1 : 0) - 1;
        }

        /// <summary>
        /// Получить IP-адрес из функции.
        /// </summary>
        /// <param name="function">Функция для поиска</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string GetIPAdressFromFunction(Function function)
        {
            var ipAddress = "";
            if (!function.Properties.FUNC_PLCGROUP_STARTADDRESS.IsEmpty)
            {
                ipAddress = function.Properties.FUNC_PLCGROUP_STARTADDRESS;
            }
            else
            {
                Logs.AddMessage($"У узла \"" +
                    $"{function.VisibleName}\" не задан IP-адрес.");
            }

            return ipAddress;
        }

        /// <summary>
        /// Получить тип узла ввода-вывода.
        /// </summary>
        /// <param name="function">Функция для поиска</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string GetNodeTypeFromFunction(Function function)
        {
            var type = string.Empty;
            foreach (var article in function.Articles)
            {
                if (!article.Properties[Eplan.EplApi.DataModel
                    .Properties.Article.ARTICLE_TYPENR].IsEmpty)
                {
                    type = article.Properties[Eplan.EplApi.DataModel
                        .Properties.Article.ARTICLE_TYPENR];

                    IO.IONodeInfo.GetNodeInfo(type, out bool isStub);
                    if (!isStub) break;
                }
            }

            return type;
        }

        /// <summary>
        /// Чтение модулей ввода-вывода
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void ReadModules()
        {
            foreach (var function in functionsForSearching)
            {
                if (NeedSkipModule(function))
                {
                    continue;
                }

                ReadModule(function);
            }
        }

        [ExcludeFromCodeCoverage]
        private void ReadDeletedModules()
        {
            foreach (var function in functionsForSearching)
            {
                var match = DeletedIONameRegex.Match(function.VisibleName);
                if (!match.Success || !FunctionHasArticle(function))
                {
                    continue;
                }

                ReadDeletedModule(function, match);
            }
        }

        [ExcludeFromCodeCoverage]
        private void ReadDeletedModule(Function function, Match match)
        {
            int moduleNumber = Convert.ToInt32(match.Groups["n"].Value);
            string type = GetModuleTypeFromFunction(function);
            IO.IOModuleInfo moduleInfo = GetIOModuleInfo(function, type);

            IO.IOModule module = new IO.IOModule(0, 0, moduleInfo,
                moduleNumber, deviceHelper.GetArticleName(function),
                new EplanFunction(function), "DEL",
                function.Properties.DESIGNATION_FULLLOCATION_WITHPREFIX,
                function.Properties.DESIGNATION_FULLLOCATION_DESCR.GetString());

            IOManager.AddDeletedModule(module);
        }

        [ExcludeFromCodeCoverage]
        private void ReadModule(Function function)
        {
            var match = IONameRegex.Match(function.VisibleName);
            int moduleNumber = Convert.ToInt32(match.Groups["n"].Value);
            int shortModuleNumber = moduleNumber % 100;
            int shortNodeNumber = moduleNumber / 100 - (isContainsA1 ? 0 : 1);

            string type = GetModuleTypeFromFunction(function);
            IO.IIONode node = IOManager[shortNodeNumber];

            if (node is null)
            {
                LogMissingNode(function.VisibleName, type, shortNodeNumber + 1);
                return;
            }

            IO.IOModuleInfo moduleInfo = GetIOModuleInfo(function, type);

            GetInAndOutOffset(shortNodeNumber, moduleInfo,
                out int inOffset, out int outOffset);

            IO.IOModule nodeModule = new IO.IOModule(inOffset,
                outOffset, moduleInfo, moduleNumber,
                deviceHelper.GetArticleName(function), new EplanFunction(function));

            AddModuleToNode(node, nodeModule, moduleInfo,
                shortModuleNumber, function.VisibleName);
        }

        [ExcludeFromCodeCoverage]
        private static void AddModuleToNode(IO.IIONode node,
            IO.IOModule nodeModule, IO.IOModuleInfo moduleInfo,
            int shortModuleNumber, string visibleName)
        {
            node.DI_count += moduleInfo.DICount;
            node.DO_count += moduleInfo.DOCount;
            node.AI_count += moduleInfo.AICount;
            node.AO_count += moduleInfo.AOCount;

            if (node[shortModuleNumber - 1] is null)
            {
                SetModule(node, nodeModule, shortModuleNumber);
                return;
            }

            LogDuplicateModule(visibleName);
        }

        [ExcludeFromCodeCoverage]
        private static void SetModule(IO.IIONode node,
            IO.IOModule nodeModule, int shortModuleNumber)
        {
            try
            {
                node.SetModule(nodeModule, shortModuleNumber);
            }
            catch (IO.IONode.AddressAreaNullReferenceException ex)
            {
                Logs.AddMessage(ex.Message);
            }
            catch (IO.IONode.ModulesPerNodeOutOfRageException ex)
            {
                Logs.AddMessage(ex.Message);
            }
            catch (IO.IONode.AddressAreaOutOfRangeException ex)
            {
                Logs.AddMessage(ex.Message);
            }
            catch (IO.IONode.IndefiniteModulesException ex)
            {
                Logs.AddMessage(ex.Message);
            }
        }

        [ExcludeFromCodeCoverage]
        private static void LogMissingNode(string visibleName, string type,
            int nodeNumber)
        {
            Logs.AddMessage($"Для \"{visibleName}\" - \"{type}\"," +
                $" не найден узел номер {nodeNumber}.");
        }

        [ExcludeFromCodeCoverage]
        private static void LogDuplicateModule(string visibleName)
        {
            Logs.AddMessage($"Главная функция модуля " +
                $"ввода-вывода \'{visibleName}\' " +
                $"определяется дважды, проверьте расстановку " +
                $"главных функций на модулях. ");
        }

        /// <summary>
        /// Нужно ли пропустить модуль ввода-вывода.
        /// </summary>
        /// <param name="function">Функция для проверки</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool NeedSkipModule(Function function)
        {            
            bool skip = NeedSkipFunction(function);
            if (skip)
            {
                return skip;
            }

            var match = IONameRegex.Match(function.VisibleName);
            int moduleNumber = Convert.ToInt32(match.Groups["n"].Value);
            if (moduleNumber % 100 == 0 || moduleNumber == numberA1)
            {
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Получить тип модуля ввода-вывода из функции.
        /// </summary>
        /// <param name="function">Функция для проверки</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private string GetModuleTypeFromFunction(Function function)
        {
            var type = "";
            if (!function.Articles[0].Properties[Eplan.EplApi.DataModel
                .Properties.Article.ARTICLE_TYPENR].IsEmpty)
            {
                type = function.Articles[0].Properties[Eplan.EplApi
                    .DataModel.Properties.Article.ARTICLE_TYPENR]
                    .ToString().Trim();
            }
            else
            {
                Logs.AddMessage($"У модуля \"" +
                    $"{function.VisibleName}\" не задан параметр изделия" +
                    $" (номер типа изделия).");
            }

            return type;
        }

        /// <summary>
        /// Получить информацию о модуле ввода-вывода
        /// </summary>
        /// <param name="function">Функция</param>
        /// <param name="type">Тип модуля ввода-вывода</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private IO.IOModuleInfo GetIOModuleInfo(Function function, string type)
        {
            bool isStub;
            var moduleInfo = IO.IOModuleInfo.GetModuleInfo(type, out isStub);
            if (isStub && type != "")
            {
                Logs.AddMessage($"Неизвестный " +
                    $"модуль \"{function.VisibleName}\" - \"{type}\".");
            }

            return moduleInfo;
        }

        private void GetInAndOutOffset(int shortNodeNumber, 
            IO.IOModuleInfo moduleInfo, out int inOffset, out int outOffset)
        {
            inOffset = 0;
            outOffset = 0;
            switch (moduleInfo.AddressSpaceType)
            {
                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                    inOffset = IOManager[shortNodeNumber].DI_count;
                    break;

                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                    outOffset = IOManager[shortNodeNumber].DO_count;
                    break;

                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                    inOffset = IOManager[shortNodeNumber].AI_count;
                    break;

                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                    outOffset = IOManager[shortNodeNumber].AO_count;
                    break;

                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI:
                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI:
                    inOffset = IOManager[shortNodeNumber].AI_count;
                    outOffset = IOManager[shortNodeNumber].AO_count;
                    break;

                case IO.IOModuleInfo.ADDRESS_SPACE_TYPE.DODI:
                    inOffset = IOManager[shortNodeNumber].DI_count;
                    outOffset = IOManager[shortNodeNumber].DO_count;
                    break;
            }
        }

        /// <summary>
        /// Нужно ли пропустить функцию.
        /// </summary>
        /// <param name="function">Функция для проверки</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool NeedSkipFunction(Function function)
        {
            var skip = false;
            var match = IONameRegex.Match(function.VisibleName);
            if (!match.Success ||
                !function.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
            {
                skip = true;
                return skip;
            }

            if (!FunctionHasArticle(function))
            {
                Logs.AddMessage($"У модуля \"" +
                    $"{function.VisibleName}\" не задано изделие.");
                skip = true;
                return skip;
            }

            return skip;
        }

        private static bool FunctionHasArticle(Function function)
        {
            return function.Articles.GetLength(0) != 0;
        }

        /// <summary>
        /// Сравнение функций между собой
        /// </summary>
        /// <param name="x">Функция 1</param>
        /// <param name="y">Функция 2</param>
        [ExcludeFromCodeCoverage]
        private static int CompareFunctions(Function x, Function y)
        {
            return x.VisibleName.CompareTo(y.VisibleName);
        }

        /// <summary>
        /// Содержит ли проект узлы ввода-вывода.
        /// </summary>
        bool isContainsNodes;

        /// <summary>
        /// Содержит ли проект управляющий контроллер А1.
        /// </summary>
        bool isContainsA1;

        /// <summary>
        /// Функции для поиска узлов и модулей ввода-вывода.
        /// </summary>
        List<Function> functionsForSearching;

        /// <summary>
        /// Обрабатывающий Regex.
        /// </summary>
        Regex IONameRegex;

        Regex DeletedIONameRegex = new Regex(@"=*-DEL(?<n>\d+)$");

        /// <summary>
        /// Номер узла А1, характерного для проектов, где используется 
        /// Phoenix Contact контроллер для управляющей программы
        /// </summary>
        const int numberA1 = 1;

        /// <summary>
        /// Менеджер узлов и модулей ввода-вывода.
        /// </summary>
        IO.IOManager IOManager;
    }
}