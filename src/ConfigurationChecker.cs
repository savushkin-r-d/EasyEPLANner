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
        /// <returns>Ошибки</returns>
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

            long startIP = ConvertIpStringToIpInt(startIPstr);
            long endIP = ConvertIpStringToIpInt(endIPstr);

            if (endIP - startIP <= 0)
            {
                errors += "Некорректно задан диапазон IP-адресов проекта.\n";
                return errors;
            }

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

                long devIP = ConvertIpStringToIpInt(IPstr);
                if (devIP - startIP < 0 || endIP - devIP < 0)
                {
                    errors += $"IP-адрес устройства {device.EPlanName} " +
                    $"вышел за диапазон.\n";
                }               
            }

            var plcWithIP = IOManager.IONodes;
            foreach (var node in plcWithIP)
            {
                string IPstr = node.IP;
                if (IPstr == "")
                {
                    continue;
                }

                long nodeIP = ConvertIpStringToIpInt(IPstr);

                if (nodeIP - startIP < 0 || endIP - nodeIP < 0)
                {
                    errors += $"IP-адрес узла A{node.FullN} " +
                        $"вышел за диапазон.\n";
                }
            }

            return errors;
        }

        private long ConvertIpStringToIpInt(string IP)
        {
            long convertedIP = 0;
            string[] IPPairs = IP.Split('.');
            for(int i = 0; i < IPPairs.Length; i++)
            {
                if (IPPairs[i].Length == 1)
                {
                    IPPairs[i] = "00" + IPPairs[i];
                    continue;
                }

                if (IPPairs[i].Length == 2)
                {
                    IPPairs[i] = "0" + IPPairs[i];
                    continue;
                }
            }

            string IPstring = string.Concat(IPPairs);
            convertedIP = Convert.ToInt64(IPstring);
            return convertedIP;
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
