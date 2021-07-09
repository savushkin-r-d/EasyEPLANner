using System.Collections.Generic;

namespace Device
{
    public class F : IODevice
    {
        public F(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.F;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "F":
                case "":
                    dSubType = DeviceSubType.F;

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                case "F_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, F, F_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == string.Empty &&
                dSubType != DeviceSubType.F_VIRT)
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
                case DeviceType.F:
                    switch (dst)
                    {
                        case DeviceSubType.F:
                            return "F";
                        case DeviceSubType.F_VIRT:
                            return "F_VIRT";
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
                case DeviceType.F:
                    switch (dst)
                    {
                        case DeviceSubType.F:
                            return new Dictionary<string, int>()
                            {
                                {"M", 1},
                                {"V", 1},
                                {"ST", 1},
                                {"ERR", 1},
                                {"ST_CH", 4},
                                {"NOMINAL_CURRENT_CH", 4},
                                {"LOAD_CURRENT_CH", 4},
                                {"ERR_CH", 4},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
