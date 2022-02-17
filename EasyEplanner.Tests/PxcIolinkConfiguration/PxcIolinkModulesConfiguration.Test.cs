using Moq;
using NUnit.Framework;
using EasyEPlanner.PxcIolinkConfiguration;
using System.IO;

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

        [SetUp]
        public void SetUpMock()
        {
            _templateReaderMock = new Mock<IXmlTemplateReader>();
            _sensorDescriptionBuilderMock = new Mock<ISensorDescriptionBuilder>();
            _sensorSerializerMock = new Mock<IXmlSensorSerializer>();
            string tempPath = Path.GetTempPath();
            _pathToAssemblyFolder = Path.Combine(tempPath, "EplAssemblyFldr");
            _pathToProjectFilesFolder = Path.Combine(tempPath, "EplProjFldr");
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
