using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using EplanDevice;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StaticHelper
{
    public static class IOHelper
    {
        /// <summary>
        /// Получить функцию выбранной клеммы
        /// </summary>
        /// <param name="selectedObject">Выбранный на схеме объект
        /// </param>
        /// <returns>Функция клеммы модуля ввода-вывода</returns>
        public static Function GetClampFunction(StorableObject selectedObject)
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

            deviceName = Regex.Match(deviceName, DeviceManager.valveTerminalPattern)
                .Value;
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
            if (clampFunction.Name.Contains(DeviceManager.ValveTerminalName))
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
                DeviceManager.valveTerminalPattern).Value;
            if (string.IsNullOrEmpty(valveTerminalName))
            {
                const string Message = "Ошибка поиска ОУ пневмоострова";
                throw new Exception(Message);
            }

            var objectFinder = new DMObjectsFinder(ProjectHelper.GetProject());
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
    }
}
