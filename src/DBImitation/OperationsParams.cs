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
        private static BaseOperationProperty[] lineWashParams()
        {
            return new BaseOperationProperty[]
            {
                new ShowedBaseOperationProperty("CIP_WASH_END", "Мойка завершена", true)
            };
        }

        private static BaseOperationProperty[] tankWashParams()
        {
            return new BaseOperationProperty[]
            {
                new ShowedBaseOperationProperty("CIP_WASH_END", "Мойка завершена", true),
                new ShowedBaseOperationProperty("DI_CIP_FREE", "МСА свободна", true)
            };
        }

        private static BaseOperationProperty[] WashParams()
        {
            var parameters = new BaseOperationProperty[]
            {
                new ShowedBaseOperationProperty("CIP_WASH_END", "Мойка завершена", true),
                new ShowedBaseOperationProperty("DI_CIP_FREE", "МСА свободна", true),
                new NonShowedBaseOperationProperty("DRAINAGE", "Номер шага дренаж", false)
            };

            return parameters;
        }
    }
}
