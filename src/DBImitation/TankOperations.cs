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
        private static BaseOperation[] baseTankOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("", ""),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams()),
                new BaseOperation("Наполнение", "luaName1", emptyParams()),
                new BaseOperation("Хранение", "luaName2", emptyParams()),
                new BaseOperation("Выдача", "luaName3", emptyParams()),
            };
        }
    }
}
