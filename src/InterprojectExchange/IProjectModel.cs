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
        DeviceSignalsDTO SourceSignals { get; }

        /// <summary>
        /// Сигналы-приемники (принимаем)
        /// </summary>
        DeviceSignalsDTO ReceiverSignals { get; }

        /// <summary>
        /// Устройства проекта
        /// </summary>
        List<DeviceDTO> Devices { get; set; }

        /// <summary>
        /// Информация о контроллере
        /// </summary>
        PacDTO PacInfo { get; set; }

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
        /// Добавление сигнала к модели, вызывается из LUA
        /// </summary>
        /// <param name="name">Имя сигнала</param>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="receiveMode">Режим получения сигналов</param>
        void AddSignal(string name, string signalType, bool receiveMode);

        /// <summary>
        /// Помечена для удаления
        /// </summary>
        bool MarkedForDelete { get; set; }

        /// <summary>
        /// Файл с межконтроллерным обменом в виде списка строк
        /// </summary>
        List<string> SharedFileAsStringList { get; set; }
    }
}
