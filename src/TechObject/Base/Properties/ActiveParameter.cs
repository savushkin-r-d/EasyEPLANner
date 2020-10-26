namespace TechObject
{
    /// <summary>
    /// Активный (отображаемый) параметр
    /// </summary>
    public class ActiveParameter : BaseParameter
    {
        public ActiveParameter(string luaName, string name, 
            string defaultValue = "") : base(luaName, name, defaultValue) { }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveParameter(LuaName, Name,
                DefaultValue);
            newProperty.SetNewValue(Value);
            newProperty.NeedDisable = NeedDisable;

            foreach (var displayObject in DisplayObjects)
            {
                newProperty.AddDisplayObject(displayObject);
            }

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
        public override bool SetNewValue(string newValue)
        {
            bool notStub = !newValue.ToLower()
                .Contains(StaticHelper.CommonConst.StubForCells.ToLower());
            if (notStub)
            {
                return base.SetNewValue(newValue);
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