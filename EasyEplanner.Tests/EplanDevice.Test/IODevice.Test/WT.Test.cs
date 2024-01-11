using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class WTTest
    {
        const string Incorrect = "Incorrect";
        const string WT = "WT";
        const string WT_VIRT = "WT_VIRT";
        const string WT_RS232 = "WT_RS232";
        const string WT_ETH = "WT_ETH";
        const string WT_AXL = "WT_AXL";

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
                new object[] { DeviceSubType.WT, WT, GetRandomWTDevice() },
                new object[] { DeviceSubType.WT_VIRT, WT_VIRT,
                    GetRandomWTDevice() },
                new object[] { DeviceSubType.WT, WT, GetRandomWTDevice() },
                new object[] { DeviceSubType.WT_RS232, WT_RS232,
                    GetRandomWTDevice() },
                new object[] { DeviceSubType.WT_ETH, WT_ETH,
                    GetRandomWTDevice()},
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomWTDevice() },
                new object[] { DeviceSubType.WT_AXL, WT_AXL,
                    GetRandomWTDevice() },
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
                new object[] { WT, string.Empty, GetRandomWTDevice() },
                new object[] { WT, WT, GetRandomWTDevice() },
                new object[] { WT_RS232, WT_RS232, GetRandomWTDevice() },
                new object[] { WT_ETH, WT_ETH, GetRandomWTDevice() },
                new object[] { string.Empty, Incorrect, GetRandomWTDevice() },
                new object[] { WT_VIRT, WT_VIRT, GetRandomWTDevice() },
                new object[] { WT_AXL, WT_AXL, GetRandomWTDevice() },
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
            var exportForWT = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_NOMINAL_W, 1},
                {IODevice.Parameter.P_DT, 1},
                {IODevice.Parameter.P_RKP, 1},
                {IODevice.Tag.P_CZ, 1},
            };

            var exportForWTVirt = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
            };

            var exportForWTRS232 = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_CZ, 1},
            };

            var exportForWTETH = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_CZ, 1},
            };

            var exportForWTAXL = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_DT, 1},
            };

            return new object[]
            {
                new object[] {exportForWT, string.Empty, GetRandomWTDevice() },
                new object[] {exportForWT, WT, GetRandomWTDevice() },
                new object[] {exportForWTVirt, WT_VIRT, GetRandomWTDevice() },
                new object[] {exportForWTETH, WT_ETH, GetRandomWTDevice() },
                new object[] {null, Incorrect, GetRandomWTDevice() },
                new object[] {exportForWTRS232, WT_RS232, GetRandomWTDevice() },
                new object[] { exportForWTAXL, WT_AXL, GetRandomWTDevice() },
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
            string[] parametersSequence, string subType, IODevice device)
        {
            device.SetSubType(subType);
            string[] actualParametersSequence = device.Parameters
                .Select(x => (string)x.Key)
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
            var parameters = new string[]
            {
                IODevice.Parameter.P_NOMINAL_W,
                IODevice.Parameter.P_RKP,
                IODevice.Parameter.P_C0,
                IODevice.Parameter.P_DT
            };

            var parametersWT_RS232_ETH = new string[]
            {
                IODevice.Parameter.P_C0,
            };

            var parametersWT_AXL = new string[]
            {
                IODevice.Parameter.P_DT,
            };

            return new object[]
            {
                new object[]
                {
                    parameters,
                    WT,
                    GetRandomWTDevice()
                },
                new object[]
                {
                    parameters,
                    string.Empty,
                    GetRandomWTDevice()
                },
                new object[]
                {
                    new string[0],
                    WT_VIRT,
                    GetRandomWTDevice(),
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomWTDevice(),
                },
                new object[]
                {
                    parametersWT_RS232_ETH,
                    WT_RS232,
                    GetRandomWTDevice(),
                },
                new object[]
                {
                    parametersWT_RS232_ETH,
                    WT_ETH,
                    GetRandomWTDevice(),
                },
                new object[]
                {
                    parametersWT_AXL,
                    WT_AXL,
                    GetRandomWTDevice(),
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
            var defaultChannels = new Dictionary<string, int>()
            {
                { AI, 2 },
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
            var RS232Channels = new Dictionary<string, int>()
            {
                { AI, 1 },
                { AO, 1 },
                { DI, 0 },
                { DO, 0 },
            };
            var AXLChanneks = new Dictionary<string, int>()
            {
                { AI, 1 },
                { AO, 0 },
                { DI, 0 },
                { DO, 0 },
            };

            return new object[]
            {
                new object[]
                {
                    defaultChannels,
                    WT,
                    GetRandomWTDevice()
                },
                new object[]
                {
                    defaultChannels,
                    string.Empty,
                    GetRandomWTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomWTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    WT_VIRT,
                    GetRandomWTDevice()
                },
                new object[]
                {
                    RS232Channels,
                    WT_RS232,
                    GetRandomWTDevice(),
                },
                new object[]
                {
                    emptyChannels,
                    WT_ETH,
                    GetRandomWTDevice(),
                },
                new object[]
                {
                    AXLChanneks,
                    WT_AXL,
                    GetRandomWTDevice(),
                },
            };
        }

        /// <summary>
        /// Генератор WT устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomWTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new WT("KOAG4WT1", "+KOAG4-WT1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new WT("LINE1WT2", "+LINE1-WT2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new WT("TANK2WT1", "+TANK2-WT1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new WT("CW_TANK3WT3", "+CW_TANK3-WT3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
