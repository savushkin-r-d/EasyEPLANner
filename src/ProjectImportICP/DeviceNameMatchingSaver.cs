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
using System.Runtime.InteropServices;

namespace EasyEPlanner.ProjectImportICP
{
    public static class DeviceNameMatchingSaver
    {
        /// <summary>
        /// Сохранить CSV-файл соответствия названий устройств между <br/>
        /// ICP CON проектом и новым проектом
        /// </summary>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="devices">Устройства</param>
        public static void Save(string path, IEnumerable<ImportDevice> devices)
        {
            Save(path, devices.Select(d => new CsvDeviceMatchingData(
                d.WagoType + d.FullNumber,
                d.Object + d.Type + d.Number,
                d.Description)));
        }

        /// <summary>
        /// Сохранить CSV-файл соответствия названий устройств между <br/>
        /// ICP CON проектом и новым проектом
        /// </summary>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="devices">Устройства</param>
        public static void Save(string path, IEnumerable<IODevice> devices)
        {
            var apiHelper = new ApiHelper();
            Save(path, devices.Select(d => new CsvDeviceMatchingData(
                d.Function.OldDeviceName,
                d.Name,
                d.Description)));
        }


        /// <summary>
        /// Сохранить CSV-файл 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="devices"></param>
        private static void Save(string path, IEnumerable<CsvDeviceMatchingData> devices)
        {
            var stringBuilder = new StringBuilder()
                .Append("Название в ICP CON;Новое название;Описание\n");

            foreach (var device in devices)
            {
                stringBuilder
                    .Append(device.WagoName).Append(";")
                    .Append(device.Name).Append(";");

                if (device.Description.Contains(";"))
                    stringBuilder.Append($"\"{device.Description}\"");
                else stringBuilder.Append($"{device.Description}");
                stringBuilder.Append("\n");
            }

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(stringBuilder.ToString());
            }
        }


        /// <summary>
        /// Модель для CSV-файла
        /// </summary>
        private sealed class CsvDeviceMatchingData
        {
            public CsvDeviceMatchingData(string wagoName, string name, string description) 
            { 
                WagoName = wagoName;
                Name = name;
                Description = description;
            }

            public string WagoName { get; }

            public string Name { get; }

            public string Description { get; }
        }
    }
}
