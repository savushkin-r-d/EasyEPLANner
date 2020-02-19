using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовая линия выдачи
    /// </summary>
    public class BaseLineOut : BaseTechObject
    {
        public BaseLineOut() : base()
        {
            S88Level = 2;
            Name = "Линия выдачи";
            EplanName = "line";
            BaseOperations = DataBase.Imitation.LineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
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
