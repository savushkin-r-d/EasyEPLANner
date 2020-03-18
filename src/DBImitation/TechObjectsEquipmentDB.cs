using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace DataBase
{
    /// <summary>
    /// Класс, содержащий оборудование для технологических объектов
    /// </summary>
    public partial class Imitation
    {
        /// <summary>
        /// Получить пустой список оборудования объекта.
        /// </summary>
        /// <returns></returns>
        static public BaseProperty[] EmptyEquipment()
        {
            return new BaseProperty[0];
        }

        /// <summary>
        /// Получить список оборудования базового узла перемешивания.
        /// </summary>
        /// <returns></returns>
        static public BaseProperty[] MixerEquipment()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("mix", "Мешалка", "M1"),
                new ShowedBaseProperty("bar", "Датчик решетки люка", "GS2"),
                new ShowedBaseProperty("hatch", "Датчик крышки люка", "GS1"),
                new ShowedBaseProperty("LT", "Датчик текущего уровня", "LT1")
            };
        }

        /// <summary>
        /// Получить список оборудования базового танка.
        /// </summary>
        /// <returns></returns>
        static public BaseProperty[] TankEquipment()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("hatch", "Датчик крышки люка", "GS1"),
                new ShowedBaseProperty("LS_up", "Датчик верхнего уровня", "LS2"),
                new ShowedBaseProperty("LS_down", "Датчик нижнего уровня", "LS1"),
                new ShowedBaseProperty("LT", "Датчик текущего уровня", "LT1"),
                new ShowedBaseProperty("TE","Датчик температуры", "TE1")
            };
        }

        /// <summary>
        /// Получить список оборудования базового узла охлаждения
        /// </summary>
        /// <returns></returns>
        static public BaseProperty[] CoolerNodeEquipment()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("TE", "Датчик температуры", "TE1")
            };
        }

        /// <summary>
        /// Получить список оборудования базового узла охлаждения ПИД
        /// </summary>
        /// <returns></returns>
        static public BaseProperty[] CoolerNodePIDEquipment()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("TE", "Датчик температуры", "TE1"),
                new ShowedBaseProperty("VC", "Регулирующий клапан", "VC1")
            };
        }

        /// <summary>
        /// Получить Список оборудования для бачка откачки ледяной воды.
        /// </summary>
        /// <returns></returns>
        static public BaseProperty[] WaterTankEquipment()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("M1", "Насос", "M1"),
                new ShowedBaseProperty("LS_up", "Датчик верхнего уровня", "LS2"),
                new ShowedBaseProperty("LS_down", "Датчик нижнего уровня", "LS1"),
            };
        }

    }
}
