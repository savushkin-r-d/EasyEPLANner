using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterprojectExchange
{
    /// <summary>
    /// Класс содержащий сигналы для модели межконтроллерного обмена
    /// </summary>
    public class DeviceSignalsInfo
    {
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

        private List<string> AISignals;
        private List<string> AOSignals;
        private List<string> DISignals;
        private List<string> DOSignals;
    }
}
