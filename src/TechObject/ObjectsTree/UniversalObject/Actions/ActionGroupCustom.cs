using System.Collections.Generic;
using System.Linq;
using System;
using Editor;

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
            ActionCustomDelegate = actionCustomDeleagate;
            AddNewGroup();
        }

        public override IAction Clone()
        {
            var clone = (ActionGroupWash)base.Clone();
            clone.SubActions = new List<IAction>();
            foreach (IAction action in SubActions)
            {
                clone.SubActions.Add(action.Clone());
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
            while (SubActions.Count <= groupNumber)
            {
                AddNewGroup();
            }

            SubActions[groupNumber].AddParam(val, paramName, groupNumber);
        }

        private void AddNewGroup()
        {
            var newAction = ActionCustomDelegate();
            newAction.DrawStyle = DrawStyle;
            SubActions.Add(newAction);
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
            return res;
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
        #endregion

        private Func<ActionCustom> ActionCustomDelegate;
    }
}
