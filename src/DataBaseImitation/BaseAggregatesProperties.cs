using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;


namespace DataBase
{
    partial class Imitation
    {
        /// <summary>
        /// Пустой массив свойств агрегата.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] EmptyAggregateProperties()
        {
            return new BaseProperty[0];
        }

        /// <summary>
        /// Свойства агрегата - узел охлаждения.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] CoolerNodeAggregateProperties()
        {
            return new BaseProperty[]
            {
                new BoolShowedProperty("NEED_COOLING", 
                "Использовать узел охлаждения", "false")
            };
        }
    }
}
