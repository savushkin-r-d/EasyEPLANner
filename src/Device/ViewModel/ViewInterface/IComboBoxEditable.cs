using IO.ViewModel;
using System.Collections.Generic;

namespace EasyEPlanner.Devices.ViewModel.ViewInterface
{
    public interface IComboBoxEditable : IEditable
    {
        IEnumerable<string> ComboBoxItems { get; }
    }
}
