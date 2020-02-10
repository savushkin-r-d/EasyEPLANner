using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовая линия приемки
    /// </summary>
    public class BaseLineIn : BaseTechObject
    {
        public BaseLineIn() : base()
        {
            S88Level = 2;
            Name = "Линия приемки";
            EplanName = "line";
            BaseOperations = DataBase.Imitation.BaseLineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
        }

        public override BaseTechObject Clone(TechObject techObject)
        {
            //TODO: clone alghoritm
            return new BaseLineIn();
        }
    }
}
