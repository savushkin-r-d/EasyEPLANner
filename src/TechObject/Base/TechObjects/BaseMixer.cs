using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый узел перемешивания
    /// </summary>
    public class BaseMixer : BaseTechObject
    {
        public BaseMixer() : base()
        {
            S88Level = 2;
            Name = "Узел перемешивания";
            EplanName = "mix_node";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "mix_node";
        }

        public override BaseTechObject Clone(TechObject techObject)
        {
            var baseTechobject = new BaseMixer();
            baseTechobject.Name = this.Name;
            baseTechobject.EplanName = this.EplanName;
            baseTechobject.BasicName = this.BasicName;
            baseTechobject.S88Level = this.S88Level;
            baseTechobject.Owner = techObject.ModesManager.Owner;

            var baseOperations = new List<BaseOperation>();
            foreach (var mode in techObject.ModesManager.Modes)
            {
                var operation = mode.GetBaseOperation();
                baseOperations.Add(operation);
            }

            baseTechobject.BaseOperations = baseOperations.ToArray();
            return baseTechobject;
        }
    }
}
