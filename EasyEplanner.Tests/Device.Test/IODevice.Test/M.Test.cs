using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Device;

namespace Tests.Devices
{
    public class MTest
    {
        const string Incorrect = "Incorrect";
        const string M = "M";
        const string M_FREQ = "M_FREQ";
        const string M_REV = "M_REV";
        const string M_REV_FREQ = "M_REV_FREQ";
        const string M_REV_2 = "M_REV_2";
        const string M_REV_FREQ_2 = "M_REV_FREQ_2";
        const string M_REV_2_ERROR = "M_REV_2_ERROR";
        const string M_REV_FREQ_2_ERROR = "M_REV_FREQ_2_ERROR";
        const string M_ATV = "M_ATV";
        const string M_VIRT = "M_VIRT";

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
                new object[] { DeviceSubType.M, M,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.M_FREQ, M_FREQ,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.M_REV, M_REV,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.M_REV_FREQ, M_REV_FREQ,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.M_REV_2, M_REV_2,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.M_REV_FREQ_2,
                    M_REV_FREQ_2, GetRandomMDevice() },
                new object[] { DeviceSubType.M_REV_2_ERROR,
                    M_REV_2_ERROR, GetRandomMDevice() },
                new object[] { DeviceSubType.M_REV_FREQ_2_ERROR,
                    M_REV_FREQ_2_ERROR, GetRandomMDevice() },
                new object[] { DeviceSubType.M_ATV, M_ATV,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomMDevice() },
                new object[] { DeviceSubType.M_VIRT, M_VIRT,
                    GetRandomMDevice() },
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
                new object[] { M, M, GetRandomMDevice() },
                new object[] { M_FREQ, M_FREQ, GetRandomMDevice() },
                new object[] { M_REV, M_REV, GetRandomMDevice() },
                new object[] { M_REV_FREQ, M_REV_FREQ,
                    GetRandomMDevice() },
                new object[] { M_REV_2, M_REV_2, GetRandomMDevice() },
                new object[] { M_REV_FREQ_2, M_REV_FREQ_2,
                    GetRandomMDevice() },
                new object[] { M_REV_2_ERROR, M_REV_2_ERROR,
                    GetRandomMDevice() },
                new object[] { M_REV_FREQ_2_ERROR, M_REV_FREQ_2_ERROR,
                    GetRandomMDevice() },
                new object[] { M_ATV, M_ATV, GetRandomMDevice() },
                new object[] { string.Empty, string.Empty, GetRandomMDevice() },
                new object[] { string.Empty, Incorrect, GetRandomMDevice() },
                new object[] { M_VIRT, M_VIRT, GetRandomMDevice() },
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
            var exportForM = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_ON_TIME, 1},
            };

            var exportForMFreq = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_ON_TIME, 1},
                {IODevice.Tag.V, 1},
            };

            var exportForMRev = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.P_ON_TIME, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.R, 1},
            };

            var exportForMATV = new Dictionary<string, int>()
            {
                {IODevice.Tag.M, 1},
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.R, 1},
                {IODevice.Tag.FRQ, 1},
                {IODevice.Tag.RPM, 1},
                {IODevice.Tag.EST, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.P_ON_TIME, 1},
            };

            return new object[]
            {
                new object[] {exportForM, M, GetRandomMDevice()},
                new object[] {exportForMFreq, M_FREQ, GetRandomMDevice()},
                new object[] {exportForMRev, M_REV, GetRandomMDevice()},
                new object[] {exportForMRev, M_REV_FREQ,
                    GetRandomMDevice()},
                new object[] {exportForMRev, M_REV_2, GetRandomMDevice()},
                new object[] {exportForMRev, M_REV_FREQ_2,
                    GetRandomMDevice()},
                new object[] {exportForMRev, M_REV_2_ERROR,
                    GetRandomMDevice()},
                new object[] {exportForMATV, M_ATV, GetRandomMDevice()},
                new object[] {null, Incorrect, GetRandomMDevice()},
                new object[] {null, string.Empty, GetRandomMDevice()},
                new object[] {null, M_VIRT, GetRandomMDevice()},
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
            var parameters = new string[]
            {
                IODevice.Parameter.P_ON_TIME
            };

            return new object[]
            {
                new object[]
                {
                    parameters,
                    M,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_FREQ,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_REV,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_REV_FREQ,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_REV_2,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_REV_FREQ_2,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_REV_2_ERROR,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_REV_FREQ_2_ERROR,
                    GetRandomMDevice()
                },
                new object[]
                {
                    parameters,
                    M_ATV,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_VIRT,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomMDevice()
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
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 1 },
                        { DO, 1 },
                    },
                    M,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 1 },
                        { DO, 1 },
                    },
                    M_FREQ,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 1 },
                        { DO, 2 },
                    },
                    M_REV,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 1 },
                        { DO, 2 },
                    },
                    M_REV_FREQ,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 1 },
                        { DO, 2 },
                    },
                    M_REV_2,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 1 },
                        { DO, 2 },
                    },
                    M_REV_FREQ_2,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 1 },
                        { DO, 2 },
                    },
                    M_REV_2_ERROR,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 2 },
                        { DO, 2 },
                    },
                    M_REV_FREQ_2_ERROR,
                    GetRandomMDevice()
                },
                new object[]
                {
                    emptyChannels,
                    M_ATV,
                    GetRandomMDevice()
                },
                new object[]
                {
                    emptyChannels,
                    string.Empty,
                    GetRandomMDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomMDevice()
                },
                new object[]
                {
                    emptyChannels,
                    M_VIRT,
                    GetRandomMDevice()
                },
            };
        }

        /// <summary>
        /// Тестирование свойств устройства
        /// </summary>
        /// <param name="parametersSequence">Ожидаемые свойства</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(PropertiesTestData))]
        public void Property_NewDev_ReturnsExpectedArrayWithParameters(
            string[] parametersSequence, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            string[] actualParametersSequence = device.Properties
                .Select(x => x.Key)
                .ToArray();
            Assert.AreEqual(parametersSequence, actualParametersSequence);
        }

        /// <summary>
        /// 1 - Свойства в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] PropertiesTestData()
        {
            var properties = new string[]
            {
                IODevice.Property.IP
            };

            return new object[]
            {
                new object[]
                {
                    new string[0],
                    M,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_FREQ,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_REV,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_REV_FREQ,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_REV_2,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_REV_FREQ_2,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_REV_2_ERROR,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_REV_FREQ_2_ERROR,
                    GetRandomMDevice()
                },
                new object[]
                {
                    properties,
                    M_ATV,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    M_VIRT,
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomMDevice()
                },
            };
        }

        /// <summary>
        /// Генератор M устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomMDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new M("KOAG4M1", "+KOAG4-M1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new M("LINE1M2", "+LINE1-M2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new M("TANK2M1", "+TANK2-M1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new M("CW_TANK3M3", "+CW_TANK3-M3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
