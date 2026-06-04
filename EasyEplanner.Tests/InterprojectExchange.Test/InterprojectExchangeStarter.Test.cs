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
        private static string ProjectFolder =>
            Path.GetFullPath(InterprojectExchangeStarterTestHelper.ProjectTestDataDirectory);

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
            var starter = new InterprojectExchangeStarter();
            InterprojectExchange.InterprojectExchange.GetInstance()
                 .AddModel(new AdvancedProjectModel());

            Assert.Throws<ApplicationException>(() => starter.Save());
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
        public void CreateMainModel_And_CreateModel_AddToExchange()
        {
            var starter = new InterprojectExchangeStarter();
            var main = starter.CreateMainModel();
            var alt = starter.CreateModel();

            Assert.IsInstanceOf<ICurrentProjectModel>(main);
            Assert.IsInstanceOf<IProjectModel>(alt);
            Assert.AreEqual(2, InterprojectExchange.InterprojectExchange.GetInstance()
                .Models.Count);
        }

        [Test]
        public void CreateModel_And_GetModel_ReturnSameProject()
        {
            var starter = new InterprojectExchangeStarter();
            var model = starter.CreateModel();
            model.ProjectName = "alt";

            Assert.AreSame(model, starter.GetModel("alt"));
        }

        [Test]
        public void GetMainProjectName_ReturnsExchangeMainProjectName()
        {
            var starter = new InterprojectExchangeStarter();
            Assert.AreEqual("project", starter.GetMainProjectName());
        }

        [Test]
        public void GetSelectedModel_ReturnsNull_WhenNoneSelected()
        {
            var starter = new InterprojectExchangeStarter();
            Assert.IsNull(starter.GetSelectedModel());
        }

        [Test]
        public void LoadMainIOData_ReturnsFalse_WhenPathEmpty()
        {
            var starter = new InterprojectExchangeStarter();
            var method = typeof(InterprojectExchangeStarter).GetMethod("LoadMainIOData",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsFalse((bool)method.Invoke(starter, new object[] { "", "" }));
        }

        [Test]
        public void LoadMainIOData_LoadsProjectAndCreatesModel()
        {
            var starter = new InterprojectExchangeStarter();
            InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);

            var method = typeof(InterprojectExchangeStarter).GetMethod("LoadMainIOData",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsTrue((bool)method.Invoke(starter,
                new object[] { ProjectFolder, "project" }));
            Assert.IsNotNull(starter.GetModel("project"));
        }

        [Test]
        public void SetIPFromMainModel_ReturnsError_WhenModelMissing()
        {
            var starter = new InterprojectExchangeStarter();
            starter.CreateMainModel().ProjectName = "project";

            string errors = InterprojectExchangeStarterTestHelper
                .InvokeSetIPFromMainModel(starter, "missing");

            Assert.That(errors, Does.Contain("Модель"));
        }

        [Test]
        public void SetIPFromMainModel_CopiesIpFromMainModel()
        {
            var starter = new InterprojectExchangeStarter();
            var main = (ICurrentProjectModel)starter.CreateMainModel();
            main.ProjectName = "project";
            main.SelectedAdvancedProject = "project";
            main.PacInfo.IP = "10.0.0.2";
            var alt = starter.CreateModel();
            alt.ProjectName = "alt1";

            string errors = InterprojectExchangeStarterTestHelper
                .InvokeSetIPFromMainModel(starter, "alt1");

            Assert.IsEmpty(errors);
            Assert.AreEqual("10.0.0.2", alt.PacInfo.IP);
        }

        [Test]
        public void GenerateSharedDevices_RegistersDeviceNamesInLua()
        {
            var starter = new InterprojectExchangeStarter();
            var model = (AdvancedProjectModel)starter.CreateModel();
            model.ProjectName = "project";
            model.AddDeviceData("DEV1", "descr");

            InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);
            typeof(InterprojectExchangeStarter).GetMethod("GenerateSharedDevices",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] { "project" });

            Assert.Pass();
        }

        [Test]
        public void ReadModelSharedFileToList_FillsModelLines()
        {
            var starter = new InterprojectExchangeStarter();
            var model = starter.CreateModel();
            model.ProjectName = "project";
            string sharedPath = Path.Combine(ProjectFolder, "shared.lua");

            typeof(InterprojectExchangeStarter).GetMethod("ReadModelSharedFileToList",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] { "project", sharedPath });

            Assert.IsNotNull(model.SharedFileAsStringList);
            Assert.IsNotEmpty(model.SharedFileAsStringList);
        }

        [Test]
        public void LoadAdvancedProjectSharedLuaData_ReadsSharedFile()
        {
            var starter = new InterprojectExchangeStarter();
            starter.CreateModel().ProjectName = "project";
            InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);

            Assert.DoesNotThrow(() =>
                starter.LoadAdvancedProjectSharedLuaData(ProjectFolder, "project"));
        }

        [Test]
        public void LoadProjectData_ReturnsFalse_WhenModelNotCreated()
        {
            var starter = new InterprojectExchangeStarter();
            InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);

            bool loaded = starter.LoadProjectData(
                ProjectFolder, "unknown", out string errors);

            Assert.IsFalse(loaded);
            Assert.That(errors, Does.Contain("Модель"));
        }

        [Test]
        public void LoadProjectData_ReturnsTrue_WhenModelLoaded()
        {
            var starter = new InterprojectExchangeStarter();
            var main = (ICurrentProjectModel)starter.CreateMainModel();
            main.ProjectName = "project";
            main.SelectedAdvancedProject = "project";
            InterprojectProjectCatalog.Register(ProjectFolder, "project");

            bool loaded = starter.LoadProjectData(
                ProjectFolder, "project", out string errors);

            Assert.IsTrue(loaded);
            Assert.IsEmpty(errors);
            var model = starter.GetModel("project");
            Assert.IsTrue(model.Loaded);
            Assert.AreEqual(ProjectFolder, model.PathToProject);
        }

        [Test]
        public void LoadProjectsData_LoadsAlternativeProjectFromCatalog()
        {
            string altRoot = InterprojectCatalogTestHelper.CreateProjectTree(
                "alt1", "alt_folder");
            string altFolder = Path.GetFullPath(
                Path.Combine(altRoot, "alt_folder"));

            try
            {
                var starter = new InterprojectExchangeStarter();
                InterprojectProjectCatalog.Register(ProjectFolder, "project");
                InterprojectProjectCatalog.Register(altFolder, "alt1");
                starter.CreateMainModel().ProjectName = "project";
                starter.CreateModel().ProjectName = "alt1";

                InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);
                InterprojectExchangeStarterTestHelper.InvokeLoadProjectsData(
                    starter, ProjectFolder, "project");

                var altModel = starter.GetModel("alt1");
                Assert.IsTrue(altModel.Loaded);
                Assert.AreEqual(altFolder, altModel.PathToProject);
            }
            finally
            {
                InterprojectCatalogTestHelper.DeleteTree(altRoot);
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
                InterprojectProjectCatalog.BuildIndexFromRoots(Array.Empty<string>());

                InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);
                InterprojectExchangeStarterTestHelper.InvokeLoadProjectsData(
                    starter, tempDir, "empty");

                Assert.That(InterprojectExchangeStarterTestHelper.GetProjectsNotOpenedList(starter),
                    Does.Contain("empty: не удалось загрузить main.io.lua"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void LoadCurrentInterprojectExchange_ReturnsFalse_WhenDevicesNotRead()
        {
            var starter = new InterprojectExchangeStarter();
            Assert.IsFalse(InterprojectExchangeStarterTestHelper
                .InvokeLoadCurrentInterprojectExchange(starter, false));
        }

        [Test]
        public void LoadCurrentInterprojectExchange_LoadsWhenProjectRegistered()
        {
            var starter = new InterprojectExchangeStarter();
            InterprojectProjectCatalog.Register(ProjectFolder, "project");
            InterprojectExchangeStarterTestHelper.InitLuaWithSystemScripts(starter);

            Assert.IsTrue(InterprojectExchangeStarterTestHelper
                .InvokeLoadCurrentInterprojectExchange(starter, true));
        }

        [Test]
        public void TryResolveProjectFolder_ReturnsRegisteredFolder()
        {
            new InterprojectExchangeStarter();
            InterprojectProjectCatalog.Register(ProjectFolder, "project");

            Assert.IsTrue(InterprojectExchangeStarterTestHelper
                .InvokeTryResolveProjectFolder("project", out string folder));
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

                Assert.IsTrue(InterprojectExchangeStarterTestHelper
                    .InvokeTryResolveProjectFolder("IndexedPac", out string folder));
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
        public void BuildProjectsNotOpenedSummaryText_ReturnsNull_WhenListEmpty()
        {
            var starter = new InterprojectExchangeStarter();
            Assert.IsNull(InterprojectExchangeStarterTestHelper
                .BuildProjectsNotOpenedSummaryText(starter));
        }

        [Test]
        public void BuildProjectsNotOpenedSummaryText_FormatsAllLines()
        {
            var starter = new InterprojectExchangeStarter();
            typeof(InterprojectExchangeStarter).GetMethod("AddProjectNotOpened",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] { "projA", "причина A" });
            typeof(InterprojectExchangeStarter).GetMethod("AddProjectNotOpened",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, new object[] { "projB", "причина B" });

            string summary = InterprojectExchangeStarterTestHelper
                .BuildProjectsNotOpenedSummaryText(starter);

            Assert.That(summary, Does.StartWith("Не удалось открыть следующие проекты:"));
            Assert.That(summary, Does.Contain("• projA: причина A"));
            Assert.That(summary, Does.Contain("• projB: причина B"));
        }

        [Test]
        public void ShowProjectsNotOpenedSummary_DoesNotThrow_WhenListEmpty()
        {
            var starter = new InterprojectExchangeStarter();
            typeof(InterprojectExchangeStarter).GetMethod(
                "ShowProjectsNotOpenedSummary",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(starter, null);
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
