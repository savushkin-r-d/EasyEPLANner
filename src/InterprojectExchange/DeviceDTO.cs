using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Объект для обмена информацией об устройства
    /// </summary>
    public class DeviceDTO
    {
        public DeviceDTO(string name, string eplanName, string description)
        {
            this.name = name;
            this.eplanName = eplanName;
            this.description = description;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string EplanName
        {
            get
            {
                return eplanName;
            }
        }

        public string Description
        {
            get 
            {
                return description;
            }
        }

        private string name;
        private string eplanName;
        private string description;
    }
}
