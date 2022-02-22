using IO;

namespace EasyEPlanner.PxcIolinkConfiguration.Interfaces
{
    public interface IPxcIolinkConfiguration
    {
        /// <summary>
        /// Создать директории для чтения/записи файлов, если они не созданы
        /// </summary>
        /// <returns>Директории созданы</returns>
        /// <param name="assemblyPath">Путь к dll с надстройкой (оттуда читаем)</param>
        /// <param name="projectFilesPath">Путь к файлам проекта .lua (куда пишем)</param>
        bool CreateFolders(string assemblyPath, string projectFilesPath);

        /// <summary>
        /// Сгенерировать описание модулей ввода-вывода и записать их каталог
        /// </summary>
        /// <param name="manager">Менеджер устройств ввода-вывода</param>
        /// <param name="templatesLoaded">Загружены шаблоны или нет</param>
        void CreateModulesDescription(bool templatesLoaded, IIOManager manager);

        /// <summary>
        /// Считать шаблоны с описанием модулей ввода-вывода, устройств.
        /// </summary>
        /// <returns>Шаблоны считаны</returns>
        /// <param name="foldersCreated">Созданы каталоги или нет</param>
        bool ReadTemplates(bool foldersCreated);
    }
}
