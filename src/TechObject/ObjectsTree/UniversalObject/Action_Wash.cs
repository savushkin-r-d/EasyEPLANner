using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Специальное действие - обработка сигналов во время мойки.
    /// </summary>
    public class Action_Wash : Action
    {

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в 
        /// таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action_Wash(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            vGroups = new List<Action>();
            vGroups.Add(new Action("DI", owner, "DI",
                new Device.DeviceType[2] { 
                    Device.DeviceType.DI,
                    Device.DeviceType.SB }));
            vGroups.Add(new Action("DO", owner, "DO",
                new Device.DeviceType[1] { 
                    Device.DeviceType.DO }));

            vGroups.Add(new Action("Устройства", owner, "devices",
                new Device.DeviceType[3] { 
                    Device.DeviceType.M,
                    Device.DeviceType.V, 
                    Device.DeviceType.DO }));

            vGroups.Add(new Action("Реверсные устройства", owner, "rev_devices",
                new Device.DeviceType[] { 
                    Device.DeviceType.M, 
                    Device.DeviceType.V }));

            items = new List<ITreeViewItem>();
            foreach (Action action in vGroups)
            {
                items.Add(action);
            }

            pumpFreq = new ObjectProperty("Частота насосов (параметр)", -1, -1);
            items.Add(pumpFreq);
        }

        override public Action Clone()
        {
            Action_Wash clone = (Action_Wash)base.Clone();

            clone.vGroups = new List<Action>();
            foreach (Action action in vGroups)
            {
                clone.vGroups.Add(action.Clone());
            }

            clone.items.Clear();
            clone.items = new List<ITreeViewItem>();
            foreach (Action action in clone.vGroups)
            {
                clone.items.Add(action);
            }

            clone.pumpFreq = pumpFreq.Clone();
            clone.items.Add(clone.pumpFreq);
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
            if (vGroups.Count == 0) return "";

            string res = "";

            foreach (Action group in vGroups)
            {
                string tmp = group.SaveAsLuaTable(prefix + "\t");
                if (tmp != "")
                {
                    res += tmp;
                }
            }

            string pumpFreqVal = pumpFreq.EditText[1].Trim();

            if (res != "")
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    (pumpFreqVal == "-1" ? "" : prefix + "\tpump_freq = " + 
                    pumpFreqVal + ",\n") + prefix + "\t},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="device">Устройство.</param>
        /// <param name="additionalParam">Дополнительный параметр.</param>
        public override void AddDev(int index, int additionalParam)
        {
            if (additionalParam < vGroups.Count /*Количество групп*/ )
            {
                (vGroups[additionalParam] as Action).AddDev(index, 0);
            }

            deviceIndex.Add(index);
        }

        /// <summary>
        /// Добавление параметра к действию.
        /// </summary>
        /// <param name="index">Индекс параметра.</param>
        /// <param name="val">Значение параметра.</param>
        public override void AddParam(int index, int val)
        {
            pumpFreq.SetValue(val);
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
                var deviceManager = Device.DeviceManager.GetInstance();
                string res = "";

                foreach (Action group in vGroups)
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

                res += "{" + pumpFreq.DisplayText[1] + "}";

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
            if (child.GetType() == typeof(ObjectProperty))
            {
                var objectProperty = child as ObjectProperty;
                objectProperty.Delete(this);
            }

            return false;
        }
        #endregion

        List<Action> vGroups;
        private ObjectProperty pumpFreq; ///< Частота насоса, параметр.

        List<ITreeViewItem> items;
    }
}
