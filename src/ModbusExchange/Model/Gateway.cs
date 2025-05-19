using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Gateway(string name) : IGateway
    {
        public const string READ = "Чтение";

        public const string WRITE = "Запись";

        public string Name => name;

        public IEnumerable<IGatewayViewItem> Roots => [Read, Write];

        public IGroup Read { get; } = new Group(READ);

        public IGroup Write { get; } = new Group(WRITE);

        public string IP { get; set; } = string.Empty;

        public int Port { get; set; } = 0;
    }
}
