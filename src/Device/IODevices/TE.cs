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
                    IOLinkSizeIn = 1;
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
    }
}
