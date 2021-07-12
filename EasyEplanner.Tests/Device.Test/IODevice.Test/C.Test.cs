using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace Tests.Devices
{
    public class CTest
    {
        const string Incorrect = "Incorrect";
        const string C = "C";

        const string AI = Device.IODevice.IOChannel.AI;
        const string AO = Device.IODevice.IOChannel.AO;
        const string DI = Device.IODevice.IOChannel.DI;
        const string DO = Device.IODevice.IOChannel.DO;

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
                new object[] { C, string.Empty, GetNewCDevice() },
                new object[] { C, C, GetNewCDevice() },
                new object[] { C, Incorrect, GetNewCDevice() },
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
                {DeviceTag.ST, 1},
                {DeviceTag.M, 1},
                {DeviceTag.V, 1},
                {DeviceTag.Z, 1}
            };

            return new object[]
            {
                new object[] { exportTags, C, GetNewCDevice() },
                new object[] { exportTags, string.Empty, GetNewCDevice() },
                new object[] { exportTags, Incorrect, GetNewCDevice() },
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
                DeviceParameter.P_k,
                DeviceParameter.P_Ti,
                DeviceParameter.P_Td,
                DeviceParameter.P_dt,
                DeviceParameter.P_max,
                DeviceParameter.P_min,
                DeviceParameter.P_acceleration_time,
                DeviceParameter.P_is_manual_mode,
                DeviceParameter.P_U_manual,
                DeviceParameter.P_k2,
                DeviceParameter.P_Ti2,
                DeviceParameter.P_Td2,
                DeviceParameter.P_out_max,
                DeviceParameter.P_out_min,
                DeviceParameter.P_is_reverse,
                DeviceParameter.P_is_zero_start
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
                    C,
                    GetNewCDevice()
                },
                new object[]
                {
                    parameters,
                    defaultValues,
                    string.Empty,
                    GetNewCDevice()
                },
                new object[]
                {
                    parameters,
                    defaultValues,
                    Incorrect,
                    GetNewCDevice()
                },
            };
        }

        [Test]
        public void Properties_NewDev_ReturnsPropertiesArrWithNullValues()
        {
            const int expectedPropertiesCount = 2;
            string[] expectedPropertiesNames = new string[]
            {
                DeviceProperty.IN_VALUE,
                DeviceProperty.OUT_VALUE
            };
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

        /// <summary>
        /// Тестирование каналов устройства
        /// </summary>
        /// <param name="expectedChannelsCount">Ожидаемое количество каналов
        /// в словаре с названием каналов</param>
        /// <param name="subType">Актуальный подтип</param>
        /// <param name="device">Тестируемое устройство</param>
        [TestCaseSource(nameof(ChannelsTestData))]
        public void Channels_ObjWithDiffSubTypes_ReturnCorrectDictionary(
            Dictionary<string, int> expectedChannelsCount, string subType,
            Device.IODevice device)
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
                    emptyChannels,
                    string.Empty,
                    GetNewCDevice()
                },
                new object[]
                {
                    emptyChannels,
                    C,
                    GetNewCDevice()
                },
                new object[]
                {
                    emptyChannels,
                    Incorrect,
                    GetNewCDevice()
                },
            };
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
