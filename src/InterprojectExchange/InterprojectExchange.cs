using System;
using System.Collections.Generic;
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

            devices = model.Devices;

            return devices;
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

        private static InterprojectExchange interprojectExchange;
        private List<InterprojectExchangeModel> interprojectExchangeModels;
    }
}
