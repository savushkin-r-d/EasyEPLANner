using System.Collections.Generic;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межпроектного обмена для каждого проекта.
    /// </summary>
    abstract public class InterprojectExchangeModel
    {
        public InterprojectExchangeModel()
        {
            Devices = new List<DeviceInfo>();
            deviceComparer = new DeviceComparer();
        }

        /// <summary>
        /// Добавляем данные об устройства из Lua
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void AddDeviceData(string name, string description)
        {
            var deviceDTO = new DeviceInfo(name, description);
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
        public virtual string ProjectName { get; set; }

        /// <summary>
        /// Устройства проекта
        /// </summary>
        public List<DeviceInfo> Devices { get; set; }

        public string PathToProject { get; set; }

        private DeviceComparer deviceComparer;
    }
}
