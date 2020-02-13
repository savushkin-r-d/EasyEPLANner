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
        public static BaseOperation[] TankOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("", "", EmptyProperties()),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams()),
            };
        }
    }
}
