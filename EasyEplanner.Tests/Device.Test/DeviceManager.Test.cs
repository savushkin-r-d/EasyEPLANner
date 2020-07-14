using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class DeviceManagerTest
    {
        [SetUp]
        public void SetUpDevices()
        {
            var firstValve = new Device.V("LINE1V2", "Test valve", 2, "LINE", 
                1, "Test V article");
            firstValve.SetSubType("V_AS_MIXPROOF");
            firstValve.SetParameter("R_AS_NUMBER", 1);
            Device.DeviceManager.GetInstance().Devices.Add(firstValve);

            var secondValve = new Device.V("TANK2V1", "Test valve", 1, "TANK", 
                2, "Test V article");
            secondValve.SetSubType("V_AS_MIXPROOF");
            secondValve.SetParameter("R_AS_NUMBER", 2);
            Device.DeviceManager.GetInstance().Devices.Add(secondValve);

            var pressureSensor = new Device.PT("KOAG3PT1", "Test PT", 1, 
                "KOAG", 3, "Test PT article");
            pressureSensor.SetSubType("PT_IOLINK");
            Device.DeviceManager.GetInstance().Devices.Add(pressureSensor);

            var temperatureSensor = new Device.TE("BATH4TE2", "Test TE", 2, 
                "BATH", 4, "Test TE article");
            temperatureSensor.SetSubType("TE");
            Device.DeviceManager.GetInstance().Devices.Add(temperatureSensor);
        }

        [TestCase("+LINE1-V2", false)]
        [TestCase("+LINE1-V2 +TANK2-V1", true)]
        [TestCase("+TANK2-V1 +KOAG3-PT1", null)]
        [TestCase("LINE1V2", false)]
        [TestCase("+LINE1-V2 TANK2V1", false)]
        [TestCase("TANK2V1 KOAG3PT1", false)]
        public void IsASInterfaseDeviceTest(string devices, bool? expected)
        {
            bool? actual = Device.DeviceManager.GetInstance()
                .IsASInterfaceDevices(devices, out _);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("+LINE-1V2", true)]
        [TestCase("LINE1V2", true)]
        [TestCase("+TANK99-TE99", true)]
        [TestCase("KOAG99PT99", true)]
        [TestCase("MCC1", false)]
        [TestCase("DI2", true)]
        public void CheckDeviceNameTest(string device, bool expected)
        {
            bool actual = Device.DeviceManager.CheckDeviceName(device, out _,
                out _, out _, out _, out _);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("+LINE1-V2", 0)]
        [TestCase("+TANK2-V1", 1)]
        [TestCase("+KOAG3-PT1", 2)]
        [TestCase("+BATH4-TE2", 3)]
        [TestCase("LINE1V2", 0)]
        [TestCase("TANK2V1", 1)]
        [TestCase("KOAG3PT1", 2)]
        [TestCase("BATH4TE2", 3)]
        [TestCase("BATH4TE99", -1)]
        [TestCase("", -1)]
        public void GetDeviceIndex(string device, int expectedNum)
        {
            int actualNum = Device.DeviceManager.GetInstance()
                .GetDeviceIndex(device);
            Assert.AreEqual(expectedNum, actualNum);
        }

        [TestCase("+LINE1-V2", "LINE1V2")]
        [TestCase("+TANK2-V1", "TANK2V1")]
        [TestCase("+KOAG3-PT1", "KOAG3PT1")]
        [TestCase("+BATH4-TE2", "BATH4TE2")]
        [TestCase("LINE1V2", "LINE1V2")]
        [TestCase("TANK2V1", "TANK2V1")]
        [TestCase("KOAG3PT1", "KOAG3PT1")]
        [TestCase("BATH4TE2", "BATH4TE2")]
        [TestCase("BATH4TE99", "заглушка")]
        [TestCase("", "")]
        public void GetDeviceTest(string devName, string expectedDevName)
        {
            var dev = Device.DeviceManager.GetInstance().GetDevice(devName);
            string actualDevName = dev.Name;
            if (expectedDevName == "заглушка")
            {
                Assert.AreEqual(expectedDevName, dev.Description);
            }
            else
            {
                Assert.AreEqual(expectedDevName, actualDevName);
            }
        }

        [TestCase("+LINE1-V2", "LINE1V2")]
        [TestCase("+TANK2-V1", "TANK2V1")]
        [TestCase("+KOAG3-PT1", "KOAG3PT1")]
        [TestCase("+BATH4-TE2", "BATH4TE2")]
        [TestCase("LINE1V2", "LINE1V2")]
        [TestCase("TANK2V1", "TANK2V1")]
        [TestCase("KOAG3PT1", "KOAG3PT1")]
        [TestCase("BATH4TE2", "BATH4TE2")]
        [TestCase("BATH4TE99", "заглушка")]
        [TestCase("", "")]
        public void GetDeviceByEplanNameTest(string devName, string expectedDevName)
        {
            var dev = Device.DeviceManager.GetInstance().GetDeviceByEplanName(
                devName);
            string actualDevName = dev.Name;
            if (expectedDevName == "заглушка")
            {
                Assert.AreEqual(expectedDevName, dev.Description);
            }
            else
            {
                Assert.AreEqual(expectedDevName, actualDevName);
            }
        }
    }
}
