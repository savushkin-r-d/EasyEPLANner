using Aga.Controls.Tree;
using EplanDevice;
using System.Diagnostics.CodeAnalysis;

namespace EasyEPlanner
{
    internal sealed class DeviceChannelBindingSource : IDeviceBindingSource
    {
        public DeviceChannelBindingSource(IODevice device, IODevice.IOChannel channel)
        {
            Device = device;
            Channel = channel;
        }

        public IODevice Device { get; }

        public IODevice.IOChannel Channel { get; }
    }

    [ExcludeFromCodeCoverage]
    internal sealed class DevicesFormBindingSource : IDeviceBindingSource
    {
        private readonly StartValuesForBinding startValues;

        public DevicesFormBindingSource(StartValuesForBinding startValues)
        {
            this.startValues = startValues;
        }

        public IODevice Device
        {
            get
            {
                TreeNodeAdv selectedNode = startValues.GetSelectedNode();
                Node node = startValues.GetNodeFromSelectedNode(selectedNode);
                return startValues.GetDevice(node);
            }
        }

        public IODevice.IOChannel Channel
        {
            get
            {
                TreeNodeAdv selectedNode = startValues.GetSelectedNode();
                Node node = startValues.GetNodeFromSelectedNode(selectedNode);
                return startValues.GetChannel(node);
            }
        }
    }
}
