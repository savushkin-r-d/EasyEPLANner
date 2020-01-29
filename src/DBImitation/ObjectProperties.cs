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
        public static BaseProperty[] AutomatProperties()
        {
            return new BaseProperty[0];
        }

        public static BaseProperty[] TankProperties()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена"),
                new ShowedBaseProperty("DI_CIP_FREE", "МСА свободна"),
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж")
            };
        }

        public static BaseProperty[] LineProperties()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена")
            };
        }
    }
}
