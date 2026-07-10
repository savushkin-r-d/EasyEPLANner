namespace IO.ViewModel
{
    public class AppendModuleTarget : IViewItem, IHasIcon
    {
        public AppendModuleTarget(IIONode node)
        {
            IONode = node;
        }

        public IIONode IONode { get; }

        public string Name => "Добавить исключенный модуль в конец";

        public string Description =>
            "Перетащите сюда исключенный модуль для добавления в конец узла";

        Icon IHasIcon.Icon => Icon.AddModule;
    }
}

