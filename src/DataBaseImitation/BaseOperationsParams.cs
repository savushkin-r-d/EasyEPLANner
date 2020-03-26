using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace DataBase
{
    public partial class Imitation
    {
        /// <summary>
        /// Получить параметры операции "Мойка".
        /// </summary>
        /// <returns></returns>
        private static BaseProperty[] WashParams()
        {
            var parameters = new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена"),
                new ShowedBaseProperty("DI_CIP_FREE", "МСА свободна"),
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж", 
                false)
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Наполнение".
        /// </summary>
        /// <returns></returns>
        private static BaseProperty[] FillParams()
        {
            var parameters = new BaseProperty[]
            {
                new ShowedBaseProperty("OPERATION_AFTER_FILL",
                "Номер операции после наполнения")
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Охлаждение"
        /// </summary>
        /// <returns></returns>
        private static BaseProperty[] CoolingParams()
        {
            var parameters = new BaseProperty[]
            {
                new BoolShowedProperty("ACTIVE_WORKING", "Активная работа", "false")
            };

            return parameters;
        }
    }
}
