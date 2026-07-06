using System;
using System.IO;

namespace Tests.InterprojectExchangeTest
{
    internal static class InterprojectCatalogTestHelper
    {
        public static string CreateProjectTree(
            string pacName,
            string folderName = null)
        {
            string projectsRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string projectFolder = Path.Combine(projectsRoot, folderName ?? pacName);
            Directory.CreateDirectory(projectFolder);
            File.WriteAllText(
                Path.Combine(projectFolder, "main.io.lua"),
                $"PAC_name = '{pacName}'\nPAC_id = ''\n");
            return projectsRoot;
        }

        public static void DeleteTree(string projectsRoot)
        {
            if (Directory.Exists(projectsRoot))
            {
                Directory.Delete(projectsRoot, true);
            }
        }
    }
}
