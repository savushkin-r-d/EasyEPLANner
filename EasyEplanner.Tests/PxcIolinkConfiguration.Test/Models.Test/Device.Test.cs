using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.PxcIolinkConfigration.Models
{
    public class DeviceTest
    {
        [Test]
        public void ShouldSerializePort_DeviceWithZeroPort_ReturnsFalse()
        {
            var device = new Device();

            Assert.IsFalse(device.ShouldSerializePort());
        }

        [Test]
        public void ShouldSerializePort_DeviceNotZeroPort_ReturnsTrue()
        {
            var device = new Device();
            device.Port = 1;

            Assert.IsTrue(device.ShouldSerializePort());
        }

        [Test]
        public void ShouldSerializeDevices_DeviceWithNullDevices_ReturnsFalse()
        {
            var device = new Device();

            Assert.IsFalse(device.ShouldSerializeDevices());
        }

        [Test]
        public void ShouldSerializeDevices_DeviceWithNotNullDevicesEmptyList_ReturnsFalse()
        {
            var device = new Device();
            device.Devices = new Devices();

            Assert.IsFalse(device.ShouldSerializeDevices());
        }

        [Test]
        public void ShouldSerializeDevices_DeviceNotNullDevicesNotEmpty_ReturnsTrue()
        {
            var device = new Device();
            device.Add(new List<Device>
            { 
                new Device()
            });

            Assert.IsTrue(device.ShouldSerializeDevices());
        }

        [Test]
        public void IsEmpty_NewDevice_ReturnsTrue()
        {
            var device = new Device();

            Assert.IsTrue(device.IsEmpty());
        }

        [Test]
        public void IsEmpty_SensorIsNotNull_ReturnsFalse()
        {
            var device = new Device();
            device.Sensor = new Sensor()
            {
                VendorId = 100,
            };

            Assert.IsFalse(device.IsEmpty());
        }

        [Test]
        public void IsEmpty_PortNotZero_ReturnsFalse()
        {
            var device = new Device();
            device.Port = 1;

            Assert.IsFalse(device.IsEmpty());
        }

        [Test]
        public void IsEmpty_ParametersNotNull_ReturnsFalse()
        {
            var device = new Device();
            device.Parameters = new Parameters()
            {
                Param = new List<Param>()
                {
                    new Param()
                }
            };

            Assert.IsFalse(device.IsEmpty());
        }

        [Test]
        public void IsEmpty_DevicesIsNotNull_ReturnsFalse()
        {
            var device = new Device();
            device.Devices = new Devices()
            {
                Device = new List<Device>
                {
                    new Device()
                }
            };

            Assert.IsFalse(device.IsEmpty());
        }

        [Test]
        public void Add_AddNewInstanseOfDevices_SuccessfullAdd()
        {
            var device = new Device();
            var devices = new List<Device>()
            {
                new Device(),
                new Device()
            };

            device.Add(devices);

            Assert.IsFalse(device.IsEmpty());
            Assert.AreEqual(devices.GetHashCode(), device.Devices.Device.GetHashCode());
        }
    }
}
