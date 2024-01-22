using System;
using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Отдельный параметр технологического объекта.
    /// </summary>
    public class Param : TreeViewItem
    {
        public Param(GetN getN, string name, bool isRuntime = false,
            double value = 0, string meter = "шт", string nameLua = "",
            bool isUseOperation = false)
        {
            items = new List<ITreeViewItem>();

            this.isRuntime = isRuntime;
            this.name = name;
            this.getN = getN;
            
            if(!isRuntime)
            {
                this.value = new ParamProperty(ValuePropertyName, value);
                this.value.AddParent(this);
                this.value.ValueChanged += sender => OnValueChanged(sender);
            }
            
            if (isUseOperation)
            {
                this.oper = new ParamOperationsProperty(OperationPropertyName, -1, -1);
                this.oper.Parent = this;
                this.oper.ValueChanged += sender => OnValueChanged(sender);
            }

            this.meter = new ParamProperty(MeterPropertyName, meter, string.Empty);
            this.nameLua = new ParamProperty(NameLuaPropertyName, nameLua, string.Empty);
            
            this.meter.ValueChanged += sender => OnValueChanged(sender);
            this.nameLua.ValueChanged += sender => OnValueChanged(sender);

            if (!isRuntime)
                items.Add(this.value);
            items.Add(this.meter);
            if (isUseOperation)
                items.Add(this.oper);
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
                var valueAsString = value.EditText[1].Trim();
                if (valueAsString == "-")
                {
                    valueAsString = "'-'";
                }
                res += $"{prefix}\tvalue = {valueAsString},\n";
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
                    oper.SetNewValue(StaticHelper.LuaHelper
                        .ConvertLuaTableToString(operN));
                }
                else
                {
                    oper.SetNewValue(operN.ToString());
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

        public void ClearOperationsBinding()
        {
            if (oper != null)
            {
                this.oper.SetNewValue("-1");
            }
        }

        /// <summary>
        /// Получить глобальный номер параметра.
        /// </summary>
        public int GetParameterNumber
        {
            get
            {
                return getN(this);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = "";
                if(!isRuntime)
                {
                    res = $"{getN(this)}. {name} - {value.EditText[1]} " +
                        $"{meter.EditText[1]}.";
                }
                else
                {
                    res = $"{getN(this)}. {name}, {meter.EditText[1]}.";
                }

                return new string[] { res, "" };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;

            OnValueChanged(this);
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

        public override bool Delete(object child)
        {
            if (child.GetType() == typeof(ObjectProperty))
            {
                var objectProperty = child as ObjectProperty;
                objectProperty.Delete(this);
            }

            return false;
        }

        #endregion

        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Возвращает объект-свойство LuaName
        /// </summary>
        public ObjectProperty LuaNameProperty => nameLua;

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
                if (oper != null)
                {
                    oper.EditText[1] = value;
                    OnValueChanged(this);
                }
            }
        }

        public bool IsUseOperation()
        {
            return oper != null;
        }

        public override void UpdateOnGenericTechObject(ITreeViewItem genericObject)
        {
            var genericParam = genericObject as Param;
            if (genericParam is null) return;

            name = genericParam.name;
            nameLua.SetNewValue(genericParam.nameLua.Value);

            if (genericParam.value.Value != "-")
                value.SetNewValue(genericParam.value.Value);
            if (genericParam.oper.IsFilled)
                oper.SetNewValue(genericParam.oper.Value);
            if (genericParam.meter.IsFilled)
                meter.SetNewValue(genericParam.meter.Value);
        }

        public override void CreateGenericByTechObjects(IEnumerable<ITreeViewItem> itemList)
        {
            var paramList = itemList.Cast<Param>().ToList();

            var refParam = paramList.FirstOrDefault();

            if (paramList.TrueForAll(param => param.value.Value == refParam.value.Value))
                value.SetNewValue(refParam.value.Value);
            else 
                value.SetNewValue("-");

            if (paramList.TrueForAll(param => param.oper.Value == refParam.oper.Value))
                oper.SetNewValue(refParam.oper.Value);
            else
                oper.SetNewValue("");

            if (paramList.TrueForAll(param => param.meter.Value == refParam.meter.Value))
                meter.SetNewValue(refParam.meter.Value);
            else
                meter.SetNewValue("");
        }

        public bool Reserved => GetNameLua() == "P" && GetName() == "Название параметра";

        public Params Params => Parent as Params;

        public ParamProperty ValueItem => value;

        public ParamProperty MeterItem => meter;

        public const string ValuePropertyName = "Значение";
        public const string OperationPropertyName = "Операция";
        public const string MeterPropertyName = "Размерность";
        public const string NameLuaPropertyName = "Lua имя";


        private GetN getN;

        private bool isRuntime;               /// Рабочий параметр или нет.
        private string name;
        private List<ITreeViewItem> items;    ///Данные для редактирования.
        private ParamProperty nameLua;        ///Имя в Lua.
        private ParamProperty value;          ///Значение.
        private ParamProperty meter;          ///Размерность.

        private ParamOperationsProperty oper; ///Номер связанной операции.
    }
}
