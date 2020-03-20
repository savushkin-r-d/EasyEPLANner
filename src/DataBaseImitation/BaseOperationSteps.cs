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
        /// Шаги объектов "Линия", "Линия приемки", "Линия выдачи" для
        /// операций "Наполнение" и "Выдача";
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineFillOutSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("Выдача", "OUT", false),
                new NonShowedBaseProperty("Наполнение", "FILL", false)
            };
        }
    }
}
