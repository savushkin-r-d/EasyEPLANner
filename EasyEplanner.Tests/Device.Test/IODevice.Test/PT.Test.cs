using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class PTTest
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
        public void GetDevicePropertiesTest(List<string> expectedProperties,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedProperties, device.GetDeviceProperties(
                device.DeviceType, device.DeviceSubType));
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
            double value1, double value2, Device.IODevice device)
        {
            device.SetSubType(subType);
            device.SetParameter("P_MIN_V", value1);
            device.SetParameter("P_MAX_V", value2);
            Assert.AreEqual(expected, device.GetRange());
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
        /// 1 - Ожидаемое перечисление подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomPTDevice() },
                new object[] { Device.DeviceSubType.PT, "PT",
                    GetRandomPTDevice() },
                new object[] { Device.DeviceSubType.PT_IOLINK, "PT_IOLINK",
                    GetRandomPTDevice() },
                new object[] { Device.DeviceSubType.DEV_SPAE, "DEV_SPAE",
                    GetRandomPTDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomPTDevice() },
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
                new object[] { "", "", GetRandomPTDevice() },
                new object[] { "PT", "PT", GetRandomPTDevice() },
                new object[] { "PT_IOLINK", "PT_IOLINK", GetRandomPTDevice() },
                new object[] { "DEV_SPAE", "DEV_SPAE", GetRandomPTDevice() },
                new object[] { "", "Incorrect", GetRandomPTDevice() },
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
            var exportForPT = new List<string>
            {
                "ST",
                "M",
                "V",
                "P_MIN_V",
                "P_MAX_V",
                "P_CZ"
            };

            var exportForPTIOLink = new List<string>
            {
                "M",
                "V",
                "P_MIN_V",
                "P_MAX_V",
            };

            var exportForDevSpae = new List<string>
            {
                "M",
                "V"
            };

            return new object[]
            {
                new object[] {exportForPT, "PT", GetRandomPTDevice()},
                new object[] {exportForPTIOLink, "PT_IOLINK", 
                    GetRandomPTDevice()},
                new object[] {exportForDevSpae, "DEV_SPAE", 
                    GetRandomPTDevice()},
                new object[] {null, "", GetRandomPTDevice()},
                new object[] {null, "Incorrect", GetRandomPTDevice()},
            };
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
                new object[] {$"_{2.0}..{4.0}", "PT", 2.0, 4.0,
                    GetRandomPTDevice()},
                new object[] {$"", "PT_IOLINK", 1.0, 3.0,
                    GetRandomPTDevice()},
                new object[] {$"", "DEV_SPAE", 1.0, 3.0,
                    GetRandomPTDevice()},
                new object[] {$"", "", 4.0, 8.0, GetRandomPTDevice()},
                new object[] {$"", "Incorrect", 7.0, 9.0, GetRandomPTDevice()},
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
                    new string[] { "P_C0", "P_MIN_V", "P_MAX_V" },
                    "PT",
                    GetRandomPTDevice()
                },
                new object[]
                {
                    new string[] { "P_ERR" },
                    "PT_IOLINK",
                    GetRandomPTDevice()
                },
                new object[]
                {
                    new string[] { "P_ERR" },
                    "DEV_SPAE",
                    GetRandomPTDevice()
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
                    "PT_IOLINK",
                    GetRandomPTDevice()
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
                    "PT",
                    GetRandomPTDevice()
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
                    "DEV_SPAE",
                    GetRandomPTDevice()
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
                    GetRandomPTDevice()
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
                    GetRandomPTDevice()
                },
            };
        }

        /// <summary>
        /// Генератор PT устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomPTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.PT("KOAG4PT1", "Test device", 1,
                        "KOAG", 4, "Test article");
                case 2:
                    return new Device.PT("LINE1PT2", "Test device", 2,
                        "LINE", 1, "Test article");
                case 3:
                    return new Device.PT("TANK2PT1", "Test device", 1,
                        "TANK", 2, "Test article");
                default:
                    return new Device.PT("CW_TANK3PT3", "Test device", 3,
                        "CW_TANK", 3, "Test article");
            }
        }
    }
}
