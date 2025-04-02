using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Clamp : IClamp, IHasIcon, IHasDescriptionIcon
    {
        private IModule owner;

        private int clamp;

        public Clamp(IModule owner, int clamp)
        {
            this.owner = owner;
            this.clamp = clamp;
        }

        public string Name => clamp.ToString();

        public string Description => string.Join(" || ",
            Module.Devices[clamp]?
                .Zip(Module.DevicesChannels[clamp],
                    (dev, channel) => $"{dev.Name}{((channel.Comment != "")? ":" + channel.Comment : "")}") ?? []);

        public IEplanFunction ClampFunction => Module.ClampFunctions[clamp];

        public IIOModule Module => owner.IOModule;

        public IIONode Node => owner.IONode;

        Icon IHasIcon.Icon => Icon.Clamp;

        Icon IHasDescriptionIcon.Icon => (Module.Devices[clamp]?.Any() ?? false)? Icon.Cable : Icon.None;

        public void Reset()
        {
            Module.ClearBind(clamp);
        }
    }
}
