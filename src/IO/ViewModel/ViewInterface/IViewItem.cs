using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Элемент модели представления
    /// Отображаемые свойства
    /// </summary>
    public interface IViewItem
    {
        /// <summary>
        /// Название
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; }
    }
}
