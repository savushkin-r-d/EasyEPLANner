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
    public abstract class BaseOperationProperty : Editor.ObjectProperty
    {
        public BaseOperationProperty(string luaName, string name, bool canSave) 
            : base(name, "")
        {
            this.luaName = luaName;
            this.name = name;
            this.value = "";
            this.canSave = canSave;
        }

        public string GetLuaName()
        {
            return luaName;
        }

        public string GetValue()
        {
            return value.ToString();
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

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

        public virtual bool СanSave()
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

        private string luaName; // Lua имя свойства
        private string name; // Имя свойства
        private object value; // Значение

        private bool canSave; // Необходимость сохранения свойства.
    }

    public class ShowedBaseOperationProperty : BaseOperationProperty
    {
        public ShowedBaseOperationProperty(string luaName, string name,
            bool canSave) : base (luaName, name, canSave) { }
    }

    public class NonShowedBaseOperationProperty : BaseOperationProperty
    {
        public NonShowedBaseOperationProperty(string luaName, string name,
            bool canSave) : base(luaName, name, canSave) { }

        public override bool isShowed()
        {
            return false;
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }
    }
}
