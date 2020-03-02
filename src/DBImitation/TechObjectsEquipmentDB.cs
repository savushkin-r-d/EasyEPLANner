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
                new ShowedBaseProperty("mix", "Мешалка"),
                new ShowedBaseProperty("bar", "Датчик решетки люка"),
                new ShowedBaseProperty("hatch", "Датчик крышки люка"),
                new ShowedBaseProperty("LT", "Датчик текущего уровня")
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
                new ShowedBaseProperty("hatch", "Датчик крышки люка"),
                new ShowedBaseProperty("LS_up", "Датчик верхнего уровня"),
                new ShowedBaseProperty("LS_down", "Датчик нижнего уровня"),
                new ShowedBaseProperty("LT", "Датчик текущего уровня"),
                new ShowedBaseProperty("TE","Датчик температуры")
            };
        }
    }
}
