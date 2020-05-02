using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик давления.
    /// Параметры:
    /// 1. P_C0    - сдвиг нуля.
    /// 2. P_MIN_V - минимальное значение.
    /// 3. P_MAX_V - максимальное значение.
    /// </summary>
    public class PT : IODevice
    {
        public PT(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.PT;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, ""));
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = "";
            switch (subType)
            {
                case "PT":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);
                    break;

                case "PT_IOLINK":
                    parameters.Add("P_ERR", null);

                    SetIOLinkSizes(ArticleName);
                    break;

                case "DEV_SPAE":
                    parameters.Add("P_ERR", null);

                    SetIOLinkSizes(ArticleName);
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (PT, PT_IOLINK, DEV_SPAE).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (PT, PT_IOLINK, DEV_SPAE).\n", Name);
                    break;
            }
            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_V") &&
                parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." +
                    parameters["P_MAX_V"].ToString();
            }
            return range;
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
                case DeviceType.PT:
                    switch (dst)
                    {
                        case DeviceSubType.PT:
                            return "PT";

                        case DeviceSubType.PT_IOLINK:
                            return "PT_IOLINK";

                        case DeviceSubType.DEV_SPAE:
                            return "DEV_SPAE";
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
                case DeviceType.PT:
                    switch (dst)
                    {
                        case DeviceSubType.PT:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "V",
                                "P_MIN_V",
                                "P_MAX_V",
                                "P_CZ"
                            });

                        case DeviceSubType.PT_IOLINK:
                            return new List<string>(new string[]
                            {
                                "M",
                                "V",
                                "P_MIN_V",
                                "P_MAX_V",
                            });

                        case DeviceSubType.DEV_SPAE:
                            return new List<string>(new string[]
                            {
                                "M",
                                "V"
                            });
                    }
                    break;
            }
            return null;
        }
    }
}
