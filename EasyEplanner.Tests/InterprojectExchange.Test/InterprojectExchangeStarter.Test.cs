using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EasyEPlanner;
using InterprojectExchange;
using Moq;
using NUnit.Framework;

namespace Tests.InterprojectExchangeTest
{
    public class InterprojectExchangeStarterTest
    {
        private static string ProjectFolder => Path.GetFullPath(Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "InterprojectExchange.Test",
            "TestData",
            "project"));

        private static string TestDataDirectory => Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "InterprojectExchange.Test",
            "TestData");

        [SetUp]
        public void SetUp()
        {
            InterprojectProjectCatalog.Invalidate();
            SetMainProjectName("project");
        }

        [TearDown]
        public void TearDown()
        {
            ResetMainProjectName();
            InterprojectProjectCatalog.Invalidate();
        }

        [Test]
        public void SaveTest()
        {
            var ipe = new InterprojectExchangeStarter();
            InterprojectExchange.InterprojectExchange.GetInstance()
                 .AddModel(new AdvancedProjectModel());

            Assert.Throws<ApplicationException>(() => ipe.Save());
        }

        [Test]
        public void LoadMainIOData_undefined()
        {
            var ipe = new InterprojectExchangeStarter();

            var method = typeof(InterprojectExchangeStarter).GetMethod("LoadMainIOData",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsFalse((bool)method.Invoke(ipe, new object[] { "", "" }));
        }

        [Test]
        public void LoadMainIOData()
        {
            var ipe = new InterprojectExchangeStarter();

            var initLua = typeof(InterprojectExchangeStarter).GetMethod("InitLuaInstance",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var loadScript = typeof(InterprojectExchangeStarter).GetMethod("LoadScript",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var method = typeof(InterprojectExchangeStarter).GetMethod("LoadMainIOData",
                BindingFlags.Instance | BindingFlags.NonPublic);

            initLua.Invoke(ipe, null);
            loadScript.Invoke(ipe,
                new object[] { Path.Combine(TestDataDirectory, "mock_script.lua") });

            Assert.IsTrue((bool)method.Invoke(ipe,
                new object[] { Path.Combine(TestDataDirectory, "project"), "project" }));
        }

        [Test]
        public void CheckProjectData_ReturnsTrue_WhenMainIoExists()
        {
            var starter = new InterprojectExchangeStarter();

            Assert.IsTrue(starter.CheckProjectData(ProjectFolder));
        }

        [Test]
        public void CheckProjectData_ReturnsFalse_WhenMainIoMissing()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var starter = new InterprojectExchangeStarter();
                Assert.IsFalse(starter.CheckProjectData(tempDir));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void TryResolveProjectFolder_ReturnsRegisteredFolder()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "project");

            Assert.IsTrue(InvokeTryResolveProjectFolder("project", out string folder));
            Assert.AreEqual(ProjectFolder, folder);
        }

        [Test]
        public void TryResolveProjectFolder_FindsProject_FromBuiltCatalog()
        {
            string projectsRoot = InterprojectCatalogTestHelper.CreateProjectTree(
                "IndexedPac", "AnyFolder");

            try
            {
                InterprojectProjectCatalog.BuildIndexFromRoots(
                    new[] { projectsRoot });

                Assert.IsTrue(InvokeTryResolveProjectFolder(
                    "IndexedPac", out string folder));
                Assert.AreEqual(
                    Path.GetFullPath(Path.Combine(projectsRoot, "AnyFolder")),
                    folder);
            }
            finally
            {
                InterprojectCatalogTestHelper.DeleteTree(projectsRoot);
            }
        }

        [Test]
        public void LoadProjectsData_AddsNotOpened_WhenMainIoMissing()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var starter = new InterprojectExchangeStarter();
                starter.CreateMainModel().ProjectName = "empty";
                InterprojectProjectCatalog.BuildIndexFromRoots(
                    Array.Empty<string>());

                InitLuaForStarter(starter);
                Assert.DoesNotThrow(() =>
                    InvokeLoadProjectsData(starter, tempDir, "empty"));

                var notOpened = GetProjectsNotOpenedList(starter);
                Assert.That(notOpened,
                    Does.Contain("empty: не удалось загрузить main.io.lua"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void ShowProjectsNotOpenedSummary_BuildsExpectedLines()
        {
            var starter = new InterprojectExchangeStarter();
            var addMethod = typeof(InterprojectExchangeStarter).GetMethod(
                "AddProjectNotOpened",
                BindingFlags.Instance | BindingFlags.NonPublic);

            addMethod.Invoke(starter, new object[] { "projA", "причина A" });
            addMethod.Invoke(starter, new object[] { "projB", "причина B" });

            var list = GetProjectsNotOpenedList(starter);
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list, Does.Contain("projA: причина A"));
            Assert.That(list, Does.Contain("projB: причина B"));
        }

        [Test]
        public void ShowProjectsNotOpenedSummary_DoesNotThrow_WhenListEmpty()
        {
            var starter = new InterprojectExchangeStarter();
            var method = typeof(InterprojectExchangeStarter).GetMethod(
                "ShowProjectsNotOpenedSummary",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.DoesNotThrow(() => method.Invoke(starter, null));
        }

        [Test]
        public void AddProjectNotOpened_AddsEntryToSummaryList()
        {
            var starter = new InterprojectExchangeStarter();
            var addMethod = typeof(InterprojectExchangeStarter).GetMethod(
                "AddProjectNotOpened",
                BindingFlags.Instance | BindingFlags.NonPublic);

            addMethod.Invoke(starter, new object[] { "proj1", "причина" });

            Assert.That(GetProjectsNotOpenedList(starter),
                Does.Contain("proj1: причина"));
        }

        [Test]
        public void CreateModel_And_GetModel_ReturnSameProject()
        {
            var starter = new InterprojectExchangeStarter();
            var model = starter.CreateModel();
            model.ProjectName = "alt";

            Assert.AreSame(model, starter.GetModel("alt"));
        }

        private static void InitLuaForStarter(InterprojectExchangeStarter starter)
        {
            typeof(InterprojectExchangeStarter).GetMethod("InitLuaInstance",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, null);
            typeof(InterprojectExchangeStarter).GetMethod("LoadScript",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] {
                    Path.Combine(TestDataDirectory, "mock_script.lua") });
        }

        private static void InvokeLoadProjectsData(
            InterprojectExchangeStarter starter,
            string projectFolder,
            string projectName)
        {
            typeof(InterprojectExchangeStarter).GetMethod(
                "LoadProjectsData",
                BindingFlags.Instance | BindingFlags.Public)
                .Invoke(starter, new object[] { projectFolder, projectName });
        }

        private static List<string> GetProjectsNotOpenedList(
            InterprojectExchangeStarter starter)
        {
            return typeof(InterprojectExchangeStarter).GetField(
                "_projectsNotOpenedOnLoad",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(starter) as List<string>;
        }

        private static bool InvokeTryResolveProjectFolder(
            string projectName,
            out string folder)
        {
            var method = typeof(InterprojectExchangeStarter).GetMethod(
                "TryResolveProjectFolder",
                BindingFlags.NonPublic | BindingFlags.Static);
            object[] args = { projectName, null };
            bool resolved = (bool)method.Invoke(null, args);
            folder = (string)args[1];
            return resolved;
        }

        private static void SetMainProjectName(string projectName)
        {
            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("eProjectManager",
                    BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, Mock.Of<IEProjectManager>(m =>
                    m.GetModifyingCurrentProjectName() == projectName));
        }

        private static void ResetMainProjectName()
        {
            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("eProjectManager",
                    BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, null);
            typeof(InterprojectExchange.InterprojectExchange)
                .GetField("interprojectExchange",
                    BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, null);
        }
    }
}
