using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    class SBTest
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
                new object[] { "SB", "", GetRandomSBDevice() },
                new object[] { "SB", "Incorrect", GetRandomSBDevice() },
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
            var exportForSB = new List<string>
                {
                    "ST",
                    "M",
                    "P_DT"
                };

            return new object[]
            {
                new object[] {exportForSB, "", GetRandomSBDevice()},
                new object[] {exportForSB, "SB", GetRandomSBDevice()},
            };
        }

        /// <summary>
        /// Генератор SB устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomSBDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.SB("KOAG4SB1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.SB("LINE1SB2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.SB("TANK2SB1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.SB("CW_TANK3SB3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
