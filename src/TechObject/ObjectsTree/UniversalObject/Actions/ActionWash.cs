using System.Collections.Generic;
using System.Linq;
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
                new EplanDevice.DeviceType[]
                { 
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.SB,
                    EplanDevice.DeviceType.GS,
                    EplanDevice.DeviceType.LS,
                    EplanDevice.DeviceType.FS
                }));
            SubActions.Add(new Action("DO", owner, "DO",
                new EplanDevice.DeviceType[]
                { 
                    EplanDevice.DeviceType.DO
                }));

            SubActions.Add(new Action("Устройства", owner, "devices",
                new EplanDevice.DeviceType[] 
                { 
                    EplanDevice.DeviceType.M,
                    EplanDevice.DeviceType.V,
                    EplanDevice.DeviceType.DO,
                    EplanDevice.DeviceType.AO,
                    EplanDevice.DeviceType.VC,
                    EplanDevice.DeviceType.C
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.M_FREQ,
                    EplanDevice.DeviceSubType.M_REV_FREQ,
                    EplanDevice.DeviceSubType.M_REV_FREQ_2,
                    EplanDevice.DeviceSubType.M_REV_FREQ_2_ERROR,
                    EplanDevice.DeviceSubType.M_ATV,
                    EplanDevice.DeviceSubType.M_ATV_LINEAR,
                    EplanDevice.DeviceSubType.M,
                    EplanDevice.DeviceSubType.M_VIRT,
                    EplanDevice.DeviceSubType.V_AS_DO1_DI2,
                    EplanDevice.DeviceSubType.V_AS_MIXPROOF,
                    EplanDevice.DeviceSubType.V_BOTTOM_MIXPROOF,
                    EplanDevice.DeviceSubType.V_DO1,
                    EplanDevice.DeviceSubType.V_DO1_DI1_FB_OFF,
                    EplanDevice.DeviceSubType.V_DO1_DI1_FB_ON,
                    EplanDevice.DeviceSubType.V_DO1_DI2,
                    EplanDevice.DeviceSubType.V_DO2,
                    EplanDevice.DeviceSubType.V_DO2_DI2,
                    EplanDevice.DeviceSubType.V_DO2_DI2_BISTABLE,
                    EplanDevice.DeviceSubType.V_IOLINK_DO1_DI2,
                    EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                    EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1,
                    EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1_DI2,
                    EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF,
                    EplanDevice.DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON,
                    EplanDevice.DeviceSubType.V_MIXPROOF,
                    EplanDevice.DeviceSubType.V_VIRT,
                    EplanDevice.DeviceSubType.V_MINI_FLUSHING,
                    EplanDevice.DeviceSubType.V_IOL_TERMINAL_MIXPROOF_DO3,
                    EplanDevice.DeviceSubType.AO,
                    EplanDevice.DeviceSubType.AO_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT,
                    EplanDevice.DeviceSubType.VC,
                    EplanDevice.DeviceSubType.VC_IOLINK,
                    EplanDevice.DeviceSubType.VC_VIRT,
                    EplanDevice.DeviceSubType.NONE
                }));

            SubActions.Add(new Action("Реверсивные устройства", owner,
                "rev_devices",
                new EplanDevice.DeviceType[]
                { 
                    EplanDevice.DeviceType.M
                },
                new EplanDevice.DeviceSubType[] 
                {
                    EplanDevice.DeviceSubType.M_FREQ,
                    EplanDevice.DeviceSubType.M_REV_FREQ,
                    EplanDevice.DeviceSubType.M_REV_FREQ_2,
                    EplanDevice.DeviceSubType.M_REV_FREQ_2_ERROR,
                    EplanDevice.DeviceSubType.M_ATV,
                    EplanDevice.DeviceSubType.M_ATV_LINEAR,
                    EplanDevice.DeviceSubType.M,
                    EplanDevice.DeviceSubType.M_VIRT,
                }));

            var pumpFreqParam = new ActionParameter("pump_freq",
                "Производительность/задание", "", new List<BaseParameter.DisplayObject> 
                { 
                    BaseParameter.DisplayObject.Parameters,
                    BaseParameter.DisplayObject.Signals}
                );

            parameters = new List<BaseParameter>
            {
                pumpFreqParam
            };

            SubActions.ForEach(subAction => (subAction as Action).ValueChanged += (sender) => OnSubActionChanged(sender));
            pumpFreqParam.ValueChanged += (sender) => OnSubActionChanged(sender);
        }

        override public IAction Clone()
        {
            var clone = new ActionWash(name, owner, luaName);

            clone.SubActions = new List<IAction>();
            foreach (var action in SubActions)
            {
                clone.SubActions.Add(action.Clone());
            }

            clone.parameters = new List<BaseParameter>();
            foreach(var parameter in parameters)
            {
                clone.parameters.Add(parameter.Clone());
            }

            clone.SubActions.ForEach(
                action => (action as ITreeViewItem).ValueChanged +=
                sender => clone.OnValueChanged(sender));

            clone.parameters.ForEach(
                par => (par as ITreeViewItem).ValueChanged +=
                sender => clone.OnValueChanged(sender));

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

            string parametersData = string.Empty;
            if (groupData != string.Empty)
            {
                foreach(var parameter in parameters)
                {
                    if (parameter.IsEmpty)
                    {
                        continue;
                    }

                    bool isParamNum = int.TryParse(parameter.Value,
                        out int paramNum);
                    string paramValue =
                        isParamNum ? $"{paramNum}" : $"'{parameter.Value}'";
                    string saveParameterValue =
                        $"{prefix}\t{parameter.LuaName} = {paramValue},\n";
                    parametersData += saveParameterValue;
                }
                
                res += prefix;
                if (luaName != string.Empty)
                {
                    res += luaName + " =";
                }

                res += " --" + name + "\n" +
                    prefix + "\t{\n" +
                    groupData +
                    parametersData +
                    prefix + "\t},\n";
            }

            return res;
        }

        public override string SaveAsExcel()
        {
            string res = string.Empty;
            if (SubActions.Count == 0)
            {
                return res;
            }

            foreach (var subAction in SubActions)
            {
                var subActionText = subAction.SaveAsExcel();
                if(subActionText != string.Empty)
                {
                    res += $"{subAction.Name}: {subActionText}.\n";
                }
            }

            foreach (var parameter in parameters)
            {
                var parameterText = parameter.Value;
                if(parameterText != string.Empty)
                {
                    res += $"{parameter.Name}: {parameterText}.\n";
                }
            }

            return res;
        }

        public override void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            var subAction = SubActions.Where(x => x.LuaName == subActionLuaName)
                .FirstOrDefault();
            if(subAction != null)
            {
                subAction.AddDev(index, 0, string.Empty);
            }
        }

        public override void AddParam(object val, string paramName,
            int groupNumber)
        {
            var parameter = parameters.Where(x => x.LuaName == paramName)
                .FirstOrDefault();
            bool haveParameter = parameter != null;
            if (haveParameter)
            {
                parameter.SetNewValue(val.ToString());
            }
        }

        #region Реализация ITreeViewItem
        override public ITreeViewItem[] Items
        {
            get
            {
                int capacity = SubActions.Count + parameters.Count;
                var items = new ITreeViewItem[capacity];

                int counter = 0;
                foreach(var subAction in SubActions)
                {
                    items[counter] = (ITreeViewItem)subAction;
                    counter++;
                }

                foreach(var parameter in parameters)
                {
                    items[counter] = parameter;
                    counter++;
                }

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

            foreach(var parameter in parameters)
            {
                res += $"{{ {parameter.DisplayText[1]} }}";
            }

            return res;
        }
    }
}
