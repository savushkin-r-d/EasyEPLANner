using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public interface ISubtype
    {
        string Description { get; }

        bool Logged {  get; set; }

        List<IChannel> Channels { get;}

        IChannel AddChannel(IChannel channel, int count = 1);
    }
}
