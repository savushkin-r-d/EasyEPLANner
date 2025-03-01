using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class CAMTest
    {
        const string Incorrect = "Incorrect";
        const string CAM_DO1_DI1 = "CAM_DO1_DI1";
        const string CAM_DO1_DI2 = "CAM_DO1_DI2";
        const string CAM_DO1_DI3 = "CAM_DO1_DI3";

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
                new object[] { DeviceSubType.CAM_DO1_DI1, CAM_DO1_DI1,
                    GetRandomCAMDevice() },
                new object[] { DeviceSubType.CAM_DO1_DI2, CAM_DO1_DI2,
                    GetRandomCAMDevice() },
                new object[] { DeviceSubType.CAM_DO1_DI3, CAM_DO1_DI3,
                    GetRandomCAMDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomCAMDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomCAMDevice() },
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
                new object[] { CAM_DO1_DI1, CAM_DO1_DI1, GetRandomCAMDevice() },
                new object[] { CAM_DO1_DI2, CAM_DO1_DI2, GetRandomCAMDevice() },
                new object[] { CAM_DO1_DI3, CAM_DO1_DI3, GetRandomCAMDevice() },
                new object[] { string.Empty, string.Empty, GetRandomCAMDevice() },
                new object[] { string.Empty, Incorrect, GetRandomCAMDevice() },
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
            var camWithoutReady = new Dictionary<ITag, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.RESULT, 1},
            };

            var camWithReady = new Dictionary<ITag, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.RESULT, 1},
                {IODevice.Tag.READY, 1},
                {IODevice.Parameter.P_READY_TIME, 1},
            };

            return new object[]
            {
                new object[] {camWithoutReady, CAM_DO1_DI1, GetRandomCAMDevice()},
                new object[] {camWithReady, CAM_DO1_DI2, GetRandomCAMDevice()},
                new object[] {camWithReady, CAM_DO1_DI3, GetRandomCAMDevice()},
                new object[] {null, Incorrect, GetRandomCAMDevice()},
                new object[] {null, string.Empty, GetRandomCAMDevice()},
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
            var parameters = new string[]
            {
                IODevice.Parameter.P_READY_TIME
            };

            return new object[]
            {
                new object[]
                {
                    new string[0],
                    CAM_DO1_DI1,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    parameters,
                    CAM_DO1_DI2,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    parameters,
                    CAM_DO1_DI3,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomCAMDevice()
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
                    CAM_DO1_DI1,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 2 },
                        { DO, 1 },
                    },
                    CAM_DO1_DI2,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 3 },
                        { DO, 1 },
                    },
                    CAM_DO1_DI3,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    emptyChannels,
                    string.Empty,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomCAMDevice()
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
                    properties,
                    CAM_DO1_DI1,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    properties,
                    CAM_DO1_DI2,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    properties,
                    CAM_DO1_DI3,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomCAMDevice()
                },
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
            return new object[]
            {
                new object[]
                {
                    new string[0],
                    CAM_DO1_DI1,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    CAM_DO1_DI2,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    CAM_DO1_DI3,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomCAMDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomCAMDevice()
                },
            };
        }

        /// <summary>
        /// Генератор M устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomCAMDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new CAM("LINE4CAM1", "+LINE4-CAM1", "Test device", 1,
                        "KOAG", 4);
                case 2:
                    return new CAM("LINE1CAM2", "+LINE1-CAM2", "Test device", 2,
                        "LINE", 1);
                case 3:
                    return new CAM("TANK2CAM1", "+TANK2-CAM1", "Test device", 1,
                        "TANK", 2);
                default:
                    return new CAM("CW_TANK3CAM3", "+CW_TANK3-CAM3",
                        "Test device", 3, "CW_TANK", 3);
            }
        }
    }
}
