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
        private static BaseOperation[] baseLineOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("",""),
                new BaseOperation("Мойка", "WASHING_CIP", lineWashParams()),
                new BaseOperation("Работа", "luaName1", emptyParams())
            };
        }
    }
}
