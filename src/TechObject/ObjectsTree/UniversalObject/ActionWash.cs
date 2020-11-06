using System.Collections.Generic;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Специальное действие - обработка сигналов во время мойки.
    /// </summary>
    public class ActionWash : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в 
        /// таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public ActionWash(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            vGroups = new List<Action>();
            vGroups.Add(new Action(DI, owner, DI,
                new Device.DeviceType[]
                { 
                    Device.DeviceType.DI,
                    Device.DeviceType.SB
                }));
            vGroups.Add(new Action(DO, owner, DO,
                new Device.DeviceType[]
                { 
                    Device.DeviceType.DO
                }));

            vGroups.Add(new Action("Устройства", owner, Devices,
                new Device.DeviceType[] 
                { 
                    Device.DeviceType.M,
                    Device.DeviceType.V,
                    Device.DeviceType.DO,
                    Device.DeviceType.AO,
                    Device.DeviceType.VC
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.M_FREQ,
                    Device.DeviceSubType.M_REV_FREQ,
                    Device.DeviceSubType.M_REV_FREQ_2,
                    Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                    Device.DeviceSubType.M_ATV,
                    Device.DeviceSubType.M
                }));

            vGroups.Add(new Action("Реверсивные устройства", owner,
                ReverseDevices,
                new Device.DeviceType[]
                { 
                    Device.DeviceType.M
                },
                new Device.DeviceSubType[] 
                {
                    Device.DeviceSubType.M_FREQ,
                    Device.DeviceSubType.M_REV_FREQ,
                    Device.DeviceSubType.M_REV_FREQ_2,
                    Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                    Device.DeviceSubType.M_ATV,
                    Device.DeviceSubType.M
                }));

            items = new List<ITreeViewItem>();
            foreach (Action action in vGroups)
            {
                items.Add(action);
            }

            var pumpFreqParam = new ActiveParameter("frequency",
                "Производительность");
            pumpFreqParam.OneValueOnly = true;
            pumpFreq = pumpFreqParam;
            items.Add(pumpFreq);
        }

        override public Action Clone()
        {
            var clone = new ActionWash(name, owner, luaName);

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

            string pumpFreqVal = pumpFreq.EditText[1].Trim();
            bool isParamNum = int.TryParse(pumpFreqVal, out int paramNum);

            if (groupData != "")
            {
                string saveFreqVal;
                if (isParamNum)
                {
                    saveFreqVal = $"{prefix}\tpump_freq = {paramNum},\n";
                }
                else
                {
                    saveFreqVal = 
                        $"{prefix}\tpump_freq = '{pumpFreqVal}',\n";
                }

                res += prefix;
                if (luaName != string.Empty)
                {
                    res += luaName + " =";
                }
                res += " --" + name + "\n" + prefix + "\t{\n" + groupData + 
                    (pumpFreqVal == string.Empty ? string.Empty : saveFreqVal) +
                    prefix + "\t},\n";
            }

            return res;
        }

        public override void AddDev(int index, int groupNumber,
            int washGroupIndex = 0)
        {
            if (groupNumber < vGroups.Count /*Количество групп*/ )
            {
                vGroups[groupNumber].AddDev(index, 0);
            }
        }

        public override void AddParam(object val, int washGroupIndex = 0)
        {           
            pumpFreq.SetNewValue(val.ToString());
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
                string res = "";

                foreach (Action action in vGroups)
                {
                    res += $"{{ {action.DisplayText[1]} }} ";
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
            if (child is BaseParameter parameter)
            {
                parameter.Delete(this);
                return true;
            }

            return false;
        }
        #endregion

        List<Action> vGroups;
        private BaseParameter pumpFreq; ///< Частота насоса, параметр.

        List<ITreeViewItem> items;
    }
}
