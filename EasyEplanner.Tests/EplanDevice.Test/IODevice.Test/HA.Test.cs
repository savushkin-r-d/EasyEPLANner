﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class HATest
    {
        const string Incorrect = "Incorrect";
        const string HA = "HA";
        const string HA_VIRT = "HA_VIRT";

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
                new object[] { HA, string.Empty, GetRandomHADevice() },
                new object[] { string.Empty, Incorrect, GetRandomHADevice() },
                new object[] { HA, HA, GetRandomHADevice() },
                new object[] { HA_VIRT, HA_VIRT, GetRandomHADevice() },
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
                new object[] { DeviceSubType.HA, string.Empty,
                    GetRandomHADevice() },
                new object[] { DeviceSubType.HA, HA,
                    GetRandomHADevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomHADevice() },
                new object[] { DeviceSubType.HA_VIRT, HA_VIRT,
                    GetRandomHADevice() },
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
            var exportForHA = new Dictionary<ITag, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
            };

            return new object[]
            {
                new object[] {exportForHA, string.Empty, GetRandomHADevice()},
                new object[] {exportForHA, HA, GetRandomHADevice()},
                new object[] {null, Incorrect, GetRandomHADevice()},
                new object[] {exportForHA, HA_VIRT, GetRandomHADevice()},
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
            return new object[]
            {
                new object[]
                {
                    new string[0],
                    HA,
                    GetRandomHADevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomHADevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomHADevice()
                },
                new object[]
                {
                    new string[0],
                    HA_VIRT,
                    GetRandomHADevice()
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
            var discreteSirenChannels = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 0 },
                { DO, 1 },
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
                    discreteSirenChannels,
                    HA,
                    GetRandomHADevice()
                },
                new object[]
                {
                    discreteSirenChannels,
                    string.Empty,
                    GetRandomHADevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomHADevice()
                },
                new object[]
                {
                    emptyChannels,
                    HA_VIRT,
                    GetRandomHADevice()
                }
            };
        }

        /// <summary>
        /// Генератор HA устройств
        /// </summary>
        /// <returns></returns>
        private static IODevice GetRandomHADevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new HA("KOAG4HA1", "+KOAG4-HA1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new HA("LINE1HA2", "+LINE1-HA2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new HA("TANK2HA1", "+TANK2-HA1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new HA("CW_TANK3HA3", "+CW_TANK3-HA3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
