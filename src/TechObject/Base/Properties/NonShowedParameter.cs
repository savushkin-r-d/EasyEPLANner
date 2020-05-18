using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class NonShowedBaseParameter : BaseParameter
    {
        public NonShowedBaseParameter(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public NonShowedBaseParameter(string luaName, string name)
            : base(luaName, name, true) { }

        public override BaseParameter Clone()
        {
            var newProperty = new NonShowedBaseParameter(this.LuaName,
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
