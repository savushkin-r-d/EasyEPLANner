using EasyEPlanner.ModbusExchange.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange
{
    /// <summary>
    /// Импорт модели обмена со шлюзом из CSV файла.
    /// </summary>
    public static class CsvImporter
    {
        /// <summary>
        /// Тип сигнала - Array.
        /// </summary>
        const string Array = "Array";

        /// <summary>
        /// Тип сигнала - Struct (начальный сигнал группы сигналов).
        /// </summary>
        const string Struct = "Struct";

        /// <summary>
        /// Тип сигнала - Bool.
        /// </summary>
        const string Bool = "Bool";

        /// <summary>
        /// Тип сигнала - Byte.
        /// </summary>
        const string Byte = "Byte";

        /// <summary>
        /// Название сигнала - начальный сигнал группы чтения.
        /// </summary>
        const string ReadStruct = "snd";

        /// <summary>
        /// Название сигнала - начальный сигнал группы записи.
        /// </summary>
        const string WriteStruct = "rcv";

        /// <summary>
        /// Импортировать модель обмена из CSV файла.
        /// </summary>
        /// <param name="model"></param>
        [ExcludeFromCodeCoverage]
        public static void ImportCSV(this IGateway model)
        {
            if (model is null)
                return;

            var openFileDialog = new OpenFileDialog
            {
                Title = "CSV структура шлюза",
                Filter = "CSV|*.csv",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() is DialogResult.Cancel)
            {
                return;
            }

            using StreamReader reader = new(openFileDialog.FileName, EncodingDetector.DetectFileEncoding(openFileDialog.FileName), true);

            var csvData = reader.ReadToEnd();

            model.Parse(csvData);
        }

        /// <summary>
        /// Разобрать данные CSV-файла по группам модели шлюза.
        /// </summary>
        /// <param name="model">Модель обмена шлюза</param>
        /// <param name="csvData">Данные CSV файла</param>
        public static void Parse(this IGateway model, string csvData)
        {
            var read = model.Read;
            var write = model.Write;

            var signals = csvData.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseSignal)
                .Where(NotSkip);

            read.AddRange(Grouping(signals.SkipWhile(s => s.Name is not ReadStruct).Skip(1).TakeWhile(s => s.Name is not WriteStruct)));
            write.AddRange(Grouping(signals.SkipWhile(s => s.Name is not WriteStruct).Skip(1)));
            
            RecalculateAddressGroup(read);
            RecalculateAddressGroup(write);
        }


        /// <summary>
        /// Разобрать строку в <see cref="ISignal">Сигнал</see>.
        /// </summary>
        /// <param name="line">строка CSV</param>
        private static ISignal ParseSignal(string line)
        {
            var parsedData = line.Split(';');

            (string name, string dataType, string address) = (
                parsedData.ElementAtOrDefault(0),
                parsedData.ElementAtOrDefault(1),
                parsedData.ElementAtOrDefault(2));

            var parsedAddress = address.Split(',', '.');

            int.TryParse(parsedAddress.ElementAtOrDefault(0), out int word);
            int.TryParse(parsedAddress.ElementAtOrDefault(1), out int bit);

            return new Signal(name, dataType, word / 2,
                bit + (dataType is Bool or Byte? 8 - 8 * (word % 2) : 0));
        }

        /// <summary>
        /// Не пропускать сигнал.
        /// </summary>
        /// <param name="signal"></param>
        private static bool NotSkip(ISignal signal)
        {
            return signal.DataType != string.Empty && !signal.DataType.Contains(Array);
        }

        /// <summary>
        /// Группировка сигналов.
        /// </summary>
        /// <param name="signals">Список сигналов</param>
        /// <returns>Сгруппированный список</returns>
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

        /// <summary>
        /// Пересчет адресов по группам.
        /// </summary>
        /// <param name="group">Группа сигналов</param>
        /// <param name="parentOffset">Отступ адреса внешней группы</param>
        private static void RecalculateAddressGroup(IGroup group, int parentOffset = 0)
        {
            var signals = group.Items.OfType<ISignal>();
            if (!signals.Any())
                return;

            var offset = group.NestedSignals.FirstOrDefault()?.Word ?? 0;

            foreach (var signal in signals)
            {
                signal.Word -= offset;
            }

            group.Offset = offset - parentOffset;

            foreach (var subgroup in group.Items.OfType<IGroup>())
            {
                RecalculateAddressGroup(subgroup, group.Offset);
            }
        }
    }
}
