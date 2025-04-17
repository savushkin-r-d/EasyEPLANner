using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Gateway(string name) : IGateway
    {
        public string Name => name;

        public IEnumerable<IGatewayViewItem> Roots => [Read, Write];

        public IGroup Read { get; } = new Group("Чтение");

        public IGroup Write { get; } = new Group("Запись");
    }
}
