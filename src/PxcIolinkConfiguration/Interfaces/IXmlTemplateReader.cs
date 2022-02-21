using System.Collections.Generic;
using System.Threading.Tasks;
using EasyEPlanner.PxcIolinkConfiguration.Models;

namespace EasyEPlanner.PxcIolinkConfiguration.Interfaces
{
    public interface IXmlTemplateReader
    {
        /// <summary>
        /// Возвращает список тасок с чтением шаблонов в dataStore. Таски
        /// уже запущены.
        /// </summary>
        /// <param name="pathToFolder">Путь к каталогу с шаблонами</param>
        /// <param name="dataStore">Хранилище, куда будут читаться шаблоны</param>
        /// <returns></returns>
        List<Task> Read(string pathToFolder,
            Dictionary<string, LinerecorderSensor> dataStore);

        /// <summary>
        /// Возвращает версию шаблона из первого считанного файла-шаблона.
        /// </summary>
        string TemplateVersion { get; }
    }
}
