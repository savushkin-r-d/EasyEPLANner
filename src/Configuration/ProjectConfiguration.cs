using Eplan.EplApi.DataModel;
using System.Collections.Generic;
using StaticHelper;
using EplanDevice;
using System.Linq;

namespace EasyEPlanner
{
    public interface IProjectConfiguration
    {
        /// <summary>
        /// Интервалы IP-адресов проекта.
        /// </summary>
        (long, long)[] RangesIP { get; set; }

        /// <summary>
        /// Сбросить интервал IP-адресов проекта.
        /// </summary>
        void ResetIPAddressesInterval();
    }

    /// <summary>
    /// Класс-фасад для работы с конфигурацией проекта
    /// </summary>
    public class ProjectConfiguration : IProjectConfiguration
    {
        /// <summary>
        /// Закрытый конструктор.
        /// </summary>
        private ProjectConfiguration()
        {
            IApiHelper apiHelper = new ApiHelper();
            IProjectHelper projectHelper = new ProjectHelper(apiHelper);
            IIOHelper ioHelper = new IOHelper(projectHelper);
            IDeviceHelper deviceHelper = new DeviceHelper(apiHelper);

            this.configurationChecker = new ConfigurationChecker(projectHelper, new ProjectHealthChecker(), this);
            this.deviceReader = new DeviceReader(apiHelper, deviceHelper, projectHelper, ioHelper,
                DeviceManager.GetInstance());
            this.deviceBindingReader = new DeviceBindingReader(projectHelper, apiHelper);
            this.deviceSynchronizer = new DeviceSynchronizer(deviceReader);
            this.IOReader = new IOReader(projectHelper, deviceHelper);

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
        /// Проверка принадлежности ip адреса к диапазону адресов проекта.
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>
        ///  true if belong to range
        /// </returns>
        public bool BelongToRangesIP(long ip)
        {
            return RangesIP?.Any(range => ip >= range.Item1 && ip <= range.Item2) ?? true;
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

        public (long, long)[] RangesIP { get; set; }

        public void ResetIPAddressesInterval()
        {
            StartingIPInterval = 0;
            EndingIPInterval = 0;
            RangesIP = null;
        }

        ConfigurationChecker configurationChecker;
        DeviceReader deviceReader;
        DeviceBindingReader deviceBindingReader;
        DeviceSynchronizer deviceSynchronizer;
        IOReader IOReader;

        static ProjectConfiguration instance;
    }
}
