﻿using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Технологическое устройство - тензодатчик (датчик веса).
    /// </summary>
    sealed public class WT : IODevice
    {
        public WT(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.WT;
            ArticleName = articleName;
        }

        public override string PIDUnitFormat => UnitFormat.Kilograms;

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = string.Empty;
            switch (subType)
            {
                case "WT":
                case "":
                    dSubType = DeviceSubType.WT;

                    AI.Add(new IOChannel("AI", -1, -1, -1, "Напряжение моста(+Ud)"));
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Референсное напряжение(+Uref)"));

                    parameters.Add(Parameter.P_NOMINAL_W, null);
                    parameters.Add(Parameter.P_RKP, null);
                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_DT, null);
                    break;

                case "WT_VIRT":
                    break;

                case "WT_RS232":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_C0, null);
                    break;

                case "WT_ETH":
                    parameters.Add(Parameter.P_C0, null);
                    properties.Add(Property.IP, null);
                    break;

                case "WT_PXC_AXL":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));

                    parameters.Add(Parameter.P_DT, null);
                    break;


                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (WT, WT_RS232, WT_VIRT, WT_ETH, WT_PXC_AXL).\n", Name);
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.WT_VIRT;
            if (needCheckArticle && emptyArticle)
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
                case DeviceType.WT:
                    switch(dst)
                    {
                        case DeviceSubType.WT:
                            return "WT";
                        case DeviceSubType.WT_VIRT:
                            return "WT_VIRT";
                        case DeviceSubType.WT_RS232:
                            return "WT_RS232";
                        case DeviceSubType.WT_ETH:
                            return "WT_ETH";
                        case DeviceSubType.WT_PXC_AXL:
                            return "WT_PXC_AXL";
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
                case DeviceType.WT:
                    switch (dst)
                    {
                        case DeviceSubType.WT:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_NOMINAL_W, 1},
                                {Parameter.P_DT, 1},
                                {Parameter.P_RKP, 1},
                                {Tag.P_CZ, 1},
                            };

                        case DeviceSubType.WT_VIRT:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                            };

                        case DeviceSubType.WT_RS232:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.P_CZ, 1},
                            };

                        case DeviceSubType.WT_ETH:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.P_CZ, 1},
                            };

                        case DeviceSubType.WT_PXC_AXL:
                            return new Dictionary<ITag, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_DT, 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
