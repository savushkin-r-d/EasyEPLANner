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
        private static BaseProperty[] LineWashParams()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена", true)
            };
        }

        private static BaseProperty[] TankWashParams()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена", true),
                new ShowedBaseProperty("DI_CIP_FREE", "МСА свободна", true)
            };
        }

        private static BaseProperty[] WashParams()
        {
            var parameters = new BaseProperty[]
            {
                new ShowedBaseProperty("CIP_WASH_END", "Мойка завершена", true),
                new ShowedBaseProperty("DI_CIP_FREE", "МСА свободна", true),
                new NonShowedBaseProperty("DRAINAGE", "Номер шага дренаж", false)
            };

            return parameters;
        }
    }
}
