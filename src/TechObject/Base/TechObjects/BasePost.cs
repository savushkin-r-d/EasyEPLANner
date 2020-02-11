using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый пост
    /// </summary>
    public class BasePost : BaseTechObject
    {
        public BasePost() : base()
        {
            S88Level = 2;
            Name = "Пост";
            EplanName = "post";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "post";
        }

        public override BaseTechObject Clone(TechObject techObject)
        {
            var baseTechobject = new BasePost();
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
