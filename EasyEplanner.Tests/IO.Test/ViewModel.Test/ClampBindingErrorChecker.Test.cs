using EasyEPlanner;
using EplanDevice;
using IO;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using static EplanDevice.IODevice;

namespace IOTests
{
    public class ClampBindingErrorCheckerTest
    {
        [Test]
        public void HasUndefinedDevice_BoundCapDevice_ReturnsTrue()
        {
            var capDevice = Mock.Of<IIODevice>(d =>
                d.Description == CommonConst.Cap);
            var module = Mock.Of<IIOModule>(m =>
                m.Devices == new List<IIODevice>[]
                {
                    null,
                    new List<IIODevice> { capDevice },
                });

            Assert.IsTrue(ClampBindingErrorChecker.HasUndefinedDevice(
                module, 1, null));
        }

        [Test]
        public void HasUndefinedDevice_FunctionalTextWithCap_ReturnsTrue()
        {
            var capDevice = new V(
                "OBJ1V1", "+OBJ1-V1", CommonConst.Cap, 1, "OBJ", 1, string.Empty);
            var deviceManager = Mock.Of<IDeviceManager>(dm =>
                dm.GetDevice("+OBJ1-V1") == capDevice);

            var clampFunction = Mock.Of<IEplanFunction>(f =>
                f.FunctionalText == "+OBJ1-V1");

            var module = Mock.Of<IIOModule>(m =>
                m.Devices == new List<IIODevice>[2]);

            Assert.IsTrue(ClampBindingErrorChecker.HasUndefinedDevice(
                module, 1, clampFunction, deviceManager));
        }

        [Test]
        public void HasUndefinedDevice_ReserveFunctionalText_ReturnsFalse()
        {
            var clampFunction = Mock.Of<IEplanFunction>(f =>
                f.FunctionalText == CommonConst.Reserve);

            var module = Mock.Of<IIOModule>(m =>
                m.Devices == new List<IIODevice>[2]);

            Assert.IsFalse(ClampBindingErrorChecker.HasUndefinedDevice(
                module, 1, clampFunction, Mock.Of<IDeviceManager>()));
        }

        [Test]
        public void HasInvalidChannelBinding_AiOnAoClamp_ReturnsTrue()
        {
            var device = new V(
                "OBJ1V1", "+OBJ1-V1", "Клапан", 1, "OBJ", 1, string.Empty);
            var channel = Mock.Of<IIOChannel>(c => c.Name == IOChannel.AI);
            var moduleInfo = Mock.Of<IIOModuleInfo>(i =>
                i.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI &&
                i.ChannelAddressesIn == new[] { -1, -1, -1, -1 } &&
                i.ChannelAddressesOut == new[] { -1, -1, 0, 1 });
            var binding = new List<(IIODevice, IIOChannel)>
            {
                new Tuple<IIODevice, IIOChannel>(device, channel).ToValueTuple(),
            };

            var module = Mock.Of<IIOModule>(m => m.Info == moduleInfo);
            Mock.Get(module).Setup(m => m.GetClampBinding(2)).Returns(binding);

            Assert.IsTrue(ClampBindingErrorChecker.HasInvalidChannelBinding(
                module, 2, null));
        }

        [Test]
        public void HasInvalidChannelBinding_AoOnAoClamp_ReturnsFalse()
        {
            var device = new V(
                "OBJ1V1", "+OBJ1-V1", "Клапан", 1, "OBJ", 1, string.Empty);
            var channel = Mock.Of<IIOChannel>(c => c.Name == IOChannel.AO);
            var moduleInfo = Mock.Of<IIOModuleInfo>(i =>
                i.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI &&
                i.ChannelAddressesIn == new[] { -1, -1, -1, -1 } &&
                i.ChannelAddressesOut == new[] { -1, -1, 0, 1 });
            var binding = new List<(IIODevice, IIOChannel)>
            {
                new Tuple<IIODevice, IIOChannel>(device, channel).ToValueTuple(),
            };

            var module = Mock.Of<IIOModule>(m => m.Info == moduleInfo);
            Mock.Get(module).Setup(m => m.GetClampBinding(2)).Returns(binding);

            Assert.IsFalse(ClampBindingErrorChecker.HasInvalidChannelBinding(
                module, 2, null));
        }

        [Test]
        public void HasBindingError_ExistingDeviceWithoutBinding_ReturnsTrue()
        {
            var device = new V(
                "OBJ1V1", "+OBJ1-V1", "Клапан", 1, "OBJ", 1, string.Empty);
            var deviceManager = Mock.Of<IDeviceManager>(dm =>
                dm.GetDevice("+OBJ1-V1") == device);
            var clampFunction = Mock.Of<IEplanFunction>(f =>
                f.FunctionalText == "+OBJ1-V1");
            var module = Mock.Of<IIOModule>(m =>
                m.Devices == new List<IIODevice>[2] &&
                m.GetClampBinding(1) == null);

            Assert.IsTrue(ClampBindingErrorChecker.HasBindingError(
                module, 1, clampFunction, deviceManager));
        }
    }
}
