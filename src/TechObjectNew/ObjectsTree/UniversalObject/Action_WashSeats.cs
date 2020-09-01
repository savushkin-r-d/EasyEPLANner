using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEditor;

namespace NewTechObject
{
    public class Action_WashSeats : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action_WashSeats(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subAction_WashGroupSeats = new List<Action>();
            subAction_WashGroupSeats.Add(
                new Action("Группа", owner, "",
                new Device.DeviceType[1] { Device.DeviceType.V },
                new Device.DeviceSubType[3] {
                Device.DeviceSubType.V_MIXPROOF,
                Device.DeviceSubType.V_AS_MIXPROOF,
                Device.DeviceSubType.V_IOLINK_MIXPROOF }));
        }

        public override Action Clone()
        {
            Action_WashSeats clone = (Action_WashSeats)base.Clone();

            clone.subAction_WashGroupSeats = new List<Action>();

            foreach (Action action in subAction_WashGroupSeats)
            {
                clone.subAction_WashGroupSeats.Add(action.Clone());
            }

            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, 
            int oldTechObjectN, string techObjectName)
        {
            foreach (Action subAction in subAction_WashGroupSeats)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, 
                    techObjectName);
            }
        }

        override public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (Action subAction in subAction_WashGroupSeats)
            {
                subAction.ModifyDevNames(newTechObjectName, 
                    newTechObjectNumber, oldTechObjectName, 
                    oldTechObjectNumber);
            }
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="groupNumber">Дополнительный параметр.</param>
        public override void AddDev(int index, int groupNumber)
        {
            while (subAction_WashGroupSeats.Count <= groupNumber)
            {
                subAction_WashGroupSeats.Add(
                    new Action("Группа", owner, "",
                    new Device.DeviceType[1] { Device.DeviceType.V },
                    new Device.DeviceSubType[3] {
                        Device.DeviceSubType.V_MIXPROOF,
                        Device.DeviceSubType.V_AS_MIXPROOF,
                        Device.DeviceSubType.V_IOLINK_MIXPROOF }));

                subAction_WashGroupSeats[
                    subAction_WashGroupSeats.Count - 1].DrawStyle = DrawStyle;
            }

            subAction_WashGroupSeats[groupNumber].AddDev(index, 0);

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.
        /// </param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (Action subAction in subAction_WashGroupSeats)
            {
                subAction.Synch(array);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            if (subAction_WashGroupSeats.Count == 0) return "";

            string res = "";

            foreach (Action group in subAction_WashGroupSeats)
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

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                Device.DeviceManager deviceManager = Device.DeviceManager
                    .GetInstance();
                string res = "";

                foreach (Action group in subAction_WashGroupSeats)
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

        override public ITreeViewItem[] Items
        {
            get
            {
                return subAction_WashGroupSeats.ToArray();
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
            var subAction = child as Action;
            if (subAction != null)
            {
                subAction_WashGroupSeats.Remove(subAction);
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

        override public ITreeViewItem Insert()
        {
            var newAction = new Action("Группа", owner);
            newAction.DrawStyle = DrawStyle;
            subAction_WashGroupSeats.Add(newAction);
            return newAction;
        }

        override public void Clear()
        {
            foreach (Action subAction in subAction_WashGroupSeats)
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
                if (subAction_WashGroupSeats != null)
                {
                    subAction_WashGroupSeats[0].DrawStyle = DrawStyle;
                }
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                switch(name)
                {
                    case "Верхние седла":
                        return ImageIndexEnum.ActionWashUpperSeats;

                    case "Нижние седла":
                        return ImageIndexEnum.ActionWashLowerSeats;

                    default:
                        return ImageIndexEnum.NONE;
                }
            }
        }
        #endregion

        private List<Action> subAction_WashGroupSeats;
    }
}
