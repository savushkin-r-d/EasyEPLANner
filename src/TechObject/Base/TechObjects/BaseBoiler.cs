using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый бойлер
    /// </summary>
    public class BaseBoiler : BaseTechObject
    {
        public BaseBoiler() : base()
        {
            S88Level = 2;
            Name = "Бойлер";
            EplanName = "boil";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "boiler";
            Equipment = DataBase.Imitation.EmptyEquipment();
        }

        /// <summary>
        /// Клонировать объект
        /// </summary>
        /// <param name="techObject">Новый владелец базового объекта</param>
        /// <returns></returns>
        public override BaseTechObject Clone(TechObject techObject)
        {
            var cloned = DataBase.Imitation.BaseTechObjects()
                .Where(x => x.Name == this.Name)
                .FirstOrDefault();
            cloned.Owner = techObject;
            return cloned;
        }
    }
}
