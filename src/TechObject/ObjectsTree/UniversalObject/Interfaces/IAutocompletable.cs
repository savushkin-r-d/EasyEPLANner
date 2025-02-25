using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Элемент дерева можно заполнить автоматически
    /// </summary>
    public interface IAutocompletable : ITreeViewItem
    {
        /// <summary>
        /// Заполнить автоматически
        /// </summary>
        void Autocomplete();
    }
}
