using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public interface IDriver
    {
        ISubtype this[string subtype] { get; } 
        
        List<ISubtype> Subtypes { get; }

        IChannel AddChannel(string stDescription, IChannel channel, int count = 1);

        IChannel AddChannel(string stDescription, string channelName, string comment = "", int count = 1);
    }
}
