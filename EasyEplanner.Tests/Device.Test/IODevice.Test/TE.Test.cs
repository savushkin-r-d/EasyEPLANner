using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    class TETest
    {
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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.TE, "TE",
                    GetRandomTEDevice() },
                new object[] { Device.DeviceSubType.TE_IOLINK, "TE_IOLINK",
                    GetRandomTEDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomTEDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomTEDevice() },
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
                new object[] { "", "", GetRandomTEDevice() },
                new object[] { "TE", "TE", GetRandomTEDevice() },
                new object[] { "TE_IOLINK", "TE_IOLINK", GetRandomTEDevice() },
                new object[] { "", "Incorrect", GetRandomTEDevice() },
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
            var exportForTE = new Dictionary<string, int>()
            {
                {"M", 1},
                {"P_CZ", 1},
                {"V", 1},
                {"ST", 1},
            };

            return new object[]
            {
                new object[] {null, "", GetRandomTEDevice()},
                new object[] {exportForTE, "TE", GetRandomTEDevice()},
                new object[] {exportForTE, "TE_IOLINK", GetRandomTEDevice()},
                new object[] {null, "Incorrect", GetRandomTEDevice()},
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
            return new object[]
            {
                new object[] 
                { 
                    new string[] { "P_C0", "P_ERR" }, 
                    "TE", 
                    GetRandomTEDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_ERR" },
                    "TE_IOLINK",
                    GetRandomTEDevice()
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
            return new object[]
            {
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "TE",
                    GetRandomTEDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "TE_IOLINK",
                    GetRandomTEDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "",
                    GetRandomTEDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "Incorrect",
                    GetRandomTEDevice()
                },
            };
        }

        /// <summary>
        /// Генератор TE устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomTEDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.TE("KOAG4TE1", "+KOAG4-TE1",
                        "Test device", 1, "KOAG", 4, "Test article");
                case 2:
                    return new Device.TE("LINE1TE2", "+LINE1-TE2",
                        "Test device", 2, "LINE", 1, "Test article");
                case 3:
                    return new Device.TE("TANK2TE1", "+TANK2-TE1",
                        "Test device", 1, "TANK", 2, "Test article");
                default:
                    return new Device.TE("CW_TANK3TE3", "+CW_TANK3-TE3",
                        "Test device", 3, "CW_TANK", 3, "Test article");
            }
        }
    }
}
