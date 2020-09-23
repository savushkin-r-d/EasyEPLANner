using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - управляемый клапан.
    /// </summary>
    public class VC : IODevice
    {
        public VC(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.VC;
            ArticleName = articleName;

            AO.Add(new IOChannel("AO", -1, -1, -1, ""));
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.VC:
                    return dt.ToString();
            }
            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.VC:
                    return new Dictionary<string, int>()
                    {
                        {"ST", 1},
                        {"M", 1},
                        {"V", 1},
                    };
            }
            return null;
        }
    }
}
