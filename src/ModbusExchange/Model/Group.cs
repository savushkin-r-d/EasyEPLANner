using EasyEPlanner.ModbusExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Group(string description) : IGroup
    {
        readonly List<IGatewayViewItem> items = [];

        public IEnumerable<IGatewayViewItem> Items => items;

        public string Name => $"{description} ({NestedSignals.Size()})";

        public IEnumerable<ISignal> NestedSignals => items.SelectMany(i => i switch
        {
            ISignal s => [s],
            IGroup g => g.NestedSignals,
            _ => []
        });


        public string DataType => string.Empty;

        public string Address => Offset.ToString();

        public string Description => description;

        public int Offset { get; set; } = 0;

        public void Add(IGatewayViewItem item) => items.Add(item);

        public void AddRange(IEnumerable<IGatewayViewItem> items) 
            => this.items.AddRange(items);
    }
}