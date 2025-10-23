using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public interface IChannel
    {
        string Description { get; }

        string Name { get; }

        string Comment { get; set; }

        bool Logged { get; set; }

        IChannel GetIndexed(int index);
    }
}
