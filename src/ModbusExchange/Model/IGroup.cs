using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public interface IGroup : IGatewayViewItem
    {
        string Description { get; }

        IEnumerable<IGatewayViewItem> Items { get; }

        void Add(IGatewayViewItem item);

        void AddRange(IEnumerable<IGatewayViewItem> items);

        IEnumerable<ISignal> Signals { get; }

        int Offset { get; set; }
    }
}
