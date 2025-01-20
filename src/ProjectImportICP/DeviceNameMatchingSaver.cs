using CsvHelper.Configuration;
using CsvHelper;
using EplanDevice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StaticHelper;
using System.Diagnostics.CodeAnalysis;

namespace EasyEPlanner.ProjectImportICP
{
    public static class DeviceNameMatchingSaver
    {
        /// <summary>
        /// Название 1й колонки в CSV
        /// </summary>
        static public readonly string IcpNameColumn = "Название в ICP CON";

        /// <summary>
        /// Название 2й колонки в CSV
        /// </summary>
        static public readonly string NewNameColumn = "Новое название";

        /// <summary>
        /// Название 3й колонки в CSV
        /// </summary>
        static public readonly string DescriptionColumn = "Описание";

        /// <summary>
        /// Сохранить CSV-файл соответствия названий устройств между <br/>
        /// ICP CON проектом и новым проектом
        /// </summary>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="devices">Устройства</param>
        public static void Save(string path, IEnumerable<ImportDevice> devices)
            => Save<CsvImportDeviceMap, ImportDevice>(path, devices);

        /// <summary>
        /// Сохранить CSV-файл соответствия названий устройств между <br/>
        /// ICP CON проектом и новым проектом
        /// </summary>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="devices">Устройства</param>
        public static void Save(string path, IEnumerable<IODevice> devices)
            => Save<CsvIODeviceMap, IODevice>(path, devices);

        /// <summary>
        /// Сохранить CSV-файл соответствия названий устройств между <br/>
        /// ICP CON проектом и новым проектом
        /// </summary>
        /// <typeparam name="TMap">Карта записи объектов в CSV</typeparam>
        /// <typeparam name="TDevice">Тип устройства (<see cref="IODevice"/>/<see cref="ImportDevice"/>) </typeparam>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="devices">Список устройства типа <typeparamref name="TDevice"/></param>
        private static void Save<TMap, TDevice>(string path, IEnumerable<TDevice> devices) 
            where TMap : ClassMap<TDevice>
        {
            using (StreamWriter writer = new StreamWriter(path))
            using (CsvWriter csvWriter =
                new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                {
                    Delimiter = ";",
                    Encoding = Encoding.UTF8,

                }))
            {
                csvWriter.Context.RegisterClassMap<TMap>();
                csvWriter.WriteRecords(devices);
            }
        }
    }

    public class CsvImportDeviceMap : ClassMap<ImportDevice>
    {
        public CsvImportDeviceMap()
        {
            Map(m => m.FullNumber)
                .Index(0)
                .Name(DeviceNameMatchingSaver.IcpNameColumn)
                .Convert(m => $"{m.Value.WagoType}{m.Value.FullNumber}");

            Map(m => m.Number)
                .Index(1)
                .Name(DeviceNameMatchingSaver.NewNameColumn)
                .Convert(m => $"{m.Value.Object}{m.Value.Type}{m.Value.Number}");

            Map(m => m.Description)
                .Index(2)
                .Name(DeviceNameMatchingSaver.DescriptionColumn);
        }
    }

    [ExcludeFromCodeCoverage]
    public class CsvIODeviceMap : ClassMap<IODevice>
    {
        private static readonly ApiHelper ApiHelper = new ApiHelper();

        public CsvIODeviceMap()
        {
            Map(m => m.EplanName)
                .Index(0)
                .Name(DeviceNameMatchingSaver.IcpNameColumn)
                .Convert(m => ApiHelper.GetSupplementaryFieldValue(m.Value.EplanObjectFunction, 10));

            Map(m => m.Name)
                .Index(1)
                .Name(DeviceNameMatchingSaver.NewNameColumn);

            Map(m => m.Description)
                .Index(2)
                .Name(DeviceNameMatchingSaver.DescriptionColumn);
        }
    }

}
