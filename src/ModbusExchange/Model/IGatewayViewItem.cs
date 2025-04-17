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
        public string Name { get; }

        /// <summary>
        /// Столбец 2. "Тип данных"
        /// </summary>
        public string DataType { get; }

        /// <summary>
        /// Столбец 3. "Адрес"
        /// </summary>
        public string Address { get; }
    }
}
