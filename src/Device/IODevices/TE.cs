using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик температуры.
    /// </summary>
    sealed public class TE : IODevice
    {
        public TE(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.TE;
            ArticleName = articleName;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "TE":
                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    break;

                case "TE_IOLINK":
                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                case "TE_VIRT":
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (TE, TE_IOLINK, TE_VIRT).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (TE, TE_IOLINK, TE_VIRT).\n", Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.TE_VIRT;
            if (needCheckArticle && emptyArticle)
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
                        case DeviceSubType.TE_VIRT:
                            return "TE_VIRT";
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
                case DeviceType.TE:
                    switch (dst)
                    {
                        case DeviceSubType.TE:
                        case DeviceSubType.TE_IOLINK:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.P_CZ, 1},
                                {Tag.V, 1},
                                {Tag.ST, 1},
                            };

                        case DeviceSubType.TE_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.ST, 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
