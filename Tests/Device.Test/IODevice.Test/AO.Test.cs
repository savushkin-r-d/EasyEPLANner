using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class AOTest
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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.AO, "", 
                    GetRandomAODevice() },
                new object[] { Device.DeviceSubType.AO, "AO", 
                    GetRandomAODevice() },
                new object[] { Device.DeviceSubType.AO_VIRT, "AO_VIRT", 
                    GetRandomAODevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect", 
                    GetRandomAODevice() },
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
                new object[] { "AO", "", GetRandomAODevice() },
                new object[] { "AO", "AO", GetRandomAODevice() },
                new object[] { "AO_VIRT", "AO_VIRT", GetRandomAODevice() },
                new object[] { "", "Incorrect", GetRandomAODevice() },
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
            var exportForAO = new List<string>
            {
                "M", 
                "V", 
                "P_MIN_V", 
                "P_MAX_V"
            };

            var exportForVirtAO = new List<string>
            {
                "M", 
                "V"
            };

            return new object[]
            {
                new object[] {exportForAO, "", GetRandomAODevice()},
                new object[] {exportForAO, "AO", GetRandomAODevice()},
                new object[] {exportForVirtAO, "AO_VIRT", GetRandomAODevice()},
                new object[] {null, "Incorrect", GetRandomAODevice()},
            };
        }

        /// <summary>
        /// 1 - Ожидаемое значение,
        /// 2 - Подтип устройства в виде строки,
        /// 3 - Значение параметрва меньшее,
        /// 4 - Значение параметра большее,
        /// 5 - Устройство для теста
        /// </summary>
        /// <returns></returns>
        public static object[] GetRangeTestData()
        {
            return new object[]
            {
                new object[] {$"_{2.0}..{4.0}", "", 2.0, 4.0,
                    GetRandomAODevice()},
                new object[] {$"_{1.0}..{3.0}", "AO", 1.0, 3.0,
                    GetRandomAODevice()},
                new object[] {$"", "AO_VIRT", 4.0, 8.0, GetRandomAODevice()},
                new object[] {$"", "Incorrect", 7.0, 9.0, GetRandomAODevice()},
            };
        }

        /// <summary>
        /// Генератор AO устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomAODevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.AO("KOAG4AO1", "Test device", 1,
                        "KOAG", 4);
                case 2:
                    return new Device.AO("LINE1AO2", "Test device", 2,
                        "LINE", 1);
                case 3:
                    return new Device.AO("TANK2AO1", "Test device", 1,
                        "TANK", 2);
                default:
                    return new Device.AO("CW_TANK3AO3", "Test device", 3,
                        "CW_TANK", 3);
            }
        }
    }
}
