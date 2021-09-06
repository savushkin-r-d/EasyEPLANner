using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - сигнальная колонна
    /// </summary>
    public class HLA : IODevice
    {
        public HLA(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HLA;
            ArticleName = articleName;
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

                    rtParameters.Add(RuntimeParameter.R_CONST_RED, null);
                    break;

                case "HLA_VIRT":
                    break;

                case "HLA_IOLINK":
                    dSubType = DeviceSubType.HLA_IOLINK;
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    SetIOLinkSizes(ArticleName);

                    properties.Add(Property.SIGNALS_SEQUENCE, null);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип " +
                        "(пустая строка, HLA, HLA_VIRT, HLA_IOLINK).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            if (dSubType == DeviceSubType.HLA_IOLINK)
            {
                return CheckSignalsSequence();
            }

            return string.Empty;
        }

        private string CheckSignalsSequence()
        {
            //TODO: Check sequence;
            return string.Empty;
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
                        case DeviceSubType.HLA_VIRT:
                            return "HLA_VIRT";
                        case DeviceSubType.HLA_IOLINK:
                            return "HLA_IOLINK";
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
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.L_RED, 1 },
                                {Tag.L_YELLOW, 1 },
                                {Tag.L_GREEN, 1 },
                                {Tag.L_SIREN, 1 },
                            };

                        case DeviceSubType.HLA_IOLINK:
                            return GetDeviceIOLinkProperties();
                    }
                    break;
            }

            return null;
        }

        private Dictionary<string, int> GetDeviceIOLinkProperties()
        {
            //TODO: Depends on sequence
            return null;
        }
    }
}
