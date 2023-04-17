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
            string startIPstr, endIPstr;
            (long, long)[] RangesIP;

            try
            {
                startIPstr = projectHelper.GetProjectProperty(startIpProperty);
                endIPstr = projectHelper.GetProjectProperty(endIpProperty);

                var startIPArray = Regex
                    .Matches(startIPstr, CommonConst.IPAddressPattern,
                        RegexOptions.None, TimeSpan.FromMilliseconds(100))
                    .Cast<Match>().Select(match => match.Value).ToArray();
                var endIPArray = Regex
                    .Matches(endIPstr, CommonConst.IPAddressPattern,
                        RegexOptions.None, TimeSpan.FromMilliseconds(100))
                    .Cast<Match>().Select(match => match.Value).ToArray();

                RangesIP = startIPArray.Zip(endIPArray, (start, end) => 
                    (IPConverter.ConvertIPStrToLong(start),
                    IPConverter.ConvertIPStrToLong(end)))
                    .ToArray();

                if (RangesIP.Length == 0 ||
                    startIPArray.Length != endIPArray.Length)
                {
                    string errMsg = $"Некорректно задан диапазон " +
                        $"IP-адресов проекта.\n";
                    throw new Exception(errMsg);
                }
            }
            catch (Exception e)
            {
                ProjectConfiguration.GetInstance().ResetIPAddressesInterval();
                if (e.Message.Contains(startIpProperty) || 
                    e.Message.Contains(endIpProperty))
                {
                    errors += "Не задан диапазон IP-адресов проекта.\n";
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
                errors += "Некорректно задан диапазон IP-адресов проекта.\n";
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

        string errors;

        EplanDevice.DeviceManager deviceManager;
        IO.IOManager IOManager;
        TechObject.ITechObjectManager techObjectManager;
        ProjectHealthChecker projectHealthChecker;

        /// <summary>
        /// Доп.поле 1 из свойств проекта
        /// </summary>
        private const string startIpProperty = "EPLAN.Project.UserSupplementaryField1";

        /// <summary>
        /// Доп.поле  из свойств проекта
        /// </summary>
        private const string endIpProperty = "EPLAN.Project.UserSupplementaryField2";
    }
}
