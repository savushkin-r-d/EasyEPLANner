using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
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

            clone.ValueChanged += sender => clone.OnValueChanged(sender);

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

            OnValueChanged(this);
            parameter.ValueChanged += sender => OnValueChanged(sender);

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
            if (paramIndex < parameters.Count)
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
            const string dfltLuaName = "P";

            // Проверка на совпадающие Lua-name для старых проектов, если совпадают - сброс:
            foreach (var param in parameters)
            {
                int[] equalParameters = parameters
                    .Where(p => p.GetNameLua() == param.GetNameLua() 
                        && p.GetNameLua() != dfltLuaName)
                    .Select(y => y.GetParameterNumber)
                    .ToArray();

                if (equalParameters.Count() <= 1)
                    continue;

                var errorStrBldr = new StringBuilder("")
                    .Append($"У объекта \"{objName}\" совпадают имена параметров с номерами")
                    .Append($"{string.Join(",", equalParameters)}.\n")
                    .Append($"Lua-имена этих параметров будут сброшены.\n");

                foreach (var parId in equalParameters)
                {
                    parameters[parId - 1].LuaNameProperty.SetNewValue("");
                }

                errors.Add(errorStrBldr.ToString());
            }

            // Проверка на дефолтные Lua-name
            int[] parametersWithDefaultLuaName = parameters
                .Where(p => p.GetNameLua() == dfltLuaName && p.GetName() != "Название параметра")
                .Select(p => p.GetParameterNumber).ToArray();
            if( parametersWithDefaultLuaName.Any())
            {
                errors.Add($"\"{objName}\": в параметрах [{string.Join(",", parametersWithDefaultLuaName)}]" +
                    $" Lua-имя не заполнено или имеет значение по-умолчанию.\n");
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
                OnValueChanged(this);
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
            var newParam = new Param(GetIdx, "Название параметра",
                isRunTimeParams, 0, "шт", "", isUseOperation);

            parameters.Add(newParam);
            newParam.AddParent(this);

            newParam.ValueChanged += sender => OnValueChanged(sender);
            OnValueChanged(this);

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
                string luaName = newParam.GetNameLua();
                string newNameLua = HaveSameLuaName(luaName)? "P" : luaName;
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

                newPar.ValueChanged += sender => OnValueChanged(sender);

                return newPar;
            }
            else
            {
                return null;
            }
        }

        override public ITreeViewItem MoveDown(object child)
        {
            if (child is Param par)
            {
                int index = parameters.IndexOf(par);
                if (index >= parameters.Count - 1)
                    return null;

                SwapParameters(index, index + 1);
                OnValueChanged(this);

                return par;
            }

            return null;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            if (child is Param par)
            {
                int index = parameters.IndexOf(par);
                if (index <= 0)
                    return null;

                SwapParameters(index, index - 1);
                OnValueChanged(this);

                return par;
            }

            return null;
        }

        public void SwapParameters(int index1, int index2)
        {
            (parameters[index1], parameters[index2]) = (parameters[index2], parameters[index1]);
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

                if (useOperation)
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

        public void UpdateOnGenericTechObject(Params genericParams)
        {
            foreach (Param genericPar in genericParams.parameters)
            {
                var par = GetParam(genericParams.GetIdx(genericPar) - 1);
                var parByLuaName = parameters.Find(p => p.GetNameLua() == genericPar.GetNameLua() && p.GetNameLua() != "P");

                if (par != null && parByLuaName != null &&
                    par != parByLuaName)
                {
                    SwapParameters(parameters.IndexOf(par), parameters.IndexOf(parByLuaName));
                    Editor.Editor.GetInstance().RefreshObject(this);
                }

                if (par is null)
                {
                    par = Insert() as Param;
                }

                par.UpdateOnGenericTechObject(genericPar);
            }
        }

        public override void CreateGenericByTechObjects(IEnumerable<ITreeViewItem> itemList)
        {
            var paramsList = itemList.Cast<Params>().ToList();

            var refParams = paramsList.OrderBy(pars => pars.parameters.Count).FirstOrDefault();
            if (refParams is null)
                return;

            foreach (var parId in Enumerable.Range(0, refParams.parameters.Count))
            {
                var refParameter = refParams.parameters[parId];
                foreach (var paramsId in Enumerable.Range(0, paramsList.Count))
                {
                    var parameter = paramsList[paramsId].parameters[parId];
                    if (refParameter.GetName() != parameter.GetName()
                        || refParameter.GetNameLua() != parameter.GetNameLua())
                        return;
                }

                var newGenericParam = AddParam(new Param(GetIdx, refParameter.GetName(), false, 0, "", refParameter.GetNameLua(), true));
                newGenericParam.CreateGenericByTechObjects(paramsList.Select(pars => pars.parameters[parId]));
            }
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
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

        public bool HaveSameLuaName(string luaName)
        {
            return parameters
                .Where(p => p.GetNameLua() == luaName && p.GetNameLua() != "P")
                .Select(y => y.GetParameterNumber)
                .Any();
        }

        public ParamsManager ParamsManager => Parent as ParamsManager;

        private string name;
        private string nameLua;
        private string chBaseName;
        private bool isRunTimeParams;
        private bool isUseOperation;
        private List<Param> parameters;
    }
}
