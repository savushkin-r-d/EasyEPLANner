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

        /// <summary>
        /// Свойства агрегата - узел расхода.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] FlowNodeAggregateProperties()
        {
            return new BaseProperty[]
            {
                new BoolShowedProperty("NEED_FLOW_CONTROL",
                "Использовать узел расхода", "false")
            };
        }

        /// <summary>
        /// Свойства агрегата - узел давления.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] PressureNodeAggregateProperties()
        {
            return new BaseProperty[]
            {
                new BoolShowedProperty("NEED_PRESSURE_CONTROL",
                "Использовать узел давления", "false")
            };
        }

        /// <summary>
        /// Свойства агрегата - узел подогрева, узел подогрева ПИД
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] HeaterNodeAggregateProperties()
        {
            return new BaseProperty[]
            {
                new BoolShowedProperty("HEATER_NODE",
                "Использовать узел подогрева", "false")
            };
        }
    }
}
