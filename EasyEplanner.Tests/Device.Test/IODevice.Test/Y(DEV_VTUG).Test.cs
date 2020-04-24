﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class DEV_VTUG
    {
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubTypeTest(Device.DeviceSubType expectedSubType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
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
                new object[] { Device.DeviceSubType.DEV_VTUG_8, "DEV_VTUG_8",
                    GetRandomDEV_VTUGDevice() },
                new object[] { Device.DeviceSubType.DEV_VTUG_16, "DEV_VTUG_16",
                    GetRandomDEV_VTUGDevice() },
                new object[] { Device.DeviceSubType.DEV_VTUG_24, "DEV_VTUG_24",
                    GetRandomDEV_VTUGDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomDEV_VTUGDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomDEV_VTUGDevice() },
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
                    "DEV_VTUG_8",
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new string[0],
                    "DEV_VTUG_16",
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new string[0],
                    "DEV_VTUG_24",
                    GetRandomDEV_VTUGDevice()
                },
            };
        }

        /// <summary>
        /// Генератор DEV_VTUG устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomDEV_VTUGDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.DEV_VTUG("KOAG4Y1", "Test device", 1,
                        "KOAG", 4, "Test article");
                case 2:
                    return new Device.DEV_VTUG("LINE1Y2", "Test device", 2,
                        "LINE", 1, "Test article");
                case 3:
                    return new Device.DEV_VTUG("TANK2Y1", "Test device", 1,
                        "TANK", 2, "Test article");
                default:
                    return new Device.DEV_VTUG("CW_TANK3Y3", "Test device", 3,
                        "CW_TANK", 3, "Test article");
            }
        }
    }

    public class Y
    {
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubTypeTest(Device.DeviceSubType expectedSubType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
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
                new object[] { Device.DeviceSubType.DEV_VTUG_8, "DEV_VTUG_8",
                    GetRandomYDevice() },
                new object[] { Device.DeviceSubType.DEV_VTUG_16, "DEV_VTUG_16",
                    GetRandomYDevice() },
                new object[] { Device.DeviceSubType.DEV_VTUG_24, "DEV_VTUG_24",
                    GetRandomYDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomYDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomYDevice() },
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
                    "DEV_VTUG_8",
                    GetRandomYDevice()
                },
                new object[]
                {
                    new string[0],
                    "DEV_VTUG_16",
                    GetRandomYDevice()
                },
                new object[]
                {
                    new string[0],
                    "DEV_VTUG_24",
                    GetRandomYDevice()
                },
            };
        }

        /// <summary>
        /// Генератор Y устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomYDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.Y("KOAG4Y1", "Test device", 1,
                        "KOAG", 4, "Test article");
                case 2:
                    return new Device.Y("LINE1Y2", "Test device", 2,
                        "LINE", 1, "Test article");
                case 3:
                    return new Device.Y("TANK2Y1", "Test device", 1,
                        "TANK", 2, "Test article");
                default:
                    return new Device.Y("CW_TANK3Y3", "Test device", 3,
                        "CW_TANK", 3, "Test article");
            }
        }
    }
}
