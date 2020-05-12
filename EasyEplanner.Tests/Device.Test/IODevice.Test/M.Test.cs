using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class MTest
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
        [TestCaseSource(nameof(GetSubTypeTestData))]
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
                new object[] { Device.DeviceSubType.M, "M",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_FREQ, "M_FREQ",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_REV, "M_REV",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_REV_FREQ, "M_REV_FREQ",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_REV_2, "M_REV_2",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_REV_FREQ_2, 
                    "M_REV_FREQ_2", GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_REV_2_ERROR, 
                    "M_REV_2_ERROR", GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_REV_FREQ_2_ERROR, 
                    "M_REV_FREQ_2_ERROR", GetRandomMDevice() },
                new object[] { Device.DeviceSubType.M_ATV, "M_ATV",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomMDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomMDevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { "M", "M", GetRandomMDevice() },
                new object[] { "M_FREQ", "M_FREQ", GetRandomMDevice() },
                new object[] { "M_REV", "M_REV", GetRandomMDevice() },
                new object[] { "M_REV_FREQ", "M_REV_FREQ", 
                    GetRandomMDevice() },
                new object[] { "M_REV_2", "M_REV_2", GetRandomMDevice() },
                new object[] { "M_REV_FREQ_2", "M_REV_FREQ_2", 
                    GetRandomMDevice() },
                new object[] { "M_REV_2_ERROR", "M_REV_2_ERROR", 
                    GetRandomMDevice() },
                new object[] { "M_REV_FREQ_2_ERROR", "M_REV_FREQ_2_ERROR", 
                    GetRandomMDevice() },
                new object[] { "M_ATV", "M_ATV", GetRandomMDevice() },
                new object[] { "", "", GetRandomMDevice() },
                new object[] { "", "Incorrect", GetRandomMDevice() },
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
            var exportForM = new List<string>
            {
                "ST",
                "M",
                "P_ON_TIME",
                "V"
            };

            var exportForMRev = new List<string>
            {
                "ST",
                "M",
                "P_ON_TIME",
                "V",
                "R"
            };

            var exportForMATV = new List<string>
            {
                "M",
                "ST",
                "R",
                "FRQ",
                "RPM",
                "EST",
                "V",
                "P_ON_TIME"
            };

            return new object[]
            {
                new object[] {exportForM, "M", GetRandomMDevice()},
                new object[] {exportForM, "M_FREQ", GetRandomMDevice()},
                new object[] {exportForMRev, "M_REV", GetRandomMDevice()},
                new object[] {exportForMRev, "M_REV_FREQ", 
                    GetRandomMDevice()},
                new object[] {exportForMRev, "M_REV_2", GetRandomMDevice()},
                new object[] {exportForMRev, "M_REV_FREQ_2", 
                    GetRandomMDevice()},
                new object[] {exportForMRev, "M_REV_2_ERROR", 
                    GetRandomMDevice()},
                new object[] {exportForMATV, "M_ATV", GetRandomMDevice()},
                new object[] {null, "Incorrect", GetRandomMDevice()},
                new object[] {null, "", GetRandomMDevice()},
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
                    new string[] { "P_ON_TIME" },
                    "M",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_FREQ",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_REV",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_REV_FREQ",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_REV_2",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_REV_FREQ_2",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_REV_2_ERROR",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_REV_FREQ_2_ERROR",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME" },
                    "M_ATV",
                    GetRandomMDevice()
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
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 1 },
                        { "DO", 1 },
                    },
                    "M",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 1 },
                        { "DO", 1 },
                    },
                    "M_FREQ",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 1 },
                        { "DO", 2 },
                    },
                    "M_REV",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 1 },
                        { "DO", 2 },
                    },
                    "M_REV_FREQ",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 1 },
                        { "DO", 2 },
                    },
                    "M_REV_2",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 1 },
                        { "DO", 2 },
                    },
                    "M_REV_FREQ_2",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 1 },
                        { "DO", 2 },
                    },
                    "M_REV_2_ERROR",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 2 },
                        { "DO", 2 },
                    },
                    "M_REV_FREQ_2_ERROR",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 1 },
                    },
                    "",
                    GetRandomMDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 1 },
                    },
                    "Incorrect",
                    GetRandomMDevice()
                },
            };
        }

        /// <summary>
        /// Генератор M устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomMDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.M("KOAG4M1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.M("LINE1M2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.M("TANK2M1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.M("CW_TANK3M3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
