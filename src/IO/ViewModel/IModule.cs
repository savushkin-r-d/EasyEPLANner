using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Модуль (элемент дерева).
    /// </summary>
    public interface IModule : IViewItem, IHasBindingError
    {
        /// <summary>
        /// Модуль.
        /// </summary>
        IIOModule IOModule { get; }

        /// <summary>
        /// Узел.
        /// </summary>
        IIONode IONode { get; }

    }
}
