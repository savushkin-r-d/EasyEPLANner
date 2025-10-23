using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public class Subtype(string description) : ISubtype
    {
        public string Description { get; } = description;

        public bool Logged { get; set; } = false;

        public List<IChannel> Channels { get; } = [];

        public IChannel AddChannel(IChannel channel, int count = 1)
        {
            if (count > 1)
            {
                foreach (var index in Enumerable.Range(1, count))
                {
                    Channels.Add(channel.GetIndexed(index));
                }
                return channel;
            }

            Channels.Add(channel);
            return channel;
        }
    }
}
