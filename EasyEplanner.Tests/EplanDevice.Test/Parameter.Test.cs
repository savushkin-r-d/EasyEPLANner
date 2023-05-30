using NUnit.Framework;
using EplanDevice;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using static Tests.TechObject.DeviceManagerMock;

namespace Tests.EplanDevices
{
    public class ParameterTest
    {
        [SetUp]
        public void SetUpDevices()
        {
            var WT = new EplanDevice.WT("LINE1WT1", "+LINE1-WT1", "TEST WT",
                1, "LINE", 1, "Test WT article");
            WT.SetSubType("WT");
            EplanDevice.DeviceManager.GetInstance().Devices.Add(WT);

            var PID1 = new EplanDevice.C("LINE1PC1", "+LINE1-PC1", "TEST PC1",
                1, "LINE", 1);
            PID1.SetSubType("C_PID");
            PID1.SetProperty("IN_VALUE", "LINE1WT1");
            PID1.SetProperty("OUT_VALUE", "LINE1PC2");
            EplanDevice.DeviceManager.GetInstance().Devices.Add(PID1);

            var PID2 = new EplanDevice.C("LINE1PC2", "+LINE1-PC2", "TEST PC2",
                2, "LINE", 1);
            PID2.SetSubType("C_PID");
            PID2.SetProperty("IN_VALUE", "LINE1PC1");
            EplanDevice.DeviceManager.GetInstance().Devices.Add(PID2);
        }


        [TestCase("P_DT", 5, "", "5 мс")]
        [TestCase("P_C0", 1, "LINE1WT1", "1 кг")]
        [TestCase("P_max", 6, "LINE1PC1", "6 кг")]
        [TestCase("P_min", 2, "LINE1PC2", "2 кг")]
        [TestCase("", 2, "LINE1PC3", "2")]
        public void GetFormatTest(string parameter, object value,
            string devName, string expected)
        {
            var device = EplanDevice.DeviceManager.GetInstance().GetDevice(devName);
            var actual = IODevice.Parameter.GetFormatValue(parameter, value, device);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("P_DT", "Время порогового фильтра")]
        [TestCase("P_max", "Макс. входное значение")]
        public void GetDescription_CheckDescription(string parameterStr, string expectedDescription)
        {
            IODevice.Parameter parameter = parameterStr;
            Assert.AreEqual(expectedDescription, parameter.Description);
        }

        [TestCase("P_DT", "{0} мс")]
        [TestCase("P_max", "{0}")]
        public void GetFormat_CheckFormat(string parameterStr, string expectedFormat)
        {
            IODevice.Parameter parameter = parameterStr;
            Assert.AreEqual(expectedFormat, parameter.Format);
        }

        [TestCaseSource(nameof(GetToString_Cases))]
        public void GetToString_CheckCorrectParameterName(IODevice.Parameter parameter, string parameter_name)
        {
            Assert.AreEqual(parameter_name, $"{parameter}");
        }

        private static object[] GetToString_Cases = new object[]
        {
            new object[] { IODevice.Parameter.P_Td, "P_Td" },
            new object[] { IODevice.Parameter.P_acceleration_time, "P_acceleration_time" },
            new object[] { IODevice.Parameter.P_is_manual_mode, "P_is_manual_mode" },
        };
    }
}