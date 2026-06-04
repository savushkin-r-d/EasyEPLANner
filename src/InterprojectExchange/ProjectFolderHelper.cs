using System.IO;

namespace InterprojectExchange
{
    /// <summary>
    /// Проверка и нормализация пути к каталогу проекта.
    /// </summary>
    public static class ProjectFolderHelper
    {
        public static bool TryGetExistingFullPath(string path, out string fullPath)
        {
            fullPath = null;
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            fullPath = Path.GetFullPath(path);
            return Directory.Exists(fullPath);
        }
    }
}
