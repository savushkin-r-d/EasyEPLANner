using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
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
        /// Загрузить модель
        /// </summary>
        /// <param name="projName">Имя проекта</param>
        /// <param name="devices">Устройства</param>
        public void LoadModel(string projName, List<DeviceDTO> devices)
        {
            var model = new InterprojectExchangeModel();
            model.Devices = devices;
            model.ProjectName = projName;
            interprojectExchangeModels.Add(model);
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
        /// Загрузить данные проекта
        /// </summary>
        /// <param name="pathToFiles">Путь к проекту</param>
        /// <returns></returns>
        public bool LoadProjectData(string pathToFiles)
        {
            var res = false;

            //TODO: Загрузка всех данных по проекту (shared, io)

            return res;
        }

        /// <summary>
        /// Проверка корректности пути к файлам проекта
        /// </summary>
        /// <param name="path">Путь к файлам проекта</param>
        /// <returns></returns>
        public bool CheckPathToProjectFiles(string path)
        {
            bool isOk = false;

            string fileWithDevicesPath = Path.Combine(path, 
                fileWithDevicesName);
            if (File.Exists(fileWithDevicesPath))
            {
                isOk = true;
            }

            return isOk;
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

        private string fileWithDevicesName = "main.io.lua";
        private string sharedFile = "shared.lua";

        private static InterprojectExchange interprojectExchange;
        private List<InterprojectExchangeModel> interprojectExchangeModels;
    }
}
