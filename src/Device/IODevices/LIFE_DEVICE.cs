using StaticHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    /// <summary>
    /// Устройство проверки связи.
    /// </summary>
    public class LIFE_DEVICE : IODevice
    {
        public LIFE_DEVICE(
            string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber,
            IDeviceManager deviceManager) : base(
                name, eplanName, description,
                deviceNumber, objectName, objectNumber)
        {
            dType = DeviceType.LIFE_DEVICE;
            dSubType = DeviceSubType.NONE;
            this.deviceManager = deviceManager;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            switch (subType)
            {
                case nameof(DeviceSubType.LIFEBIT):
                case nameof(DeviceSubType.LIFECOUNTER):
                    parameters.Add(Parameter.P_DT, null);
                    properties.Add(Property.DEV, null);
                    break;

                default:
                    return $"" +
                        $"\"{Name}\" -  неверный тип " +
                        $"({nameof(DeviceSubType.LIFEBIT)}, " +
                        $"{nameof(DeviceSubType.LIFECOUNTER)}).\n";
            }
            return string.Empty;
        }

        public override string GetDeviceSubTypeStr(DeviceType dt, DeviceSubType dst)
            => dt switch
            {
                DeviceType.LIFE_DEVICE => dst switch
                {
                    DeviceSubType.LIFEBIT => nameof(DeviceSubType.LIFEBIT),
                    DeviceSubType.LIFECOUNTER => nameof(DeviceSubType.LIFECOUNTER),
                    _ => string.Empty,
                },
                _ => string.Empty,
            };


        public override Dictionary<string, int> GetDeviceProperties(DeviceType dt, DeviceSubType dst)
            => dt switch
            {
                DeviceType.LIFE_DEVICE => dst switch
                {
                    DeviceSubType.LIFEBIT or
                    DeviceSubType.LIFECOUNTER 
                    => new()
                    {
                        { Tag.ST, 1 },
                        { Tag.M, 1 },
                        { Parameter.P_DT, 1 },
                    },
                    _ => null,
                },
                _ => null,
            };

        public override string Check()
        {
            var res = base.Check();

            if (properties.TryGetValue(Property.DEV, out var dev_string) &&
                !string.IsNullOrEmpty(dev_string?.ToString()))
            {
                var dev = deviceManager.GetDevice(dev_string.ToString());
                if (dev.Description is CommonConst.Cap)
                    res += $"{Name}: к свойству {Property.DEV} привязано неизвестное устройство;\n";

                else if (dSubType is DeviceSubType.LIFEBIT &&
                    dev.DeviceType is not DeviceType.DI)
                    res += $"{Name}: к свойству {Property.DEV} привязано устройство неверного типа ({DeviceType.DI});\n";

                else if (dSubType is DeviceSubType.LIFECOUNTER &&
                    dev.DeviceType is not DeviceType.AI)
                    res += $"{Name}: к свойству {Property.DEV} привязано устройство неверного типа ({DeviceType.AI});\n";
            }

            return res;
        }

        private IDeviceManager deviceManager;
    }
}
