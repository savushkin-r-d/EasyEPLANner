using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Элемент может быть удален/сброшен
    /// </summary>
    public interface IDeletable : IViewItem
    {
        /// <summary>
        /// Удалить
        /// </summary>
        void Delete();
    }
}
