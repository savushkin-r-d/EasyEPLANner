using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    /// <summary>
    /// Счетчик логических каналов
    /// </summary>
    /// <param name="deviceManager">менеджер устройств</param>
    public class ControlChannelsCounter(ISummaryDevices summaryDevices) : IControlChannelsCounter
    {

        public (int DI, int DO, int AI, int AO) CalculateUsedChannelsCount()
        {
            var subtypes = summaryDevices.NumberUsedTypes().SelectMany(t => t.Value);

            return (
                subtypes.Select(st => GetSubtypeControlChannels(st.Key).DI * st.Value).Sum(),
                subtypes.Select(st => GetSubtypeControlChannels(st.Key).DO * st.Value).Sum(),
                subtypes.Select(st => GetSubtypeControlChannels(st.Key).AI * st.Value).Sum(),
                subtypes.Select(st => GetSubtypeControlChannels(st.Key).AO * st.Value).Sum()
                );
        } 

        public List<SubtypeControlChannels> SubtypeChannels { get; set; } = [];

        public void AddChannelsCount(string dstStr, int DI, int DO, int AI, int AO)
        {
            if (Enum.TryParse<DeviceSubType>(dstStr, out var dst) &&
                !SubtypeChannels.Any(c => c.SubType == dst))
            {
                SubtypeChannels.Add(new(dst, DI, DO, AI, AO));
            }
        }

        public SubtypeControlChannels GetSubtypeControlChannels(string dstStr)
        {
            if (Enum.TryParse<DeviceSubType>(dstStr, out var dst))
            {
                return SubtypeChannels.Find(c => c.SubType == dst) ?? new SubtypeControlChannels(dst, 0, 0, 0, 0);
            }

            return new SubtypeControlChannels(DeviceSubType.NONE, 0, 0, 0, 0);
        }

        public int[] GetChannelsCount(string dstStr)
        {
            return GetSubtypeControlChannels(dstStr)?.ChannelsCount ?? [0, 0, 0, 0];
        }
    }
}
