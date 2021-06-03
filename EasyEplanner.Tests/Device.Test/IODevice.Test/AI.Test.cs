using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests
{
    public class AITest
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
                new object[] { Device.DeviceSubType.AI, "", 
                    GetRandomAIDevice() },
                new object[] { Device.DeviceSubType.AI, "AI", 
                    GetRandomAIDevice() },
                new object[] { Device.DeviceSubType.AI_VIRT, "AI_VIRT", 
                    GetRandomAIDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect", 
                    GetRandomAIDevice() },
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
                new object[] { "AI", "", GetRandomAIDevice() },
                new object[] { "AI", "AI", GetRandomAIDevice() },
                new object[] { "AI_VIRT", "AI_VIRT", GetRandomAIDevice() },
                new object[] { "", "Incorrect", GetRandomAIDevice() },
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
            var exportForAI = new Dictionary<string, int>()
            {
                {"M", 1},
                {"ST", 1},
                {"P_MIN_V", 1},
                {"P_MAX_V", 1},
                {"V", 1},
            };

            var exportForAIVirt = new Dictionary<string, int>()
            {
                {"M", 1},
                {"ST", 1},
                {"V", 1},
            };

            return new object[]
            {
                new object[] {exportForAI, "", GetRandomAIDevice()},
                new object[] {exportForAI, "AI", GetRandomAIDevice()},
                new object[] {exportForAIVirt, "AI_VIRT", GetRandomAIDevice()},
                new object[] {null, "Incorrect", GetRandomAIDevice()},
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
			    new object[] {$"_{2.0}..{4.0}", "", 2.0, 4.0, 
                    GetRandomAIDevice()},
			    new object[] {$"_{1.0}..{3.0}", "AI", 1.0, 3.0, 
                    GetRandomAIDevice()},
			    new object[] {$"", "AI_VIRT", 4.0, 8.0, GetRandomAIDevice()},
			    new object[] {$"", "Incorrect", 7.0, 9.0, GetRandomAIDevice()},
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
                    "",
                    GetRandomAIDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_MIN_V", "P_MAX_V" },
                    "AI",
                    GetRandomAIDevice()
                },
                new object[]
                {
                    new string[0],
                    "AI_VIRT",
                    GetRandomAIDevice()
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
                    "", 
                    GetRandomAIDevice() 
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
                    "AI", 
                    GetRandomAIDevice() 
                },
                new object[] 
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "AI_VIRT", 
                    GetRandomAIDevice() 
                },
                new object[] 
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "Incorrect", 
                    GetRandomAIDevice() 
                },
            };
        }

        /// <summary>
        /// Генератор AI устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomAIDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.AI("KOAG4AI1", "+KOAG4-AI1",
                        "Test device", 1, "KOAG", 4);
                case 2:
                    return new Device.AI("LINE1AI2", "+LINE1-AI2",
                        "Test device", 2, "LINE", 1);
                case 3:
                    return new Device.AI("TANK2AI1", "+TANK2-AI1",
                        "Test device", 1, "TANK", 2);
                default:
                    return new Device.AI("CW_TANK3AI3", "+CW_TANK3-AI3",
                        "Test device", 3, "CW_TANK", 3);
            }
        }
    }
}
