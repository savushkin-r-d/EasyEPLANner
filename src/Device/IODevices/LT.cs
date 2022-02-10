using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - датчик текущего уровня.
    /// </summary>
    sealed public class LT : IODevice
    {
        public LT(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.LT;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "LT":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);
                    break;

                case "LT_CYL":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);
                    parameters.Add(Parameter.P_MAX_P, null);
                    parameters.Add(Parameter.P_R, null);
                    break;

                case "LT_CONE":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);
                    parameters.Add(Parameter.P_MAX_P, null);
                    parameters.Add(Parameter.P_R, null);
                    parameters.Add(Parameter.P_H_CONE, null);
                    break;

                case "LT_TRUNC":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);
                    parameters.Add(Parameter.P_MAX_P, null);
                    parameters.Add(Parameter.P_R, null);
                    parameters.Add(Parameter.P_H_TRUNC, null);
                    break;

                case "LT_IOLINK":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_ERR, null);
                    parameters.Add(Parameter.P_MAX_P, null);
                    parameters.Add(Parameter.P_R, null);
                    parameters.Add(Parameter.P_H_CONE, 0);
    
                    properties.Add(Property.PT, "\'\'");

                    SetIOLinkSizes(ArticleName);
                    break;

                case "LT_VIRT":
                    dSubType = DeviceSubType.LT_VIRT;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип (LT, " +
                        "LT_CYL, LT_CONE, LT_TRUNC, LT_IOLINK, LT_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (LT, " +
                        "LT_CYL, LT_CONE, LT_TRUNC, LT_IOLINK, LT_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.LT_VIRT;
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
                case DeviceType.LT:
                    switch (dst)
                    {
                        case DeviceSubType.LT:
                            return "LT";
                        case DeviceSubType.LT_IOLINK:
                            return "LT_IOLINK";
                        case DeviceSubType.LT_CONE:
                            return "LT_CONE";
                        case DeviceSubType.LT_CYL:
                            return "LT_CYL";
                        case DeviceSubType.LT_TRUNC:
                            return "LT_TRUNC";
                        case DeviceSubType.LT_VIRT:
                            return "LT_VIRT";
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
                case DeviceType.LT:
                    switch (dst)
                    {
                        case DeviceSubType.LT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.P_CZ, 1},
                                {Tag.V, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.LT_IOLINK:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.P_CZ, 1},
                                {Tag.V, 1},
                                {Parameter.P_H_CONE, 1},
                                {Parameter.P_MAX_P, 1},
                                {Parameter.P_R, 1},
                                {Tag.CLEVEL, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.LT_CYL:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.P_CZ, 1},
                                {Tag.V, 1},
                                {Parameter.P_MAX_P, 1},
                                {Parameter.P_R, 1},
                                {Tag.CLEVEL, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.LT_CONE:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.P_CZ, 1},
                                {Tag.V, 1},
                                {Parameter.P_MAX_P, 1},
                                {Parameter.P_R, 1},
                                {Parameter.P_H_CONE, 1},
                                {Tag.CLEVEL, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.LT_TRUNC:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.P_CZ, 1},
                                {Tag.V, 1},
                                {Parameter.P_MAX_P, 1},
                                {Parameter.P_R, 1},
                                {Parameter.P_H_TRUNC, 1},
                                {Tag.CLEVEL, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.LT_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
