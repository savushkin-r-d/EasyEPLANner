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
        public DeviceDTO(string name, string eplanName, string description,
            string type)
        {
            this.name = name;
            this.eplanName = eplanName;
            this.description = description;
            this.type = type;
        }

        /// <summary>
        /// Имя устройства А1BBB2
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Имя устройства в Eplan +A1-BB2
        /// </summary>
        public string EplanName
        {
            get
            {
                return eplanName;
            }
        }

        /// <summary>
        /// Описание устройства
        /// </summary>
        public string Description
        {
            get 
            {
                return description;
            }
        }

        /// <summary>
        /// Тип устройства (AI,TE..)
        /// </summary>
        public string Type
        {
            get
            {
                return type.ToUpper();
            }
        }

        private string name;
        private string eplanName;
        private string description;
        private string type;
    }
}
