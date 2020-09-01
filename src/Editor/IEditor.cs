namespace Editor
{
    /// <summary>    
    /// Интерфейс редактора.
    /// </summary>
    public interface IEditor
    {
        string Edit(ITreeViewItem data);
        bool IsShown();
        void CloseEditor();
    }
}
