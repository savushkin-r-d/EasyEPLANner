using System;
using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Настраиваемое действие
    /// </summary>
    public class ActionCustom : GroupableAction
    {
        /// <summary>
        /// Создание нового действия
        /// </summary>
        /// <param name="name">Название действия</param>
        /// <param name="owner">Шаг</param>
        /// <param name="luaName">Название в Lua</param>
        public ActionCustom(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            parameters = new List<BaseParameter>();
        }

        override public IAction Clone()
        {
            var clone = new ActionCustom(name, owner, luaName);

            clone.SubActions = new List<IAction>();
            foreach (var action in SubActions)
            {
                clone.SubActions.Add(action.Clone());
            }

            clone.parameters = new List<BaseParameter>();
            foreach (var parameter in parameters)
            {
                clone.parameters.Add(parameter.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Добавить действия для группы
        /// </summary>
        /// <param name="action"></param>
        public void CreateAction(Action action)
        {
            SubActions.Add(action);
        }

        /// <summary>
        /// Добавить параметр для группы
        /// </summary>
        /// <param name="parameter"></param>
        public void CreateParameter(BaseParameter parameter)
        {
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// Если хотя бы одно поддействие или параметр не имеет Lua-имени,
        /// то необходимо сохранять все поддействия/параметры в их порядке
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public override string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            string group = string.Empty;

            if (SubActions.All(subaction => subaction.Empty) &&
                Parameters.All(parameter => parameter.Value == string.Empty))
            {
                return res;
            }

            if (SubActions.Any(sa => sa.LuaName == string.Empty) ||
                Parameters.Any(param => param.LuaName == string.Empty))
            {
                return SaveAsLuaTableShortFormat(prefix);
            }

            foreach (var subAction in SubActions)
            {
                 group += $"\n{subAction.SaveAsLuaTable(prefix + "\t")}";
            }

            foreach (var parameter in Parameters)
            {
                string parameterValue =
                    (parameter.CurrentValueType == BaseParameter.ValueType.Other) ?
                    $"\'{parameter.Value}\'" : parameter.Value;

                if (parameterValue != string.Empty)
                {
                    group += $"{prefix}\t{parameter.LuaName} = {parameterValue} --{parameter.Name},\n";
                }
            }

            if(group != string.Empty)
            {
                res += prefix;
                res += (luaName != string.Empty)? $"{LuaName} = " : "";
                res += (Name != string.Empty) ? $"--{Name}" : "";
                res += (LuaName != string.Empty || Name != string.Empty)?
                    "\n" : "";
                res += $"{prefix}\t{{{group}\n" +
                    $"{prefix}\t}},\n";
            }
                
            return res;
        }


        /// <summary>
        /// Сохраняет действие в кратком формате:
        ///  - без Lua-имен переменных
        ///  - сохраняются даже пустые действия({}) и параметры(-1),
        ///     так как в случае отсутсвия имен, важен порядок
        ///  - срабатывает, когда хотя бы у одного поддействия или параметра
        ///     отсутствует Lua-имя;
        /// </summary>
        /// <returns></returns>
        public string SaveAsLuaTableShortFormat(string prefix)
        {
            var subactionsData = string.Empty;
            var parametersData = string.Empty;

            bool inLine = SubActions.Count <= 1 && Parameters.Count <= 1;

            foreach (var subAction in SubActions)
            {
                var subactionData = (subAction.Empty) ?
                    $" {{}}," : $" {subAction.SaveAsLuaTableInline()},";

                subactionsData += (inLine) ?
                    subactionData : $"{prefix}\t    {subactionData}\n";
            }

            foreach (var parameter in Parameters)
            {
                string parameterValue =
                    (parameter.CurrentValueType == BaseParameter.ValueType.Other) ?
                    $"\'{parameter.Value}\'" : parameter.Value;

                parametersData += (parameterValue == string.Empty) ?
                    $" -1," : $" {parameterValue},";
            }

            if (inLine)
            {
                return $"{prefix}{{{subactionsData}{parametersData} }},\n";
            }
            else
            {
                return $"{prefix}{{\n" +
                    $"{subactionsData}" +
                    $"{prefix}\t{parametersData}\n" +
                    $"{prefix}}},\n";
            }
            
        }

        public override string SaveAsExcel()
        {
            string res = string.Empty;
            return res;
        }

        public override void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            var subAction = SubActions.Where(x => x.LuaName == subActionLuaName)
                .FirstOrDefault();
            if (subAction != null)
            {
                subAction.AddDev(index, 0, string.Empty);
            }
        }

        public override void AddParam(object val, string paramName,
            int groupNumber)
        {
            var parameter = parameters.Where(x => x.LuaName == paramName)
                .FirstOrDefault();
            bool haveParameter = parameter != null;
            if (haveParameter)
            {
                parameter.SetNewValue(val.ToString());
            }
        }

        #region Реализация ITreeViewItem
        override public ITreeViewItem[] Items
        {
            get
            {
                int capacity = SubActions.Count + parameters.Count;
                var items = new ITreeViewItem[capacity];

                int counter = 0;
                foreach (var subAction in SubActions)
                {
                    items[counter] = (ITreeViewItem)subAction;
                    counter++;
                }

                foreach (var parameter in parameters)
                {
                    items[counter] = parameter;
                    counter++;
                }

                return items;
            }
        }

        public override bool Delete(object child)
        {
            if (child is BaseParameter parameter)
            {
                parameter.Delete(this);
                return true;
            }

            if (child is IAction action)
            {
                action.Clear();
                return true;
            }

            return false;
        }
        #endregion

        public override string ToString()
        {
            string res = string.Empty;
            foreach (var subAction in SubActions)
            {
                string subActionStr = subAction.ToString();
                bool invalidString =
                    string.IsNullOrWhiteSpace(subActionStr) ||
                    string.IsNullOrEmpty(subActionStr);
                if (invalidString)
                {
                    res += $"{{ }} ";
                }
                else
                {
                    res += $"{{ {subAction} }} ";
                }

            }

            foreach (var parameter in parameters)
            {
                res += $"{{ {parameter.DisplayText[1]} }}";
            }

            return res;
        }

        public List<BaseParameter> Parameters
        {
            get { return parameters; }
        }

        private List<BaseParameter> parameters;
    }
}
