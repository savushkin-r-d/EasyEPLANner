﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Device;

namespace Tests.Devices
{
    public class LSTest
    {
        const string Incorrect = "Incorrect";
        const string LS_MIN = "LS_MIN";
        const string LS_MAX = "LS_MAX";
        const string LS_IOLINK_MIN = "LS_IOLINK_MIN";
        const string LS_IOLINK_MAX = "LS_IOLINK_MAX";
        const string LS_VIRT = "LS_VIRT";

        const string AI = IODevice.IOChannel.AI;
        const string AO = IODevice.IOChannel.AO;
        const string DI = IODevice.IOChannel.DI;
        const string DO = IODevice.IOChannel.DO;

        /// <summary>
        /// Тест установки подтипа устройства
        /// </summary>
        /// <param name="expectedSubType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubType_NewDev_ReturnsExpectedSubType(
            DeviceSubType expectedSubType, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
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
                new object[] { DeviceSubType.LS_MIN, LS_MIN,
                    GetRandomLSDevice() },
                new object[] { DeviceSubType.LS_MAX, LS_MAX,
                    GetRandomLSDevice() },
                new object[] { DeviceSubType.LS_IOLINK_MIN,
                    LS_IOLINK_MIN, GetRandomLSDevice() },
                new object[] { DeviceSubType.LS_IOLINK_MAX,
                    LS_IOLINK_MAX, GetRandomLSDevice() },
                new object[] { DeviceSubType.LS_VIRT, LS_VIRT,
                    GetRandomLSDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomLSDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomLSDevice() },
            };
        }

        /// <summary>
        /// Тест получения подтипа устройства
        /// </summary>
        /// <param name="expectedType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDeviceSubTypeStrTestData))]
        public void GetDeviceSubTypeStr_NewDev_ReturnsExpectedTypeStr(
            string expectedType, string subType, IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedType, device.GetDeviceSubTypeStr(
                device.DeviceType, device.DeviceSubType));
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
                new object[] { LS_MIN, LS_MIN, GetRandomLSDevice() },
                new object[] { LS_MAX, LS_MAX, GetRandomLSDevice() },
                new object[] { LS_IOLINK_MIN, LS_IOLINK_MIN,
                    GetRandomLSDevice() },
                new object[] { LS_IOLINK_MAX, LS_IOLINK_MAX,
                    GetRandomLSDevice() },
                new object[] { LS_VIRT, LS_VIRT, GetRandomLSDevice() },
                new object[] { string.Empty, string.Empty, GetRandomLSDevice() },
                new object[] { string.Empty, Incorrect, GetRandomLSDevice() },
            };
        }

        /// <summary>
        /// Тест свойств устройств в зависимости от подтипа
        /// </summary>
        /// <param name="expectedProperties">Ожидаемый список свойств</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDevicePropertiesTestData))]
        public void GetDeviceProperties_NewDev_ReturnsExpectedDictOfProperties(
            Dictionary<string, int> expectedProperties, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedProperties, device.GetDeviceProperties(
                device.DeviceType, device.DeviceSubType));
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetDevicePropertiesTestData()
        {
            var exportForIOLinkLS = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_DT, 1},
                {IODevice.Parameter.P_ERR, 1},
            };

            var exportForLS = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Parameter.P_DT, 1},
            };

            return new object[]
            {
                new object[] {exportForLS, LS_MIN, GetRandomLSDevice()},
                new object[] {exportForLS, LS_MAX, GetRandomLSDevice()},
                new object[] {exportForLS, LS_VIRT, GetRandomLSDevice()},
                new object[] {exportForIOLinkLS, LS_IOLINK_MIN,
                    GetRandomLSDevice()},
                new object[] {exportForIOLinkLS, LS_IOLINK_MAX,
                    GetRandomLSDevice()},
                new object[] {null, Incorrect, GetRandomLSDevice()},
                new object[] {null, string.Empty, GetRandomLSDevice()},
            };
        }

        /// <summary>
        /// Тестирование получения типа подключения датчика
        /// </summary>
        /// <param name="expected">Ожидаемый тип подключения</param>
        /// <param name="subType">Подтип устройства</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetConnectionTestData))]
        public void GetConnectionType_NewDev_ReturnsExpectedString(
            string expected, string subType, IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expected, device.GetConnectionType());
        }

        /// <summary>
        /// 1 - Ожидаемое значение,
        /// 2 - Подтип устройства в виде строки,
        /// 3 - Устройство для теста
        /// </summary>
        /// <returns></returns>
        private static object[] GetConnectionTestData()
        {
            var min = "_Min";
            var max = "_Max";

            return new object[]
            {
                new object[] {min, LS_MIN, GetRandomLSDevice()},
                new object[] {min, LS_IOLINK_MIN, GetRandomLSDevice()},
                new object[] {max, LS_MAX, GetRandomLSDevice()},
                new object[] {max, LS_IOLINK_MAX, GetRandomLSDevice()},
                new object[] { string.Empty, string.Empty, GetRandomLSDevice()},
                new object[] { string.Empty, Incorrect, GetRandomLSDevice()},
                new object[] { string.Empty, LS_VIRT, GetRandomLSDevice()},
            };
        }

        /// <summary>
        /// Тестирование параметров устройства
        /// </summary>
        /// <param name="parametersSequence">Ожидаемые параметры</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(ParametersTestData))]
        public void Parameters_NewDev_ReturnsExpectedArrayWithParameters(
            string[] parametersSequence, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            string[] actualParametersSequence = device.Parameters
                .Select(x => x.Key)
                .ToArray();
            Assert.AreEqual(parametersSequence, actualParametersSequence);
        }

        /// <summary>
        /// 1 - Параметры в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] ParametersTestData()
        {
            var defaultParameters = new string[]
            {
                IODevice.Parameter.P_DT
            };

            var iolinkParameters = new string[]
            {
                IODevice.Parameter.P_DT,
                IODevice.Parameter.P_ERR,
            };

            return new object[]
            {
                new object[]
                {
                    defaultParameters,
                    LS_MIN,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    defaultParameters,
                    LS_MAX,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    new string[0],
                    LS_VIRT,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    iolinkParameters,
                    LS_IOLINK_MIN,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    iolinkParameters,
                    LS_IOLINK_MAX,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomLSDevice()
                },
            };
        }

        /// <summary>
        /// Тестирование каналов устройства
        /// </summary>
        /// <param name="expectedChannelsCount">Ожидаемое количество каналов
        /// в словаре с названием каналов</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(ChannelsTestData))]
        public void Channels_NewDev_ReturnsExpectedCount(
            Dictionary<string, int> expectedChannelsCount, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            int actualAI = device.Channels.Where(x => x.Name == AI).Count();
            int actualAO = device.Channels.Where(x => x.Name == AO).Count();
            int actualDI = device.Channels.Where(x => x.Name == DI).Count();
            int actualDO = device.Channels.Where(x => x.Name == DO).Count();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedChannelsCount[AI], actualAI);
                Assert.AreEqual(expectedChannelsCount[AO], actualAO);
                Assert.AreEqual(expectedChannelsCount[DI], actualDI);
                Assert.AreEqual(expectedChannelsCount[DO], actualDO);
            });
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
            var discreteSensorChannels = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 1 },
                { DO, 0 },
            };

            var iolinkSensorChannels = new Dictionary<string, int>()
            {
                { AI, 1 },
                { AO, 0 },
                { DI, 0 },
                { DO, 0 },
            };

            var emptyChannels = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 0 },
                { DO, 0 },
            };

            return new object[]
            {
                new object[]
                {
                    discreteSensorChannels,
                    LS_MIN,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    discreteSensorChannels,
                    LS_MAX,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    iolinkSensorChannels,
                    LS_IOLINK_MIN,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    iolinkSensorChannels,
                    LS_IOLINK_MAX,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    emptyChannels,
                    LS_VIRT,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    emptyChannels,
                    string.Empty,
                    GetRandomLSDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomLSDevice()
                },
            };
        }

        /// <summary>
        /// Генератор LS устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomLSDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new LS("KOAG4LS1", "+KOAG4-LS1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new LS("LINE1LS2", "+LINE1-LS2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new LS("TANK2LS1", "+TANK2-LS1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new LS("CW_TANK3LS3", "+CW_TANK3-LS3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
