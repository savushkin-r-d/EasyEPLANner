using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class WTTest
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
                new object[] { "WT", "", GetRandomWTDevice() },
                new object[] { "WT", "Incorrect", GetRandomWTDevice() },
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
            var exportForWT = new List<string>
            {
                "ST",
                "M",
                "V",
                "P_NOMINAL_W",
                "P_DT",
                "P_RKP",
                "P_CZ"
            };

            return new object[]
            {
                new object[] {exportForWT, "", GetRandomWTDevice()},
                new object[] {exportForWT, "WT", GetRandomWTDevice()},
            };
        }

        /// <summary>
        /// Генератор WT устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomWTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.WT("KOAG4WT1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.WT("LINE1WT2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.WT("TANK2WT1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.WT("CW_TANK3WT3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
