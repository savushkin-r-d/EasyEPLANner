using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class GTest
    {
        const string Incorrect = "Incorrect";
        const string G_IOL_4 = "G_IOL_4";
        const string G_IOL_8 = "G_IOL_8";

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
        /// 1 - Ожидаемое перечисление подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomGDevice() },
                new object[] { DeviceSubType.G_IOL_4, G_IOL_4,
                    GetRandomGDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomGDevice() },
                new object[] { DeviceSubType.G_IOL_8, G_IOL_8,
                    GetRandomGDevice() },
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
                new object[] { string.Empty, string.Empty, GetRandomGDevice() },
                new object[] { G_IOL_4, G_IOL_4, GetRandomGDevice() },
                new object[] { G_IOL_8, G_IOL_8, GetRandomGDevice() },
                new object[] { string.Empty, Incorrect, GetRandomGDevice() },
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
            Dictionary<ITag, int> expectedProperties, string subType,
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
            var exportForG_IOL_4 = new Dictionary<ITag, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.ERR, 1},
                {IODevice.Tag.ST_CH, 4},
                {IODevice.Tag.NOMINAL_CURRENT_CH, 4},
                {IODevice.Tag.LOAD_CURRENT_CH, 4},
                {IODevice.Tag.SUM_CURRENTS, 1},
                {IODevice.Tag.VOLTAGE, 1},
                {IODevice.Tag.OUT_POWER_90, 1},
            };

            var exportForG_IOL_8 = new Dictionary<ITag, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.ERR, 1},
                {IODevice.Tag.ST_CH, 8},
                {IODevice.Tag.NOMINAL_CURRENT_CH, 8},
                {IODevice.Tag.LOAD_CURRENT_CH, 8},
                {IODevice.Tag.SUM_CURRENTS, 1},
                {IODevice.Tag.VOLTAGE, 1},
                {IODevice.Tag.OUT_POWER_90, 1},
            };

            return new object[]
            {
                new object[] {null, string.Empty, GetRandomGDevice()},
                new object[] {exportForG_IOL_4, G_IOL_4, GetRandomGDevice()},
                new object[] {null, Incorrect, GetRandomGDevice()},
                new object[] {exportForG_IOL_8, G_IOL_8, GetRandomGDevice()},
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
            return new object[]
            {
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomGDevice()
                },
                new object[]
                {
                    new string[0],
                    G_IOL_4,
                    GetRandomGDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomGDevice()
                },
                new object[]
                {
                    new string[0],
                    G_IOL_8,
                    GetRandomGDevice()
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
            var iolinkDeviceChannels = new Dictionary<string, int>()
            {
                { AI, 1 },
                { AO, 1 },
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
                    emptyChannels,
                    string.Empty,
                    GetRandomGDevice()
                },
                new object[]
                {
                    iolinkDeviceChannels,
                    G_IOL_4,
                    GetRandomGDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomGDevice()
                },
                new object[]
                {
                    iolinkDeviceChannels,
                    G_IOL_8,
                    GetRandomGDevice()
                },
            };
        }

        /// <summary>
        /// Генератор AI устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomGDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new G("KOAG4G1", "+KOAG4-G1",
                        "Test device", 1, "KOAG", 4, string.Empty);
                case 2:
                    return new G("LINE1G2", "+LINE1-G2",
                        "Test device", 2, "LINE", 1, string.Empty);
                case 3:
                    return new G("TANK2G1", "+TANK2-G1",
                        "Test device", 1, "TANK", 2, string.Empty);
                default:
                    return new G("CW_TANK3G3", "+CW_TANK3-G3",
                        "Test device", 3, "CW_TANK", 3, string.Empty);
            }
        }
    }
}
