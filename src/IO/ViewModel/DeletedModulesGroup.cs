using System.Collections.Generic;
using System.Linq;

namespace IO.ViewModel
{
    public class DeletedModulesGroup : IViewItem, IExpandable, IHasIcon
    {
        private readonly List<IViewItem> modules = [];

        public DeletedModulesGroup(IEnumerable<IIOModule> modules)
        {
            this.modules.AddRange(modules
                .OrderBy(module => module.PhysicalNumber)
                .Select(module => new DeletedModule(module)));
        }

        public string Name => "Исключенные модули";

        public string Description => "Модули, исключенные из активной структуры PLC";

        public IEnumerable<IViewItem> Items => modules;

        public bool Expanded { get; set; } = true;

        Icon IHasIcon.Icon => Icon.EmptyModule;
    }
}
