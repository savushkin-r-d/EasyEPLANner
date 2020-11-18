using System.Collections.Generic;

namespace InterprojectExchange
{
    /// <summary>
    /// Интерфейс модели проекта
    /// </summary>
    public interface IProjectModel
    {
        /// <summary>
        /// Имя проекта
        /// </summary>
        string ProjectName { get; set; }

        /// <summary>
        /// Выбран ли проект в списке
        /// </summary>
        bool Selected { get; set; }

        /// <summary>
        /// Сигналы-источники (отдаем)
        /// </summary>
        DeviceSignalsInfo SourceSignals { get; }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        DeviceSignalsInfo ReceiverSignals { get; }

        /// <summary>
        /// Устройства проекта
        /// </summary>
        List<DeviceInfo> Devices { get; set; }

        /// <summary>
        /// Информация о контроллере
        /// </summary>
        PacInfo PacInfo { get; set; }

        /// <summary>
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        /// <param name="projName">Имя проекта, связываемый</param>
        void AddSignal(string name, string signalType, bool receiveMode, 
            string projName);

        /// <summary>
        /// Помечена для удаления
        /// </summary>
        bool MarkedForDelete { get; set; }

        /// <summary>
        /// Файл с межконтроллерным обменом в виде списка строк
        /// </summary>
        List<string> SharedFileAsStringList { get; set; }

        /// <summary>
        /// Добавляем данные о ПЛК из Lua
        /// </summary>
        /// <param name="pacName">Имя проекта</param>
        void AddPLCData(string pacName);

        /// <summary>
        /// Добавляем данные о ПЛК из Lua 
        /// </summary>
        /// <param name="IP">IP-Адрес</param>
        /// <param name="pacName">Имя контроллера</param>
        void AddPLCData(string IP, string pacName);

        /// <summary>
        /// Добавляем данные о ПЛК из Lua
        /// </summary>
        /// <param name="emulationEnabled">Включена эмуляция</param>
        /// <param name="cycleTime">Время цикла, мс</param>
        /// <param name="timeout">Таймаут, мс</param>
        /// <param name="port">Порт</param>
        /// <param name="gateEnabled">Включен шлюз</param>
        /// <param name="station">Станция, номер</param>
        /// <param name="projectName">Имя контроллера</param>
        void AddPLCData(bool emulationEnabled, int cycleTime,
            int timeout, int port, bool gateEnabled, int station, 
            string projectName);

        /// <summary>
        /// Путь к файлам проекта.
        /// </summary>
        /// <returns></returns>
        string PathToProject { get; set; }
    }
}
