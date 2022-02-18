using Moq;
using NUnit.Framework;
using EasyEPlanner.PxcIolinkConfiguration;
using System.IO;
using System;
using System.Collections.Generic;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using System.Threading.Tasks;
using IO;

namespace Tests.PxcIolinkConfigration
{
    public class PxcIolinkModulesConfigurationTest
    {
        private Mock<IXmlTemplateReader> _templateReaderMock;
        private Mock<IXmlSensorSerializer> _sensorSerializerMock;
        private Mock<ISensorDescriptionBuilder> _sensorDescriptionBuilderMock;

        private string _devicesFolder = "Devices";
        private string _modulesFolder = "Modules";
        private string _iolConfFolder = "IOL-Conf";

        private string _pathToAssemblyFolder;
        private string _pathToProjectFilesFolder;
        private string _pathToDeviceTemplatesFolder;
        private string _pathToModuleTemplatesFolder;

        [SetUp]
        public void SetUpMock()
        {
            _templateReaderMock = new Mock<IXmlTemplateReader>();
            _sensorDescriptionBuilderMock = new Mock<ISensorDescriptionBuilder>();
            _sensorSerializerMock = new Mock<IXmlSensorSerializer>();
            string tempPath = Path.GetTempPath();
            _pathToAssemblyFolder = Path.Combine(tempPath, "EplAssemblyFldr");
            _pathToProjectFilesFolder = Path.Combine(tempPath, "EplProjFldr");
            _pathToDeviceTemplatesFolder = Path.Combine(_pathToAssemblyFolder, _iolConfFolder, _devicesFolder);
            _pathToModuleTemplatesFolder = Path.Combine(_pathToAssemblyFolder, _iolConfFolder ,_modulesFolder);
        }

        [Test]
        public void CreateFolders_EmptyAssemblyPath_ThrowsInvalidDataException()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            var exception = Assert.Throws<InvalidDataException>(
                () => config.CreateFolders(string.Empty, _pathToProjectFilesFolder));
            Assert.AreEqual("Невозможно проверить существование папок с шаблонами IOL-Conf.", exception.Message);
        }

        [Test]
        public void CreateFolders_NullAssemblyPath_ThrowsInvalidDataException()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            var exception = Assert.Throws<InvalidDataException>(
                () => config.CreateFolders(null, _pathToProjectFilesFolder));
            Assert.AreEqual("Невозможно проверить существование папок с шаблонами IOL-Conf.", exception.Message);
        }

        [Test]
        public void CreateFolder_EmptyProjectsPath_ThrowsInvalidDataException()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            var exception = Assert.Throws<InvalidDataException>(
                () => config.CreateFolders(_pathToAssemblyFolder, string.Empty));
            Assert.AreEqual("Невозможно найти генерируемые файлы проекта.", exception.Message);
        }

        [Test]
        public void CreateFolder_NullProjectsPath_ThrowsInvalidDataException()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            var exception = Assert.Throws<InvalidDataException>(
                () => config.CreateFolders(_pathToAssemblyFolder, null));
            Assert.AreEqual("Невозможно найти генерируемые файлы проекта.", exception.Message);
        }

        [Test]
        public void CreateFolder_ActualPaths_CreatesFoldersReturnsTrue()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            bool created = config.CreateFolders(_pathToAssemblyFolder, _pathToProjectFilesFolder);

            Assert.IsTrue(created);
            Assert.IsTrue(Directory.Exists(Path.Combine(_pathToAssemblyFolder, _iolConfFolder, _devicesFolder)));
            Assert.IsTrue(Directory.Exists(Path.Combine(_pathToAssemblyFolder, _iolConfFolder, _modulesFolder)));
            Assert.IsTrue(Directory.Exists(Path.Combine(_pathToProjectFilesFolder, _iolConfFolder)));
        }

        [Test]
        public void ReadTemplates_FoldersNotCreated_ThrowsInvalidOperationException()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(
                () => config.ReadTemplates(false));
            Assert.AreEqual("Каталоги не созданы, сначала создайте каталоги.", exception.Message);
        }

        [Test]
        public void ReadTemplates_NoDevicesTemplates_ThrowsException()
        {
            _templateReaderMock
                .Setup(i => i.Read(_pathToDeviceTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>());
            _templateReaderMock
                .Setup(i => i.Read(_pathToModuleTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>());
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            // Returns true
            bool foldersCreated = config.CreateFolders(_pathToAssemblyFolder, _pathToProjectFilesFolder);
            var exception = Assert.Throws<Exception>(
                () => config.ReadTemplates(foldersCreated));
            Assert.AreEqual("Отсутствуют описания устройств.", exception.Message);
        }

        [Test]
        public void ReadTemplates_NoModulesTemplates_ThrowsException()
        {
            _templateReaderMock
                .Setup(i => i.Read(_pathToDeviceTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { }), Task.Run(() => { }) });
            _templateReaderMock
                .Setup(i => i.Read(_pathToModuleTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>());
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            // Returns true
            bool foldersCreated = config.CreateFolders(_pathToAssemblyFolder, _pathToProjectFilesFolder);
            var exception = Assert.Throws<Exception>(
                () => config.ReadTemplates(foldersCreated));
            Assert.AreEqual("Отсутствуют описания модулей ввода-вывода.", exception.Message);
        }

        [Test]
        public void ReadTemplates_ExceptionDuringReadDevicesTemplate_ThrowsAggregateException()
        {
            var expectedException = new Exception();
            _templateReaderMock
                .Setup(i => i.Read(_pathToDeviceTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { throw expectedException; }) });
            _templateReaderMock
                .Setup(i => i.Read(_pathToModuleTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { }) });
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            // Returns true
            bool foldersCreated = config.CreateFolders(_pathToAssemblyFolder, _pathToProjectFilesFolder);
            var exception = Assert.Throws<AggregateException>(
                () => config.ReadTemplates(foldersCreated));
            int expectedExceptionsCount = 1;
            Assert.AreEqual(expectedExceptionsCount, exception.InnerExceptions.Count);
        }

        [Test]
        public void ReadTemplates_ExceptionDuringReadModulesTemplate_ThrowsAggregateException()
        {
            var expectedException = new Exception();
            _templateReaderMock
                .Setup(i => i.Read(_pathToDeviceTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { }) });
            _templateReaderMock
                .Setup(i => i.Read(_pathToModuleTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { throw expectedException; }) });
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            // Returns true
            bool foldersCreated = config.CreateFolders(_pathToAssemblyFolder, _pathToProjectFilesFolder);
            var exception = Assert.Throws<AggregateException>(
                () => config.ReadTemplates(foldersCreated));
            int expectedExceptionsCount = 1;
            Assert.AreEqual(expectedExceptionsCount, exception.InnerExceptions.Count);
        }

        [Test]
        public void ReadTemplates_ExceptionsInBothReadMethods_ThrowsAggregateException()
        {
            var expectedException = new Exception();
            _templateReaderMock
                .Setup(i => i.Read(_pathToDeviceTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { throw expectedException; }) });
            _templateReaderMock
                .Setup(i => i.Read(_pathToModuleTemplatesFolder,
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new List<Task>() { Task.Run(() => { throw expectedException; }) });
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            // Returns true
            bool foldersCreated = config.CreateFolders(_pathToAssemblyFolder, _pathToProjectFilesFolder);
            var exception = Assert.Throws<AggregateException>(
                () => config.ReadTemplates(foldersCreated));
            int expectedExceptionsCount = 2;
            Assert.AreEqual(expectedExceptionsCount, exception.InnerExceptions.Count);
        }

        [Test]
        public void CreateModulesDescription_TemplatesNotRead_ThrowsInvalidOperationException()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                config.CreateModulesDescription(false, Mock.Of<IIOManager>()));
            Assert.AreEqual("Шаблоны не загружены, загрузите шаблоны." , exception.Message);
        }

        [Test]
        public void CreateModulesDescription_TemplatesLoadedButIoManagerEmpty_DoNotCallGenerateAndSerializeMethods()
        {
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);
            var ioManagerMock = new Mock<IIOManager>();
            ioManagerMock.Setup(x => x.IONodes).Returns(new List<IIONode>());

            config.CreateModulesDescription(true, ioManagerMock.Object);

            Mock.Get(_sensorSerializerMock.Object).Verify(x => 
            x.Serialize(It.IsAny<LinerecorderMultiSensor>(), It.IsAny<string>()), Times.Never);
            Mock.Get(_sensorDescriptionBuilderMock.Object).Verify(x => 
            x.CreateModuleDescription(It.IsAny<IIOModule>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, LinerecorderSensor>>(), 
                It.IsAny<Dictionary<string, LinerecorderSensor>>()), Times.Never);
        }

        [Test]
        public void CreateModulesDescription_TemplatesLoadedButTheyEmpty_DoNotCallSerialize()
        {
            //Arrange
            _sensorDescriptionBuilderMock
                .Setup(x => x.CreateModuleDescription(It.IsAny<IIOModule>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, LinerecorderSensor>>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new LinerecorderMultiSensor());
            int iolinkModulesInMockCount = 4;
            var ioManagerMock = new Mock<IIOManager>();
            var ioNodeMock = new Mock<IIONode>();
            var ioModuleMock = new Mock<IIOModule>();
            ioModuleMock
                .Setup(x => x.IsIOLink(It.IsAny<bool>()))
                .Returns(true);
            ioNodeMock
                .Setup(x => x.IOModules)
                .Returns(new List<IIOModule>()
                {
                    ioModuleMock.Object,
                    ioModuleMock.Object,
                });
            ioManagerMock
                .Setup(x => x.IONodes)
                .Returns(new List<IIONode>()
                {
                    ioNodeMock.Object,
                    ioNodeMock.Object
                });
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);
            
            //Act 
            config.CreateModulesDescription(true, ioManagerMock.Object);

            //Assert
            Mock.Get(_sensorSerializerMock.Object).Verify(x =>
                x.Serialize(It.IsAny<LinerecorderMultiSensor>(), It.IsAny<string>()),
                    Times.Never);
            Mock.Get(_sensorDescriptionBuilderMock.Object).Verify(x =>
                x.CreateModuleDescription(It.IsAny<IIOModule>(), It.IsAny<string>(),
                    It.IsAny<Dictionary<string, LinerecorderSensor>>(),
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()),
                    Times.Exactly(iolinkModulesInMockCount));
        }

        [Test]
        public void CreateModulesDescription_TemplatesLoaded_DoNotCallSerialize()
        {
            //Arrange
            _sensorDescriptionBuilderMock
                .Setup(x => x.CreateModuleDescription(It.IsAny<IIOModule>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, LinerecorderSensor>>(), It.IsAny<Dictionary<string, LinerecorderSensor>>()))
                .Returns(new LinerecorderMultiSensor()
                {
                    Devices = new Devices()
                    {
                        Device = new List<Device>()
                        {
                            new Device(),
                            new Device()
                        }
                    }
                });
            int iolinkModulesInMockCount = 4;
            var ioManagerMock = new Mock<IIOManager>();
            var ioNodeMock = new Mock<IIONode>();
            var ioModuleMock = new Mock<IIOModule>();
            ioModuleMock
                .Setup(x => x.IsIOLink(It.IsAny<bool>()))
                .Returns(true);
            ioNodeMock
                .Setup(x => x.IOModules)
                .Returns(new List<IIOModule>()
                {
                    ioModuleMock.Object,
                    ioModuleMock.Object,
                });
            ioManagerMock
                .Setup(x => x.IONodes)
                .Returns(new List<IIONode>()
                {
                    ioNodeMock.Object,
                    ioNodeMock.Object
                });
            var config = new PxcIolinkModulesConfiguration
                (_sensorSerializerMock.Object, _templateReaderMock.Object,
                _sensorDescriptionBuilderMock.Object);

            //Act 
            config.CreateModulesDescription(true, ioManagerMock.Object);

            //Assert
            Mock.Get(_sensorSerializerMock.Object).Verify(x =>
                x.Serialize(It.IsAny<LinerecorderMultiSensor>(), It.IsAny<string>()),
                    Times.Exactly(iolinkModulesInMockCount));
            Mock.Get(_sensorDescriptionBuilderMock.Object).Verify(x =>
                x.CreateModuleDescription(It.IsAny<IIOModule>(), It.IsAny<string>(),
                    It.IsAny<Dictionary<string, LinerecorderSensor>>(),
                    It.IsAny<Dictionary<string, LinerecorderSensor>>()),
                    Times.Exactly(iolinkModulesInMockCount));
        }

        [TearDown]
        public void ClearTestsData()
        {
            ClearCreatedFoldersIfNeed();
        }

        private void ClearCreatedFoldersIfNeed()
        {
            if (Directory.Exists(_pathToAssemblyFolder))
            {
                Directory.Delete(_pathToAssemblyFolder, true);
            }

            if (Directory.Exists(_pathToProjectFilesFolder))
            {
                Directory.Delete(_pathToProjectFilesFolder, true);
            }
        }

    }
}
