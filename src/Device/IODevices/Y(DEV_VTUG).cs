namespace Device
{
    /// <summary>
    /// Устройство - пневмоостров.
    /// </summary>

    public class Y : IODevice
    {
        public Y(string name, string eplanName, string description, 
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.Y;
            ArticleName = articleName;

            AO.Add(new IOChannel("AO", -1, -1, -1, ""));
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);
            string errStr = "";
            int sizeOut;
            switch (subtype)
            {
                case "DEV_VTUG_8":
                    sizeOut = 1;
                    IOLinkProperties.SizeOut = sizeOut;
                    IOLinkProperties.SizeOutFromFile = sizeOut;
                    break;

                case "DEV_VTUG_16":
                    sizeOut = 2;
                    IOLinkProperties.SizeOut = sizeOut;
                    IOLinkProperties.SizeOutFromFile = sizeOut;
                    break;

                case "DEV_VTUG_24":
                    sizeOut = 3;
                    IOLinkProperties.SizeOut = sizeOut;
                    IOLinkProperties.SizeOutFromFile = sizeOut;

                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип " +
                        "(DEV_VTUG_8, ...).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип " +
                        "(DEV_VTUG_8, ...).\n", Name);
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

    /// <summary>
    /// Устройство - пневмоостров.
    /// СОВМЕСТИМОСТЬ СО СТАРЫМИ ПРОЕКТАМИ
    /// </summary>
    public class DEV_VTUG : IODevice
    {
        public DEV_VTUG(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DEV_VTUG;
            ArticleName = articleName;

            AO.Add(new IOChannel("AO", -1, -1, -1, ""));
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DEV_VTUG_8":
                    IOLinkProperties.SizeOut = 1;
                    break;

                case "DEV_VTUG_16":
                    IOLinkProperties.SizeOut = 2;
                    break;

                case "DEV_VTUG_24":
                    IOLinkProperties.SizeOut = 3;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип " +
                        "(DEV_VTUG_8, ...).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип " +
                        "(DEV_VTUG_8, ...).\n", Name);
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
