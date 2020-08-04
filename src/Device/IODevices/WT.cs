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
        public WT(string fullName, string description, int deviceNumber,
            string objectName, int objectNumber, string articleName) : base(
                fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.WT;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, "Напряжение моста(+Ud)"));
            AI.Add(new IOChannel("AI", -1, -1, -1, "Референсное напряжение(+Uref)"));

            parameters.Add("P_NOMINAL_W", null);
            parameters.Add("P_RKP", null);
            parameters.Add("P_C0", null);
            parameters.Add("P_DT", null);
        }

        public override string Check()
        {
            string res = base.Check();

            if (ArticleName == "")
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
                    return dt.ToString();
            }
            return "";
        }

        public override Dictionary<string, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.WT:
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
            return null;
        }
    }
}
