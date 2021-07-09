using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Devices
{
    class SBTest
    {
        const string Incorrect = "Incorrect";
        const string SB = "SB";
        const string SB_VIRT = "SB_VIRT";

        const string AI = Device.IODevice.IOChannel.AI;
        const string AO = Device.IODevice.IOChannel.AO;
        const string DI = Device.IODevice.IOChannel.DI;
        const string DO = Device.IODevice.IOChannel.DO;

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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.SB, SB,
                    GetRandomSBDevice() },
                new object[] { Device.DeviceSubType.SB_VIRT, SB_VIRT,
                    GetRandomSBDevice() },
                new object[] { Device.DeviceSubType.SB, string.Empty,
                    GetRandomSBDevice() },
                new object[] { Device.DeviceSubType.NONE, Incorrect,
                    GetRandomSBDevice() },

            };
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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetDeviceSubTypeStrTestData()
        {
            return new object[]
            {
                new object[] { SB, string.Empty, GetRandomSBDevice() },
                new object[] { string.Empty, Incorrect, GetRandomSBDevice() },
                new object[] { SB, SB, GetRandomSBDevice() },
                new object[] { SB_VIRT, SB_VIRT, GetRandomSBDevice() },
            };
        }

        /// <summary>
        /// Тест свойств устройств в зависимости от подтипа
        /// </summary>
        /// <param name="expectedProperties">Ожидаемый список свойств</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDevicePropertiesTestData))]
        public void GetDevicePropertiesTest(
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
            var exportForSB = new Dictionary<string, int>()
            {
                {DeviceTag.ST, 1},
                {DeviceTag.M, 1},
                {DeviceTag.P_DT, 1},
            };

            return new object[]
            {
                new object[] {exportForSB, string.Empty, GetRandomSBDevice()},
                new object[] {exportForSB, SB, GetRandomSBDevice()},
                new object[] {null, SB_VIRT, GetRandomSBDevice()},
            };
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
                    SB,
                    GetRandomSBDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomSBDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomSBDevice()
                },
                new object[]
                {
                    new string[0],
                    SB_VIRT,
                    GetRandomSBDevice()
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
        public void ChannelsTest(Dictionary<string, int> expectedChannelsCount,
            string subType, Device.IODevice device)
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
            var defaultSignals = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 1 },
                { DO, 0 },
            };

            var emptySignals = new Dictionary<string, int>()
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
                    defaultSignals,
                    SB,
                    GetRandomSBDevice()
                },
                new object[]
                {
                    defaultSignals,
                    string.Empty,
                    GetRandomSBDevice()
                },
                new object[]
                {
                    emptySignals,
                    Incorrect,
                    GetRandomSBDevice()
                },
                new object[]
                {
                    emptySignals,
                    SB_VIRT,
                    GetRandomSBDevice()
                }
            };
        }

        /// <summary>
        /// Генератор SB устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomSBDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.SB("KOAG4SB1", "+KOAG4-SB1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.SB("LINE1SB2", "+LINE1-SB2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.SB("TANK2SB1", "+TANK2-SB1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new Device.SB("CW_TANK3SB3", "+CW_TANK3-SB3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
