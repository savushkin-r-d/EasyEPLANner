using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый автомат
    /// </summary>
    public class BaseAutomat : BaseTechObject
    {
        public BaseAutomat() : base()
        {
            S88Level = 2;
            Name = "Автомат";
            EplanName = "automat";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.AutomatProperties();
            BasicName = "automat";
        }

        public override BaseTechObject Clone()
        {
            return new BaseAutomat();
        }
    }
}
