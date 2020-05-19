using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace DataBase
{
    /// <summary>
    /// Имитация базы данных.
    /// </summary>
    public partial class Imitation
    {
        /// <summary>
        /// Получить базовый технологический объект по обычному названию.
        /// </summary>
        /// <param name="name">Название объекта</param>
        /// <returns></returns>
        public static BaseTechObject GetTechObject(string name)
        {
            foreach (BaseTechObject baseTechObject in BaseTechObjects())
            {
                if (name == baseTechObject.Name || 
                    name == baseTechObject.EplanName)
                {
                    return baseTechObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Получить массив всех базовых объектов.
        /// </summary>
        /// <returns></returns>
        public static BaseTechObject[] BaseTechObjects()
        {
            return new BaseTechObject[]
            {
                new BaseTechObject(),
                new BaseTechObject("Автомат", "automat", 2, 
                    BaseEmptyOperations(), "automat", EmptyEquipment(), 
                    EmptyAggregateParameters()),
                new BaseTechObject("Бачок откачки лёдводы", "_tank", 2,
                    WaterTankOperations(), "ice_water_pump_tank", 
                    WaterTankEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Бачок откачки лёдводы ПИД", "_tank_PID", 2,
                    WaterTankOperations(), "ice_water_pump_tank_PID", 
                    WaterTankPIDEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Бойлер", "boil", 2, BoilerOperations(),
                    "boiler", EmptyEquipment(), BoilerAggregateParameters()),
                new BaseTechObject("Мастер", "master", 1, BaseEmptyOperations(),
                    "master", EmptyEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Линия", "line", 2, LineOperations(),
                    "line", EmptyEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Линия приемки", "line_in", 2, 
                    LineInOperations(), "line", EmptyEquipment(), 
                    EmptyAggregateParameters()),
                new BaseTechObject("Линия выдачи", "line_out", 2,
                    LineOutOperations(), "line", EmptyEquipment(), 
                    EmptyAggregateParameters()),
                new BaseTechObject("Пастеризатор", "pasteurizator", 2,
                    POUOperations(), "pasteurizator", EmptyEquipment(), 
                    EmptyAggregateParameters()),
                new BaseTechObject("Пост", "post", 2, BaseEmptyOperations(),
                    "post", EmptyEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Танк", "tank", 1, TankOperations(),
                    "tank", TankEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Узел давления ПИД", "pressure_node_PID", 2,
                    PressureNodePIDOperations(), "pressure_node_PID", 
                    PressureNodePIDEquipment(), 
                    PressureNodeAggregateParameters()),
                new BaseTechObject("Узел подогрева", "heater_node", 2, 
                    HeaterNodeOperations(), "heater_node", EmptyEquipment(), 
                    HeaterNodeAggregateParameters()),
                new BaseTechObject("Узел подогрева ПИД", "heater_node_PID", 2,
                    HeaterNodeOperations(), "heater_node_PID", 
                    HeaterNodePIDEquipment(), HeaterNodeAggregateParameters()),
                new BaseTechObject("Узел расхода ПИД", "flow_node_PID", 2,
                    FlowNodePIDOperations(), "flow_node_PID",
                    FlowNodePIDEquipment(), FlowNodeAggregateParameters()),
                new BaseTechObject("Узел охлаждения", "cooler_node", 2,
                    CoolerNodeOperations(), "cooler_node",
                    CoolerNodeEquipment(), CoolerNodeAggregateParameters()),
                new BaseTechObject("Узел охлаждения ПИД", "cooler_node_PID", 2,
                    CoolerNodeOperations(), "cooler_node_PID",
                    CoolerNodePIDEquipment(), EmptyAggregateParameters()),
                new BaseTechObject("Узел перемешивания", "mix_node", 2,
                    BaseEmptyOperations(), "mix_node", MixerEquipment(), 
                    EmptyAggregateParameters())
            };
        }
    }
}
