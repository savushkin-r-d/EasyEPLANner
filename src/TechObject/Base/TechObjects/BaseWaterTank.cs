using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый бачок
    /// </summary>
    public class BaseWaterTank : BaseTechObject
    {
        public BaseWaterTank() : base()
        {
            S88Level = 2;
            Name = "Бачок";
            EplanName = "_tank";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "cooler";
        }

        public override BaseTechObject Clone()
        {
            return new BaseWaterTank();
        }
    }
}
