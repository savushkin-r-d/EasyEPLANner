using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Devices
{
    public class DITest
    {
        const string Incorrect = "Incorrect";
        const string DISubType = "DI";
        const string DI_VIRT = "DI_VIRT";

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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.DI, string.Empty,
                    GetRandomDIDevice() },
                new object[] { Device.DeviceSubType.DI, DISubType,
                    GetRandomDIDevice() },
                new object[] { Device.DeviceSubType.DI_VIRT, DI_VIRT,
                    GetRandomDIDevice() },
                new object[] { Device.DeviceSubType.NONE, Incorrect,
                    GetRandomDIDevice() },
            };
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
                new object[] { DISubType, string.Empty, GetRandomDIDevice() },
                new object[] { DISubType, DISubType, GetRandomDIDevice() },
                new object[] { DI_VIRT, DI_VIRT, GetRandomDIDevice() },
                new object[] { string.Empty, Incorrect, GetRandomDIDevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetDevicePropertiesTestData()
        {
            var exportForDI = new Dictionary<string, int>()
            {
                {DeviceTag.ST, 1},
                {DeviceTag.M, 1},
                {DeviceTag.P_DT, 1},
            };

            var exportForVirtDI = new Dictionary<string, int>()
            {
                {DeviceTag.ST, 1},
                {DeviceTag.M, 1},
            };

            return new object[]
            {
                new object[] {exportForDI, string.Empty, GetRandomDIDevice()},
                new object[] {exportForDI, DISubType, GetRandomDIDevice()},
                new object[] {exportForVirtDI, DI_VIRT, GetRandomDIDevice()},
                new object[] {null, Incorrect, GetRandomDIDevice()},
            };
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
                DeviceParameter.P_DT,
            };

            return new object[]
            {
                new object[]
                {
                    defaultParameters,
                    DISubType,
                    GetRandomDIDevice()
                },
                new object[]
                {
                    defaultParameters,
                    string.Empty,
                    GetRandomDIDevice()
                },
                new object[]
                {
                    new string[0],
                    DI_VIRT,
                    GetRandomDIDevice()
                },
            };
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
            var oneDiscreteInputChannel = new Dictionary<string, int>()
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
                    oneDiscreteInputChannel,
                    string.Empty,
                    GetRandomDIDevice()
                },
                new object[]
                {
                    oneDiscreteInputChannel,
                    DISubType,
                    GetRandomDIDevice()
                },
                new object[]
                {
                    emptyChannels,
                    DI_VIRT,
                    GetRandomDIDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomDIDevice()
                },
            };
        }

        /// <summary>
        /// Генератор DI устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomDIDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.DI("KOAG4DI1", "+KOAG4-DI1",
                        "Test device", 1, "KOAG", 4);
                case 2:
                    return new Device.DI("LINE1DI2", "+LINE1-DI2",
                        "Test device", 2, "LINE", 1);
                case 3:
                    return new Device.DI("TANK2DI1", "+TANK2-DI1",
                        "Test device", 1, "TANK", 2);
                default:
                    return new Device.DI("CW_TANK3DI3", "+CW_TANK3-DI3",
                        "Test device", 3, "CW_TANK", 3);
            }
        }
    }
}
