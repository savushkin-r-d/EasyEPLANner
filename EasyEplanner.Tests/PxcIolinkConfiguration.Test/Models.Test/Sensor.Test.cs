using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;

namespace Tests.PxcIolinkConfigration.Models
{
    public class SensorTest
    {
        [Test]
        public void IsEmpty_NewSensor_ReturnsTrue()
        {
            var sensor = new Sensor();

            Assert.IsTrue(sensor.IsEmpty());
        }

        [Test]
        public void IsEmpty_VendorIdNotZero_ReturnsFalse()
        {
            var sensor = new Sensor();
            sensor.VendorId = 1;

            Assert.IsFalse(sensor.IsEmpty());
        }

        [Test]
        public void IsEmpty_DeviceIdNotZero_ReturnsFalse()
        {
            var sensor = new Sensor();
            sensor.ProductId = "1";

            Assert.IsFalse(sensor.IsEmpty());
        }

        [Test]
        public void IsEmpty_ProductIdNotZero_ReturnsFalse()
        {
            var sensor = new Sensor();
            sensor.DeviceId = 1;

            Assert.IsFalse(sensor.IsEmpty());
        }

        [Test]
        public void Clone_FilledSensor_CorrectCloning()
        {
            var sensor = new Sensor();
            sensor.VendorId = 1;
            sensor.ProductId = "2";
            sensor.DeviceId = 3;

            var cloned = (Sensor)sensor.Clone();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(sensor.ProductId, cloned.ProductId);
                Assert.AreEqual(sensor.VendorId, cloned.VendorId);
                Assert.AreEqual(sensor.DeviceId, cloned.DeviceId);
            });
        }
    }
}
