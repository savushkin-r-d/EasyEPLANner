using System;
using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Параметры технологического объекта.
    /// </summary>
    public class Params : TreeViewItem
    {
        public Params(string name, string nameLua, bool isRunTimeParams,
            string chBaseName, bool isUseOperation = false)
        {
            this.nameLua = nameLua;
            this.name = name;
            this.isRunTimeParams = isRunTimeParams;
            this.chBaseName = chBaseName;
            this.isUseOperation = isUseOperation;

            parameters = new List<Param>();
        }

        public Params Clone()
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
        /// <param name="name">Имя.</param>
        /// <param name="value">Значение.</param>
        /// <param name="meter">Размерность.</param>
        /// <param name="nameLua">Имя в Lua.</param>
        public Param AddParam(Param parameter)
        {
            parameter.Parent = this;
            parameters.Add(parameter);
            return parameter;
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
        /// Получение параметра по его индексу.
        /// </summary>
        /// <param name="paramIndex">Индекс параметра</param>
        /// <returns></returns>
        public Param GetParam(int paramIndex)
        {
            if(paramIndex < parameters.Count)
            {
                return parameters[paramIndex];
            }
            else
            {
                return null;
            }
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

        public bool IsRuntimeParameters
        {
            get
            {
                return isRunTimeParams;
            }
        }

        public string NameForChannelBase
        {
            get { return chBaseName; }
        }

        public string LuaName
        {
            get
            {
                return nameLua;
            }
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

        override public ITreeViewItem[] Items
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
            var removingParam = child as Param;
            if (removingParam != null)
            {
                removingParam.ClearOperationsBinding();
                parameters.Remove(removingParam);
                return true;
            }

            return false;
        }

        override public bool SetNewValue(string newValue)
        {
            return false;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem Insert()
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

                newParam = new Param(GetIdx, newName, IsRuntimeParameters,
                    newValue, newMeter, newNameLua, useOperation);
                newParam.Parent = this;

                if (useOperation)
                {
                    newParam.SetOperationN(parameters[parameters.Count - 1]
                        .GetOperationN());
                }
            }
            else
            {
                newParam = new Param(GetIdx, "Название параметра", false, 0,
                    "шт", "");
            }

            parameters.Add(newParam);

            newParam.AddParent(this);
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

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem InsertCopy(object obj)
        {
            var newParam = obj as Param;
            if (newParam != null)
            {
                string newName = newParam.GetName();
                string newValueStr = newParam.GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = newParam.GetMeter();
                string newNameLua = newParam.GetNameLua();
                bool useOperation = newParam.IsUseOperation();

                var newPar = new Param(GetIdx, newName, IsRuntimeParameters,
                    newValue, newMeter, newNameLua, useOperation);
                newPar.Parent = this;

                if (useOperation)
                {
                    newPar.SetOperationN(newParam.GetOperationN());
                }

                parameters.Add(newPar);

                newPar.AddParent(this);
                return newPar;
            }
            else
            {
                return null;
            }
        }

        override public ITreeViewItem MoveDown(object child)
        {
            var par = child as Param;
            if (par != null)
            {
                int index = parameters.IndexOf(par);
                if (index <= parameters.Count - 2)
                {
                    parameters.Remove(par);
                    parameters.Insert(index + 1, par);

                    parameters[index].AddParent(this);
                    return parameters[index];
                }
            }

            return null;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var par = child as Param;
            if (par != null)
            {
                int index = parameters.IndexOf(par);
                if (index > 0)
                {
                    parameters.Remove(par);
                    parameters.Insert(index - 1, par);

                    parameters[index].AddParent(this);
                    return parameters[index];
                }
            }

            return null;
        }

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            var par = child as Param;
            var copiedObject = copyObject as Param;
            bool objectsNotNull = par != null && copiedObject != null;
            if (objectsNotNull)
            {
                string newName = (copyObject as Param).GetName();
                string newValueStr = (copyObject as Param).GetValue();
                double newValue = Convert.ToDouble(newValueStr);
                string newMeter = (copyObject as Param).GetMeter();
                string newNameLua = (copyObject as Param).GetNameLua();
                bool useOperation = (copyObject as Param).IsUseOperation();

                var newPar = new Param(GetIdx, newName, IsRuntimeParameters,
                    newValue, newMeter, newNameLua, useOperation);
                newPar.Parent = this;

                if(useOperation)
                {
                    newPar.SetOperationN((copyObject as Param).GetOperationN());
                }

                int index = parameters.IndexOf(par);

                par.ClearOperationsBinding();
                parameters.Remove(par);
                parameters.Insert(index, newPar);

                newPar.AddParent(this);
                return newPar;
            }

            return null;
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }

        public override bool IsFilled
        {
            get
            {
                if(parameters.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.NONE;
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=process_parameter";
        }

        private string name;
        private string nameLua;
        private string chBaseName;
        private bool isRunTimeParams;
        private bool isUseOperation;
        private List<Param> parameters;
    }
}
