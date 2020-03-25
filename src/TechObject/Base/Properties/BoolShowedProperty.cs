using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    public class BoolShowedproperty : BaseProperty
    {
        private BoolShowedproperty(string luaName, string name, bool canSave)
            : base(luaName, name, canSave) { }

        public BoolShowedproperty(string luaName, string name,
            string defaultValue) : base(luaName, name, true, defaultValue) { }

        public override BaseProperty Clone()
        {
            var newProperty = new BoolShowedproperty(this.LuaName, this.Name,
                this.CanSave());
            newProperty.SetValue(this.Value);
            return newProperty;
        }

        #region реализация ItreeViewItem
        public override bool SetNewValue(string newValue)
        {
            var value = newValue == "true" ? "false" : "true";
            base.SetValue(value);
            return true;
        }

        public override string[] DisplayText
        {
            get
            {
                string value = Value == "true" ? "Да" : "Нет";
                return new string[] { this.Name, value };
            }
        }

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
                return true;
            }
        }
        #endregion
    }
}
