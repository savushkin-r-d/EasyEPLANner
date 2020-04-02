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
        public static List<BaseProperty> EmptyProperties()
        {
            return new List<BaseProperty>();
        }

        /// <summary>
        /// Получить свойства базового объекта "Танк"
        /// </summary>
        /// <returns></returns>
        public static List<BaseProperty> TankProperties()
        {
            return new List<BaseProperty>
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
        public static List<BaseProperty> LineProperties()
        {
            return new List<BaseProperty>
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена")
            };
        }

        /// <summary>
        /// Получить свойства базового объекта "Бачок"
        /// </summary>
        /// <returns></returns>
        public static List<BaseProperty> WaterTankProperties()
        {
            return new List<BaseProperty>
            {
                new BoolShowedProperty("ACTIVE_WORKING", "Активная работа", "false"),
            };
        }

        /// <summary>
        /// Получить свойства базового объекта "Пастеризатор"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] POUProperties()
        {
            return new List<BaseProperty>
            {
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж",
                false),
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена")
            };
        }
    }
}
