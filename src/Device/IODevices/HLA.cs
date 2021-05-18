using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - сигнальная колонна
    /// </summary>
    public class HLA : IODevice
    {
        public HLA(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HLA;
            ArticleName = articleName;
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

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "":
                case "HLA_R":
                    dSubType = DeviceSubType.HLA_R;
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сирена"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красная лампа"));
                    break;

                case "HLA_RY":
                    dSubType = DeviceSubType.HLA_RY;
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сирена"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красная лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Желтая лампа"));
                    break;

                case "HLA_RG":
                    dSubType = DeviceSubType.HLA_RG;
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сирена"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красная лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Зеленая лампа"));
                    break;

                case "HLA_RYG":
                    dSubType = DeviceSubType.HLA_RYG;
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сирена"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красная лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Желтая лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Зеленая лампа"));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (HLA_R, HLA_RY, HLA_RG, HLA_RYG).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.HLA:
                    switch(dst)
                    {
                        case DeviceSubType.HLA_R:
                            return "HLA_R";

                        case DeviceSubType.HLA_RY:
                            return "HLA_RY";

                        case DeviceSubType.HLA_RG:
                            return "HLA_RG";

                        case DeviceSubType.HLA_RYG:
                            return "HLA_RYG";
                    }
                    break;
            }

            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                //TODO: Build correct properties
                case DeviceType.HLA:
                    return new Dictionary<string, int>()
                    {
                        {"ST", 1},
                        {"M", 1},
                        {"P_DT", 1},
                    };
            }
            return null;
        }
    }
}
