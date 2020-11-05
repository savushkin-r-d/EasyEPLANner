using System.Collections.Generic;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Действие - обработка сигналов во время мойки
    /// с возможностью группировки объектов
    /// </summary>
    public class ActionGroupWash : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        /// <param name="devSubTypes">Допустимые подтипы устройств</param>
        /// <param name="devTypes">Допустимые типы устройств</param>
        public ActionGroupWash(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subActions = new List<Action>();
            var newAction = new ActionWash(GroupDefaultName, owner,
                string.Empty);
            subActions.Add(newAction);
        }

        public override Action Clone()
        {
            var clone = (ActionGroupWash)base.Clone();
            clone.subActions = new List<Action>();
            foreach (Action action in subActions)
            {
                clone.subActions.Add(action.Clone());
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, 
            int oldTechObjectN, string techObjectName)
        {
            foreach (Action subAction in subActions)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, 
                    techObjectName);
            }
        }

        override public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (Action subAction in subActions)
            {
                subAction.ModifyDevNames(newTechObjectName, 
                    newTechObjectNumber, oldTechObjectName, 
                    oldTechObjectNumber);
            }
        }

        public override void AddDev(int index, int groupNumber,
            int washGroupIndex)
        {
            while (subActions.Count <= washGroupIndex)
            {
                var newAction = new ActionWash(GroupDefaultName, owner,
                    string.Empty);
                newAction.DrawStyle = DrawStyle;
                subActions.Add(newAction);
            }

            subActions[washGroupIndex].AddDev(index, groupNumber, 0);
        }

        public override void AddParam(object val, int washGroupIndex)
        {
            subActions[washGroupIndex].AddParam(val);
        }

        #region Синхронизация устройств в объекте.
        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.
        /// </param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (ActionWash subAction in subActions)
            {
                subAction.Synch(array);
            }
        }
        #endregion

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            if (subActions.Count == 0)
            {
                return res;
            }

            string groupData = string.Empty;
            foreach (ActionWash group in subActions)
            {
                groupData += group.SaveAsLuaTable(prefix + "\t");
            }

            if (groupData != string.Empty)
            {
                res += prefix + luaName + " = --" + name + "\n" + prefix +
                    "\t{\n" + groupData + prefix + "\t},\n";
            }

            return res;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Empty;

                foreach (ActionWash group in subActions)
                {
                    res += $"{{ {group.DisplayText[1]} }} ";
                }

                return new string[] { name, res };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return subActions.ToArray();
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            var subAction = child as ActionWash;
            if (subAction != null)
            {
                int minCount = 1;
                if(subActions.Count > minCount)
                {
                    subActions.Remove(subAction);
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
            subActions.Add(newAction);
            return newAction;
        }

        override public void Clear()
        {
            foreach (ActionWash subAction in subActions)
            {
                subAction.Clear();
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        override public DrawInfo.Style DrawStyle
        {
            get
            {
                return base.DrawStyle;
            }
            set
            {
                base.DrawStyle = value;
                if (subActions != null)
                {
                    foreach(var subAction in subActions)
                    {
                        subAction.DrawStyle = DrawStyle;
                    }
                }
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
                switch (luaName)
                {
                    case "wash_data":
                        return ImageIndexEnum.ActionWash;

                    default:
                        return ImageIndexEnum.NONE;
                }
            }
        }
        #endregion

        private List<Action> subActions;
    }
}
