using EasyEPlanner;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace InterprojectExchange
{
    /// <summary>
    /// Чтение PAC_name из main.io.lua.
    /// </summary>
    public static class MainIoProjectNameReader
    {
        public const string MainIoFileName = "main.io.lua";

        private static readonly Regex PacNameRegex = new Regex(
            @"PAC_name\s*=\s*['""]([^'""]*)['""]",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(100));

        public static bool TryReadFromFolder(string projectFolder,
            out string projectName, out string error)
        {
            projectName = null;
            error = null;

            string pathToIoFile = Path.Combine(projectFolder, MainIoFileName);
            if (!File.Exists(pathToIoFile))
            {
                error = $"Не найден файл {MainIoFileName} в каталоге проекта.";
                return false;
            }

            return TryReadFromFile(pathToIoFile, out projectName, out error);
        }

        public static bool TryReadFromFile(string pathToIoFile,
            out string projectName, out string error)
        {
            projectName = null;
            error = null;

            var encoding = EncodingDetector.DetectFileEncoding(pathToIoFile);
            string content = File.ReadAllText(pathToIoFile, encoding);
            var match = PacNameRegex.Match(content);
            if (!match.Success)
            {
                error = $"В файле {MainIoFileName} не задано имя проекта (PAC_name).";
                return false;
            }

            projectName = match.Groups[1].Value.Trim();
            if (string.IsNullOrEmpty(projectName))
            {
                error = $"В файле {MainIoFileName} пустое имя проекта (PAC_name).";
                return false;
            }

            return true;
        }
    }
}
