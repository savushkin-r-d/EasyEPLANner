using System.Collections.Generic;

namespace InterprojectExchange
{
    /// <summary>
    /// Обмен сигналами между контроллерами. Обмен с формами
    /// </summary>
    public interface IInterprojectExchange
    {
        /// <summary>
        /// Путь к папке с проектами
        /// </summary>
        string DefaultPathWithProjects { get; }

        /// <summary>
        /// Имена каналов для устройств
        /// </summary>
        string[] DeviceChannelsNames { get; }

        /// <summary>
        /// Режим редактирования
        /// </summary>
        EditMode EditMode { get; }

        /// <summary>
        /// Имена загруженных альтернативных моделей
        /// </summary>
        string[] LoadedAdvancedModelNames { get; }

        /// <summary>
        /// Главная модель (текущий проект)
        /// </summary>
        ICurrentProjectModel MainModel { get; }

        /// <summary>
        /// Имя текущего проекта
        /// </summary>
        string MainProjectName { get; }

        /// <summary>
        /// Все модели обмена сигналами
        /// </summary>
        List<IProjectModel> Models { get; }

        /// <summary>
        /// Класс-владелец
        /// </summary>
        InterprojectExchangeStarter Owner { get; set; }

        /// <summary>
        /// Выбранная в GUI модель альтернативного проекта
        /// </summary>
        IProjectModel SelectedModel { get; }

        /// <summary>
        /// Добавить модель
        /// </summary>
        /// <param name="model">Модель</param>
        void AddModel(IProjectModel model);

        /// <summary>
        /// Связать сигналы
        /// </summary>
        /// <param name="signalType">Тип сигнала текущего проекта</param>
        /// <param name="currentProjectDevice">Устройство текущего проекта
        /// </param>
        /// <param name="advancedProjectDevice">Устройство альтернативного проекта</param>
        bool BindSignals(string signalType, string currentProjectDevice, string advancedProjectDevice);

        /// <summary>
        /// Изменить режим редактирования связей
        /// </summary>
        void ChangeEditMode(int selectedModeIndex);

        /// <summary>
        /// Проверка связанных сигналов при открытии проекта
        /// </summary>
        string CheckBindingSignals();

        /// <summary>
        /// Проверка корректности пути к файлам проекта
        /// </summary>
        /// <param name="path">Путь к файлам проекта</param>
        bool CheckPathToProjectFiles(string path);

        /// <summary>
        /// Очистка обмена между проектами
        /// </summary>
        void Clear();

        /// <summary>
        /// Удалить обмен с проектом
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        void DeleteExchangeWithProject(string projectName);

        /// <summary>
        /// Удаление связи между сигналами
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="currentProjectDevice">Устройство текущего проекта
        /// </param>
        /// <param name="advancedProjectDevice">Устройство альтернативного проекта</param>
        bool DeleteSignalsBind(string signalType, string currentProjectDevice, string advancedProjectDevice);

        /// <summary>
        /// Получить сигналы активных проектов для вставки в список связанных сигналов
        /// </summary>
        Dictionary<string, List<string[]>> GetBindedSignals();

        /// <summary>
        /// Получить модель
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        IProjectModel GetModel(string projName);

        /// <summary>
        /// Загрузка данных проекта (вызывает событие)
        /// </summary>
        /// <param name="pathToProjectDir">Путь к папке с файлами проекта</param>
        /// <param name="errors">Ошибки возникшие при загрузке</param>
        /// <returns>Успешно или не успешно загружены данные</returns>
        bool LoadProjectData(string pathToProjectDir, out string errors);

        /// <summary>
        /// Подвинуть уже привязанные сигналы
        /// </summary>
        /// <param name="signalType">Группа (тип сигналов)</param>
        /// <param name="currProjSignal">Выбранный сигнал текущего проекта </param>
        /// <param name="advProjSignal">Выбранный сигнал альтернативного проекта </param>
        /// <param name="move">Индекс сдвига (1 - вниз, -1 - вверх)</param>
        bool MoveSignalsBind(string signalType, string currProjSignal, string advProjSignal, int move);

        /// <summary>
        /// Восстановить модель
        /// </summary>
        /// <param name="projectName">Имя проекта для проверки</param>
        /// <returns>Возможно или нет это действие</returns>
        bool RestoreModel(string projectName);

        /// <summary>
        /// Сохранение межконтроллерного обмена
        /// </summary>
        void Save();

        /// <summary>
        /// Отметка выбранной модели в GUI. Другие модели снимают выбор.
        /// </summary>
        /// <param name="selectingModel">Выбранная модель</param>
        void SelectModel(IProjectModel selectingModel);

        /// <summary>
        /// Изменение устройства в связи текущего проекта
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        /// <param name="mainProject">Редактируется главный проект или нет</param>
        /// <param name="needSwap">Нужно ли поменять старые и новые значения местами</param>
        bool UpdateProjectBinding(string signalType, string oldValue, string newValue, bool mainProject, out bool needSwap);
    }
}