using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class HLATest
    {
        /// <summary>
        /// Тест установки подтипа устройства
        /// </summary>
        /// <param name="expectedSubType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubType_NewDev_ReturnsExpectedSubType(
            Device.DeviceSubType expectedSubType, string subType,
            Device.IODevice device)
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
                new object[] { Device.DeviceSubType.HLA, "HLA",
                    GetRandomHLADevice() },
                new object[] { Device.DeviceSubType.HLA, "",
                    GetRandomHLADevice() },
                new object[] { Device.DeviceSubType.NONE,
                    "Incorrect", GetRandomHLADevice() },
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
            string expectedType, string subType, Device.IODevice device)
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
                new object[] { "HLA", "", GetRandomHLADevice() },
                new object[] { "HLA", "", GetRandomHLADevice() },
                new object[] { "", "Incorrect", GetRandomHLADevice() },
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
            Device.IODevice device)
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
                {"ST", 1},
                {"M", 1},
            };

            return new object[]
            {
                new object[] {exportForHLA, "", GetRandomHLADevice()},
                new object[] {exportForHLA, "HLA", GetRandomHLADevice()},
                new object[] {null, "Incorrect", GetRandomHLADevice()},
            };
        }

        /// <summary>
        /// Тестирование свойств устройства
        /// </summary>
        /// <param name="expectedProperties">Ожидаемые свойства</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(PropertiesTestData))]
        public void Properties_NewDev_ReturnsExpectedProperties(
            string[] expectedProperties, string subType,
            Device.IODevice device)
        {
            device.SetSubType(subType);
            string[] actualSequence = device.Properties
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
        private static object[] PropertiesTestData()
        {
            return new object[]
            {
                new object[]
                {
                    new string[] { "P_CONST_RED" },
                    "HLA",
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new string[] { "P_CONST_RED" },
                    "",
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new string[0],
                    "InCorrect",
                    GetRandomHLADevice()
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
            Device.IODevice device)
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
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 4 },
                    },
                    "HLA",
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 4 },
                    },
                    "",
                    GetRandomHLADevice()
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
                    GetRandomHLADevice()
                }
            };
        }

        /// <summary>
        /// Генератор HL устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomHLADevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.HLA("KOAG4HL1", "+KOAG4-HL1",
                        "Test device", 1, "KOAG", 4);
                case 2:
                    return new Device.HLA("LINE1HL2", "+LINE1-HL2",
                        "Test device", 2, "LINE", 1);
                case 3:
                    return new Device.HLA("TANK2HL1", "+TANK2-HL1",
                        "Test device", 1, "TANK", 2);
                default:
                    return new Device.HLA("CW_TANK3HL3", "+CW_TANK3-HL3",
                        "Test device", 3, "CW_TANK", 3);
            }
        }
    }
}
