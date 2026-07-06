using InterprojectExchange;
using NUnit.Framework;
using System;
using System.IO;

namespace Tests.InterprojectExchangeTest
{
    public class ProjectFolderHelperTest
    {
        [TestCase(null)]
        [TestCase("")]
        public void TryGetExistingFullPath_ReturnsFalse_WhenPathEmpty(string path)
        {
            Assert.IsFalse(ProjectFolderHelper.TryGetExistingFullPath(
                path, out string fullPath));
            Assert.IsNull(fullPath);
        }

        [Test]
        public void TryGetExistingFullPath_ReturnsFalse_WhenDirectoryMissing()
        {
            string missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Assert.IsFalse(ProjectFolderHelper.TryGetExistingFullPath(
                missing, out _));
        }

        [Test]
        public void TryGetExistingFullPath_ReturnsFullPath_WhenDirectoryExists()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                Assert.IsTrue(ProjectFolderHelper.TryGetExistingFullPath(
                    tempDir, out string fullPath));
                Assert.AreEqual(Path.GetFullPath(tempDir), fullPath);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
