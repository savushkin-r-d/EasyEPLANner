using Editor;

namespace TechObject
{
    /// <summary>
    /// Интерфейс объекта дерева с дополнительным текстом всплывающей подсказки.
    /// </summary>
    internal interface IToolTip : ITreeViewItem
    {
        /// <summary>
        /// Дополнительный текст подсказки по колонкам дерева:
        /// Name - левая колонка, Value - правая колонка.
        /// Редактор всегда показывает <see cref="ITreeViewItem.DisplayText"/>,
        /// а этот текст добавляет после него, если он задан для колонки.
        /// </summary>
        (string Name, string Value) ToolTipText { get; }
    }
}
