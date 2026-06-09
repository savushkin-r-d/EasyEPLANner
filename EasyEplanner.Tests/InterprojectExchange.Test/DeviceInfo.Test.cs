using EplanDevice;
using InterprojectExchange;
using NUnit.Framework;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class DeviceInfoTest
    {
        [Test]
        public void Type_VirtualDevice_ReturnsVirtSubType()
        {
            int deviceType = (int)DeviceType.DO;
            int subTypeIndex = DeviceSubType.DO_VIRT.GetIndex();

            var deviceInfo = new DeviceInfo("+TANK1DO1", "Test DO virt",
                deviceType, subTypeIndex);

            Assert.AreEqual("DO_VIRT", deviceInfo.Type);
        }

        [Test]
        public void Type_NonVirtualDevice_ReturnsTypeFromName()
        {
            int deviceType = (int)DeviceType.DO;
            int subTypeIndex = DeviceSubType.DO.GetIndex();

            var deviceInfo = new DeviceInfo("+TANK1DO1", "Test DO",
                deviceType, subTypeIndex);

            Assert.AreEqual("DO", deviceInfo.Type);
        }

        [Test]
        public void Type_WithoutSubTypeInfo_ReturnsTypeFromName()
        {
            var deviceInfo = new DeviceInfo("+TANK1DO1", "Test DO");

            Assert.AreEqual("DO", deviceInfo.Type);
        }
    }
}
