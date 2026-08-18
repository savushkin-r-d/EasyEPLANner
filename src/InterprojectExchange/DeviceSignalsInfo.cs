using System.Collections.Generic;
using System.Linq;

namespace InterprojectExchange
{
    /// <summary>
    /// Класс содержащий сигналы для модели межконтроллерного обмена
    /// </summary>
    public class DeviceSignalsInfo
    {
        /// <summary>
        /// Маркер несвязанного сигнала при несовпадении количества каналов
        /// </summary>
        public const string UnpairedSignal = "-";

        public DeviceSignalsInfo()
        {
            AISignals = new List<string>();
            AOSignals = new List<string>();
            DISignals = new List<string>();
            DOSignals = new List<string>();
        }

        /// <summary>
        /// Список AI сигналов
        /// </summary>
        public List<string> AI
        {
            get
            {
                return AISignals;
            }
        }

        /// <summary>
        /// Список AO сигналов
        /// </summary>
        public List<string> AO
        {
            get
            {
                return AOSignals;
            }
        }

        /// <summary>
        /// Список DI сигналов
        /// </summary>
        public List<string> DI
        {
            get
            {
                return DISignals;
            }
        }

        /// <summary>
        /// Список DO сигналов
        /// </summary>
        public List<string> DO
        {
            get
            {
                return DOSignals;
            }
        }

        public int Count
        {
            get
            {
                return AISignals.Count + AOSignals.Count +
                    DISignals.Count + DOSignals.Count;
            }
        }

        /// <summary>
        /// Проверка соответсвия количества каналов привязки
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// Список типов каналов с несоответсвующим количеством
        /// </returns>
        public string CountCompare(DeviceSignalsInfo other)
        {
            var errorsCahnnels = new List<string>();

            if (AO.Count != other.AO.Count)
                errorsCahnnels.Add("AO");
            if (AI.Count != other.AI.Count)
                errorsCahnnels.Add("AI");
            if (DO.Count != other.DO.Count)
                errorsCahnnels.Add("DO");
            if (DI.Count != other.DI.Count)
                errorsCahnnels.Add("DI");

            return string.Join(", ", errorsCahnnels);
        }

        /// <summary>
        /// Есть ли несвязанные сигналы (плейсхолдеры)
        /// </summary>
        public bool ContainsUnpairedSignals()
        {
            return AI.Contains(UnpairedSignal) ||
                AO.Contains(UnpairedSignal) ||
                DI.Contains(UnpairedSignal) ||
                DO.Contains(UnpairedSignal);
        }

        /// <summary>
        /// Удалить плейсхолдеры несвязанных сигналов
        /// </summary>
        public void StripUnpairedSignals()
        {
            AI.RemoveAll(s => s == UnpairedSignal);
            AO.RemoveAll(s => s == UnpairedSignal);
            DI.RemoveAll(s => s == UnpairedSignal);
            DO.RemoveAll(s => s == UnpairedSignal);
        }

        /// <summary>
        /// Развести сигналы по отдельным строкам, если количество не совпадает.
        /// Каждая сторона получает сигнал с "-" напротив.
        /// </summary>
        public static void ExpandMismatchedChannels(DeviceSignalsInfo left,
            DeviceSignalsInfo right)
        {
            ExpandChannel(left.AI, right.AI);
            ExpandChannel(left.AO, right.AO);
            ExpandChannel(left.DI, right.DI);
            ExpandChannel(left.DO, right.DO);
        }

        private static void ExpandChannel(List<string> left, List<string> right)
        {
            if (left.Count == right.Count)
            {
                return;
            }

            var leftCopy = left.ToList();
            var rightCopy = right.ToList();
            left.Clear();
            right.Clear();

            foreach (var signal in leftCopy)
            {
                left.Add(signal);
                right.Add(UnpairedSignal);
            }

            foreach (var signal in rightCopy)
            {
                left.Add(UnpairedSignal);
                right.Add(signal);
            }
        }

        /// <summary>
        /// Является ли имя маркером несвязанного сигнала
        /// </summary>
        public static bool IsUnpaired(string signal)
        {
            return signal == UnpairedSignal;
        }

        private List<string> AISignals;
        private List<string> AOSignals;
        private List<string> DISignals;
        private List<string> DOSignals;
    }
}
