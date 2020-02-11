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
            BaseOperations = DataBase.Imitation.BaseLineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
        }

        public override BaseTechObject Clone(TechObject techObject)
        {
            var baseTechobject = new BaseLineOut();
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
