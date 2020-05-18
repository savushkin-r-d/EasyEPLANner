using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Булевский параметр для базовой операции.
    /// </summary>
    public class BoolShowedParameter : BaseParameter
    {
        private BoolShowedParameter(string luaName, string name, bool canSave,
            string defaultValue) : base(luaName, name, canSave, defaultValue)
        { }

        public BoolShowedParameter(string luaName, string name,
            string defaultValue) : base(luaName, name, true, defaultValue) 
        { }

        public override BaseParameter Clone()
        {
            var newProperty = new BoolShowedParameter(this.LuaName, this.Name,
                this.CanSave(), this.DefaultValue);
            newProperty.SetValue(this.Value);
            return newProperty;
        }

        #region реализация ItreeViewItem
        public override bool SetNewValue(string newValue)
        {
            var value = newValue == "Да" ? "false" : "true";
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

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        public override bool IsBoolParameter
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}
