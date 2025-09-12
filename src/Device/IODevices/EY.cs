using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    public class EY : IODevice
    {
        public EY(
            string name, string eplanName, string description,
            int deviceNumber, string objectName, int objectNumber) 
            : base(name, eplanName, description,
                  deviceNumber, objectName, objectNumber)
        {

            dSubType = DeviceSubType.NONE;
            dType = DeviceType.EY;
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            switch (subType)
            {
                case "": return SetSubType(DeviceSubType.DEV_CONV_AO2.ToString());

                case nameof(DeviceSubType.DEV_CONV_AO2):
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    SetIOLinkSizes(ArticleName);
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
                DeviceType.EY => dst switch
                {
                    DeviceSubType.DEV_CONV_AO2 => nameof(DeviceSubType.DEV_CONV_AO2),
                    _ => string.Empty,
                },
                _ => string.Empty,
            };


        public override Dictionary<ITag, int> GetDeviceProperties(DeviceType dt, DeviceSubType dst)
            => dt switch
            {
                DeviceType.EY => dst switch
                {
                    DeviceSubType.DEV_CONV_AO2
                    => new()
                    {
                        { Tag.ST, 1 },
                        { Tag.V, 1 },
                        { Tag.V2, 1 },
                        { Tag.ERR, 1 },
                    },
                    _ => null,
                },
                _ => null,
            };
    }
}
