using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Корневой элемент представления "узлы и модули ввода-вывода"
    /// </summary>
    public interface IRoot : IViewItem, IExpandable
    {
        IIOViewModel Context { get; }

    }
}
