using System;
using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Свойство, операции в которых участвует параметр
    /// </summary>
    class ParamProperty : ObjectProperty
    {
        public ParamProperty(string name, object value,
            object defaultValue = null, bool editable = true)
            : base(name, value, defaultValue)
        {
            this.editable = editable;
        }

        public override bool SetNewValue(string newValue)
        {
            return base.SetNewValue(newValue);          
        }

        #region реализация ITreeViewItem
        public override bool IsEditable => editable;

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
