using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using StaticHelper;
using System.Text.RegularExpressions;
using EplanDevice;

namespace EasyEPlanner
{
    public interface IDeviceReader
    {
        /// <summary>
        /// Считанные устройства
        /// </summary>
        List<IODevice> Devices { get; }

        /// <summary>
        /// Количество считанных устройств
        /// </summary>
        int DevicesCount { get; }

        /// <summary>
        /// Копировать устройства в массив
        /// </summary>
        /// <param name="array">Массив для копируемых данных</param>
        void CopyDevices(IODevice[] array);

        /// <summary>
        /// Прочитать устройства
        /// </summary>
        void Read();
    }

    /// <summary>
    /// Считыватель устройств
    /// </summary>
    public class DeviceReader : IDeviceReader
    {
        IApiHelper apiHelper;
        IProjectHelper projectHelper;
        IDeviceHelper deviceHelper;
        IIOHelper ioHelper;

        public DeviceReader(IApiHelper apiHelper, IDeviceHelper deviceHelper,
            IProjectHelper projectHelper, IIOHelper ioHelper)
        {
            this.deviceManager = DeviceManager.GetInstance();
            this.apiHelper = apiHelper;
            this.deviceHelper = deviceHelper;
            this.projectHelper = projectHelper;
            this.ioHelper = ioHelper;
        }

        /// <summary>
        /// Получить привязку для сброса канала устройства
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetBindingForResettingChannel(
            Function deviceClampFunction, IO.IOModuleInfo moduleInfo,
            string devicesDescription = "")
        {
            bool correctClampNumber = IsCorrectClampNumber(
                deviceClampFunction, moduleInfo);
            bool correctPageType = IsCorrectPageType(deviceClampFunction);
            bool correctDeviceDescription = IsCorrectDeviceDescription(
                deviceClampFunction, devicesDescription);

            var res = new Dictionary<string, string>();
            if (!correctClampNumber ||
                !correctDeviceDescription ||
                !correctPageType)
            {
                return res;
            }

            var comment = string.Empty;
            Match actionMatch;
            bool isMultipleBinding = deviceManager.IsMultipleBinding(
                devicesDescription);
            if (isMultipleBinding == false)
            {
                int endPos = devicesDescription.IndexOf(CommonConst.NewLine);
                if (endPos > 0)
                {
                    comment = devicesDescription.Substring(endPos + 1);
                    devicesDescription = devicesDescription
                        .Substring(0, endPos);
                }

                devicesDescription = DeviceBindingHelper
                    .ReplaceRusBigLettersByEngBig(devicesDescription);
                actionMatch = DeviceBindingHelper
                    .FindCorrectClampCommentMatch(comment);
            }
            else
            {
                devicesDescription = DeviceBindingHelper
                    .ReplaceRusBigLettersByEngBig(devicesDescription);
                actionMatch = DeviceBindingHelper
                    .FindCorrectClampCommentMatch(comment);
            }

            var clampComment = string.Empty;
            var descrMatch = Regex.Match(devicesDescription,
                DeviceManager.BINDING_DEVICES_DESCRIPTION_PATTERN);
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

        private bool IsCorrectClampNumber(Function deviceClampFunction,
            IO.IOModuleInfo moduleInfo)
        {
            string clampNumberAsString = DeviceBindingHelper
                .GetClampNumberAsString(deviceClampFunction, ioHelper);
            bool isDigit = int.TryParse(clampNumberAsString,
                out int clampNumber);
            if (isDigit == false)
            {
                return false;
            }

            bool clampNumberNotExistsInModule =
                Array.IndexOf(moduleInfo.ChannelClamps, clampNumber) < 0;
            if (clampNumberNotExistsInModule)
            {
                return false;
            }

            return true;
        }

        private bool IsCorrectPageType(Function deviceClampFunction)
        {
            DocumentTypeManager.DocumentType pageType = deviceClampFunction
                .Page.PageType;
            bool notAllowedPageTypes =
                pageType != DocumentTypeManager.DocumentType.Circuit &&
                pageType != DocumentTypeManager.DocumentType.Overview;
            if (notAllowedPageTypes)
            {
                return false;
            }

            return true;
        }

        private bool IsCorrectDeviceDescription(Function deviceClampFunction,
            string devicesDescription)
        {
            if (devicesDescription == string.Empty)
            {
                devicesDescription = apiHelper.GetFunctionalText(
                    deviceClampFunction);
                if (devicesDescription == string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

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
            deviceManager.Clear();
            var objectFinder = new DMObjectsFinder(projectHelper.GetProject());

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
            foreach (var function in deviceFunctions)
            {
                bool skip = NeedToSkip(function);
                if (skip == true)
                {
                    continue;
                }

                string name = deviceHelper.GetName(function);
                string description = deviceHelper.GetDescription(function);
                string subType = deviceHelper.GetSubType(function);
                string parameters = deviceHelper.GetParameters(function);
                string properties = deviceHelper.GetProperties(function);
                string runtimeParameters = deviceHelper.GetRuntimeParameters(function);
                int deviceLocation = deviceHelper.GetLocation(function);
                string articleName = deviceHelper.GetArticleName(function);

                string error;
                deviceManager.AddDeviceAndEFunction(name, description,
                    subType, parameters, runtimeParameters, properties,
                    deviceLocation, function, out error, articleName);

                if (error != "")
                {
                    Logs.AddMessage(error);
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

            if (function.VisibleName == "" ||
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

        public void CopyDevices(IODevice[] array)
        {
            deviceManager.Devices.CopyTo(array);
        }

        public int DevicesCount
        {
            get
            {
                return deviceManager.Devices.Count;
            }
        }

        public List<IODevice> Devices
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
        /// Функции для поиска устройств.
        /// </summary>
        Function[] deviceFunctions;

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        DeviceManager deviceManager;
    }
}
