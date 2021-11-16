using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - камера.
    /// </summary>
    sealed public class CAM : IODevice
    {
        public CAM(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.CAM;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "CAM_DO1_DI2":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сигнал активации"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Результат обработки"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Готовность"));

                    parameters.Add(Parameter.P_READY_TIME, null);

                    properties.Add(Property.IP, null);
                    break;

                case "CAM_DO1_DI1":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сигнал активации"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Результат обработки"));

                    properties.Add(Property.IP, null);
                    break;

                case "CAM_DO1_DI3":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Сигнал активации"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Результат обработки"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Готовность"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Результат обработки 2"));

                    parameters.Add(Parameter.P_READY_TIME, null);

                    properties.Add(Property.IP, null);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (CAM_DO1_DI2, CAM_DO1_DI1, CAM_DO1_DI3).\n",
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
                case DeviceType.CAM:
                    switch (dst)
                    {
                        case DeviceSubType.CAM_DO1_DI1:
                            return "CAM_DO1_DI1";
                        case DeviceSubType.CAM_DO1_DI2:
                            return "CAM_DO1_DI2";
                        case DeviceSubType.CAM_DO1_DI3:
                            return "CAM_DO1_DI3";
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
                case DeviceType.CAM:
                    switch (dst)
                    {
                        case DeviceSubType.CAM_DO1_DI1:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.RESULT, 1},
                            };

                        case DeviceSubType.CAM_DO1_DI2:
                        case DeviceSubType.CAM_DO1_DI3:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.READY, 1},
                                {Tag.RESULT, 1},
                                {Parameter.P_READY_TIME, 1}
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
