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

        private List<DeviceDTO> devices;
        private string projectName;
    }
}
