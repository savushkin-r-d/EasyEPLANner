using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Device;

namespace Tests.Devices
{
    public class FQTTest
    {
        const string Incorrect = "Incorrect";
        const string FQT = "FQT";
        const string FQT_F = "FQT_F";
        const string FQT_F_OK = "FQT_F_OK";
        const string FQT_VIRT = "FQT_VIRT";

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
                new object[] { DeviceSubType.FQT, FQT,
                    GetRandomFQTDevice() },
                new object[] { DeviceSubType.FQT_F, FQT_F,
                    GetRandomFQTDevice() },
                new object[] { DeviceSubType.FQT_F_OK, FQT_F_OK,
                    GetRandomFQTDevice() },
                new object[] { DeviceSubType.FQT_VIRT, FQT_VIRT,
                    GetRandomFQTDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomFQTDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomFQTDevice() },
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
                new object[] { string.Empty, string.Empty, GetRandomFQTDevice() },
                new object[] { FQT, FQT, GetRandomFQTDevice() },
                new object[] { FQT_F, FQT_F, GetRandomFQTDevice() },
                new object[] { FQT_F_OK, FQT_F_OK, GetRandomFQTDevice() },
                new object[] { FQT_VIRT, FQT_VIRT, GetRandomFQTDevice() },
                new object[] { string.Empty, Incorrect, GetRandomFQTDevice() },
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
            var exportForFQT = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.ABS_V, 1},
            };

            var exportForFQTF = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_MIN_FLOW, 1},
                {IODevice.Tag.P_MAX_FLOW, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.F, 1},
                {IODevice.Tag.P_DT, 1},
                {IODevice.Tag.ABS_V, 1},
            };

            var exportForFQTFOK = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_MIN_FLOW, 1},
                {IODevice.Tag.P_MAX_FLOW, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.F, 1},
                {IODevice.Tag.P_DT, 1},
                {IODevice.Tag.ABS_V, 1},
                {IODevice.Tag.OK, 1},
            };

            var exportForFQTVirt = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_MIN_FLOW, 1},
                {IODevice.Tag.P_MAX_FLOW, 1},
                {IODevice.Tag.P_CZ, 1},
                {IODevice.Tag.F, 1},
                {IODevice.Tag.P_DT, 1},
                {IODevice.Tag.ABS_V, 1},
            };

            return new object[]
            {
                new object[] {exportForFQT, FQT, GetRandomFQTDevice()},
                new object[] {exportForFQTF, FQT_F, GetRandomFQTDevice()},
                new object[] {exportForFQTFOK, FQT_F_OK,
                    GetRandomFQTDevice()},
                new object[] {exportForFQTVirt, FQT_VIRT,
                    GetRandomFQTDevice()},
                new object[] {null, Incorrect, GetRandomFQTDevice()},
                new object[] {null, string.Empty, GetRandomFQTDevice()},
            };
        }

        /// <summary>
        /// Тестирование диапазона измерения устройства
        /// </summary>
        /// <param name="expected">Ожидаемый диапазон</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="value1">Начало диапазона</param>
        /// <param name="value2">Конец диапазона</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetRangeTestData))]
        public void GetRange_NewDev_ReturnsExpectedRangeString(string expected,
            string subType, double value1, double value2,
            IODevice device)
        {
            device.SetSubType(subType);
            device.SetParameter(IODevice.Parameter.P_MIN_F, value1);
            device.SetParameter(IODevice.Parameter.P_MAX_F, value2);
            Assert.AreEqual(expected, device.GetRange());
        }

        /// <summary>
        /// 1 - Ожидаемое значение,
        /// 2 - Подтип устройства в виде строки,
        /// 3 - Значение параметра меньшее,
        /// 4 - Значение параметра большее,
        /// 5 - Устройство для теста
        /// </summary>
        /// <returns></returns>
        private static object[] GetRangeTestData()
        {
            return new object[]
            {
                new object[] {$"_{2.0}..{4.0}", FQT_F, 2.0, 4.0,
                    GetRandomFQTDevice()},
                new object[] {$"_{1.0}..{3.0}", FQT_F_OK, 1.0, 3.0,
                    GetRandomFQTDevice()},
                new object[] {string.Empty, FQT, 4.0, 8.0,
                    GetRandomFQTDevice()},
                new object[] {string.Empty, FQT_VIRT, 7.0, 9.0,
                    GetRandomFQTDevice()},
                new object[] {string.Empty, string.Empty, 4.0, 8.0,
                    GetRandomFQTDevice()},
                new object[] {string.Empty, Incorrect, 7.0, 9.0,
                    GetRandomFQTDevice()},
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
                IODevice.Parameter.P_MIN_F,
                IODevice.Parameter.P_MAX_F,
                IODevice.Parameter.P_C0,
                IODevice.Parameter.P_DT
            };

            return new object[]
            {
                new object[]
                {
                    new string[0],
                    FQT,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    defaultParameters,
                    FQT_F,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    defaultParameters,
                    FQT_F_OK,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    FQT_VIRT,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomFQTDevice()
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
                    new Dictionary<string, int>()
                    {
                        { AI, 1 },
                        { AO, 0 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    FQT,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 2 },
                        { AO, 0 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    FQT_F,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 2 },
                        { AO, 0 },
                        { DI, 1 },
                        { DO, 0 },
                    },
                    FQT_F_OK,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    FQT_VIRT,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    emptyChannels,
                    string.Empty,
                    GetRandomFQTDevice()
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
                IODevice.Property.MT,
            };

            return new object[]
            {
                new object[]
                {
                    new string[0],
                    FQT,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    defaultProperties,
                    FQT_F,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    defaultProperties,
                    FQT_F_OK,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    FQT_VIRT,
                    GetRandomFQTDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomFQTDevice()
                },
            };
        }

        /// <summary>
        /// Генератор FQT устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomFQTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new FQT("KOAG4FQT1", "+KOAG4-FQT1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new FQT("LINE1FQT2", "+LINE1-FQT2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new FQT("TANK2FQT1", "+TANK2-FQT1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new FQT("CW_TANK3FQT3", "+CW_TANK3-FQT3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
