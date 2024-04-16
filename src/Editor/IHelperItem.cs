namespace Editor
{
    /// <summary>
    /// Интерфейс для получения ссылки на ресурс справки по элементу
    /// </summary>
    public interface IHelperItem
    {
        /// <summary>
        /// Получить ссылку на ресурс справки по элементу
        /// </summary>
        /// <returns></returns>
        string GetLinkToHelpPage();

        /// <summary>
        /// Системный идентификатор в БЗ (sys_id)
        /// </summary>
        string SystemIdentifier { get; }
    }
}
