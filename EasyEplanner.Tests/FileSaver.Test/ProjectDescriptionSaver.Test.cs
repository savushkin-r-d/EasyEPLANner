using EasyEPlanner;
using IO;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using TechObject;
using static EasyEPlanner.ProjectDescriptionSaver;


namespace Tests.FileSaver
{
    public class ProjectDescriptionSaverTest
    {
        [TestCase("")]
        [TestCase(";./dairy-sys/?.lua")]
        public void SaveMainFile_Checks_FileExists_ProjectName_Packages(string package)
        {
            FieldInfo field = typeof(ProjectDescriptionSaver).GetField(
                "packages", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, null);

            var projectName = "ProjectName";
            var path = Path.GetTempPath();
            var pathToFile = path + "main.plua";
            var parameters = new ParametersForSave(projectName, path, false);
            
            ProjectDescriptionSaver.AddPackage(package);
            EncodingDetector.MainFilesEncoding = System.Text.Encoding.UTF8;

            var method = typeof(ProjectDescriptionSaver).GetMethod(
                "SaveMainFile", BindingFlags.NonPublic | BindingFlags.Static);
            object res = method.Invoke(null, new object[] { parameters });

            Assert.That(pathToFile, Does.Exist);

            string actualFile = File.ReadAllText(pathToFile);
            string[] actualFileData = Regex
                .Split(actualFile.ToString(), "\r\n|\n\r|\r|\n");
            
            var actualLineProjectName = actualFileData[0];
            var actualLinePackages = actualFileData[1];

            Assert.Multiple(() =>
            {
                StringAssert.Contains(projectName, actualLineProjectName);
                StringAssert.Contains(package, actualLinePackages);

                File.Delete(path + "main.plua");
            });
        }

        [TestCaseSource(nameof(AddPackageTestCase))]
        public void AddPackage_CheckListPackges(List<string> packages)
        {
            FieldInfo field = typeof(ProjectDescriptionSaver).GetField(
                "packages", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, null);

            foreach (var package in packages)
            {
                ProjectDescriptionSaver.AddPackage(package);
            }

            List<string> actualPackages = ProjectDescriptionSaver.Packages;

            Assert.Multiple(() =>
            {
                Assert.NotNull(actualPackages);
                Assert.AreEqual(packages, actualPackages);
            });
        }

        static object[] AddPackageTestCase = new object[]
        {
            new List<string>() { "package1", "package2", "package3" },
        };
    }
}
