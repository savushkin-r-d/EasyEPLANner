using EplanDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Signal(string description, string dataType, int word, int bit) : ISignal
    {
        public int Word => word;

        public int Bit => bit;

        public string Name => Device is not null ? Device.Name : Description;

        public string DataType => dataType;

        public string Address => $"{word}.{bit}";

        public IIODevice Device { get; set; } = null;

        public string Description => description;
    }
}
