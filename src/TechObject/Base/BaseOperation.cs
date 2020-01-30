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

        // Конструктор для инициализации хранилища с именами базовых операций
        public BaseOperation(string name, string luaName)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
        }

        // Конструктор для инициализации базовой операции и параметров
        public BaseOperation(string name, string luaName, 
            BaseOperationProperty[] baseOperationProperties)
        {
            this.operationName = name;
            this.luaOperationName = luaName;
            this.baseOperationProperties = baseOperationProperties;
        }

        public string GetName()
        {
            return operationName;
        }

        public string GetLuaName()
        {
            return luaOperationName;
        }

        public void SetName(string operationName)
        {
            this.operationName = operationName;
        }

        public void SetLuaName(string luaName)
        {
            this.luaOperationName = luaName;
        }

        // Получение количества параметров у операции
        public int GetParamsCount()
        {
            if (BaseOperationProperties == null)
            {
                return 0;
            }
            return baseOperationProperties.Length;
        }

        // Инициализация полей при выборе базовой операции
        public void Init(string baseOperName)
        {
            // Базовый объект для поиска операции по этому объекту
            TechObject baseTechObject = owner.Owner.Owner;
            string baseTechObjectName = baseTechObject.GetBaseTechObjectName();

            if (baseTechObjectName != "")
            {
                SetName(baseOperName); // Установка имени базовой операции
                var luaName = DataBase.Imitation
                    .FindOperationLuaName(baseOperName);
                SetLuaName(luaName); // Установка имени операции для файла Lua

                // Инициализирую список параметров
                baseOperationProperties = new BaseOperationProperty[0]; 

                // Инициализация операции в зависимости от выбранной 
                //операции и базового объекта
                baseOperationProperties = DataBase.Imitation.GetOperParams(
                    baseOperName, baseTechObjectName);

                SetItems();
            }
        }

        // Добавление полей в массив для отображения на дереве
        private void SetItems()
        {
            items = new Editor.ITreeViewItem[baseOperationProperties.Length];
            var counter = 0;
            foreach (var operParam in baseOperationProperties)
            {
                items[counter] = operParam;
                counter++;
            }
        }

        // Сохранение в виде таблицы Lua
        public string SaveAsLuaTable(string prefix)
        {
            var res = "";
            res += prefix + "props =\n" + prefix + "\t{\n";
            foreach (var operParam in baseOperationProperties)
            {
                res += "\t" + prefix + operParam.GetLuaName() + " = \'" + 
                    operParam.GetValue() + "\',\n";
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
        public BaseOperationProperty[] BaseOperationProperties
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
        private BaseOperationProperty[] baseOperationProperties;

        private string operationName; /// Имя базовой операции
        private string luaOperationName; /// Имя базовой операции для файла Lua

        private Mode owner;
    }
}
