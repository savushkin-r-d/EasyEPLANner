using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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

        [Test]
        public void GenerateDeviceTags_NewObject_ReturnsTreeNodeForChannelBase()
        {
            var dev = GetNewRDevice();

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
        public void SaveParameters_NewObject_ReturnsStringWithCorrectString(
            string prefix)
        {
            var dev = GetNewRDevTestDevice();
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

        private static Device.IODevice GetNewRDevice()
        {
            return new Device.R(TestDevName, TestDevDescription, TestDevNum,
                TestDevObjName, TestDevObjNum);
        }

        private static RDevTest GetNewRDevTestDevice()
        {
            return new RDevTest(TestDevName, TestDevDescription, TestDevNum,
                TestDevObjName, TestDevObjNum);
        }

        const string TestDevName = "TANK1R1";
        const string TestDevDescription = "PID";
        const int TestDevObjNum = 1;
        const string TestDevObjName = "TANK";
        const int TestDevNum = 1;

        public class RDevTest : Device.R
        {
            // Используем только внутри этого класса, поскольку цель -  это
            // протестировать protected метод.
            public RDevTest(string fullName, string description,
                int deviceNumber, string objectName, int objectNumber) : base(
                    fullName, description, deviceNumber, objectName,
                    objectNumber) { }

            public new string SaveParameters(string prefix)
            {
                return base.SaveParameters(prefix);
            }
        }
    }
}
