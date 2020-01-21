using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Читатель привязки устройств.
    /// </summary>
    class DeviceBindingReader
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public DeviceBindingReader()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
        }

        /// <summary>
        /// Прочитать привязку.
        /// </summary>
        public void Read()
        {
            // TODO: Reading binding
            EplanDeviceManager.GetInstance().ReadConfigurationFromIOModules();
        }

        /// <summary>
        /// Менеджер устройств.
        /// </summary>
        Device.DeviceManager deviceManager;
    }
}
