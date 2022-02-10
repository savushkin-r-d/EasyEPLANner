using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EplanDevice;

namespace Tests.EplanDevices
{
    public class HLATest
    {
        const string Incorrect = "Incorrect";
        const string HLA = "HLA";
        const string HLA_VIRT = "HLA_VIRT";
        const string HLA_IOLINK = "HLA_IOLINK";

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
                new object[] { DeviceSubType.HLA, HLA,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.HLA_VIRT, HLA_VIRT,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.HLA, string.Empty,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.NONE, Incorrect,
                    GetRandomHLADevice() },
                new object[] { DeviceSubType.HLA_IOLINK, HLA_IOLINK,
                    GetRandomHLADevice() },
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
                new object[] { HLA, string.Empty, GetRandomHLADevice() },
                new object[] { HLA, HLA, GetRandomHLADevice() },
                new object[] { HLA_VIRT, HLA_VIRT, GetRandomHLADevice() },
                new object[] { HLA_IOLINK, HLA_IOLINK, GetRandomHLADevice() },
                new object[] { string.Empty, Incorrect, GetRandomHLADevice() },
            };
        }

        /// <summary>
        /// Тест свойств устройств без свойства последовательности сигналов
        /// </summary>
        /// <param name="expectedProperties">Ожидаемый список свойств</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDevicePropertiesNoSignalsSequenceData))]
        public void GetDeviceProperties_NoSignalsSequence_ReturnsProperties(
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
        private static object[] GetDevicePropertiesNoSignalsSequenceData()
        {
            var exportForHLA = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
                {IODevice.Tag.L_RED, 1 },
                {IODevice.Tag.L_YELLOW, 1 },
                {IODevice.Tag.L_GREEN, 1 },
                {IODevice.Tag.L_SIREN, 1 }
            };

            return new object[]
            {
                new object[] {exportForHLA, string.Empty, GetRandomHLADevice()},
                new object[] {exportForHLA, HLA, GetRandomHLADevice()},
                new object[] {null, Incorrect, GetRandomHLADevice()},
            };
        }

        /// <summary>
        /// Тест свойств устройств со свойством последовательности сигналов
        /// </summary>
        /// <param name="expectedProperties">Ожидаемый список свойств</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        /// <param name="sequenceValue">Последовательность сигналов</param>
        [TestCaseSource(nameof(GetDevicePropertiesWithoutSequenceSignalsData))]
        public void GetDeviceProperties_HasSignalsSequence_ReturnsProperties(
            Dictionary<string, int> expectedProperties, string subType,
            IODevice device, string sequenceValue)
        {
            string signalsSequenceName = IODevice.Property.SIGNALS_SEQUENCE;
            device.SetSubType(subType);
            device.SetProperty(signalsSequenceName, sequenceValue);

            Dictionary<string, int> actualDeviceProperties = device
                .GetDeviceProperties(device.DeviceType, device.DeviceSubType);

            Assert.AreEqual(expectedProperties, actualDeviceProperties);
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        private static object[] GetDevicePropertiesWithoutSequenceSignalsData()
        {

            var defaultProps = new Dictionary<string, int>()
            {
                {IODevice.Tag.ST, 1},
                {IODevice.Tag.M, 1},
            };

            var sirenProps = new Dictionary<string, int>(defaultProps);
            sirenProps.Add(IODevice.Tag.L_SIREN, 1);

            var sirenBlueProps = new Dictionary<string, int>(sirenProps);
            sirenBlueProps.Add(IODevice.Tag.L_BLUE, 1);

            var sirenBlueGreenProps =
                new Dictionary<string, int>(sirenBlueProps);
            sirenBlueGreenProps.Add(IODevice.Tag.L_GREEN, 1);

            var sirenBlueGreenYellowProps =
                new Dictionary<string, int>(sirenBlueGreenProps);
            sirenBlueGreenYellowProps.Add(IODevice.Tag.L_YELLOW, 1);

            var sirenBluGreenYellowRedProps =
                new Dictionary<string, int>(sirenBlueGreenYellowProps);
            sirenBluGreenYellowRedProps.Add(IODevice.Tag.L_RED, 1);

            string emptySequence = string.Empty;
            string siren = "A";
            string sirenBlue = siren + "B";
            string sirenBlueGreen = sirenBlue + "G";
            string sirenBlueGreenYellow = sirenBlueGreen + "Y";
            string sirenBlueGreenYellowRed = sirenBlueGreenYellow + "R";
            string wrongSequence = "BlahBlah";
            string sirenBlueButWrongSequence = "AKKKB";

            return new object[]
            {
                new object[]
                {
                    defaultProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    emptySequence
                },
                new object[]
                {
                    defaultProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    emptySequence
                },
                new object[]
                {
                    defaultProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    wrongSequence
                },
                new object[]
                {
                    defaultProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    wrongSequence
                },
                new object[]
                {
                    sirenProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    siren
                },
                new object[]
                {
                    sirenProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    siren
                },
                new object[]
                {
                    sirenBlueProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    sirenBlue
                },
                new object[]
                {
                    sirenBlueProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    sirenBlue
                },
                new object[]
                {
                    sirenBlueProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    sirenBlueButWrongSequence
                },
                new object[]
                {
                    sirenBlueProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    sirenBlueButWrongSequence
                },
                new object[]
                {
                    sirenBlueGreenProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    sirenBlueGreen
                },
                new object[]
                {
                    sirenBlueGreenProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    sirenBlueGreen
                },
                new object[]
                {
                    sirenBlueGreenYellowProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    sirenBlueGreenYellow
                },
                new object[]
                {
                    sirenBlueGreenYellowProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    sirenBlueGreenYellow
                },
                new object[]
                {
                    sirenBluGreenYellowRedProps,
                    HLA_VIRT,
                    GetRandomHLADevice(),
                    sirenBlueGreenYellowRed
                },
                new object[]
                {
                    sirenBluGreenYellowRedProps,
                    HLA_IOLINK,
                    GetRandomHLADevice(),
                    sirenBlueGreenYellowRed
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
            var discreteHLAChannels = new Dictionary<string, int>()
            {
                { AI, 0 },
                { AO, 0 },
                { DI, 0 },
                { DO, 4 },
            };

            var ioLinkHLAChannels = new Dictionary<string, int>()
            {
                { AI, 1 },
                { AO, 1 },
                { DI, 0 },
                { DO, 0 }
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
                    discreteHLAChannels,
                    HLA,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    discreteHLAChannels,
                    string.Empty,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    emptyChannels,
                    HLA_VIRT,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    ioLinkHLAChannels,
                    HLA_IOLINK,
                    GetRandomHLADevice()
                }
            };
        }

        /// <summary>
        /// Тестирование рабочих свойств устройства
        /// </summary>
        /// <param name="expectedProperties">Ожидаемые свойства</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(RuntimeParametersTestData))]
        public void RuntimeParameters_NewDev_ReturnsExpectedProperties(
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
        /// 1 - Рабочие свойства в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] RuntimeParametersTestData()
        {
            var discreteHLARuntimeProperties = new string[]
            {
               IODevice.RuntimeParameter.R_CONST_RED
            };

            return new object[]
            {
                new object[]
                {
                    discreteHLARuntimeProperties,
                    HLA,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    discreteHLARuntimeProperties,
                    string.Empty,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new string[0],
                    Incorrect,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    new string[0],
                    HLA_VIRT,
                    GetRandomHLADevice()
                },
            };
        }

        /// <summary>
        /// Тестирование свойств устройства
        /// </summary>
        /// <param name="expectedProperties">Ожидаемые свойства</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(PropertiesParametersTestData))]
        public void Properties_NewDev_ReturnsExpectedProperties(
            string[] expectedProperties, string subType,
            IODevice device)
        {
            device.SetSubType(subType);
            string[] actualSequence = device.Properties
                .Select(x => x.Key)
                .ToArray();
            Assert.AreEqual(expectedProperties, actualSequence);
        }

        /// <summary>
        /// 1 - Свойства в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        private static object[] PropertiesParametersTestData()
        {
            var ioLinkProperties = new string[]
            {
               IODevice.Property.SIGNALS_SEQUENCE
            };

            var noProperties = new string[0];

            return new object[]
            {
                new object[]
                {
                    noProperties,
                    HLA,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    noProperties,
                    string.Empty,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    noProperties,
                    Incorrect,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    ioLinkProperties,
                    HLA_VIRT,
                    GetRandomHLADevice()
                },
                new object[]
                {
                    ioLinkProperties,
                    HLA_IOLINK,
                    GetRandomHLADevice()
                },
            };
        }

        [Test]
        public void ArticleName_PutArticleNameAnArgument_CorrectSetAndGet()
        {
            string expectedArticle = "articleArticle";

            var device = GetRandomHLADevice(expectedArticle);

            Assert.AreEqual(expectedArticle, device.ArticleName);
        }

        /// <summary>
        /// Генератор HL устройств
        /// </summary>
        /// <param name="articleName">Изделие устройства</param>
        /// <returns></returns>
        private static IODevice GetRandomHLADevice(string articleName = "")
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new HLA("KOAG4HL1", "+KOAG4-HL1",
                        "Test device", 1, "KOAG", 4, articleName);
                case 2:
                    return new HLA("LINE1HL2", "+LINE1-HL2",
                        "Test device", 2, "LINE", 1, articleName);
                case 3:
                    return new HLA("TANK2HL1", "+TANK2-HL1",
                        "Test device", 1, "TANK", 2, articleName);
                default:
                    return new HLA("CW_TANK3HL3", "+CW_TANK3-HL3",
                        "Test device", 3, "CW_TANK", 3, articleName);
            }
        }
    }
}
