using System;
using System.IO;
using System.Reflection;
using InterprojectExchange;
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

        [SetUp]
        public void SetUp()
        {
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
                new object[] { Path.Combine(TestContext.CurrentContext.TestDirectory, "InterprojectExchange.Test", "TestData", "mock_script.lua") });

            Assert.IsTrue((bool)method.Invoke(ipe,
                new object[] {
                    Path.Combine(TestContext.CurrentContext.TestDirectory,
                        "InterprojectExchange.Test", "TestData", "project"),
                    "project" }));
        }

        [Test]
        public void TryResolveProjectFolder_ReturnsRegisteredFolder()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "project");

            var method = typeof(InterprojectExchangeStarter).GetMethod(
                "TryResolveProjectFolder",
                BindingFlags.NonPublic | BindingFlags.Static);
            object[] args = { "project", null };

            bool resolved = (bool)method.Invoke(null, args);

            Assert.IsTrue(resolved);
            Assert.AreEqual(ProjectFolder, (string)args[1]);
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
            var listField = typeof(InterprojectExchangeStarter).GetField(
                "_projectsNotOpenedOnLoad",
                BindingFlags.Instance | BindingFlags.NonPublic);

            addMethod.Invoke(starter, new object[] { "proj1", "причина" });

            var list = listField.GetValue(starter) as System.Collections.Generic.List<string>;
            Assert.That(list, Does.Contain("proj1: причина"));
        }
    }
}
