using System.Collections.Generic;
using System.Linq;
using System;
using Editor;
using EplanDevice;

namespace TechObject
{
    /// <summary>
    /// Действие - группировка кастомного действия
    /// </summary>
    public class ActionGroupCustom : GroupableAction
    {
        /// <summary>
        /// Создание действия
        /// </summary>
        /// <param name="name">Название действия</param>
        /// <param name="owner">Шаг</param>
        /// <param name="luaName">Название сохраняниемое в Lua</param>
        /// <param name="actionCustomDeleagate">Делегат описывающий группу</param>
        public ActionGroupCustom(string name, Step owner, string luaName,
            Func<ActionCustom> actionCustomDeleagate)
            : base(name, owner, luaName)
        {
            parameters = new List<BaseParameter>();
            ActionCustomDelegate = actionCustomDeleagate;
            AddNewGroup();
        }

        public override IAction Clone()
        {
            var clone = (ActionGroupCustom)base.Clone();
            clone.SubActions = new List<IAction>();
            clone.parameters = new List<BaseParameter>();
            
            foreach (var action in SubActions)
            {
                var cloneAction = action.Clone();
                clone.SubActions.Add(cloneAction);
                (cloneAction as ITreeViewItem).ValueChanged += 
                    sender => clone.OnValueChanged(sender);
            }

            foreach (var parameter in Parameters)
            {
                var cloneParameter = parameter.Clone();
                clone.parameters.Add(cloneParameter);
                cloneParameter.ValueChanged += 
                    sender => clone.OnValueChanged(sender);
            }

            return clone;
        }

        public override void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            while (SubActions.Count <= groupNumber)
            {
                AddNewGroup();
            }

            SubActions[groupNumber].AddDev(index, 0, subActionLuaName);
        }

        public override void AddParam(object val, string paramName,
            int groupNumber)
        {
            if (groupNumber == -1)
            {
                BaseParameter parameter = null;

                if (paramName.StartsWith("P_"))
                {
                    var index = int.Parse(paramName.Substring(2));
                    if (Parameters.Count > index)
                    {
                        parameter = Parameters[int.Parse(paramName.Substring(2))];
                    }
                }
                else
                {
                    parameter = parameters.Where(x => x.LuaName == paramName)
                        .FirstOrDefault();
                }

                bool haveParameter = parameter != null;
                if (haveParameter)
                {
                    parameter.SetNewValue(val.ToString());
                }
                return;
            }

            while (SubActions.Count <= groupNumber)
            {
                AddNewGroup();
            }

            SubActions[groupNumber].AddParam(val, paramName, groupNumber);
        }

        public void CreateParameter(BaseParameter parameter)
        {
            parameters.Add(parameter);
        }

        private void AddNewGroup()
        {
            var newAction = ActionCustomDelegate();
            newAction.DrawStyle = DrawStyle;
            SubActions.Add(newAction);

            SetUpEvents();
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            if (SubActions.Count == 0)
            {
                return res;
            }

            string groupData = string.Empty;
            groupData += SaveGroup(prefix);

            if (groupData != string.Empty)
            {
                if (name != string.Empty || luaName != string.Empty)
                {
                    res += prefix;
                    res += luaName != string.Empty ? $"{luaName} =" : "";
                    res += name != string.Empty ? $" --{name}" : "";
                    res += "\n";
                }
                res += 
                    $"{prefix}\t{{\n{groupData}" +
                    $"{prefix}\t}},\n";
            }

            return res;
        }

        public override string SaveAsExcel()
        {
            string res = "";
            int groupIndex = 1;

            foreach (var group in SubActions)
            {
                var groupText = group.SaveAsExcel();
                if (groupText != string.Empty)
                {
                    res += $"Група {groupIndex++}:\n{groupText}\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить все группы дейтсвия
        /// </summary>
        /// <param name="prefix">Префикс</param>
        /// <returns></returns>
        private string SaveGroup(string prefix)
        {
            string res = string.Empty;
            foreach (IAction group in SubActions)
            {
                res += group.SaveAsLuaTable(prefix + "\t");
            }
            if (res != string.Empty)
            {
                foreach (var parameter in Parameters)
                {
                    var parameterLuaName = (parameter.LuaName != string.Empty) ?
                        $"{parameter.LuaName}=" : string.Empty;
                    res += $"{prefix + "\t"}{parameterLuaName}{parameter.Value},\n";
                }
            }
            return res;
        }

        public override void UpdateOnGenericTechObject(IAction genericAction)
        {
            if (genericAction is null)
                return;

            var genericActionCustomGroup = genericAction as ActionGroupCustom;
            if (genericActionCustomGroup is null)
                return;

            foreach (var subActionIndex in Enumerable.Range(0, genericActionCustomGroup.SubActions.Count))
            {
                var genericSubAction = genericActionCustomGroup.SubActions.ElementAtOrDefault(subActionIndex);
                var subAction = SubActions.ElementAtOrDefault(subActionIndex);
                if (subAction is null)
                {
                    subAction = Insert() as IAction;
                }

                subAction.UpdateOnGenericTechObject(genericSubAction);
            }

            foreach (var parameterIndex in Enumerable.Range(0, genericActionCustomGroup.Parameters.Count))
            {
                parameters.ElementAtOrDefault(parameterIndex)
                    ?.SetNewValue(genericActionCustomGroup.Parameters.ElementAtOrDefault(parameterIndex).Value);
            }
        }

        #region Реализация ITreeViewItem
        override public bool Delete(object child)
        {
            var subAction = child as IAction;
            if (subAction != null)
            {
                int minCount = 1;
                if (SubActions.Count > minCount)
                {
                    SubActions.Remove(subAction);
                    return true;
                }
            }

            return false;
        }

        public override void GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
            out EplanDevice.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            SubActions.First().GetDisplayObjects(out devTypes, out devSubTypes,
                out displayParameters);
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
            var newAction = ActionCustomDelegate();
            newAction.DrawStyle = DrawStyle;
            SubActions.Add(newAction);

            SetUpEvents();
            OnValueChanged(this);

            newAction.AddParent(this);
            return newAction as ITreeViewItem;
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return base.Items.Concat(parameters).ToArray();
            }
        }
        #endregion

        public void SetUpEvents()
        {
            foreach (var action in SubActions)
            {
                if (action is ITreeViewItem item)
                {
                    item.ValueChanged += sender => OnValueChanged(sender);
                }
            }

            foreach (var parameter in Parameters)
            {
                parameter.ValueChanged += 
                    sender => OnValueChanged(sender);
            }
        }

        public List<BaseParameter> Parameters
        {
            get { return parameters; }
        }

        private Func<ActionCustom> ActionCustomDelegate;

        private List<BaseParameter> parameters;
    }
}
