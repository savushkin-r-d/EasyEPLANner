using IO.ViewModel;

namespace EasyEPlanner.Devices.ViewModel
{
    public interface IDevicesRoot : IViewItem
    {
        IDevicesViewModel Context { get; }
    }
}
