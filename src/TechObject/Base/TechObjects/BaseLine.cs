using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовая линия
    /// </summary>
    public class BaseLine : BaseTechObject
    {
        public BaseLine() : base()
        {
            S88Level = 2;
            Name = "Линия";
            EplanName = "line";
            BaseOperations = DataBase.Imitation.BaseLineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
        }

        public override BaseTechObject Clone()
        {
            return new BaseLine();
        }
    }
}
