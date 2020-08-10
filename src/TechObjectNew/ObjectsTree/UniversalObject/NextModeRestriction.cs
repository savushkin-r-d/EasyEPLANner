using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechObject
{
    public class NextModeRestriction : Restriction
    {
        public NextModeRestriction(string Name, string Value, string LuaName,
            SortedDictionary<int, List<int>> dict) : base(Name, Value,
                LuaName, dict)
        {
        }

        override public bool IsLocalRestrictionUse
        {
            get
            {
                return true;
            }
        }
    }
}
