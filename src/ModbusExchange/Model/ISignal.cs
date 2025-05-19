using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// Обмен с сигналом.
    /// </summary>
    public interface ISignal : IGatewayViewItem
    {
        string Description { get; }

        /// <summary>
        /// Адрес в словах.
        /// </summary>
        int Word { get; set; }

        /// <summary>
        /// Бит.
        /// </summary>
        int Bit { get; }

        /// <summary>
        /// Сигнал для обмена
        /// </summary>
        IIODevice Device { get; set; } 
    }
}
