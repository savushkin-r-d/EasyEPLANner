namespace IO.ViewModel
{
    /// <summary>
    /// Элемент содержит ошибку привязки у себя или у потомков.
    /// </summary>
    public interface IHasBindingError
    {
        /// <summary>
        /// Есть ошибка привязки.
        /// </summary>
        bool HasBindingError { get; }
    }
}
