using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    public class ActionCustom : GroupableAction
    {

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
        /// Создает копию группы для ActionCustomGroup
        /// </summary>
        /// <returns>скопированое действие</returns>
        public IAction CopyGroup()
        {
            var copyGroup = new ActionCustom(name, owner, luaName);

            foreach (var subAction in SubActions)
            {
                EplanDevice.DeviceType[] types = null;
                EplanDevice.DeviceSubType[] subTypes = null;
                bool displayParameter = false;

                subAction.GetDisplayObjects(out types, out subTypes, out displayParameter);
                copyGroup.SubActions.Add(new Action(subAction.Name, owner,
                    subAction.LuaName, types, subTypes));
            }

            foreach (var parameter in Parameters)
            {
                copyGroup.Parameters.Add(new ActiveParameter(parameter.LuaName,
                    parameter.Name, ""));
            }

            return copyGroup;
        }


        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public override string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            res += $"{prefix}--Группа\n{prefix}{{\n";

            foreach (var subAction in SubActions)
            {
                res += subAction.SaveAsLuaTable(prefix + "\t"); 
            }

            foreach (var parameter in Parameters)
            {
                if(parameter.Value != string.Empty)
                    res += $"{prefix}\t{parameter.LuaName} = \"{parameter.Value}\" --{parameter.Name},\n";
            }

            res += $"{prefix}}},\n";

            return res;
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
