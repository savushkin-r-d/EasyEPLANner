using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Group(string name) : IGroup
    {
        readonly List<IGatewayViewItem> items = [];

        public IEnumerable<IGatewayViewItem> Items => items;

        public string Name => name;

        public string DataType => string.Empty;

        public string Address => string.Empty;

        public void Add(IGatewayViewItem item) => items.Add(item);

        public void AddRange(IEnumerable<IGatewayViewItem> items) 
            => this.items.AddRange(items);
    }
}
