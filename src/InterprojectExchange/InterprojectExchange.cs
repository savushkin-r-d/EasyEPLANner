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
            interprojectExchangeModels = new List<InterprojectExchangeModel>();
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
        public void AddModel(InterprojectExchangeModel model)
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
            return Owner.LoadProjectData(pathToProjectDir);
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
        public InterprojectExchangeModel GetModel(string projName)
        {
            var model = interprojectExchangeModels
                .Where(x => x.ProjectName == projName)
                .FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Отметка выбранной модели в GUI. Другие модели снимают выбор.
        /// </summary>
        /// <param name="selectingModel">Выбранная модель</param>
        public void SelectModel(InterprojectExchangeModel selectingModel)
        {
            foreach(var model in Models)
            {
                if (model.ProjectName == selectingModel.ProjectName)
                {
                    model.Selected = true;
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
            var model = GetModel(projectName);
            if (model != null)
            {
                Models.Remove(model);
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

            InterprojectExchangeModel currentProjectModel = Models
                .Where(x => x.ProjectName == CurrentProjectName)
                .FirstOrDefault();
            InterprojectExchangeModel advancedProjectModel = Models
                .Where(x => x.Selected == true).FirstOrDefault();

            DeviceSignalsDTO currentProjectSignals;
            DeviceSignalsDTO advancedProjectSignals;
            if (editMode == EditModeEnum.SourceReciever)
            {
                currentProjectSignals = currentProjectModel.SourceSignals;
                advancedProjectSignals = advancedProjectModel.RecieverSignals;
            }
            else
            {
                currentProjectSignals = currentProjectModel.RecieverSignals;
                advancedProjectSignals = advancedProjectModel.SourceSignals;
            }

            List<string[]> AISignals = GetSignalsPairs(
                currentProjectSignals.AI, advancedProjectSignals.AI);
            signals.Add("AI", AISignals);
            List<string[]> AOSignals = GetSignalsPairs(
                currentProjectSignals.AO, advancedProjectSignals.AO);
            signals.Add("AO", AOSignals);
            List<string[]> DISignals = GetSignalsPairs(
                currentProjectSignals.DI, advancedProjectSignals.DI);
            signals.Add("DI", DISignals);
            List<string[]> DOSignals = GetSignalsPairs(
                currentProjectSignals.DO, advancedProjectSignals.DO);
            signals.Add("DO", DOSignals);

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
            for(int i = 0; i < currentProjectSignals.Count; i++)
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
            InterprojectExchangeModel currentProjectModel = Models
                .Where(x => x.ProjectName == CurrentProjectName)
                .FirstOrDefault();
            InterprojectExchangeModel advancedProjectModel = Models
                .Where(x => x.Selected == true).FirstOrDefault();

            DeviceSignalsDTO currentProjectSignals;
            DeviceSignalsDTO advancedProjectSignals;
            if (editMode == EditModeEnum.SourceReciever)
            {
                currentProjectSignals = currentProjectModel.SourceSignals;
                advancedProjectSignals = advancedProjectModel.RecieverSignals;
            }
            else
            {
                currentProjectSignals = currentProjectModel.RecieverSignals;
                advancedProjectSignals = advancedProjectModel.SourceSignals;
            }

            switch (signalType)
            {
                case "AI":
                    currentProjectSignals.AI.Add(currentProjectDevice);
                    advancedProjectSignals.AI.Add(advancedProjectDevice);
                    break;

                case "AO":
                    currentProjectSignals.AO.Add(currentProjectDevice);
                    advancedProjectSignals.AO.Add(advancedProjectDevice);
                    break;

                case "DI":
                    currentProjectSignals.DI.Add(currentProjectDevice);
                    advancedProjectSignals.DI.Add(advancedProjectDevice);
                    break;

                case "DO":
                    currentProjectSignals.DO.Add(currentProjectDevice);
                    advancedProjectSignals.DO.Add(advancedProjectDevice);
                    break;
            }

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
            InterprojectExchangeModel currentProjectModel = Models
            .Where(x => x.ProjectName == CurrentProjectName)
            .FirstOrDefault();
            InterprojectExchangeModel advancedProjectModel = Models
                .Where(x => x.Selected == true).FirstOrDefault();

            DeviceSignalsDTO currentProjectSignals;
            DeviceSignalsDTO advancedProjectSignals;
            if (editMode == EditModeEnum.SourceReciever)
            {
                currentProjectSignals = currentProjectModel.SourceSignals;
                advancedProjectSignals = advancedProjectModel.RecieverSignals;
            }
            else
            {
                currentProjectSignals = currentProjectModel.RecieverSignals;
                advancedProjectSignals = advancedProjectModel.SourceSignals;
            }

            switch (signalType)
            {
                case "AI":
                    currentProjectSignals.AI.Remove(currentProjectDevice);
                    advancedProjectSignals.AI.Remove(advancedProjectDevice);
                    break;

                case "AO":
                    currentProjectSignals.AO.Remove(currentProjectDevice);
                    advancedProjectSignals.AO.Remove(advancedProjectDevice);
                    break;

                case "DI":
                    currentProjectSignals.DI.Remove(currentProjectDevice);
                    advancedProjectSignals.DI.Remove(advancedProjectDevice);
                    break;

                case "DO":
                    currentProjectSignals.DO.Remove(currentProjectDevice);
                    advancedProjectSignals.DO.Remove(advancedProjectDevice);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Получить имя текущего проекта для формы
        /// </summary>
        /// <returns></returns>
        public string CurrentProjectName
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
        public string PathWithProjects
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
        public List<InterprojectExchangeModel> Models
        {
            get
            {
                return interprojectExchangeModels;
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
        private List<InterprojectExchangeModel> interprojectExchangeModels;
    }
}