using InterprojectExchange;
using NUnit.Framework;
using System.IO;

namespace Tests.InterprojectExchangeTest
{
    public class InterprojectExchangeLoadProjectDataTest
    {
        private static string ProjectFolder => Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "InterprojectExchange.Test",
            "TestData",
            "project");

        [SetUp]
        public void SetUp()
        {
            InterprojectExchange.InterprojectExchange.GetInstance().Clear();
            InterprojectProjectCatalog.Invalidate();
        }

        [TearDown]
        public void TearDown()
        {
            InterprojectExchange.InterprojectExchange.GetInstance().Clear();
            InterprojectProjectCatalog.Invalidate();
        }

        [Test]
        public void LoadProjectData_ReturnsFalse_WhenMainIoMissing()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            try
            {
                var exchange = InterprojectExchange.InterprojectExchange.GetInstance();

                bool loaded = exchange.LoadProjectData(tempDir, out string errors);

                Assert.IsFalse(loaded);
                Assert.IsNotNull(errors);
                Assert.IsNotEmpty(errors);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void Clear_InvalidatesProjectCatalog()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "project");

            InterprojectExchange.InterprojectExchange.GetInstance().Clear();

            var dictionaryField = typeof(InterprojectProjectCatalog)
                .GetField("_projectNameToFolder",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static);
            var isBuiltField = typeof(InterprojectProjectCatalog)
                .GetField("_isBuilt",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static);

            Assert.IsNull(dictionaryField.GetValue(null));
            Assert.IsFalse((bool)isBuiltField.GetValue(null));
        }
    }
}
