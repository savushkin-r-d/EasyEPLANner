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
            Devices = new List<DeviceDTO>();
            deviceComparer = new DeviceComparer();
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

        private DeviceComparer deviceComparer;
    }
}
