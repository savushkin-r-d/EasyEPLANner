using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - аварийная звуковая сигнализация.
    /// </summary>
    sealed public class HA : IODevice
    {
        public HA(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HA;
            ArticleName = articleName;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "HA":
                case "":
                    dSubType = DeviceSubType.HA;

                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    break;

                case "HA_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, HA, HA_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.HA_VIRT;
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
                case DeviceType.HA:
                    switch (dst)
                    {
                        case DeviceSubType.HA:
                            return "HA";
                        case DeviceSubType.HA_VIRT:
                            return "HA_VIRT";
                    }
                    break;
            }

            return string.Empty;
        }

        public override Dictionary<ITag, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.HA:
                    switch (dst)
                    {
                        case DeviceSubType.HA:
                        case DeviceSubType.HA_VIRT:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
