using InterprojectExchange;
using NUnit.Framework;
using System;
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

        [Test]
        public void TryReadFromFolder_ReturnsFalse_WhenMainIoMissing()
        {
            string tempDir = CreateTempDirectory();

            try
            {
                Assert.IsFalse(MainIoProjectNameReader.TryReadFromFolder(
                    tempDir, out string name, out string error));
                Assert.IsNull(name);
                Assert.That(error, Does.Contain(MainIoProjectNameReader.MainIoFileName));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void TryReadFromFile_ReturnsFalse_WhenPacNameMissing()
        {
            string tempDir = CreateTempDirectory();
            string ioFile = Path.Combine(tempDir, MainIoProjectNameReader.MainIoFileName);

            try
            {
                File.WriteAllText(ioFile, "PAC_id = '1'\ndevices = {}\n");

                Assert.IsFalse(MainIoProjectNameReader.TryReadFromFile(
                    ioFile, out string name, out string error));
                Assert.IsNull(name);
                Assert.That(error, Does.Contain("PAC_name"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void TryReadFromFile_ReturnsFalse_WhenPacNameEmpty()
        {
            string tempDir = CreateTempDirectory();
            string ioFile = Path.Combine(tempDir, MainIoProjectNameReader.MainIoFileName);

            try
            {
                File.WriteAllText(ioFile, "PAC_name = ''\n");

                Assert.IsFalse(MainIoProjectNameReader.TryReadFromFile(
                    ioFile, out string name, out string error));
                Assert.IsEmpty(name);
                Assert.That(error, Does.Contain("пустое"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestCase("PAC_name = \"AltProject\"", "AltProject")]
        [TestCase("pac_name = 'lowerCase'", "lowerCase")]
        public void TryReadFromFile_ReadsPacNameWithDifferentQuotes(
            string content, string expectedName)
        {
            string tempDir = CreateTempDirectory();
            string ioFile = Path.Combine(tempDir, MainIoProjectNameReader.MainIoFileName);

            try
            {
                File.WriteAllText(ioFile, content);

                Assert.IsTrue(MainIoProjectNameReader.TryReadFromFile(
                    ioFile, out string name, out string error));
                Assert.AreEqual(expectedName, name);
                Assert.IsNull(error);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        private static string CreateTempDirectory()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
    }
}
