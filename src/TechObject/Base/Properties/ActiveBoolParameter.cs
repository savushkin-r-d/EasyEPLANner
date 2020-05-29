using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Булевый активный (отображаемый) параметр.
    /// </summary>
    public class ActiveBoolParameter : BaseParameter
    {
        public ActiveBoolParameter(string luaName, string name, 
            string defaultValue) : base(luaName, name, defaultValue) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveBoolParameter(this.LuaName, this.Name,
                this.DefaultValue);
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
