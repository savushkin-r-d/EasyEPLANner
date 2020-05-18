using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class ShowedBaseParameter : BaseParameter
    {
        public ShowedBaseParameter(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public ShowedBaseParameter(string luaName, string name) : base(luaName,
            name, true) { }

        public ShowedBaseParameter(string luaName, string name, 
            string defaultValue) : base(luaName, name, true, defaultValue) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ShowedBaseParameter(this.LuaName, this.Name,
                this.CanSave());
            newProperty.SetNewValue(this.Value);
            return newProperty;
        }

        #region реализация ItreeViewItem
        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        public override object Copy()
        {
            return this;
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        public override bool IsDeletable
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}