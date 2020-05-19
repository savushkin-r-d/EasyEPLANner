using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Пассивный (не отображаемый) параметр.
    /// </summary>
    public class PassiveParameter : BaseParameter
    {
        public PassiveParameter(string luaName, string name)
            : base(luaName, name) { }

        public override BaseParameter Clone()
        {
            var newProperty = new PassiveParameter(this.LuaName, this.Name);
            newProperty.SetNewValue(this.Value);
            return newProperty;
        }

        public override bool isShowed()
        {
            return false;
        }

        public override bool needToSave
        {
            get
            {
                return false;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }
    }
}
