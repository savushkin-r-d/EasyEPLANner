using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - аналоговый выход.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение.
    /// 2. P_MAX_V - максимальное значение.
    /// </summary>
    public class AO : IODevice
    {
        public AO(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AO;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "AO_VIRT":
                    break;

                case "AO":
                case "":
                    dSubType = DeviceSubType.AO;
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, AO, AO_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = string.Empty;
            if (parameters.ContainsKey("P_MIN_V") &&
                parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." +
                    parameters["P_MAX_V"].ToString();
            }

            return range;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.AO:
                    switch (dst)
                    {
                        case DeviceSubType.AO:
                            return "AO";
                        case DeviceSubType.AO_VIRT:
                            return "AO_VIRT";
                    }
                    break;
            }

            return string.Empty;
        }

        public override Dictionary<string,int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.AO:
                    switch (dst)
                    {
                        case DeviceSubType.AO:
                            return new Dictionary<string, int>()
                            {
                                {"M", 1},
                                {"V", 1},
                                {"P_MIN_V", 1},
                                {"P_MAX_V", 1},
                            };

                        case DeviceSubType.AO_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {"M", 1},
                                {"V", 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}