namespace EasyEPlanner.mpk.ViewModel
{
    public interface IMpkSaverContext
    {
        string MainContainerName { get; set; }
        string MpkDirectory { get; set; }
        bool Rewrite { get; set; }
    }
}