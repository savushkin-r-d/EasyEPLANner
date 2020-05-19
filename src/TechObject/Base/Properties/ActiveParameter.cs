using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Активный (отображаемый) параметр
    /// </summary>
    public class ActiveParameter : BaseParameter
    {
        public ActiveParameter(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public ActiveParameter(string luaName, string name) : base(luaName,
            name, true) { }

        public ActiveParameter(string luaName, string name, 
            string defaultValue) : base(luaName, name, true, defaultValue) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveParameter(this.LuaName, this.Name,
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