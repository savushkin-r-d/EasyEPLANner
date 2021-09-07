using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - кнопка.
    /// </summary>
    sealed public class SB : IODevice
    {
        public SB(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.SB;
            ArticleName = articleName;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "SB":
                case "":
                    dSubType = DeviceSubType.SB;
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "SB_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (SB, SB_VIRT).\n", Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == string.Empty)
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
                    switch (dst)
                    {
                        case DeviceSubType.SB:
                            return "SB";
                        case DeviceSubType.SB_VIRT:
                            return "SB_VIRT";
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
                case DeviceType.SB:
                    switch (dst)
                    {
                        case DeviceSubType.SB:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_DT, 1},
                            };

                        case DeviceSubType.SB_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
