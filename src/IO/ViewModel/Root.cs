using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Root : IRoot
    {
        private readonly List<ILocation> locations = [];

        public Root(IIOViewModel context)
        {
            Context = context;

            var locs = context.IOManager?.IONodes
                .GroupBy(n => (n.Location, n.LocationDescription))
                .Select(g => new Location(g.Key.Location, g.Key.LocationDescription, [.. g]))
                ?? [];

            locations.AddRange(locs);
        }

        public IIOViewModel Context { get; private set; } 

        public string Name => "ПЛК";

        public string Description => string.Empty;

        public IEnumerable<IViewItem> Items => locations;
    }
}
