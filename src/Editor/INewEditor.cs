namespace Editor
{
    /// <summary>    
    /// Интерфейс редактора.
    /// </summary>
    public interface INewEditor
    {
        /// <summary>
        /// Открыть редактор и начать редактирование
        /// </summary>
        /// <param name="objectTree">Дерево объектов</param>
        /// <returns></returns>
        void OpenEditor(ITreeViewItem objectTree);

        /// <summary>
        /// Показан ли редактор
        /// </summary>
        /// <returns></returns>
        bool IsShown();

        /// <summary>
        /// Закрыть редактор и прекратить редактирование
        /// </summary>
        void CloseEditor();
    }
}
