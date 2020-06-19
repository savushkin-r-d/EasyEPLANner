using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterprojectExchange
{
    /// <summary>
    /// Объект с информацией об узле проекта
    /// </summary>
    public class PacDTO
    {
        public PacDTO()
        {
            IP = "";
            IPEmulator = "127.0.0.1";
            EmulationEnabled = false;
            CycleTime = 200;
            TimeOut = 300;
            Port = 105022;
            GateEnabled = true;
            Station = 0;
        }

        /// <summary>
        /// IP-Адрес
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// IP-Адрес эмулятора
        /// </summary>
        public string IPEmulator { get; set; }

        /// <summary>
        /// Включена ли эмуляция
        /// </summary>
        public bool EmulationEnabled { get; set; }

        /// <summary>
        /// Время цикла
        /// </summary>
        public int CycleTime { get; set; }

        /// <summary>
        /// Тайм-аут
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Опрашиваемый порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Включен ли шлюз
        /// </summary>
        public bool GateEnabled { get; set; }

        /// <summary>
        /// Номер Modbus-станции
        /// </summary>
        public int Station { get; set; }
    }
}
