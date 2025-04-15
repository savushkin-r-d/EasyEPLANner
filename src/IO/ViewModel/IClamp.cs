using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Клемма.
    /// </summary>
    public interface IClamp : IViewItem
    {
        /// <summary>
        /// Функция клеммы на ФСА.
        /// </summary>
        IEplanFunction ClampFunction { get; }
        
        /// <summary>
        /// Узел.
        /// </summary>
        IIONode Node { get; }

        /// <summary>
        /// Модуль.
        /// </summary>
        IIOModule Module { get; }
        
        /// <summary>
        /// Сброс привязки клеммы в программе (не функционального текста)
        /// </summary>
        void Reset();
    }
}
