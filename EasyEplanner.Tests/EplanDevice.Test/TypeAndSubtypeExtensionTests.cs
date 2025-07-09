using EplanDevice;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEplannerTests.EplanDeviceTest
{
    public class TypeAndSubtypeExtensionTests
    {
        [Test]
        public void DeviceSubTypeExtensionsTest()
        {
            var st = DeviceSubType.FQT_F;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(7002, (int)st);
                Assert.AreEqual(2, st.GetIndex());
                Assert.AreEqual(DeviceType.FQT, st.GetDeviceType());
            });
        }

        [Test]
        public void DeviceTypeExtensionsTest()
        {
            var type = DeviceType.DO;

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new List<string>() { "DO", "DO_VIRT" }, type.SubTypeNames());
                CollectionAssert.AreEqual(new List<DeviceSubType>() { DeviceSubType.DO, DeviceSubType.DO_VIRT }, type.SubTypes());

            });
        }
    }
}
