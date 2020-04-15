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
        public AO(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(fullName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AO;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "AO_VIRT":
                    dSubType = DeviceSubType.AO_VIRT;
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
                        " (AO, AO_VIRT).\n",
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

    }
}
