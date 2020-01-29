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
        public static BaseOperation[] BaseTankOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("", ""),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams()),
                new BaseOperation("Наполнение", "luaName1", EmptyProperties()),
                new BaseOperation("Хранение", "luaName2", EmptyProperties()),
                new BaseOperation("Выдача", "luaName3", EmptyProperties()),
            };
        }
    }
}
