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

        public bool IsLogged { get; set; } = false;

        public bool IsRequestByTime { get; set; } = false;

        public int RequestPeriod { get; set; } = 1;

        public double Delta { get; set; } = 0;

        public IChannel Logged() 
        { 
            IsLogged = true; 
            return this; 
        }

        public IChannel GetIndexedCopy(int index)
        {
            return new Channel(Name + $"[ {index} ]")
            {
                Comment = Comment + $" {index}",
                IsLogged = IsLogged
            };
        }

        public IChannel RequestByTime(int requestPeriod = 5000, double delta = 0.2)
        {
            IsRequestByTime = true;
            RequestPeriod = requestPeriod;
            Delta = delta;

            return this;
        }
    }
}
