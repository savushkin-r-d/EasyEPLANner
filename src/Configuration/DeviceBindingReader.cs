using Eplan.EplApi.DataModel;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EplanDevice;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель привязки устройств.
    /// </summary>
    class DeviceBindingReader
    {
        IProjectHelper projectHelper;
        IApiHelper apiHelper;

        public DeviceBindingReader(IProjectHelper projectHelper, IApiHelper apiHelper)
        {
            this.deviceManager = DeviceManager.GetInstance();
            this.IOManager = IO.IOManager.GetInstance();
            this.projectHelper = projectHelper;
            this.apiHelper = apiHelper;
        }

        /// <summary>
        /// Прочитать привязку.
        /// </summary>
        public void Read()
        {
            PrepareForReading();
            ReadBinding();
        }

        /// <summary>
        /// Подготовка к чтению привязки.
        /// </summary>
        private void PrepareForReading()
        {
            var objectsFinder = new DMObjectsFinder(projectHelper.GetProject());

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
            foreach (var node in IOManager.IONodes)
            {
                foreach (var module in node.IOModules)
                {
                    if (module.Function == null)
                    {
                        continue;
                    }

                    foreach (var function in module.Function.SubFunctions)
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

            string description = apiHelper.GetFunctionalText(clampFunction);
            var descriptionMatches = Regex.Matches(description, 
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            
            const string ValveTerminalNamePattern = @"=*-Y(?<n>\d+)";
            var valveTerminalRegex = new Regex(ValveTerminalNamePattern);
            var valveTerminalMatch = valveTerminalRegex.Match(description);
            if (valveTerminalMatch.Success && descriptionMatches.Count == 1)
            {
                description = ReadValveTerminalBinding(description);
            }

            string comment;
            Match actionMatch;
            CorrectDataIfMultipleBinding(ref description, out actionMatch, 
                out comment);

            SetBind(description, actionMatch, module, node, clampFunction, 
                comment);
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
            if (documentType != DocumentTypeManager.DocumentType.Circuit)
            {
                skip = true;
                return skip;
            }

            string description = apiHelper.GetFunctionalText(
                clampFunction);
            if (description == "" || description.Contains(CommonConst.Reserve))
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
            Function[] subFunctions = GetClampsByValveTerminalName(
                description);
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
            int eplanNameLength;

            if (description.Contains(CommonConst.NewLineWithCarriageReturn) == 
                true)
            {
                eplanNameLength = description.IndexOf(CommonConst
                    .NewLineWithCarriageReturn);
                if (eplanNameLength > 0)
                {
                    description = description.Substring(0, eplanNameLength);
                }
            }
            else
            {
                eplanNameLength = description.IndexOf(CommonConst.NewLine);
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
            foreach (var valveTerminalFunction in functionsForSearching)
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
            var description = "";
            foreach (var function in subFunctions)
            {
                if (function.Category != Function.Enums.Category.PLCTerminal)
                {
                    continue;
                }

                string clampNumber = function.Properties.
                FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                string clampBindedDevice = apiHelper.GetFunctionalText(
                    function);
                
                bool isInt = int.TryParse(clampNumber, out _);
                if (isInt == true && 
                    clampBindedDevice.Length != 0 && 
                    clampBindedDevice != CommonConst.Reserve)
                {
                    var deviceMatch = Regex.Match(clampBindedDevice, 
                        DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
                    while (deviceMatch.Success)
                    {
                        string deviceName = deviceMatch.Groups["name"].Value;
                        description += $"{CommonConst.NewLineWithCarriageReturn}" +
                            $"{deviceName}";

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
            var dev = DeviceManager.GetInstance().GetDevice(deviceName);
            foreach (KeyValuePair<string, double> parameter in parameters)
            {
                dev.SetRuntimeParameter(parameter.Key, parameter.Value);
            }
        }

        /// <summary>
        /// Проверка на множественную привязку и корректировка, если надо.
        /// </summary>
        /// <param name="description">Описание устройства</param>
        /// <param name="actionMatch">Действие канала</param>
        /// <param name="comment">Комментарий к устройству</param>
        private void CorrectDataIfMultipleBinding(ref string description, 
            out Match actionMatch, out string comment)
        {
            comment = string.Empty;
            bool isMultipleBinding = DeviceManager.GetInstance()
                .IsMultipleBinding(description);
            if (isMultipleBinding == false)
            {
                //Для многострочного описания убираем tab+\r\n
                description = description.Replace(
                    $"\t{CommonConst.NewLineWithCarriageReturn}", "");
                description = description.Replace(
                    $"\t{CommonConst.NewLine}", "");

                int endPosition = description.IndexOf(CommonConst.NewLine);
                if (endPosition > 0)
                {
                    comment = description.Substring(endPosition + 1);
                    description = description.Substring(0, endPosition);
                }

                description = DeviceBindingHelper
                    .ReplaceRusBigLettersByEngBig(description);
                actionMatch = DeviceBindingHelper
                    .FindCorrectClampCommentMatch(comment);
                comment = ReplaceClampCommentInComment(comment,
                    actionMatch.Value);
            }
            else
            {
                description = DeviceBindingHelper
                    .ReplaceRusBigLettersByEngBig(description);
                actionMatch = DeviceBindingHelper
                    .FindCorrectClampCommentMatch(comment);
            }
        }

        private string ReplaceClampCommentInComment(string comment,
            string foundClampComment)
        {
            bool invalidComment =
                comment == CommonConst.NewLineWithCarriageReturn ||
                comment == CommonConst.NewLine ||
                comment == string.Empty;
            if (invalidComment)
            {
                return string.Empty;
            }

            string replaced = Regex.Replace(comment,
                Regex.Escape(foundClampComment), string.Empty,
                RegexOptions.IgnoreCase);
            replaced = replaced.Replace(CommonConst.NewLine, ". ").Trim();
            if (replaced.Length > 0 && replaced[replaced.Length - 1] != '.')
            {
                replaced += ".";
            }

            return replaced;
        }

        /// <summary>
        /// Установить привязку устройства.
        /// </summary>
        /// <param name="description">Описание устройства</param>
        /// <param name="actionMatch">Действие канала</param>
        /// <param name="module">Модуль ввода-вывода</param>
        /// <param name="node">Узел ввода-вывода</param>
        /// <param name="clampFunction">Функция клеммы</param>
        /// <param name="comment">Комментарий к устройству</param>
        private void SetBind(string description, Match actionMatch, 
            IO.IOModule module, IO.IONode node, Function clampFunction, 
            string comment)
        {
            string clampStr = clampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            int.TryParse(clampStr, out int clamp);
            
            var descriptionMatches = Regex.Matches(description,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            int devicesCount = descriptionMatches.Count;
            if (devicesCount < 1 && !description.Equals(CommonConst.Reserve))
            {
                Logs.AddMessage(
                    $"\"{module.Function.VisibleName}:{clampStr}\"" +
                    $" - неверное имя привязанного устройства - " +
                    $"\"{description}\".");
            }

            foreach (Match descriptionMatch in descriptionMatches)
            {
                string deviceName = descriptionMatch.Groups["name"].Value;
                var device = deviceManager.GetDevice(deviceName);

                var clampComment = "";
                if (actionMatch.Success)
                {
                    clampComment = actionMatch.Value;
                    if (clampComment.Contains(
                        CommonConst.NewLineWithCarriageReturn))
                    {
                        clampComment = clampComment.Replace(
                            CommonConst.NewLineWithCarriageReturn, "");
                    }
                }

                string error;
                string channelName = "IO-Link";
                int logicalPort = Array
                    .IndexOf(module.Info.ChannelClamps, clamp) + 1;
                int moduleOffset = module.InOffset;

                if (devicesCount == 1 &&
                    module.Info.AddressSpaceType == 
                    IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    if (device.Channels.Count == 1)
                    {
                        List<IODevice.IOChannel> chanels = 
                            device.Channels;
                        channelName = DeviceBindingHelper
                            .GetChannelNameForIOLinkModuleFromString(
                            chanels.First().Name);
                    }
                    else
                    {
                        channelName = DeviceBindingHelper
                            .GetChannelNameForIOLinkModuleFromString(comment);
                    }
                }

                DeviceManager.GetInstance().AddDeviceChannel(device,
                        module.Info.AddressSpaceType, node.N - 1, 
                        module.PhysicalNumber % 100, clamp, clampComment, 
                        out error, module.PhysicalNumber, logicalPort, 
                        moduleOffset, channelName);

                if (error != "")
                {
                    error = string.Format("\"{0}:{1}\" : {2}",
                        module.Function.VisibleName , clampStr, error);
                    Logs.AddMessage(error);
                }
            }
        }

        /// <summary>
        /// Функции для поиска модулей ввода-вывода
        /// </summary>
        Function[] functionsForSearching;

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        DeviceManager deviceManager;

        /// <summary>
        /// Менеджер узлов и модулей ввода-вывода.
        /// </summary>
        IO.IOManager IOManager;
    }
}
