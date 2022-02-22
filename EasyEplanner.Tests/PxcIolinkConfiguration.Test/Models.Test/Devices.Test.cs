using EasyEPlanner.PxcIolinkConfiguration.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.PxcIolinkConfigration.Models
{
    public class DevicesTest
    {
        [Test]
        public void IsEmpty_NewDevices_ReturnsTrue()
        {
            var devices = new Devices();

            Assert.IsTrue(devices.IsEmpty());
        }

        [Test]
        public void IsEmpty_DeviceNotEmpty_ReturnsFalse()
        {
            var devices = new Devices()
            {
                Device = new List<Device>
                {
                    new Device()
                }
            };

            Assert.IsFalse(devices.IsEmpty());
        }
    }
}
