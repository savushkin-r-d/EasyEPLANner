﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class FQTTest
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
            device.SetParameter("P_MIN_F", value1);
            device.SetParameter("P_MAX_F", value2);
            Assert.AreEqual(expected, device.GetRange());
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
                new object[] { Device.DeviceSubType.FQT, "FQT",
                    GetRandomFQTDevice() },
                new object[] { Device.DeviceSubType.FQT_F, "FQT_F",
                    GetRandomFQTDevice() },
                new object[] { Device.DeviceSubType.FQT_F_OK, "FQT_F_OK",
                    GetRandomFQTDevice() },
                new object[] { Device.DeviceSubType.FQT_VIRT, "FQT_VIRT",
                    GetRandomFQTDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomFQTDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomFQTDevice() },
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
                new object[] { "", "", GetRandomFQTDevice() },
                new object[] { "FQT", "FQT", GetRandomFQTDevice() },
                new object[] { "FQT_F", "FQT_F", GetRandomFQTDevice() },
                new object[] { "FQT_F_OK", "FQT_F_OK", GetRandomFQTDevice() },
                new object[] { "FQT_VIRT", "FQT_VIRT", GetRandomFQTDevice() },
                new object[] { "", "Incorrect", GetRandomFQTDevice() },
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
            var exportForFQT = new List<string>
            {
                "ST", 
                "M", 
                "V", 
                "ABS_V"
            };

            var exportForFQTF = new List<string>
            {
                "ST", 
                "M", 
                "V", 
                "P_MIN_FLOW", 
                "P_MAX_FLOW", 
                "P_CZ", 
                "F", 
                "P_DT", 
                "ABS_V"
            };

            var exportForFQTFOK = new List<string>
            {
                "ST",
                "M",
                "V",
                "P_MIN_FLOW",
                "P_MAX_FLOW",
                "P_CZ",
                "F",
                "P_DT",
                "ABS_V",
                "OK"
            };

            var exportForFQTVirt = new List<string>
            {
                "ST",
                "M",
                "V",
                "P_MIN_FLOW",
                "P_MAX_FLOW",
                "P_CZ",
                "F",
                "P_DT",
                "ABS_V",
            };

            return new object[]
            {
                new object[] {exportForFQT, "FQT", GetRandomFQTDevice()},
                new object[] {exportForFQTF, "FQT_F", GetRandomFQTDevice()},
                new object[] {exportForFQTFOK, "FQT_F_OK", 
                    GetRandomFQTDevice()},
                new object[] {exportForFQTVirt, "FQT_VIRT", 
                    GetRandomFQTDevice()},
                new object[] {null, "Incorrect", GetRandomFQTDevice()},
                new object[] {null, "", GetRandomFQTDevice()},
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
                new object[] {$"_{2.0}..{4.0}", "FQT_F", 2.0, 4.0,
                    GetRandomFQTDevice()},
                new object[] {$"_{1.0}..{3.0}", "FQT_F_OK", 1.0, 3.0,
                    GetRandomFQTDevice()},
                new object[] {$"", "FQT", 4.0, 8.0, GetRandomFQTDevice()},
                new object[] {$"", "FQT_VIRT", 7.0, 9.0, GetRandomFQTDevice()},
                new object[] {$"", "", 4.0, 8.0, GetRandomFQTDevice()},
                new object[] {$"", "Incorrect", 7.0, 9.0, GetRandomFQTDevice()},
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
                    new string[0],
                    "FQT",
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[] { "P_MIN_F", "P_MAX_F", "P_C0", "P_DT" },
                    "FQT_F",
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[] { "P_MIN_F", "P_MAX_F", "P_C0", "P_DT" },
                    "FQT_F_OK",
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    "FQT_VIRT",
                    GetRandomFQTDevice()
                },
            };
        }
        /// <summary>
        /// Генератор FQT устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomFQTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.FQT("KOAG4FQT1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.FQT("LINE1FQT2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.FQT("TANK2FQT1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.FQT("CW_TANK3FQT3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
