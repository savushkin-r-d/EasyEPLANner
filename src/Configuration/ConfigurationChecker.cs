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
        IProjectHelper projectHelper;

        public ConfigurationChecker(
            IProjectHelper projectHelper,
            IProjectHealthChecker projectHealthChecker,
            IProjectConfiguration projectConfiguration)
        {
            this.projectHelper = projectHelper;
            deviceManager = EplanDevice.DeviceManager.GetInstance();
            IOManager = IO.IOManager.GetInstance();
            techObjectManager = TechObject.TechObjectManager.GetInstance();
            this.projectHealthChecker = projectHealthChecker;
            this.projectConfiguration = projectConfiguration;
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
        public string CheckProjectIPAddresses()
        {
            string errors = "";
            string rangesIP_str;
            (long, long)[] RangesIP;

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
                    string errMsg = WrongIPRanges + ExampleIPRanges;
                    throw new Exception(errMsg);
                }
            }
            catch (Exception e)
            {
                projectConfiguration.ResetIPAddressesInterval();
                if (e.Message.Contains(rangesIP_FieldName))
                {
                    errors += ErrorIPRangeIsNotSet + ExampleIPRanges;
                }
                else
                {
                    errors += e.Message;
                }
                return errors;
            }

            if (RangesIP.All(range => range.Item2 - range.Item1 <= 0))
            {
                projectConfiguration.ResetIPAddressesInterval();
                errors += WrongIPRanges + ExampleIPRanges;
                return errors;
            }

            projectConfiguration.RangesIP = RangesIP;
            return errors;
        }

        public string Errors 
        { 
            get
            {
                return errors;
            } 
        }

        string errors;

        EplanDevice.DeviceManager deviceManager;
        IO.IOManager IOManager;
        TechObject.ITechObjectManager techObjectManager;
        IProjectHealthChecker projectHealthChecker;
        IProjectConfiguration projectConfiguration;

        public const string ExampleIPRanges = "Пример: Свойства->Пользователь, доп. поле 1: \"127.0.0.1 - 127.0.0.10, 127.0.0.21 - 127.0.0.30\"\n";
        public const string ErrorIPRangeIsNotSet = "Не задан диапазон IP-адресов проекта.\n";
        public const string WrongIPRanges = "Некорректно задан диапазон IP-адресов проекта.\n";

        /// <summary>
        /// Доп.поле 1 из свойств проекта
        /// </summary>
        private const string rangesIP_FieldName = "EPLAN.Project.UserSupplementaryField1";

        /// <summary>
        /// Доп.поле 2 из свойств проекта
        /// </summary>
        private const string endIpProperty = "EPLAN.Project.UserSupplementaryField2";
    }
}
