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
        public static BaseParameter[] EmptyAggregateParameters()
        {
            return new BaseParameter[0];
        }

        /// <summary>
        /// Параметры агрегата - узел охлаждения.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] CoolerNodeAggregateParameters()
        {
            return new BaseParameter[]
            {
                new BoolShowedParameter("NEED_COOLING", 
                "Использовать узел охлаждения", "false")
            };
        }

        /// <summary>
        /// Параметры агрегата - узел расхода.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] FlowNodeAggregateParameters()
        {
            return new BaseParameter[]
            {
                new BoolShowedParameter("NEED_FLOW_CONTROL",
                "Использовать узел расхода", "false")
            };
        }

        /// <summary>
        /// Параметры агрегата - узел давления.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] PressureNodeAggregateParameters()
        {
            return new BaseParameter[]
            {
                new BoolShowedParameter("NEED_PRESSURE_CONTROL",
                "Использовать узел давления", "false")
            };
        }

        /// <summary>
        /// Параметры агрегата - узел подогрева, узел подогрева ПИД
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] HeaterNodeAggregateParameters()
        {
            return new BaseParameter[]
            {
                new BoolShowedParameter("HEATER_NODE",
                "Использовать узел подогрева", "false")
            };
        }
      
        /// Параметры агрегата - бойлер.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] BoilerAggregateParameters()
        {
            return new BaseParameter[]
            {
                new BoolShowedParameter("BOILER", "Использовать бойлер", "false")
            };
        }
    }
}
