using EasyEPlanner.Devices.ViewModel.ViewInterface;
using IO.ViewModel;

namespace EasyEPlanner.Devices.ViewModel
{
    public sealed class DevicesRoot : FilterableViewItemBase, IDevicesRoot, IBoldName
    {
        private int deviceCount;

        public DevicesRoot(IDevicesViewModel context)
            : base(context, null)
        {
            Context = context;
            DevicesTreeBuilder.Build(this, context);
            UpdateHeader();
        }

        public new IDevicesViewModel Context { get; }

        public override string Name { get; protected set; } = "Устройства проекта";

        public override DevicesIcon Icon => DevicesIcon.None;

        public void SetDeviceCount(int count)
        {
            deviceCount = count;
            UpdateHeader();
        }

        private void UpdateHeader() => Name = $"Устройства проекта ({deviceCount})";
    }
}
