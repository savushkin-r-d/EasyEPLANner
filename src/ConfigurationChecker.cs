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
            errors = deviceManager.Check();
            errors += IOManager.Check();
            errors += techObjectManager.Check();
            errors += CheckProjectIPAddresses();
        }

        /// <summary>
        /// Проверка IP-адресов проекта.
        /// </summary>
        /// <returns></returns>
        private string CheckProjectIPAddresses()
        {
            string errors = "";
            string startIPstr = "";
            string endIPstr = "";
            string startIpProperty = "EPLAN.Project.UserSupplementaryField1";
            string endIpProperty = "EPLAN.Project.UserSupplementaryField2";
            string ipProperty = "IP";

            try
            {
                startIPstr = ApiHelper.GetProjectProperty(startIpProperty);
                endIPstr = ApiHelper.GetProjectProperty(endIpProperty);
            }
            catch (Exception e)
            {
                errors += e.Message;
                return errors;
            }

            int[] startIP = startIPstr.Split('.').Select(int.Parse).ToArray();
            int[] endIP = endIPstr.Split('.').Select(int.Parse).ToArray();

            var deivcesWithIP = deviceManager.Devices
                .Where(x => x.Properties.ContainsKey(ipProperty)).ToArray();
            foreach (var device in deivcesWithIP)
            {
                string IPstr = Regex.Match(device.Properties[ipProperty]
                    .ToString(), CommonConst.IPAddressPattern).Value;
                if (IPstr == "")
                {
                    continue;
                }

                int[] devIPPairs = IPstr.Split('.').Select(int.Parse).ToArray();
                for (int IPPair = 0; IPPair < devIPPairs.Length; IPPair++)
                {
                    if (devIPPairs[IPPair] > endIP[IPPair] ||
                        devIPPairs[IPPair] < startIP[IPPair])
                    {
                        errors += $"IP-адрес устройства {device.EPlanName} " +
                            $"вышел за диапазон\n";
                    }
                }
            }

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
