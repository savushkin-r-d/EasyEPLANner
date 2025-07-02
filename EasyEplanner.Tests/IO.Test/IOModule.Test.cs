using EplanDevice;
using IO;
using Moq;
using NUnit.Framework;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTests
{
    public class IOModuleTest
    {

        [Test]
        public void AddClampFunction()
        {
            var module = new IOModule(0, 0, null);

            var clampFunction = Mock.Of<IEplanFunction>(f => f.ClampNumber == 1);
            module.AddClampFunction(clampFunction);

            Assert.AreSame(clampFunction, module.ClampFunctions[1]);
        }

        [Test]
        public void ClearBind()
        {
            var module = new IOModule(0, 0, null);

            module.Devices[1] = new List<IIODevice>() { Mock.Of<IIODevice>() };
            module.DevicesChannels[1] = new List<IODevice.IIOChannel>() { Mock.Of<IODevice.IIOChannel>() };

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(module.Devices[1]);
                Assert.IsNotNull(module.DevicesChannels[1]);

                module.ClearBind(1);

                Assert.IsNull(module.Devices[1]);
                Assert.IsNull(module.DevicesChannels[1]);
            });
        }

        [Test]
        public void GetClampBinding()
        {
            var module = new IOModule(0, 0, null);


            var dev = Mock.Of<IIODevice>();
            var channel = Mock.Of<IODevice.IIOChannel>();
            module.Devices[1] = new List<IIODevice>() { dev };
            module.DevicesChannels[1] = new List<IODevice.IIOChannel>() { channel };

            CollectionAssert.AreEqual(new List<(IIODevice, IODevice.IIOChannel)>() { (dev, channel) }, module.GetClampBinding(1));
        }
    }
}
