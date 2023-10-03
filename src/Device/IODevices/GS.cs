using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - датчик положения.
    /// </summary>
    sealed public class GS : IODevice
    {
        public GS(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.GS;
            ArticleName = articleName;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "GS":
                case "":
                    parameters.Add(Parameter.P_DT, null);

                    dSubType = DeviceSubType.GS;

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "GS_INVERSE":
                    parameters.Add(Parameter.P_DT, null);

                    dSubType = DeviceSubType.GS_INVERSE;

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "GS_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, GS, GS_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.GS_VIRT;
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
                case DeviceType.GS:
                    switch (dst)
                    {
                        case DeviceSubType.GS:
                            return "GS";
                        case DeviceSubType.GS_VIRT:
                            return "GS_VIRT";
                        case DeviceSubType.GS_INVERSE:
                            return nameof(DeviceSubType.GS_INVERSE);
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
                case DeviceType.GS:
                    switch (dst)
                    {
                        case DeviceSubType.GS_INVERSE:
                        case DeviceSubType.GS:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_DT, 1},
                            };

                        case DeviceSubType.GS_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1}
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
