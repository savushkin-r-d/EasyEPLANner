using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechObject
{
    /// <summary>
    /// Отдельный параметр технологического объекта.
    /// </summary>
    public class Param : Editor.TreeViewItem
    {
        public Param(GetN getN, string name, bool isRuntime = false,
            double value = 0, string meter = "шт", string nameLua = "",
            bool isUseOperation = false)
        {
            this.isRuntime = isRuntime;

            this.name = name;
            this.getN = getN;
            if (!isRuntime)
            {
                this.value = new Editor.ObjectProperty("Значение", value);
            }
            if (isUseOperation)
            {
                this.oper = new ParamProperty("Операция", -1);
            }

            this.meter = new Editor.ObjectProperty("Размерность", meter);
            this.nameLua = new Editor.ObjectProperty("Lua имя", nameLua);

            items = new List<Editor.ITreeViewItem>();
            if (!isRuntime)
            {
                items.Add(this.value);
            }
            items.Add(this.meter);
            if (isUseOperation)
            {
                items.Add(oper);
            }
            items.Add(this.nameLua);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "[ " + getN(this) + " ] =\n";
            res += prefix + "\t{\n";
            res += prefix + "\tname = \'" + name + "\',\n";
            if (!isRuntime)
            {
                res += prefix + "\tvalue = " + value.EditText[1] + ",\n";
            }
            res += prefix + "\tmeter = \'" + meter.EditText[1] + "\',\n";
            if (oper != null)
            {
                var operations = oper.EditText[1].Trim().Replace(' ', ',');
                res += prefix + "\toper = { " + operations + " },\n";
            }
            res += prefix + "\tnameLua = \'" + nameLua.EditText[1] + "\'\n";

            res += prefix + "\t},\n";
            return res;
        }

        public void SetOperationN(object operN)
        {
            if (oper != null)
            {
                if (operN.GetType().Name == "LuaTable")
                {
                    oper.SetValue(StaticHelper.LuaHelper
                        .ConvertLuaTableToString(operN));
                }
                else
                {
                    oper.SetValue(operN);
                }
            }
        }

        public string GetOperationN()
        {
            if (oper != null)
            {
                try
                {
                    return oper.EditText[1];
                }
                catch (Exception)
                {
                    return "-1";
                }
            }

            return "-1";
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = "";
                if (!isRuntime)
                {
                    res = getN(this) + ". " + name + " - " + 
                        value.EditText[1] + " " + meter.EditText[1] + ".";
                }
                else
                {
                    res = getN(this) + ". " + name +
                        ", " + meter.EditText[1] + ".";
                }

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;

            return true;
        }

        override public bool IsEditable
        {
            get
            {
                return true;
            }
        }

        override public int[] EditablePart
        {
            get
            {
                //Можем редактировать содержимое первой колонки.
                return new int[] { 0, -1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, "" };
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public bool IsMoveable
        {
            get
            {
                return true;
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public object Copy()
        {
            return this;
        }
        #endregion

        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Возвращает объект-свойство LuaName
        /// </summary>
        public Editor.ObjectProperty LuaNameProperty
        {
            get
            {
                return nameLua;
            }
        }

        /// <summary>
        /// Возвращает имя параметра для Lua
        /// </summary>
        /// <returns></returns>
        public string GetNameLua()
        {
            if (nameLua.EditText[1] != "")
            {
                return nameLua.EditText[1];
            }

            return "P";
        }

        public string GetValue()
        {
            if (value != null)
            {
                return value.EditText[1];
            }

            return "0";
        }

        public string GetMeter()
        {
            return meter.EditText[1];
        }

        /// <summary>
        /// Возвращает или устанавливает список операций, 
        /// к которым принадлежит параметр.
        /// </summary>
        public string Operations
        {
            get
            {
                if (oper != null)
                {
                    return oper.EditText[1];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                oper.EditText[1] = value;
            }
        }

        public bool IsUseOperation()
        {
            return oper != null;
        }

        private GetN getN;

        private bool isRuntime;
        private string name;
        private List<Editor.ITreeViewItem> items; ///Данные для редактирования.
        private Editor.ObjectProperty nameLua;     ///Имя в Lua.
        private Editor.ObjectProperty value;       ///Значение.
        private Editor.ObjectProperty meter;       ///Размерность.

        private Editor.ObjectProperty oper;        ///Номер связанной операции.
    }
}
