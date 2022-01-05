using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using StaticHelper;
using System.Text.RegularExpressions;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель узлов и модулей ввода-вывода.
    /// </summary>
    public class IOReader
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public IOReader()
        {
            this.IOManager = IO.IOManager.GetInstance();
            this.isContainsNodes = false;
            this.isContainsA1 = false;
            this.IONameRegex = new Regex(IO.IOManager.IONamePattern);
            this.functionsForSearching = new List<Function>();
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
        public void Read()
        {
            PrepareForReading();
            ReadNodes();
            
            if (isContainsNodes == false)
            {
                Logs.AddMessage($"Не найден ни один узловой модуль " +
                    $"(A100, A200, ...).");
                return;
            }

            ReadModules(isContainsNodes);
        }

        /// <summary>
        /// Подготовка к чтению IO.
        /// </summary>
        private void PrepareForReading()
        {
            var project = ApiHelper.GetProject();
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
        private bool CheckA1()
        {
            List<int> theNumbers = new List<int>();
            foreach (var function in functionsForSearching)
            {
                Match match = IONameRegex.Match(function.VisibleName);
                if (match.Success)
                {
                    int number = Convert.ToInt32(match.Groups["n"].Value);
                    theNumbers.Add(number);
                }
            }

            if (theNumbers.Contains(numberA1) == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Чтение узлов ввода-вывода.
        /// </summary>
        private void ReadNodes()
        {
            foreach (var function in functionsForSearching)
            {
                bool needToSkip = NeedSkipNode(function);
                if (needToSkip == true)
                {
                    continue;
                }

                isContainsNodes = true;
                var match = IONameRegex.Match(function.VisibleName);
                int nodeNumber = Convert.ToInt32(match.Groups["n"].Value);
                string name = $"A{nodeNumber}";
                string ipAdress = GetIPAdressFromFunction(function);
                string type = GetNodeTypeFromFunction(function);

                if (type != "")
                {
                    if (isContainsA1 == true)
                    {
                        if (nodeNumber == numberA1)
                        {
                            IOManager.AddNode(numberA1, type, ipAdress, name);
                        }
                        else
                        {
                            IOManager.AddNode(nodeNumber/100 + numberA1, type, 
                                ipAdress, name);
                        }
                    }
                    else
                    {
                        IOManager.AddNode(nodeNumber/100, type, ipAdress, 
                            name);
                    }
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
        /// Нужно ли пропустить узел ввода-вывода.
        /// </summary>
        /// <param name="function">Функция</param>
        /// <returns></returns>
        private bool NeedSkipNode(Function function)
        {            
            bool skip = NeedSkipFunction(function);
            if (skip == true)
            {
                return skip;
            }

            var match = IONameRegex.Match(function.VisibleName);
            int nodeNumber = Convert.ToInt32(match.Groups["n"].Value);
            if (nodeNumber % 100 != 0 && nodeNumber != numberA1)
            {
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Получить IP-адрес из функции.
        /// </summary>
        /// <param name="function">Функция для поиска</param>
        /// <returns></returns>
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

                    IO.IONodeInfo.GetNodeInfo(type, out bool isValidPlcType);
                    if (isValidPlcType == true) break;
                }
            }

            return type;
        }

        /// <summary>
        /// Чтение модулей ввода-вывода
        /// </summary>
        /// <param name="isContainsNodes">Прочитаны или нет узлы</param>
        private void ReadModules(bool isContainsNodes)
        {
            foreach (var function in functionsForSearching)
            {
                bool needSkipModule = NeedSkipModule(function);
                if (needSkipModule == true)
                {
                    continue;
                }

                var match = IONameRegex.Match(function.VisibleName);
                int moduleNumber = Convert.ToInt32(match.Groups["n"].Value);
                int shortModuleNumber = moduleNumber % 100;
                int shortNodeNumber;
                if (isContainsA1 == true)
                {
                    shortNodeNumber = moduleNumber/100;
                }
                else
                {
                    shortNodeNumber = moduleNumber/100 - numberA1;
                }

                string type = GetModuleTypeFromFunction(function);
                IO.IONode node = IOManager[shortNodeNumber];
                if (IOManager[shortNodeNumber] != null)
                {
                    IO.IOModuleInfo moduleInfo = GetIOModuleInfo(function, 
                        type);

                    int inOffset;
                    int outOffset;
                    GetInAndOutOffset(shortNodeNumber, moduleInfo,
                        out inOffset, out outOffset);

                    IO.IOModule nodeModule = new IO.IOModule(inOffset,
                        outOffset, moduleInfo, moduleNumber, function);

                    node.DI_count += moduleInfo.DICount;
                    node.DO_count += moduleInfo.DOCount;
                    node.AI_count += moduleInfo.AICount;
                    node.AO_count += moduleInfo.AOCount;

                    bool moduleNotExist = node[shortModuleNumber - 1] == null;
                    if(moduleNotExist)
                    {
                        node.SetModule(nodeModule, shortModuleNumber);
                    }
                    else
                    {
                        Logs.AddMessage($"Главная функция модуля " +
                            $"ввода-вывода \'{function.VisibleName}\' " +
                            $"определяется дважды, проверьте расстановку " +
                            $"главных функций на модулях. ");
                    }
                }
                else
                {
                    Logs.AddMessage($"Для" +
                        $" \"{function.VisibleName}\" - \"{type}\", " +
                        $"не найден узел номер {++shortNodeNumber}.");
                }
            }
        }

        /// <summary>
        /// Нужно ли пропустить модуль ввода-вывода.
        /// </summary>
        /// <param name="function">Функция для проверки</param>
        /// <returns></returns>
        private bool NeedSkipModule(Function function)
        {            
            bool skip = NeedSkipFunction(function);
            if (skip == true)
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
        private bool NeedSkipFunction(Function function)
        {
            var skip = false;
            var match = IONameRegex.Match(function.VisibleName);
            if (match.Success == false ||
                !function.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
            {
                skip = true;
                return skip;
            }

            if (function.Articles.GetLength(0) == 0)
            {
                Logs.AddMessage($"У модуля \"" +
                    $"{function.VisibleName}\" не задано изделие.");
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Сравнение функций между собой
        /// </summary>
        /// <param name="x">Функция 1</param>
        /// <param name="y">Функция 2</param>
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