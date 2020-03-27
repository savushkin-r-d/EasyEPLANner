﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Специальное действие - выдача дискретных сигналов при наличии входного 
    /// дискретного сигнала.
    /// </summary>
    public class Action_DI_DO : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в 
        /// таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action_DI_DO(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subAction_DI_DO_Group = new List<Action>();
            subAction_DI_DO_Group.Add(new Action("Группа", owner, "",
                new Device.DeviceType[3] { 
                    Device.DeviceType.DI, 
                    Device.DeviceType.SB, 
                    Device.DeviceType.DO }));
        }

        override public Action Clone()
        {
            Action_DI_DO clone = (Action_DI_DO)base.Clone();

            clone.subAction_DI_DO_Group = new List<Action>();
            foreach (Action action in subAction_DI_DO_Group)
            {
                clone.subAction_DI_DO_Group.Add(action.Clone());
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, 
            int oldTechObjectN, string techObjectName)
        {
            foreach (Action subAction in subAction_DI_DO_Group)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, 
                    techObjectName);
            }
        }

        override public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (Action subAction in subAction_DI_DO_Group)
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
        override public string SaveAsLuaTable(string prefix)
        {
            if (subAction_DI_DO_Group.Count == 0) return "";

            string res = "";

            foreach (Action group in subAction_DI_DO_Group)
            {
                res += group.SaveAsLuaTable(prefix + "\t");
            }

            if (res != "")
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    prefix + "\t},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="groupNumber">Дополнительный параметр.</param>
        public override void AddDev(int index, int groupNumber)
        {
            while (subAction_DI_DO_Group.Count <= groupNumber)
            {
                subAction_DI_DO_Group.Add(new Action("Группа", owner));
            }

            subAction_DI_DO_Group[groupNumber].AddDev(index, 0);

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in subAction_DI_DO_Group)
            {
                subAction.Synch(array);
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager
                    .GetInstance();
                string res = "";

                foreach (Action group in subAction_DI_DO_Group)
                {
                    res += "{";

                    foreach (int index in group.DeviceIndex)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + 
                            " ";
                    }

                    if (group.DeviceIndex.Count > 0)
                    {
                        res = res.Remove(res.Length - 1);
                    }

                    res += "} ";
                }

                return new string[] { name, res };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return subAction_DI_DO_Group.ToArray();
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
            Action subAction = child as Action;

            if (subAction != null)
            {
                subAction_DI_DO_Group.Remove(subAction);
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

        override public Editor.ITreeViewItem Insert()
        {
            subAction_DI_DO_Group.Add(new Action("Группа", owner));
            return subAction_DI_DO_Group[subAction_DI_DO_Group.Count - 1];
        }

        override public void Clear()
        {
            foreach (Action subAction in subAction_DI_DO_Group)
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
        #endregion

        private List<Action> subAction_DI_DO_Group;
    }
}
