using EplanDevice;
using InterprojectExchange;
using NUnit.Framework;

namespace EasyEplannerTests.InterprojectExchangeTest
{
    public class InterprojectExchangeModelTest
    {
        [Test]
        public void AddDeviceData_WithVirtSubType_StoresVirtFilterType()
        {
            var model = new AdvancedProjectModel();
            int deviceType = (int)DeviceType.DI;
            int subTypeIndex = DeviceSubType.DI_VIRT.GetIndex();

            model.AddDeviceData("+TANK1DI1", "Virtual DI", deviceType,
                subTypeIndex);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, model.Devices.Count);
                Assert.AreEqual("DI_VIRT", model.Devices[0].Type);
            });
        }

        [Test]
        public void AddDeviceData_WithoutSubTypeInfo_StoresTypeFromName()
        {
            var model = new AdvancedProjectModel();

            model.AddDeviceData("+TANK1DO1", "Discrete output");

            Assert.AreEqual("DO", model.Devices[0].Type);
        }
    }
}
