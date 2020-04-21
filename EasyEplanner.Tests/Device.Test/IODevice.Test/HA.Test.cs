using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class HATest
    {
        [TestCaseSource(nameof(GetSubTypeTestData))]
        public void GetDeviceSubTypeStrTest(string expectedType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedType, device.GetDeviceSubTypeStr(
                device.DeviceType, device.DeviceSubType));
        }

        [TestCaseSource(nameof(GetDevicePropertiesTestData))]
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
        public static object[] GetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { "HA", "", GetRandomHADevice() },
                new object[] { "HA", "Incorrect", GetRandomHADevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] GetDevicePropertiesTestData()
        {
            var exportForFS = new List<string>
            {
                "ST",
                "M",
            };

            return new object[]
            {
                new object[] {exportForFS, "", GetRandomHADevice()},
                new object[] {exportForFS, "HA", GetRandomHADevice()},
            };
        }

        /// <summary>
        /// Генератор HA устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomHADevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.HA("KOAG4HA1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.HA("LINE1HA2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.HA("TANK2HA1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.HA("CW_TANK3HA3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
