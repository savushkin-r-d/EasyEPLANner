using System.Drawing;

namespace IO.ViewModel
{
    public class DeletedModule : IViewItem, IHasIcon, IToolTip
    {
        private readonly IIOModule module;

        public DeletedModule(IIOModule module)
        {
            this.module = module;
        }

        public string Name => module.Name;

        public string Description => module.Info.Description;

        string IToolTip.Name => module.Function?.VisibleName ?? Name;

        string IToolTip.Description =>
            $"Артикул: {module.Info.Name}\n" +
            $"Описание: {module.Info.Description}\n" +
            $"{module.Info.TypeName}";

        Icon IHasIcon.Icon => module.Info.ModuleColor.Name switch
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
