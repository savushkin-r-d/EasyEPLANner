using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик температуры.
    /// Параметры:
    /// 1. P_C0  - сдвиг нуля.
    /// 2. P_ERR - аварийное значение температуры.
    /// </summary>
    public class TE : IODevice
    {
        public TE(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.TE;
            ArticleName = articleName;

            parameters.Add("P_C0", null);
            parameters.Add("P_ERR", null);

            AI.Add(new IOChannel("AI", -1, -1, -1, ""));
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = "";
            switch (subType)
            {
                case "TE":
                    break;

                case "TE_IOLINK":
                    SetIOLinkSizes(ArticleName);
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (TE, TE_IOLINK).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (TE, TE_IOLINK).\n", Name);
                    break;
            }
            return errStr;
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
                case DeviceType.TE:
                    switch (dst)
                    {
                        case DeviceSubType.TE:
                            return "TE";
                        case DeviceSubType.TE_IOLINK:
                            return "TE_IOLINK";
                    }
                    break;
            }
            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.TE:
                    switch (dst)
                    {
                        case DeviceSubType.TE:
                        case DeviceSubType.TE_IOLINK:
                            return new Dictionary<string, int>()
                            {
                                {"M", 1},
                                {"P_CZ", 1},
                                {"V", 1},
                                {"ST", 1},
                            };
                    }
                    break;
            }
            return null;
        }
    }
}
