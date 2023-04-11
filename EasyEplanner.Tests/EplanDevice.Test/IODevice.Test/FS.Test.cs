using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class FSTest
    {
        const string Incorrect = "Incorrect";
        const string FS = "FS";
        const string FS_VIRT = "FS_VIRT";

        const string AI = IODevice.IOChannel.AI;
        const string AO = IODevice.IOChannel.AO;
        const string DI = IODevice.IOChannel.DI;
        const string DO = IODevice.IOChannel.DO;

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
                new object[] { FS, string.Empty, GetRandomFSDevice() },
                new object[] { string.Empty, Incorrect, GetRandomFSDevice() },
                new object[] { FS, FS, GetRandomFSDevice() },
                new object[] { FS_VIRT, FS_VIRT, GetRandomFSDevice() },
            };
        }

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
                new object[] { DeviceSubType.FS, string.Empty,
                    GetRandomFSDevice() },
                new object[] { DeviceSubType.FS, FS,
                    GetRandomFSDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomFSDevice() },
                new object[] { DeviceSubType.FS_VIRT, FS_VIRT,
                    GetRandomFSDevice() },
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
            var exportForFS = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Parameter.P_DT, 1},
            };

            var exportForFSVirt = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
            };

            return new object[]
            {
                new object[] {exportForFS, string.Empty, GetRandomFSDevice()},
                new object[] {exportForFS, FS, GetRandomFSDevice()},
                new object[] {exportForFSVirt, FS_VIRT, GetRandomFSDevice()},
                new object[] {null, Incorrect, GetRandomFSDevice()},
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
            var defaultParameters = new string[]
            {
                IODevice.Parameter.P_DT
            };

            return new object[]
            {
                new object[]
                {
                    defaultParameters,
                    FS,
                    GetRandomFSDevice()
                },
                new object[]
                {
                    defaultParameters,
                    string.Empty,
                    GetRandomFSDevice()
                },
                new object[]
                {
                    new string[0],
                    FS_VIRT,
                    GetRandomFSDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomFSDevice()
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
            var discreteSensorChannels = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 1 },
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
                    discreteSensorChannels,
                    FS,
                    GetRandomFSDevice()
                },
                new object[]
                {
                    discreteSensorChannels,
                    string.Empty,
                    GetRandomFSDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomFSDevice()
                },
                new object[]
                {
                    emptyChannels,
                    FS_VIRT,
                    GetRandomFSDevice()
                }
            };
        }

        /// <summary>
        /// Генератор FS устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomFSDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new FS("KOAG4FS1", "+KOAG4-FS1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new FS("LINE1FS2", "+LINE1-FS2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new FS("TANK2FS1", "+TANK2-FS1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new FS("CW_TANK3FS3", "+CW_TANK3-FS3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
