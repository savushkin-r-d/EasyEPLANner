using TechObject;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using EasyEPlanner;

namespace Tests.TechObject
{
    class BaseTechObjectLoaderTest
    {
        [Test]
        public void LoadBaseTechObjectsDescription_returnReadedDataFromFile()
        {
            using (StreamWriter writer = new StreamWriter("pathToFile.txt"))
            {
                writer.WriteLine("Base Tech Object Dwscription");
            }
            var baseTechObjectLoader = new BaseTechObjectLoader();
            var method = typeof(BaseTechObjectLoader).GetMethod(
                "LoadBaseTechObjectsDescription", BindingFlags.NonPublic | BindingFlags.Instance);
            object res = method.Invoke(baseTechObjectLoader, new object[] { "pathToFile.txt" });

            File.Delete("pathToFile.txt");

            Assert.AreEqual("Base Tech Object Dwscription\r\n", res);
        }

        [TestCaseSource(nameof(AddPackageTestCase))]
        public void AddPackage_CheckListPackges(List<string> packages)
        {
            FieldInfo field = typeof(ProjectDescriptionSaver).GetField(
                "packages", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, null);

            var baseTechObjectLoader = new BaseTechObjectLoader();
            foreach (var package in packages)
            {
                baseTechObjectLoader.AddPackage(package);
            }

            List<string> actualPackages = ProjectDescriptionSaver.Packages;

            Assert.NotNull(actualPackages);
            Assert.AreEqual(packages, actualPackages);
        }

        static object[] AddPackageTestCase = new object[]
        {
            new List<string>() { "package1", "package2", "package3" },
        };
    }

}