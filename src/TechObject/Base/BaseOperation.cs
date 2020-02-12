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
            this.baseOperationProperties = new BaseProperty[0];
            this.owner = owner;
        }

        public BaseOperation(string name, string luaName, Mode owner)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.baseOperationProperties = new BaseProperty[0];
            this.owner = owner;
        }

        /// <summary>
        /// Конструктор для инициализации базовой операции и параметров
        /// </summary>
        /// <param name="name">Имя операции</param>
        /// <param name="luaName">Lua имя операции</param>
        /// <param name="baseOperationProperties">Свойства операции</param>
        public BaseOperation(string name, string luaName, 
            BaseProperty[] baseOperationProperties)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.baseOperationProperties = baseOperationProperties;
        }

        /// <summary>
        /// Получить имя операции
        /// </summary>
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

        /// <summary>
        /// Получить Lua имя операции
        /// </summary>
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

        /// <summary>
        /// Инициализация базовой операции по имени
        /// </summary>
        /// <param name="baseOperName">Имя операции</param>
        public void Init(string baseOperName)
        {
            TechObject techObject = owner.Owner.Owner;
            string baseTechObjectName = techObject.BaseTechObject.Name;
            if (baseTechObjectName != "")
            {
                BaseOperation operation = techObject.BaseTechObject
                    .GetBaseOperationByName(baseOperName);
                if (operation != null)
                {
                    Name = operation.Name;
                    LuaName = operation.LuaName;
                    baseOperationProperties =
                        FindBaseOperationProperties(operation);
                }
            }
            else
            {
                Name = "";
                LuaName = "";
                baseOperationProperties = new BaseProperty[0];
            }

            SetItems();
        }

        /// <summary>
        /// Поиск свойств операции объекта
        /// </summary>
        /// <param name="operation">Базовая операция</param>
        /// <returns></returns>
        private BaseProperty[] FindBaseOperationProperties(BaseOperation 
            operation)
        {
            var baseTechObject = owner.Owner.Owner.BaseTechObject;
            var baseTechObjectProperties = baseTechObject.BaseProperties;
            var baseOperationProperties = operation.Properties;
            var properties = new List<BaseProperty>();

            foreach(var property in baseOperationProperties)
            {
                var samePropertyAtTechObject = baseTechObjectProperties
                    .Where(x => x.LuaName == property.LuaName)
                    .FirstOrDefault();
                if (samePropertyAtTechObject != null)
                {
                    property.Clear();
                    properties.Add(property);
                }
            }

            return properties.ToArray();
        }

        /// <summary>
        /// Добавление полей в массив для отображения на дереве
        /// </summary>
        private void SetItems()
        {
            var showedParameters = new List<BaseProperty>();
            foreach(var parameter in Properties)
            {
                if (parameter.isShowed())
                {
                    showedParameters.Add(parameter);
                }
            }
            items = showedParameters.ToArray();
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua
        /// </summary>
        /// <param name="prefix">Префикс (отступ)</param>
        /// <returns></returns>
        public string SaveAsLuaTable(string prefix)
        {
            var res = "";
            
            if (Properties == null)
            {
                return res;
            }

            var propertiesCountForSave = Properties
                .Where(x => x.CanSave() == true).Count();
            if (propertiesCountForSave <= 0)
            {
                return res;
            } 

            res += prefix + "props =\n" + prefix + "\t{\n";
            foreach (var operParam in Properties)
            {
                if (operParam.CanSave())
                {
                    res += "\t" + prefix + operParam.LuaName + " = \'" + 
                        operParam.Value + "\',\n";
                }
            }
            res += prefix + "\t},\n";
            return res;
        }

        /// <summary>
        /// Установка свойств базовой операции
        /// </summary>
        /// <param name="extraParams">Свойства операции</param>
        public void SetExtraProperties(Editor.ObjectProperty[] extraParams)
        {
            foreach (Editor.ObjectProperty extraParam in extraParams)
            {
                var property = Properties
                    .Where(x => x.LuaName
                    .Equals(extraParam.DisplayText[0]))
                    .FirstOrDefault();

                if (property != null)
                {
                    property.SetValue(extraParam.DisplayText[1]);
                }
            }
        }

        /// <summary>
        /// Получить свойства базовой операции
        /// </summary>
        public BaseProperty[] Properties
        {
            get
            {
                return baseOperationProperties;
            }
            set
            {
                baseOperationProperties = value;
            }
        }

        /// <summary>
        /// Копирование объекта
        /// </summary>
        /// <param name="owner">Новая операция-владелец объекта</param>
        /// <returns></returns>
        public BaseOperation Clone(Mode owner)
        {
            var properties = new BaseProperty[baseOperationProperties.Length];
            for (int i = 0; i < baseOperationProperties.Length; i++)
            {
                properties[i] = baseOperationProperties[i].Clone();
            }
            var operation = new BaseOperation(this.operationName, 
                this.luaOperationName, properties);
            operation.owner = this.owner;
            
            operation.SetItems();
            return operation;
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
        
        private BaseProperty[] baseOperationProperties;
        private string operationName;
        private string luaOperationName;

        private Mode owner;
    }
}
