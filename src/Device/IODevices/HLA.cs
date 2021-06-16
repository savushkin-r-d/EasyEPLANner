using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - сигнальная колонна
    /// </summary>
    public class HLA : IODevice
    {
        public HLA(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(fullName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HLA;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == string.Empty)
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "":
                case "HLA":
                    dSubType = DeviceSubType.HLA;
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красная лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Желтая лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Зеленая лампа"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сирена"));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (HLA).\n",
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
                        case DeviceSubType.HLA:
                            return "HLA";
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
                case DeviceType.HLA:
                    switch(dst)
                    {
                        case DeviceSubType.HLA:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1}, //TODO
                                {"M", 1},
                                {"P_DT", 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
