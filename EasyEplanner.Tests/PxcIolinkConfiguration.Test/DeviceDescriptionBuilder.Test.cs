using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyEPlanner.PxcIolinkConfiguration;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using IO;
using Moq;

namespace Tests.PxcIolinkConfigration
{
    public class DeviceDescriptionBuilderTest
    {
        private Mock<IIOModule> _ioModule;

        [SetUp]
        public void SetUp()
        {
            _ioModule = new Mock<IIOModule>();
        }

        [Test]
        public void Build_NoDevicesInIOModule_ReturnsEmptyDescriptionList()
        {
            _ioModule.Setup(x => x.Devices).Returns(new List<EplanDevice.IIODevice>[0]);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            var devicesDescription = deviceDescriptionBuilder
                .Build(_ioModule.Object, new Dictionary<string, LinerecorderSensor>());

            Assert.AreEqual(0, devicesDescription.Count);
        }

        [Test]
        public void Build_IOModuleWithIncorrectLogicalClampsAndNoTemplates_ReturnsEmptyDescriptionList()
        {
            // In test we skip incorrect logical clamps and templates that we can't find.

            //Arrange
            string articleDevice = "test.article";
            var articleSensor = new LinerecorderSensor()
            {
                Sensor = new Sensor() { DeviceId = 1, ProductId = "2", VendorId = 3 }
            };
            var templates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleDevice, articleSensor }
            };
            var testDevices = new List<EplanDevice.IIODevice>[]
            {
                null,
                new List<EplanDevice.IIODevice>() { Mock.Of<EplanDevice.IIODevice>(d => d.ArticleName == articleDevice) },
                null,
                new List<EplanDevice.IIODevice>() { Mock.Of<EplanDevice.IIODevice>(d => d.ArticleName == string.Empty) },
                null,
                new List<EplanDevice.IIODevice>() { Mock.Of<EplanDevice.IIODevice>(d => d.ArticleName == string.Empty) },
            };
            var testDevicesChannels = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                null,
                new List<EplanDevice.IODevice.IIOChannel>() { Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == 0)},
                null,
                new List<EplanDevice.IODevice.IIOChannel>() { Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == 1)},
                null,
                new List<EplanDevice.IODevice.IIOChannel>() { Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == 2)},
            };

            _ioModule.Setup(x => x.Devices).Returns(testDevices);
            _ioModule.Setup(x => x.DevicesChannels).Returns(testDevicesChannels);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            //Act
            var devicesDescription = deviceDescriptionBuilder
                .Build(_ioModule.Object, templates);

            //Assert
            Assert.AreEqual(0, devicesDescription.Count);
        }

        [Test]
        public void Build_WrongPropertyInEplanDevice_ThrowsArgumentException()
        {
            //Arrange
            string articleDevice = "test.article";
            string wrongPropName = "wrongProp";
            string eplanName = "+-EplDev1-Lol";
            var articleSensor = new LinerecorderSensor()
            {
                Sensor = new Sensor() { DeviceId = 1, ProductId = "2", VendorId = 3 },
                Parameters = new Parameters() { Param = new List<Param>() }
            };
            var templates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleDevice, articleSensor }
            };
            var deviceMock = new Mock<EplanDevice.IIODevice>();
            deviceMock.Setup(d => d.ArticleName).Returns(articleDevice);
            deviceMock.Setup(d => d.IolConfProperties).Returns(new Dictionary<string, double>() { { wrongPropName, 111 } });
            deviceMock.Setup(d => d.EplanName).Returns(eplanName);
            var testDevices = new List<EplanDevice.IIODevice>[]
            {
                new List<EplanDevice.IIODevice>() { deviceMock.Object },
            };
            var testDevicesChannels = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                new List<EplanDevice.IODevice.IIOChannel>() { Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == 1)},
            };

            _ioModule.Setup(x => x.Devices).Returns(testDevices);
            _ioModule.Setup(x => x.DevicesChannels).Returns(testDevicesChannels);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            //Assert
            var exc = Assert.Throws<ArgumentException>(() => deviceDescriptionBuilder.Build(_ioModule.Object, templates));
            string expectedExceptionMessage = $"В устройстве {eplanName}" +
                        $" задано свойство {wrongPropName}, его не существует в шаблоне устройства" +
                        $" {articleDevice}.";
            Assert.AreEqual(expectedExceptionMessage , exc.Message);
        }

        [Test]
        public void Build_IncorrectInternalValueInTemplate_ThrowsArgumentException()
        {
            //Arrange
            string articleDevice = "test.article";
            string propName = "propName";
            string eplanName = "+-EplDev1-Lol";
            var articleSensor = new LinerecorderSensor()
            {
                Sensor = new Sensor() { DeviceId = 1, ProductId = "2", VendorId = 3 },
                Parameters = new Parameters()
                { 
                    Param = new List<Param>()
                    {
                        new Param() { Id = propName, InternalValue = "0", Value = "100" }
                    }
                }
            };
            var templates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleDevice, articleSensor }
            };
            var deviceMock = new Mock<EplanDevice.IIODevice>();
            deviceMock.Setup(d => d.ArticleName).Returns(articleDevice);
            deviceMock.Setup(d => d.IolConfProperties).Returns(new Dictionary<string, double>() { { propName, 111 } });
            deviceMock.Setup(d => d.EplanName).Returns(eplanName);
            var testDevices = new List<EplanDevice.IIODevice>[]
            {
                new List<EplanDevice.IIODevice>() { deviceMock.Object },
            };
            var testDevicesChannels = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                new List<EplanDevice.IODevice.IIOChannel>() { Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == 1)},
            };

            _ioModule.Setup(x => x.Devices).Returns(testDevices);
            _ioModule.Setup(x => x.DevicesChannels).Returns(testDevicesChannels);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            //Assert
            var exc = Assert.Throws<ArgumentException>(() => deviceDescriptionBuilder.Build(_ioModule.Object, templates));
            string expectedExceptionMessage = $"В шаблоне изделия {articleDevice}, параметр {propName}" +
                        $" содержит Value или InternalValue равное 0. Деление на 0. " +
                        $"Установите другие значения в шаблоне.";
            Assert.AreEqual(expectedExceptionMessage, exc.Message);
        }

        [Test]
        public void Build_IncorrectValueInTemplate_ThrowsArgumentException()
        {
            //Arrange
            string articleDevice = "test.article";
            string propName = "propName";
            string eplanName = "+-EplDev1-Lol";
            var articleSensor = new LinerecorderSensor()
            {
                Sensor = new Sensor() { DeviceId = 1, ProductId = "2", VendorId = 3 },
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        new Param() { Id = propName, InternalValue = "110", Value = "0" }
                    }
                }
            };
            var templates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleDevice, articleSensor }
            };
            var deviceMock = new Mock<EplanDevice.IIODevice>();
            deviceMock.Setup(d => d.ArticleName).Returns(articleDevice);
            deviceMock.Setup(d => d.IolConfProperties).Returns(new Dictionary<string, double>() { { propName, 111 } });
            deviceMock.Setup(d => d.EplanName).Returns(eplanName);
            var testDevices = new List<EplanDevice.IIODevice>[]
            {
                new List<EplanDevice.IIODevice>() { deviceMock.Object },
            };
            var testDevicesChannels = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                new List<EplanDevice.IODevice.IIOChannel>() { Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == 1)},
            };

            _ioModule.Setup(x => x.Devices).Returns(testDevices);
            _ioModule.Setup(x => x.DevicesChannels).Returns(testDevicesChannels);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            //Assert
            var exc = Assert.Throws<ArgumentException>(() => deviceDescriptionBuilder.Build(_ioModule.Object, templates));
            string expectedExceptionMessage = $"В шаблоне изделия {articleDevice}, параметр {propName}" +
                        $" содержит Value или InternalValue равное 0. Деление на 0. " +
                        $"Установите другие значения в шаблоне.";
            Assert.AreEqual(expectedExceptionMessage, exc.Message);
        }

        [Test]
        public void Build_DeviceWithoutOverridedParameters_ReturnsCorrectDeviceList()
        {
            //Arrange
            string articleDevice1 = "test.article1";
            string propName = "propName";
            int sensorDeviceId1 = 1;
            string sensorProductId1 = "11";
            int sensorVendorId1 = 3;
            string dev1PropInternalValue = "1000";
            string dev1PropValue = "100";
            int dev1LogicalPort = 5;
            var articleSensor1 = new LinerecorderSensor()
            {
                Sensor = new Sensor()
                {
                    DeviceId = sensorDeviceId1,
                    ProductId = sensorProductId1,
                    VendorId = sensorVendorId1
                },
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        new Param() { Id = propName, InternalValue = dev1PropInternalValue, Value = dev1PropValue }
                    }
                }
            };

            string articleDevice2 = "test.article2";
            int sensorDeviceId2 = 2;
            string sensorProductId2 = "22";
            int sensorVendorId2 = 33;
            string dev2PropInternalValue = "2000";
            string dev2PropValue = "200";
            int dev2LogicalPort = 8;
            var articleSensor2 = new LinerecorderSensor()
            {
                Sensor = new Sensor()
                {
                    DeviceId = sensorDeviceId2,
                    ProductId = sensorProductId2,
                    VendorId = sensorVendorId2
                },
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        new Param() { Id = propName, InternalValue = dev2PropInternalValue, Value = dev2PropValue }
                    }
                }
            };

            var templates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleDevice1, articleSensor1 },
                { articleDevice2, articleSensor2 }
            };

            var deviceMock1 = new Mock<EplanDevice.IIODevice>();
            deviceMock1.Setup(d => d.ArticleName).Returns(articleDevice1);
            deviceMock1.Setup(d => d.IolConfProperties).Returns(new Dictionary<string, double>());
            var deviceMock2 = new Mock<EplanDevice.IIODevice>();
            deviceMock2.Setup(d => d.ArticleName).Returns(articleDevice2);
            deviceMock2.Setup(d => d.IolConfProperties).Returns(new Dictionary<string, double>());
            var testDevices = new List<EplanDevice.IIODevice>[]
            {
                new List<EplanDevice.IIODevice>() { deviceMock1.Object },
                new List<EplanDevice.IIODevice>() { deviceMock2.Object },
            };
            var testDevicesChannels = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                new List<EplanDevice.IODevice.IIOChannel>()
                { 
                    Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == dev1LogicalPort)
                },
                new List<EplanDevice.IODevice.IIOChannel>()
                { 
                    Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == dev2LogicalPort)
                },
            };

            _ioModule.Setup(x => x.Devices).Returns(testDevices);
            _ioModule.Setup(x => x.DevicesChannels).Returns(testDevicesChannels);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            //Act
            var description = deviceDescriptionBuilder.Build(_ioModule.Object, templates);

            //Assert
            int expectedDevsCount = 2;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedDevsCount, description.Count);

                var dev1 = description[0];
                Assert.AreEqual(dev1LogicalPort ,dev1.Port);
                Assert.AreEqual(sensorProductId1, dev1.Sensor.ProductId);
                Assert.AreEqual(sensorDeviceId1, dev1.Sensor.DeviceId);
                Assert.AreEqual(sensorVendorId1, dev1.Sensor.VendorId);
                Assert.AreEqual(propName, dev1.Parameters.Param.First().Id);
                Assert.AreEqual(dev1PropInternalValue, dev1.Parameters.Param.First().InternalValue);
                Assert.AreEqual(dev1PropValue, dev1.Parameters.Param.First().Value);

                var dev2 = description[1];
                Assert.AreEqual(dev2LogicalPort, dev2.Port);
                Assert.AreEqual(sensorProductId2, dev2.Sensor.ProductId);
                Assert.AreEqual(sensorDeviceId2, dev2.Sensor.DeviceId);
                Assert.AreEqual(sensorVendorId2, dev2.Sensor.VendorId);
                Assert.AreEqual(propName, dev2.Parameters.Param.First().Id);
                Assert.AreEqual(dev2PropInternalValue, dev2.Parameters.Param.First().InternalValue);
                Assert.AreEqual(dev2PropValue, dev2.Parameters.Param.First().Value);
            });
        }

        [Test]
        public void Build_DeviceWithOverridedParameters_ReturnsCorrectDeviceList()
        {
            //Arrange
            string articleDevice1 = "test.article1";
            string propName = "propName";
            int sensorDeviceId1 = 1;
            string sensorProductId1 = "11";
            int sensorVendorId1 = 3;
            string dev1PropInternalValue = "1000";
            string dev1PropValue = "100";
            int dev1LogicalPort = 5;
            string expectedDev1PropInternalValue = "500";
            string expectedDev1PropValue = "50";
            var articleSensor1 = new LinerecorderSensor()
            {
                Sensor = new Sensor()
                {
                    DeviceId = sensorDeviceId1,
                    ProductId = sensorProductId1,
                    VendorId = sensorVendorId1
                },
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        new Param() { Id = propName, InternalValue = dev1PropInternalValue, Value = dev1PropValue }
                    }
                }
            };

            string articleDevice2 = "test.article2";
            int sensorDeviceId2 = 2;
            string sensorProductId2 = "22";
            int sensorVendorId2 = 33;
            string dev2PropInternalValue = "2000";
            string dev2PropValue = "200";
            int dev2LogicalPort = 8;
            string expectedDev2PropInternalValue = "100";
            string expectedDev2PropValue = "10";
            var articleSensor2 = new LinerecorderSensor()
            {
                Sensor = new Sensor()
                {
                    DeviceId = sensorDeviceId2,
                    ProductId = sensorProductId2,
                    VendorId = sensorVendorId2
                },
                Parameters = new Parameters()
                {
                    Param = new List<Param>()
                    {
                        new Param() { Id = propName, InternalValue = dev2PropInternalValue, Value = dev2PropValue }
                    }
                }
            };

            var templates = new Dictionary<string, LinerecorderSensor>()
            {
                { articleDevice1, articleSensor1 },
                { articleDevice2, articleSensor2 }
            };

            var deviceMock1 = new Mock<EplanDevice.IIODevice>();
            deviceMock1.Setup(d => d.ArticleName).Returns(articleDevice1);
            deviceMock1.Setup(d => d.IolConfProperties)
                .Returns(new Dictionary<string, double>()
                {
                    { propName, double.Parse(expectedDev1PropValue) }
                });
            var deviceMock2 = new Mock<EplanDevice.IIODevice>();
            deviceMock2.Setup(d => d.ArticleName).Returns(articleDevice2);
            deviceMock2.Setup(d => d.IolConfProperties)
                .Returns(new Dictionary<string, double>()
                {
                    { propName, double.Parse(expectedDev2PropValue) }
                });
            var testDevices = new List<EplanDevice.IIODevice>[]
            {
                new List<EplanDevice.IIODevice>() { deviceMock1.Object },
                new List<EplanDevice.IIODevice>() { deviceMock2.Object },
            };
            var testDevicesChannels = new List<EplanDevice.IODevice.IIOChannel>[]
            {
                new List<EplanDevice.IODevice.IIOChannel>()
                {
                    Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == dev1LogicalPort)
                },
                new List<EplanDevice.IODevice.IIOChannel>()
                {
                    Mock.Of<EplanDevice.IODevice.IIOChannel>(c => c.LogicalClamp == dev2LogicalPort)
                },
            };

            _ioModule.Setup(x => x.Devices).Returns(testDevices);
            _ioModule.Setup(x => x.DevicesChannels).Returns(testDevicesChannels);
            var deviceDescriptionBuilder = new DevicesDescriptionBuilder();

            //Act
            var description = deviceDescriptionBuilder.Build(_ioModule.Object, templates);

            //Assert
            int expectedDevsCount = 2;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedDevsCount, description.Count);

                var dev1 = description[0];
                Assert.AreEqual(dev1LogicalPort, dev1.Port);
                Assert.AreEqual(sensorProductId1, dev1.Sensor.ProductId);
                Assert.AreEqual(sensorDeviceId1, dev1.Sensor.DeviceId);
                Assert.AreEqual(sensorVendorId1, dev1.Sensor.VendorId);
                Assert.AreEqual(propName, dev1.Parameters.Param.First().Id);
                Assert.AreEqual(expectedDev1PropInternalValue, dev1.Parameters.Param.First().InternalValue);
                Assert.AreEqual(expectedDev1PropValue, dev1.Parameters.Param.First().Value);

                var dev2 = description[1];
                Assert.AreEqual(dev2LogicalPort, dev2.Port);
                Assert.AreEqual(sensorProductId2, dev2.Sensor.ProductId);
                Assert.AreEqual(sensorDeviceId2, dev2.Sensor.DeviceId);
                Assert.AreEqual(sensorVendorId2, dev2.Sensor.VendorId);
                Assert.AreEqual(propName, dev2.Parameters.Param.First().Id);
                Assert.AreEqual(expectedDev2PropInternalValue, dev2.Parameters.Param.First().InternalValue);
                Assert.AreEqual(expectedDev2PropValue, dev2.Parameters.Param.First().Value);
            });
        }
    }
}
