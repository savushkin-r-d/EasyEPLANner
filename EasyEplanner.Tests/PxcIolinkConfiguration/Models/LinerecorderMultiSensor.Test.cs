using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests.PxcIolinkConfigration.Models
{
    public class LinerecorderMultiSensorTest
    {
        [Test]
        public void Add_NewSensor_CorrectAdd()
        {
            var sensor = new LinerecorderMultiSensor();
            Device device = new Device()
            {
                Sensor = new Sensor(),
                Devices = new Devices(),
                Parameters = new Parameters(),
                Port = 1,
            };

            sensor.Add(device);

            int expectedDevicesCount = 1;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedDevicesCount, sensor.Devices.Device.Count);
                Assert.AreEqual(device.GetHashCode(), sensor.Devices.Device.First().GetHashCode());
            });
        }

        [Test]
        public void IsEmpty_NewSensor_ReturnsTrue()
        {
            var sensor = new LinerecorderMultiSensor();

            Assert.IsTrue(sensor.IsEmpty());
        }

        [Test]
        public void IsEmpty_NewSensorNotEmptyDevices_ReturnsFalse()
        {
            var sensor = new LinerecorderMultiSensor();
            sensor.Devices = new Devices()
            {
                Device = new List<Device>
                {
                    new Device(){ }
                }
            };

            Assert.IsFalse(sensor.IsEmpty());
        }
    }
}
