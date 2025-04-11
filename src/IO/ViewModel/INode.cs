using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Узел (элемент дерева).
    /// </summary>
    public interface INode : IViewItem
    {
        /// <summary>
        /// Узел.
        /// </summary>
        IIONode IONode { get; }
    }
}
