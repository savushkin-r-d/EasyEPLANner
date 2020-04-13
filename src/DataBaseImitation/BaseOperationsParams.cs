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
        private static List<BaseProperty> WashParams()
        {
            var parameters = new List<BaseProperty>
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена"),
                new ShowedBaseProperty("DI_CIP_FREE", "МСА свободна"),
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж", 
                false),
                new ShowedBaseProperty("CIP_WASH_REQUEST", 
                "Автоматическое включение мойки"),
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Наполнение".
        /// </summary>
        /// <returns></returns>
        private static List<BaseProperty> FillParams()
        {
            var parameters = new List<BaseProperty>
            {
                new ShowedBaseProperty("OPERATION_AFTER_FILL",
                "Номер операции после наполнения")
            };

            return parameters;
        }

        /// <summary>
        /// Параметры операции "Выдача"
        /// </summary>
        /// <returns></returns>
        private static List<BaseProperty> OutParams()
        {
            var parameters = new List<BaseProperty>
            {
                new BoolShowedProperty("NEED_STORING_AFTER", 
                "Включить хранение после выдачи", "true"),
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Охлаждение"
        /// </summary>
        /// <returns></returns>
        private static List<BaseProperty> CoolingParams()
        {
            var parameters = new List<BaseProperty>
            {
                new BoolShowedProperty("ACTIVE_WORKING", "Активная работа", "false")
            };

            return parameters;
        }
    }
}
