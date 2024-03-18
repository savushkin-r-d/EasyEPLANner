using Editor;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TechObject
{
    /// <summary>
    /// Параметр для действий. <br/>
    /// Используется для установки параметра для действия. <br/>
    /// По-умолчанию устанавливается номер параметра. <br/>
    /// Также может быть установлено имя параметра или устройство: для этого
    /// в конструкторе нужно установить displayObjects
    /// </summary>
    public class ActionParameter : BaseParameter
    {
        public ActionParameter(string luaName, string name,
            string defaultValue = "", List<DisplayObject> displayObjects = null) 
            : base(luaName, name, defaultValue, displayObjects)
        { }

        public TechObject GetTechObject(ITreeViewItem item)
        {
            if (item is null)
                return null;

            return item is TechObject techobject ?
                techobject : GetTechObject(item.Parent);
        }

        public override bool SetNewValue(string newValue)
        {
            if (int.TryParse(newValue, out var parameterIndex) && parameterIndex != -1 &&
                 GetTechObject(Parent)?.GetParamsManager()?.Float.GetParam(parameterIndex - 1) == null)
            {
                return false;
            }

            return base.SetNewValue(newValue);
        }

        public override string[] DisplayText
        {
            get
            {
                var parameters = GetTechObject(Parent)?.GetParamsManager()?.Float;
                int.TryParse(Value, out int value);
                if ((parameters?.GetParam(value - 1) ?? parameters?.GetParam(Value)) is Param param )
                {
                    return new string[] { Name, $"{param.GetParameterNumber}. {param.GetNameLua()}: {param.GetValue()} {param.GetMeter()}" };
                }
                
                if (value == -1)
                {
                    return new string[] { Name, CommonConst.StubForCells };
                }
                
                return new string[] { Name, Value };
            }

        }

        public override BaseParameter Clone()
        {
            var newProperty = new ActionParameter(LuaName, Name,
                DefaultValue, DisplayObjects);
            newProperty.SetNewValue(Value);
            newProperty.NeedDisable = NeedDisable;
            
            return newProperty;
        }
    }
}
