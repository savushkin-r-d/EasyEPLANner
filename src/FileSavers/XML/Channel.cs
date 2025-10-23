using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.FileSavers.XML
{
    public class Channel : IChannel
    {
        public Channel (string name)
        {
            Name = name;
        }

        public Channel (string name, string comment)
        {
            Name = name;
            Comment = comment;
        }

        public string Description => Name + (Comment is null or "" ? "" : $" -- {Comment}");

        public string Name { get; private set; }

        public string Comment { get; set; } = "";

        public bool Logged { get; set; } = false;

        public IChannel GetIndexed(int index)
        {
            return new Channel(Name + $"[ {index} ]")
            {
                Comment = Comment + $" {index}",
                Logged = Logged
            };
        }
    }
}
