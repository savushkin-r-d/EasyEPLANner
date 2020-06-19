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
        public string ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                projectName = value;
            }
        }

        /// <summary>
        /// Устройства проекта
        /// </summary>
        public List<DeviceDTO> Devices
        {
            get
            {
                return devices;
            }
            set
            {
                devices = value;
            }
        }

        /// <summary>
        /// Пометка на удаление модели
        /// </summary>
        public bool MarkedToDelete
        {
            get
            {
                return markToDelete;
            }
            set
            {
                markToDelete = value;
            }
        }

        private bool markToDelete;
        private List<DeviceDTO> devices;
        private string projectName;
    }
}
