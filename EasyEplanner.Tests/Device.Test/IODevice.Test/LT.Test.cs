﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class LTTest
    {
        /// <summary>
        /// Тест установки подтипа устройства
        /// </summary>
        /// <param name="expectedSubType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubTypeTest(Device.DeviceSubType expectedSubType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
        }

        /// <summary>
        /// Тест получения подтипа устройства
        /// </summary>
        /// <param name="expectedType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDeviceSubTypeStrTestData))]
        public void GetDeviceSubTypeStrTest(string expectedType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedType, device.GetDeviceSubTypeStr(
                device.DeviceType, device.DeviceSubType));
        }

        /// <summary>
        /// Тест свойств устройств в зависимости от подтипа
        /// </summary>
        /// <param name="expectedProperties">Ожидаемый список свойств</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDevicePropertiesTestData))]
        public void GetDevicePropertiesTest(List<string> expectedProperties,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedProperties, device.GetDeviceProperties(
                device.DeviceType, device.DeviceSubType));
        }

        /// <summary>
        /// Тестирование параметров устройства
        /// </summary>
        /// <param name="parametersSequence">Ожидаемые параметры</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
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
        /// Тестирование каналов устройства
        /// </summary>
        /// <param name="expectedChannelsCount">Ожидаемое количество каналов
        /// в словаре с названием каналов</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(ChannelsTestData))]
        public void ChannelsTest(Dictionary<string, int> expectedChannelsCount,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            int actualAI = device.Channels.Where(x => x.Name == "AI").Count();
            int actualAO = device.Channels.Where(x => x.Name == "AO").Count();
            int actualDI = device.Channels.Where(x => x.Name == "DI").Count();
            int actualDO = device.Channels.Where(x => x.Name == "DO").Count();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedChannelsCount["AI"], actualAI);
                Assert.AreEqual(expectedChannelsCount["AO"], actualAO);
                Assert.AreEqual(expectedChannelsCount["DI"], actualDI);
                Assert.AreEqual(expectedChannelsCount["DO"], actualDO);
            });
        }

        /// <summary>
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.LT, "LT",
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.LT_CYL, "LT_CYL",
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.LT_CONE, "LT_CONE", 
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.LT_TRUNC, "LT_TRUNC", 
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.LT_IOLINK, "LT_IOLINK",
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.LT_VIRT, "LT_VIRT",
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomLTDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomLTDevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetDeviceSubTypeStrTestData()
        {
            return new object[]
            {
                new object[] { "LT", "LT", GetRandomLTDevice() },
                new object[] { "LT_CYL", "LT_CYL", GetRandomLTDevice() },
                new object[] { "LT_CONE", "LT_CONE", GetRandomLTDevice() },
                new object[] { "LT_TRUNC", "LT_TRUNC", GetRandomLTDevice() },
                new object[] { "LT_IOLINK", "LT_IOLINK", GetRandomLTDevice() },
                new object[] { "LT_VIRT", "LT_VIRT", GetRandomLTDevice() },
                new object[] { "", "", GetRandomLTDevice() },
                new object[] { "", "Incorrect", GetRandomLTDevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetDevicePropertiesTestData()
        {
            var exportForLT = new List<string>
            {
                "M",
                "P_CZ",
                "V",
                "P_ERR"
            };

            var exportForLTIOLink = new List<string>
            {
                "M",
                "P_CZ",
                "V",
                "P_H_CONE",
                "P_MAX_P",
                "P_R",
                "CLEVEL",
                "P_ERR"
            };

            var exportForLTCyl = new List<string>
            {
                "M",
                "P_CZ",
                "V",
                "P_MAX_P",
                "P_R",
                "CLEVEL",
                "P_ERR"
            };

            var exportForLTCone = new List<string>
            {
                "M",
                "P_CZ",
                "V",
                "P_MAX_P",
                "P_R",
                "P_H_CONE",
                "CLEVEL",
                "P_ERR"
            };

            var exportForLTTrunc = new List<string>
            {
                "M",
                "P_CZ",
                "V",
                "P_MAX_P",
                "P_R",
                "P_H_TRUNC",
                "CLEVEL",
                "P_ERR"
            };

            var exportForLTVirt = new List<string>
            {
                "M",
                "V",
            };

            return new object[]
            {
                new object[] {exportForLT, "LT", GetRandomLTDevice()},
                new object[] {exportForLTIOLink, "LT_IOLINK", 
                    GetRandomLTDevice()},
                new object[] {exportForLTCyl, "LT_CYL", GetRandomLTDevice()},
                new object[] {exportForLTCone, "LT_CONE",
                    GetRandomLTDevice()},
                new object[] {exportForLTTrunc, "LT_TRUNC",
                    GetRandomLTDevice()},
                new object[] {exportForLTVirt, "LT_VIRT", GetRandomLTDevice()},
                new object[] {null, "Incorrect", GetRandomLTDevice()},
                new object[] {null, "", GetRandomLTDevice()},
            };
        }

        /// <summary>
        /// 1 - Параметры в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] ParametersTestData()
        {
            return new object[]
            {
                new object[]
                {
                    new string[] { "P_C0", "P_ERR" },
                    "LT",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_ERR", "P_MAX_P", "P_R" },
                    "LT_CYL",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_ERR", "P_MAX_P", "P_R", 
                        "P_H_CONE" },
                    "LT_CONE",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_ERR", "P_MAX_P", "P_R", 
                        "P_H_TRUNC" },
                    "LT_TRUNC",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_ERR", "P_MAX_P", "P_R",
                        "P_H_CONE" },
                    "LT_IOLINK",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    "LT_VIRT",
                    GetRandomLTDevice()
                },
            };
        }

        /// <summary>
        /// Данные для тестирования каналов устройств по подтипам.
        /// 1. Словарь с количеством каналов и их типами
        /// 2. Подтип устройства
        /// 3. Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] ChannelsTestData()
        {
            return new object[]
            {
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "LT",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "LT_CYL",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "LT_CONE",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "LT_TRUNC",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "LT_IOLINK",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "LT_VIRT",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "",
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "Incorrect",
                    GetRandomLTDevice()
                },
            };
        }

        /// <summary>
        /// Генератор LT устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomLTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.LT("KOAG4LT1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.LT("LINE1LT2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.LT("TANK2LT1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.LT("CW_TANK3LT3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
