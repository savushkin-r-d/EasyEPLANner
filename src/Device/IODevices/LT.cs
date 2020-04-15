namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик текущего уровня.
    /// Параметры:
    /// 1. P_C0 - сдвиг нуля.    
    /// </summary>
    public class LT : IODevice
    {
        public LT(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.LT;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "LT":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_ERR", null);
                    break;

                case "LT_CYL":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    parameters.Add("P_ERR", null);
                    break;

                case "LT_CONE":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    parameters.Add("P_H_CONE", null);
                    parameters.Add("P_ERR", null);
                    break;

                case "LT_TRUNC":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    parameters.Add("P_H_TRUNC", null);
                    parameters.Add("P_ERR", null);
                    break;

                case "LT_IOLINK":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    parameters.Add("P_H_CONE", 0);
                    parameters.Add("P_ERR", null);

                    IOLinkSizeIn = 2;
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

            if (ArticleName == "" && dSubType != DeviceSubType.LT_VIRT)
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
            return "";
        }
    }
}
