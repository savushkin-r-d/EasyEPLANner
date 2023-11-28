using Editor;
using System;

namespace TechObject
{
    /// <summary>
    /// Свойство, операции в которых участвует параметр
    /// </summary>
    public class ParamProperty : ObjectProperty
    {
        public ParamProperty(string name, object value,
            object defaultValue = null, bool editable = true)
            : base(name, value, defaultValue)
        {
            this.editable = editable;
        }

        public override bool SetNewValue(string newValue)
        {
            if (Param?.Params?.ParamsManager?.TechObject is GenericTechObject &&
                Name == Param.ValuePropertyName)
            { // Установка пустого значения для параметра в типовом объекте
                if (newValue.Trim() == "-")
                {
                    value = "-";
                    defaultValue = "-";
                }
                else if (double.TryParse(newValue, out var newValueAsDouble))
                {
                    value = newValueAsDouble;
                    defaultValue = string.Empty;
                    OnValueChanged(this);
                }
                else
                {
                    return false;
                }

                return true;
            }
            else if (Name == Param.NameLuaPropertyName 
                && (Param?.Params?.HaveSameLuaName(newValue) ?? false))
            { // Если уже есть параметр с таким именем, то оставляем прошлое значение
                return false;
            }
            else
            {
                return base.SetNewValue(newValue);
            }
        }

        #region реализация ITreeViewItem
        public override bool IsEditable => editable;

        public Param Param => Parent as Param;

        public override int[] EditablePart
        {
            get
            {
                if (editable)
                {
                    return new int[] { -1, 1 };
                }
                else
                {
                    return new int[] { -1, -1 };
                }
            }
        }
        #endregion

        private bool editable = true;
    }
}
