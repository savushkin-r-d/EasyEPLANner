using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class DITest
    {
        [TestCaseSource(nameof(SetSubTypeTestData))]
        public void SetSubTypeTest(Device.DeviceSubType expectedSubType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedSubType, device.DeviceSubType);
        }

        [TestCaseSource(nameof(GetSubTypeTestData))]
        public void GetDeviceSubTypeStrTest(string expectedType,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedType, device.GetDeviceSubTypeStr(
                device.DeviceType, device.DeviceSubType));
        }

        [TestCaseSource(nameof(GetDevicePropertiesTestData))]
        public void GetDevicePropertiesTest(List<string> expectedProperties,
            string subType, Device.IODevice device)
        {
            device.SetSubType(subType);
            Assert.AreEqual(expectedProperties, device.GetDeviceProperties(
                device.DeviceType, device.DeviceSubType));
        }

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
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] SetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { Device.DeviceSubType.DI, "",
                    GetRandomDIDevice() },
                new object[] { Device.DeviceSubType.DI, "DI",
                    GetRandomDIDevice() },
                new object[] { Device.DeviceSubType.DI_VIRT, "DI_VIRT",
                    GetRandomDIDevice() },
                new object[] { Device.DeviceSubType.NONE, "Incorrect",
                    GetRandomDIDevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемое значение подтипа,
        /// 2 - Задаваемое значение подтипа,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] GetSubTypeTestData()
        {
            return new object[]
            {
                new object[] { "DI", "", GetRandomDIDevice() },
                new object[] { "DI", "DI", GetRandomDIDevice() },
                new object[] { "DI_VIRT", "DI_VIRT", GetRandomDIDevice() },
                new object[] { "", "Incorrect", GetRandomDIDevice() },
            };
        }

        /// <summary>
        /// 1 - Ожидаемый список свойств для экспорта,
        /// 2 - Задаваемый подтип устройства,
        /// 3 - Устройство для тестов
        /// </summary>
        /// <returns></returns>
        public static object[] GetDevicePropertiesTestData()
        {
            var exportForDI = new List<string>
            {
                "ST", 
                "M", 
                "P_DT"
            };

            var exportForVirtDI = new List<string>
            {
                "ST", 
                "M"
            };

            return new object[]
            {
                new object[] {exportForDI, "", GetRandomDIDevice()},
                new object[] {exportForDI, "DI", GetRandomDIDevice()},
                new object[] {exportForVirtDI, "DI_VIRT", GetRandomDIDevice()},
                new object[] {null, "Incorrect", GetRandomDIDevice()},
            };
        }

        /// <summary>
        /// 1 - Параметры в том порядке, который нужен
        /// 2 - Подтип устройства
        /// 3 - Устройство
        /// </summary>
        /// <returns></returns>
        public static object[] ParametersTestData()
        {
            return new object[]
            {
                new object[]
                {
                    new string[] { "P_DT" },
                    "DI",
                    GetRandomDIDevice()
                },
                new object[]
                {
                    new string[] { "P_DT" },
                    "",
                    GetRandomDIDevice()
                },
                new object[]
                {
                    new string[0],
                    "DI_VIRT",
                    GetRandomDIDevice()
                },
            };
        }

        /// <summary>
        /// Генератор DI устройств
        /// </summary>
        /// <returns></returns>
        public static Device.IODevice GetRandomDIDevice()
        {
            var randomizer = new Random();
            int value = randomizer.Next(1, 3);
            switch (value)
            {
                case 1:
                    return new Device.DI("KOAG4DI1", "Test device", 1,
                        "KOAG", 4);
                case 2:
                    return new Device.DI("LINE1DI2", "Test device", 2,
                        "LINE", 1);
                case 3:
                    return new Device.DI("TANK2DI1", "Test device", 1,
                        "TANK", 2);
                default:
                    return new Device.DI("CW_TANK3DI3", "Test device", 3,
                        "CW_TANK", 3);
            }
        }
    }
}
