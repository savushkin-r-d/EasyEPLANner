using StaticHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    /// <summary>
    /// Устройство проверки связи.
    /// </summary>
    public class WATCHDOG : IODevice
    {
        public WATCHDOG(
            string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            IDeviceManager deviceManager) : base(
                name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dType = DeviceType.WATCHDOG;
            dSubType = DeviceSubType.NONE;
            this.deviceManager = deviceManager;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            switch (subType)
            {
                case "":
                case nameof(DeviceSubType.WATCHDOG):
                    parameters.Add(Parameter.P_T_GEN, null);
                    parameters.Add(Parameter.P_T_ERR, null);
                    
                    properties.Add(Property.DI_DEV, null);
                    properties.Add(Property.AI_DEV, null);
                    properties.Add(Property.DO_DEV, null);
                    properties.Add(Property.AO_DEV, null);
                    break;

                default:
                    return $"" +
                        $"\"{Name}\" -  неверный тип " +
                        $"({nameof(DeviceSubType.WATCHDOG)}).\n";
            }
            return string.Empty;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt, DeviceSubType dst)
            => dt switch
            {
                DeviceType.WATCHDOG => dst switch
                {
                    DeviceSubType.WATCHDOG => nameof(DeviceSubType.WATCHDOG),
                    _ => string.Empty,
                },
                _ => string.Empty,
            };


        public override Dictionary<ITag, int> GetDeviceProperties(DeviceType dt, DeviceSubType dst)
            => dt switch
            {
                DeviceType.WATCHDOG => dst switch
                {
                    DeviceSubType.WATCHDOG
                    => new()
                    {
                        { Tag.ST, 1 },
                        { Tag.M, 1 },
                        { Parameter.P_T_GEN, 1 },
                        { Parameter.P_T_ERR, 1 },
                    },
                    _ => null,
                },
                _ => null,
            };

        public override string Check()
        {
            var res = base.Check();

            res += CheckSignal(Property.DI_DEV);
            res += CheckSignal(Property.AI_DEV);
            res += CheckSignal(Property.DO_DEV);
            res += CheckSignal(Property.AO_DEV);

            return res;
        }

        private string CheckSignal(Property property)
        {
            if (properties.TryGetValue(property, out var dev_string) &&
                !string.IsNullOrEmpty(dev_string?.ToString()) &&
                deviceManager.GetDevice(dev_string.ToString()).Description is CommonConst.Cap)
            {
                return $"{Name}: к свойству {property} привязано неизвестное устройство;\n";
            }

            return string.Empty;
        }


        private readonly IDeviceManager deviceManager;
    }
}
