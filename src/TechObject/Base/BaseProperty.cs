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
    public abstract class BaseProperty : Editor.ObjectProperty
    {
        /// <summary>
        /// Абстрактный метод копирования объекта.
        /// </summary>
        /// <returns></returns>
        public abstract new BaseProperty Clone();

        public BaseProperty(string luaName, string name, bool canSave) 
            : base(name, "")
        {
            this.luaName = luaName;
            this.name = name;
            this.value = "";
            this.canSave = canSave;
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

        /// <summary>
        /// Значение свойства.
        /// </summary>
        public string Value
        {
            get
            {
                return value.ToString();
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Имя свойства.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Нужно ли отображать свойство в дереве объектов.
        /// </summary>
        /// <returns></returns>
        public virtual bool isShowed()
        {
            return true;
        }

        override public bool IsUseDevList
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Нужно ли сохранять это свойство в файле описания.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSave()
        {
            return canSave;
        }

        #region реализация ITreeView
        public override bool SetNewValue(string newValue)
        {
            newValue = newValue.Trim();
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

        private string luaName;
        private string name;
        private object value;
        private bool canSave;
    }
}
