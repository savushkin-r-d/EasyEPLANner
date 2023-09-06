using EplanDevice;
using NUnit.Framework;
using System;

namespace Tests.EplanDevices
{
    public class DeviceManagerTest
    {
        [SetUp]
        public void SetUpDevices()
        {
            //set manager instance null
            var instance = typeof(DeviceManager).GetField("instance",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static);
            instance.SetValue(null, null);

            var devManager = DeviceManager.GetInstance();

            var firstValve = new V("LINE1V2", "+LINE1-V2", "Test valve",
                2, "LINE", 1, "Test V article");
            firstValve.SetSubType("V_AS_MIXPROOF");
            firstValve.SetParameter("R_AS_NUMBER", 1);
            devManager.Devices.Add(firstValve);

            var secondValve = new V("TANK2V1", "+LINE2-V2", "Test valve",
                1, "TANK", 2, "Test V article");
            secondValve.SetSubType("V_AS_MIXPROOF");
            secondValve.SetParameter("R_AS_NUMBER", 2);
            devManager.Devices.Add(secondValve);

            var pressureSensor = new PT("KOAG3PT1", "+KOAG3-PT1",
                "Test PT", 1, "KOAG", 3, "Test PT article");
            pressureSensor.SetSubType("PT_IOLINK");
            devManager.Devices.Add(pressureSensor);

            var temperatureSensor = new TE("BATH4TE2", "+BATH4-TE2",
                "Test TE", 2, "BATH", 4, "Test TE article");
            temperatureSensor.SetSubType("TE");
            devManager.Devices.Add(temperatureSensor);

            var C_PID = new C("OBJ1C1", "+OBJ1-C1", string.Empty, 1, "OBJ", 1);
            C_PID.SetSubType("C_PID");
            devManager.Devices.Add(C_PID);

            var C_THLD = new C("OBJ1C2", "+OBJ1-C2", string.Empty, 2, "OBJ", 1);
            C_THLD.SetSubType("C_THLD");
            devManager.Devices.Add(C_THLD);
        }

        [TestCase("+LINE1-V2", false)]
        [TestCase("+LINE1-V2 +TANK2-V1", true)]
        [TestCase("+TANK2-V1 +KOAG3-PT1", null)]
        [TestCase("LINE1V2", false)]
        [TestCase("+LINE1-V2 TANK2V1", false)]
        [TestCase("TANK2V1 KOAG3PT1", false)]
        public void IsASInterfaseDeviceTest(string devices, bool? expected)
        {
            bool? actual = EplanDevice.DeviceManager.GetInstance()
                .IsASInterfaceDevices(devices, out _);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("+LINE-1V2", true)]
        [TestCase("LINE1V2", true)]
        [TestCase("+TANK99-TE99", true)]
        [TestCase("KOAG99PT99", true)]
        [TestCase("MKK1", false)]
        [TestCase("DI2", true)]
        [TestCase("FC2", true)]
        [TestCase("TC1", true)]
        [TestCase("TRC2", true)]
        [TestCase("TKK1", false)]
        public void CheckDeviceNameTest(string device, bool expected)
        {
            bool actual = EplanDevice.DeviceManager.GetInstance()
                .CheckDeviceName(device, out _, out _, out _, out _, out _,
                out _);
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
            int actualNum = EplanDevice.DeviceManager.GetInstance()
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
        [TestCase("BATH4TE99", StaticHelper.CommonConst.Cap)]
        [TestCase("", "")]
        public void GetDeviceTest(string devName, string expectedDevName)
        {
            var dev = EplanDevice.DeviceManager.GetInstance().GetDevice(devName);
            string actualDevName = dev.Name;
            if (expectedDevName == StaticHelper.CommonConst.Cap)
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
        [TestCase("BATH4TE99", StaticHelper.CommonConst.Cap)]
        [TestCase("", "")]
        public void GetDeviceByEplanNameTest(string devName, string expectedDevName)
        {
            var dev = EplanDevice.DeviceManager.GetInstance()
                .GetDeviceByEplanName(devName);
            string actualDevName = dev.Name;
            if (expectedDevName == StaticHelper.CommonConst.Cap)
            {
                Assert.AreEqual(expectedDevName, dev.Description);
            }
            else
            {
                Assert.AreEqual(expectedDevName, actualDevName);
            }
        }

        [Test]
        public void CheckControllerIOProperties_CorrectAllowedDevices()
        {
            var manager = DeviceManager.GetInstance();

            var pid = manager.GetDevice("OBJ1C1");
            var thld = manager.GetDevice("OBJ1C2");

            pid.Properties[IODevice.Property.IN_VALUE] = "KOAG3PT1"; // PID in: PT
            thld.Properties[IODevice.Property.IN_VALUE] = "KOAG3PT1"; // THLD in: PT
            pid.Properties[IODevice.Property.OUT_VALUE] = "OBJ1C2"; // PID out: C_THLD
            thld.Properties[IODevice.Property.OUT_VALUE] = "LINE1V2"; // THLD out: V

            var method = typeof(DeviceManager).GetMethod("CheckControllerIOProperties",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var res = method.Invoke(manager, new object[] { });
            Assert.AreEqual(string.Empty, res);
        }


        [Test]
        public void CheckControllerIOProperties_WrongAllowedDevice()
        {
            var manager = DeviceManager.GetInstance();

            var pid = manager.GetDevice("OBJ1C1");
            var thld = manager.GetDevice("OBJ1C2");

            pid.Properties[IODevice.Property.IN_VALUE] = "KOAG3PT10"; // PID in: WRONG_DEVICE
            thld.Properties[IODevice.Property.IN_VALUE] = "KOAG3PT10"; // THLD in: WRONG_DEVICE
            pid.Properties[IODevice.Property.OUT_VALUE] = "LINE1V2"; // PID out: WRONG TYPE
            thld.Properties[IODevice.Property.OUT_VALUE] = "OBJ1C1"; // THLD out: WRONG TYPE

            var method = typeof(DeviceManager).GetMethod("CheckControllerIOProperties",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var res = method.Invoke(manager, new object[] { });
            Assert.AreNotEqual(string.Empty, res);
        }

        [Test]
        public void CheckControllerIOProperties_EmptyAllowedDevice()
        {
            var manager = DeviceManager.GetInstance();

            var pid = manager.GetDevice("OBJ1C1");
            var thld = manager.GetDevice("OBJ1C2");

            pid.Properties[IODevice.Property.IN_VALUE] = "";
            thld.Properties[IODevice.Property.IN_VALUE] = null;
            pid.Properties[IODevice.Property.OUT_VALUE] = "";
            thld.Properties[IODevice.Property.OUT_VALUE] = null;

            var method = typeof(DeviceManager).GetMethod("CheckControllerIOProperties",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var res = method.Invoke(manager, new object[] { });
            Assert.AreEqual(string.Empty, res);
        }

    }
}
