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
            string defaultValue = "", List<DisplayObject> displayObjects = null)
            : base(name, defaultValue, defaultValue)
        {
            this.luaName = luaName;

            if(displayObjects != null)
            {
                displayObjectsFlags = new List<DisplayObject>();
                foreach(var displayObject in displayObjects)
                {
                    displayObjectsFlags.Add(displayObject);
                }
            }
            else
            {
                displayObjectsFlags = new List<DisplayObject>() 
                { DisplayObject.None };
            }

            SetUpDisplayObjects();
        }

        /// <summary>
        /// Настройка отображаемых объектов
        /// </summary>
        private void SetUpDisplayObjects()
        {
            deviceTypes = new Device.DeviceType[0];
            displayParameters = false;
            foreach (var displayObject in DisplayObjects)
            {
                switch (displayObject)
                {
                    case DisplayObject.Parameters:
                        displayParameters = true;
                        break;

                    case DisplayObject.Signals:
                        deviceTypes = new Device.DeviceType[]
                        {
                            Device.DeviceType.AI,
                            Device.DeviceType.AO,
                            Device.DeviceType.DI,
                            Device.DeviceType.DO
                        };
                        break;
                }
            }
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
                bool replaceNoneValue =
                    displayObjectsFlags.Contains(DisplayObject.None) &&
                    displayObjectsFlags.Count == 1 &&
                    parsedEnum != DisplayObject.None;
                bool setToNoneValue = parsedEnum == DisplayObject.None;
                if (replaceNoneValue || setToNoneValue)
                {
                    displayObjectsFlags.Clear();
                    displayObjectsFlags.Add(parsedEnum);
                }

                if (!displayObjectsFlags.Contains(parsedEnum))
                {
                    displayObjectsFlags.Add(parsedEnum);
                }

                SetUpDisplayObjects();
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

        public override void GetDisplayObjects(out Device.DeviceType[] devTypes, 
            out Device.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            devSubTypes = null; // Not used;
            devTypes = deviceTypes;
            displayParameters = this.displayParameters;
        }
        #endregion

        private object owner;
        private string luaName;
        private List<DisplayObject> displayObjectsFlags;

        private Device.DeviceType[] deviceTypes;
        private bool displayParameters;

        public enum DisplayObject
        {
            None = 1,
            Signals,
            Parameters,
        }
    }
}
