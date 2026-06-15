using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Location : ILocation, IExpandable, IHasIcon, IHasBindingError,
        IHasDescriptionIcon
    {
        private readonly List<INode> nodes = [];
        private readonly List<IViewItem> items = [];

        public Location(string name, string description,
            IEnumerable<IIONode> nodes)
            : this(name, description, nodes, [])
        {
        }

        public Location(string name, string description,
            IEnumerable<IIONode> nodes, IEnumerable<IIOModule> deletedModules)
        {
            Name = name;
            Description = description;

            this.nodes.AddRange(nodes.Select(n => new Node(n, this)));
            items.AddRange(this.nodes);

            if (deletedModules.Any())
            {
                items.Add(new DeletedModulesGroup(deletedModules));
            }

            Expanded = nodes.Any(n => n.Function?.Expanded ?? false) ||
                deletedModules.Any();
        }

        public IEnumerable<IViewItem> Items => items;

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool Expanded { get; set; }

        Icon IHasIcon.Icon => Icon.Cab;

        public bool HasBindingError => nodes.Any(n => n.HasBindingError);

        Icon IHasDescriptionIcon.Icon =>
            HasBindingError ? Icon.Error : Icon.None;
    }
}
