namespace EplanDevice
{
    /// <summary>
    /// Устройство - пневмоостров.
    /// </summary>
    sealed public class Y : IODevice
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
            string errStr = string.Empty;
            int sizeOut;
            switch (subtype)
            {
                case "DEV_VTUG_8":
                    sizeOut = 1;
                    iolinkProperties.SizeOut = sizeOut;
                    iolinkProperties.SizeOutFromFile = sizeOut;
                    break;

                case "DEV_VTUG_16":
                    sizeOut = 2;
                    iolinkProperties.SizeOut = sizeOut;
                    iolinkProperties.SizeOutFromFile = sizeOut;
                    break;

                case "DEV_VTUG_24":
                    sizeOut = 3;
                    iolinkProperties.SizeOut = sizeOut;
                    iolinkProperties.SizeOutFromFile = sizeOut;

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

            bool emptyArticle = ArticleName == string.Empty;
            if (emptyArticle)
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
    sealed public class DEV_VTUG : IODevice
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

            string errStr = string.Empty;
            switch (subtype)
            {
                case "DEV_VTUG_8":
                    iolinkProperties.SizeOut = 1;
                    break;

                case "DEV_VTUG_16":
                    iolinkProperties.SizeOut = 2;
                    break;

                case "DEV_VTUG_24":
                    iolinkProperties.SizeOut = 3;
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

            bool emptyArticle = ArticleName == string.Empty;
            if (emptyArticle)
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }
    }
}
