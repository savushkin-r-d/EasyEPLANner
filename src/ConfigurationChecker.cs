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
                errors += e.Message;
                return errors;
            }

            long startIP = ConvertIPStrToLong(startIPstr);
            long endIP = ConvertIPStrToLong(endIPstr);
            if (endIP - startIP <= 0)
            {
                errors += "Некорректно задан диапазон IP-адресов проекта.\n";
                return errors;
            }

            errors += CheckDevicesIP(startIP, endIP);
            errors += CheckIONodesIP(startIP, endIP);

            return errors;
        }

        /// <summary>
        /// Проверить IP-адреса устройств.
        /// </summary>
        /// <param name="startIP">Начало диапазона</param>
        /// <param name="endIP">Конец диапазона</param>
        /// <returns>Ошибки</returns>
        private string CheckDevicesIP(long startIP, long endIP)
        {
            string errors = "";
            string ipProperty = "IP";

            var devicesWithIP = deviceManager.Devices
                .Where(x => x.Properties.ContainsKey(ipProperty)).ToArray();
            foreach (var device in devicesWithIP)
            {
                string IPstr = Regex.Match(device.Properties[ipProperty]
                    .ToString(), CommonConst.IPAddressPattern).Value;
                if (IPstr == "")
                {
                    continue;
                }

                long devIP = ConvertIPStrToLong(IPstr);
                if (devIP - startIP < 0 || endIP - devIP < 0)
                {
                    errors += $"IP-адрес устройства {device.EPlanName} " +
                    $"вышел за диапазон.\n";
                }
            }

            return errors;
        }
        
        /// <summary>
        /// Проверка IP-адресов узлов ввода-вывода
        /// </summary>
        /// <param name="startIP">Начало диапазона</param>
        /// <param name="endIP">Конец диапазона</param>
        /// <returns>Ошибки</returns>
        private string CheckIONodesIP(long startIP, long endIP)
        {
            string errors = "";
            var plcWithIP = IOManager.IONodes;
            foreach (var node in plcWithIP)
            {
                string IPstr = node.IP;
                if (IPstr == "")
                {
                    continue;
                }

                long nodeIP = ConvertIPStrToLong(IPstr);

                if (nodeIP - startIP < 0 || endIP - nodeIP < 0)
                {
                    errors += $"IP-адрес узла A{node.FullN} " +
                        $"вышел за диапазон.\n";
                }
            }

            return errors;
        }

        /// <summary>
        /// Конвертировать IP-адрес из строкового типа в long
        /// </summary>
        /// <param name="IP">Строка с адресом</param>
        /// <returns></returns>
        private long ConvertIPStrToLong(string IP)
        {
            long convertedIP;
            const int oneDigit = 1;
            const int twoDigits = 2;

            string[] IPPairs = IP.Split('.');
            for(int i = 0; i < IPPairs.Length; i++)
            {
                if (IPPairs[i].Length == oneDigit)
                {
                    IPPairs[i] = string.Format("00{0}", IPPairs[i]);
                    continue;
                }

                if (IPPairs[i].Length == twoDigits)
                {
                    IPPairs[i] = string.Format("0{0}", IPPairs[i]);
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
