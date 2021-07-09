using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - световая сигнализация.
    /// </summary>
    public class HL : IODevice
    {
        public HL(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HL;
            ArticleName = articleName;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "HL":
                case "":
                    dSubType = DeviceSubType.HL;

                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    break;

                case "HL_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, HL, HL_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == string.Empty &&
                dSubType != DeviceSubType.HL_VIRT)
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
                case DeviceType.HL:
                    switch (dst)
                    {
                        case DeviceSubType.HL:
                            return "HL";
                        case DeviceSubType.HL_VIRT:
                            return "HL_VIRT";
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
                case DeviceType.HL:
                    switch (dst)
                    {
                        case DeviceSubType.HL:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
