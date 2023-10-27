using EasyEPlanner.PxcIolinkConfiguration;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.PxcIolinkConfigration
{
    internal class ModuleDescriptionBuilderTest
    {
        private Mock<IIOModule> _ioModule;

        [SetUp]
        public void SetUp()
        {
            _ioModule = new Mock<IIOModule>();
        }

        [Test]
        public void Build_EmptyTemplates_ReturnsEmptyDescription()
        {
            _ioModule
                .Setup(x => x.ArticleName)
                .Returns(string.Empty);
            var moduleDescriptionBuilder = new ModuleDescriptionBuilder();

            var description = moduleDescriptionBuilder
                .Build(_ioModule.Object, new Dictionary<string, LinerecorderSensor>());

            Assert.IsTrue(description.IsEmpty());
        }

        [Test]
        public void Build_IncorrectParamNameInTemplate_ThrowsInvalidDataException()
        {
            string articleName = "Device.Article";
            string notFoundParamName = "V_PortConfig_02";
            string moduleName = "IOLModuleName";
            int moduleClampsCount = 4;
            IOModuleInfo.AddModuleInfo(0, moduleName, string.Empty, 0, string.Empty,
                string.Empty, new int[moduleClampsCount], new List<List<int>>(), new int[0], new int[0], 0, 0, 0, 0, 0, string.Empty);
            _ioModule.Setup(x => x.ArticleName).Returns(articleName);
            _ioModule.Setup(x => x.Name).Returns(moduleName);
            _ioModule.Setup(x => x.Info).Returns(IOModuleInfo.GetModuleInfo(moduleName, out _));
            _ioModule.Setup(x => x.DevicesChannels).Returns(new List<EplanDevice.IODevice.IIOChannel>[0]);
            var linerecorderSensor = new LinerecorderSensor()
            {
                Sensor = new Sensor() {  DeviceId = 2, ProductId = "222", VendorId = 1 },
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        new Param() { Id = "V_PortConfig_01" },
                        new Param() { Id = "WrongIOLParam" }
                    } 
                }
            };
            var moduleTemplates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleName, linerecorderSensor }
            };
            var moduleDescriptionBuilder = new ModuleDescriptionBuilder();

            var exception = Assert
                .Throws<InvalidDataException>(() => moduleDescriptionBuilder.Build(_ioModule.Object, moduleTemplates));
            Assert.AreEqual(
                $"Параметр клеммы {notFoundParamName} модуля {moduleName} {articleName} не найден.", exception.Message);
        }

        [Test]
        public void Build_FullTemplateWithAllSignalsTypes_ReturnsCorrectDescription()
        {
            // Arrange
            string expectedArticleName = "Device.Article";
            string expectedModuleName = "IOLModuleName";
            int expectedModuleClampsCount = 5;

            IOModuleInfo.AddModuleInfo(0, expectedModuleName, string.Empty, 0, string.Empty,
                string.Empty, new int[expectedModuleClampsCount], new List<List<int>>(), new int[0], new int[0], 0, 0, 0, 0, 0, string.Empty);

            _ioModule.Setup(x => x.ArticleName).Returns(expectedArticleName);
            _ioModule.Setup(x => x.Name).Returns(expectedModuleName);
            _ioModule.Setup(x => x.Info).Returns(IOModuleInfo.GetModuleInfo(expectedModuleName, out _));

            var channelMock1 = new Mock<EplanDevice.IODevice.IIOChannel>();
            channelMock1.Setup(x => x.LogicalClamp).Returns(1);
            channelMock1.Setup(x => x.Name).Returns("AI");
            var channelMock2 = new Mock<EplanDevice.IODevice.IIOChannel>();
            channelMock2.Setup(x => x.LogicalClamp).Returns(2);
            channelMock2.Setup(x => x.Name).Returns("AO");
            var channelMock3 = new Mock<EplanDevice.IODevice.IIOChannel>();
            channelMock3.Setup(x => x.LogicalClamp).Returns(3);
            channelMock3.Setup(x => x.Name).Returns("DI");
            var channelMock4 = new Mock<EplanDevice.IODevice.IIOChannel>();
            channelMock4.Setup(x => x.LogicalClamp).Returns(4);
            channelMock4.Setup(x => x.Name).Returns("DO");
            var channelMock5 = new Mock<EplanDevice.IODevice.IIOChannel>();
            channelMock5.Setup(x => x.LogicalClamp).Returns(5);
            channelMock5.Setup(x => x.Name).Returns("None");
            var channelsList = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                new List<EplanDevice.IODevice.IIOChannel>()
                {
                    channelMock1.Object,
                    channelMock2.Object,
                    channelMock3.Object,
                    channelMock4.Object,
                    channelMock5.Object,
                }
            };

            _ioModule.Setup(x => x.DevicesChannels).Returns(channelsList);
            var expectedSensor = new Sensor() { DeviceId = 2, ProductId = "222", VendorId = 1 };
            var linerecorderSensor = new LinerecorderSensor()
            {
                Sensor = expectedSensor,
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        // Size depends on expectedModuleClampsCount variable
                        new Param() { Id = "V_PortConfig_01" },
                        new Param() { Id = "V_PortConfig_02" },
                        new Param() { Id = "V_PortConfig_03" },
                        new Param() { Id = "V_PortConfig_04" },
                        new Param() { Id = "V_PortConfig_05" },
                    }
                }
            };

            var moduleTemplates = new Dictionary<string, LinerecorderSensor>()
            {
                { expectedArticleName, linerecorderSensor }
            };

            var moduleDescriptionBuilder = new ModuleDescriptionBuilder();

            // Act
            var description = moduleDescriptionBuilder.Build(_ioModule.Object, moduleTemplates);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedSensor.DeviceId, description.Sensor.DeviceId);
                Assert.AreEqual(expectedSensor.ProductId, description.Sensor.ProductId);
                Assert.AreEqual(expectedSensor.VendorId, description.Sensor.VendorId);

                var paramList = description.Parameters.Param;
                Assert.AreEqual("0", paramList[0].InternalValue); // First param (V_PortConfig_01)
                Assert.AreEqual("0", paramList[0].Value);
                Assert.AreEqual("IO-Link", paramList[0].Text);
                Assert.AreEqual("0", paramList[1].InternalValue); // V_PortConfig_02
                Assert.AreEqual("0", paramList[1].Value);
                Assert.AreEqual("IO-Link", paramList[1].Text);
                Assert.AreEqual("1", paramList[2].InternalValue); // V_PortConfig_03
                Assert.AreEqual("1", paramList[2].Value);
                Assert.AreEqual("DI", paramList[2].Text);
                Assert.AreEqual("2", paramList[3].InternalValue); // V_PortConfig_04
                Assert.AreEqual("2", paramList[3].Value);
                Assert.AreEqual("DO", paramList[3].Text);
                Assert.AreEqual("3", paramList[4].InternalValue); // V_PortConfig_05
                Assert.AreEqual("3", paramList[4].Value);
                Assert.AreEqual("Disabled", paramList[4].Text);
            });
        }
    }
}
