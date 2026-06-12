using Aga.Controls.Tree;
using EasyEPlanner;
using EplanDevice;
using NUnit.Framework;
using System;
using System.Linq;

namespace EasyEPlanner.Devices.Tests
{
    public class StartValuesForBindingTest
    {
        [Test]
        public void GetNodeFromSelectedNode_ReturnsNodeFromTag()
        {
            var channelNode = CreateChannelTreeNode(out IODevice _);
            var treeNodeAdv = new TreeNodeAdv(channelNode);
            var startValues = new StartValuesForBinding(null);

            var node = startValues.GetNodeFromSelectedNode(treeNodeAdv);

            Assert.AreSame(channelNode, node);
        }

        [Test]
        public void GetNodeFromSelectedNode_ThrowsWhenTagIsNotNode()
        {
            var treeNodeAdv = new TreeNodeAdv("invalid");
            var startValues = new StartValuesForBinding(null);

            var ex = Assert.Throws<Exception>(() =>
                startValues.GetNodeFromSelectedNode(treeNodeAdv));

            Assert.AreEqual("Ошибка инициализации выбранного узла", ex.Message);
        }

        [Test]
        public void GetChannel_ReturnsChannelFromNodeTag()
        {
            var channelNode = CreateChannelTreeNode(out IODevice device);
            var channel = device.Channels.Single();
            var startValues = new StartValuesForBinding(null);

            var result = startValues.GetChannel(channelNode);

            Assert.AreSame(channel, result);
        }

        [Test]
        public void GetChannel_ThrowsWhenTagIsNotChannel()
        {
            var node = new Node("broken") { Tag = "not-a-channel" };
            var startValues = new StartValuesForBinding(null);

            var ex = Assert.Throws<Exception>(() => startValues.GetChannel(node));

            Assert.AreEqual("Канал не найден", ex.Message);
        }

        [Test]
        public void GetDevice_ReturnsDeviceFromParentTag()
        {
            var channelNode = CreateChannelTreeNode(out IODevice device);
            var startValues = new StartValuesForBinding(null);

            var result = startValues.GetDevice(channelNode);

            Assert.AreSame(device, result);
        }

        [Test]
        public void GetDevice_ThrowsWhenParentDeviceMissing()
        {
            var channelNode = CreateChannelTreeNode(out IODevice device);
            var orphanParent = new Node("orphan");
            channelNode.Parent = orphanParent;
            var startValues = new StartValuesForBinding(null);

            var ex = Assert.Throws<Exception>(() => startValues.GetDevice(channelNode));

            Assert.AreEqual("Устройство не найдено", ex.Message);
        }

        private static Node CreateChannelTreeNode(out IODevice device)
        {
            device = new DO("TANK2DO1", "+TANK2-DO1", "desc", 1, "TANK", 2);
            device.SetSubType("DO");
            var channel = device.Channels.Single();

            var deviceNode = new Node("device") { Tag = device };
            var channelNode = new Node("channel") { Tag = channel };
            channelNode.Parent = deviceNode;
            return channelNode;
        }
    }
}
