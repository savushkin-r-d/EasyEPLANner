using System.Collections.Generic;

namespace EasyEPlanner.mpk.Model
{
    /// <summary>
    /// Компонет
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Список сообщений
        /// </summary>
        List<IMessage> Messages { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Свойства
        /// </summary>
        List<IProperty> Properties { get; set; }
    }
}