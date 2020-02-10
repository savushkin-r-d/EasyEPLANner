using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class NonShowedBaseProperty : BaseProperty
    {
        public NonShowedBaseProperty(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public NonShowedBaseProperty(string luaName, string name)
            : base(luaName, name, true) { }

        public override BaseProperty Clone()
        {
            var newProperty = new NonShowedBaseProperty(this.LuaName,
                this.Name, this.CanSave());
            newProperty.Value = this.Value;
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
