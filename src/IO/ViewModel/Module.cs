using System.Collections.Generic;
using System.Linq;

namespace IO.ViewModel
{
    public class Module : ModuleViewItem, IModule, IExpandable
    {
        private readonly INode owner;

        private readonly List<IClamp> binds = [];


        public Module(IIOModule module, INode owner)
            : base(module)
        {
            this.owner = owner;

            binds.AddRange(module.Info.ChannelClamps
                .OrderBy(c => c)
                .Select(c => new Clamp(this, c)));
        }

        public IEnumerable<IViewItem> Items => binds;

        public IIONode IONode => owner.IONode;

        public bool Expanded 
        { 
            get => IOModule.Function?.Expanded ?? false;
            set => IOModule.Function?.Expanded = value;
        }
    }
}
