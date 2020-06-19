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
        /// Получить устройства проекта по имени
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        /// <returns></returns>
        public List<DeviceDTO> GetProjectDevices(string projectName)
        {
            var devices = new List<DeviceDTO>();

            var model = interprojectExchangeModels
                .Where(x => x.ProjectName == projectName)
                .FirstOrDefault();

            if(model != null)
            {
                devices = model.Devices;
                return devices;
            }
            else
            {
                return new List<DeviceDTO>();
            }
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
        /// Пометить модель на удаление
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        public void MarkToDelete(string projName)
        {
            var model = GetModel(projName);
            if (model != null)
            {
                model.MarkedToDelete = true;
            }
            else
            {
                throw new Exception("Модель имеет значение null");
            }
        }

        /// <summary>
        /// Получить имя текущего проекта для формы
        /// </summary>
        /// <returns></returns>
        public string GetCurrentProjectName()
        {
            return EProjectManager.GetInstance()
                .GetModifyingCurrentProjectName();
        }

        /// <summary>
        /// Получить путь к папке с проектами
        /// </summary>
        public string GetPathWithProjects
        {
            get
            {
                return ProjectManager.GetInstance().GetPtusaProjectsPath("");
            }
        }

        /// <summary>
        /// Получить модель
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        /// <returns></returns>
        private InterprojectExchangeModel GetModel(string projName)
        {
            var model = interprojectExchangeModels
                .Where(x => x.ProjectName == projName)
                .FirstOrDefault();
            return model;
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

        private InterprojectExchangeStarter interprojectExchangeStarter;
        private static InterprojectExchange interprojectExchange;
        private List<InterprojectExchangeModel> interprojectExchangeModels;
    }
}