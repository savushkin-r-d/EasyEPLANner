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
        /// Получить пустой массив свойств.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] EmptyProperties()
        {
            return new BaseProperty[0];
        }

        /// <summary>
        /// Получить свойства базового объекта "Танк"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] TankProperties()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена"),
                new ShowedBaseProperty("DI_CIP_FREE", "МСА свободна"),
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж", 
                false),
                new ShowedBaseProperty("OPERATION_AFTER_FILL", 
                "Операция после наполнения")
            };
        }

        /// <summary>
        /// Получить свойства базового объекта "Линия"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineProperties()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена")
            };
        }

        /// <summary>
        /// Получить свойства базового объекта "Бачок"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] WaterTankProperties()
        {
            return new BaseProperty[]
            {
                new BoolShowedProperty("ACTIVE_WORKING", "Активная работа", "false"),
            };
        }

        public static BaseProperty[] POUProperties()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж",
                false)
            };
        }
    }
}
