using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class QTTest
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

        [TestCaseSource(nameof(GetRangeTestData))]
        public void GetRangeTest(string expected, string subType,
            double value1, double value2, Device.IODevice device)
        {
            device.SetSubType(subType);
            device.SetParameter("P_MIN_V", value1);
            device.SetParameter("P_MAX_V", value2);
            Assert.AreEqual(expected, device.GetRange());
        }

        /// <summary>
        /// 1 - Ожидаемое перечисление подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT, "QT",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT_IOLINK, "QT_IOLINK",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT_OK, "QT_OK",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomQTDevice() },
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
                new object[] { "", "", GetRandomQTDevice() },
                new object[] { "QT", "QT", GetRandomQTDevice() },
                new object[] { "QT_IOLINK", "QT_IOLINK", GetRandomQTDevice() },
                new object[] { "QT_OK", "QT_OK", GetRandomQTDevice() },
                new object[] { "", "Incorrect", GetRandomQTDevice() },
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
            var exportForQT = new List<string>
            {
                "ST",
                "M",
                "V",
                "P_MIN_V",
                "P_MAX_V",
                "P_CZ"
            };

            var exportForQTIOLink = new List<string>
            {
                "ST",             
                "M",
                "V",
                "P_CZ",
                "T",
            };

            var exportForQTOk = new List<string>
            {
                "ST",
                "M",
                "V",
                "OK",
                "P_MIN_V",
                "P_MAX_V",
                "P_CZ"
            };

            return new object[]
            {
                new object[] {exportForQT, "QT", GetRandomQTDevice()},
                new object[] {exportForQTIOLink, "QT_IOLINK",
                    GetRandomQTDevice()},
                new object[] {exportForQTOk, "QT_OK",
                    GetRandomQTDevice()},
                new object[] {null, "", GetRandomQTDevice()},
                new object[] {null, "Incorrect", GetRandomQTDevice()},
            };
        }

        /// <summary>
        /// 1 - Ожидаемое значение,
        /// 2 - Подтип устройства в виде строки,
        /// 3 - Значение параметра меньшее,
        /// 4 - Значение параметра большее,
        /// 5 - Устройство для теста
        /// </summary>
        /// <returns></returns>
        public static object[] GetRangeTestData()
        {
            return new object[]
            {
                new object[] {$"_{2.0}..{4.0}", "QT", 2.0, 4.0,
                    GetRandomQTDevice()},
                new object[] {$"", "QT_IOLINK", 1.0, 3.0,
                    GetRandomQTDevice()},
                new object[] {$"_{1.0}..{3.0}", "QT_OK", 1.0, 3.0,
                    GetRandomQTDevice()},
                new object[] {$"", "", 4.0, 8.0, GetRandomQTDevice()},
                new object[] {$"", "Incorrect", 7.0, 9.0, GetRandomQTDevice()},
            };
        }

        /// <summary>
        /// Генератор QT устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomQTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.QT("KOAG4QT1", "Test device", 1,
                        "KOAG", 4, "Test article");
                case 2:
                    return new Device.QT("LINE1QT2", "Test device", 2,
                        "LINE", 1, "Test article");
                case 3:
                    return new Device.QT("TANK2QT1", "Test device", 1,
                        "TANK", 2, "Test article");
                default:
                    return new Device.QT("CW_TANK3QT3", "Test device", 3,
                        "CW_TANK", 3, "Test article");
            }
        }
    }
}
