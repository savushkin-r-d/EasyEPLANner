using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Devices
{
    public class VCTest
    {
        const string VC = "VC";
        const string VC_IOLINK = "VC_IOLINK";
        const string VC_VIRT = "VC_VIRT";
        const string Incorrect = "Incorrect";

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
                new object[] { Device.DeviceSubType.VC, VC,
                    GetRandomVCDevice() },
                new object[] { Device.DeviceSubType.VC_IOLINK, VC_IOLINK,
                    GetRandomVCDevice() },
                new object[] { Device.DeviceSubType.NONE, string.Empty,
                    GetRandomVCDevice() },
                new object[] { Device.DeviceSubType.NONE, Incorrect,
                    GetRandomVCDevice() },
                new object[] { Device.DeviceSubType.VC_VIRT, VC_VIRT,
                    GetRandomVCDevice() },
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
                new object[] { VC, VC, GetRandomVCDevice() },
                new object[] { VC_IOLINK, VC_IOLINK, GetRandomVCDevice() },
                new object[] { string.Empty, string.Empty, GetRandomVCDevice() },
                new object[] { string.Empty, Incorrect, GetRandomVCDevice() },
                new object[] { VC_VIRT, VC_VIRT, GetRandomVCDevice() },
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
            var exportForVC = new Dictionary<string, int>()
            {
                {DeviceTag.ST, 1},
                {DeviceTag.M, 1},
                {DeviceTag.V, 1},
            };

            var exportForVCIOL = new Dictionary<string, int>()
            {
                {DeviceTag.ST, 1},
                {DeviceTag.M, 1},
                {DeviceTag.V, 1},
                {DeviceTag.BLINK, 1},
                {DeviceTag.NAMUR_ST, 1},
                {DeviceTag.OPENED, 1},
                {DeviceTag.CLOSED, 1},
            };

            return new object[]
            {
                new object[] {null, Incorrect, GetRandomVCDevice()},
                new object[] {null, string.Empty, GetRandomVCDevice()},
                new object[] {exportForVC, VC, GetRandomVCDevice()},
                new object[] {exportForVCIOL, VC_IOLINK, GetRandomVCDevice()},
                new object[] {null, VC_VIRT, GetRandomVCDevice()}
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
                    VC,
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new string[0],
                    VC_IOLINK,
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new string[0],
                    "Incorrect",
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new string[0],
                    VC_VIRT,
                    GetRandomVCDevice()
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
            return new object[]
            {
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    VC,
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    string.Empty,
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    Incorrect,
                    GetRandomVCDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 1 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    VC_IOLINK,
                    GetRandomVCDevice()
                }
            };
        }

        /// <summary>
        /// Генератор VC устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomVCDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.VC("KOAG4VC1", "+KOAG4-VC1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.VC("LINE1VC2", "+LINE1-VC2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.VC("TANK2VC1", "+TANK2-VC1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new Device.VC("CW_TANK3VC3", "+CW_TANK3-VC3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
