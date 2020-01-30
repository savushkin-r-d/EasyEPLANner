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

        public override BaseTechObject Clone()
        {
            return new BasePost();
        }
    }
}
