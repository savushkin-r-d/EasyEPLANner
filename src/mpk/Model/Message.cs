using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyEPlanner.mpk.Model
{
    public enum MessageType
    {
        Local = 0,
        Global,
        Request
    }

    public class Message : IMessage
    {
        public string Name { get; set; }

        public string Caption { get; set; }

        public bool Visible { get; set; } = true;

        public bool Report { get; set; } = false;

        public MessageType Type { get; set; }

        public int Priority { get; set; } = 5;
    }
}
