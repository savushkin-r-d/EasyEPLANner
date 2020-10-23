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
            var newProperty = new ActiveBoolParameter(LuaName, Name,
                DefaultValue);
            newProperty.SetValue(Value);
            newProperty.NeedDisable = NeedDisable;
            
            foreach(var showData in ShowDatas)
            {
                newProperty.AddShowData(showData);
            }

            return newProperty;
        }

        #region реализация ItreeViewItem
        public override bool SetNewValue(string newValue)
        {
            var value = newValue ==
                trueDisplayValue ? trueLogicValue : falseLogicValue;
            base.SetValue(value);
            return true;
        }

        public override bool SetNewValue(string newValue, bool isExtraValue)
        {
            return SetNewValue(newValue);
        }

        public override string[] DisplayText
        {
            get
            {
                string value = Value ==
                    trueLogicValue ? trueDisplayValue : falseDisplayValue;
                return new string[] { Name, value };
            }
        }

        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
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

        public override List<string> BaseObjectsList
        {
            get
            {
                return new List<string>() 
                { 
                    trueDisplayValue, 
                    falseDisplayValue 
                };
            }
        }

        public override bool NeedRebuildParent
        {
            get
            {
                return true;
            }
        }
        #endregion

        const string trueDisplayValue = "Да";
        const string falseDisplayValue = "Нет";
        const string trueLogicValue = "true";
        const string falseLogicValue = "false";
    }
}
