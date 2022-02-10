using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class DEV_VTUGTest
    {
        const string Incorrect = "Incorrect";
        const string DEV_VTUG_8 = "DEV_VTUG_8";
        const string DEV_VTUG_16 = "DEV_VTUG_16";
        const string DEV_VTUG_24 = "DEV_VTUG_24";

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
                new object[] { DeviceSubType.DEV_VTUG_8, DEV_VTUG_8,
                    GetRandomDEV_VTUGDevice() },
                new object[] { DeviceSubType.DEV_VTUG_16, DEV_VTUG_16,
                    GetRandomDEV_VTUGDevice() },
                new object[] { DeviceSubType.DEV_VTUG_24, DEV_VTUG_24,
                    GetRandomDEV_VTUGDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomDEV_VTUGDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomDEV_VTUGDevice() },
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
            return new object[]
            {
                new object[]
                {
                    new string[0],
                    DEV_VTUG_8,
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new string[0],
                    DEV_VTUG_16,
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new string[0],
                    DEV_VTUG_24,
                    GetRandomDEV_VTUGDevice()
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
            return new object[]
            {
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    DEV_VTUG_8,
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    DEV_VTUG_16,
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    DEV_VTUG_24,
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    string.Empty,
                    GetRandomDEV_VTUGDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    Incorrect,
                    GetRandomDEV_VTUGDevice()
                },
            };
        }

        /// <summary>
        /// Генератор DEV_VTUG устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomDEV_VTUGDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new DEV_VTUG("KOAG4Y1", "+KOAG4-Y1",
                        "Test device", 1, "KOAG", 4, "Test article");
                case 2:
                    return new DEV_VTUG("LINE1Y2", "+LINE1-Y2",
                        "Test device", 2, "LINE", 1, "Test article");
                case 3:
                    return new DEV_VTUG("TANK2Y1", "+TANK2-Y1",
                        "Test device", 1, "TANK", 2, "Test article");
                default:
                    return new DEV_VTUG("CW_TANK3Y3", "+CW_TANK3-Y3",
                        "Test device", 3, "CW_TANK", 3, "Test article");
            }
        }
    }

    public class YTest
    {
        const string Incorrect = "Incorrect";
        const string DEV_VTUG_8 = "DEV_VTUG_8";
        const string DEV_VTUG_16 = "DEV_VTUG_16";
        const string DEV_VTUG_24 = "DEV_VTUG_24";

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
                new object[] { DeviceSubType.DEV_VTUG_8, DEV_VTUG_8,
                    GetRandomYDevice() },
                new object[] { DeviceSubType.DEV_VTUG_16, DEV_VTUG_16,
                    GetRandomYDevice() },
                new object[] { DeviceSubType.DEV_VTUG_24, DEV_VTUG_24,
                    GetRandomYDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomYDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomYDevice() },
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
            return new object[]
            {
                new object[]
                {
                    new string[0],
                    DEV_VTUG_8,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new string[0],
                    DEV_VTUG_16,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new string[0],
                    DEV_VTUG_24,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomYDevice()
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
            return new object[]
            {
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    DEV_VTUG_8,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    DEV_VTUG_16,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    DEV_VTUG_24,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    string.Empty,
                    GetRandomYDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    Incorrect,
                    GetRandomYDevice()
                },
            };
        }

        /// <summary>
        /// Генератор Y устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomYDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Y("KOAG4Y1", "+KOAG4-Y1",
                        "Test device", 1, "KOAG", 4, "Test article");
                case 2:
                    return new Y("LINE1Y2", "+LINE1-Y2",
                        "Test device", 2, "LINE", 1, "Test article");
                case 3:
                    return new Y("TANK2Y1", "+TANK2-Y1",
                        "Test device", 1, "TANK", 2, "Test article");
                default:
                    return new Y("CW_TANK3Y3", "+CW_TANK3-Y3",
                        "Test device", 3, "CW_TANK", 3, "Test article");
            }
        }
    }
}
