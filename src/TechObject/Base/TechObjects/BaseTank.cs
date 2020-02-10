using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый танк
    /// </summary>
    public class BaseTank : BaseTechObject
    {
        public BaseTank() : base()
        {
            S88Level = 1;
            Name = "Танк";
            EplanName = "tank";
            BaseOperations = DataBase.Imitation.BaseTankOperations();
            BaseProperties = DataBase.Imitation.TankProperties();
            BasicName = "tank";
        }

        public override BaseTechObject Clone(TechObject techObject)
        {
            //TODO: clone alghoritm
            return new BaseTank();
        }
    }
}
