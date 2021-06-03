using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - счетчик.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение потока.
    /// 2. P_MAX_V - максимальное значение потока.
    /// 3. P_C0    - сдвиг нуля для потока.
    /// 4. P_DT    - дельта.
    /// 5. MT      - связанные моторы.
    /// </summary>
    public class FQT : IODevice
    {
        public FQT(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.FQT;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "FQT":
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Объем"));
                    break;

                case "FQT_F":
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Объем"));
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Поток"));

                    parameters.Add("P_MIN_F", null);
                    parameters.Add("P_MAX_F", null);
                    parameters.Add("P_C0", null);
                    parameters.Add("P_DT", null);

                    properties.Add("MT", null); //Связанные моторы.

                    break;

                case "FQT_F_OK":
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Объем"));
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Поток"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_MIN_F", null);
                    parameters.Add("P_MAX_F", null);
                    parameters.Add("P_C0", null);
                    parameters.Add("P_DT", null);

                    properties.Add("MT", null); //Связанные моторы.

                    break;

                case "FQT_VIRT":
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (FQT, FQT_F, FQT_F_OK, FQT_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (FQT, FQT_F, FQT_F_OK, FQT_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_F") &&
                parameters.ContainsKey("P_MAX_F"))
            {
                range = "_" + parameters["P_MIN_F"].ToString() + ".." +
                    parameters["P_MAX_F"].ToString();
            }
            return range;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == "" && dSubType != DeviceSubType.FQT_VIRT)
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
                case DeviceType.FQT:
                    switch (dst)
                    {
                        case DeviceSubType.FQT:
                            return "FQT";
                        case DeviceSubType.FQT_F:
                            return "FQT_F";
                        case DeviceSubType.FQT_F_OK:
                            return "FQT_F_OK";
                        case DeviceSubType.FQT_VIRT:
                            return "FQT_VIRT";
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
                case DeviceType.FQT:
                    switch (dst)
                    {
                        case DeviceSubType.FQT:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"V", 1},
                                {"ABS_V", 1},
                            };

                        case DeviceSubType.FQT_F:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"V", 1},
                                {"P_MIN_FLOW", 1},
                                {"P_MAX_FLOW", 1},
                                {"P_CZ", 1},
                                {"F", 1},
                                {"P_DT", 1},
                                {"ABS_V", 1},
                            };

                        case DeviceSubType.FQT_F_OK:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"V", 1},
                                {"P_MIN_FLOW", 1},
                                {"P_MAX_FLOW", 1},
                                {"P_CZ", 1},
                                {"F", 1},
                                {"P_DT", 1},
                                {"ABS_V", 1},
                                {"OK", 1},
                            };

                        case DeviceSubType.FQT_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"V", 1},
                                {"P_MIN_FLOW", 1},
                                {"P_MAX_FLOW", 1},
                                {"P_CZ", 1},
                                {"F", 1},
                                {"P_DT", 1},
                                {"ABS_V", 1},
                            };
                    }
                    break;
            }
            return null;
        }
    }
}
