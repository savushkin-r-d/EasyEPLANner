using System;
using System.Collections.Generic;
using System.Linq;
using EasyEPlanner;

namespace InterprojectExchange
{
    /// <summary>
    /// Межконтроллерный обмен сигналами. Обмен с формами
    /// </summary>
    public class InterprojectExchange
    {
        private InterprojectExchange()
        {
            interprojectExchangeModels = new List<IProjectModel>();
        }

        /// <summary>
        /// Очистка обмена между проектами
        /// </summary>
        public void Clear()
        {
            interprojectExchangeStarter = null;
            interprojectExchangeModels.Clear();
        }

        /// <summary>
        /// Добавить модель
        /// </summary>
        /// <param name="model">Модель</param>
        public void AddModel(IProjectModel model)
        {
            interprojectExchangeModels.Add(model);
        }

        /// <summary>
        /// Загрузка данных проекта (вызывает событие)
        /// </summary>
        /// <param name="pathToProjectDir">Путь к папке с файлами проекта</param>
        /// <returns></returns>
        public bool LoadProjectData(string pathToProjectDir)
        {
            // Генерация пути к папке с проектами и его имени из полного пути
            string[] splittedPath = pathToProjectDir.Split('\\');
            int lastElem = splittedPath.Length - 1;
            string projName = splittedPath[lastElem];
            string pathToProjectsDir = pathToProjectDir.Replace(projName, "");
            return Owner.LoadProjectData(pathToProjectsDir, projName);
        }

        /// <summary>
        /// Проверка корректности пути к файлам проекта
        /// </summary>
        /// <param name="path">Путь к файлам проекта</param>
        /// <returns></returns>
        public bool CheckPathToProjectFiles(string path)
        {
            return Owner.CheckProjectData(path);
        }

        /// <summary>
        /// Получить модель
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        public IProjectModel GetModel(string projName)
        {
            IProjectModel model = interprojectExchangeModels
                .Where(x => x.ProjectName == projName)
                .FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Отметка выбранной модели в GUI. Другие модели снимают выбор.
        /// </summary>
        /// <param name="selectingModel">Выбранная модель</param>
        public void SelectModel(IProjectModel selectingModel)
        {
            CurrentProjectModel currentProjectModel = MainModel;

            foreach(var model in Models)
            {
                if (model.ProjectName == selectingModel.ProjectName)
                {
                    model.Selected = true;
                    currentProjectModel.SelectedAdvancedProject = model
                        .ProjectName;
                }
                else
                {
                    model.Selected = false;
                }
            }
        }

        /// <summary>
        /// Удалить обмен с проектом
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        public void DeleteExchangeWithProject(string projectName)
        {
            IProjectModel model = GetModel(projectName);
            if (model != null)
            {
                model.MarkedForDelete = true;
            }
            else
            {
                throw new Exception("Ошибка при удалении связи с проектом");
            }
        }

        /// <summary>
        /// Изменить режим редактирования связей
        /// </summary>
        public void ChangeEditMode(int selectedModeIndex)
        {
            editMode = (EditModeEnum)selectedModeIndex;
        }

        /// <summary>
        /// Получить сигналы активных проектов для вставки в список связанных
        /// сигналов
        /// </summary>
        public Dictionary<string, List<string[]>> GetBindedSignals()
        {
            var signals = new Dictionary<string, List<string[]>>();

            foreach(var channelName in DeviceChannelsNames)
            {
                List<string[]> channelSignals = GetSignalsPairs(
                    GetCurrentProjectSignals(channelName), 
                    GetAdvancedProjectSignals(channelName));
                signals.Add(channelName, channelSignals);
            }

            return signals;
        }

        /// <summary>
        /// Получить связанные сигналы
        /// </summary>
        /// <param name="currentProjectSignals">Список сигналов текущего проекта
        /// </param>
        /// <param name="advancedProjectSignals">Список сигналов альтернативного
        /// проекта</param>
        /// <returns></returns>
        private List<string[]> GetSignalsPairs(
            List<string> currentProjectSignals, 
            List<string> advancedProjectSignals)
        {
            var result = new List<string[]>();
            if (currentProjectSignals.Count > 0 &&
                advancedProjectSignals.Count > 0)
            {
                for (int i = 0; i < currentProjectSignals.Count; i++)
                {
                    var bindedSignals = new string[]
                    {
                        currentProjectSignals[i],
                        advancedProjectSignals[i]
                    };
                    result.Add(bindedSignals);
                }
                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Связать сигналы
        /// </summary>
        /// <param name="signalType">Тип сигнала текущего проекта</param>
        /// <param name="currentProjectDevice">Устройство текущего проекта
        /// </param>
        /// <param name="advancedProjectDevice">Устройство альтернативного 
        /// проекта</param>
        public bool BindSignals(string signalType, string currentProjectDevice,
            string advancedProjectDevice)
        {
            List<string> currentProjSignals = GetCurrentProjectSignals(
                signalType);
            List<string> advancedProjSignals = GetAdvancedProjectSignals(
                signalType);

            currentProjSignals.Add(currentProjectDevice);
            advancedProjSignals.Add(advancedProjectDevice);

            return true;
        }

        /// <summary>
        /// Удаление связи между сигналами
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="currentProjectDevice">Устройство текущего проекта
        /// </param>
        /// <param name="advancedProjectDevice">Устройство альтернативного
        /// проекта</param>
        /// <returns></returns>
        public bool DeleteSignalsBind(string signalType,
            string currentProjectDevice, string advancedProjectDevice)
        {
            List<string> currentProjSignals = GetCurrentProjectSignals(
                signalType);
            List<string> advancedProjSignals = GetAdvancedProjectSignals(
                signalType);

            currentProjSignals.Remove(currentProjectDevice);
            advancedProjSignals.Remove(advancedProjectDevice);

            return true;
        }

        /// <summary>
        /// Изменение устройства в связи текущего проекта
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        /// <returns></returns>
        public bool UpdateProjectBinding(string signalType, 
            string oldValue, string newValue, bool mainProject)
        {
            List<string> signals;
            if (mainProject)
            {
                signals = GetCurrentProjectSignals(signalType);
            }
            else
            {
                signals = GetAdvancedProjectSignals(signalType);
            }

            int oldValueIndex = signals.IndexOf(oldValue);
            signals.RemoveAt(oldValueIndex);
            signals.Insert(oldValueIndex, newValue);

            return true;
        }

        /// <summary>
        /// Получить сигналы текущего проекта
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <returns></returns>
        private List<string> GetCurrentProjectSignals(string signalType)
        {
            IProjectModel currentProjectModel = MainModel;
            DeviceSignalsInfo currentProjectSignals;
            if (editMode == EditModeEnum.SourceReciever)
            {
                currentProjectSignals = currentProjectModel.SourceSignals;
            }
            else
            {
                currentProjectSignals = currentProjectModel.ReceiverSignals;
            }

            switch (signalType)
            {
                case "AI":
                    return currentProjectSignals.AI;

                case "AO":
                    return currentProjectSignals.AO;

                case "DI":
                    return currentProjectSignals.DI;

                case "DO":
                    return currentProjectSignals.DO;
            }

            return new List<string>();
        }

        /// <summary>
        /// Получить сигналы альтернативного проекта
        /// </summary>
        /// <param name="signalType">Тип сигнала</param>
        /// <returns></returns>
        private List<string> GetAdvancedProjectSignals(string signalType)
        {
            IProjectModel advancedProjectModel = SelectedModel;
            DeviceSignalsInfo advancedProjectSignals;
            if (editMode == EditModeEnum.SourceReciever)
            {
                advancedProjectSignals = advancedProjectModel.ReceiverSignals;
            }
            else
            {
                advancedProjectSignals = advancedProjectModel.SourceSignals;
            }

            switch (signalType)
            {
                case "AI":
                    return advancedProjectSignals.AI;

                case "AO":
                    return advancedProjectSignals.AO;

                case "DI":
                    return advancedProjectSignals.DI;

                case "DO":
                    return advancedProjectSignals.DO;
            }

            return new List<string>();
        }

        /// <summary>
        /// Сохранение межконтроллерного обмена
        /// </summary>
        public void Save()
        {
            Owner.Save();
        }

        /// <summary>
        /// Восстановить модель
        /// </summary>
        /// <param name="projectName">Имя проекта для проверки</param>
        /// <returns>Возможно или нет это действие</returns>
        public bool RestoreModel(string projectName)
        {
            var canRestore = false;

            foreach(var model in Models)
            {
                bool foundMarkedModel = 
                    projectName == model.ProjectName && model.MarkedForDelete;
                if (foundMarkedModel)
                {
                    canRestore = true;
                    model.MarkedForDelete = false;
                }
            }

            return canRestore;
        }

        /// <summary>
        /// Имена загруженных альтернативных моделей
        /// </summary>
        public string[] LoadedAdvancedModelNames
        {
            get
            {
                return interprojectExchange.Models
                    .Where(x => x.ProjectName != interprojectExchange
                    .MainProjectName &&
                    x.MarkedForDelete == false)
                    .Select(x => x.ProjectName)
                    .ToArray();
            }
        }

        /// <summary>
        /// Главная модель
        /// </summary>
        public CurrentProjectModel MainModel 
        { 
            get
            {
                return GetModel(MainProjectName) as CurrentProjectModel;
            } 
        }

        /// <summary>
        /// Получить имя текущего проекта для формы
        /// </summary>
        /// <returns></returns>
        public string MainProjectName
        {
            get
            {
                return EProjectManager.GetInstance()
                .GetModifyingCurrentProjectName();
            }
        }

        /// <summary>
        /// Получить путь к папке с проектами
        /// </summary>
        public string DefaultPathWithProjects
        {
            get
            {
                return ProjectManager.GetInstance().GetPtusaProjectsPath("");
            }
        }

        /// <summary>
        /// Класс-владелец
        /// </summary>
        public InterprojectExchangeStarter Owner
        {
            get
            {
                return interprojectExchangeStarter;
            }
            set
            {
                interprojectExchangeStarter = value;
            }
        }

        /// <summary>
        /// Модели с данными по проектам
        /// </summary>
        public List<IProjectModel> Models
        {
            get
            {
                return interprojectExchangeModels;
            }
        }

        /// <summary>
        /// Выбранная в GUI модель альтернативного проекта
        /// </summary>
        public IProjectModel SelectedModel
        {
            get
            {
                return Models.Where(x => x.Selected == true).FirstOrDefault();
            }
        }

        /// <summary>
        /// Имена каналов для устройств
        /// </summary>
        public string[] DeviceChannelsNames
        {
            get
            {
                return new string[] { "AI", "AO", "DI", "DO" };
            }
        }

        /// <summary>
        /// Получить экземпляр класса. Singleton
        /// </summary>
        /// <returns></returns>
        public static InterprojectExchange GetInstance()
        {
            if (interprojectExchange == null)
            {
                interprojectExchange = new InterprojectExchange();
            }

            return interprojectExchange;
        }

        /// <summary>
        /// Режим редактирования связей
        /// </summary>
        enum EditModeEnum
        {
            SourceReciever, // Источник >> Приемник
            RecieverSource  // Приемник >> Источник
        }

        private EditModeEnum editMode;
        private InterprojectExchangeStarter interprojectExchangeStarter;
        private static InterprojectExchange interprojectExchange;
        private List<IProjectModel> interprojectExchangeModels;
    }
}