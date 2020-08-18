using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик наличия потока.
    /// Параметры:
    /// 1. P_DT - время порогового фильтра, мсек.    
    /// </summary>
    public class FS : IODevice
    {
        public FS(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.FS;
            ArticleName = articleName;

            DI.Add(new IOChannel("DI", -1, -1, -1, ""));

            parameters.Add("P_DT", null);
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
                case DeviceType.FS:
                    return dt.ToString();
            }
            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.FS:
                    return new Dictionary<string, int>()
                    {
                        {"ST", 1},
                        {"M", 1},
                        {"P_DT", 1},
                    };
            }
            return null;
        }
    }
}
