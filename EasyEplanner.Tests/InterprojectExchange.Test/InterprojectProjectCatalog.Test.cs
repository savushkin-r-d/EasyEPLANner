using InterprojectExchange;
using NUnit.Framework;
using System.IO;

namespace Tests.InterprojectExchangeTest
{
    public class InterprojectProjectCatalogTest
    {
        [SetUp]
        public void SetUp()
        {
            InterprojectProjectCatalog.Invalidate();
        }

        [Test]
        public void Register_AllowsLookupWithoutRescan()
        {
            string folder = Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "InterprojectExchange.Test",
                "TestData",
                "project"));

            InterprojectProjectCatalog.Register(folder, "project");

            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "project", out string resolved));
            Assert.AreEqual(folder, resolved);
        }

        [Test]
        public void TryGetProjectFolder_UsesCacheAfterFirstLookup()
        {
            string folder = Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "InterprojectExchange.Test",
                "TestData",
                "project"));

            InterprojectProjectCatalog.Register(folder, "cached_project");
            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "cached_project", out string first));
            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "cached_project", out string second));
            Assert.AreEqual(first, second);
        }
    }
}
