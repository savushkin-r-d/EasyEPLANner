using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - дискретный выход.
    /// </summary>
    public class DO : IODevice
    {
        public DO(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(fullName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DO;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DO_VIRT":
                    dSubType = DeviceSubType.DO_VIRT;
                    break;

                case "DO":
                case "":
                    dSubType = DeviceSubType.DO;
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (DO, DO_VIRT).\n",
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
            return "";
        }

        public override List<string> GetDeviceProperties(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.DO:
                    return new List<string>(new string[]
                    {
                        "ST",
                        "M"
                    });
            }
            return null;
        }
    }
}
