using EplanDevice;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public class Subtype : ISubtype
    {
        private static readonly List<string> defaultLogged = 
        [                
            "TE_V",
            "QT_V",
            "FQT_F",
            "PT_V",
            "VC_V",
            "M_V",
            "M_ST",
            "LT_CLEVEL",
            "V_ST",
            "LS_ST",
            "FS_ST",
            "GS_ST",
            "SB_ST",
            "DI_ST",
            "DO_ST",
            "SB_ST",
            "HL_ST",
            "HA_ST",
            "AO_V",
            "AI_V",
            $"{nameof(DeviceType.WATCHDOG)}_{IODevice.Tag.ST}",
        ];

        /// <summary>
        /// Список каналов с НЕ дефолтной протоколируемостью
        /// </summary>
        public static Dictionary<string, bool> ChannelsLogging { get; set; } 
            = [];

        private static readonly List<string> defaultRequestedByTime =
        [
            "TE_V",
            "QT_V",
            "LT_V",
            "PT_V",
            "AO_V",
            "AI_V",
            "FQT_F",
            "M_V",
            "VC_V",
            "LT_CLEVEL",
            "V_V"
        ];

        private static int GetRequestPeriod(string description) => description switch
        {
            "LT_CLEVEL" or "V_V" => 5000,
            _ => 3000,
        };

        private static double GetDelta(string description) => description switch
        {
            "V_V" => 1,
            "VC_V" or "M_V" => 0.5,
            _ when description.StartsWith("QT_") || description.StartsWith("FQT_") => 0.1,
            _ => 0.2,
        };

        public Subtype(string description)
        {
            Description = description;

            if (defaultLogged.Contains(description))
                IsLogged = true;

            if (defaultRequestedByTime.Contains(description))
            {
                IsRequestByTime = true;
                RequestPeriod = GetRequestPeriod(description);
                Delta = GetDelta(description);
            }
        }

        public string Description { get; private set; }

        public bool IsLogged { get; set; } = false;

        public bool IsRequestByTime { get; set; } = false;

        public int RequestPeriod { get; set; } = 1;

        public double Delta { get; set; } = 0;

        public List<IChannel> Channels { get; } = [];

        public IChannel this[string channelName] 
            => Channels.Find(ch => ch.Name == channelName);

        public IChannel AddChannel(IChannel channel, int count = 1)
        {
            channel.IsLogged = IsLogged;
            if (ChannelsLogging.TryGetValue(channel.Name, out var logged))
            {
                channel.IsLogged = logged;
            }

            channel.IsRequestByTime = IsRequestByTime;
            channel.RequestPeriod = RequestPeriod;
            channel.Delta = Delta;

            if (count > 1)
            {
                foreach (var index in Enumerable.Range(1, count))
                {
                    Channels.Add(channel.GetIndexedCopy(index));
                }
                return channel;
            }

            Channels.Add(channel);
            return channel;
        }
    }
}
