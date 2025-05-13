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

        public string Name => $"{description} ({Signals.Size()})";

        public IEnumerable<ISignal> Signals => items.SelectMany(i => i switch
        {
            ISignal s => [s],
            IGroup g => g.Signals,
            _ => []
        });


        public string DataType => string.Empty;

        public string Address => string.Empty;

        public string Description => description;

        public int Offset { get; set; } = 0;

        public void Add(IGatewayViewItem item) => items.Add(item);

        public void AddRange(IEnumerable<IGatewayViewItem> items) 
            => this.items.AddRange(items);
    }
}