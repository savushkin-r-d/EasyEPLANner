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
                new Device.DeviceType[]
                { 
                    Device.DeviceType.DI,
                    Device.DeviceType.SB,
                    Device.DeviceType.GS,
                    Device.DeviceType.LS,
                    Device.DeviceType.FS
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

            var pumpFreqParam = new ActiveParameter("pump_freq",
                "Производительность");
            pumpFreqParam.OneValueOnly = true;
            parameters = new List<BaseParameter>();
            parameters.Add(pumpFreqParam);
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

        private List<BaseParameter> parameters;
    }
}
