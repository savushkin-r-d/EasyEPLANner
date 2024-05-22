using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - блок питания с автоматическим выключателем.
    /// </summary>
    sealed public class G : IODevice
    {
        public G(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.G;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "G":
                case "":
                    dSubType = DeviceSubType.G;

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                case "G_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, G, G_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.G_VIRT;
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
                case DeviceType.G:
                    switch (dst)
                    {
                        case DeviceSubType.G:
                            return "G";
                        case DeviceSubType.G_VIRT:
                            return "G_VIRT";
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
                case DeviceType.G:
                    switch (dst)
                    {
                        case DeviceSubType.G_IOL_4:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.ST, 1},
                                {Tag.ERR, 1},
                                {Tag.ST_CH, 4},
                                {Tag.NOMINAL_CURRENT_CH, 4},
                                {Tag.LOAD_CURRENT_CH, 4},
                                {Tag.ERR_CH, 4},
                            };

                        case DeviceSubType.G_IOL_8:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.ST, 1},
                                {Tag.ERR, 1},
                                {Tag.ST_CH, 8},
                                {Tag.NOMINAL_CURRENT_CH, 8},
                                {Tag.LOAD_CURRENT_CH, 8},
                                {Tag.ERR_CH, 8},
                            };

                        case DeviceSubType.G_VIRT:
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
    }
}
