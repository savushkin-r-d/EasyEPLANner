using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class DOTest
    {
        [TestCaseSource(nameof(SetSubTypeTestDevices))]
        public void SetSubTypeTest(Device.DeviceSubType expectedSubType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
        }

        [TestCaseSource(nameof(GetSubTypeTestDevices))]
        public void GetDeviceSubTypeStrTest(string expectedType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedType, device.GetDeviceSubTypeStr(
                device.DeviceType, device.DeviceSubType));
        }

        [TestCaseSource(nameof(GetDevicePropertiesTestDevices))]
        public void GetDevicePropertiesTest(List<string> expectedProperties,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedProperties, device.GetDeviceProperties(
                device.DeviceType, device.DeviceSubType));
        }

        /// <summary>
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] SetSubTypeTestDevices()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.DO, "",
                    GetRandomDODevice() },
                new object[] { Device.DeviceSubType.DO, "DO",
                    GetRandomDODevice() },
                new object[] { Device.DeviceSubType.DO_VIRT, "DO_VIRT",
                    GetRandomDODevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomDODevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] GetSubTypeTestDevices()
        {
            return new object[]
            {
                new object[] { "DO", "", GetRandomDODevice() },
                new object[] { "DO", "DO", GetRandomDODevice() },
                new object[] { "DO_VIRT", "DO_VIRT", GetRandomDODevice() },
                new object[] { "", "Incorrect", GetRandomDODevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] GetDevicePropertiesTestDevices()
        {
            var exportForDI = new List<string>
            {
                "ST", 
                "M"
            };

            return new object[]
            {
                new object[] {exportForDI, "", GetRandomDODevice()},
                new object[] {exportForDI, "DI", GetRandomDODevice()},
                new object[] {exportForDI, "DI_VIRT", GetRandomDODevice()},
                new object[] {exportForDI, "Incorrect", GetRandomDODevice()},
            };
        }

        /// <summary>
        /// Генератор DO устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomDODevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.DO("KOAG4DI1", "Test device", 1,
                        "KOAG", 4);
                case 2:
                    return new Device.DO("LINE1DI2", "Test device", 2,
                        "LINE", 1);
                case 3:
                    return new Device.DO("TANK2DI1", "Test device", 1,
                        "TANK", 2);
                default:
                    return new Device.DO("CW_TANK3DI3", "Test device", 3,
                        "CW_TANK", 3);
            }
        }
    }
}
