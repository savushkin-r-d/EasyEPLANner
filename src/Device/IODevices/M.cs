using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - мотор.
    /// </summary>
    sealed public class M : IODevice
    {
        public M(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.M;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "M":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_FREQ":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_REV":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_REV_FREQ":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_REV_2":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_REV_FREQ_2":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;


                case "M_REV_2_ERROR":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Авария"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_REV_FREQ_2_ERROR":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Авария"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_ATV":
                    properties.Add(Property.IP, null);

                    parameters.Add(Parameter.P_ON_TIME, null);
                    break;

                case "M_ATV_LINEAR":
                    properties.Add(Property.IP, null);

                    parameters.Add(Parameter.P_ON_TIME, null);

                    parameters.Add(Parameter.P_SHAFT_DIAMETER, null);
                    parameters.Add(Parameter.P_TRANSFER_RATIO, null);
                    break;

                case "M_VIRT":
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (M, M_FREQ, M_REV, M_REV_FREQ, M_REV_2," +
                        " M_REV_FREQ_2, M_REV_2_ERROR, M_REV_FREQ_2_ERROR, " +
                        "M_ATV, M_ATV_LINEAR, M_VIRT).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (M, M_FREQ, M_REV, M_REV_FREQ, M_REV_2," +
                        " M_REV_FREQ_2, M_REV_2_ERROR, M_REV_FREQ_2_ERROR, " +
                        "M_ATV, M_ATV_LINEAR, M_VIRT).\n", Name);
                    break;
            }

            return errStr;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.M:
                    switch (dst)
                    {
                        case DeviceSubType.M:
                            return "M";
                        case DeviceSubType.M_FREQ:
                            return "M_FREQ";
                        case DeviceSubType.M_REV:
                            return "M_REV";
                        case DeviceSubType.M_REV_FREQ:
                            return "M_REV_FREQ";
                        case DeviceSubType.M_REV_2:
                            return "M_REV_2";
                        case DeviceSubType.M_REV_FREQ_2:
                            return "M_REV_FREQ_2";
                        case DeviceSubType.M_REV_2_ERROR:
                            return "M_REV_2_ERROR";
                        case DeviceSubType.M_REV_FREQ_2_ERROR:
                            return "M_REV_FREQ_2_ERROR";
                        case DeviceSubType.M_ATV:
                            return "M_ATV";
                        case DeviceSubType.M_ATV_LINEAR:
                            return "M_ATV_LINEAR";
                        case DeviceSubType.M_VIRT:
                            return "M_VIRT";
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
                case DeviceType.M:
                    switch (dst)
                    {
                        case DeviceSubType.M:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_ON_TIME, 1},
                            };

                        case DeviceSubType.M_FREQ:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_ON_TIME, 1},
                                {Tag.V, 1},
                            };

                        case DeviceSubType.M_REV:
                        case DeviceSubType.M_REV_FREQ:
                        case DeviceSubType.M_REV_2:
                        case DeviceSubType.M_REV_FREQ_2:
                        case DeviceSubType.M_REV_2_ERROR:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_ON_TIME, 1},
                                {Tag.V, 1},
                                {Tag.R, 1},
                            };

                        case DeviceSubType.M_ATV:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.ST, 1},
                                {Tag.R, 1},
                                {Tag.FRQ, 1},
                                {Tag.RPM, 1},
                                {Tag.EST, 1},
                                {Tag.V, 1},
                                {Parameter.P_ON_TIME, 1},
                            };

                        case DeviceSubType.M_ATV_LINEAR:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.ST, 1},
                                {Tag.R, 1},
                                {Tag.FRQ, 1},
                                {Tag.RPM, 1},
                                {Tag.EST, 1},
                                {Tag.V, 1},
                                {Parameter.P_ON_TIME, 1},
                                {Parameter.P_SHAFT_DIAMETER, 1},
                                {Parameter.P_TRANSFER_RATIO, 1}
                            };

                        case DeviceSubType.M_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.ST, 1},
                                {Tag.V, 1},
                            };
                    }
                    break;
            }

            return null;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType == DeviceSubType.M_ATV ||
                DeviceSubType == DeviceSubType.M_ATV_LINEAR;
            if (needCheckArticle && emptyArticle)
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }
    }
}
