using IO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Элемент можно редактировать.
    /// </summary>
    public interface IEditable : IViewItem
    {
        /// <summary>
        /// Редактируемое значение.
        /// </summary>
        /// <remarks>
        /// Редактор запрашивает данное значение при активации редактора ячейки.
        /// </remarks>
        string Value { get; }

        /// <summary>
        /// Установить новое значение.
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <returns>Элемент отредактирован.</returns>
        bool SetValue(string value);
    }
}