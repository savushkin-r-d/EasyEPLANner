namespace NewTechObject
{
    /// <summary>
    /// Активный (отображаемый) параметр
    /// </summary>
    public class ActiveParameter : BaseParameter
    {
        public ActiveParameter(string luaName, string name)
            : base(luaName, name) { }

        public ActiveParameter(string luaName, string name, 
            string defaultValue) : base(luaName, name, defaultValue) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveParameter(this.LuaName, this.Name,
                this.DefaultValue);
            newProperty.SetNewValue(this.Value);
            newProperty.NeedDisable = this.NeedDisable;
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
                return true;
            }
        }

        public override bool IsFilled
        {
            get
            {
                if(Value == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion
    }
}