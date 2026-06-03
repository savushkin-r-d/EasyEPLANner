using InterprojectExchange;
using NUnit.Framework;
using System.IO;

namespace Tests.InterprojectExchangeTest
{
    public class MainIoProjectNameReaderTest
    {
        private static string ProjectFolder => Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "InterprojectExchange.Test",
            "TestData",
            "project");

        [Test]
        public void TryReadFromFolder_ReadsPacName()
        {
            Assert.IsTrue(MainIoProjectNameReader.TryReadFromFolder(
                ProjectFolder, out string name, out string error));
            Assert.AreEqual("project", name);
            Assert.IsNull(error);
        }
    }
}
