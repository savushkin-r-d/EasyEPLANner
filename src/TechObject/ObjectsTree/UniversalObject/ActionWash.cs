using System.Collections.Generic;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Специальное действие - обработка сигналов во время мойки.
    /// </summary>
    public class ActionWash : GroupableAction
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
            SubActions.Add(new Action("DI", owner,"DI",
                new Device.DeviceType[]
                { 
                    Device.DeviceType.DI,
                    Device.DeviceType.SB
                }));
            SubActions.Add(new Action("DO", owner, "DO",
                new Device.DeviceType[]
                { 
                    Device.DeviceType.DO
                }));

            SubActions.Add(new Action("Устройства", owner, "devices",
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

            SubActions.Add(new Action("Реверсивные устройства", owner,
                "rev_devices",
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

            var pumpFreqParam = new ActiveParameter("frequency",
                "Производительность");
            pumpFreqParam.OneValueOnly = true;
            pumpFreq = pumpFreqParam;
        }

        override public IAction Clone()
        {
            var clone = new ActionWash(name, owner, luaName);

            clone.SubActions = new List<IAction>();
            foreach (var action in SubActions)
            {
                clone.SubActions.Add(action.Clone());
            }

            clone.pumpFreq = pumpFreq.Clone();
            return clone;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public override string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            if (SubActions.Count == 0)
            {
                return res;
            }

            string groupData = string.Empty;
            foreach (IAction group in SubActions)
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
            if (groupNumber < SubActions.Count)
            {
                SubActions[groupNumber].AddDev(index, 0);
            }

            deviceIndex.Add(index);
        }

        public override void AddParam(object val, int washGroupIndex = 0)
        {           
            pumpFreq.SetNewValue(val.ToString());
        }

        #region Реализация ITreeViewItem
        override public ITreeViewItem[] Items
        {
            get
            {
                int parametersCount = 1;
                int capacity = SubActions.Count + parametersCount;
                var items = new ITreeViewItem[capacity];

                int counter = 0;
                foreach(var subAction in SubActions)
                {
                    items[counter] = (ITreeViewItem)subAction;
                    counter++;
                }

                items[counter] = pumpFreq;

                return items;
            }
        }

        public override bool Delete(object child)
        {
            if (child is BaseParameter parameter)
            {
                parameter.Delete(this);
                return true;
            }

            if (child is IAction action)
            {
                action.Clear();
                return true;
            }

            return false;
        }
        #endregion

        public override string ToString()
        {
            string res = string.Empty;
            foreach (var subAction in SubActions)
            {
                string subActionStr = subAction.ToString();
                bool invalidString =
                    string.IsNullOrWhiteSpace(subActionStr) ||
                    string.IsNullOrEmpty(subActionStr);
                if (invalidString)
                {
                    res += $"{{ }} ";
                }
                else
                {
                    res += $"{{ {subAction} }} ";
                }

            }

            res += $"{{ {pumpFreq.DisplayText[1]} }}";

            return res;
        }

        private BaseParameter pumpFreq; ///< Частота насоса, параметр.
    }
}
