using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межпроектного обмена текущего проекта.
    /// </summary>
    public class CurrentProjectModel : InterprojectExchangeModel, IProjectModel
    {
        public CurrentProjectModel()
        {
            receiverSignals = new Dictionary<string, DeviceSignalsDTO>();
            sourceSignals = new Dictionary<string, DeviceSignalsDTO>();
            SharedFileAsStringList = new List<string>();
            pacDTOs = new Dictionary<string, PacDTO>();
        }

        /// <summary>
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        /// <param name="projName">Имя проекта, связываемый</param>
        public void AddSignal(string name, string signalType, 
            bool receiveMode, string projName)
        {
            if(!receiverSignals.ContainsKey(projName))
            {
                receiverSignals.Add(projName, new DeviceSignalsDTO());
            }
            if(!sourceSignals.ContainsKey(projName))
            {
                sourceSignals.Add(projName, new DeviceSignalsDTO());
            }

            switch(signalType)
            {
                case "AI":
                    if (receiveMode)
                    {
                        receiverSignals[projName].AI.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].AI.Add(name);
                    }
                    break;

                case "AO":
                    if (receiveMode)
                    {
                        receiverSignals[projName].AO.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].AO.Add(name);
                    }
                    break;

                case "DI":
                    if (receiveMode)
                    {
                        receiverSignals[projName].DI.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].DI.Add(name);
                    }
                    break;

                case "DO":
                    if (receiveMode)
                    {
                        receiverSignals[projName].DO.Add(name);
                    }
                    else
                    {
                        sourceSignals[projName].DO.Add(name);
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
            if(ProjectName == null)
            {
                ProjectName = pacName;
            }

            if (!pacDTOs.ContainsKey(pacName))
            {
                pacDTOs.Add(pacName, new PacDTO());
            }
            pacDTOs[pacName].IP = IP;
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
        /// <param name="pacName">Имя ПЛК</param>
        public void AddPLCData(bool emulationEnabled, int cycleTime,
            int timeout, int port, bool gateEnabled, int station, 
            string pacName)
        {
            if (!pacDTOs.ContainsKey(pacName))
            {
                pacDTOs.Add(pacName, new PacDTO());
            }

            if (ProjectName == null)
            {
                ProjectName = pacName;
            }

            pacDTOs[pacName].EmulationEnabled = emulationEnabled;
            pacDTOs[pacName].CycleTime = cycleTime;
            pacDTOs[pacName].TimeOut = timeout;
            pacDTOs[pacName].Port = port;
            pacDTOs[pacName].GateEnabled = gateEnabled;
            pacDTOs[pacName].Station = station;
        }

        /// <summary>
        /// Выбран ли проект в списке (здесь будет всегда false)
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// Альтернативный проект, который выбран для обмена с текущим
        /// </summary>
        public string SelectedAdvancedProject { get; set; }

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
        public DeviceSignalsDTO SourceSignals
        {
            get
            {
                if (!sourceSignals.ContainsKey(SelectedAdvancedProject))
                {
                    sourceSignals.Add(SelectedAdvancedProject,
                        new DeviceSignalsDTO());
                }
                return sourceSignals[SelectedAdvancedProject];
            }
        }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        public DeviceSignalsDTO ReceiverSignals
        {
            get
            {
                if(!receiverSignals.ContainsKey(SelectedAdvancedProject))
                {
                    receiverSignals.Add(SelectedAdvancedProject, 
                        new DeviceSignalsDTO());
                }
                return receiverSignals[SelectedAdvancedProject];
            }
        }

        public PacDTO PacInfo
        {
            get
            {
                if(!pacDTOs.ContainsKey(SelectedAdvancedProject))
                {
                    pacDTOs.Add(SelectedAdvancedProject, new PacDTO());
                }
                return pacDTOs[SelectedAdvancedProject];
            }
            set
            {
                if (!pacDTOs.ContainsKey(SelectedAdvancedProject))
                {
                    pacDTOs.Add(SelectedAdvancedProject, new PacDTO());
                }
                pacDTOs[SelectedAdvancedProject] = value;
            }
        }

        private Dictionary<string, DeviceSignalsDTO> sourceSignals;
        private Dictionary<string, DeviceSignalsDTO> receiverSignals;
        private Dictionary<string, PacDTO> pacDTOs;
    }
}
