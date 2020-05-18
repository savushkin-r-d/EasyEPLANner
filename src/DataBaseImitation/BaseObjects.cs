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
        /// Получить базовое название объекта по его обычному названию.
        /// </summary>
        /// <param name="baseTechObjectName">Название объекта</param>
        /// <returns></returns>
        public static string GetBasicName(string baseTechObjectName)
        {
            string basicName = "";

            foreach (BaseTechObject baseTechObject in BaseTechObjects())
            {
                if (baseTechObject.Name == baseTechObjectName)
                {
                    basicName = baseTechObject.BasicName;
                }
            }

            return basicName;
        }

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
                    BaseEmptyOperations(), EmptyProperties(), "automat",
                    EmptyEquipment(), EmptyAggregateProperties()),
                new BaseTechObject("Бачок откачки лёдводы", "_tank", 2,
                    WaterTankOperations(), WaterTankProperties(), 
                    "ice_water_pump_tank", WaterTankEquipment(), 
                    EmptyAggregateProperties()),
                new BaseTechObject("Бачок откачки лёдводы ПИД", "_tank_PID", 2,
                    WaterTankOperations(), WaterTankProperties(), 
                    "ice_water_pump_tank_PID", WaterTankPIDEquipment(),
                    EmptyAggregateProperties()),
                new BaseTechObject("Бойлер", "boil", 2, BoilerOperations(),
                    EmptyProperties(), "boiler", EmptyEquipment(),
                    BoilerAggregateProperties()),
                new BaseTechObject("Мастер", "master", 1, BaseEmptyOperations(),
                    EmptyProperties(), "master", EmptyEquipment(),
                    EmptyAggregateProperties()),
                new BaseTechObject("Линия", "line", 2, LineOperations(),
                    LineProperties(), "line", EmptyEquipment(),
                    EmptyAggregateProperties()),
                new BaseTechObject("Линия приемки", "line_in", 2, 
                    LineInOperations(), LineProperties(), "line", 
                    EmptyEquipment(), EmptyAggregateProperties()),
                new BaseTechObject("Линия выдачи", "line_out", 2,
                    LineOutOperations(), LineProperties(), "line",
                    EmptyEquipment(), EmptyAggregateProperties()),
                new BaseTechObject("Пастеризатор", "pasteurizator", 2,
                    POUOperations(), POUProperties(), "pasteurizator",
                    EmptyEquipment(), EmptyAggregateProperties()),
                new BaseTechObject("Пост", "post", 2, BaseEmptyOperations(),
                    EmptyProperties(), "post", EmptyEquipment(),
                    EmptyAggregateProperties()),
                new BaseTechObject("Танк", "tank", 1, TankOperations(),
                    TankProperties(), "tank", TankEquipment(),
                    EmptyAggregateProperties()),
                new BaseTechObject("Узел давления ПИД", "pressure_node_PID", 2,
                    PressureNodePIDOperations(), EmptyProperties(),
                    "pressure_node_PID", PressureNodePIDEquipment(),
                    PressureNodeAggregateProperties()),
                new BaseTechObject("Узел подогрева", "heater_node", 2, 
                    HeaterNodeOperations(), EmptyProperties(), "heater_node",
                    EmptyEquipment(), HeaterNodeAggregateProperties()),
                new BaseTechObject("Узел подогрева ПИД", "heater_node_PID", 2,
                    HeaterNodeOperations(), EmptyProperties(), "heater_node_PID",
                    HeaterNodePIDEquipment(), HeaterNodeAggregateProperties()),
                new BaseTechObject("Узел расхода ПИД", "flow_node_PID", 2,
                    FlowNodePIDOperations(), EmptyProperties(), "flow_node_PID",
                    FlowNodePIDEquipment(), FlowNodeAggregateProperties()),
                new BaseTechObject("Узел охлаждения", "cooler_node", 2,
                    CoolerNodeOperations(), EmptyProperties(), "cooler_node",
                    CoolerNodeEquipment(), CoolerNodeAggregateProperties()),
                new BaseTechObject("Узел охлаждения ПИД", "cooler_node_PID", 2,
                    CoolerNodeOperations(), EmptyProperties(), "cooler_node_PID",
                    CoolerNodePIDEquipment(), EmptyAggregateProperties()),
                new BaseTechObject("Узел перемешивания", "mix_node", 2,
                    BaseEmptyOperations(), EmptyProperties(), "mix_node",
                    MixerEquipment(), EmptyAggregateProperties())
            };
        }
    }
}
