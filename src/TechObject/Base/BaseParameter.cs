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
    public abstract class BaseParameter : Editor.ObjectProperty
    {
        /// <summary>
        /// Абстрактный метод копирования объекта.
        /// </summary>
        /// <returns></returns>
        public abstract new BaseParameter Clone();

        public BaseParameter(string luaName, string name, 
            string defaultValue = "") : base(name, defaultValue)
        {
            this.luaName = luaName;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Lua имя свойства.
        /// </summary>
        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        public string DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Нужно ли отображать параметр.
        /// </summary>
        /// <returns></returns>
        public virtual bool isShowed()
        {
            return true;
        }

        /// <summary>
        /// Нужно ли сохранять параметр в файле описания.
        /// </summary>
        public virtual bool needToSave
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
                return true;
            }
        }

        #region реализация ITreeView
        public override bool SetNewValue(string newValue)
        {
            newValue = newValue.Trim();
            base.SetNewValue(newValue);
            return true;
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { luaName, Value };
            }
        }

        override public string[] DisplayText
        {
            get
            {
                return new string[] { Name, Value };
            }
        }
        #endregion

        private string luaName;
        private string defaultValue;
    }
}
