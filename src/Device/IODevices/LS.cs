﻿using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - предельный уровень.
    /// Параметры:
    /// 1. P_DT - время порогового фильтра, мсек.
    /// </summary>
    public class LS : IODevice
    {
        public LS(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.LS;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "LS_MIN":
                    parameters.Add("P_DT", null);

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "LS_MAX":
                    parameters.Add("P_DT", null);

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "LS_IOLINK_MIN":
                    parameters.Add("P_DT", null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    SetIOLinkSizes(ArticleName);
                    break;

                case "LS_IOLINK_MAX":
                    parameters.Add("P_DT", null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    SetIOLinkSizes(ArticleName);
                    break;

                case "LS_VIRT":
                    dSubType = DeviceSubType.LS_VIRT;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип (LS_MIN, " +
                        "LS_MAX, LS_IOLINK_MIN, LS_IOLINK_MAX, LS_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (LS_MIN, " +
                        "LS_MAX, LS_IOLINK_MIN, LS_IOLINK_MAX, LS_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetConnectionType()
        {
            string connectionType = "";
            switch (dSubType)
            {
                case DeviceSubType.LS_MIN:
                case DeviceSubType.LS_IOLINK_MIN:
                    connectionType = "_Min";
                    break;

                case DeviceSubType.LS_MAX:
                case DeviceSubType.LS_IOLINK_MAX:
                    connectionType = "_Max";
                    break;


                default:
                    connectionType = "";
                    break;
            }
            return connectionType;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == "" && dSubType != DeviceSubType.LS_VIRT)
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.LS:
                    switch (dst)
                    {
                        case DeviceSubType.LS_MIN:
                            return "LS_MIN";
                        case DeviceSubType.LS_MAX:
                            return "LS_MAX";
                        case DeviceSubType.LS_IOLINK_MIN:
                            return "LS_IOLINK_MIN";
                        case DeviceSubType.LS_IOLINK_MAX:
                            return "LS_IOLINK_MAX";
                        case DeviceSubType.LS_VIRT:
                            return "LS_VIRT";
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
                case DeviceType.LS:
                    switch (dst)
                    {
                        case DeviceSubType.LS_MIN:
                        case DeviceSubType.LS_MAX:
                        case DeviceSubType.LS_VIRT:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "P_DT"
                            });

                        case DeviceSubType.LS_IOLINK_MIN:
                        case DeviceSubType.LS_IOLINK_MAX:
                            return new List<string>(new string[]
                            {
                                "ST",
                                "M",
                                "V"
                            });
                    }
                    break;
            }
            return null;
        }
    }
}
