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

        public string Name => $"{name} " +
            $"({
                Signals.GroupBy(s => $"{s.DataType}_{s.Word}")
                .Aggregate(0, (r, s) => r + (s.FirstOrDefault()?.DataType switch
                {
                    null => 0,
                    "Real" => 2,
                    _ => 1
                }))
            })";

        private IEnumerable<ISignal> Signals => items.SelectMany(i =>
        {
            if (i is ISignal s)
                return [s];
            if (i is IGroup g)
                return g.Items.OfType<ISignal>();
            return [];
        });


        public string DataType => string.Empty;

        public string Address => string.Empty;

        public void Add(IGatewayViewItem item) => items.Add(item);

        public void AddRange(IEnumerable<IGatewayViewItem> items) 
            => this.items.AddRange(items);
    }
}
