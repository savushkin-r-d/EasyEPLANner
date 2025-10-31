using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Editor;
using StaticHelper;

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
            items = [];

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
                this.oper.AddParent(this);
                this.oper.ValueChanged += sender => OnValueChanged(sender);
            }

            this.meter = new ParamProperty(MeterPropertyName, meter, string.Empty);
            luaName = nameLua;

            this.meter.ValueChanged += sender => OnValueChanged(sender);

            if (!isRuntime)
                items.Add(this.value);
            items.Add(this.meter);
            if (isUseOperation)
                items.Add(this.oper);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = 
                $"{prefix}[ {getN(this)} ] =\n" +
                $"{prefix}\t{{\n" +
                $"{prefix}\tname = \'{name}\',\n";
            if (!isRuntime)
            {
                var valueAsString = value.EditText[1].Trim();
                if (valueAsString == "-")
                {
                    valueAsString = "'-'";
                }
                res += $"{prefix}\tvalue = {valueAsString},\n";
            }
            res += $"{prefix}\tmeter = \'{meter.Value}\',\n";
            if (IsUseOperation)
            {
                var operations = oper.EditText[1].Trim().Replace(' ', ',');
                res += $"{prefix}\toper = {{ {operations} }},\n";
            }
            res += 
                $"{prefix}\tnameLua = \'{luaName}\'\n" +
                $"{prefix}\t}},\n";
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
            if (IsUseOperation)
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
            if (IsUseOperation)
            {
                this.oper.SetNewValue("-1");
            }
        }

        /// <summary>
        /// Получить глобальный номер параметра.
        /// </summary>
        public int GetParameterNumber => getN(this);

        #region Реализация ITreeViewItem
        override public string[] DisplayText => [DisplayedName, DisplayedLuaName];

        public string DisplayedName => isRuntime switch
        {
            true => $"{getN(this)}. {name}, {meter?.Value}.",
            false => $"{getN(this)}. {name} - {value?.Value} {meter?.Value}."
        };

        public string DisplayedLuaName => IsStub ? "Заглушка" : luaName;

        public bool IsStub => luaName is "" or "P";

        override public ITreeViewItem[] Items => [.. items];

        override public bool SetNewValue(string newName, int column)
        => column switch
        {
            0 => SetName(newName),
            1 => SetLuaName(newName),
            _ => false,
        };

        public bool SetName(string name)
        {
            this.name = name;
            OnValueChanged(this);

            return true;
        }

        public bool SetLuaName(string luaName)
        {
            if (luaName == string.Empty ||
                Regex.IsMatch(luaName, CommonConst.LuaNamePattern,
                    RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            {
                this.luaName = luaName;
                OnValueChanged(this);

                return true;
            }

            return false;
        }

        override public bool IsEditable => true;

        override public int[] EditablePart => [0, 1];

        override public string[] EditText => [name, luaName];

        override public bool IsCopyable => true;

        override public bool IsMoveable => true;

        override public bool IsReplaceable => true;

        override public bool IsDeletable => true;

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

        /// <summary>
        /// Название
        /// </summary>
        public string GetName() => name;

        /// <summary>
        /// Lua-название
        /// </summary>
        public string GetNameLua()
        {
            if (string.IsNullOrEmpty(luaName))
                return "P";

            return luaName;

        }

        /// <summary>
        /// Значение
        /// </summary>
        public string GetValue()
        {
            if (value != null)
            {
                return value.EditText[1];
            }

            return "0";
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
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
                if (IsUseOperation)
                {
                    return oper.EditText[1];
                }
                
                return "";
            }
            set
            {
                if (IsUseOperation)
                {
                    oper.EditText[1] = value;
                    OnValueChanged(this);
                }
            }
        }

        /// <summary>
        /// Использовать в операции N: Добавляет в список операций новую.
        /// </summary>
        /// <param name="operationNumber">Номер операции</param>
        public void UseInOperation(int operationNumber)
        {
            try
            {
                var ops = Operations.Split(' ')
                    .Select(int.Parse)
                    .Append(operationNumber)
                    .Where(n => n != -1)
                    .Distinct()
                    .OrderBy(n => n)
                    .Aggregate("", (r, n) => $"{r} {n}")
                    .Trim();

                oper.SetNewValue(ops);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in {nameof(UseInOperation)}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Параметр может использоваться в операциях
        /// </summary>
        public bool IsUseOperation => oper is not null;

        public override void UpdateOnGenericTechObject(ITreeViewItem genericObject)
        {
            var genericParam = genericObject as Param;
            if (genericParam is null) return;

            name = genericParam.name;
            luaName = genericParam.luaName;

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



        public Params Params => Parent as Params;

        public ParamProperty ValueItem => value;

        public ParamProperty MeterItem => meter;

        public const string ValuePropertyName = "Значение";
        public const string OperationPropertyName = "Операция";
        public const string MeterPropertyName = "Размерность";

        private GetN getN;

        private bool isRuntime;               /// Рабочий параметр или нет.
        private string name;
        private string luaName;
        private List<ITreeViewItem> items;    ///Данные для редактирования.
        private ParamProperty value;          ///Значение.
        private ParamProperty meter;          ///Размерность.

        private ParamOperationsProperty oper; ///Номер связанной операции.
    }
}
