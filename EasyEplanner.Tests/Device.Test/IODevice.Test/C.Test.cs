using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace Tests
{
    public class CTest
    {
        /// <summary>
        /// Тест получения подтипа устройства
        /// </summary>
        /// <param name="expectedType">Ожидаемый подтип</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(GetDeviceSubTypeStrTestData))]
        public void GetDeviceSubTypeStr_NewDev_ReturnsDevType(
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
                new object[] { "C", "", GetNewCDevice() },
                new object[] { "C", "C", GetNewCDevice() },
                new object[] { "C", "Random", GetNewCDevice() },
                new object[] { "C", "Incorrect", GetNewCDevice() },
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
                new object[] { exportTags, "C", GetNewCDevice() },
                new object[] { exportTags, "Random", GetNewCDevice() },
                new object[] { exportTags, "Incorrect", GetNewCDevice() },
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
        public void Parameters_NewDev_ReturnsParametersArrWithDefaultValues(
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
                    "C",
                    GetNewCDevice()
                },
                new object[]
                {
                    parameters,
                    defaultValues,
                    "",
                    GetNewCDevice()
                },
                new object[]
                {
                    parameters,
                    defaultValues,
                    "Incorrect",
                    GetNewCDevice()
                },
            };
        }

        [Test]
        public void Properties_NewDev_ReturnsPropertiesArrWithNullValues()
        {
            const int expectedPropertiesCount = 2;
            string[] expectedPropertiesNames = 
                new string[] {"IN_VALUE", "OUT_VALUE" };
            var dev = GetNewCDevice();
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
        public void DefaultConstructor_NewDev_ReturnsCorrectTypeAndSubType()
        {
            var dev = GetNewCDevice();

            Assert.AreEqual(Device.DeviceType.C, dev.DeviceType);
            Assert.AreEqual(Device.DeviceSubType.NONE, dev.DeviceSubType);
        }

        [Test]
        public void Channels_NewDev_ReturnsEmptyChannels()
        {
            var dev = GetNewCDevice();

            Assert.AreEqual(0, dev.Channels.Count);
        }

        [Test]
        public void GenerateDeviceTags_NewDev_ReturnsTreeNodeForChannelBase()
        {
            var dev = GetNewCDevice();

            TreeNode actualNode = new TreeNode("obj");
            dev.GenerateDeviceTags(actualNode);

            TreeNode expectedNode = new TreeNode(dev.Name);
            var devProps = dev.GetDeviceProperties(dev.DeviceType,
                dev.DeviceSubType).Keys.ToList();
            devProps.AddRange(dev.Parameters.Keys.ToList());
            foreach (var property in devProps)
            {
                expectedNode.Nodes.Add($"{dev.Name}.{property}",
                    $"{dev.Name}.{property}");
            }

            for(int i = 0; i < expectedNode.Nodes.Count; i++)
            {
                string expectedText = expectedNode.Nodes[i].Text;
                string actualText = actualNode.FirstNode.Nodes[i].Text;
                Assert.AreEqual(expectedText, actualText);
            }     
        }

        [TestCase("")]
        [TestCase("\t")]
        public void SaveParameters_NewDev_ReturnsStringWithCorrectString(
            string prefix)
        {
            var dev = GetNewCDevTestDevice();
            string expectedSaveString = string.Empty;
            string tmp = string.Empty;
            foreach (var par in dev.Parameters)
            {
                if (par.Value != null)
                {
                    tmp += $"{prefix}\t{par.Key} = {par.Value},\n";
                }
            }
            if (tmp != string.Empty)
            {
                expectedSaveString += $"{prefix}par =\n";
                expectedSaveString += $"{prefix}\t{{\n";
                expectedSaveString += tmp.Remove(tmp.Length - 2) + "\n";
                expectedSaveString += $"{prefix}\t}}\n";
            }

            string actualSaveString = dev.SaveParameters(prefix);

            Assert.AreEqual(expectedSaveString, actualSaveString);
        }

        private static Device.IODevice GetNewCDevice()
        {
            return new Device.C(TestDevName, TestEplanName, TestDevDescription,
                TestDevNum, TestDevObjName, TestDevObjNum);
        }

        private static CDevTest GetNewCDevTestDevice()
        {
            return new CDevTest(TestDevName, TestEplanName,TestDevDescription,
                TestDevNum, TestDevObjName, TestDevObjNum);
        }

        const string TestDevName = "TANK1C1";
        const string TestEplanName = "+TANK1-C1";
        const string TestDevDescription = "PID";
        const int TestDevObjNum = 1;
        const string TestDevObjName = "TANK";
        const int TestDevNum = 1;

        public class CDevTest : Device.C
        {
            // Используем только внутри этого класса, поскольку цель -  это
            // протестировать protected метод.
            public CDevTest(string name, string eplanName, string description,
                int deviceNumber, string objectName, int objectNumber) : base(
                    name, eplanName, description, deviceNumber, objectName,
                    objectNumber) { }

            public new string SaveParameters(string prefix)
            {
                return base.SaveParameters(prefix);
            }
        }
    }
}
