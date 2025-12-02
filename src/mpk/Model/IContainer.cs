using System.Collections.Generic;

namespace EasyEPlanner.mpk.Model
{
    /// <summary>
    /// Контейнер - корневой объект модели
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Атрибуты контейнера
        /// </summary>
        IAttributes Attributes { get; set; }
        
        /// <summary>
        /// Версия сборки
        /// </summary>
        int Build { get; set; }

        /// <summary>
        /// Список компонентов
        /// </summary>
        List<IComponent> Components { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Версия файла
        /// </summary>
        int Version { get; set; }
    }
}