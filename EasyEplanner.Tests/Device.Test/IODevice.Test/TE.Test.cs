using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    class TETest
    {
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubTypeTest(Device.DeviceSubType expectedSubType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
        }

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
        public static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.TE, "TE",
                    GetRandomTEDevice() },
                new object[] { Device.DeviceSubType.TE_IOLINK, "TE_IOLINK",
                    GetRandomTEDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomTEDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomTEDevice() },
            };
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
                new object[] { "", "", GetRandomTEDevice() },
                new object[] { "TE", "TE", GetRandomTEDevice() },
                new object[] { "TE_IOLINK", "TE_IOLINK", GetRandomTEDevice() },
                new object[] { "", "Incorrect", GetRandomTEDevice() },
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
            var exportForTE = new List<string>
            {
                "M",
                "P_CZ",
                "V",
                "ST"
            };

            return new object[]
            {
                new object[] {null, "", GetRandomTEDevice()},
                new object[] {exportForTE, "TE", GetRandomTEDevice()},
                new object[] {exportForTE, "TE_IOLINK", GetRandomTEDevice()},
                new object[] {null, "Incorrect", GetRandomTEDevice()},
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
                    new string[] { "P_C0", "P_ERR" }, 
                    "TE", 
                    GetRandomTEDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_ERR" },
                    "TE_IOLINK",
                    GetRandomTEDevice()
                },
            };
        }

        /// <summary>
        /// Генератор TE устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomTEDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.TE("KOAG4TE1", "Test device", 1,
                        "KOAG", 4, "Test article");
                case 2:
                    return new Device.TE("LINE1TE2", "Test device", 2,
                        "LINE", 1, "Test article");
                case 3:
                    return new Device.TE("TANK2TE1", "Test device", 1,
                        "TANK", 2, "Test article");
                default:
                    return new Device.TE("CW_TANK3TE3", "Test device", 3,
                        "CW_TANK", 3, "Test article");
            }
        }
    }
}
