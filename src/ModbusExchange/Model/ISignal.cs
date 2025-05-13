using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISignal : IGatewayViewItem
    {
        string Description { get; }

        /// <summary>
        /// Адрес в словах.
        /// </summary>
        int Word { get; }

        /// <summary>
        /// Бит.
        /// </summary>
        int Bit { get; }

        IIODevice Device { get; set; } 
    }
}
