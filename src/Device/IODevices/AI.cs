﻿using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - аналоговый вход.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение.
    /// 2. P_MAX_V - максимальное значение.
    /// 3. P_C0    - сдвиг нуля.
    /// </summary>
    public class AI : IODevice
    {
        public AI(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(fullName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AI;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "AI_VIRT":
                    dSubType = DeviceSubType.AI_VIRT;
                    break;

                case "AI":
                case "":
                    dSubType = DeviceSubType.AI;

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (AI, AI_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_V") &&
                parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." +
                    parameters["P_MAX_V"].ToString();
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
            return "";
        }

        public override List<string> GetDeviceProperties(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.AI:
                    switch (dst)
                    {
                        case DeviceSubType.AI:
                            return new List<string>(new string[]
                            {
                                "M",
                                "ST",
                                "P_MIN_V",
                                "P_MAX_V",
                                "V"
                            });

                        case DeviceSubType.AI_VIRT:
                            return new List<string>(new string[]
                            {
                                "M",
                                "ST",
                                "V"
                            });
                    }
                    break;
            }
            return null;
        }
    }
}
