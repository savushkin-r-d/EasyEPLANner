namespace EasyEPlanner.mpk.Model
{
    /// <summary>
    /// Атрибуты
    /// </summary>
    public interface IAttributes
    {
        /// <summary>
        /// Автор
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Текущая дата
        /// </summary>
        string CurrentDate { get; }

        /// <summary>
        /// Организация
        /// </summary>
        string Organization { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// Тема
        /// </summary>
        string Theme { get; set; }
    }
}