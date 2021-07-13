using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Device;

namespace Tests.Devices
{
    public class AOTest
    {
        const string Incorrect = "Incorrect";
        const string AOSubType = "AO";
        const string AO_VIRT = "AO_VIRT";

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
                new object[] { DeviceSubType.AO, string.Empty,
                    GetRandomAODevice() },
                new object[] { DeviceSubType.AO, AOSubType,
                    GetRandomAODevice() },
                new object[] { DeviceSubType.AO_VIRT, AO_VIRT,
                    GetRandomAODevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomAODevice() },
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
                new object[] { AOSubType, string.Empty, GetRandomAODevice() },
                new object[] { AOSubType, AOSubType, GetRandomAODevice() },
                new object[] { AO_VIRT, AO_VIRT, GetRandomAODevice() },
                new object[] { string.Empty, Incorrect, GetRandomAODevice() },
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
            var exportForAO = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_MIN_V, 1},
                {IODevice.Tag.P_MAX_V, 1},
            };

            var exportForVirtAO = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
            };

            return new object[]
            {
                new object[] {exportForAO, string.Empty, GetRandomAODevice()},
                new object[] {exportForAO, AOSubType, GetRandomAODevice()},
                new object[] {exportForVirtAO, AO_VIRT, GetRandomAODevice()},
                new object[] {null, Incorrect, GetRandomAODevice()},
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
        public void GetRangeTest(string expected, string subType,
            double value1, double value2, IODevice device)
        {
            device.SetSubType(subType);
            device.SetParameter(IODevice.Parameter.P_MIN_V, value1);
            device.SetParameter(IODevice.Parameter.P_MAX_V, value2);
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
                new object[] {$"_{2.0}..{4.0}", string.Empty, 2.0, 4.0,
                    GetRandomAODevice()},
                new object[] {$"_{1.0}..{3.0}", AOSubType, 1.0, 3.0,
                    GetRandomAODevice()},
                new object[] {string.Empty, AO_VIRT, 4.0, 8.0,
                    GetRandomAODevice()},
                new object[] {string.Empty, Incorrect, 7.0, 9.0,
                    GetRandomAODevice()},
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
                IODevice.Parameter.P_MIN_V,
                IODevice.Parameter.P_MAX_V,
            };

            return new object[]
            {
                new object[]
                {
                    defaultParameters,
                    AOSubType,
                    GetRandomAODevice()
                },
                new object[]
                {
                    defaultParameters,
                    string.Empty,
                    GetRandomAODevice()
                },
                new object[]
                {
                    new string[0],
                    AO_VIRT,
                    GetRandomAODevice()
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
            var oneAnalogOutputChannel = new Dictionary<string, int>()
            {
                { AI, 0 },
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
                    oneAnalogOutputChannel,
                    string.Empty,
                    GetRandomAODevice()
                },
                new object[]
                {
                    oneAnalogOutputChannel,
                    AOSubType,
                    GetRandomAODevice()
                },
                new object[]
                {
                    emptyChannels,
                    AO_VIRT,
                    GetRandomAODevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomAODevice()
                },
            };
        }

        /// <summary>
        /// Генератор AO устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomAODevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new AO("KOAG4AO1", "+KOAG4-AO1",
                        "Test device", 1, "KOAG", 4);
                case 2:
                    return new AO("LINE1AO2", "+LINE1-AO2",
                        "Test device", 2, "LINE", 1);
                case 3:
                    return new AO("TANK2AO1", "+TANK2-AO1",
                        "Test device", 1, "TANK", 2);
                default:
                    return new AO("CW_TANK3AO3", "+CW_TANK3-AO3",
                        "Test device", 3, "CW_TANK", 3);
            }
        }
    }
}
