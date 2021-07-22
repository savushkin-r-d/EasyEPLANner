using System.Collections.Generic;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Специальное действие - переход к шагу по условию.
    /// </summary>
    public class ActionToStepByCondition : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в 
        /// таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public ActionToStepByCondition(string name, Step owner, 
            string luaName) : base(name, owner, luaName)
        {
            var allowedDevTypes = new Device.DeviceType[]
            {
                Device.DeviceType.V
            };

            vGroups = new List<Action>();
            vGroups.Add(new Action("Включение устройств", owner, "on_devices",
                allowedDevTypes));
            vGroups.Add(new Action("Выключение устройств", owner, "off_devices",
                allowedDevTypes));

            items = new List<ITreeViewItem>();
            foreach (Action action in vGroups)
            {
                items.Add(action);
            }
        }

        override public Action Clone()
        {
            var clone = new ActionToStepByCondition(name, owner, luaName);

            clone.vGroups = new List<Action>();
            foreach (var action in vGroups)
            {
                clone.vGroups.Add(action.Clone());
            }

            clone.items.Clear();
            clone.items = new List<ITreeViewItem>();
            foreach (var action in clone.vGroups)
            {
                clone.items.Add(action);
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, 
            int oldTechObjectN, string techObjectName)
        {
            foreach (Action subAction in vGroups)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, 
                    techObjectName);
            }
        }

        override public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (Action subAction in vGroups)
            {
                subAction.ModifyDevNames(newTechObjectName,
                    newTechObjectNumber, oldTechObjectName,
                    oldTechObjectNumber);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public override string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            if (vGroups.Count == 0)
            {
                return res;
            }

            string groupData = string.Empty;
            foreach (Action group in vGroups)
            {
                groupData += group.SaveAsLuaTable(prefix + "\t");
            }

            if (groupData != string.Empty)
            {
                res += prefix;
                if (luaName != string.Empty)
                {
                    res += luaName + " =";
                }
                res += " --" + name + "\n" +
                    prefix +"\t{\n" +
                    groupData + 
                    prefix + "\t},\n";
            }

            return res;
        }

        public override void AddDev(int index, int groupNumber,
            int washGroupIndex = 0)
        {
            if (groupNumber < vGroups.Count)
            {
                vGroups[groupNumber].AddDev(index, 0);
            }

            deviceIndex.Add(index);
        }

        #region Синхронизация устройств в объекте.
        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in vGroups)
            {
                subAction.Synch(array);
            }
        }
        #endregion

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = string.Empty;
                foreach (Action action in vGroups)
                {
                    res += $"{{ {action.DisplayText[1]} }} ";
                }

                return new string[] { name, res };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public void Clear()
        {
            foreach (Action subAction in vGroups)
            {
                subAction.Clear();
            }
        }

        override public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.NONE;
            }
        }

        public override bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        public override bool Delete(object child)
        {
            if (child is BaseParameter parameter)
            {
                parameter.Delete(this);
                return true;
            }

            return false;
        }
        #endregion

        List<Action> vGroups;
        List<ITreeViewItem> items;
    }
}
