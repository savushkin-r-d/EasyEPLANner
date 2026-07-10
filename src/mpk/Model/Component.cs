using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.mpk.Model
{
    public class Component : IComponent
    {
        public string Name { get; set; }

        public List<IProperty> Properties { get; set; }

        public List<IMessage> Messages { get; set; }
    }
}
