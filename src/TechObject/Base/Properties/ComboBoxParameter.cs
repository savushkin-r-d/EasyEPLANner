using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Параметр в виде комбинированного списка.
    /// </summary>
    public class ComboBoxParameter : BaseParameter
    {
        /// <summary>
        /// Создание комбинированого списка
        /// </summary>
        /// <param name="luaName">Lua-имя параметра.</param>
        /// <param name="name">Отображаемое имя параметра</param>
        /// <param name="parameterValues">Значение параметров в виде словаря 
        /// {отображаемое имя - Lua-имя}.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        /// <param name="displayObjects"></param>
        public ComboBoxParameter(string luaName, string name,
            Dictionary<string, string> parameterValues, 
            string defaultValue, List<DisplayObject> displayObjects = null)
            : base(luaName, name, defaultValue, displayObjects) 
        {
            this.parameterValues = parameterValues;
        }

        public override BaseParameter Clone()
        {
            var newProperty = new ActiveBoolParameter(LuaName, Name,
                DefaultValue, DisplayObjects);
            newProperty.SetValue(Value);
            newProperty.NeedDisable = NeedDisable;
            return newProperty;
        }

        #region реализация ItreeViewItem
        /// <summary>
        /// Установка нового значения по Lua-имени.
        /// </summary>
        public override bool SetNewValue(string newValue)
        {
            var value = parameterValues
                .FirstOrDefault(item => item.Value == newValue)
                .Key;
            if (value == null)
                return base.SetNewValue(DefaultValue);
            else
                return base.SetNewValue(value);
        }

        /// <summary>
        /// Установка нового значения(в редакторе).
        /// </summary>
        public override bool SetNewValue(string newValue, bool isExtraValue)
        {
            return base.SetNewValue(newValue);
        }

        public override string[] DisplayText
        {
            get
            {
                return new string[] { Name, Value };
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
                return new List<string>(parameterValues.Keys);
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
        
        /// <summary>
        /// Получение Lua-имени значения.
        /// </summary>
        public string LuaValue
        {
            get { return parameterValues[Value.Trim()]; }
        }

        private Dictionary<string, string> parameterValues;
    }
}
