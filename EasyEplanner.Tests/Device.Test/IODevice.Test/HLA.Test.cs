using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Device;

namespace Tests.Devices
{
    public class HLATest
    {
        const string Incorrect = "Incorrect";
        const string HLA = "HLA";
        const string HLA_VIRT = "HLA_VIRT";

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
                new object[] { DeviceSubType.HLA, HLA,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.HLA_VIRT, HLA_VIRT,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.HLA, string.Empty,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.NONE,
                    Incorrect, GetRandomHLADevice() },
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
                new object[] { HLA, string.Empty, GetRandomHLADevice() },
                new object[] { HLA, HLA, GetRandomHLADevice() },
                new object[] { HLA_VIRT, HLA_VIRT, GetRandomHLADevice() },
                new object[] { string.Empty, Incorrect, GetRandomHLADevice() },
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
            var exportForHLA = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.L_RED, 1 },
                {IODevice.Tag.L_YELLOW, 1 },
                {IODevice.Tag.L_GREEN, 1 },
                {IODevice.Tag.L_SIREN, 1 }
            };

            return new object[]
            {
                new object[] {exportForHLA, string.Empty, GetRandomHLADevice()},
                new object[] {exportForHLA, HLA, GetRandomHLADevice()},
                new object[] {null, Incorrect, GetRandomHLADevice()},
                new object[] {null, HLA_VIRT, GetRandomHLADevice()},
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
            var discreteHLAChannels = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 0 },
                { DO, 4 },
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
                    discreteHLAChannels,
                    HLA,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    discreteHLAChannels,
                    string.Empty,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    emptyChannels,
                    HLA_VIRT,
                    GetRandomHLADevice()
                }
            };
        }

        /// <summary>
        /// Тестирование свойств устройства
        /// </summary>
        /// <param name="expectedProperties">Ожидаемые свойства</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(RuntimeParametersTestData))]
        public void RuntimeParameters_NewDev_ReturnsExpectedProperties(
            string[] expectedProperties, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            string[] actualSequence = device.RuntimeParameters
                .Select(x => x.Key)
                .ToArray();
            Assert.AreEqual(expectedProperties, actualSequence);
        }

        /// <summary>
        /// 1 - Параметры в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] RuntimeParametersTestData()
        {
            var defaultParameters = new string[]
            {
               IODevice.RuntimeParameter.R_CONST_RED
            };

            return new object[]
            {
                new object[]
                {
                    defaultParameters,
                    HLA,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    defaultParameters,
                    string.Empty,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new string[0],
                    HLA_VIRT,
                    GetRandomHLADevice()
                },
            };
        }

        /// <summary>
        /// Генератор HL устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomHLADevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new HLA("KOAG4HL1", "+KOAG4-HL1",
                        "Test device", 1, "KOAG", 4);
                case 2:
                    return new HLA("LINE1HL2", "+LINE1-HL2",
                        "Test device", 2, "LINE", 1);
                case 3:
                    return new HLA("TANK2HL1", "+TANK2-HL1",
                        "Test device", 1, "TANK", 2);
                default:
                    return new HLA("CW_TANK3HL3", "+CW_TANK3-HL3",
                        "Test device", 3, "CW_TANK", 3);
            }
        }
    }
}
