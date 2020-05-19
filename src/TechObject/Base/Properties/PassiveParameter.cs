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
        public PassiveParameter(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public PassiveParameter(string luaName, string name)
            : base(luaName, name, true) { }

        public override BaseParameter Clone()
        {
            var newProperty = new PassiveParameter(this.LuaName,
                this.Name, this.CanSave());
            newProperty.SetNewValue(this.Value);
            return newProperty;
        }

        public override bool isShowed()
        {
            return false;
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
