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

        public override List<string> GetDeviceProperties(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.VC:
                    return new List<string>(new string[]
                    {
                        "ST",
                        "M",
                        "V"
                    });
            }
            return null;
        }
    }
}
