using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticHelper;
using System.Text.RegularExpressions;

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
        /// Прочитать устройства
        /// </summary>
        public void Read()
        {
            //TODO: Временно, для тестов
            EplanDeviceManager.GetInstance().ReadConfigurationFromScheme();
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
        /// MatchEvaluator для regular expression,
        /// замена русских букв на английские
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string RussianToEnglish(Match m)
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

        const string RusAsEngPattern = @"[АВСЕКМНХРОТ]";
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

        Device.DeviceManager deviceManager;
    }
}
