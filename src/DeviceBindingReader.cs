using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель привязки устройств.
    /// </summary>
    class DeviceBindingReader
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public DeviceBindingReader()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
            this.IOManager = IO.IOManager.GetInstance();
        }

        /// <summary>
        /// Прочитать привязку.
        /// </summary>
        public void Read()
        {
            PrepareForReading();
            ReadBinding();

            // TODO: Reading binding
            EplanDeviceManager.GetInstance().ReadConfigurationFromIOModules();
        }

        /// <summary>
        /// Подготовка к чтению привязки.
        /// </summary>
        private void PrepareForReading()
        {
            var objectsFinder = new DMObjectsFinder(ApiHelper.GetProject());

            var properties = new FunctionPropertyList();
            properties.FUNC_MAINFUNCTION = true;
            
            var plcFilter = new FunctionsFilter();
            plcFilter.SetFilteredPropertyList(properties);
            plcFilter.Category = Function.Enums.Category.PLCBox;

            functionsForSearching = objectsFinder.GetFunctions(plcFilter);
        }

        /// <summary>
        /// Чтение привязки.
        /// </summary>
        private void ReadBinding()
        {
            foreach (IO.IONode node in IOManager.IONodes)
            {
                foreach (IO.IOModule module in node.IOModules)
                {
                    foreach (Function function in module.Function.SubFunctions)
                    {
                        ReadModuleClampBinding(node, module, function);
                    }
                }
            }
        }

        /// <summary>
        /// Чтение привязки клеммы модуля ввода-вывода.
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="module">Модуль</param>
        /// <param name="clampFunction">Функция клеммы</param>
        private void ReadModuleClampBinding(IO.IONode node, IO.IOModule module,
            Function clampFunction)
        {
            bool skip = NeedToSkip(module, clampFunction);
            if (skip == true)
            {
                return;
            }

            string description = ApiHelper.GetFunctionalText(clampFunction);
            var descriptionMatches = Regex.Matches(description, 
                Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            
            const string ValveTerminalNamePattern = @"=*-Y(?<n>\d+)";
            var valveTerminalRegex = new Regex(ValveTerminalNamePattern);
            var valveTerminalMatch = valveTerminalRegex.Match(description);

            if (valveTerminalMatch.Success && descriptionMatches.Count == 1)
            {
                description = ReadValveTerminalBinding(description);
            }

            //Check multiple binding and AS-i function
            //BindDevice function
        }

        /// <summary>
        /// Необходимость в пропуске данной клеммы.
        /// </summary>
        /// <param name="module">Модуль</param>
        /// <param name="clampFunction">Функция клеммы</param>
        /// <returns></returns>
        private bool NeedToSkip(IO.IOModule module, Function clampFunction)
        {
            var skip = false;

            string clampString = clampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            int clamp;
            bool isDigit = int.TryParse(clampString, out clamp);
            if (isDigit == false)
            {
                skip = true;
                return skip;
            }

            IO.IOModuleInfo moduleInfo = module.Info;
            if (Array.IndexOf(moduleInfo.ChannelClamps, clamp) < 0)
            {
                skip = true;
                return skip;
            }

            DocumentTypeManager.DocumentType documentType = clampFunction.Page
                .PageType;
            if (documentType != DocumentTypeManager.DocumentType.Circuit ||
                documentType != DocumentTypeManager.DocumentType.Overview)
            {
                skip = true;
                return skip;
            }

            string description = ApiHelper.GetFunctionalText(
                clampFunction);
            if (description == string.Empty)
            {
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Чтение привязки пневмоострова.
        /// </summary>
        private string ReadValveTerminalBinding(string description) 
        {
            description = GetValveTerminalNameFromDescription(description);
            Function[] subFunctions = GetClampsByValveTerminalName(description);
            description += ReadValveTerminalClampsBinding(subFunctions);

            return description;
        }

        /// <summary>
        /// Получить ОУ пневмоострова из его описания.
        /// </summary>
        /// <param name="description">Описание</param>
        /// <returns></returns>
        private string GetValveTerminalNameFromDescription(string description)
        {
            const string NewLine = "\n";
            const string NewLineWithCarriage = "\r\n";
            int eplanNameLength;

            if (description.Contains(NewLineWithCarriage) == true)
            {
                eplanNameLength = description.IndexOf(NewLineWithCarriage);
                if (eplanNameLength > 0)
                {
                    description = description.Substring(0, eplanNameLength);
                }
            }
            else
            {
                eplanNameLength = description.IndexOf(NewLine);
                if (eplanNameLength > 0)
                {
                    description = description.Substring(0, eplanNameLength);
                }
            }

            return description;
        }

        /// <summary>
        /// Получить функции клемм пневмоострова по его ОУ.
        /// </summary>
        /// <param name="name">ОУ пневмоострова</param>
        /// <returns></returns>
        private Function[] GetClampsByValveTerminalName(string description)
        {
            var subFunctions = new Function[0];
            foreach (Function valveTerminalFunction in functionsForSearching)
            {
                if (valveTerminalFunction.Name.Contains(description))
                {
                    subFunctions = valveTerminalFunction.SubFunctions;
                    break;
                }
            }

            return subFunctions;
        }

        /// <summary>
        /// Чтение привязки клемм пневмоострова.
        /// </summary>
        /// <param name="subFunctions">Функции клемм пневмоострова</param>
        /// <returns></returns>
        private string ReadValveTerminalClampsBinding(Function[] subFunctions)
        {
            var description = string.Empty;
            const string NewLineWithCarriage = "\r\n";
            const string Reserve = "Резерв";

            foreach (Function function in subFunctions)
            {
                if (function.Category != Function.Enums.Category.PLCTerminal)
                {
                    continue;
                }

                string clampNumber = function.Properties.
                FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                string clampBindedDevice = ApiHelper.GetFunctionalText(
                    function);
                
                bool isInt = int.TryParse(clampNumber, out _);
                if (isInt == true && 
                    clampBindedDevice.Length != 0 && 
                    clampBindedDevice != Reserve)
                {
                    var deviceMatch = Regex.Match(clampBindedDevice, Device
                        .DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
                    while (deviceMatch.Success)
                    {
                        string deviceName = deviceMatch.Groups["name"].Value;
                        description += $"{NewLineWithCarriage}{deviceName}";

                        // Дополнительно установить параметр R_VTUG_NUMBER
                        int vtugNumber = Convert.ToInt32(clampNumber);
                        var runtimeParams = new Dictionary<string, double>();
                        runtimeParams.Add("R_VTUG_NUMBER", vtugNumber);
                        SetDeviceRuntimeParameters(runtimeParams, deviceName);

                        deviceMatch = deviceMatch.NextMatch();
                    }
                }
            }

            return description;
        }

        /// <summary>
        /// Установка рабочих параметров устройства.
        /// </summary>
        /// <param name="parameters">Словарь параметров</param>
        /// <param name="deviceName">Имя устройства</param>
        private void SetDeviceRuntimeParameters(
            Dictionary<string, double> parameters, string deviceName)
        {
            Device.IODevice dev = Device.DeviceManager.GetInstance()
                .GetDevice(deviceName);
            foreach (KeyValuePair<string, double> parameter in parameters)
            {
                dev.SetRuntimeParameter(parameter.Key, parameter.Value);
            }
        }

        /// <summary>
        /// Функции для поиска модулей ввода-вывода
        /// </summary>
        Function[] functionsForSearching;

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        Device.DeviceManager deviceManager;

        /// <summary>
        /// Менеджер узлов и модулей ввода-вывода.
        /// </summary>
        IO.IOManager IOManager;
    }
}
