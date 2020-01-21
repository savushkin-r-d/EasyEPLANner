using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using StaticHelper;
using System.Text.RegularExpressions;
using Eplan.EplApi.Base;

namespace EasyEPlanner
{
    /// <summary>
    /// Считыватель устройств
    /// </summary>
    class DeviceReader
    {
        public DeviceReader()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
        }

        /// <summary>
        /// Получить привязку для сброса канала устройства
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetBindingForResettingChannel(
            Function deviceClampFunction, IO.IOModuleInfo moduleInfo,
            string devicesDescription = "")
        {
            const string EmptyString = "";
            var res = new Dictionary<string, string>();
            string clampNumberAsString = deviceClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();

            if (deviceClampFunction.Name.Contains("-Y"))
            {
                Function IOModuleFunction = ApiHelper
                    .GetIOModuleFunction(deviceClampFunction);
                string bindedDevice = deviceClampFunction.Name;
                Function IOModuleClampFunction = ApiHelper
                    .GetClampFunction(IOModuleFunction, bindedDevice);
                clampNumberAsString = IOModuleClampFunction.Properties
                .FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();
            }

            int clampNumber;
            bool isDigit = int.TryParse(clampNumberAsString, out clampNumber);
            if (isDigit == false)
            {
                return res;
            }

            if (Array.IndexOf(moduleInfo.ChannelClamps, clampNumber) < 0)
            {
                return res;
            }

            DocumentTypeManager.DocumentType pageType = deviceClampFunction
                .Page.PageType;
            if (pageType != DocumentTypeManager.DocumentType.Circuit &&
                pageType != DocumentTypeManager.DocumentType.Overview)
            {
                return res;
            }

            if (devicesDescription == EmptyString)
            {
                devicesDescription = ApiHelper.GetFunctionalText(
                    deviceClampFunction);
            }

            if (devicesDescription == EmptyString)
            {
                return res;
            }

            var comment = EmptyString;
            var clampComment = EmptyString;
            Match actionMatch;
            var deviceEvaluator = new MatchEvaluator(RussianToEnglish);
            bool isMultipleBinding = deviceManager.IsMultipleBinding(
                devicesDescription);
            if (isMultipleBinding == false)
            {
                int endPos = devicesDescription.IndexOf("\n");
                if (endPos > 0)
                {
                    comment = devicesDescription.Substring(endPos + 1);
                    devicesDescription = devicesDescription.Substring(0,
                        endPos);
                }

                devicesDescription = Regex.Replace(devicesDescription,
                    RusAsEngPattern, deviceEvaluator);

                actionMatch = Regex.Match(comment, ChannelCommentPattern,
                    RegexOptions.IgnoreCase);

                comment = Regex.Replace(comment, ChannelCommentPattern,
                    EmptyString, RegexOptions.IgnoreCase);
                comment = comment.Replace("\n", ". ").Trim();
                if (comment.Length > 0 && comment[comment.Length - 1] != '.')
                {
                    comment += ".";
                }
            }
            else
            {
                devicesDescription = Regex.Replace(devicesDescription,
                    RusAsEngPattern, deviceEvaluator);
                actionMatch = Regex.Match(comment, ChannelCommentPattern,
                    RegexOptions.IgnoreCase);
            }

            var descrMatch = Regex.Match(devicesDescription,
                Device.DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
            while (descrMatch.Success)
            {
                string devName = descrMatch.Groups["name"].Value;

                if (actionMatch.Success)
                {
                    clampComment = actionMatch.Value;
                }

                res.Add(devName, clampComment);
                descrMatch = descrMatch.NextMatch();
            }

            return res;
        }

        /// <summary>
        /// Прочитать устройства
        /// </summary>
        public void Read()
        {
            PrepareForReading();
            ReadDevices();            
        }

        /// <summary>
        /// Подготовка к чтению устройств
        /// </summary>
        private void PrepareForReading() 
        {
            var objectFinder = new DMObjectsFinder(ApiHelper.GetProject());

            var propertyList = new FunctionPropertyList();
            propertyList.FUNC_MAINFUNCTION = true;

            var functionsFilter = new FunctionsFilter();
            functionsFilter.IsPlaced = true;
            functionsFilter.SetFilteredPropertyList(propertyList);

            deviceFunctions = objectFinder.GetFunctions(functionsFilter);
        }

        /// <summary>
        /// Чтение устройств
        /// </summary>
        private void ReadDevices() 
        {
            foreach (Function function in deviceFunctions)
            {
                bool skip = NeedToSkip(function);
                if (skip == true)
                {
                    continue;
                }

                string name = GetName(function);
                string description = GetDescription(function);
                string subType = GetSubType(function);
                string parameters = GetParameters(function);
                string properties = GetProperties(function);
                string runtimeParameters = GetRuntimeParameters(function);
                int deviceLocation = GetDeviceLocation(function);
                string articleName = GetArticleName(function);

                string error;
                deviceManager.AddDeviceAndEFunction(name, description,
                    subType, parameters, runtimeParameters, properties,
                    deviceLocation, function, out error, articleName);

                if (error != string.Empty)
                {
                    ProjectManager.GetInstance().AddLogMessage(error);
                }
            }

            deviceManager.Sort();
        }

        /// <summary>
        /// Нужно ли пропускать функцию.
        /// </summary>
        /// <param name="function">Функция для проверки</param>
        /// <returns></returns>
        private bool NeedToSkip(Function function)
        {
            var skip = false;

            if (function.VisibleName == string.Empty ||
                    function.Page == null ||
                    function.Page.PageType != DocumentTypeManager.
                    DocumentType.ProcessAndInstrumentationDiagram)
            {
                skip = true;
                return skip;
            }

            //Признак пропуска данного устройства (прим., для ручной заслонки).
            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[1].IsEmpty)
            {
                skip = true;
                return skip;
            }

            return skip;
        }

        /// <summary>
        /// Получить имя устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetName(Function function)
        {
            var name = function.Name;
            name = Regex.Replace(name, RusAsEngPattern, deviceEvaluator);
            return name;
        }

        /// <summary>
        /// Получить описание устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetDescription(Function function)
        {
            var description = string.Empty;
            string descriptionPattern = "([\'\"])";

            if (!function.Properties.FUNC_COMMENT.IsEmpty)
            {
                description = function.Properties.FUNC_COMMENT
                    .ToString(ISOCode.Language.L___);

                if (description == string.Empty)
                {
                    description = function.Properties.FUNC_COMMENT
                        .ToString(ISOCode.Language.L_ru_RU);
                }

                description = Regex.Replace(description,
                    descriptionPattern, string.Empty);
            }

            if (description == null)
            {
                description = string.Empty;
            }

            return description;
        }

        /// <summary>
        /// Получить подтип устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetSubType(Function function)
        {
            var subType = string.Empty;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[2].IsEmpty)
            {
                subType = function.Properties.FUNC_SUPPLEMENTARYFIELD[2]
                    .ToString(ISOCode.Language.L___);

                if (subType == string.Empty)
                {
                    subType = function.Properties.FUNC_SUPPLEMENTARYFIELD[2]
                        .ToString(ISOCode.Language.L_ru_RU);
                }

                subType = subType.Trim();
                subType = Regex.Replace(subType, RusAsEngPattern,
                    deviceEvaluator);
            }

            if (subType == null)
            {
                subType = string.Empty;
            }

            return subType;
        }

        /// <summary>
        /// Получить свойства устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetProperties(Function function)
        {
            var properties = string.Empty;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[4].IsEmpty)
            {
                properties = function.Properties.FUNC_SUPPLEMENTARYFIELD[4]
                    .ToString(ISOCode.Language.L___);

                if (properties == string.Empty)
                {
                    properties = function.Properties.FUNC_SUPPLEMENTARYFIELD[4]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                properties = Regex.Replace(properties, RusAsEngPattern, 
                    deviceEvaluator);
            }

            if (properties == null)
            {
                properties = string.Empty;
            }

            return properties;
        }

        /// <summary>
        /// Получить параметры времени выполнения.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetRuntimeParameters(Function function)
        {
            var runtimeParameters = string.Empty;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[5].IsEmpty)
            {
                runtimeParameters = function.Properties
                    .FUNC_SUPPLEMENTARYFIELD[5]
                    .ToString(ISOCode.Language.L___);

                if (runtimeParameters == string.Empty)
                {
                    runtimeParameters = function.Properties.
                        FUNC_SUPPLEMENTARYFIELD[5]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                if (runtimeParameters == null)
                {
                    runtimeParameters = string.Empty;
                }

                runtimeParameters = Regex.Replace(runtimeParameters,
                    RusAsEngPattern, deviceEvaluator);
            }

            return runtimeParameters;
        }

        /// <summary>
        /// Получить номер шкафа, где располагается устройство.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private int GetDeviceLocation(Function function)
        {
            int deviceLocation = 0;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[6].IsEmpty)
            {
                deviceLocation = int.Parse(function.Properties
                    .FUNC_SUPPLEMENTARYFIELD[6]
                    .ToString(ISOCode.Language.L___));
            }

            return deviceLocation;
        }

        private string GetArticleName(Function function)
        {
            var articleName = string.Empty;

            return articleName;
        }

        /// <summary>
        /// Получить параметры устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        private string GetParameters(Function function)
        {
            var parameters = string.Empty;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[3].IsEmpty)
            {
                parameters = function.Properties.FUNC_SUPPLEMENTARYFIELD[3]
                    .ToString(ISOCode.Language.L___);

                if (parameters == string.Empty)
                {
                    parameters = function.Properties.FUNC_SUPPLEMENTARYFIELD[3]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                parameters = Regex.Replace(parameters, RusAsEngPattern, 
                    deviceEvaluator);
            }

            if (parameters == null)
            {
                parameters = string.Empty;
            }

            return parameters;
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
        /// Копировать устройства в массив
        /// </summary>
        /// <param name="array">Массив для копируемых данных</param>
        public void CopyDevices(Device.IODevice[] array)
        {
            deviceManager.Devices.CopyTo(array);
        }

        /// <summary>
        /// Количество считанных устройств
        /// </summary>
        public int DevicesCount
        {
            get
            {
                return deviceManager.Devices.Count;
            }
        }

        /// <summary>
        /// Считанные устройства
        /// </summary>
        public List<Device.IODevice> Devices
        {
            get
            {
                return deviceManager.Devices;
            }
        }

        /// <summary>
        /// Свойство, указывающее прочитаны устройства или нет.
        /// </summary>
        public bool DevicesIsRead
        {
            get
            {
                if (deviceManager.Devices.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
        /// Evaluator для замены заглавных русских букв на английские.
        /// </summary>
        MatchEvaluator deviceEvaluator = new MatchEvaluator(RussianToEnglish);

        /// <summary>
        /// Функции для поиска устройств.
        /// </summary>
        Function[] deviceFunctions;

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        Device.DeviceManager deviceManager;
    }
}
