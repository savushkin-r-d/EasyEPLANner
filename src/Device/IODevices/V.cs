using IO;
using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - клапан.
    /// Параметры:
    /// 1. P_ON_TIME - время включения, мсек.
    /// 2. P_FB - обратная связь, 1/0 (Да/Нет).
    /// 3. R_AS_NUMBER - номер клапана в AS-i.
    /// 4. R_VTUG_SIZE - размер области клапана для пневмоострова.
    /// 5. R_VTUG_NUMBER - номер клапана на пневмоострове.
    /// </summary>
    public class V : IODevice
    {
        public V(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.V;
            ArticleName = articleName;
        }

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public override string Check()
        {
            string res = base.Check();

            if (dSubType == DeviceSubType.NONE)
            {
                res += string.Format(
                    "\"{0}\" - не задан тип (V_DO1, V_DO2, ...).\n", name);
            }

            if ((dSubType == DeviceSubType.V_IOLINK_VTUG_DO1 ||
                    dSubType == DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF ||
                    dSubType == DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON) &&
                AO[0].Node >= 0 && AO[0].Module > 0)
            {
                // DEV_VTUG - поддержка старых проектов
                if (
                    IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0] != null &&
                    IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0].DeviceType != DeviceType.Y &&
                    IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0].DeviceType != DeviceType.DEV_VTUG)
                {
                    res += string.Format("\"{0}\" - первым в списке привязанных устройств должен идти пневмоостров Festo VTUG.\n",
                        name);
                }
                else
                {
                    var vtug = IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0];
                    switch (vtug.DeviceSubType)
                    {
                        case DeviceSubType.DEV_VTUG_8:
                            rtParameters["R_VTUG_SIZE"] = 1;
                            break;

                        case DeviceSubType.DEV_VTUG_16:
                            rtParameters["R_VTUG_SIZE"] = 2;
                            break;

                        case DeviceSubType.DEV_VTUG_24:
                            rtParameters["R_VTUG_SIZE"] = 3;
                            break;
                    }
                }
            }

            if (ArticleName == "")
            {
                res += $"\"{name}\" - не задано изделие.\n";
            }

            return res;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "V_DO1":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    break;

                case "V_DO2":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Закрыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    break;

                case "V_DO1_DI1_FB_OFF":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_DO1_DI1_FB_ON":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_DO1_DI2":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_DO2_DI2":
                case "V_DO2_DI2_BISTABLE":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Закрыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_MIXPROOF":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть НС"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть ВС"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_IOLINK_MIXPROOF":
                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    SetIOLinkSizes(ArticleName);
                    break;

                case "V_AS_MIXPROOF":
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    rtParameters.Add("R_AS_NUMBER", null);
                    break;

                case "V_BOTTOM_MIXPROOF":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть мини"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть НС"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_AS_DO1_DI2":
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    rtParameters.Add("R_AS_NUMBER", null);
                    break;

                case "V_IOLINK_DO1_DI2":
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    SetIOLinkSizes(ArticleName);
                    break;

                case "V_IOLINK_VTUG_DO1":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    break;

                case "V_IOLINK_VTUG_DO1_FB_OFF":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_IOLINK_VTUG_DO1_FB_ON":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_IOLINK_VTUG_DO1_DI2":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "":
                    errStr = string.Format(
                        "\"{0}\" - не задан тип (V_DO1, V_DO2, ...).\n", name);
                    break;

                default:
                    errStr = string.Format(
                        "\"{0}\" - неверный тип (V_DO1, V_DO2, ...).\n", name);
                    break;
            }

            return errStr;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt, 
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.V:
                    switch (dst)
                    {
                        case DeviceSubType.V_DO1:
                            return "V_DO1";
                        case DeviceSubType.V_DO2:
                            return "V_DO2";
                        case DeviceSubType.V_DO1_DI1_FB_OFF:
                            return "V_DO1_DI1_FB_OFF";
                        case DeviceSubType.V_DO1_DI1_FB_ON:
                            return "V_DO1_DI1_FB_ON";
                        case DeviceSubType.V_DO1_DI2:
                            return "V_DO1_DI2";
                        case DeviceSubType.V_DO2_DI2:
                            return "V_DO2_DI2";
                        case DeviceSubType.V_MIXPROOF:
                            return "V_MIXPROOF";
                        case DeviceSubType.V_IOLINK_MIXPROOF:
                            return "V_IOLINK_MIXPROOF";
                        case DeviceSubType.V_AS_MIXPROOF:
                            return "V_AS_MIXPROOF";
                        case DeviceSubType.V_BOTTOM_MIXPROOF:
                            return "V_BOTTOM_MIXPROOF";
                        case DeviceSubType.V_AS_DO1_DI2:
                            return "V_AS_DO1_DI2";
                        case DeviceSubType.V_IOLINK_DO1_DI2:
                            return "V_IOLINK_DO1_DI2";
                        case DeviceSubType.V_DO2_DI2_BISTABLE:
                            return "V_DO2_DI2_BISTABLE";
                        case DeviceSubType.V_IOLINK_VTUG_DO1:
                            return "V_IOLINK_VTUG_DO1";
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF:
                            return "V_IOLINK_VTUG_DO1_FB_OFF";
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON:
                            return "V_IOLINK_VTUG_DO1_FB_ON";
                        case DeviceSubType.V_IOLINK_VTUG_DO1_DI2:
                            return "V_IOLINK_VTUG_DO1_DI2";
                    }
                    break;
            }
            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch(dt)
            {
                case DeviceType.V:
                    switch (dst)
                    {
                        case DeviceSubType.V_DO1:
                        case DeviceSubType.V_DO2:
                        case DeviceSubType.V_IOLINK_VTUG_DO1:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                            };

                        case DeviceSubType.V_DO1_DI1_FB_ON:
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON:
                        case DeviceSubType.V_DO1_DI1_FB_OFF:
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"P_ON_TIME", 1},
                                {"P_FB", 1},
                                {"FB_OFF_ST", 1},
                            };

                        case DeviceSubType.V_DO1_DI2:
                        case DeviceSubType.V_DO2_DI2:
                        case DeviceSubType.V_DO2_DI2_BISTABLE:
                        case DeviceSubType.V_MIXPROOF:
                        case DeviceSubType.V_AS_MIXPROOF:
                        case DeviceSubType.V_AS_DO1_DI2:
                        case DeviceSubType.V_BOTTOM_MIXPROOF:
                        case DeviceSubType.V_IOLINK_VTUG_DO1_DI2:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"P_ON_TIME", 1},
                                {"P_FB", 1},
                                {"FB_OFF_ST", 1},
                                {"FB_ON_ST", 1},
                            };

                        case DeviceSubType.V_IOLINK_MIXPROOF:
                        case DeviceSubType.V_IOLINK_DO1_DI2:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"P_ON_TIME", 1},
                                {"P_FB", 1},
                                {"V", 1},
                                {"BLINK", 1},
                                {"CS", 1},
                                {"ERR", 1},
                            };
                    }
                    break;
            }
            return null;
        }
    }
}