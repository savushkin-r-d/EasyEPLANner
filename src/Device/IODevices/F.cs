using System.Collections.Generic;

namespace Device
{
    public class F : IODevice
    {
        public F(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.F;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "F":
                case "":
                    dSubType = DeviceSubType.F;

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, F).\n",
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
                case DeviceType.F:
                    switch (dst)
                    {
                        case DeviceSubType.F:
                            return "F";
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
                case DeviceType.F:
                    switch (dst)
                    {
                        case DeviceSubType.F:
                            return new List<string>(new string[]
                            {
                                "M",
                                "V",
                                "ST",
                                "ERR",
                                "ST_CH[1]",
                                "ST_CH[2]",
                                "ST_CH[3]",
                                "ST_CH[4]",
                                "NOMINAL_CURRENT[1]",
                                "NOMINAL_CURRENT[2]",
                                "NOMINAL_CURRENT[3]",
                                "NOMINAL_CURRENT[4]",
                                "LOAD_CURRENT[1]",
                                "LOAD_CURRENT[2]",
                                "LOAD_CURRENT[3]",
                                "LOAD_CURRENT[4]",
                                "ERR_CH[1]",
                                "ERR_CH[2]",
                                "ERR_CH[3]",
                                "ERR_CH[4]",
                            });
                    }
                    break;
            }
            return null;
        }
    }
}
