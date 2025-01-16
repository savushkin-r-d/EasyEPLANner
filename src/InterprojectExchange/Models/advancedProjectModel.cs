using System.Collections.Generic;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межпроектного обмена альтернативного проекта.
    /// </summary>
    public class AdvancedProjectModel : InterprojectExchangeModel, IProjectModel
    {
        public AdvancedProjectModel() : base()
        {
            receiverSignals = new DeviceSignalsInfo();
            sourceSignals = new DeviceSignalsInfo();
            SharedFileAsStringList = new List<string>();
            pacDTO = new PacInfo();
        }

        /// <summary>
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        /// <param name="projectName">Имя проекта (только для интерфейса)
        /// </param>
        public void AddSignal(string name, string signalType, 
            bool receiveMode, string projectName = "")
        {
            switch(signalType)
            {
                case "AI":
                    if (receiveMode)
                    {
                        ReceiverSignals.AI.Add(name);
                    }
                    else
                    {
                        SourceSignals.AI.Add(name);
                    }
                    break;

                case "AO":
                    if (receiveMode)
                    {
                        ReceiverSignals.AO.Add(name);
                    }
                    else
                    {
                        SourceSignals.AO.Add(name);
                    }
                    break;

                case "DI":
                    if (receiveMode)
                    {
                        ReceiverSignals.DI.Add(name);
                    }
                    else
                    {
                        SourceSignals.DI.Add(name);
                    }
                    break;

                case "DO":
                    if (receiveMode)
                    {
                        ReceiverSignals.DO.Add(name);
                    }
                    else
                    {
                        SourceSignals.DO.Add(name);
                    }
                    break;
            }
        }

        /// <summary>
        /// Добавляем данные о ПЛК из Lua
        /// </summary>
        /// <param name="pacName">Имя проекта</param>
        public void AddPLCData(string pacName)
        {
            ProjectName = pacName;
        }

        /// <summary>
        /// Добавляем данные о ПЛК из Lua 
        /// </summary>
        /// <param name="IP">IP-Адрес</param>
        /// <param name="pacName">Имя контроллера</param>
        public void AddPLCData(string IP, string pacName)
        {
            PacInfo.IP = IP;
            ProjectName = pacName;
        }

        /// <summary>
        /// Добавляем данные о ПЛК из Lua
        /// </summary>
        /// <param name="emulationEnabled">Включена эмуляция</param>
        /// <param name="cycleTime">Время цикла, мс</param>
        /// <param name="timeout">Таймаут, мс</param>
        /// <param name="port">Порт</param>
        /// <param name="gateEnabled">Включен шлюз</param>
        /// <param name="station">Станция, номер</param>
        /// <param name="pacName">Имя контроллера, отсутствует, только
        /// для интерфейса</param>
        public void AddPLCData(bool emulationEnabled, int cycleTime,
            int timeout, int port, bool gateEnabled, int station, 
            string pacName = "")
        {
            if(ProjectName == null && pacName != "")
            {
                ProjectName = pacName;
            }

            PacInfo.EmulationEnabled = emulationEnabled;
            PacInfo.CycleTime = cycleTime;
            PacInfo.TimeOut = timeout;
            PacInfo.Port = port;
            PacInfo.GateEnabled = gateEnabled;
            PacInfo.Station = station;
            PacInfo.ModelLoaded = Loaded;
        }

        /// <summary>
        /// Выбрана ли модель сейчас в GUI
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// Помечена на удаление
        /// </summary>
        public bool MarkedForDelete { get; set; } = false;

        /// <summary>
        /// Файл с межконтроллерным обменом в виде списка строк
        /// </summary>
        public List<string> SharedFileAsStringList { get; set; }

        /// <summary>
        /// Сигналы-источники (отдаем)
        /// </summary>
        virtual public DeviceSignalsInfo SourceSignals
        {
            get
            {
                return sourceSignals;
            }
        }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        virtual public DeviceSignalsInfo ReceiverSignals
        {
            get
            {
                return receiverSignals;
            }
        }
        
        /// <summary>
        /// Информация о контроллере
        /// </summary>
        public PacInfo PacInfo
        {
            get
            {
                return pacDTO;
            }
            set
            {
                pacDTO = value;
            }
        }

        public bool Loaded { get; set; }

        private DeviceSignalsInfo sourceSignals;
        private DeviceSignalsInfo receiverSignals;
        private PacInfo pacDTO;
    }
}
