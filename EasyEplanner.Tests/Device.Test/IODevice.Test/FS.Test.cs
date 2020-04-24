using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class FSTest
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

        [TestCaseSource(nameof(ParametersTestData))]
        public void ParametersTest(string[] parametersSequence, string subType,
            Device.IODevice device)
        {
            device.SetSubType(subType);
            string[] actualParametersSequence = device.Parameters
                .Select(x => x.Key)
                .ToArray();
            Assert.AreEqual(parametersSequence, actualParametersSequence);
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
                new object[] { "FS", "", GetRandomFSDevice() },
                new object[] { "FS", "Incorrect", GetRandomFSDevice() },
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
                "P_DT"
            };

            return new object[]
            {
                new object[] {exportForFS, "", GetRandomFSDevice()},
                new object[] {exportForFS, "FS", GetRandomFSDevice()},
            };
        }

        /// <summary>
        /// 1 - Параметры в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        public static object[] ParametersTestData()
        {
            return new object[]
            {
                new object[]
                {
                    new string[] { "P_DT" },
                    "FS",
                    GetRandomFSDevice()
                },
                new object[]
                {
                    new string[] { "P_DT" },
                    "",
                    GetRandomFSDevice()
                },
            };
        }

        /// <summary>
        /// Генератор FS устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomFSDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.FS("KOAG4FS1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.FS("LINE1FS2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.FS("TANK2FS1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.FS("CW_TANK3FS3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
