using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests
{
    public class RTest
    {
        /// <summary>
        /// Тест получения подтипа устройства
        /// </summary>
        /// <param name="expectedType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDeviceSubTypeStrTestData))]
        public void GetDeviceSubTypeStr_NewObject_ReturnsDevType(
            string expectedType, string subType, Device.IODevice device)
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
                new object[] { "R", "", GetNewRDevice() },
                new object[] { "R", "R", GetNewRDevice() },
                new object[] { "R", "Random", GetNewRDevice() },
                new object[] { "R", "Incorrect", GetNewRDevice() },
            };
        }

        /// <summary>
        /// Тест свойств устройств в зависимости от подтипа
        /// </summary>
        /// <param name="expectedProperties">Ожидаемый список свойств</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDevicePropertiesTestData))]
        public void GetDeviceProperties_NewObj_ReturnsTagsArr(
            Dictionary<string, int> expectedProperties, string subType,
            Device.IODevice device)
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
            var exportTags = new Dictionary<string, int>()
            {
                {"ST", 1},
                {"M", 1},
                {"V", 1},
                {"Z", 1}
            };

            return new object[]
            {
                new object[] { exportTags, "R", GetNewRDevice() },
                new object[] { exportTags, "Random", GetNewRDevice() },
                new object[] { exportTags, "Incorrect", GetNewRDevice() },
            };
        }

        /// <summary>
        /// Тестирование параметров устройства
        /// </summary>
        /// <param name="parametersSequence">Ожидаемые параметры</param>
        /// <param name="defaultValuesSequence">Стандартные значения</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(ParametersTestData))]
        public void Parameters_NewObject_ReturnsParametersArrWithDefaultValues(
            string[] parametersSequence, double[] defaultValuesSequence,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            string[] actualParametersSequence = device.Parameters
                .Select(x => x.Key)
                .ToArray();
            double[] actualDefaultValuesSequence = device.Parameters
                .Select(x => Convert.ToDouble(x.Value))
                .ToArray();

            Assert.AreEqual(parametersSequence, actualParametersSequence);
            Assert.AreEqual(defaultValuesSequence, actualDefaultValuesSequence);
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
                "P_k", "P_Ti", "P_Td", "P_dt","P_max",
                "P_min", "P_acceleration_time", "P_is_manual_mode",
                "P_U_manual", "P_k2", "P_Ti2", "P_Td2", "P_out_max",
                "P_out_min", "P_is_reverse", "P_is_zero_start"
            };

            var defaultValues = new double[]
            {
                1, 15, 0.01, 1000, 100, 0, 30, 0, 65, 0, 0, 0, 100, 0, 0, 1
            };

            return new object[]
            {
                new object[]
                {
                    parameters,
                    defaultValues,
                    "R",
                    GetNewRDevice()
                },
                new object[]
                {
                    parameters,
                    defaultValues,
                    "",
                    GetNewRDevice()
                },
                new object[]
                {
                    parameters,
                    defaultValues,
                    "Incorrect",
                    GetNewRDevice()
                },
            };
        }

        [Test]
        public void Properties_NewObject_ReturnsPropertiesArrWithNullValues()
        {
            const int expectedPropertiesCount = 2;
            string[] expectedPropertiesNames = 
                new string[] {"IN_VALUE", "OUT_VALUE" };
            var dev = GetNewRDevice();
            var properties = dev.Properties;

            bool allNull = properties.All(x => x.Value == null);
            Assert.IsTrue(allNull);
            Assert.AreEqual(expectedPropertiesCount, properties.Count);

            foreach(var propName in expectedPropertiesNames)
            {
                Assert.IsTrue(properties.Keys.Contains(propName));
            }
        }

        [Test]
        public void DefaultConstructor_NewObject_ReturnsCorrectTypeAndSubType()
        {
            var dev = GetNewRDevice();

            Assert.AreEqual(Device.DeviceType.R, dev.DeviceType);
            Assert.AreEqual(Device.DeviceSubType.NONE, dev.DeviceSubType);
        }

        [Test]
        public void Channels_NewObject_ReturnsEmptyChannels()
        {
            var dev = GetNewRDevice();

            Assert.AreEqual(0, dev.Channels.Count);
        }

        /// <summary>
        /// Создать новое R устройство
        /// </summary>
        /// <returns></returns>
        private static Device.IODevice GetNewRDevice()
        {
            return new Device.R("TANK1R1", "PID", 1,  "TANK", 1);
        }
    }
}
