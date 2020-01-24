using Eplan.EplApi.DataModel;
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
                ProjectManager.GetInstance().AddLogMessage(errors);
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

        ConfigurationChecker configurationChecker;
        DeviceReader deviceReader;
        DeviceBindingReader deviceBindingReader;
        DeviceSynchronizer deviceSynchronizer;
        IOReader IOReader;

        static ProjectConfiguration instance;

    }
}
