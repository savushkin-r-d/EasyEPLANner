using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class VTest
    {
        const string Incorrect = "Incorrect";
        const string V_DO1 = "V_DO1";
        const string V_DO2 = "V_DO2";
        const string V_DO1_DI1_FB_OFF = "V_DO1_DI1_FB_OFF";
        const string V_DO1_DI1_FB_ON = "V_DO1_DI1_FB_ON";
        const string V_DO1_DI2 = "V_DO1_DI2";
        const string V_DO2_DI2 = "V_DO2_DI2";
        const string V_MIXPROOF = "V_MIXPROOF";
        const string V_AS_MIXPROOF = "V_AS_MIXPROOF";
        const string V_BOTTOM_MIXPROOF = "V_BOTTOM_MIXPROOF";
        const string V_AS_DO1_DI2 = "V_AS_DO1_DI2";
        const string V_DO2_DI2_BISTABLE = "V_DO2_DI2_BISTABLE";
        const string V_IOLINK_VTUG_DO1 = "V_IOLINK_VTUG_DO1";
        const string V_IOLINK_VTUG_DO1_FB_OFF = "V_IOLINK_VTUG_DO1_FB_OFF";
        const string V_IOLINK_VTUG_DO1_FB_ON = "V_IOLINK_VTUG_DO1_FB_ON";
        const string V_IOLINK_MIXPROOF = "V_IOLINK_MIXPROOF";
        const string V_IOLINK_DO1_DI2 = "V_IOLINK_DO1_DI2";
        const string V_IOLINK_VTUG_DO1_DI2 = "V_IOLINK_VTUG_DO1_DI2";
        const string V_VIRT = "V_VIRT";
        const string V_MINI_FLUSHING = "V_MINI_FLUSHING";
        const string V_IOL_TERMINAL_MIXPROOF_DO3 = "V_IOL_TERMINAL_MIXPROOF_DO3";   

        const string AI = IODevice.IOChannel.AI;
        const string AO = IODevice.IOChannel.AO;
        const string DI = IODevice.IOChannel.DI;
        const string DO = IODevice.IOChannel.DO;

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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { DeviceSubType.V_DO1, V_DO1,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_DO2, V_DO2,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_DO1_DI1_FB_OFF,
                    V_DO1_DI1_FB_OFF, GetRandomVDevice() },
                new object[] { DeviceSubType.V_DO1_DI1_FB_ON,
                    V_DO1_DI1_FB_ON, GetRandomVDevice() },
                new object[] { DeviceSubType.V_DO1_DI2, V_DO1_DI2,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_DO2_DI2, V_DO2_DI2,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_MIXPROOF, V_MIXPROOF,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_IOLINK_MIXPROOF,
                    V_IOLINK_MIXPROOF, GetRandomVDevice() },
                new object[] { DeviceSubType.V_AS_MIXPROOF,
                    V_AS_MIXPROOF, GetRandomVDevice() },
                new object[] { DeviceSubType.V_BOTTOM_MIXPROOF,
                    V_BOTTOM_MIXPROOF, GetRandomVDevice() },
                new object[] { DeviceSubType.V_AS_DO1_DI2,
                    V_AS_DO1_DI2, GetRandomVDevice() },
                new object[] { DeviceSubType.V_IOLINK_DO1_DI2,
                    V_IOLINK_DO1_DI2, GetRandomVDevice() },
                new object[] { DeviceSubType.V_DO2_DI2_BISTABLE,
                    V_DO2_DI2_BISTABLE, GetRandomVDevice() },
                new object[] { DeviceSubType.V_IOLINK_VTUG_DO1,
                    V_IOLINK_VTUG_DO1, GetRandomVDevice() },
                new object[] { DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF,
                    V_IOLINK_VTUG_DO1_FB_OFF, GetRandomVDevice() },
                new object[] { DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON,
                    V_IOLINK_VTUG_DO1_FB_ON, GetRandomVDevice() },
                new object[] {DeviceSubType.V_IOLINK_VTUG_DO1_DI2,
                    V_IOLINK_VTUG_DO1_DI2, GetRandomVDevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.NONE, string.Empty,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_VIRT, V_VIRT,
                    GetRandomVDevice() },
                new object[] { DeviceSubType.V_MINI_FLUSHING,
                    V_MINI_FLUSHING, GetRandomVDevice() },
                new object[] { DeviceSubType.V_IOL_TERMINAL_MIXPROOF_DO3, 
                    V_IOL_TERMINAL_MIXPROOF_DO3, GetRandomVDevice() },
            };
        }

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
                new object[] { V_DO1, V_DO1, GetRandomVDevice() },
                new object[] { V_DO2, V_DO2, GetRandomVDevice() },
                new object[] { V_DO1_DI1_FB_OFF, V_DO1_DI1_FB_OFF,
                    GetRandomVDevice() },
                new object[] { V_DO1_DI1_FB_ON, V_DO1_DI1_FB_ON,
                    GetRandomVDevice() },
                new object[] { V_DO1_DI2, V_DO1_DI2, GetRandomVDevice() },
                new object[] { V_DO2_DI2, V_DO2_DI2, GetRandomVDevice() },
                new object[] { V_MIXPROOF, V_MIXPROOF, GetRandomVDevice() },
                new object[] { V_IOLINK_MIXPROOF, V_IOLINK_MIXPROOF,
                    GetRandomVDevice() },
                new object[] { V_AS_MIXPROOF, V_AS_MIXPROOF,
                    GetRandomVDevice() },
                new object[] { V_BOTTOM_MIXPROOF, V_BOTTOM_MIXPROOF,
                    GetRandomVDevice() },
                new object[] { V_AS_DO1_DI2, V_AS_DO1_DI2,
                    GetRandomVDevice() },
                new object[] { V_IOLINK_DO1_DI2, V_IOLINK_DO1_DI2,
                    GetRandomVDevice() },
                new object[] { V_DO2_DI2_BISTABLE, V_DO2_DI2_BISTABLE,
                    GetRandomVDevice() },
                new object[] { V_IOLINK_VTUG_DO1, V_IOLINK_VTUG_DO1,
                    GetRandomVDevice() },
                new object[] { V_IOLINK_VTUG_DO1_FB_OFF,
                    V_IOLINK_VTUG_DO1_FB_OFF, GetRandomVDevice() },
                new object[] { V_IOLINK_VTUG_DO1_FB_ON,
                    V_IOLINK_VTUG_DO1_FB_ON, GetRandomVDevice() },
                new object[] { V_IOLINK_VTUG_DO1_DI2,
                    V_IOLINK_VTUG_DO1_DI2, GetRandomVDevice() },
                new object[] { V_IOL_TERMINAL_MIXPROOF_DO3,
                    V_IOL_TERMINAL_MIXPROOF_DO3, GetRandomVDevice() },
                new object[] { string.Empty, Incorrect, GetRandomVDevice() },
                new object[] { string.Empty, string.Empty, GetRandomVDevice() },
                new object[] { V_VIRT, V_VIRT, GetRandomVDevice() },
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
            var exportForV = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
            };

            var exportForVFBOff = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Parameter.P_ON_TIME, 1},
                {IODevice.Parameter.P_FB, 1},
                {IODevice.Tag.FB_OFF_ST, 1},
            };

            var exportForVFBOn = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Parameter.P_ON_TIME, 1},
                {IODevice.Parameter.P_FB, 1},
                {IODevice.Tag.FB_OFF_ST, 1},
                {IODevice.Tag.FB_ON_ST, 1},
            };

            var exportForVIOLinkMixproof = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Parameter.P_ON_TIME, 1},
                {IODevice.Parameter.P_FB, 1},
                {IODevice.Tag.V, 1},
                {IODevice.Tag.BLINK, 1},
                {IODevice.Tag.CS, 1},
                {IODevice.Tag.ERR, 1},
            };

            var exportForVVirt = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.V, 1},
            };

            return new object[]
            {
                new object[] {exportForV, V_DO1, GetRandomVDevice()},
                new object[] {exportForV, V_DO2, GetRandomVDevice()},
                new object[] {exportForV, V_IOLINK_VTUG_DO1,
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, V_DO1_DI1_FB_ON,
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, V_IOLINK_VTUG_DO1_FB_ON,
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, V_DO1_DI1_FB_OFF,
                    GetRandomVDevice()},
                new object[] {exportForVFBOff, V_IOLINK_VTUG_DO1_FB_OFF,
                    GetRandomVDevice()},
                new object[] {exportForVFBOn, V_DO1_DI2, GetRandomVDevice()},
                new object[] {exportForVFBOn, V_DO2_DI2, GetRandomVDevice()},
                new object[] {exportForVFBOn, V_DO2_DI2_BISTABLE,
                    GetRandomVDevice()},
                new object[] {exportForVFBOn, V_MIXPROOF, GetRandomVDevice()},
                new object[] {exportForVFBOn, V_AS_MIXPROOF,
                    GetRandomVDevice()},
                new object[] {exportForVFBOn, V_MIXPROOF, GetRandomVDevice()},
                new object[] {exportForVFBOn, V_BOTTOM_MIXPROOF,
                    GetRandomVDevice()},
                new object[] {exportForVFBOn, V_IOLINK_VTUG_DO1_DI2,
                    GetRandomVDevice()},
                new object[] {exportForVIOLinkMixproof, V_IOLINK_MIXPROOF,
                    GetRandomVDevice()},
                new object[] {exportForVIOLinkMixproof, V_IOLINK_DO1_DI2,
                    GetRandomVDevice()},
                new object[] { exportForV, V_IOL_TERMINAL_MIXPROOF_DO3,
                    GetRandomVDevice()},
                new object[] {null, Incorrect, GetRandomVDevice()},
                new object[] {null, string.Empty, GetRandomVDevice()},
                new object[] {exportForVVirt, V_VIRT, GetRandomVDevice()},
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
            var parameters = new string[]
            { 
                IODevice.Parameter.P_ON_TIME,
                IODevice.Parameter.P_FB
            };

            return new object[]
            {
                new object[]
                {
                    new string[0],
                    V_DO1,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_DO2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_DO1_DI1_FB_OFF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_DO2_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_DO2_DI2_BISTABLE,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_IOLINK_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_AS_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_BOTTOM_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_AS_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_IOLINK_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_IOLINK_VTUG_DO1,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_IOLINK_VTUG_DO1_FB_OFF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_IOLINK_VTUG_DO1_FB_ON,
                    GetRandomVDevice()
                },
                new object[]
                {
                    parameters,
                    V_IOLINK_VTUG_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_VIRT,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_IOL_TERMINAL_MIXPROOF_DO3,
                    GetRandomVDevice()
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
            var emptySignals = new Dictionary<string, int>()
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
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 0 },
                        { DO, 1 },
                    },
                    V_DO1,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 0 },
                        { DO, 2 },
                    },
                    V_DO2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 1 },
                        { DO, 1 },
                    },
                    V_DO1_DI1_FB_OFF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 2 },
                        { DO, 1 },
                    },
                    V_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 2 },
                        { DO, 2 },
                    },
                    V_DO2_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 2 },
                        { DO, 2 },
                    },
                    V_DO2_DI2_BISTABLE,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 2 },
                        { DO, 3 },
                    },
                    V_MIXPROOF,
                    GetRandomVDevice()
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
                    V_IOLINK_MIXPROOF,
                    GetRandomVDevice()
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
                    V_AS_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 0 },
                        { DI, 2 },
                        { DO, 3 },
                    },
                    V_BOTTOM_MIXPROOF,
                    GetRandomVDevice()
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
                    V_AS_DO1_DI2,
                    GetRandomVDevice()
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
                    V_IOLINK_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    V_IOLINK_VTUG_DO1,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 1 },
                        { DO, 0 },
                    },
                    V_IOLINK_VTUG_DO1_FB_OFF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 1 },
                        { DO, 0 },
                    },
                    V_IOLINK_VTUG_DO1_FB_ON,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 1 },
                        { DI, 2 },
                        { DO, 0 },
                    },
                    V_IOLINK_VTUG_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new Dictionary<string, int>()
                    {
                        { AI, 0 },
                        { AO, 3 },
                        { DI, 0 },
                        { DO, 0 },
                    },
                    V_IOL_TERMINAL_MIXPROOF_DO3,
                    GetRandomVDevice()
                },
                new object[]
                {
                    emptySignals,
                    string.Empty,
                    GetRandomVDevice()
                },
                new object[]
                {
                    emptySignals,
                    Incorrect,
                    GetRandomVDevice()
                },
                new object[]
                {
                    emptySignals,
                    V_VIRT,
                    GetRandomVDevice()
                },
            };
        }

        /// <summary>
        /// Тестирование свойств устройства
        /// </summary>
        /// <param name="expectedProperties">Ожидаемые свойства</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(RuntimeParametersTestData))]
        public void RuntimeParameters_NewDev_ReturnsExpectedRuntimeParameters(
            string[] expectedProperties, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            string[] actualSequence = device.RuntimeParameters
                .Select(x => x.Key)
                .ToArray();
            Assert.AreEqual(expectedProperties, actualSequence);
        }

        /// <summary>
        /// 1 - Параметры в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] RuntimeParametersTestData()
        {
            var vtugParameters = new string[]
            {
                IODevice.RuntimeParameter.R_VTUG_NUMBER,
                IODevice.RuntimeParameter.R_VTUG_SIZE,
            };

            var interfaceASiParameters = new string[]
            {
                IODevice.RuntimeParameter.R_AS_NUMBER,
            };

            var V_IOL_TERMINAL_MIXPROOF_DO3_RTParameters = new string[]
            {
                IODevice.RuntimeParameter.R_ID_ON,
                IODevice.RuntimeParameter.R_ID_UPPER_SEAT,
                IODevice.RuntimeParameter.R_ID_LOWER_SEAT,
            };

            return new object[]
            {
                new object[]
                {
                    new string[0],
                    V_DO1,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_DO2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_DO1_DI1_FB_OFF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_DO2_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_DO2_DI2_BISTABLE,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_IOLINK_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    interfaceASiParameters,
                    V_AS_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_BOTTOM_MIXPROOF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    interfaceASiParameters,
                    V_AS_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_IOLINK_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    vtugParameters,
                    V_IOLINK_VTUG_DO1,
                    GetRandomVDevice()
                },
                new object[]
                {
                    vtugParameters,
                    V_IOLINK_VTUG_DO1_FB_OFF,
                    GetRandomVDevice()
                },
                new object[]
                {
                    vtugParameters,
                    V_IOLINK_VTUG_DO1_FB_ON,
                    GetRandomVDevice()
                },
                new object[]
                {
                    vtugParameters,
                    V_IOLINK_VTUG_DO1_DI2,
                    GetRandomVDevice()
                },
                new object[]
                {
                    V_IOL_TERMINAL_MIXPROOF_DO3_RTParameters,
                    V_IOL_TERMINAL_MIXPROOF_DO3,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    string.Empty,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomVDevice()
                },
                new object[]
                {
                    new string[0],
                    V_VIRT,
                    GetRandomVDevice()
                },
            };
        }

        /// <summary>
        /// Генератор V устройств
        /// </summary>
        /// <returns></returns>
        public static IODevice GetRandomVDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new V("KOAG4V1", "+KOAG4-V1",
                        "Test device", 1, "KOAG", 4, "DeviceArticle");
                case 2:
                    return new V("LINE1V2", "+LINE1-V2",
                        "Test device", 2, "LINE", 1, "DeviceArticle");
                case 3:
                    return new V("TANK2V1", "+TANK2-V1",
                        "Test device", 1, "TANK", 2, "DeviceArticle");
                default:
                    return new V("CW_TANK3V3", "+CW_TANK3-V3",
                        "Test device", 3, "CW_TANK", 3, "DeviceArticle");
            }
        }
    }
}
