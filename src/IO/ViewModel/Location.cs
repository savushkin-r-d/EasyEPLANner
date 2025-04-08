using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Location : ILocation, IExpandable, IHasIcon
    {
        private List<INode> nodes = [];

        public Location(string name, string description,IEnumerable<IIONode> nodes, IRoot owner)
        {
            Name = name;
            Description = description;

            this.nodes.AddRange(nodes.Select(n => new Node(n, this)));
        }

        public IEnumerable<IViewItem> Items => nodes;

        public string Name { get; private set; }

        public string Description { get; private set; }

        Icon IHasIcon.Icon => Icon.Cab;
    }
}
