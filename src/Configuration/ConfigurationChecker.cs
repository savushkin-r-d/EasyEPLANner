using System.Linq;
using System.Text.RegularExpressions;
using System;
using StaticHelper;
using Spire.Pdf.Exporting.XPS.Schema;
using EplanDevice;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, проверяющий текущую конфигурацию проекта.
    /// </summary>
    public class ConfigurationChecker
    {
        /// <summary>
        /// Доп.поле 1 из свойств проекта
        /// </summary>
        private const string rangesIP_FieldName = "EPLAN.Project.UserSupplementaryField1";

        /// <summary>
        /// Доп.поле 2 из свойств проекта
        /// </summary>
        private const string endIpProperty = "EPLAN.Project.UserSupplementaryField2";

        string errors;

        IProjectHelper projectHelper;
        EplanDevice.DeviceManager deviceManager;
        IO.IOManager IOManager;
        TechObject.ITechObjectManager techObjectManager;
        ProjectHealthChecker projectHealthChecker;

        public ConfigurationChecker(IProjectHelper projectHelper)
        {
            this.projectHelper = projectHelper;
            deviceManager = EplanDevice.DeviceManager.GetInstance();
            IOManager = IO.IOManager.GetInstance();
            techObjectManager = TechObject.TechObjectManager.GetInstance();
            projectHealthChecker = new ProjectHealthChecker();
        }

        public void Check() 
        {
            errors = "";
            errors += CheckProjectIPAddresses();
            errors += deviceManager.Check();
            errors += IOManager.Check();
            errors += techObjectManager.Check();
            errors += projectHealthChecker.Check();
        }

        /// <summary>
        /// Проверка IP-адресов проекта.
        /// </summary>
        /// <returns>Ошибки</returns>
        private string CheckProjectIPAddresses()
        {
            string errors = "";
            string rangesIP_str;
            (long, long)[] RangesIP;

            string exampleRanges = "Пример: Свойства->Пользователь, доп. поле 1: \"127.0.0.1 - 127.0.0.10, 127.0.0.21 - 127.0.0.30\"\n";
            string errorRangeIsNotSet = "Не задан диапазон IP-адресов проекта.\n";
            string wrongRanges = "Некорректно задан диапазон IP-адресов проекта.\n";

            try
            {
                rangesIP_str = projectHelper.GetProjectProperty(rangesIP_FieldName);

                RangesIP = Regex
                    .Matches(rangesIP_str, CommonConst.RangeOfIPAddresses,
                        RegexOptions.None, TimeSpan.FromMilliseconds(100))
                    .Cast<Match>().Select(match => (IPConverter.ConvertIPStrToLong(match.Groups["ip"].Captures[0].Value),
                    IPConverter.ConvertIPStrToLong(match.Groups["ip"].Captures[1].Value))).ToArray();

                if ( RangesIP.Length == 0 )
                {
                    string errMsg = wrongRanges + exampleRanges;
                    throw new Exception(errMsg);
                }
            }
            catch (Exception e)
            {
                ProjectConfiguration.GetInstance().ResetIPAddressesInterval();
                if (e.Message.Contains(rangesIP_FieldName))
                {
                    errors += errorRangeIsNotSet + exampleRanges;
                }
                else
                {
                    errors += e.Message;
                }
                return errors;
            }

            if (RangesIP.All(range => range.Item2 - range.Item1 <= 0))
            {
                ProjectConfiguration.GetInstance().ResetIPAddressesInterval();
                errors += wrongRanges + exampleRanges;
                return errors;
            }

            ProjectConfiguration.GetInstance().RangesIP = RangesIP;
            return errors;
        }

        public string Errors 
        { 
            get
            {
                return errors;
            } 
        }




    }
}
