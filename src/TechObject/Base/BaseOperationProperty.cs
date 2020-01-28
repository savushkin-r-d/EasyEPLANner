using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>    
    /// Свойство для базовой операции.
    /// </summary>
    public class BaseOperationProperty : Editor.ObjectProperty
    {
        public BaseOperationProperty(string luaName, string name, object value) 
            : base(name, value)
        {
            this.luaName = luaName;
            this.name = name;
            this.value = value;
        }

        public string GetLuaName()
        {
            return luaName;
        }

        public string GetName()
        {
            return name;
        }

        public string GetValue()
        {
            return value.ToString();
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        #region реализация ITreeView
        public override bool SetNewValue(string newValue)
        {
            this.value = newValue;
            return true;
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { luaName, value.ToString() };
            }
        }

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, value.ToString() };
            }
        }
        #endregion

        private string luaName; // Lua имя свойства
        private string name; // Имя свойства
        private object value; // Значение
    }
}
