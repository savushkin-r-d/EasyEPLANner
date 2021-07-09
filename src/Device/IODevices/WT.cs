using System.Collections.Generic;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - тензодатчик(датчик веса).
    /// Параметры:
    /// 1. P_NOMINAL_W - Номинальная нагрузка в кг.
    /// 2. P_RKP - рабочий коэффициент передачи
    /// 3. P_C0    - сдвиг нуля.
    /// 4. P_DT - дельта.
    /// </summary>
    public class WT : IODevice
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

                    parameters.Add("P_NOMINAL_W", null);
                    parameters.Add("P_RKP", null);
                    parameters.Add("P_C0", null);
                    parameters.Add("P_DT", null);
                    break;

                case "WT_VIRT":
                    dSubType = DeviceSubType.WT_VIRT;
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (WT, WT_VIRT).\n", Name);
                    break;
            }
            return errStr;
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == string.Empty)
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
                            return dt.ToString();

                        case DeviceSubType.WT_VIRT:
                            return "WT_VIRT";
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
                case DeviceType.WT:
                    switch (dst)
                    {
                        case DeviceSubType.WT:
                            return new Dictionary<string, int>()
                            {
                                {"ST", 1},
                                {"M", 1},
                                {"V", 1},
                                {"P_NOMINAL_W", 1},
                                {"P_DT", 1},
                                {"P_RKP", 1},
                                {"P_CZ", 1},
                            };
                    }
                    break;
            }

            return null;
        }
    }
}
