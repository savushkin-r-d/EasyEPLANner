using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межпроектного обмена для каждого проекта.
    /// </summary>
    abstract public class InterprojectExchangeModel
    {
        public InterprojectExchangeModel()
        {
            pacDTO = new PacDTO();
            Devices = new List<DeviceDTO>();
            deviceComparer = new DeviceComparer();
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
        public void AddPLCData(bool emulationEnabled, int cycleTime, 
            int timeout, int port, bool gateEnabled, int station)
        {
            PacInfo.EmulationEnabled = emulationEnabled;
            PacInfo.CycleTime = cycleTime;
            PacInfo.TimeOut = timeout;
            PacInfo.Port = port;
            PacInfo.GateEnabled = gateEnabled;
            PacInfo.Station = station;
        }

        /// <summary>
        /// Добавляем данные об устройства из Lua
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void AddDeviceData(string name, string description)
        {
            var deviceDTO = new DeviceDTO(name, description);
            Devices.Add(deviceDTO);
        }

        /// <summary>
        /// Сортировка устройств из Lua
        /// </summary>
        public void SortDeviceData()
        {
            Devices.Sort(deviceComparer);
        }

        /// <summary>
        /// Имя проекта
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Устройства проекта
        /// </summary>
        public List<DeviceDTO> Devices { get; set; }


        /// <summary>
        /// Информация о контроллере
        /// </summary>
        public PacDTO PacInfo
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


        private PacDTO pacDTO;
        private DeviceComparer deviceComparer;
    }
}
