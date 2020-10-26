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
            string defaultValue = "") : base(name, defaultValue, defaultValue)
        {
            this.luaName = luaName;

            displayObjectsFlags = new List<DisplayObject>() 
            { 
                DisplayObject.None 
            };
        }

        /// <summary>
        /// Добавить вид отображаемых объектов. Вызывается из LUA.
        /// </summary>
        /// <param name="flagValue">Строковое значение перечисления</param>
        public void AddDisplayObject(string flagValue)
        {
            bool ignoreCase = true;
            var parsed = Enum.TryParse(flagValue, ignoreCase,
                out DisplayObject parsedEnum);
            if(parsed)
            {
                AddDisplayObject(parsedEnum);
            }
        }

        /// <summary>
        /// Добавить вид отображаемых объектов
        /// </summary>
        /// <param name="displayObject">Значение перечисления</param>
        public void AddDisplayObject(DisplayObject displayObject)
        {
            bool replaceNoneValue = 
                displayObjectsFlags.Contains(DisplayObject.None) &&
                displayObjectsFlags.Count == 1 &&
                displayObject != DisplayObject.None;
            bool setToNoneValue = displayObject == DisplayObject.None;
            if (replaceNoneValue || setToNoneValue)
            {
                displayObjectsFlags.Clear();
                displayObjectsFlags.Add(displayObject);
            }

            if (!displayObjectsFlags.Contains(displayObject))
            {
                displayObjectsFlags.Add(displayObject);
            }
        }

        public List<DisplayObject> DisplayObjects
        {
            get
            {
                return displayObjectsFlags;
            }
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

        override public bool IsUseDevList
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Пустой ли параметр (nil or '')
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Объект-владелец параметра.
        /// </summary>
        public object Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
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

        private object owner;
        private string luaName;
        private List<DisplayObject> displayObjectsFlags;

        public enum DisplayObject
        {
            None = 1,
            Signals,
            Parameters,
        }
    }
}
