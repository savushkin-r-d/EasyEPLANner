using System.Collections.Generic;

namespace IO.ViewModel.ViewInterface
{
    /// <summary>
    /// Элемент редактируется через ComboBox.
    /// </summary>
    public interface IComboBoxEditable : IEditable
    {
        IEnumerable<string> ComboBoxItems { get; }
    }
}
