using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public class Driver : IDriver
    {

        public List<ISubtype> Subtypes { get; } = [];

        public ISubtype this[string subtype] => Subtypes.Find(st => st.Description == subtype);

        public IChannel AddChannel(string stDescription, IChannel channel, int count = 1)
        {
            if (this[stDescription] is { } subtype)
            {
                return subtype.AddChannel(channel, count);
            }

            var newSubtype = new Subtype(stDescription);
            Subtypes.Add(newSubtype);
            
            return newSubtype.AddChannel(channel, count);
        }

        public IChannel AddChannel(string stDescription, string channelName, string comment = "", int count = 1)
        {
            return AddChannel(stDescription, new Channel(channelName, comment), count);
        }
    }
}
