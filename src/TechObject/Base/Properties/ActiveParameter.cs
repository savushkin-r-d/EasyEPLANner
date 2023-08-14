using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Активный (отображаемый) параметр
    /// </summary>
    public class ActiveParameter : BaseParameter
    {
        public ActiveParameter(string luaName, string name, 
            string defaultValue = "", List<DisplayObject> displayObjects = null)
            : base(luaName, name, defaultValue, displayObjects) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveParameter(LuaName, Name,
                DefaultValue, DisplayObjects);
            newProperty.NeedDisable = NeedDisable;
            newProperty.OneValueOnly = OneValueOnly;
            newProperty.SetNewValue(Value);
            return newProperty;
        }

        public override bool IsEmpty
        {
            get
            {
                bool isEmpty = Value == DefaultValue &&
                    DefaultValue == "";
                if (isEmpty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Записывается только одно значение, которое может быть перезаписано.
        /// </summary>
        public bool OneValueOnly { get; set; }

        #region реализация ItreeViewItem
        public override bool SetNewValue(string newValue)
        {   
            if (DisplayParameters && Owner is BaseOperation) 
                OneValueOnly = true;

            bool notStub = !newValue.ToLower()
                .Contains(StaticHelper.CommonConst.StubForCells.ToLower());
            if (notStub)
            {
                if (OneValueOnly)
                {
                    string[] values = newValue.Trim().Split(' ');
                    return base.SetNewValue(values.Last());
                }
                else
                {
                    return base.SetNewValue(newValue);
                }
            }
            else
            {
                string value = StaticHelper.CommonConst.StubForCells;
                return base.SetNewValue(value);
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
        #endregion
    }
}