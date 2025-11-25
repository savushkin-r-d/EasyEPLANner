namespace EasyEPlanner.mpk.Model
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        int Priority { get; set; }
        
        /// <summary>
        /// Протоколирование
        /// </summary>
        bool Report { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        MessageType Type { get; set; }

        /// <summary>
        /// Инициировать клиентом
        /// </summary>
        bool Visible { get; set; }
    }
}