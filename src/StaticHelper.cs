using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using LuaInterface;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StaticHelper
{
    /// <summary>
    /// Обертка над Eplan API
    /// </summary>
    public static class ApiHelper
    {
        /// <summary>
        /// Получить текущий проект
        /// </summary>
        /// <returns>Проект</returns>
        public static Project GetProject()
        {
            SelectionSet selection = GetSelectionSet();
            const bool useDialog = false;
            Project project = selection.GetCurrentProject(useDialog);
            return project;
        }

        /// <summary>
        /// Получить выборку из выбранных объектов в Eplan API
        /// </summary>
        /// <returns></returns>
        private static SelectionSet GetSelectionSet()
        {
            var selection = new SelectionSet();
            selection.LockSelectionByDefault = false;
            return selection;
        }

        /// <summary>
        /// Получить выбранный на графической схеме объект
        /// </summary>
        /// <returns>Выбранный на схеме объект</returns>
        public static StorableObject GetSelectedObject()
        {
            SelectionSet selection = GetSelectionSet();
            const bool isFirstObject = true;
            StorableObject selectedObject = selection.
                GetSelectedObject(isFirstObject);
            return selectedObject;
        }

        /// <summary>
        /// Получить функцию выбранной клеммы
        /// </summary>
        /// <param name="selectedObject">Выбранный на схеме объект
        /// </param>
        /// <returns>Функция клеммы модуля ввода-вывода</returns>
        public static Function GetClampFunction(
            StorableObject selectedObject)
        {
            if (selectedObject is Function == false)
            {
                const string Message = "Выбранный на схеме объект не " +
                    "является функцией";
                throw new Exception(Message);
            }

            var clampFunction = selectedObject as Function;

            if (clampFunction.Category != Function.Enums.Category.PLCTerminal)
            {
                const string Message = "Выбранная функция не является " +
                    "клеммой модуля ввода-вывода";
                throw new Exception(Message);
            }

            return clampFunction;
        }

        /// <summary>
        /// Получить функцию выбранной клеммы
        /// </summary>
        /// <param name="IOModuleFunction">Функция модуля ввода-вывода</param>
        /// <param name="deviceName">Привязанное к клемме устройство</param>
        /// <returns></returns>
        public static Function GetClampFunction(
            Function IOModuleFunction, string deviceName)
        {
            var clampFunction = new Function();

            deviceName = Regex.Match(deviceName, 
                Device.DeviceManager.valveTerminalPattern).Value;
            Function[] subFunctions = IOModuleFunction.SubFunctions;
            if (subFunctions != null)
            {
                foreach (Function subFunction in subFunctions)
                {
                    var functionalText = subFunction.Properties
                        .FUNC_TEXT_AUTOMATIC
                        .ToString(ISOCode.Language.L___);
                    if (functionalText.Contains(deviceName))
                    {
                        clampFunction = subFunction;
                        return clampFunction;
                    }
                }
            }

            if (clampFunction.Category != Function.Enums.Category.PLCTerminal)
            {
                const string Message = "Выбранная функция не является " +
                    "клеммой модуля ввода-вывода";
                throw new Exception(Message);
            }

            return clampFunction;
        }

        /// <summary>
        /// Получить функцию модуля ввода-вывода.
        /// Модуль, куда привязывается устройство.
        /// </summary>
        /// <param name="clampFunction">Функция клеммы модуля 
        /// ввода-вывода</param>
        public static Function GetIOModuleFunction(Function clampFunction)
        {
            var isValveTerminalClampFunction = false;
            Function IOModuleFunction = null;
            if (clampFunction.Name
                .Contains(Device.DeviceManager.ValveTerminalName))
            {
                IOModuleFunction = GetValveTerminalIOModuleFunction(
                    clampFunction);
                if (IOModuleFunction != null)
                {
                    isValveTerminalClampFunction = true;
                }
            }

            if (isValveTerminalClampFunction == false)
            {
                IOModuleFunction = clampFunction.ParentFunction;
            }

            if (IOModuleFunction == null)
            {
                MessageBox.Show(
                    "Данная клемма названа некорректно. Измените" +
                    " ее название (пример корректного названия " +
                    "\"===DSS1+CAB4-A409\"), затем повторите " +
                    "попытку привязки устройства.",
                    "EPlaner",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            return IOModuleFunction;
        }

        /// <summary>
        /// Поиск функции модуля ввода-вывода к которой привязан пневмоостров
        /// </summary>
        /// <param name="clampFunction">Функция клеммы модуля 
        /// ввода-вывода</param>   
        /// <returns>Функция модуля ввода-вывода</returns>
        private static Function GetValveTerminalIOModuleFunction(
            Function clampFunction)
        {
            var IOModuleFunction = new Function();

            string valveTerminalName = Regex.Match(clampFunction.Name, 
                Device.DeviceManager.valveTerminalPattern).Value;
            if (string.IsNullOrEmpty(valveTerminalName))
            {
                const string Message = "Ошибка поиска ОУ пневмоострова";
                throw new Exception(Message);
            }

            var objectFinder = new DMObjectsFinder(ApiHelper.GetProject());
            var functionsFilter = new FunctionsFilter();
            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;
            functionsFilter.SetFilteredPropertyList(properties);
            functionsFilter.Category = Function.Enums.Category.PLCBox;
            Function[] functions = objectFinder.GetFunctions(functionsFilter);

            foreach (Function function in functions)
            {
                Function[] subFunctions = function.SubFunctions;
                if (subFunctions != null)
                {
                    foreach (Function subFunction in subFunctions)
                    {
                        var functionalText = subFunction.Properties
                            .FUNC_TEXT_AUTOMATIC
                            .ToString(ISOCode.Language.L___);
                        if (functionalText.Contains(valveTerminalName))
                        {
                            IOModuleFunction = subFunction.ParentFunction;
                            return IOModuleFunction;
                        }
                    }
                }
            }
            return IOModuleFunction;
        }

        /// <summary>
        /// Получить функциональный текст из функции
        /// </summary>
        /// <param name="function">Функция</param>
        /// <returns></returns>
        public static string GetFunctionalText(Function function)
        {
            string functionalText = function.Properties.FUNC_TEXT_AUTOMATIC
                .ToString(ISOCode.Language.L___);
            
            if (string.IsNullOrEmpty(functionalText))
            {
                functionalText = function.Properties.FUNC_TEXT_AUTOMATIC
                    .ToString(ISOCode.Language.L_ru_RU);
            }
            if (string.IsNullOrEmpty(functionalText))
            {
                functionalText = function.Properties.FUNC_TEXT
                    .ToString(ISOCode.Language.L_ru_RU);
            }
            if(string.IsNullOrEmpty(functionalText))
            {
                return "";
            }

            return functionalText;
        }

        /// <summary>
        /// Получить свойство проекта.
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns></returns>
        public static string GetProjectProperty(string propertyName)
        {
            string result = "";
            var project = GetProject();
            if (project.Properties[propertyName].IsEmpty)
            {
                string errMsg = "Не задан диапазон IP-адресов проекта.\n";
                throw new Exception(errMsg);
            }

            result = Regex.Match(project.Properties[propertyName]
                .ToString(ISOCode.Language.L___), CommonConst.IPAddressPattern)
                .Value;
            if (result == "")
            {
                string errMsg = "Некорректно задан диапазон IP-адресов " +
                    "проекта.\n";
                throw new Exception(errMsg);
            }

            return result;
        }

        /// <summary>
        /// Возвращает имя канала (IO-Link, DI, DO) из строки для IO-Link
        /// модуля.
        /// </summary>
        /// <param name="source">Строка для поиска</param>
        /// <returns></returns>
        public static string GetChannelNameForIOLinkModuleFromString(
            string source)
        {
            const string IOLink = "IO-Link";
            const string DI = "DI";
            const string DO = "DO";

            if (source.Contains(DI) &&
                !source.Contains(IOLink) &&
                !source.Contains(DO))
            {
                return DI;
            }

            if (source.Contains(DO) &&
                !source.Contains(IOLink) &&
                !source.Contains(DI))
            {
                return DO;
            }

            return "";
        }

        /// <summary>
        /// MatchEvaluator для regular expression,
        /// замена русских букв на английские
        /// </summary>
        public static string RussianToEnglish(Match m)
        {
            switch (m.ToString()[0])
            {
                case 'А':
                    return "A";
                case 'В':
                    return "B";
                case 'С':
                    return "C";
                case 'Е':
                    return "E";
                case 'К':
                    return "K";
                case 'М':
                    return "M";
                case 'Н':
                    return "H";
                case 'Х':
                    return "X";
                case 'Р':
                    return "P";
                case 'О':
                    return "O";
                case 'Т':
                    return "T";
            }

            return m.ToString();
        }
    }

    /// <summary>
    /// Класс содержащий общие константы  для проекта.
    /// </summary>
    public static class CommonConst
    {
        /// <summary>
        /// Evaluator для замены заглавных русских букв на английские.
        /// </summary>
        public static MatchEvaluator RusAsEnsEvaluator = new MatchEvaluator(
            ApiHelper.RussianToEnglish);

        /// <summary>
        /// Шаблон для поиска русских букв.
        /// </summary>
        public const string RusAsEngPattern = @"[АВСЕКМНХРОТ]";

        /// <summary>
        /// Символ переноса строки.
        /// </summary>
        public const string NewLine = "\n";

        /// <summary>
        /// Символ переноса строки с возвратом каретки.
        /// </summary>
        public const string NewLineWithCarriageReturn = "\r\n";

        /// <summary>
        /// Текст "Резерв".
        /// </summary>
        public const string Reserve = "Резерв";

        /// <summary>
        /// Шаблон поиска IP-адреса.
        /// </summary>
        public const string IPAddressPattern = @"\b(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
    }

    public static class LuaHelper
    {
        /// <summary>
        /// Конвертация Lua-таблицы в массив свойств
        /// </summary>
        /// <param name="table">Lua таблица</param>
        /// <returns></returns>
        public static Editor.ObjectProperty[] ConvertLuaTableToCArray(
            LuaTable table)
        {
            var Keys = new string[table.Values.Count];
            var Values = new string[table.Values.Count];
            var res = new Editor.ObjectProperty[Keys.Length];

            table.Values.CopyTo(Values, 0);
            table.Keys.CopyTo(Keys, 0);

            for (int i = 0; i < Keys.Length; i++)
            {
                res[i] = new Editor.ObjectProperty(Keys[i], Values[i]);
            }

            return res;
        }

        /// <summary>
        /// Конвертация Lua-таблицы в строку с разделителем - пробел.
        /// </summary>
        /// <param name="table">Lua таблица</param>
        /// <returns></returns>
        public static string ConvertLuaTableToString(object table)
        {
            var luaTable = table as LuaTable;
            var values = new object[luaTable.Values.Count];
            luaTable.Values.CopyTo(values, 0);

            var result = string.Empty;
            foreach (object obj in values)
            {
                result += obj.ToString() + " ";
            }
            result.Trim();

            return result;
        }
    }
}
