using Eplan.EplApi.DataModel;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
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


        private void CorrectDataIfMultipleBinding(ref string description, 
            out Match actionMatch, out string comment)
        {
            actionMatch = Match.Empty;
            comment = string.Empty;
            var deviceEvaluator = new MatchEvaluator(RussianToEnglish);
            bool isMultipleBinding = Device.DeviceManager.GetInstance()
                .IsMultipleBinding(description);
            if (isMultipleBinding == false)
            {
                //Для многострочного описания убираем tab+\r\n
                description = description.Replace("\t\r\n", "");
                description = description.Replace("\t\n", "");

                int endPosition = description.IndexOf("\n");
                if (endPosition > 0)
                {
                    comment = description.Substring(endPosition + 1);
                    description = description.Substring(0, endPosition);
                }

                description = Regex.Replace(description, RusAsEngPattern, 
                    deviceEvaluator);
                actionMatch = Regex.Match(comment, ChannelCommentPattern,
                    RegexOptions.IgnoreCase);

                comment = Regex.Replace(comment, ChannelCommentPattern,
                    "", RegexOptions.IgnoreCase);
                comment = comment.Replace("\n", ". ").Trim();
                if (comment.Length > 0 && comment[comment.Length - 1] != '.')
                {
                    comment += ".";
                }
            }
            else
            {
                description = Regex.Replace(description, RusAsEngPattern, 
                    deviceEvaluator);
                actionMatch = Regex.Match(comment, ChannelCommentPattern,
                    RegexOptions.IgnoreCase);
            }
        }

        private void SetBind(string description, Match actionMatch, 
            IO.IOModule module, IO.IONode node, Function clampFunction, 
            string comment)
        {
            int clamp = Convert.ToInt32(clampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString());
            
            var descriptionMatches = Regex.Matches(description,
                Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            int devicesCount = descriptionMatches.Count;
            if (devicesCount < 1 && !description.Equals("Pезерв"))
            {
                ProjectManager.GetInstance().AddLogMessage(
                    $"\"{clampFunction.VisibleName}:{clamp}\" - неверное " +
                    $"имя привязанного устройства - \"{description}\".");
            }

            foreach (Match descriptionMatch in descriptionMatches)
            {
                string deviceName = descriptionMatch.Groups["name"].Value;
                Device.IODevice device = deviceManager.GetDevice(deviceName);

                var clampComment = string.Empty;
                if (actionMatch.Success)
                {
                    clampComment = actionMatch.Value;
                    if (clampComment.Contains("\r\n"))
                    {
                        clampComment = clampComment.Replace("\r\n", "");
                    }
                }

                var error = string.Empty;
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
                        List<Device.IODevice.IOChannel> chanels = 
                            device.Channels;
                        channelName = ApiHelper
                            .GetChannelNameForIOLinkModuleFromString(
                            chanels.First().Name);
                    }
                    else
                    {
                        channelName = ApiHelper
                            .GetChannelNameForIOLinkModuleFromString(comment);
                    }
                }

                Device.DeviceManager.GetInstance().AddDeviceChannel(device,
                        module.Info.AddressSpaceType, node.N - 1, 
                        module.PhysicalNumber % 100, clamp, clampComment, 
                        out error, module.PhysicalNumber, logicalPort, 
                        moduleOffset, channelName);

                if (error != string.Empty)
                {
                    error = string.Format("\"{0}:{1}\" : {2}",
                        clampFunction.VisibleName, clamp, error);
                    ProjectManager.GetInstance().AddLogMessage(error);
                }
            }
        }

        /// <summary>
        /// MatchEvaluator для regular expression,
        /// замена русских букв на английские
        /// </summary>
        private static string RussianToEnglish(Match m)
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

        /// <summary>
        /// Шаблон для поиска русских букв.
        /// </summary>
        const string RusAsEngPattern = @"[АВСЕКМНХРОТ]";

        /// <summary>
        /// Шаблон для разбора комментария к устройству.
        /// </summary>
        const string ChannelCommentPattern =
            @"(Открыть мини(?n:\s+|$))|" +
            @"(Открыть НС(?n:\s+|$))|" +
            @"(Открыть ВС(?n:\s+|$))|" +
            @"(Открыть(?n:\s+|$))|" +
            @"(Закрыть(?n:\s+|$))|" +
            @"(Открыт(?n:\s+|$))|" +
            @"(Закрыт(?n:\s+|$))|" +
            @"(Объем(?n:\s+|$))|" +
            @"(Поток(?n:\s+|$))|" +
            @"(Пуск(?n:\s+|$))|" +
            @"(Реверс(?n:\s+|$))|" +
            @"(Обратная связь(?n:\s+|$))|" +
            @"(Частота вращения(?n:\s+|$))|" +
            @"(Авария(?n:\s+|$))|" +
            @"(Напряжение моста\(\+Ud\)(?n:\s+|$))|" +
            @"(Референсное напряжение\(\+Uref\)(?n:\s+|$))";

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
