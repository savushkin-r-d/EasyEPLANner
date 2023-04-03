using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - датчик давления.
    /// </summary>
    sealed public class PT : IODevice
    {
        public PT(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.PT;
            ArticleName = articleName;
        }

        public override string PIDUnitFormat => UnitFormat.Bars;

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "PT":
                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_MIN_V, null);
                    parameters.Add(Parameter.P_MAX_V, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    break;

                case "PT_IOLINK":
                    parameters.Add(Parameter.P_ERR, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                case "PT_VIRT":
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (PT, PT_IOLINK, PT_VIRT).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (PT, PT_IOLINK, PT_VIRT).\n", Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = string.Empty;
            if (parameters.ContainsKey(Parameter.P_MIN_V) &&
                parameters.ContainsKey(Parameter.P_MAX_V))
            {
                range = "_" + parameters[Parameter.P_MIN_V].ToString() + 
                    ".." + parameters[Parameter.P_MAX_V].ToString();
            }

            return range;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.PT_VIRT;
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
                case DeviceType.PT:
                    switch (dst)
                    {
                        case DeviceSubType.PT:
                            return "PT";
                        case DeviceSubType.PT_IOLINK:
                            return "PT_IOLINK";
                        case DeviceSubType.PT_VIRT:
                            return "PT_VIRT";
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
                case DeviceType.PT:
                    switch (dst)
                    {
                        case DeviceSubType.PT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_MIN_V, 1},
                                {Parameter.P_MAX_V, 1},
                                {Tag.P_CZ, 1},
                            };

                        case DeviceSubType.PT_IOLINK:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_MIN_V, 1},
                                {Parameter.P_MAX_V, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.PT_VIRT:
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
