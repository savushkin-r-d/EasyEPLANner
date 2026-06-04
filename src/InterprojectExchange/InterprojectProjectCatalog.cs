using EasyEPlanner;
using PInvoke;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace InterprojectExchange
{
    /// <summary>
    /// Индекс PAC_name → каталог проекта по всем корневым папкам из configuration.ini.
    /// Сканирование выполняется один раз до сброса кэша.
    /// </summary>
    public static class InterprojectProjectCatalog
    {
        private static readonly object Sync = new object();
        private static Dictionary<string, string> _projectNameToFolder;
        private static bool _isBuilt;

        public static void Invalidate()
        {
            lock (Sync)
            {
                _isBuilt = false;
                _projectNameToFolder = null;
            }
        }

        /// <summary>
        /// Регистрация проекта без полного пересканирования (например, после выбора папки вручную).
        /// </summary>
        public static void Register(string projectFolder, string projectName)
        {
            if (string.IsNullOrEmpty(projectFolder) || string.IsNullOrEmpty(projectName))
            {
                return;
            }

            lock (Sync)
            {
                if (_projectNameToFolder == null)
                {
                    _projectNameToFolder = new Dictionary<string, string>(
                        StringComparer.Ordinal);
                }

                _projectNameToFolder[projectName] = Path.GetFullPath(projectFolder);
            }
        }

        public static bool TryGetProjectFolder(string projectName, out string projectFolder)
        {
            projectFolder = null;
            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }

            lock (Sync)
            {
                if (_projectNameToFolder != null &&
                    _projectNameToFolder.TryGetValue(projectName, out projectFolder))
                {
                    return true;
                }

                EnsureBuilt();
                return _projectNameToFolder.TryGetValue(projectName,
                    out projectFolder);
            }
        }

        private static void EnsureBuilt()
        {
            if (_isBuilt)
            {
                return;
            }

            BuildIndexFromRoots(GetProjectsRootPaths());
        }

        /// <summary>
        /// Построить индекс PAC_name по корневым каталогам с проектами.
        /// </summary>
        public static void BuildIndexFromRoots(IEnumerable<string> projectRoots)
        {
            lock (Sync)
            {
                if (_projectNameToFolder == null)
                {
                    _projectNameToFolder = new Dictionary<string, string>(
                        StringComparer.Ordinal);
                }

                if (projectRoots != null)
                {
                    foreach (string projectsRoot in projectRoots)
                    {
                        if (!Directory.Exists(projectsRoot))
                        {
                            continue;
                        }

                        ScanProjectsRoot(projectsRoot);
                    }
                }

                _isBuilt = true;
            }
        }

        private static void ScanProjectsRoot(string projectsRoot)
        {
            foreach (string projectFolder in Directory.GetDirectories(projectsRoot))
            {
                if (!MainIoProjectNameReader.TryReadFromFolder(projectFolder,
                    out string projectName, out _))
                {
                    continue;
                }

                _projectNameToFolder[projectName] = Path.GetFullPath(projectFolder);
            }
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> GetProjectsRootPaths()
        {
            string configPath = Path.Combine(
                ProjectManager.GetInstance().OriginalAssemblyPath,
                CommonConst.ConfigFileName);

            if (!File.Exists(configPath))
            {
                yield break;
            }

            var iniFile = new IniFile(configPath);
            string[] roots = iniFile
                .ReadString("path", "folder_path", "")
                .Split(';');

            foreach (string root in roots)
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                yield return root.Trim();
            }
        }
    }
}
