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
            vGroups = new List<IAction>();
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
                    Device.DeviceType.VC,
                    Device.DeviceType.C
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.M_FREQ,
                    Device.DeviceSubType.M_REV_FREQ,
                    Device.DeviceSubType.M_REV_FREQ_2,
                    Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                    Device.DeviceSubType.M_ATV,
                    Device.DeviceSubType.M_ATV_LINEAR,
                    Device.DeviceSubType.M,
                    Device.DeviceSubType.M_VIRT,
                    Device.DeviceSubType.V_AS_DO1_DI2,
                    Device.DeviceSubType.V_AS_MIXPROOF,
                    Device.DeviceSubType.V_BOTTOM_MIXPROOF,
                    Device.DeviceSubType.V_DO1,
                    Device.DeviceSubType.V_DO1_DI1_FB_OFF,
                    Device.DeviceSubType.V_DO1_DI1_FB_ON,
                    Device.DeviceSubType.V_DO1_DI2,
                    Device.DeviceSubType.V_DO2,
                    Device.DeviceSubType.V_DO2_DI2,
                    Device.DeviceSubType.V_DO2_DI2_BISTABLE,
                    Device.DeviceSubType.V_IOLINK_DO1_DI2,
                    Device.DeviceSubType.V_IOLINK_MIXPROOF,
                    Device.DeviceSubType.V_IOLINK_VTUG_DO1,
                    Device.DeviceSubType.V_IOLINK_VTUG_DO1_DI2,
                    Device.DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF,
                    Device.DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON,
                    Device.DeviceSubType.V_MIXPROOF,
                    Device.DeviceSubType.V_VIRT,
                    Device.DeviceSubType.AO,
                    Device.DeviceSubType.AO_VIRT,
                    Device.DeviceSubType.DO,
                    Device.DeviceSubType.DO_VIRT,
                    Device.DeviceSubType.VC,
                    Device.DeviceSubType.VC_IOLINK,
                    Device.DeviceSubType.VC_VIRT,
                    Device.DeviceSubType.NONE
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
                    Device.DeviceSubType.M_ATV_LINEAR,
                    Device.DeviceSubType.M,
                    Device.DeviceSubType.M_VIRT,
                }));

            items = new List<ITreeViewItem>();
            foreach (IAction action in vGroups)
            {
                items.Add((ITreeViewItem)action);
            }

            var pumpFreqParam = new ActiveParameter("frequency",
                "Производительность");
            pumpFreqParam.OneValueOnly = true;
            pumpFreq = pumpFreqParam;
            items.Add(pumpFreq);
        }

        override public IAction Clone()
        {
            var clone = new ActionWash(name, owner, luaName);

            clone.vGroups = new List<IAction>();
            foreach (var action in vGroups)
            {
                clone.vGroups.Add(action.Clone());
            }

            clone.items.Clear();
            clone.items = new List<ITreeViewItem>();
            foreach (var action in clone.vGroups)
            {
                clone.items.Add((ITreeViewItem)action);
            }

            clone.pumpFreq = pumpFreq.Clone();
            clone.items.Add(clone.pumpFreq);
            return clone;
        }

        override public void ModifyDevNames(int newTechObjectN, 
            int oldTechObjectN, string techObjectName)
        {
            foreach (IAction subAction in vGroups)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN, 
                    techObjectName);
            }
        }

        override public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (IAction subAction in vGroups)
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
            foreach (IAction group in vGroups)
            {
                groupData += group.SaveAsLuaTable(prefix + "\t");
            }

            if (groupData != "")
            {
                string pumpFreqVal = pumpFreq.EditText[1].Trim();
                bool isParamNum = int.TryParse(pumpFreqVal, out int paramNum);
                string paramValue = 
                    isParamNum ? $"{paramNum}" : $"'{pumpFreqVal}'";
                string saveFreqVal = $"{prefix}\tpump_freq = {paramValue},\n";

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
            if (groupNumber < vGroups.Count)
            {
                vGroups[groupNumber].AddDev(index, 0);
            }

            deviceIndex.Add(index);
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
            foreach (IAction subAction in vGroups)
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

                foreach (IAction action in vGroups)
                {
                    res += $"{{ {string.Join(" ", action.DevicesNames)} }} ";
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
            foreach (IAction subAction in vGroups)
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

        public override bool HasSubActions
        {
            get => true;
        }

        public override List<IAction> SubActions => vGroups;

        List<IAction> vGroups;
        private BaseParameter pumpFreq; ///< Частота насоса, параметр.

        List<ITreeViewItem> items;
    }
}
