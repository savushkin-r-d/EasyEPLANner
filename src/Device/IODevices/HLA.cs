using StaticHelper;
using System.Collections.Generic;
using System.Linq;

namespace Device
{
    /// <summary>
    /// Технологическое устройство - сигнальная колонна
    /// </summary>
    public class HLA : IODevice
    {
        public HLA(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            string articleName) : base(name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HLA;
            ArticleName = articleName;

            hasAlarm = false;
            hasBlue = false;
            hasGreen = false;
            hasYellow = false;
            hasRed = false;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = string.Empty;
            switch (subtype)
            {
                case "":
                case "HLA":
                    dSubType = DeviceSubType.HLA;
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Красный цвет"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Желтый цвет"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Зеленый цвет"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Звуковая сигнализация"));

                    hasAlarm = true;
                    hasGreen = true;
                    hasYellow = true;
                    hasRed = true;

                    rtParameters.Add(RuntimeParameter.R_CONST_RED, null);
                    break;

                case "HLA_VIRT":
                    break;

                case "HLA_IOLINK":
                    dSubType = DeviceSubType.HLA_IOLINK;
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    SetIOLinkSizes(ArticleName);

                    properties.Add(Property.SIGNALS_SEQUENCE, null);
                    break;

                default:
                    errStr = $"\"{Name}\" - неверный тип " +
                        $"(пустая строка, HLA, HLA_VIRT, HLA_IOLINK)." +
                        $"{CommonConst.NewLine}";
                    break;
            }

            return errStr;
        }

        public override string Check()
        {
            string errors = string.Empty;
            errors += base.Check();

            if (dSubType == DeviceSubType.HLA_IOLINK)
            {
                errors += CheckSignalsSequence();
            }

            return errors;
        }

        private string CheckSignalsSequence()
        {
            string sequenceLuaName = Property.SIGNALS_SEQUENCE;
            string sequenceValue = Properties[sequenceLuaName] as string;
            if (sequenceValue == null)
            {
                return $"Не заполнена последовательность сигналов " +
                    $"{sequenceLuaName} сигнальной колонны \"{eplanName}\"." +
                    $"{CommonConst.NewLine}";
            }

            sequenceValue = sequenceValue.Trim();

            int sequenceLength = sequenceValue.Length;
            int minLength = 1;
            int maxLength = 5;
            bool wrongLength = sequenceLength < minLength ||
                sequenceLength > maxLength;
            if (wrongLength)
            {
                return $"Неправильно заполнена последовательность сигналов " +
                    $"{sequenceLuaName} сигнальной колонны \"{eplanName}\". " +
                    $"Длина последовательности должна быть от " +
                    $"{minLength} до {maxLength} символов." +
                    $"{CommonConst.NewLine}";
            }

            hasAlarm = sequenceValue.Count(x => x == 'A') == 1;
            hasBlue = sequenceValue.Count(x => x == 'B') == 1;
            hasGreen = sequenceValue.Count(x => x == 'G') == 1;
            hasYellow = sequenceValue.Count(x => x == 'Y') == 1;
            hasRed = sequenceValue.Count(x => x == 'R') == 1;

            if (hasAlarm) sequenceLength--;
            if (hasBlue) sequenceLength--;
            if (hasGreen) sequenceLength--;
            if (hasYellow) sequenceLength--;
            if (hasRed) sequenceLength--;
            bool wrongCharsInSequence = sequenceLength != 0;
            if (wrongCharsInSequence)
            {
                return $"Неправильно заполнена последовательность сигналов " +
                    $"{sequenceLuaName} сигнальной колонны \"{eplanName}\". " +
                    $"Дублирование символов или неправильные символы." +
                    $"{CommonConst.NewLine}";
            }

            return string.Empty;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.HLA:
                    switch(dst)
                    {
                        case DeviceSubType.HLA:
                            return "HLA";
                        case DeviceSubType.HLA_VIRT:
                            return "HLA_VIRT";
                        case DeviceSubType.HLA_IOLINK:
                            return "HLA_IOLINK";
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
                case DeviceType.HLA:
                    GetDeviceIOLinkProperties();
                    break;
            }

            return null;
        }

        private Dictionary<string, int> GetDeviceIOLinkProperties()
        {
            var defaultTags = new Dictionary<string, int>()
            {
                {Tag.ST, 1},
                {Tag.M, 1},
            };

            if (hasAlarm) defaultTags.Add(Tag.L_SIREN, 1);
            if (hasBlue) defaultTags.Add(Tag.L_BLUE, 1);
            if (hasGreen) defaultTags.Add(Tag.L_GREEN, 1);
            if (hasYellow) defaultTags.Add(Tag.L_YELLOW, 1);
            if (hasRed) defaultTags.Add(Tag.L_RED, 1);
            
            return defaultTags;
        }

        private bool hasAlarm;
        private bool hasBlue;
        private bool hasGreen;
        private bool hasYellow;
        private bool hasRed;
    }
}
