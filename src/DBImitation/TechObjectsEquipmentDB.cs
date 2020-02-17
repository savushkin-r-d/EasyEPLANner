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
        static public BaseProperty[] BaseMixerEquipment()
        {
            return new BaseProperty[]
            {
                new ShowedBaseProperty("mix", "Мешалка"),
                new ShowedBaseProperty("bar", "Датчик решетки люка"),
                new ShowedBaseProperty("hatch", "Датчик люка")
            };
        }
    }
}
