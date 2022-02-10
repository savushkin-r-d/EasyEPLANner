using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - дискретный вход.
    /// </summary>
    sealed public class DI : IODevice
    {
        public DI(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DI;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "DI_VIRT":
                    break;

                case "DI":
                case "":
                    parameters.Add(Parameter.P_DT, null);

                    dSubType = DeviceSubType.DI;

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, DI, DI_VIRT).\n",
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
                case DeviceType.DI:
                    switch (dst)
                    {
                        case DeviceSubType.DI:
                            return "DI";
                        case DeviceSubType.DI_VIRT:
                            return "DI_VIRT";
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
                case DeviceType.DI:
                    switch (dst)
                    {
                        case DeviceSubType.DI:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Parameter.P_DT, 1},
                            };

                        case DeviceSubType.DI_VIRT:
                            return new Dictionary<string, int>()
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
