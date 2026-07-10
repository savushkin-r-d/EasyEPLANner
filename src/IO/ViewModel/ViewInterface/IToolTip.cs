using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Подсказка по полю.
    /// </summary>
    /// <remarks>
    /// Скрывает поля <see cref="IViewItem"/>. Определяется явно.
    /// </remarks>
    public interface IToolTip : IViewItem
    {
        /// <summary>
        /// Подсказка по названию.
        /// </summary>
        new string Name { get; }

        /// <summary>
        /// Подсказка по описанию.
        /// </summary>
        new string Description { get; }
    }
}
