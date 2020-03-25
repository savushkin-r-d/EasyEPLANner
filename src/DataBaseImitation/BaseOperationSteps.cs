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
        /// Шаги объектов "Линия", "Линия приемки" для операции "Наполнение"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineFillSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("IN_DRAINAGE", "В дренаж", false),
                new NonShowedBaseProperty("IN_TANK", "В приемник", false),
                new NonShowedBaseProperty("WAITING_KEY", "Ожидание", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия выдачи" для операции "Выдача";
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineOutSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("OUT_WATER", "Проталкивание", false),
                new NonShowedBaseProperty("OUT_TANK", "Из источника", false),
                new NonShowedBaseProperty("WAITING_KEY", "Ожидание", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Танк", "Линия приемки", "Линия", "Линия выдачи" для
        /// операции мойка.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] WashSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("DRAINAGE", "Дренаж", false)
            };
        }
    }
}
