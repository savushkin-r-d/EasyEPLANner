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
        /// <summary>
        /// Адрес в словах.
        /// </summary>
        public int Word { get; }

        /// <summary>
        /// Бит.
        /// </summary>
        public int Bit { get; }
    }
}
