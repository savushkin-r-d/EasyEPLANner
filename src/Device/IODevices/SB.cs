using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - кнопка.
    /// </summary>
    public class SB : IODevice
    {
        public SB(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.SB;
            ArticleName = articleName;

            DI.Add(new IOChannel("DI", -1, -1, -1, ""));
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
                case DeviceType.SB:
                    return dt.ToString();
            }
            return "";
        }

        public override List<string> GetDeviceProperties(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.SB:
                    return new List<string>(new string[]
                    {
                        "ST",
                        "M",
                        "P_DT"
                    });
            }
            return null;
        }
    }
}
