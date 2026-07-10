using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// Интерфейс для отображения в дереве
    /// </summary>
    public interface IGatewayViewItem
    {
        /// <summary>
        /// Столбец 1. "Название"
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Столбец 2. "Тип данных"
        /// </summary>
        string DataType { get; }

        /// <summary>
        /// Столбец 3. "Адрес"
        /// </summary>
        string Address { get; }
    }
}
