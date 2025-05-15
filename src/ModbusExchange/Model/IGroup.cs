using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// Группа сигналов
    /// </summary>
    public interface IGroup : IGatewayViewItem
    {
        /// <summary>
        /// Описание - название группы
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Элементы группы (сигналы/подгруппы)
        /// </summary>
        IEnumerable<IGatewayViewItem> Items { get; }

        /// <summary>
        /// Добавить элемент в группу
        /// </summary>
        void Add(IGatewayViewItem item);

        /// <summary>
        /// Добавить множество элементов в группу
        /// </summary>
        void AddRange(IEnumerable<IGatewayViewItem> items);

        /// <summary>
        /// Все сигналы группы (включая сигналы подгрупп)
        /// </summary>
        IEnumerable<ISignal> NestedSignals { get; }

        /// <summary>
        /// Отступ адресного пространства в словах
        /// </summary>
        int Offset { get; set; }
    }
}
