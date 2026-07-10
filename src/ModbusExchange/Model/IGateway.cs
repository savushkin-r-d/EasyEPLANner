using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// Модель обмена со шлюзом
    /// </summary>
    public interface IGateway
    {
        /// <summary>
        /// Название модели
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// IP-адрес Modbus-клиента
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Порт Modbus-клиента
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Список элементов для редактора
        /// </summary>
        IEnumerable<IGatewayViewItem> Roots { get; }

        /// <summary>
        /// Группа сигналов на чтение
        /// </summary>
        IGroup Read { get; }

        /// <summary>
        /// Группа сигналов на запись
        /// </summary>
        IGroup Write { get; }
    }
}
