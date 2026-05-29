using EplanDevice;
using System.Collections.Generic;

namespace EasyEPlanner.Devices.ViewModel
{
    public interface IDevicesViewModel
    {
        IDevicesRoot Root { get; }

        IEnumerable<IDevicesRoot> Roots { get; }

        DeviceManager DeviceManager { get; }

        DevicesGroupingMode GroupingMode { get; set; }

        DevicesSearchContext SearchContext { get; }

        void RebuildTree();
    }
}
