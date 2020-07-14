namespace InterprojectExchange
{
    /// <summary>
    /// Объект с информацией об узле проекта
    /// </summary>
    public class PacInfo
    {
        public PacInfo()
        {
            IP = "";
            IPEmulator = "127.0.0.1";
            EmulationEnabled = false;
            CycleTime = 200;
            TimeOut = 300;
            Port = 10502;
            GateEnabled = true;
            Station = -1;
        }

        /// <summary>
        /// Клонирование объекта
        /// </summary>
        /// <returns></returns>
        public PacInfo Clone()
        {
            var cloned = new PacInfo();

            cloned.IP = IP;
            cloned.IPEmulator = IPEmulator;
            cloned.EmulationEnabled = EmulationEnabled;
            cloned.CycleTime = CycleTime;
            cloned.TimeOut = TimeOut;
            cloned.Port = Port;
            cloned.GateEnabled = GateEnabled;
            cloned.Station = Station;

            return cloned;
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
