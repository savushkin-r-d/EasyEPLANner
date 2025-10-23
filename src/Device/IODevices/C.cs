using EasyEPlanner.FileSavers.XML;
using Eplan.EplApi.DataModel;
using EplanDevice;
using StaticHelper;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;

namespace EplanDevice
{
    /// <summary>
    /// ПИД-регулятор
    /// </summary>
    public class C : IODevice
    {
        public C(string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) : base(name,
                eplanName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.C;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.C:
                    switch (dst)
                    {
                        case DeviceSubType.C_PID:
                            return nameof(DeviceSubType.C_PID);
                        case DeviceSubType.C_THLD:
                            return nameof(DeviceSubType.C_THLD);
                    }
                    break;
            }

            return string.Empty;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);
            
            string errStr = string.Empty;
            switch (subType)
            {
                case "":
                case "C_PID":
                    dSubType = DeviceSubType.C_PID;

                    properties.Add(Property.IN_VALUE, null);
                    properties.Add(Property.OUT_VALUE, null);

                    parameters.Add(Parameter.P_k, 1);
                    parameters.Add(Parameter.P_Ti, 15);
                    parameters.Add(Parameter.P_Td, 0.01);
                    parameters.Add(Parameter.P_dt, 1000);

                    parameters.Add(Parameter.P_max, 100);
                    parameters.Add(Parameter.P_min, 0);

                    parameters.Add(Parameter.P_acceleration_time, 30);
                    parameters.Add(Parameter.P_is_manual_mode, 0);
                    parameters.Add(Parameter.P_U_manual, 65);

                    parameters.Add(Parameter.P_k2, 0);
                    parameters.Add(Parameter.P_Ti2, 0);
                    parameters.Add(Parameter.P_Td2, 0);

                    parameters.Add(Parameter.P_out_max, 100);
                    parameters.Add(Parameter.P_out_min, 0);

                    parameters.Add(Parameter.P_is_reverse, 0);
                    parameters.Add(Parameter.P_is_zero_start, 1);
                    break;

                case "C_THLD":
                    properties.Add(Property.IN_VALUE, null);
                    properties.Add(Property.OUT_VALUE, null);

                    parameters.Add(Parameter.P_is_reverse, null);
                    parameters.Add(Parameter.P_delta, null);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (пустая строка, C_PID, C_THLD).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override Dictionary<ITag, int> GetDeviceProperties(
            DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.C:
                    return new Dictionary<ITag, int>()
                    {
                        {Tag.ST, 1},
                        {Tag.M, 1},
                        {Tag.V, 1},
                        {Tag.Z, 1},
                    };
            }

            return null;
        }

        public override string Check()
        {
            string res = base.Check();

            bool propertyChanged = false;
            var propertiesToCheck = new[] { Property.IN_VALUE, Property.OUT_VALUE };
            foreach (var property in propertiesToCheck)
            {
                propertyChanged |= CheckDeviceProperty(property, out var r);
                res += r;
            }

            if (propertyChanged)
                UpdateProperties();

            return res;
        }

        [ExcludeFromCodeCoverage]
        private bool CheckDeviceProperty(string property, out string res)
        {
            res = string.Empty;
            if (properties.TryGetValue(property, out var value) &&
                value is string dev &&
                dev != string.Empty &&
                DeviceManager.GetInstance().GetDevice(dev).Description is CommonConst.Cap)
            {
                properties[property] = string.Empty;
                res = $"{Name}: в свойстве {property} сброшено несуществующее устройство {dev}.\n";
                return true;
            }

            return false;
        }

        #region сохранение в Lua
        protected override string SaveParameters(string prefix)
        {
            string res = string.Empty;

            if (parameters.Count > 0)
            {
                string tmp = string.Empty;
                foreach (var par in parameters)
                {
                    if (par.Value != null)
                    {
                        tmp += $"{prefix}\t{par.Key} = {par.Value},\n";
                    }
                }

                if (tmp != string.Empty)
                {
                    res += $"{prefix}par =\n";
                    res += $"{prefix}\t{{\n";
                    res += tmp.Remove(tmp.Length - 2) + "\n";
                    res += $"{prefix}\t}}\n";
                }
            }

            return res;
        }
        #endregion

        #region сохранение базы каналов
        public override void GenerateDeviceTags(IDriver root)
        {
            var devTags = GetDeviceProperties(DeviceType, DeviceSubType).Keys;
            var devParameters = new List<(string name, string description)>();
            devParameters.AddRange(devTags.Select(t => (t.Name, t.Description)));
            devParameters.AddRange(parameters.Select(parameter => (parameter.Key.Name, parameter.Key.Description)));

            foreach (var par in devParameters)
            {
                root.AddChannel(Name, new Channel($"{Name}.{par.name}") 
                { 
                    Comment = par.description 
                });
            }
        }
        #endregion
    }
}
