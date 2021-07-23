using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Device;

namespace Tests.Devices
{
    public class LTTest
    {
        const string Incorrect = "Incorrect";
        const string LT = "LT";
        const string LT_IOLINK = "LT_IOLINK";
        const string LT_CYL = "LT_CYL";
        const string LT_CONE = "LT_CONE";
        const string LT_TRUNC = "LT_TRUNC";
        const string LT_VIRT = "LT_VIRT";

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
                new object[] { DeviceSubType.LT, LT,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.LT_CYL, LT_CYL,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.LT_CONE, LT_CONE,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.LT_TRUNC, LT_TRUNC,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.LT_IOLINK, LT_IOLINK,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.LT_VIRT, LT_VIRT,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomLTDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomLTDevice() },
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
                new object[] { LT, LT, GetRandomLTDevice() },
                new object[] { LT_CYL, LT_CYL, GetRandomLTDevice() },
                new object[] { LT_CONE, LT_CONE, GetRandomLTDevice() },
                new object[] { LT_TRUNC, LT_TRUNC, GetRandomLTDevice() },
                new object[] { LT_IOLINK, LT_IOLINK, GetRandomLTDevice() },
                new object[] { LT_VIRT, LT_VIRT, GetRandomLTDevice() },
                new object[] { string.Empty, string.Empty,
                    GetRandomLTDevice() },
                new object[] { string.Empty, Incorrect, GetRandomLTDevice() },
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
            var exportForLT = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_ERR, 1},
            };

            var exportForLTIOLink = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_H_CONE, 1},
                {IODevice.Parameter.P_MAX_P, 1},
                {IODevice.Parameter.P_R, 1},
                {IODevice.Tag.CLEVEL, 1},
                {IODevice.Parameter.P_ERR, 1},
            };

            var exportForLTCyl = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_MAX_P, 1},
                {IODevice.Parameter.P_R, 1},
                {IODevice.Tag.CLEVEL, 1},
                {IODevice.Parameter.P_ERR, 1},
            };

            var exportForLTCone = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_MAX_P, 1},
                {IODevice.Parameter.P_R, 1},
                {IODevice.Parameter.P_H_CONE, 1},
                {IODevice.Tag.CLEVEL, 1},
                {IODevice.Parameter.P_ERR, 1},
            };

            var exportForLTTrunc = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Parameter.P_MAX_P, 1},
                {IODevice.Parameter.P_R, 1},
                {IODevice.Parameter.P_H_TRUNC, 1},
                {IODevice.Tag.CLEVEL, 1},
                {IODevice.Parameter.P_ERR, 1},
            };

            var exportForLTVirt = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
            };

            return new object[]
            {
                new object[] {exportForLT, LT, GetRandomLTDevice()},
                new object[] {exportForLTIOLink, LT_IOLINK,
                    GetRandomLTDevice()},
                new object[] {exportForLTCyl, LT_CYL, GetRandomLTDevice()},
                new object[] {exportForLTCone, LT_CONE,
                    GetRandomLTDevice()},
                new object[] {exportForLTTrunc, LT_TRUNC,
                    GetRandomLTDevice()},
                new object[] {exportForLTVirt, LT_VIRT, GetRandomLTDevice()},
                new object[] {null, Incorrect, GetRandomLTDevice()},
                new object[] {null, string.Empty, GetRandomLTDevice()},
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
            var colAndIOLSubTypesParameters = new string[]
            {
                IODevice.Parameter.P_C0,
                IODevice.Parameter.P_ERR,
                IODevice.Parameter.P_MAX_P,
                IODevice.Parameter.P_R,
                IODevice.Parameter.P_H_CONE
            };

            return new object[]
            {
                new object[]
                {
                    new string[]
                    {
                        IODevice.Parameter.P_C0,
                        IODevice.Parameter.P_ERR
                    },
                    LT,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[]
                    {
                        IODevice.Parameter.P_C0,
                        IODevice.Parameter.P_ERR,
                        IODevice.Parameter.P_MAX_P,
                        IODevice.Parameter.P_R
                    },
                    LT_CYL,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    colAndIOLSubTypesParameters,
                    LT_CONE,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[]
                    {
                        IODevice.Parameter.P_C0,
                        IODevice.Parameter.P_ERR,
                        IODevice.Parameter.P_MAX_P,
                        IODevice.Parameter.P_R,
                        IODevice.Parameter.P_H_TRUNC
                    },
                    LT_TRUNC,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    colAndIOLSubTypesParameters,
                    LT_IOLINK,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    LT_VIRT,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomLTDevice()
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
                    defaultChannels,
                    LT,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    defaultChannels,
                    LT_CYL,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    defaultChannels,
                    LT_CONE,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    defaultChannels,
                    LT_TRUNC,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    defaultChannels,
                    LT_IOLINK,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    LT_VIRT,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    string.Empty,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomLTDevice()
                },
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
            IODevice device)
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
            var defaultProperties = new string[]
            {
                IODevice.Property.PT,
            };

            return new object[]
            {
                new object[]
                {
                    defaultProperties,
                    LT_IOLINK,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    LT_VIRT,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    LT_TRUNC,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    LT_CONE,
                    GetRandomLTDevice()
                },
                new object[]
                {
                    new string[0],
                    LT_CYL,
                    GetRandomLTDevice()
                },
            };
        }

        /// <summary>
        /// Генератор LT устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomLTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new LT("KOAG4LT1", "+KOAG4-LT1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new LT("LINE1LT2", "+LINE1-LT2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new LT("TANK2LT1", "+TANK2-LT1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new LT("CW_TANK3LT3", "+CW_TANK3-LT3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
