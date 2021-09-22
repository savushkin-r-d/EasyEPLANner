﻿using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - датчик проводимости.
    /// </summary>
    sealed public class QT : IODevice
    {
        public QT(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.QT;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "QT":
                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_MIN_V, null);
                    parameters.Add(Parameter.P_MAX_V, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    break;

                case "QT_OK":
                    parameters.Add(Parameter.P_C0, null);
                    parameters.Add(Parameter.P_MIN_V, null);
                    parameters.Add(Parameter.P_MAX_V, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "QT_IOLINK":
                    parameters.Add(Parameter.P_ERR, null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                case "QT_VIRT":
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (QT, QT_OK, QT_IOLINK, QT_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (QT, QT_OK, QT_IOLINK, QT_VIRT).\n",
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

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public override string Check()
        {
            string res = base.Check();

            if (DeviceSubType != DeviceSubType.QT_IOLINK &&
                DeviceSubType != DeviceSubType.QT_VIRT)
            {
                if (parameters.Count < 2)
                {
                    res += string.Format(
                        "\"{0}\" - не указан диапазон измерений\n", name);
                }
            }

            bool emptyArticle = ArticleName == string.Empty;
            bool needCheckArticle = DeviceSubType != DeviceSubType.QT_VIRT;
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
                case DeviceType.QT:
                    switch (dst)
                    {
                        case DeviceSubType.QT:
                            return "QT";
                        case DeviceSubType.QT_OK:
                            return "QT_OK";
                        case DeviceSubType.QT_IOLINK:
                            return "QT_IOLINK";
                        case DeviceSubType.QT_VIRT:
                            return "QT_VIRT";
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
                case DeviceType.QT:
                    switch (dst)
                    {
                        case DeviceSubType.QT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Parameter.P_MIN_V, 1},
                                {Parameter.P_MAX_V, 1},
                                {Tag.P_CZ, 1},
                            };

                        case DeviceSubType.QT_OK:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.OK, 1},
                                {Parameter.P_MIN_V, 1},
                                {Parameter.P_MAX_V, 1},
                                {Tag.P_CZ, 1},
                            };

                        case DeviceSubType.QT_IOLINK:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
                                {Tag.M, 1},
                                {Tag.V, 1},
                                {Tag.P_CZ, 1},
                                {Tag.T, 1},
                                {Parameter.P_ERR, 1},
                            };

                        case DeviceSubType.QT_VIRT:
                            return new Dictionary<string, int>()
                            {
                                {Tag.ST, 1},
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
