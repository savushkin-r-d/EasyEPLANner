using NUnit.Framework;
using System.IO;
using Moq;
using EasyEPlanner.PxcIolinkConfiguration;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Tests.PxcIolinkConfigration
{
    public class XmlTemplateReaderTest
    {
        private string _pathToFolder;
        private string _folderName = "EplTemplates";
        private string _fileExtension = ".lrp";
        private Mock<IXmlSensorSerializer> _sensorSerializerMock;

        private string _wrongTemplate1FileName = "wTemplate1";
        private string _wrongTemplate2FileName = "wTemplate2";
        private string _correctTemplate1FileName = "cTemplate1";
        private string _correctTemplate2FileName = "cTemplate2";
        private string _currentTemplateVersion = "1.9.4.88";

        private int _correctTemplateCount = 2;
        private int _wrongTemplateCount = 2;

        private string _wrongTemplate1Xml;
        private string _wrongTemplate2Xml;
        private string _correctTemplate1Xml;
        private string _correctTemplate2Xml;

        [SetUp]
        public void SetUpTests()
        {
            _pathToFolder = Path.Combine(Path.GetTempPath(), _folderName);
            Directory.CreateDirectory(_pathToFolder);

            _sensorSerializerMock = new Mock<IXmlSensorSerializer>();

            _wrongTemplate1Xml = new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>")
                .Append("<Linerecorder_Sensor>")
                .Append($"<Version>{_currentTemplateVersion}</Version>")
                .Append("<Sensor>")
                .Append("<VendorId>1211</VendorId>")
                .Append("<DeviceId>5</DeviceId>")
                .Append("<ProductId>Thin</ProductId>")
                .Append("</Sensor>")
                .Append("</Linerecorder_Sensor>")
                .ToString();

            _wrongTemplate2Xml = new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>")
                .Append("<Linerecorder_Sensor>")
                .Append($"<Version>{_currentTemplateVersion}</Version>")
                .Append("<Sensor>")
                .Append("<VendorId>121</VendorId>")
                .Append("<DeviceId>54</DeviceId>")
                .Append("<ProductId>Thi44n</ProductId>")
                .Append("</Sensor>")
                .Append("</Linerecorder_Sensor>")
                .ToString();
            _correctTemplate1Xml =
                 new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>")
                .Append("<Linerecorder_Sensor  xmlns = \"http://www.ifm.com/datalink/LinerecorderSensor4\">")
                .Append($"<Version>{_currentTemplateVersion}</Version>")
                .Append("<Sensor>")
                .Append("<VendorId>1292</VendorId>")
                .Append("<DeviceId>2</DeviceId>")
                .Append("<ProductId>ThinkTop V70 IO-Link</ProductId>")
                .Append("</Sensor>")
                .Append("<Parameters>")
                .Append("<Param id=\"V_FunctionTag\" subindex=\"\" internalValue=\"***\" name=\"Function Tag\" value=\"***\" unit=\"\" />")
                .Append("</Parameters>")
                .Append("</Linerecorder_Sensor>")
                .ToString();

            _correctTemplate2Xml =
                 new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>")
                .Append("<Linerecorder_Sensor  xmlns = \"http://www.ifm.com/datalink/LinerecorderSensor4\">")
                .Append($"<Version>{_currentTemplateVersion}</Version>")
                .Append("<Sensor>")
                .Append("<VendorId>1211</VendorId>")
                .Append("<DeviceId>5</DeviceId>")
                .Append("<ProductId>Thin</ProductId>")
                .Append("</Sensor>")
                .Append("<Parameters>")
                .Append("<Param id=\"V_FunctionTag\" subindex=\"\" internalValue=\"***\" name=\"Function Tag\" value=\"***\" unit=\"\" />")
                .Append("</Parameters>")
                .Append("</Linerecorder_Sensor>")
                .ToString();
        }

        private void CreateCorrectTemplate1()
        {
            File.WriteAllText(Path.Combine(_pathToFolder,
                string.Concat(_correctTemplate1FileName, _fileExtension)), _correctTemplate1Xml);
        }

        private void CreateCorrectTemplate2()
        {
            File.WriteAllText(Path.Combine(_pathToFolder,
                string.Concat(_correctTemplate2FileName, _fileExtension)), _correctTemplate2Xml);
        }

        private void CreateWrongTemplate1()
        {
            File.WriteAllText(Path.Combine(_pathToFolder,
                string.Concat(_wrongTemplate1FileName, _fileExtension)), _wrongTemplate1Xml);
        }

        private void CreateWrongTemplate2()
        {
            File.WriteAllText(Path.Combine(_pathToFolder,
                string.Concat(_wrongTemplate2FileName, _fileExtension)), _wrongTemplate2Xml);
        }

        [Test]
        public void Read_EmptyFolder_DoNotCallDeserializeDoNotSetTemplateVersion()
        {
            var templateReader = new XmlTemplateReader(_sensorSerializerMock.Object);

            templateReader.Read(_pathToFolder, null);

            Mock.Get(_sensorSerializerMock.Object)
                .Verify(x => x.Deserialize(It.IsAny<string>()), Times.Never);
            Assert.IsNull(templateReader.TemplateVersion);
        }

        [Test]
        public void Read_WrongXml_ThrowsExceptionDoNotSetTemplateVersion()
        {
            var templateReader = new XmlTemplateReader(new XmlSensorSerializer());
            CreateWrongTemplate1();

            var dataStore = new Dictionary<string, LinerecorderSensor>();
            var tasks = templateReader.Read(_pathToFolder, dataStore);
            Task.WhenAll(tasks);

            Assert.Throws<Exception>(() => tasks.First().GetAwaiter().GetResult());
            Assert.IsNull(templateReader.TemplateVersion);
        }

        [Test]
        public void Read_AllWrongXml_ThrowsExceptionDoNotSetTemplateVersion()
        {
            var templateReader = new XmlTemplateReader(new XmlSensorSerializer());
            CreateWrongTemplate1();
            CreateWrongTemplate2();

            var dataStore = new Dictionary<string, LinerecorderSensor>();
            var tasks = templateReader.Read(_pathToFolder, dataStore);
            Task.WhenAll(tasks);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(_wrongTemplateCount, tasks.Count);
                Assert.IsNull(templateReader.TemplateVersion);
                foreach(var task in tasks)
                {
                    Assert.Throws<Exception>(() => task.GetAwaiter().GetResult());
                }
            });
        }

        [Test]
        public void Read_DuplicateTemplate_ThrowsInvalidDataExceptionDoNotSetTemplateVersion()
        {
            var templateReader = new XmlTemplateReader(new XmlSensorSerializer());
            CreateCorrectTemplate1();

            var dataStore = new Dictionary<string, LinerecorderSensor>();
            var tasks = templateReader.Read(_pathToFolder, dataStore);
            Task.WhenAll(tasks).Wait();
            var duplicateTasks = templateReader.Read(_pathToFolder, dataStore);
            Task.WhenAll(duplicateTasks);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(templateReader.TemplateVersion);
                foreach (var task in duplicateTasks)
                {
                    Assert.Throws<InvalidDataException>(() => task.GetAwaiter().GetResult());
                }
            });
        }

        [Test]
        public void Read_CorrectTemplates_FillDataStoreAndSetTemplateVersion()
        {
            var templateReader = new XmlTemplateReader(new XmlSensorSerializer());
            CreateCorrectTemplate1();
            CreateCorrectTemplate2();

            var dataStore = new Dictionary<string, LinerecorderSensor>();
            var tasks = templateReader.Read(_pathToFolder, dataStore);
            Task.WhenAll(tasks).Wait();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(_correctTemplateCount, dataStore.Count);
                Assert.AreEqual(_correctTemplateCount, tasks.Count);
                Assert.AreEqual(_currentTemplateVersion,templateReader.TemplateVersion);
                Assert.AreEqual(dataStore.First().Value.Version, templateReader.TemplateVersion);
            });
        }

        [TearDown]
        public void TearDownTests()
        {
            if(Directory.Exists(_pathToFolder))
            {
                Directory.Delete(_pathToFolder, true);
            }
        }
    }
}
