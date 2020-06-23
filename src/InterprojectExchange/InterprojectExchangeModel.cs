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
    public class InterprojectExchangeModel
    {
        public InterprojectExchangeModel()
        {
            pacDTO = new PacDTO();
            Devices = new List<DeviceDTO>();
            deviceComparer = new DeviceComparer();
            recieverSignals = new DeviceSignalsDTO();
            sourceSignals = new DeviceSignalsDTO();
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
        /// Путь к папке с проектом
        /// </summary>
        public string PathToProjectDir { get; set; }

        /// <summary>
        /// Прочитаны ли сигналы для обмена
        /// </summary>
        public bool SharedIsReaded { get; set; } = false;

        /// <summary>
        /// Выбрана ли модель сейчас в GUI
        /// </summary>
        public bool Selected { get; set; } = false;

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

        /// <summary>
        /// Сигналы-источники (отдаем)
        /// </summary>
        public DeviceSignalsDTO SourceSignals
        {
            get
            {
                return sourceSignals;
            }
        }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        public DeviceSignalsDTO RecieverSignals
        {
            get
            {
                return recieverSignals;
            }
        }

        private DeviceSignalsDTO sourceSignals;
        private DeviceSignalsDTO recieverSignals;

        private PacDTO pacDTO;
        private DeviceComparer deviceComparer;
    }
}
