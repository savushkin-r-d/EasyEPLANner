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
    }
}
