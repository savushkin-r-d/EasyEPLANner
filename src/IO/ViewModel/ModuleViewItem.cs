using System.Drawing;

namespace IO.ViewModel
{
    public abstract class ModuleViewItem : IHasIcon, IToolTip
    {
        protected ModuleViewItem(IIOModule module)
        {
            IOModule = module;
        }

        public string Name => IOModule.Name;

        public string Description => IOModule.Info.Description;

        public IIOModule IOModule { get; }

        protected virtual string ToolTipName => Name;

        string IToolTip.Name => ToolTipName;

        string IToolTip.Description =>
            $"Артикул: {IOModule.Info.Name}\n" +
            $"Описание: {IOModule.Info.Description}\n" +
            $"{IOModule.Info.TypeName}";

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
    }
}

