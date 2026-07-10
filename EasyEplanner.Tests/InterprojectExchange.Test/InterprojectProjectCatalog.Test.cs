using InterprojectExchange;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tests.InterprojectExchangeTest
{
    public class InterprojectProjectCatalogTest
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
        public void Register_AllowsLookupWithoutRescan()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "project");

            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "project", out string resolved));
            Assert.AreEqual(ProjectFolder, resolved);
        }

        [Test]
        public void TryGetProjectFolder_UsesCacheAfterFirstLookup()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "cached_project");
            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "cached_project", out string first));
            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "cached_project", out string second));
            Assert.AreEqual(first, second);
        }

        [TestCase(null)]
        [TestCase("")]
        public void TryGetProjectFolder_ReturnsFalse_WhenProjectNameEmpty(string projectName)
        {
            Assert.IsFalse(InterprojectProjectCatalog.TryGetProjectFolder(
                projectName, out string folder));
            Assert.IsNull(folder);
        }

        [Test]
        public void Register_IgnoresNullFolder()
        {
            InterprojectProjectCatalog.Register(null, "project");

            Assert.IsFalse(CatalogContains("project"));
        }

        [Test]
        public void Register_IgnoresEmptyFolder()
        {
            InterprojectProjectCatalog.Register(string.Empty, "project");

            Assert.IsFalse(CatalogContains("project"));
        }

        [Test]
        public void Register_IgnoresEmptyProjectName()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, string.Empty);

            Assert.IsFalse(CatalogContains(string.Empty));
        }

        [Test]
        public void Register_IgnoresNullProjectName()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, null);

            Assert.IsFalse(CatalogContains("project"));
        }

        [Test]
        public void Register_OverwritesPreviousPathForSameProjectName()
        {
            string otherFolder = Path.GetFullPath(Path.GetTempPath());

            InterprojectProjectCatalog.Register(ProjectFolder, "project");
            InterprojectProjectCatalog.Register(otherFolder, "project");

            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "project", out string resolved));
            Assert.AreEqual(otherFolder, resolved);
        }

        [Test]
        public void BuildIndexFromRoots_IndexesByPacName_NotFolderName()
        {
            string projectsRoot = InterprojectCatalogTestHelper.CreateProjectTree(
                "PacFromFile", "DifferentFolderName");

            try
            {
                InterprojectProjectCatalog.BuildIndexFromRoots(
                    new[] { projectsRoot });

                Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                    "PacFromFile", out string folder));
                Assert.AreEqual(
                    Path.GetFullPath(Path.Combine(projectsRoot, "DifferentFolderName")),
                    folder);
            }
            finally
            {
                InterprojectCatalogTestHelper.DeleteTree(projectsRoot);
            }
        }

        [Test]
        public void BuildIndexFromRoots_SkipsFoldersWithoutMainIo()
        {
            string projectsRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(projectsRoot);
            Directory.CreateDirectory(Path.Combine(projectsRoot, "empty_folder"));

            try
            {
                InterprojectProjectCatalog.BuildIndexFromRoots(
                    new[] { projectsRoot });

                Assert.IsFalse(CatalogContains("empty_folder"));
            }
            finally
            {
                InterprojectCatalogTestHelper.DeleteTree(projectsRoot);
            }
        }

        [Test]
        public void BuildIndexFromRoots_PreservesManualRegister()
        {
            string projectsRoot = InterprojectCatalogTestHelper.CreateProjectTree(
                "ScannedPac", "scan_folder");

            try
            {
                InterprojectProjectCatalog.Register(ProjectFolder, "manual");
                InterprojectProjectCatalog.BuildIndexFromRoots(
                    new[] { projectsRoot });

                Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                    "manual", out string manualPath));
                Assert.AreEqual(ProjectFolder, manualPath);
                Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                    "ScannedPac", out _));
            }
            finally
            {
                InterprojectCatalogTestHelper.DeleteTree(projectsRoot);
            }
        }

        [Test]
        public void Invalidate_ClearsCatalogState()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "project");
            InterprojectProjectCatalog.Invalidate();

            Assert.IsFalse(IsCatalogBuilt());
            Assert.IsNull(GetCatalogDictionary());
        }

        [Test]
        public void Invalidate_AllowsRegisterAfterReset()
        {
            InterprojectProjectCatalog.Register(ProjectFolder, "project");
            InterprojectProjectCatalog.Invalidate();
            InterprojectProjectCatalog.Register(ProjectFolder, "project_after_reset");

            Assert.IsTrue(InterprojectProjectCatalog.TryGetProjectFolder(
                "project_after_reset", out string resolved));
            Assert.AreEqual(ProjectFolder, resolved);
        }

        private static bool CatalogContains(string projectName)
        {
            var dictionary = GetCatalogDictionary();
            return dictionary != null && dictionary.ContainsKey(projectName);
        }

        private static Dictionary<string, string> GetCatalogDictionary()
        {
            return typeof(InterprojectProjectCatalog)
                .GetField("_projectNameToFolder",
                    BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null) as Dictionary<string, string>;
        }

        private static bool IsCatalogBuilt()
        {
            return (bool)typeof(InterprojectProjectCatalog)
                .GetField("_isBuilt", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        }
    }
}
