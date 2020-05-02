using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик проводимости.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение.
    /// 2. P_MAX_V - максимальное значение.
    /// 3. P_C0    - сдвиг нуля.
    /// </summary>
    public class QT : IODevice
    {
        public QT(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.QT;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, ""));
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "QT":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);
                    break;

                case "QT_OK":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "QT_IOLINK":
                    parameters.Add("P_ERR", null);

                    SetIOLinkSizes(ArticleName);
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (QT, QT_OK, QT_IOLINK).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (QT, QT_OK, QT_IOLINK).\n",
                        Name);
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

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public override string Check()
        {
            string res = base.Check();

            if (this.DeviceSubType != DeviceSubType.QT_IOLINK)
            {
                if (parameters.Count < 2)
                {
                    res += string.Format(
                        "{0} - не указан диапазон измерений\n", name);
                }
            }

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
                case DeviceType.QT:
                    switch (dst)
                    {
                        case DeviceSubType.QT:
                            return "QT";
                        case DeviceSubType.QT_OK:
                            return "QT_OK";
                        case DeviceSubType.QT_IOLINK:
                            return "QT_IOLINK";
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
                case DeviceType.QT:
                    switch (dst)
                    {
                        case DeviceSubType.QT:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "V",
                                "P_MIN_V",
                                "P_MAX_V",
                                "P_CZ"
                            });

                        case DeviceSubType.QT_OK:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "V",
                                "OK",
                                "P_MIN_V",
                                "P_MAX_V",
                                "P_CZ"
                            });

                        case DeviceSubType.QT_IOLINK:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "V",
                                "P_CZ",
                                "T",
                            });
                    }
                    break;
            }
            return null;
        }
    }
}
