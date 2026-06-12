using EasyEPlanner.Devices.ViewModel;
using EasyEPlanner.Devices.ViewModel.ViewInterface;
using EplanDevice;
using NUnit.Framework;
using System.Linq;

namespace EasyEPlanner.Devices.Tests
{
    public class DevicesChannelItemTest
    {
        [Test]
        public void Description_ReflectsChannelBindingState()
        {
            var device = CreateDeviceWithChannel();
            var channel = device.Channels.Single();
            var channelItem = CreateChannelItem(device, channel);

            Assert.AreEqual(string.Empty, channelItem.Description);

            channel.SetChannel(0, 1, 5, 101, 0, 0);

            Assert.AreEqual("A101:5", channelItem.Description);

            channel.Clear();

            Assert.AreEqual(string.Empty, channelItem.Description);
        }

        [Test]
        public void DescriptionIcon_ReflectsChannelBindingState()
        {
            var device = CreateDeviceWithChannel();
            var channel = device.Channels.Single();
            var channelItem = CreateChannelItem(device, channel);

            Assert.AreEqual(DevicesIcon.None, channelItem.DescriptionIcon);

            channel.SetChannel(0, 1, 5, 101, 0, 0);

            Assert.AreEqual(DevicesIcon.Clamp, channelItem.DescriptionIcon);
        }

        [Test]
        public void Device_ReturnsAncestorDeviceNode()
        {
            var device = CreateDeviceWithChannel();
            var channel = device.Channels.Single();
            var channelItem = CreateChannelItem(device, channel);

            Assert.AreSame(device, channelItem.Device);
        }

        private static DO CreateDeviceWithChannel()
        {
            var device = new DO("TANK2DO1", "+TANK2-DO1", "desc", 1, "TANK", 2);
            device.SetSubType("DO");
            return device;
        }

        private static DevicesChannelItem CreateChannelItem(IODevice device,
            IODevice.IOChannel channel)
        {
            var context = new DevicesViewModel(null);
            var deviceNode = new DevicesDeviceNode(context,
                (FilterableViewItemBase)context.Root, device, device.Name);
            return new DevicesChannelItem(context, deviceNode, channel);
        }
    }
}
