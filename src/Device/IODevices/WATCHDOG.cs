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
                    
                    properties.Add(Property.DI_dev, null);
                    properties.Add(Property.AI_dev, null);
                    properties.Add(Property.DO_dev, null);
                    properties.Add(Property.AO_dev, null);
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
            string res = string.Empty;

            res += string.Join("", Parameters.Where(par => par.Value is null)
                .Select(par => $"{name} : не задан параметр (доп. поле 3) \"{par.Key.Name}\".\n"));

            res += CheckSignalsPair(Property.AI_dev, Property.DI_dev);
            res += CheckSignalsPair(Property.AO_dev, Property.DO_dev);

            res += CheckValidSignal(Property.DI_dev);
            res += CheckValidSignal(Property.AI_dev);
            res += CheckValidSignal(Property.DO_dev);
            res += CheckValidSignal(Property.AO_dev);

            return res;
        }

        private string CheckSignalsPair(Property firstProperty, Property secondProperty)
        {
            if (properties.TryGetValue(firstProperty, out var first_string) &&
                properties.TryGetValue(secondProperty, out var second_string) &&
                string.IsNullOrEmpty(first_string?.ToString()) &&
                string.IsNullOrEmpty(second_string?.ToString()))
            {
                return $"{Name}: к свойствам {firstProperty} и {secondProperty} (доп. поле 4) не привязано устройство;\n";
            }

            return string.Empty;
        }

        private string CheckValidSignal(Property property)
        {
            if (properties.TryGetValue(property, out var dev_string) &&
                dev_string?.ToString() != string.Empty &&
                !string.IsNullOrEmpty(dev_string?.ToString()) &&
                deviceManager.GetDevice(dev_string.ToString()).Description is CommonConst.Cap)
            {
                return $"{Name}: к свойству {property} (доп. поле 4) привязано неизвестное устройство;\n";
            }

            return string.Empty;
        }


        private readonly IDeviceManager deviceManager;
    }
}
