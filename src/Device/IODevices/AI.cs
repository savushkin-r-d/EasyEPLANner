using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - аналоговый вход.
    /// </summary>
    public class AI : IODevice
    {
        public AI(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AI;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "AI_VIRT":
                    break;

                case "AI":
                case "":
                    dSubType = DeviceSubType.AI;

                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_MIN_V, null);
                    parameters.Add(Parameter.P_MAX_V, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, AI, AI_VIRT).\n",
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
                case DeviceType.AI:
                    switch (dst)
                    {
                        case DeviceSubType.AI:
                            return "AI";
                        case DeviceSubType.AI_VIRT:
                            return "AI_VIRT";
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
                case DeviceType.AI:
                    switch (dst)
                    {
                        case DeviceSubType.AI:
                            return new Dictionary<string, int>()
                            {
                                {Tag.M, 1},
                                {Tag.ST, 1},
                                {Tag.P_MIN_V, 1},
                                {Tag.P_MAX_V, 1},
                                {Tag.V, 1},
                            };

                        case DeviceSubType.AI_VIRT:
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
