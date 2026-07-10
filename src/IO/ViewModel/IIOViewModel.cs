using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Модель представления списка узлов и модулей
    /// </summary>
    public interface IIOViewModel
    {
        /// <summary>
        /// Менеджер узлов и модулей
        /// </summary>
        IIOManager IOManager { get; }

        /// <summary>
        /// => [ <see cref="Root"/> ]
        /// </summary>
        IEnumerable<IRoot> Roots { get; }

        /// <summary>
        /// Корень модели представления
        /// </summary>
        IRoot Root { get; }

        /// <summary>
        /// Выбранная функция клеммы в окне
        /// </summary>
        IEplanFunction SelectedClampFunction { get; set; }

        /// <summary>
        /// Выбранная клемма в окне
        /// </summary>
        IClamp SelectedClamp { get; set; }

        /// <summary>
        /// Обновить (перестроить) дерево узлов и модулей
        /// </summary>
        void RebuildTree();
    }
}
