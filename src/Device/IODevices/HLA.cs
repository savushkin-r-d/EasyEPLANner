using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - сигнальная колонна
    /// </summary>
    public class HLA : IODevice
    {
        public HLA(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HLA;
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
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красный цвет"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Желтый цвет"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Зеленый цвет"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Звуковая сигнализация"));

                    properties.Add("CONST_RED", null);
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
                                {"ST", 1},
                                {"M", 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
