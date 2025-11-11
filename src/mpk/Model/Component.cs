using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.mpk.Model
{
    public class Component
    {
        public string Name { get; set; }

        public List<Property> Properties { get; set; }

        public List<Message> Messages { get; set; }
    }
}
