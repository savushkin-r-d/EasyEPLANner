using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Действие - обработка сигналов во время мойки
    /// с возможностью группировки объектов
    /// </summary>
    public class ActionGroupWash : GroupableAction
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public ActionGroupWash(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            SubActions = new List<IAction>();
            var newAction = new ActionWash(GroupDefaultName, owner,
                string.Empty);
            SubActions.Add(newAction);
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
            var newAction = new ActionWash(GroupDefaultName, owner,
                string.Empty);
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

            string singleGroupData = string.Empty;
            string multiGroupData = string.Empty;
            singleGroupData += SaveSingleGroup(prefix);
            multiGroupData += SaveMultiGroup(prefix);

            string oldVersionCompatibility = 
                "Совместимость со старой версией";
            if (singleGroupData != string.Empty)
            {
                res += $"{prefix}{SingleGroupAction} = --{name}." +
                    $" {oldVersionCompatibility}\n" + 
                    $"{singleGroupData}";
            }

            if (multiGroupData != string.Empty)
            {
                res += $"{prefix}{MultiGroupAction} = --{name}\n" +
                    $"{prefix}\t{{\n{multiGroupData}{prefix}\t}},\n";
            }

            return res;
        }

        public override string SaveAsExcelCell()
        {
            string res = "";
            int groupIndex = 1;

            foreach (var group in SubActions)
            {
                var groupText = group.SaveAsExcelCell();
                if (groupText != string.Empty)
                {
                    res += $"Група {groupIndex++}:\n{groupText}\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить первую группу мойки. Старая функциональность
        /// </summary>
        /// <param name="prefix">Префикс</param>
        /// <returns></returns>
        private string SaveSingleGroup(string prefix)
        {
            string res = string.Empty;
            var firstGroup = SubActions.First();
            res += firstGroup?.SaveAsLuaTable(prefix);
            return res;
        }

        /// <summary>
        /// Сохранить все группы мойки
        /// </summary>
        /// <param name="prefix">Префикс</param>
        /// <returns></returns>
        private string SaveMultiGroup(string prefix)
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
                if(SubActions.Count > minCount)
                {
                    SubActions.Remove(subAction);
                    return true;
                }
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

        override public ITreeViewItem Insert()
        {
            var newAction = new ActionWash(GroupDefaultName, owner,
                string.Empty);
            newAction.DrawStyle = DrawStyle;
            SubActions.Add(newAction);

            newAction.AddParent(this);
            return newAction;
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }
        #endregion

        /// <summary>
        /// Название действия, для сохранения всех групп.
        /// </summary>
        public const string MultiGroupAction = "devices_data";

        /// <summary>
        /// Название действия, для сохранения первой группы.
        /// Старая функциональность.
        /// </summary>
        public const string SingleGroupAction = "wash_data";
    }
}
