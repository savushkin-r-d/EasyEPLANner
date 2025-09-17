using Eplan.EplApi.DataModel;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EplanDevice;
using System.Text;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using Aga.Controls.Tree;
using IO;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель привязки устройств.
    /// </summary>
    public class DeviceBindingReader
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
            var objectsFinder = new DMObjectsFinder((projectHelper as ProjectHelper).GetProject());

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
        [ExcludeFromCodeCoverage]
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

                    CurrentReadingModuleClampsDescription.Clear();

                    foreach (var function in module.Function.SubFunctions)
                    {
                        ReadModuleClampBinding(node, module, function);

                        // Добавляем функцию клеммы в модуль
                        if (module.Info.ChannelClamps.Contains(function.ClampNumber) && function.PlacedOnCircuit)
                        {
                            module.AddClampFunction(function);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Чтение привязки.
        /// </summary>
        /// <param name="clampFunction">Функция клеммы</param>
        [ExcludeFromCodeCoverage]
        public void ReadModuleClampBinding(IEplanFunction clampFunction)
        {
            if (Regex.Match(clampFunction.Name, @"=*-E?Y(?<n>\d+)", RegexOptions.None, TimeSpan.FromMilliseconds(100))
                .Success)
            {
                // чтение привязки к пневмоострову 
                var valveTerminal = DeviceManager.GetInstance()
                    .GetDevice(clampFunction.Name.Split(':')[0]);

                if (valveTerminal.DeviceType is not DeviceType.Y)
                    return;

                var channel = valveTerminal.Channels[0];

                var node = IOManager.IONodes[channel.Node];
                var module = IOManager.GetModuleByPhysicalNumber(channel.FullModule);
                var clamp = module.ClampFunctions[channel.PhysicalClamp];

                PrepareForReading();
                ReadModuleClampBinding(node, module, clamp);
                return;
            }

            foreach (var node in IOManager.IONodes)
            {
                foreach (var module in node.IOModules)
                {
                    if (module.ClampFunctions.ContainsValue(clampFunction))
                    {
                        module.ClearBind(clampFunction.ClampNumber);
                        ReadModuleClampBinding(node, module, clampFunction);
                        return;
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
        public void ReadModuleClampBinding(IO.IIONode node, IO.IIOModule module,
            IEplanFunction clampFunction)
        {
            string description = clampFunction.FunctionalText;
            var descriptionMatches = Regex.Matches(description,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

            bool skip = NeedToSkip(module, clampFunction, description, descriptionMatches);
            if (skip == true)
            {
                return;
            }

            List<string> comments = null;
            List<string> actions = null;

            const string ValveTerminalNamePattern = @"=*-E?Y(?<n>\d+)";
            var valveTerminalRegex = new Regex(ValveTerminalNamePattern);
            var valveTerminalMatch = valveTerminalRegex.Match(description);
            if (valveTerminalMatch.Success && descriptionMatches.Count == 1)
            {
                var terminal = description.Split(["\n", "\r\n"], StringSplitOptions.None)[0];
                var subFunctions = functionsForSearching.FirstOrDefault(f => f.Name.Contains(terminal))?.SubFunctions ?? [];

                description += ReadValveTerminalClampsBinding(subFunctions, terminal);

                CorrectDataMultipleBindingToValveTerminal(ref description,
                    out actions, out comments);
            }
            else
            {
                CorrectDataIfMultipleBinding(ref description, out actions,
                    out comments);
            }

            SetBind(description, actions, module, node, clampFunction,
                comments, valveTerminalMatch.Success);
        }

        /// <summary>
        /// Необходимость в пропуске данной клеммы.
        /// </summary>
        /// <param name="module">Модуль</param>
        /// <param name="clampFunction">Функция клеммы</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private bool NeedToSkip(IO.IIOModule module, IEplanFunction clampFunction, string description , MatchCollection descriptionMatches)
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

            var clamp = clampFunction.ClampNumber;
            if (clamp == -1)
            {
                if (device != null)
                    Logs.AddMessage(error);
                skip = true;
                return skip;
            }

            IO.IIOModuleInfo moduleInfo = module.Info;
            if (Array.IndexOf(moduleInfo.ChannelClamps, clamp) < 0)
            {
                if (device != null)
                    Logs.AddMessage(error);
                skip = true;
                return skip;
            }

            if (!clampFunction.PlacedOnCircuit)
            {
                skip = true;
                return skip;
            }

            if (description == "" || description.Contains(CommonConst.Reserve))
            {
                skip = true;
                return skip;
            }

            int? alternateClampBinded = AlternateClampBinded(moduleInfo, clamp, CurrentReadingModuleClampsDescription);

            if (alternateClampBinded.HasValue)
            {
                Logs.AddMessage($"'-{module.Name}:{clamp:D2}': нельзя одновременно привязывать устройства к клеммам: {alternateClampBinded:D2} и {clamp:D2}.\n");
                return true; //skip
            }

            CurrentReadingModuleClampsDescription[clamp] = description;
            return skip;
        }

        /// <summary>
        /// Проверка одновременной привязки на альтернативных клемах
        /// </summary>
        /// <returns>
        /// null - альтернативные клеммы не привязаны
        /// n - номер привязанной альернативной клеммы
        /// </returns>
        private int? AlternateClampBinded(IIOModuleInfo moduleInfo, int clamp, Dictionary<int, string> currentBinding)
        {
            return moduleInfo.AlternateChannelsClamps
                .Find(altClamps => altClamps.Contains(clamp) && altClamps.Count > 1)?
                .Where(altClamp => altClamp != clamp && currentBinding.TryGetValue(altClamp, out var value) && value != string.Empty)
                .Select(altClamp => (int?)altClamp)
                .FirstOrDefault()
                ?? null;
        }

        /// <summary>
        /// Описание привязки текущего читаемого модуля
        /// </summary>
        private readonly Dictionary<int, string> CurrentReadingModuleClampsDescription = new Dictionary<int, string>();

        /// <summary>
        /// Чтение привязки клемм пневмоострова.
        /// </summary>
        /// <param name="subFunctions">Функции клемм пневмоострова</param>
        /// <returns></returns>
        private string ReadValveTerminalClampsBinding(Function[] subFunctions, string terminal)
        {
            var descriptionBuilder = new StringBuilder();
            foreach (var function in subFunctions.Where(f => f.Category is Function.Enums.Category.PLCTerminal))
            {
                string clampNumber = function.Properties.
                FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
                string clampBindedDevice = apiHelper.GetFunctionalText(
                    function);

                bool isInt = int.TryParse(clampNumber, out _);
                if (isInt != true ||
                    clampBindedDevice.Length == 0 ||
                    clampBindedDevice == CommonConst.Reserve)
                {
                    continue;
                }

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
                    terminal = DeviceManager.GetInstance().GetDevice(terminal)?.Name;

                    var device = DeviceManager.GetInstance().GetDevice(deviceName);
                    (device as ISetupTerminal)?.SetupTerminal(terminal, action, vtugNumber);

                    deviceMatch = deviceMatch.NextMatch();
                }
            }

            return descriptionBuilder.ToString().TrimEnd('\t');
        }

        /// <summary>
        /// Коректировка множественной привязки для пневмоострова
        /// </summary>
        /// <param name="description">Описание в устройстве</param>
        /// <param name="actions">Список деййствий канала</param>
        /// <param name="comments">Комментарии к каналам</param>
        private void CorrectDataMultipleBindingToValveTerminal(
            ref string description,
            out List<string> actions,
            out List<string> comments)
        {
            actions = new List<string>();
            comments = new List<string>();
            if (DeviceManager.GetInstance()
                .IsMultipleBinding(description))
            {
                string comment = string.Empty;

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
                description = DeviceBindingHelper
                    .ReplaceRusBigLettersByEngBig(description);
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
            IO.IIOModule module, IO.IIONode node, IEplanFunction clampFunction,
            List<string> comments, bool bindToValveTerminal)
        {
            var clamp = clampFunction.ClampNumber;

            var descriptionMatches = Regex.Matches(description,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            int devicesCount = descriptionMatches.Count;
            if (devicesCount < 1 && !description.Equals(CommonConst.Reserve))
            {
                Logs.AddMessage(
                    $"\"{module.Function.VisibleName}:{clamp:00}\"" +
                    $" - неверное имя привязанного устройства - " +
                    $"\"{description}\".");
            }

            int devIndex = 0;
            var devices = descriptionMatches.Cast<Match>().Select(match => deviceManager.GetDevice(match.Groups["name"].Value));
            foreach (var device in devices)
            {
                var clampComment = GetClampComment(actions?.ElementAtOrDefault(devIndex) ?? string.Empty);

                string error;
                string channelName = "IO-Link";
                int logicalPort = Array
                    .IndexOf(module.Info.ChannelClamps, clamp) + 1;
                int moduleOffset = module.InOffset;

                if (devicesCount == 1 &&
                    module.Info.AddressSpaceType ==
                    IO.IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    channelName = GetChannelNameForIOLinkModule(device, comments?.ElementAtOrDefault(devIndex) ?? string.Empty);
                }

                DeviceManager.GetInstance().AddDeviceChannel(device,
                        module.Info.AddressSpaceType, node.N - 1,
                        module.PhysicalNumber % 100, clamp, clampComment,
                        out error, module.PhysicalNumber, logicalPort,
                        moduleOffset, channelName);

                CheckAndSetExtraOffset(module, device, logicalPort);

                if (error != "")
                {
                    error = string.Format("\"{0}:{1:00}\" : {2}",
                        module.Function.VisibleName, clamp, error);
                    Logs.AddMessage(error);
                }

                var bindingError = CheckValveBinding(device, module, bindToValveTerminal, devices.FirstOrDefault(), clampComment);
                if (bindingError != string.Empty)
                {
                    Logs.AddMessage(bindingError);
                }

                devIndex++;
            }

            var devicesGroupingByAS = devices
                .GroupBy(dev =>
                    {
                        dev.RuntimeParameters.TryGetValue(IODevice.RuntimeParameter.R_AS_NUMBER,
                            out var r_as_number_str);
                        if (r_as_number_str != null && int.TryParse(r_as_number_str.ToString(), out var r_as_number))
                            return r_as_number;
                        return -1;
                    })
                .Where(r_as_dev => r_as_dev.Key != -1);

            foreach (var group in devicesGroupingByAS.Where(r_as_dev => r_as_dev.Count() > 1))
            {
                Logs.AddMessage($"К модулю {module.Name} узла {node.Name} привязано несколько устройств " +
                    $"с одинаковым AS ({group.Key}): {string.Join(", ", group.Select(dev => dev.EplanName))}");
            }
        }


        /// <summary>
        /// Проверка и установка доп.смещения привязки клапанов для IO-Link мастера
        /// </summary>
        /// <param name="module">Модуль</param>
        /// <param name="device">Устройство</param>
        /// <param name="logicalPort">Логический номер клеммы (сигнальной)</param>
        public static void CheckAndSetExtraOffset(IIOModule module, IODevice device, int logicalPort)
        {
            if (module.Info.Number != 657 || // 750-657 - IO-Link Master
                !device.RuntimeParameters.ContainsKey(IODevice.RuntimeParameter.R_EXTRA_OFFSET))
                return;

            device.RuntimeParameters[IODevice.RuntimeParameter.R_EXTRA_OFFSET] = -(logicalPort - 1);
        }

        /// <summary>
        /// Проверка типов привязки к модулю/пневмоострову
        /// </summary>
        /// <param name="device">Привязываемое устройство</param>
        /// <param name="module">Модуль</param>
        /// <param name="bindToValveTerminal">Привязка к пневмоострову</param>
        /// <param name="valveTerminal">Пневмоостров</param>
        [ExcludeFromCodeCoverage]
        public string CheckValveBinding(IODevice device, IIOModule module, bool bindToValveTerminal, IODevice valveTerminal, string clampComment)
        {
            if (bindToValveTerminal)
                return CheckBindAllowedSubTypesToValveTerminal(device, valveTerminal.EplanName);

            return CheckBindAllowedSubTypesToDOModule(device, module, clampComment);
        }


        /// <summary>
        /// Проверка привязки НЕ IO-link клапанов к пневмоострову
        /// </summary>
        /// <param name="device">Устройство</param>
        /// <param name="ValveTerminalName">Название пневмоострова</param>
        /// <exception cref="InvalidBindingTypeException">Не IO-link клапан привязан к пневмоострову</exception>
        public static string CheckBindAllowedSubTypesToValveTerminal(IODevice device, string ValveTerminalName)
        {
            if (device.DeviceType != DeviceType.V)
                return string.Empty;

            switch (device.DeviceSubType)
            {
                case DeviceSubType.V_DO1:
                case DeviceSubType.V_DO2:
                case DeviceSubType.V_DO1_DI1_FB_OFF:
                case DeviceSubType.V_DO1_DI1_FB_ON:
                case DeviceSubType.V_DO1_DI2:
                case DeviceSubType.V_DO2_DI2:
                    return $"Каналы устройства '{device.EplanName}' с подтипом {device.GetDeviceSubTypeStr(device.DeviceType, device.DeviceSubType)} привязаны к пневмоострову '{ValveTerminalName}';\n";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Проверка привязки IO-link клапанов к DI/DO модулям ввода вывода  
        /// </summary>
        /// <param name="device">Устройство</param>
        /// <param name="module">Модулю</param>
        /// <exception cref="InvalidBindingTypeException">IO-link клапан привязан к модулю DO/DI</exception>
        public static string CheckBindAllowedSubTypesToDOModule(IODevice device, IIOModule module, string clampComment)
        {
            if (device.DeviceType != DeviceType.V)
                return string.Empty;

            switch (module.Info.AddressSpaceType)
            {
                case IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                case IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                case IOModuleInfo.ADDRESS_SPACE_TYPE.DODI:
                    break;

                default:
                    return string.Empty;
            }

            switch (device.DeviceSubType)
            {
                case DeviceSubType.V_IOLINK_DO1_DI2:
                case DeviceSubType.V_IOLINK_VTUG_DO1:
                case DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF:
                case DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON:
                case DeviceSubType.V_IOLINK_VTUG_DO1_DI2:
                case DeviceSubType.V_IOL_TERMINAL_MIXPROOF_DO3:
                case DeviceSubType.V_IOLINK_MIXPROOF:
                    break;

                default:
                    return string.Empty;
            }

            if (device.GetChannels(module.Info.AddressSpaceType, "DO", clampComment).Any())
                return string.Empty;

            return $"Каналы устройства '{device.EplanName}' с подтипом {device.GetDeviceSubTypeStr(device.DeviceType, device.DeviceSubType)} привязаны к модулю DO/DI: '{module.Name}';\n";
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
