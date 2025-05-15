using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange.Model
{
    public static class SignalsSizeCalculator
    {
        /// <summary>
        /// Размер адресного пространства сигналов
        /// </summary>
        /// <param name="signals">Список сигналов</param>
        public static int Size(this IEnumerable<ISignal> signals)
        {
            return signals.GroupBy(s => $"{s.DataType}_{s.Word}")
                    .Aggregate(0, (r, s) => r + DataTypeSize(s.FirstOrDefault()?.DataType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataType">Тип сигнала</param>
        public static int DataTypeSize(string DataType) => DataType switch
        {
            null => 0,
            "Real" or "Float" => 2,
            _ => 1
        };
    }
}
