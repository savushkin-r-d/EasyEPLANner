using Eplan.EplApi.DataModel;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EplanDevice;
using System.Text;
using EasyEPlanner.PxcIolinkConfiguration.Models;

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
                // Конвертируем в IOModule, потому что не тестируется IIOModule
                // из-за Function (еплановская библиотека).
                foreach (IO.IOModule module in node.IOModules)
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
        private void ReadModuleClampBinding(IO.IIONode node, IO.IIOModule module,
            Function clampFunction)
        {
            string description = apiHelper.GetFunctionalText(clampFunction);
            var descriptionMatches = Regex.Matches(description,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            bool skip = NeedToSkip(module, clampFunction, description, descriptionMatches);
            if (skip == true)
            {
                return;
            }

            const string ValveTerminalNamePattern = @"=*-Y(?<n>\d+)";
            var valveTerminalRegex = new Regex(ValveTerminalNamePattern);
            var valveTerminalMatch = valveTerminalRegex.Match(description);
            if (valveTerminalMatch.Success && descriptionMatches.Count == 1)
            {
                description = ReadValveTerminalBinding(description);
            }

            List<string> comments = null;
            List<string> actions = null;

            CorrectDataIfMultipleBinding(ref description, out actions, 
                out comments);

            SetBind(description, actions, module, node, clampFunction, 
                comments);
        }

        /// <summary>
        /// Необходимость в пропуске данной клеммы.
        /// </summary>
        /// <param name="module">Модуль</param>
        /// <param name="clampFunction">Функция клеммы</param>
        /// <returns></returns>
        private bool NeedToSkip(IO.IIOModule module, Function clampFunction, string description , MatchCollection descriptionMatches)
        {
            var skip = false;

            IODevice device = null;
            string error = string.Empty;

            if (description != string.Empty && descriptionMatches.Count > 0)
            {
                    device = DeviceManager.GetInstance()
                        .GetDevice(descriptionMatches[0].ToString());
                    error = $"К не сигнальной клемме \"{clampFunction.Name}\"" +
                        $" привязано устройство \"{device?.EplanName}\".";  
            }

            string clampString = clampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            int clamp;
            bool isDigit = int.TryParse(clampString, out clamp);
            if (isDigit == false)
            {
                if (device != null)
                    Logs.AddMessage(error);
                skip = true;
                return skip;
            }

            IO.IOModuleInfo moduleInfo = module.Info;
            if (Array.IndexOf(moduleInfo.ChannelClamps, clamp) < 0)
            {
                if (device != null)
                    Logs.AddMessage(error);
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
            var descriptionBuilder = new StringBuilder();
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
                        var actionMatch = DeviceBindingHelper.FindCorrectClampCommentMatch(clampBindedDevice);
                        var action = actionMatch.Value;
                        
                        descriptionBuilder.Append(CommonConst.NewLineWithCarriageReturn)
                            .Append(deviceName);
                        if (action != string.Empty)
                        {
                            descriptionBuilder.Append(CommonConst.NewLineWithCarriageReturn)
                                .Append(action);
                        }
                        descriptionBuilder.Append("\t");


                        int vtugNumber = Convert.ToInt32(clampNumber);
                        var device = DeviceManager.GetInstance().GetDevice(deviceName);
                        
                        SetValveRTParameters(device, action, vtugNumber);

                        deviceMatch = deviceMatch.NextMatch();
                    }
                }
            }

            return descriptionBuilder.ToString();
        }

        /// <summary>
        /// Установка номера клемы пневмоострова в рабочие параметры для клапана  
        /// </summary>
        /// <param name="device">Клапан</param>
        /// <param name="action">Действие клеммы</param>
        /// <param name="vtugNumber">Номер клеммы пневмоострова</param>
        private void SetValveRTParameters(IODevice device, string action, int vtugNumber)
        {
            if (!(device != null && device.DeviceType == DeviceType.V))
                return;

            if (device.DeviceSubType == DeviceSubType.V_IOL_TERMINAL_MIXPROOF_DO3)
            {
                string rtParName = string.Empty;


                switch (action)
                {
                    case "Открыть":
                        rtParName = "R_ID_ON";
                        break;
                    case "Открыть ВС":
                        rtParName = "R_ID_UPPER_SEAT";
                        break;
                    case "Открыть НС":
                        rtParName = "R_ID_LOWER_SEAT";
                        break;
                }

                device.SetRuntimeParameter(rtParName, vtugNumber);
            }
            else
            {
                device.SetRuntimeParameter("R_VTUG_NUMBER", vtugNumber);
            }
        }

        /// <summary>
        /// Проверка на множественную привязку и корректировка, если надо.
        /// </summary>
        /// <param name="description">Описание устройства</param>
        /// <param name="actionMatch">Действие канала</param>
        /// <param name="comment">Комментарий к устройству</param>
        private void CorrectDataIfMultipleBinding(ref string description, 
            out List<string> actions, out List<string> comments)
        {
            actions = new List<string>();
            comments = new List<string>();
            
            var comment = string.Empty;

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

                description = DeviceBindingHelper.ReplaceRusBigLettersByEngBig(description);

                var action = DeviceBindingHelper.FindCorrectClampCommentMatch(comment).Value;            
                comment = ReplaceClampCommentInComment(comment, action);

                actions.Add(action);
                comments.Add(comment);
            }
            else
            {
                int endPos = description.IndexOf(CommonConst.NewLine);
                
                if (endPos > 0)
                {
                    comment = description.Substring(endPos + 1);
                    description = description.Substring(0, endPos - 1);
                }

                description = DeviceBindingHelper
                    .ReplaceRusBigLettersByEngBig(description);

                var splitComment = comment.Split('\t');

                actions.Add(string.Empty);
                comments.Add(string.Empty);

                var descriptionBuilder = new StringBuilder(description);
                foreach (var commentLine in splitComment)
                {
                    var action = DeviceBindingHelper.FindCorrectClampCommentMatch(commentLine).Value;
                    
                    actions.Add(action);
                    comments.Add(ReplaceClampCommentInComment(commentLine, action));

                    if (commentLine.Contains(CommonConst.NewLineWithCarriageReturn))
                    {
                        descriptionBuilder.Append(CommonConst.NewLineWithCarriageReturn)
                            .Append(commentLine.Split(new string[] { CommonConst.NewLineWithCarriageReturn }, 
                                StringSplitOptions.RemoveEmptyEntries).First());
                    }
                    else
                    {
                        descriptionBuilder.Append(CommonConst.NewLineWithCarriageReturn)
                            .Append(commentLine);
                    }
                }
                description = descriptionBuilder.ToString();
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
        private void SetBind(string description, List<string> actions, 
            IO.IIOModule module, IO.IIONode node, Function clampFunction, 
            List<string> comments)
        {
            // Конвертируем в IOModule, потому что не тестируется IIOModule
            // из-за Function (еплановская библиотека).
            var ioModule = module as IO.IOModule;
            string clampStr = clampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            int.TryParse(clampStr, out int clamp);
            
            var descriptionMatches = Regex.Matches(description,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            int devicesCount = descriptionMatches.Count;
            if (devicesCount < 1 && !description.Equals(CommonConst.Reserve))
            {
                Logs.AddMessage(
                    $"\"{ioModule.Function.VisibleName}:{clampStr}\"" +
                    $" - неверное имя привязанного устройства - " +
                    $"\"{description}\".");
            }

            int matchIndex = 0;

            foreach (Match descriptionMatch in descriptionMatches)
            {
                string deviceName = descriptionMatch.Groups["name"].Value;
                var device = deviceManager.GetDevice(deviceName);

                var clampComment = GetClampComment(actions?[matchIndex]);

                string error;
                string channelName = "IO-Link";
                int logicalPort = Array
                    .IndexOf(module.Info.ChannelClamps, clamp) + 1;
                int moduleOffset = module.InOffset;

                if (devicesCount == 1 &&
                    module.Info.AddressSpaceType == 
                    IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    channelName = GetChannelNameForIOLinkModule(device, comments?[matchIndex]);
                }

                DeviceManager.GetInstance().AddDeviceChannel(device,
                        module.Info.AddressSpaceType, node.N - 1, 
                        module.PhysicalNumber % 100, clamp, clampComment, 
                        out error, module.PhysicalNumber, logicalPort, 
                        moduleOffset, channelName);

                if (error != "")
                {
                    error = string.Format("\"{0}:{1}\" : {2}",
                        ioModule.Function.VisibleName , clampStr, error);
                    Logs.AddMessage(error);
                }

                matchIndex++;
            }
        }

        private string GetClampComment(string action)
        {
            if (action == null)
                return string.Empty;

            if (action.Contains(
                CommonConst.NewLineWithCarriageReturn))
            {
                action = action.Replace(
                    CommonConst.NewLineWithCarriageReturn, "");
            }

            return action;
        }

        private string GetChannelNameForIOLinkModule(IODevice device, string comment)
        {
            var channelName = string.Empty;

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

            return channelName;
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
