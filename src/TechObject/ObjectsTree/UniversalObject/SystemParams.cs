using System;
using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Системные параметры технологического объекта.
    /// </summary>
    public class SystemParams : TreeViewItem
    {
        public SystemParams() : this("Системные параметры", "system_parameters")
        { }

        public SystemParams(string name, string nameLua)
        {
            this.nameLua = nameLua;
            this.name = name;

            parameters = new List<SystemParam>();
        }

        public SystemParams Clone()
        {
            var clone = (SystemParams)MemberwiseClone();
            clone.parameters = new List<SystemParam>();

            foreach (SystemParam par in parameters)
            {
                clone.InsertCopy(par);
            }

            return clone;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            if (parameters.Count == 0)
            {
                return "";
            }

            string res = prefix + nameLua + " =\n";
            res += prefix + "\t{\n";
            foreach (SystemParam param in parameters)
            {
                res += param.SaveAsLuaTable(prefix + "\t");
            }
            res += prefix + "\t},\n";

            return res;
        }

        /// <summary>
        /// Получение индекса параметра.
        /// </summary>
        /// <param name="param">Параметр, индекс которого требуется узнать.
        /// </param>
        /// <returns>Индекс искомого параметра.</returns>
        public int GetIdx(object param)
        {
            return parameters.IndexOf(param as SystemParam) + 1;
        }

        public SystemParam GetParam(string nameOrLuaName)
        {
            SystemParam foundParam = parameters
                .Where(x => x.Name == nameOrLuaName ||x.LuaName == nameOrLuaName)
                .FirstOrDefault();
            return foundParam;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = name;
                if (parameters.Count > 0)
                {
                    res += " (" + parameters.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        override public bool IsInsertableCopy => true;

        override public ITreeViewItem InsertCopy(object obj)
        {
            var newParam = obj as SystemParam;
            if (newParam != null)
            {
                string newName = newParam.Name;
                string newValueStr = newParam.Value.Value;
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = newParam.Meter;
                string newNameLua = newParam.LuaName;

                var newPar = new SystemParam(GetIdx, newName, newValue,
                    newMeter, newNameLua);
                newPar.Parent = this;

                parameters.Add(newPar);

                newPar.AddParent(this);
                return newPar;
            }
            else
            {
                return null;
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return parameters.ToArray();
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.ParamsManager;
            }
        }
        #endregion

        public void AddParam(SystemParam param)
        {
            parameters.Add(param);
        }

        public void SetUpFromBaseTechObject(SystemParams systemParams)
        {
            parameters.Clear();

            var insertingParameters = systemParams.parameters;
            foreach(var parameter in insertingParameters)
            {
                parameters.Add(parameter);
            }
        }

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=process_parameter";
        }

        public void Clear() => parameters.Clear();

        public int Count => parameters.Count;

        private string name;
        private string nameLua;
        private List<SystemParam> parameters;
    }
}
