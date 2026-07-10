using EplanDevice;

namespace EasyEPlanner.Devices.ViewModel
{
    public class DevicesViewModel : IDevicesViewModel
    {
        public DevicesViewModel(DeviceManager deviceManager)
        {
            DeviceManager = deviceManager;
            SearchContext = new DevicesSearchContext();
            Root = new DevicesRoot(this);
        }

        public IDevicesRoot Root { get; private set; }

        public System.Collections.Generic.IEnumerable<IDevicesRoot> Roots =>
            [Root];

        public DeviceManager DeviceManager { get; }

        public DevicesGroupingMode GroupingMode { get; set; } =
            DevicesGroupingMode.TypeThenObject;

        public DevicesSearchContext SearchContext { get; }

        public void RebuildTree()
        {
            Root = new DevicesRoot(this);
        }
    }
}
