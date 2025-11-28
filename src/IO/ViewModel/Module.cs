using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    public class Module : IModule, IExpandable, IHasIcon, IToolTip
    {
        private readonly INode owner;

        private readonly List<IClamp> binds = [];


        public Module(IIOModule module, INode owner)
        {
            this.owner = owner;

            IOModule = module;

            binds.AddRange(module.Info.ChannelClamps.Select(c => new Clamp(this, c)));
        }

        public IEnumerable<IViewItem> Items => binds;

        public string Name => IOModule.Name;

        public string Description => IOModule.Info.Description;

        string IToolTip.Description => 
            $"Артикул: {IOModule.Info.Name}\n" +
            $"Описание: {IOModule.Info.Description}\n" +
            $"{IOModule.Info.TypeName}";

        public IIOModule IOModule { get; private set; }

        public IIONode IONode => owner.IONode;

        Icon IHasIcon.Icon => IOModule.Info.ModuleColor.Name switch
        {
            nameof(Color.Black) => Icon.BlackModule,
            nameof(Color.Blue) => Icon.BlueModule,
            nameof(Color.Gray) => Icon.GrayModule,
            nameof(Color.Green) => Icon.GreenModule,
            nameof(Color.Lime) => Icon.LimeModule,
            nameof(Color.Orange) => Icon.OrangeModule,
            nameof(Color.Red) => Icon.RedModule,
            nameof(Color.Violet) => Icon.VioletModule,
            nameof(Color.Yellow) => Icon.YellowModule,
            "0" => Icon.EmptyModule,
            _ => Icon.None
        };

        public bool Expanded 
        { 
            get => IOModule.Function?.Expanded ?? false;
            set => IOModule.Function?.Expanded = value;
        }
    }
}
