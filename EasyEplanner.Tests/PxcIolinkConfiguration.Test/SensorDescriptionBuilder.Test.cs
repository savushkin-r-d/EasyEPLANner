using EasyEPlanner.PxcIolinkConfiguration;
using EasyEPlanner.PxcIolinkConfiguration.Interfaces;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.PxcIolinkConfigration
{
    internal class SensorDescriptionBuilderTest
    {
        private Mock<IModuleDescriptionBuilder> _moduleDescriptionBuilderMock;
        private Mock<IDevicesDescriptionBuilder> _devicesDescriptionBuilderMock;
        private Mock<IIOModule> _ioModule;

        [SetUp]
        public void SetUp()
        {
            _moduleDescriptionBuilderMock = new Mock<IModuleDescriptionBuilder>();
            _devicesDescriptionBuilderMock = new Mock<IDevicesDescriptionBuilder>();
            _ioModule = new Mock<IIOModule>();
        }

        [Test]
        public void CreateModuleDescription_BadModuleDescriptionBuilder_ThrowsException()
        {
            var exceptionToThrow = new Exception("Test exception");
            _moduleDescriptionBuilderMock
                .Setup(x => x.Build(It.IsAny<IIOModule>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Throws(exceptionToThrow);
            var sensorDescriptionBuilder = new SensorDescriptionBuilder(
                _moduleDescriptionBuilderMock.Object, _devicesDescriptionBuilderMock.Object);

            var thrownException = Assert.Throws<Exception>(() => sensorDescriptionBuilder
                .CreateModuleDescription( _ioModule.Object, string.Empty, new Dictionary<string, LinerecorderSensor>(),
                new Dictionary<string, LinerecorderSensor>()));
            Mock.Get(_moduleDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Assert.AreEqual(exceptionToThrow.Message, thrownException.Message);
        }

        [Test]
        public void CreateModuleDescription_BadDevicesDescriptionBuilder_ThrowsException()
        {
            var exceptionToThrow = new Exception("Test exception");
            _devicesDescriptionBuilderMock
                .Setup(x => x.Build(It.IsAny<IIOModule>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Throws(exceptionToThrow);
            var sensorDescriptionBuilder = new SensorDescriptionBuilder(
                _moduleDescriptionBuilderMock.Object, _devicesDescriptionBuilderMock.Object);

            var thrownException = Assert.Throws<Exception>(() => sensorDescriptionBuilder
                .CreateModuleDescription(_ioModule.Object, string.Empty, new Dictionary<string, LinerecorderSensor>(),
                new Dictionary<string, LinerecorderSensor>()));
            Mock.Get(_moduleDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Mock.Get(_devicesDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Assert.AreEqual(exceptionToThrow.Message, thrownException.Message);
        }

        [Test]
        public void CreateModuleDescription_EmptyTemplates_ReturnsEmptyDescription()
        {
            _moduleDescriptionBuilderMock
                .Setup(x => x.Build(It.IsAny<IIOModule>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new Device());
            _devicesDescriptionBuilderMock
                .Setup(x => x.Build(It.IsAny<IIOModule>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Device>());
            var sensorDescriptionBuilder = new SensorDescriptionBuilder(
                _moduleDescriptionBuilderMock.Object, _devicesDescriptionBuilderMock.Object);

            var moduleDescr = sensorDescriptionBuilder.CreateModuleDescription(_ioModule.Object, string.Empty,
                new Dictionary<string, LinerecorderSensor>(), new Dictionary<string, LinerecorderSensor>());

            Mock.Get(_moduleDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Mock.Get(_devicesDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Assert.IsTrue(moduleDescr.IsEmpty());
            Assert.IsNull(moduleDescr.Version);
        }

        [Test]
        public void CreateModuleDescription__ReturnsNotEmptyDesscription()
        {
            string version = "4.44.88";
            _moduleDescriptionBuilderMock
                .Setup(x => x.Build(It.IsAny<IIOModule>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new Device()
                {
                    Sensor = new Sensor()
                    {
                        DeviceId = 3,
                        ProductId = "44",
                        VendorId = 222
                    }
                });;
            _devicesDescriptionBuilderMock
                .Setup(x => x.Build(It.IsAny<IIOModule>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Device>()
                {
                    new Device() { Port = 4 },
                    new Device() { Port = 3 },
                });
            var sensorDescriptionBuilder = new SensorDescriptionBuilder(
                _moduleDescriptionBuilderMock.Object, _devicesDescriptionBuilderMock.Object);

            var moduleDescr = sensorDescriptionBuilder.CreateModuleDescription(_ioModule.Object, version,
                new Dictionary<string, LinerecorderSensor>(), new Dictionary<string, LinerecorderSensor>());

            Mock.Get(_moduleDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Mock.Get(_devicesDescriptionBuilderMock.Object).Verify(x => x.Build(It.IsAny<IIOModule>(),
                new Dictionary<string, LinerecorderSensor>()), Times.Once());
            Assert.IsFalse(moduleDescr.IsEmpty());
            Assert.AreEqual(version, moduleDescr.Version);
        }
    }
}
