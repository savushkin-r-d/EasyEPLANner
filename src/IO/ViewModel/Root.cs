using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Root : IRoot, IExpandable
    {
        private readonly List<IViewItem> items = [];

        public Root(IIOViewModel context)
        {
            Context = context;

            /// stub_id - индекс заглушки
            /// если узел не определен - у него нет location - группируем его отдельно
            var stub_id = 0;
            var deletedModules = context.IOManager?.DeletedModules ?? [];
            var locs = context.IOManager?.IONodes
                .GroupBy(n => ((string location, string description)) (n.Type is IONode.TYPES.T_EMPTY ? ("", $"{stub_id++}") : (n.Location, n.LocationDescription)))
                .Select(g =>  (IViewItem) (g.Key.location is "" ? new Node(g.First(), null) : new Location(g.Key.location, g.Key.description, [.. g], GetDeletedModulesByLocation(deletedModules, g.Key.location))))
                ?? [];

            items.AddRange(locs);

            var deletedModulesWithoutLocation = GetDeletedModulesByLocation(
                deletedModules, string.Empty);
            if (deletedModulesWithoutLocation.Any())
            {
                items.Add(new DeletedModulesGroup(
                    deletedModulesWithoutLocation));
            }
        }

        private static IEnumerable<IIOModule> GetDeletedModulesByLocation(
            IEnumerable<IIOModule> deletedModules, string location)
        {
            return deletedModules.Where(module => module.Location == location);
        }

        public IIOViewModel Context { get; private set; } 

        public string Name => "ПЛК";

        public string Description => string.Empty;

        public IEnumerable<IViewItem> Items => items;

        public bool Expanded { get; set; } = true;
    }
}
