using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - дискретный выход.
    /// </summary>
    public class DO : IODevice
    {
        public DO(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DO;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "DO_VIRT":
                    break;

                case "DO":
                case "":
                    dSubType = DeviceSubType.DO;

                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, DO, DO_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.DO:
                    switch (dst)
                    {
                        case DeviceSubType.DO:
                            return "DO";
                        case DeviceSubType.DO_VIRT:
                            return "DO_VIRT";
                    }
                    break;
            }

            return string.Empty;
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.DO:
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
