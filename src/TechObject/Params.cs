using System;
using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Параметры технологического объекта.
    /// </summary>
    public class Params : Editor.TreeViewItem
    {
        public Params(string name, string nameLua, string initFunctionName,
            bool isRunTimeParams, bool isUseOperation = false)
        {
            this.name = name;
            this.nameLua = nameLua;
            this.initFunctionName = initFunctionName;
            this.isRunTimeParams = isRunTimeParams;
            this.isUseOperation = isUseOperation;

            parameters = new List<Param>();
        }

        public Params Clone(ParamsManager owner)
        {
            Params clone = (Params)MemberwiseClone();
            clone.parameters = new List<Param>();

            foreach (Param par in parameters)
            {
                clone.InsertCopy(par);

            }

            return clone;
        }

        /// <summary>
        /// Добавление параметра в список.
        /// </summary>
        /// <param name="param">Добавляемый параметр.</param>
        public Param AddParam(Param param)
        {
            parameters.Add(param);
            return param;
        }

        public bool IsRuntimeParameter()
        {
            return isRunTimeParams;
        }

        /// <summary>
        /// Получение индекса параметра.
        /// </summary>
        /// <param name="param">Параметр, индекс которого требуется узнать.
        /// </param>
        /// <returns>Индекс искомого параметра.</returns>
        public int GetIdx(object param)
        {
            return parameters.IndexOf(param as Param) + 1;
        }

        /// <summary>
        /// Получение параметра по его Lua-имени.
        /// </summary>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param GetParam(string nameLua)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].GetNameLua() == nameLua)
                {
                    return parameters[i];
                }
            }

            return null;
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
            foreach (Param param in parameters)
            {
                res += param.SaveAsLuaTable(prefix + "\t");
            }
            res += prefix + "\t},\n";

            return res;
        }

        /// <summary>
        /// Проверить параметры объекта
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <returns></returns>
        public string Check(string objName)
        {
            var errors = new List<string>();
            var emptyParamName = "";
            var newParamName = "P";
            foreach(var param in parameters)
            {
                int[] equalParameters = parameters
                    .Where(x => x.GetNameLua() == param.GetNameLua() &&
                    x.GetNameLua() != newParamName &&
                    x.GetNameLua() != emptyParamName)
                    .Select(y => y.GetParameterNumber)
                    .ToArray();

                if (equalParameters.Length > 1)
                {
                    string errorMessage = $"У объекта \"{objName}\" совпадают" +
                        $" имена параметров с номерами " +
                        $"{string.Join(",", equalParameters)}\n";
                    errors.Add(errorMessage);
                }
            }

            errors = errors.Distinct().ToList();
            return string.Concat(errors);
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Format("{0} ({1})", name, parameters.Count);

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return parameters.ToArray();
            }
        }

        override public bool IsDeletable
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

        override public bool Delete(object child)
        {
            Param removedParam = child as Param;
            if (removedParam != null)
            {
                removedParam.ClearOperationsBinding();
                parameters.Remove(removedParam);
                return true;
            }

            return false;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public bool SetNewValue(string newValue)
        {
            return false;
        }

        override public Editor.ITreeViewItem Insert()
        {
            Param newParam;

            if (parameters.Count > 0)
            {
                string newName = parameters[parameters.Count - 1].GetName();
                string newValueStr = parameters[parameters.Count - 1]
                    .GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = parameters[parameters.Count - 1].GetMeter();
                string newNameLua = parameters[parameters.Count - 1]
                    .GetNameLua();
                bool useOperation = isUseOperation;

                newParam = new Param(GetIdx, newName, isRunTimeParams, 
                    newValue, newMeter, newNameLua, useOperation);
                if (useOperation)
                {
                    newParam.Parent = this;
                    newParam.SetOperationN(parameters[parameters.Count - 1]
                        .GetOperationN());
                }
            }
            else
            {
                newParam = new Param(GetIdx, "Название параметра", 
                    isRunTimeParams, 0, "шт", "", isUseOperation);
            }

            parameters.Add(newParam);
            return newParam;
        }

        public void Clear()
        {
            foreach (var parameter in parameters)
            {
                parameter.ClearOperationsBinding();
            }
            parameters.Clear();
        }

        override public bool IsCopyable
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

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem InsertCopy(object obj)
        {
            if (obj is Param)
            {
                Param newParam = obj as Param;

                string newName = newParam.GetName();
                string newValueStr = newParam.GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = newParam.GetMeter();
                string newNameLua = newParam.GetNameLua();
                bool useOperation = newParam.IsUseOperation();

                Param newPar = new Param(GetIdx, newName, isRunTimeParams, 
                    newValue, newMeter, newNameLua, useOperation);
                if (useOperation)
                {
                    newPar.Parent = this;
                    newPar.SetOperationN(newParam.GetOperationN());
                }

                parameters.Add(newPar);
                return newPar;
            }
            else
            {
                return null;
            }
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            Param par = child as Param;

            if (par != null)
            {
                int index = parameters.IndexOf(par);
                if (index <= parameters.Count - 2)
                {
                    parameters.Remove(par);
                    parameters.Insert(index + 1, par);

                    return parameters[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            Param par = child as Param;

            if (par != null)
            {
                int index = parameters.IndexOf(par);
                if (index > 0)
                {
                    parameters.Remove(par);
                    parameters.Insert(index - 1, par);

                    return parameters[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            Param par = child as Param;
            if (copyObject is Param && par != null)
            {
                string newName = (copyObject as Param).GetName();
                string newValueStr = (copyObject as Param).GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = (copyObject as Param).GetMeter();
                string newNameLua = (copyObject as Param).GetNameLua();
                bool useOperation = (copyObject as Param).IsUseOperation();

                Param newPar = new Param(GetIdx, newName, isRunTimeParams,
                    newValue, newMeter, newNameLua, useOperation);
                if (useOperation)
                {
                    newPar.Parent = this;
                    newPar.SetOperationN((copyObject as Param).GetOperationN());
                }

                int index = parameters.IndexOf(par);

                parameters.Remove(par);
                parameters.Insert(index, newPar);

                return newPar;
            }

            return null;
        }

        public override bool NeedRebuildMainObject
        {
            get
            {
                return true;
            }
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.Properties.Resources.ResourceManager
                .GetString("ostisLink");
            return ostisLink + "?sys_id=process_parameter";
        }

        private string nameLua;
        private string initFunctionName;
        private bool isRunTimeParams;
        private bool isUseOperation;

        private string name;

        private List<Param> parameters;
    }
}
