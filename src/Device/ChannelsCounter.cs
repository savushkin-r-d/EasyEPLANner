using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    /// <summary>
    /// Счетчик каналов
    /// </summary>
    /// <param name="deviceManager">менеджер устройств</param>
    public class ChannelsCounter(IDeviceManager deviceManager) : IChannelsCounter
    {
        public Dictionary<string, Dictionary<string, int>> GetTypesCount()
        {
            var devices = new Dictionary<string, Dictionary<string, int>>();
            foreach (IODevice dev in deviceManager.Devices)
            {
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

        public (int DI, int DO, int AI, int AO) CalculateUsedChannelsCount()
        {
            var subtypes = GetTypesCount().SelectMany(t => t.Value);

            return (
                subtypes.Select(st => GetSubtypeChannels(st.Key).DI * st.Value).Sum(),
                subtypes.Select(st => GetSubtypeChannels(st.Key).DO * st.Value).Sum(),
                subtypes.Select(st => GetSubtypeChannels(st.Key).AI * st.Value).Sum(),
                subtypes.Select(st => GetSubtypeChannels(st.Key).AO * st.Value).Sum()
                );
        }

        public List<SubtypeChannels> SubtypeChannels { get; set; } = [];

        public void AddChannelsCount(string dstStr, int DI, int DO, int AI, int AO)
        {
            if (Enum.TryParse<DeviceSubType>(dstStr, out var dst) &&
                !SubtypeChannels.Any(c => c.SubType == dst))
            {
                SubtypeChannels.Add(new(dst, DI, DO, AI, AO));
            }
        }

        public SubtypeChannels GetSubtypeChannels(string dstStr)
        {
            if (Enum.TryParse<DeviceSubType>(dstStr, out var dst))
            {
                return SubtypeChannels.Find(c => c.SubType == dst) ?? new SubtypeChannels(dst, 0, 0, 0, 0);
            }

            return new SubtypeChannels(DeviceSubType.NONE, 0, 0, 0, 0);
        }

        public int[] GetChannelsCount(string dstStr)
        {
            return GetSubtypeChannels(dstStr)?.Count ?? [0, 0, 0, 0];
        }
    }

    /// <summary>
    /// Список каналов подтипа
    /// </summary>
    /// <param name="subType">Подтип</param>
    /// <param name="DI">Количество каналов DI</param>
    /// <param name="DO">Количество каналов DO</param>
    /// <param name="AI">Количество каналов AI</param>
    /// <param name="AO">Количество каналов AO</param>
    public class SubtypeChannels(DeviceSubType subType, int DI, int DO, int AI, int AO)
    {
        public DeviceSubType SubType { get; private set; } = subType;

        public int DI { get; private set; } = DI;

        public int DO { get; private set; } = DO;

        public int AI { get; private set; } = AI;

        public int AO { get; private set; } = AO;

        public int[] Count => [DI, DO, AI, AO];
    }
}
