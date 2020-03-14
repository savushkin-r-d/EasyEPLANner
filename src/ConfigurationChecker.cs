using System.Linq;
using System.Text.RegularExpressions;
using System;
using StaticHelper;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, проверяющий текущую конфигурацию проекта.
    /// </summary>
    public class ConfigurationChecker
    {
        public ConfigurationChecker()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
            this.IOManager = IO.IOManager.GetInstance();
            this.techObjectManager = TechObject.TechObjectManager.GetInstance();
        }

        public void Check() 
        {
            errors = "";
            errors += CheckProjectIPAddresses();
            errors += deviceManager.Check();
            errors += IOManager.Check();
            errors += techObjectManager.Check();
        }

        /// <summary>
        /// Проверка IP-адресов проекта.
        /// </summary>
        /// <returns>Ошибки</returns>
        private string CheckProjectIPAddresses()
        {
            string errors = "";
            string startIPstr, endIPstr;
            string startIpProperty = "EPLAN.Project.UserSupplementaryField1";
            string endIpProperty = "EPLAN.Project.UserSupplementaryField2";

            try
            {
                startIPstr = ApiHelper.GetProjectProperty(startIpProperty);
                endIPstr = ApiHelper.GetProjectProperty(endIpProperty);
            }
            catch (Exception e)
            {
                ProjectConfiguration.GetInstance().ResetIPAddressesInterval();
                errors += e.Message;
                return errors;
            }

            long startIP = IPConverter.ConvertIPStrToLong(startIPstr);
            long endIP = IPConverter.ConvertIPStrToLong(endIPstr);
            if (endIP - startIP <= 0)
            {
                ProjectConfiguration.GetInstance().ResetIPAddressesInterval();
                errors += "Некорректно задан диапазон IP-адресов проекта.\n";
                return errors;
            }

            ProjectConfiguration.GetInstance().StartingIPInterval = startIP;
            ProjectConfiguration.GetInstance().EndingIPInterval = endIP;
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

        Device.DeviceManager deviceManager;
        IO.IOManager IOManager;
        TechObject.TechObjectManager techObjectManager;
    }
}
