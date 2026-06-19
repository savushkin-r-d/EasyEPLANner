using EasyEPlanner.Devices.View;
using EasyEPlanner.Devices.ViewModel;
using EasyEPlanner.Devices.ViewModel.ViewInterface;
using EplanDevice;
using NUnit.Framework;

namespace EasyEPlanner.Devices.Tests
{
    public class DevicesTreeViewKeysTest
    {
        [Test]
        public void GetViewItemKey_ReturnsStableKeysForTreeNodes()
        {
            var context = new DevicesViewModel(null);
            var root = context.Root;
            var device = new DO("TANK2DO1", "+TANK2-DO1", "desc", 1, "TANK", 2);
            device.SetSubType("DO");

            var deviceNode = new DevicesDeviceNode(context,
                (FilterableViewItemBase)root, device, device.Name);
            var channelsGroup = new DevicesGroupNode(context, deviceNode, "Каналы",
                DevicesIcon.Channels, new DevicesChannelItem[0]);
            var channelItem = new DevicesChannelItem(context, channelsGroup,
                device.Channels[0]);

            Assert.AreEqual("root", DevicesTreeViewKeys.GetViewItemKey(root));
            Assert.AreEqual("device:+TANK2-DO1",
                DevicesTreeViewKeys.GetViewItemKey(deviceNode));
            Assert.AreEqual("group:+TANK2-DO1:Каналы",
                DevicesTreeViewKeys.GetViewItemKey(channelsGroup));
            Assert.IsNull(DevicesTreeViewKeys.GetViewItemKey(channelItem));
        }
    }
}
