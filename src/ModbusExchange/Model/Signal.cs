using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public class Signal(string name, string dataType, int word, int bit) : ISignal
    {
        public int Word => word;

        public int Bit => bit;

        public string Name => name;

        public string DataType => dataType;

        public string Address => $"{word}.{bit}";
    }
}
