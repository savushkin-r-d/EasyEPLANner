using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый мастер
    /// </summary>
    public class BaseMaster : BaseTechObject
    {
        public BaseMaster() : base()
        {
            S88Level = 1;
            Name = "Мастер";
            EplanName = "master";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "master";
        }

        public override BaseTechObject Clone(TechObject techObject)
        {
            var cloned = DataBase.Imitation.BaseTechObjectArr()
                .Where(x => x.Name == this.Name)
                .FirstOrDefault();
            cloned.Owner = techObject.ModesManager.Owner;
            return cloned;
        }
    }
}
