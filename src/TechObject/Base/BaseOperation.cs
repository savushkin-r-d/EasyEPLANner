using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс реализующий базовую операцию для технологического объекта
    /// </summary>
    public class BaseOperation : Editor.TreeViewItem
    {
        public BaseOperation(Mode owner)
        {
            this.operationName = "";
            this.luaOperationName = "";
            this.owner = owner;
        }

        public BaseOperation(string name, string luaName, Mode owner)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.owner = owner;
        }

        // Конструктор для инициализации базовой операции и параметров
        public BaseOperation(string name, string luaName, 
            BaseProperty[] baseOperationProperties)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.baseOperationProperties = baseOperationProperties;
        }

        public string Name
        {
            get
            {
                return operationName;
            }

            set
            {
                operationName = value;
            }
        }

        public string LuaName
        {
            get
            {
                return luaOperationName;
            }

            set
            {
                luaOperationName = value;
            }
        }

        public int ParametersCount
        {
            get
            {
                if (BaseOperationProperties == null)
                {
                    return 0;
                }

                return baseOperationProperties.Length;
            }
        }

        // Инициализация полей при выборе базовой операции
        public void Init(string baseOperName)
        {
            // Базовый объект для поиска операции по этому объекту
            TechObject techObject = owner.Owner.Owner;
            string baseTechObjectName = techObject.GetBaseTechObjectName();

            if (baseTechObjectName == "")
            {
                return;
            }

            BaseOperation operation = techObject.BaseTechObject
                .GetBaseOperationByName(baseOperName);
            if (operation != null)
            {
                Name = operation.Name;
                LuaName = operation.LuaName;
                baseOperationProperties = operation.BaseOperationProperties;
            }
            else
            {
                Name = "";
                LuaName = "";
                baseOperationProperties = new BaseProperty[0];
            }

            SetItems();
        }

        // Добавление полей в массив для отображения на дереве
        private void SetItems()
        {
            var showedParameters = new List<BaseProperty>();
            foreach(var parameter in baseOperationProperties)
            {
                if (parameter.isShowed())
                {
                    showedParameters.Add(parameter);
                }
            }
            items = showedParameters.ToArray();
        }

        // Сохранение в виде таблицы Lua
        public string SaveAsLuaTable(string prefix)
        {
            var res = "";
            res += prefix + "props =\n" + prefix + "\t{\n";
            foreach (var operParam in baseOperationProperties)
            {
                if (operParam.СanSave())
                {
                    res += "\t" + prefix + operParam.GetLuaName() + " = \'" + 
                        operParam.GetValue() + "\',\n";
                }
            }
            res += prefix + "\t},\n";
            return res;
        }

        // Установка параметров базовой операции
        public void SetExtraProperties(Editor.ObjectProperty[] extraParams)
        {
            foreach (Editor.ObjectProperty extraParam in extraParams)
            {
                var property = baseOperationProperties
                    .Where(x => x.GetLuaName()
                    .Equals(extraParam.DisplayText[0]))
                    .FirstOrDefault();

                if (property != null)
                {
                    property.SetValue(extraParam.DisplayText[1]);
                }
            }
        }

        // Возврат параметров базовой операции
        public BaseProperty[] BaseOperationProperties
        {
            get
            {
                return baseOperationProperties;
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                if (items.Count() > 0)
                {
                    string res = string.Format("Дополнительные сигналы ({0})", 
                        items.Count());
                    return new string[] { res, "" };
                }
                else
                {
                    string res = string.Format("Дополнительные сигналы");
                    return new string[] { res, "" };
                }
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return items;
            }
        }
        #endregion

        private Editor.ITreeViewItem[] items = new Editor.ITreeViewItem[0];
        // Свойства базовой операции для имитационного хранилища
        private BaseProperty[] baseOperationProperties;

        private string operationName; /// Имя базовой операции
        private string luaOperationName; /// Имя базовой операции для файла Lua

        private Mode owner;
    }
}
