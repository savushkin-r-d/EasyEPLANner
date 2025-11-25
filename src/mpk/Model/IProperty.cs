namespace EasyEPlanner.mpk.Model
{
    /// <summary>
    /// Свойство
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// Заголовок/Комментарий
        /// </summary>
        string Caption { get; set; }
        
        /// <summary>
        /// ID канала
        /// </summary>
        int ChannelId { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Приоритет
        /// </summary>
        int Priority { get; set; }
        
        /// <summary>
        /// Тип свойства
        /// </summary>
        PropertyModel PropModel { get; set; }
    
        /// <summary>
        /// Тип значние свойства
        /// </summary>
        PropertyType PropType { get; set; }
        
        /// <summary>
        /// Протоколирование
        /// </summary>
        bool Report { get; set; }

        /// <summary>
        /// Сохраняемое
        /// </summary>
        bool Saved { get; set; }

        /// <summary>
        /// Название тега
        /// </summary>
        string TagName { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Редактирование клиентом
        /// </summary>
        bool Visible { get; set; }
    }
}