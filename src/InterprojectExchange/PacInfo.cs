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
            return new PacInfo
            {
                IP = IP,
                IPEmulator = IPEmulator,
                EmulationEnabled = EmulationEnabled,
                CycleTime = CycleTime,
                TimeOut = TimeOut,
                Port = Port,
                GateEnabled = GateEnabled,
                Station = Station,
                ModelLoaded = ModelLoaded
            };
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

        /// <summary>
        /// Модель загружена
        /// </summary>
        public bool ModelLoaded { get; set; } = false;
    }
}
