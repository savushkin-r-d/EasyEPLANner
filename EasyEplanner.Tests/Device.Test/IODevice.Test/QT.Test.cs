using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Devices
{
    public class QTTest
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
                new object[] { Device.DeviceSubType.NONE, string.Empty,
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT, "QT",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT_IOLINK, "QT_IOLINK",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT_OK, "QT_OK",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomQTDevice() },
                new object[] { Device.DeviceSubType.QT_VIRT, "QT_VIRT",
                    GetRandomQTDevice() },
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
                new object[] { string.Empty, string.Empty, GetRandomQTDevice() },
                new object[] { "QT", "QT", GetRandomQTDevice() },
                new object[] { "QT_IOLINK", "QT_IOLINK", GetRandomQTDevice() },
                new object[] { "QT_OK", "QT_OK", GetRandomQTDevice() },
                new object[] { string.Empty, "Incorrect", GetRandomQTDevice() },
                new object[] { "QT_VIRT", "QT_VIRT", GetRandomQTDevice() },
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
            var exportForQT = new Dictionary<string, int>()
            {
                {"ST", 1},
                {"M", 1},
                {"V", 1},
                {"P_MIN_V", 1},
                {"P_MAX_V", 1},
                {"P_CZ", 1},
            };

            var exportForQTIOLink = new Dictionary<string, int>()
            {
                {"ST", 1},
                {"M", 1},
                {"V", 1},
                {"P_CZ", 1},
                {"T", 1},
                {"P_ERR", 1}
            };

            var exportForQTOk = new Dictionary<string, int>()
            {
                {"ST", 1},
                {"M", 1},
                {"V", 1},
                {"OK", 1},
                {"P_MIN_V", 1},
                {"P_MAX_V", 1},
                {"P_CZ", 1},
            };

            return new object[]
            {
                new object[] {exportForQT, "QT", GetRandomQTDevice()},
                new object[] {exportForQTIOLink, "QT_IOLINK",
                    GetRandomQTDevice()},
                new object[] {exportForQTOk, "QT_OK",
                    GetRandomQTDevice()},
                new object[] {null, string.Empty, GetRandomQTDevice()},
                new object[] {null, "Incorrect", GetRandomQTDevice()},
                new object[] {null, "QT_VIRT", GetRandomQTDevice()},
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
                new object[] {$"_{2.0}..{4.0}", "QT", 2.0, 4.0,
                    GetRandomQTDevice()},
                new object[] {$"", "QT_IOLINK", 1.0, 3.0,
                    GetRandomQTDevice()},
                new object[] {$"_{1.0}..{3.0}", "QT_OK", 1.0, 3.0,
                    GetRandomQTDevice()},
                new object[] {string.Empty, string.Empty, 4.0, 8.0,
                    GetRandomQTDevice()},
                new object[] {string.Empty, "Incorrect", 7.0, 9.0,
                    GetRandomQTDevice()},
                new object[] {string.Empty, "QT_VIRT", 2.0, 3.0,
                    GetRandomQTDevice()},
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
                    "QT",
                    GetRandomQTDevice()
                },
                new object[]
                {
                    new string[] { "P_C0", "P_MIN_V", "P_MAX_V" },
                    "QT_OK",
                    GetRandomQTDevice()
                },
                new object[]
                {
                    new string[] { "P_ERR" },
                    "QT_IOLINK",
                    GetRandomQTDevice()
                },
                new object[]
                {
                    new string[0],
                    "QT_VIRT",
                    GetRandomQTDevice()
                },
                new object[]
                {
                    new string[0],
                    "Incorrect",
                    GetRandomQTDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomQTDevice()
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
                    "QT",
                    GetRandomQTDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 0 },
                        { "DI", 1 },
                        { "DO", 0 },
                    },
                    "QT_OK",
                    GetRandomQTDevice()
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
                    "QT_IOLINK",
                    GetRandomQTDevice()
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
                    "",
                    GetRandomQTDevice()
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
                    GetRandomQTDevice()
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
                    "QT_VIRT",
                    GetRandomQTDevice()
                },
            };
        }

        /// <summary>
        /// Генератор QT устройств
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetRandomQTDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.QT("KOAG4QT1", "+KOAG4-QT1",
                        "Test device", 1, "KOAG", 4, "Test article");
                case 2:
                    return new Device.QT("LINE1QT2", "+LINE1-QT2",
                        "Test device", 2, "LINE", 1, "Test article");
                case 3:
                    return new Device.QT("TANK2QT1", "+TANK2-QT1",
                        "Test device", 1, "TANK", 2, "Test article");
                default:
                    return new Device.QT("CW_TANK3QT3", "+CW_TANK3-QT3",
                        "Test device", 3, "CW_TANK", 3, "Test article");
            }
        }
    }
}
