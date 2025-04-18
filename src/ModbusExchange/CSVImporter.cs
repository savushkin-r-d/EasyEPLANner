using EasyEPlanner.ModbusExchange.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ModbusExchange
{
    public class CSVImporter
    {
        const string Array = "Array";
        const string Struct = "Struct";
        const string ReadStruct = "snd";
        const string WriteStruct = "rcv";
        const string Bool = "Bool";
        const string Byte = "Byte";

        private CSVImporter() { }

        public static void Import(IGroup read, IGroup write, string csvData)
        {
            var signals = csvData.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseSignal)
                .Where(NotEmpty);

            read.AddRange(Grouping(signals.SkipWhile(s => s.Name is not ReadStruct).Skip(1).TakeWhile(s => s.Name is not WriteStruct)));
            write.AddRange(Grouping(signals.SkipWhile(s => s.Name is not WriteStruct).Skip(1)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static ISignal ParseSignal(string line)
        {
            var parsedData = line.Split(';');

            (string name, string dataType, string address) = (
                parsedData.ElementAtOrDefault(0),
                parsedData.ElementAtOrDefault(1),
                parsedData.ElementAtOrDefault(2));

            var parsedAddress = address.Split([',', '.']);

            int.TryParse(parsedAddress.ElementAtOrDefault(0), out int word);
            int.TryParse(parsedAddress.ElementAtOrDefault(1), out int bit);

            return new Signal(name, dataType, word / 2,
                bit + (dataType is Bool or Byte? 8 - 8 * (word % 2) : 0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        private static bool NotEmpty(ISignal signal)
        {
            return signal.DataType != string.Empty && !signal.DataType.Contains(Array);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        /// <returns></returns>
        private static IEnumerable<IGatewayViewItem> Grouping(IEnumerable<ISignal> signals)
        {
            List<IGatewayViewItem> result = [];
            IGroup currentGroup = null;

            foreach (var signal in signals)
            {
                if (signal.DataType is Struct)
                {
                    currentGroup = new Group(signal.Name);
                    result.Add(currentGroup);
                    continue;
                }

                if (currentGroup is not null)
                    currentGroup.Add(signal);
                else
                    result.Add(signal);
            }

            return result;
        }
    }
}
