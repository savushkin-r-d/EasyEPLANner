using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - аналоговый выход.
    /// </summary>
    sealed public class AO : IODevice
    {
        public AO(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AO;
        }

        public override string PIDUnitFormat => UnitFormat.Percentages;

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "AO_VIRT":
                    break;

                case "AO":
                case "":
                    dSubType = DeviceSubType.AO;
                    parameters.Add(Parameter.P_MIN_V, null);
                    parameters.Add(Parameter.P_MAX_V, null);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    break;

                case nameof(DeviceSubType.AO_EY):
                    RuntimeParameters.Add(RuntimeParameter.R_EY_NUMBER.Name, null);
                    goto case nameof(DeviceSubType.AO);

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, AO, AO_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = string.Empty;
            if (parameters.ContainsKey(Parameter.P_MIN_V) &&
                parameters.ContainsKey(Parameter.P_MAX_V))
            {
                range = "_" + parameters[Parameter.P_MIN_V].ToString() +
                    ".." + parameters[Parameter.P_MAX_V].ToString();
            }

            return range;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.AO:
                    switch (dst)
                    {
                        case DeviceSubType.AO:
                            return "AO";
                        case DeviceSubType.AO_VIRT:
                            return "AO_VIRT";
                        case DeviceSubType.AO_EY:
                            return nameof(DeviceSubType.AO_EY);
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
                case DeviceType.AO:
                    switch (dst)
                    {
                        case DeviceSubType.AO:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_MIN_V, 1},
                                {Parameter.P_MAX_V, 1},
                            };

                        case DeviceSubType.AO_EY:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_MIN_V, 1},
                                {Parameter.P_MAX_V, 1},
                                {Tag.ERR, 1}
                            };

                        case DeviceSubType.AO_VIRT:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.M, 1},
                                {Tag.V, 1},
                            };


                    }
                    break;
            }

            return null;
        }
    }
}