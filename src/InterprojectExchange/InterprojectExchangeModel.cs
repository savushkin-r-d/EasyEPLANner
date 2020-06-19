using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Модель межпроектного обмена для каждого проекта.
    /// </summary>
    public class InterprojectExchangeModel
    {
        public InterprojectExchangeModel()
        {

        }

        /// <summary>
        /// Имя проекта
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Устройства проекта
        /// </summary>
        public List<DeviceDTO> Devices { get; set; }

        /// <summary>
        /// Пометка на удаление модели
        /// </summary>
        public bool MarkedToDelete { get; set; }

        /// <summary>
        /// Путь к папке с проектом
        /// </summary>
        public string PathToProjectDir { get; set; }
    }
}
