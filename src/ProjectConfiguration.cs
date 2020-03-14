﻿using Eplan.EplApi.DataModel;
using System.Collections.Generic;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс-фасад для работы с конфигурацией проекта
    /// </summary>
    public class ProjectConfiguration
    {
        /// <summary>
        /// Закрытый конструктор.
        /// </summary>
        private ProjectConfiguration()
        {
            this.configurationChecker = new ConfigurationChecker();
            this.deviceReader = new DeviceReader();
            this.deviceBindingReader = new DeviceBindingReader();
            this.deviceSynchronizer = new DeviceSynchronizer();
            this.IOReader = new IOReader();

            StartingIPInterval = 0;
            EndingIPInterval = 0;
        }

        /// <summary>
        /// Получить экземпляр класса.
        /// </summary>
        public static ProjectConfiguration GetInstance()
        {
            if (instance == null)
            {
                instance = new ProjectConfiguration();
            }

            return instance;
        }
  
        /// <summary>
        /// Синхронизация устройств в проекте с уже имеющимся описанием.
        /// </summary>
        public void SynchronizeDevices()
        {
            deviceSynchronizer.Synchronize();
        }

        /// <summary>
        /// Чтение конфигурации устройств.
        /// </summary>
        public void ReadDevices() 
        {
            deviceReader.Read();
        }

        /// <summary>
        /// Чтение конфигурации узлов и модулей ввода-вывода.
        /// </summary>
        public void ReadIO() 
        {
            IOReader.Read();
        }

        /// <summary>
        /// Чтение привязки устройств к модулям ввода-вывода.
        /// </summary>
        public void ReadBinding() 
        {
            deviceBindingReader.Read();
        }

        /// <summary>
        /// Возвращает словарь, содержащий привязку конкретного канала для
        /// его сброса
        /// </summary>
        public Dictionary<string, string> GetBindingForResettingChannel(
            Function deviceClampFunction, IO.IOModuleInfo moduleInfo, 
            string devicesDescription = "")
        {
            Dictionary<string, string> binding = deviceReader
                .GetBindingForResettingChannel(deviceClampFunction,
                moduleInfo, devicesDescription);
            return binding;
        }

        /// <summary>
        /// Проверка конфигурации.
        /// </summary>
        /// <param name="silentMode">Тихий режим (без окна логов)</param>
        public void Check(bool silentMode = false) 
        {
            configurationChecker.Check();

            string errors = configurationChecker.Errors;
            if (errors != "" && silentMode == false)
            {
                Logs.AddMessage(errors);
            }
        }

        /// <summary>
        /// Свойство, указывающее прочитаны устройства или нет.
        /// </summary>
        public bool DevicesIsRead
        {
            get
            {
                return deviceReader.DevicesIsRead;
            }
        }

        /// <summary>
        /// Начальный интервал IP-адресов проекта.
        /// </summary>
        public long StartingIPInterval { get; set; }

        /// <summary>
        /// Конечный интервал IP-адресов проекта.
        /// </summary>
        public long EndingIPInterval { get; set; }

        /// <summary>
        /// Сбросить интервал IP-адресов проекта.
        /// </summary>
        public void ResetIPAddressesInterval()
        {
            StartingIPInterval = 0;
            EndingIPInterval = 0;
        }

        ConfigurationChecker configurationChecker;
        DeviceReader deviceReader;
        DeviceBindingReader deviceBindingReader;
        DeviceSynchronizer deviceSynchronizer;
        IOReader IOReader;

        static ProjectConfiguration instance;
    }
}
