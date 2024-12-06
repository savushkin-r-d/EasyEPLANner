using EasyEPlanner.ProjectImportICP;
using EplanDevice;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyEplanner.Tests.ProjectImportICP.Test
{
    public class XmlNodeChabaseExtension
    {
        [Test]
        public void UpdateDeviceTags()
        {
            var xmlDoc = new XmlDocument();
            var expectedXmlDoc = new XmlDocument();
            
            xmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "UpdateDevicesTags.src.xml"));
            expectedXmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "UpdateDevicesTags.expected.xml"));

            var srcRoot = xmlDoc.DocumentElement;

            var V = new V("", "", "", 1, "", 1, "");
            var V_no_st = new V("", "", "", 1, "", 1, "");

            V.SetSubType("V_DO1");

            srcRoot.UpdateDeviceTags(new List<(string, string)>()
            {
                ("TANK32V1", "V3201"),
                ("TANK32V2", "V3202"),
                ("TANK32V3", "V3203"),
                ("TANK32V4", "V3204"),
                ("TANK32V5", null),
            }, 
            Mock.Of<IDeviceManager>(m =>
                m.GetDevice(It.IsAny<string>()) == V_no_st &&
                m.GetDevice("TANK32V1") == V &&
                m.GetDevice("TANK32V3") == V));


            Assert.AreEqual(expectedXmlDoc.InnerXml, xmlDoc.InnerXml);
        }

        [Test]
        public void ShiftIds()
        {
            var xmlDoc = new XmlDocument();
            var expectedXmlDoc = new XmlDocument();

            xmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "ShiftIds.dst.xml"));
            expectedXmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "ShiftIds.expected.xml"));

            var root = xmlDoc.DocumentElement;
            root.ShiftIds(0x25, 0x10);

            Assert.AreEqual(expectedXmlDoc.InnerXml, xmlDoc.InnerXml);
        }

        [Test]
        public void InsertSubtypes()
        {
            var srcXmlDoc = new XmlDocument();
            var dstXmlDoc = new XmlDocument();
            var expectedXmlDoc = new XmlDocument();

            srcXmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "Insert.src.xml"));
            dstXmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "Insert.dst.xml"));
            expectedXmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "Insert.expected.xml"));

            var srcRoot = srcXmlDoc.DocumentElement;
            var dstRoot = dstXmlDoc.DocumentElement;

            dstRoot.InsertSubtypes(srcRoot);

            Assert.AreEqual(expectedXmlDoc.InnerXml, dstXmlDoc.InnerXml);
        }

        [Test]
        public void DisableTags()
        {
            var xmlDoc = new XmlDocument();
            var expectedXmlDoc = new XmlDocument();

            xmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "DisableTags.src.xml"));
            expectedXmlDoc.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectImportICP.Test", "TestData", "DisableTags.expected.xml"));

            var root = xmlDoc.DocumentElement;
            root.DisableTags("//subtypes:enabled/text()");

            Assert.AreEqual(expectedXmlDoc.InnerXml, xmlDoc.InnerXml);
        }
    }
}
