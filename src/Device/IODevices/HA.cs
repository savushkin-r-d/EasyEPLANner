using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - аварийная звуковая сигнализация.
    /// </summary>
    public class HA : IODevice
    {
        public HA(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HA;
            ArticleName = articleName;

            DO.Add(new IOChannel("DO", -1, -1, -1, ""));
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == "")
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.HA:
                    return dt.ToString();
            }
            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.HA:
                    return new Dictionary<string, int>()
                    {
                        {"ST", 1},
                        {"M", 1},
                    };
            }
            return null;
        }
    }
}
