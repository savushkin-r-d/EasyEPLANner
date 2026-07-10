using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    public class SummaryDevices(IDeviceManager deviceManager) : ISummaryDevices
    {
        public Dictionary<string, Dictionary<string, int>> NumberUsedTypes()
        {
            var devices = new Dictionary<string, Dictionary<string, int>>();
            foreach (IODevice dev in deviceManager.Devices)
            {
                if (dev.Description == CommonConst.Cap)
                    continue;

                string deviceSubType = dev.GetDeviceSubTypeStr(dev.DeviceType, dev.DeviceSubType);
                if (!devices.ContainsKey(dev.DeviceType.ToString()))
                    devices.Add(dev.DeviceType.ToString(), []);


                if (devices[dev.DeviceType.ToString()].ContainsKey(deviceSubType))
                {
                    devices[dev.DeviceType.ToString()][deviceSubType]++;
                }
                else
                {
                    devices[dev.DeviceType.ToString()][deviceSubType] = 1;
                }
            }

            return devices;
        }
    }
}
