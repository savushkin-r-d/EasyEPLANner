using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class VTest
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
                new object[] { Device.DeviceSubType.V_DO1, "V_DO1", 
                    GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_DO2, "V_DO2", 
                    GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_DO1_DI1_FB_OFF,
                    "V_DO1_DI1_FB_OFF", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_DO1_DI1_FB_ON, 
                    "V_DO1_DI1_FB_ON", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_DO1_DI2, "V_DO1_DI2", 
                    GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_DO2_DI2, "V_DO2_DI2", 
                    GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_MIXPROOF, "V_MIXPROOF", 
                    GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_IOLINK_MIXPROOF, 
                    "V_IOLINK_MIXPROOF", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_AS_MIXPROOF,
                    "V_AS_MIXPROOF", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_BOTTOM_MIXPROOF,
                    "V_BOTTOM_MIXPROOF", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_AS_DO1_DI2,
                    "V_AS_DO1_DI2", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_IOLINK_DO1_DI2, 
                    "V_IOLINK_DO1_DI2", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_DO2_DI2_BISTABLE, 
                    "V_DO2_DI2_BISTABLE", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_IOLINK_VTUG_DO1, 
                    "V_IOLINK_VTUG_DO1", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF, 
                    "V_IOLINK_VTUG_DO1_FB_OFF", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON,
                    "V_IOLINK_VTUG_DO1_FB_ON", GetRandomVDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect", 
                    GetRandomVDevice() },
                new object[] { Device.DeviceSubType.NONE, "",
                    GetRandomVDevice() },
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
                new object[] { "V_DO1", "V_DO1", GetRandomVDevice() },
                new object[] { "V_DO2", "V_DO2", GetRandomVDevice() },
                new object[] { "V_DO1_DI1_FB_OFF", "V_DO1_DI1_FB_OFF", 
                    GetRandomVDevice() },
                new object[] { "V_DO1_DI1_FB_ON", "V_DO1_DI1_FB_ON", 
                    GetRandomVDevice() },
                new object[] { "V_DO1_DI2", "V_DO1_DI2", GetRandomVDevice() },
                new object[] { "V_DO2_DI2", "V_DO2_DI2", GetRandomVDevice() },
                new object[] { "V_MIXPROOF", "V_MIXPROOF", GetRandomVDevice() },
                new object[] { "V_IOLINK_MIXPROOF", "V_IOLINK_MIXPROOF", 
                    GetRandomVDevice() },
                new object[] { "V_AS_MIXPROOF", "V_AS_MIXPROOF", 
                    GetRandomVDevice() },
                new object[] { "V_BOTTOM_MIXPROOF", "V_BOTTOM_MIXPROOF", 
                    GetRandomVDevice() },
                new object[] { "V_AS_DO1_DI2", "V_AS_DO1_DI2", 
                    GetRandomVDevice() },
                new object[] { "V_IOLINK_DO1_DI2", "V_IOLINK_DO1_DI2", 
                    GetRandomVDevice() },
                new object[] { "V_DO2_DI2_BISTABLE", "V_DO2_DI2_BISTABLE", 
                    GetRandomVDevice() },
                new object[] { "V_IOLINK_VTUG_DO1", "V_IOLINK_VTUG_DO1", 
                    GetRandomVDevice() },
                new object[] { "V_IOLINK_VTUG_DO1_FB_OFF", 
                    "V_IOLINK_VTUG_DO1_FB_OFF", GetRandomVDevice() },
                new object[] { "V_IOLINK_VTUG_DO1_FB_ON",
                    "V_IOLINK_VTUG_DO1_FB_ON", GetRandomVDevice() },
                new object[] { "", "Incorrect", GetRandomVDevice() },
                new object[] { "", "", GetRandomVDevice() },
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
            var exportForV = new List<string>
            {
                "ST",
                "M"
            };


            var exportForVFBOff = new List<string>
            {
                "ST",
                "M",
                "P_ON_TIME",
                "P_FB",
                "FB_OFF_ST"
            };

            var exportForVFBOn = new List<string>
            {
                "ST",
                "M",
                "P_ON_TIME",
                "P_FB",
                "FB_OFF_ST",
                "FB_ON_ST"
            };

            var exportForVIOLinkMixproof = new List<string>
            {
                "ST",
                "M",
                "P_ON_TIME",
                "P_FB",
                "V",
                "BLINK",
                "CS",
                "ERR",
            };

            return new object[]
            {
                new object[] {exportForV, "V_DO1", GetRandomVDevice()},
                new object[] {exportForV, "V_DO2", GetRandomVDevice()},
                new object[] {exportForV, "V_IOLINK_VTUG_DO1", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, "V_DO1_DI1_FB_ON", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, "V_IOLINK_VTUG_DO1_FB_ON", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, "V_DO1_DI1_FB_OFF", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, "V_IOLINK_VTUG_DO1_FB_OFF", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, "FQT_VIRT", GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_DO1_DI2", GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_DO2_DI2", GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_DO2_DI2_BISTABLE", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_MIXPROOF", GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_AS_MIXPROOF", 
                    GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_MIXPROOF", GetRandomVDevice()},
                new object[] {exportForVFBOn, "V_BOTTOM_MIXPROOF", 
                    GetRandomVDevice()},
                new object[] {exportForVIOLinkMixproof, "V_IOLINK_MIXPROOF", 
                    GetRandomVDevice()},
                new object[] {exportForVIOLinkMixproof, "V_IOLINK_DO1_DI2", 
                    GetRandomVDevice()},
                new object[] {null, "Incorrect", GetRandomVDevice()},
                new object[] {null, "", GetRandomVDevice()},
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
                    new string[0],
                    "V_DO1",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    "V_DO2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_DO1_DI1_FB_OFF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_DO1_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_DO2_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_DO2_DI2_BISTABLE",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_IOLINK_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_AS_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_BOTTOM_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_AS_DO1_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_IOLINK_DO1_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    "V_IOLINK_VTUG_DO1",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_IOLINK_VTUG_DO1_FB_OFF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[] { "P_ON_TIME", "P_FB" },
                    "V_IOLINK_VTUG_DO1_FB_ON",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    "",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    "Incorrect",
                    GetRandomVDevice()
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
                        { "DI", 0 },
                        { "DO", 1 },
                    },
                    "V_DO1",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 0 },
                        { "DO", 2 },
                    },
                    "V_DO2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 1 },
                        { "DO", 1 },
                    },
                    "V_DO1_DI1_FB_OFF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 2 },
                        { "DO", 1 },
                    },
                    "V_DO1_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 2 },
                        { "DO", 2 },
                    },
                    "V_DO2_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 2 },
                        { "DO", 2 },
                    },
                    "V_DO2_DI2_BISTABLE",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 2 },
                        { "DO", 3 },
                    },
                    "V_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 1 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "V_IOLINK_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 1 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "V_AS_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 0 },
                        { "DI", 2 },
                        { "DO", 3 },
                    },
                    "V_BOTTOM_MIXPROOF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 1 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "V_AS_DO1_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 1 },
                        { "AO", 1 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "V_IOLINK_DO1_DI2",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 0 },
                        { "DO", 0 },
                    },
                    "V_IOLINK_VTUG_DO1",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 1 },
                        { "DO", 0 },
                    },
                    "V_IOLINK_VTUG_DO1_FB_OFF",
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { "AI", 0 },
                        { "AO", 1 },
                        { "DI", 1 },
                        { "DO", 0 },
                    },
                    "V_IOLINK_VTUG_DO1_FB_ON",
                    GetRandomVDevice()
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
                    GetRandomVDevice()
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
                    GetRandomVDevice()
                },
            };
        }

        /// <summary>
        /// Генератор V устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomVDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.V("KOAG4V1", "Test device", 1,
                        "KOAG", 4, "DeviceArticle");
                case 2:
                    return new Device.V("LINE1V2", "Test device", 2,
                        "LINE", 1, "DeviceArticle");
                case 3:
                    return new Device.V("TANK2V1", "Test device", 1,
                        "TANK", 2, "DeviceArticle");
                default:
                    return new Device.V("CW_TANK3V3", "Test device", 3,
                        "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
