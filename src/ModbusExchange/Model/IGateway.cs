using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public interface IGateway
    {
        public string Name { get; }

        IEnumerable<IGatewayViewItem> Roots { get; }

        IGroup Read { get; }

        IGroup Write { get; }
    }
}
