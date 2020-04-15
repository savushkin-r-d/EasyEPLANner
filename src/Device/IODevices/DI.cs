﻿using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - дискретный вход.
    /// </summary>
    public class DI : IODevice
    {
        public DI(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber) : base(fullName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DI;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DI_VIRT":
                    dSubType = DeviceSubType.DI_VIRT;
                    break;

                case "DI":
                case "":
                    parameters.Add("P_DT", null);
                    dSubType = DeviceSubType.DI;
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (DI, DI_VIRT).\n",
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
            return "";
        }

        public override List<string> GetDeviceProperties(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.DI:
                    switch (dst)
                    {
                        case DeviceSubType.DI:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "P_DT"
                            });

                        case DeviceSubType.DI_VIRT:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M"
                            });
                    }
                    break;
            }
            return null;
        }
    }
}
