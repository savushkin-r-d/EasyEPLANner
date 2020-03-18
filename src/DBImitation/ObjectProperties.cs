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

        public static BaseProperty[] CoolerPIDProperties()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("PID_n", "номер ПИД-регулятора"),
            };
        }
    }
}
