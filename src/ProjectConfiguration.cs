using Eplan.EplApi.DataModel;
using Eplan.EplApi.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StaticHelper;

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
            this.deviceSynchronizer = new DeviceSynchronizer();
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

        }

        /// <summary>
        /// Чтение привязки устройств к модулям ввода-вывода.
        /// </summary>
        public void ReadBinding() 
        {

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
            if (errors != string.Empty && silentMode == false)
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
        DeviceSynchronizer deviceSynchronizer;

        static ProjectConfiguration instance;

    }
}
