using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - датчик наличия потока.
    /// </summary>
    sealed public class FS : IODevice
    {
        public FS(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.FS;
            ArticleName = articleName;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "FS":
                case "":
                    parameters.Add(Parameter.P_DT, null);

                    dSubType = DeviceSubType.FS;

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "FS_VIRT":
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, FS, FS_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.FS_VIRT;
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
                case DeviceType.FS:
                    switch (dst)
                    {
                        case DeviceSubType.FS:
                            return "FS";
                        case DeviceSubType.FS_VIRT:
                            return "FS_VIRT";
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
                case DeviceType.FS:
                    switch (dst)
                    {
                        case DeviceSubType.FS:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_DT, 1},
                            };

                        case DeviceSubType.FS_VIRT:
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
