using NUnit.Framework;
using System.Linq;
using System.Windows.Forms;
using EplanDevice;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System;

namespace Tests.EplanDevices
{
    public class IODeviceTest
    {
        class IODeviceFake : IODevice
        {
            public IODeviceFake(string name, string eplanName,
            string description, string deviceType, int deviceNumber,
            string objectName, int objectNumber) : base(name, eplanName,
                description, deviceType, deviceNumber, objectName, objectNumber)
            {
                OnPropertyChanged += () => InvokedOnPropertyChanged = true;
            }

            /// <summary>
            /// Property for checking invoking of OnPropertyChanged event
            /// </summary>
            public bool InvokedOnPropertyChanged { get; set; } = false;
        }

        [Test]
        public void Constructor_SetCorrectDevType_ReturnsDevType()
        {
            string settingDevType = "V"; //Valve
            DeviceType expectedDevType = DeviceType.V;
            
            var device = new IODeviceFake(string.Empty, string.Empty,
                string.Empty, settingDevType, 0, string.Empty, 0);

            Assert.AreEqual(expectedDevType, device.DeviceType);
        }

        [Test]
        public void Constructor_SetWrongDevType_ReturnsNoneDevType()
        {
            string settingDevType = "SuperDuperDev"; //Not exists
            DeviceType expectedDevType = DeviceType.NONE;

            var device = new IODeviceFake(string.Empty, string.Empty,
                string.Empty, settingDevType, 0, string.Empty, 0);

            Assert.AreEqual(expectedDevType, device.DeviceType);
        }

        [Test]
        public void SetProperty_SetCorrectValue_InvokeEventAndSetValue()
        {
            var device = new IODeviceFake(string.Empty, string.Empty,
                string.Empty, string.Empty, 0, string.Empty, 0);
            string propertyName = "TEST_PROPERTY";
            device.Properties.Add(propertyName, null);
            var propertyValue = "value";

            string errors = device.SetProperty(propertyName, propertyValue);

            Assert.Multiple(() =>
            {
                Assert.IsEmpty(errors);
                Assert.AreEqual(propertyValue, device.Properties[propertyName]);
                Assert.IsTrue(device.InvokedOnPropertyChanged);
            });
        }

        [Test]
        public void SetProperty_SetIncorrectValue_NotSetAndReturnErrorString()
        {
            var device = new IODeviceFake(string.Empty, string.Empty,
                string.Empty, string.Empty, 0, string.Empty, 0);
            string wrongPropertyName = "BlahBlahBlah";
            string anyValue = "JustValue";
            // Property is not exists, we will have error

            string errors = device.SetProperty(wrongPropertyName, anyValue);

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(errors);
                Assert.IsFalse(device.InvokedOnPropertyChanged);
            });
        }


        [TestCaseSource(nameof(TestSortingChannelsForVDeviceData))]
        public void SortChannels_NewValveDevices_ReturnsSortedArrayOfChannels(
            IODevice dev, string subType, string[] expected)
        {
            dev.SetSubType(subType);
            dev.SortChannels();
            string[] actual = dev.Channels
                .Where(x => x.Comment != string.Empty)
                .Select(x => x.Comment ).ToArray();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// 1 - Устройство V (клапан) для тестирования
        /// 2 - Задаваемый подтип
        /// 3 - Ожидаемое значение
        /// </summary>
        /// <returns></returns>
        public static object[] TestSortingChannelsForVDeviceData()
        {
            return new object[] 
            {
                new object[] 
                {
                    VTest.GetRandomVDevice(), 
                    "V_DO2", 
                    new string[] {"Открыть", "Закрыть"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_DO1_DI2",
                    new string[] {"Открыт", "Закрыт"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(), 
                    "V_DO2_DI2",
                    new string[] {"Открыть", "Закрыть", "Открыт", "Закрыт"} },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_MIXPROOF",
                    new string[] {"Открыть", "Открыть ВС", "Открыть НС", 
                        "Открыт", "Закрыт"} 
                },
                new object[] 
                {
                    VTest.GetRandomVDevice(),
                    "V_BOTTOM_MIXPROOF",
                    new string[] {"Открыть", "Открыть мини", "Открыть НС", 
                        "Открыт", "Закрыт"} 
                },
            };
        }

        [TestCaseSource(nameof(GenerateDeviceTagsCaseSource))]
        public void GenerateDeviceTags_DeviceAODefault_ReturnsTree(
            IODevice dev, TreeNode expectedNode)
        {
            var actualNode = new TreeNode();
            
            dev.GenerateDeviceTags(actualNode);
            
            for(int i = 0; i < actualNode.Nodes.Count; i++)
            {
                TreeNode actualSubNode = actualNode.Nodes[i];
                TreeNode expectedSubNode = expectedNode.Nodes[i];
                Assert.AreEqual(expectedSubNode.Text, actualSubNode.Text);
                for (int j = 0; j < actualNode.Nodes.Count; j++)
                {
                    Assert.AreEqual(expectedSubNode.Nodes[j].Text,
                        actualSubNode.Nodes[j].Text);
                }
            }
        }

        public static object[] GenerateDeviceTagsCaseSource()
        {
            string devName = "TANK1AO1";
            string eplanName = "+TANK1-AO1";
            string descr = "Сигнал AO";
            string objName = "TANK";
            int objNum = 1;
            int devNum = 1;

            string devMTag = "M";
            string devVTag = "V";
            string devPMinVTag = "P_MIN_V";
            string devPMaxVTag = "P_MAX_V";

            var expectedNode = new TreeNode();
            var mNode = new TreeNode($"AO_{devMTag}");
            mNode.Nodes.Add($"{devName}.{devMTag}");
            var vNode = new TreeNode($"AO_{devVTag}");
            vNode.Nodes.Add($"{devName}.{devVTag}");
            var pMinVNode = new TreeNode($"AO_{devPMinVTag}");
            pMinVNode.Nodes.Add($"{devName}.{devPMinVTag}");
            var pMaxVNode = new TreeNode($"AO_{devPMaxVTag}");
            pMaxVNode.Nodes.Add($"{devName}.{devPMaxVTag}");
            expectedNode.Nodes.Add(mNode);
            expectedNode.Nodes.Add(vNode);
            expectedNode.Nodes.Add(pMinVNode);
            expectedNode.Nodes.Add(pMaxVNode);

            var dev = new AO(devName, eplanName, descr, devNum, objName,
                objNum);
            var defaultAODev = new object[] { dev, expectedNode };

            return new object[] { defaultAODev };
        }

        [Test]
        public void SetIolConfProperty_NewDevice_AddNewProperties()
        {
            //Arrange
            var dev = new IODeviceFake(string.Empty, string.Empty,
                string.Empty, "V", 1, string.Empty, 1);
            int expectedInitialValueOfProps = 0;
            int actualInitialValueOfProps = dev.IolConfProperties.Count;
            var propsToSet = new Dictionary<string, double>
            {
                { "t1", 0 },
                { "t2", 1 },
                { "t3", 2 },
            };

            //Act
            foreach(var prop in propsToSet)
            {
                dev.SetIolConfProperty(prop.Key, prop.Value);
            }

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedInitialValueOfProps, actualInitialValueOfProps);
                Assert.AreEqual(propsToSet.Count, dev.IolConfProperties.Count);
                foreach(var prop in propsToSet)
                {
                    Assert.AreEqual(prop.Value, dev.IolConfProperties[prop.Key]);
                    Assert.IsTrue(dev.IolConfProperties.ContainsKey(prop.Key));
                }
            });
        }

        /// <summary>
        /// Проверка установки паракметров
        /// Вызывает исключение, так как связано с API
        /// </summary>
        [Test]
        public void UpdateParameters_ThrowsException()
        {
            IODevice dev = new AO("KOAG4AO1", "+KOAG4-AO1", "Test device", 1, "KOAG", 4);
            dev.SetSubType("AO");

            dev.SetParameter("P_MIN_V", 1);

            Assert.Throws<System.IO.FileNotFoundException>(() => dev.UpdateParameters());
        }

        [Test]
        public void Check_CheckEmpty()
        {
            IODevice dev = new AO("KOAG4AO1", "+KOAG4-AO1", "Test device", 1, "KOAG", 4);
            dev.SetSubType("AO");

            string expectedResult = "\"KOAG4AO1\" : не привязанный канал  AO \"\".\n\"KOAG4AO1\" : не задан параметр \"P_MIN_V\".\n\"KOAG4AO1\" : не задан параметр \"P_MAX_V\".\n";

            Assert.AreEqual(expectedResult, dev.Check());
        }
    }
}
