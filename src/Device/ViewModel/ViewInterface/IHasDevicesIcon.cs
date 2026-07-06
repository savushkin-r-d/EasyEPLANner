namespace EasyEPlanner.Devices.ViewModel.ViewInterface
{
    public enum DevicesIcon
    {
        None = -1,
        Root = 0,
        Type,
        Object,
        Device,
        Data,
        Parameters,
        RuntimeParameters,
        Properties,
        Channels,
        Channel,
        Clamp,
        GoToFas,
    }

    public interface IHasDevicesIcon
    {
        DevicesIcon Icon { get; }
    }
}
