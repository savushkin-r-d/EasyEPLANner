namespace IO.ViewModel
{
    public class DeletedModule : ModuleViewItem
    {
        public DeletedModule(IIOModule module)
            : base(module)
        {
        }

        protected override string ToolTipName =>
            IOModule.Function?.VisibleName ?? Name;
    }
}
