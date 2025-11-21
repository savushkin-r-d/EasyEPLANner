using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasyEPlanner.mpk.Model
{
    public class Container
    {
        public string Name { get; set; }

        public int Build { get; set; } = 20;

        public int Version { get; set; } = 1;

        public Attributes Attributes { get; set; } = new();

        public List<Component> Components { get; set; } = [];
    }
}
