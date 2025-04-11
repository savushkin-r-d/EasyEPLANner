using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Location : ILocation, IExpandable, IHasIcon
    {
        private readonly List<INode> nodes = [];

        public Location(string name, string description, IEnumerable<IIONode> nodes)
        {
            Name = name;
            Description = description;

            this.nodes.AddRange(nodes.Select(n => new Node(n, this)));

            Expanded = nodes.Any(n => n.Function.Expanded);
        }

        public IEnumerable<IViewItem> Items => nodes;

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool Expanded { get; set; }

        Icon IHasIcon.Icon => Icon.Cab;
    }
}
