using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ProjectImportICP
{
    /// <summary>
    /// Описание импортированного устройства
    /// </summary>
    public interface IImportDevice
    {
        /// <summary>
        /// Тип устройства
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Тип устройства в WAGO
        /// </summary>
        string WagoType { get; set; }
    
        /// <summary>
        /// Подтип устройства
        /// </summary>
        string Subtype { get; set; }

        /// <summary>
        /// Полный номер устройства ICP-CON проекта.
        /// Пример 3302 ~ TANK33DEV2
        /// </summary>
        int FullNumber { get; set; }

        /// <summary>
        /// Название и номер объекта
        /// </summary>
        string Object { get; set; }

        /// <summary>
        /// Номер устройства
        /// </summary>
        int Number { get; set; }

        /// <summary>
        /// Описание устройства
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Описание каналов ввода-вывода устройства
        /// </summary>
        List<(string type, int node, int offset, string comment)> Channels { get; set; }

        /// <summary>
        /// [[ LuaMemeber ]] - вызывается из Lua
        /// 
        /// Добавить описание канала ввода-вывода и его привязки
        /// </summary>
        /// <param name="type">Тип: AO|AI|DO|DI</param>
        /// <param name="node">Узел</param>
        /// <param name="offset">Смещение адреса клеммы</param>
        /// <param name="comment">Примечание: номер канала, если несколько одного типа</param>
        void AddChannel(string type, int node, int offset, string comment);
    }

    [ExcludeFromCodeCoverage]
    public class ImportDevice : IImportDevice
    {
        public string Type { get; set; }

        public string WagoType {  get; set; }

        public string Subtype { get; set; }

        public int FullNumber { get; set; }

        public string Object { get; set; }

        public int Number { get; set; }

        public string Description { get; set; }

        public List<(string type, int node, int offset, string comment)> Channels { get; set; } = new List<(string, int, int, string)>();

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public void AddChannel(string type, int node, int offset, string comment)
        {
            Channels.Add((type, node, offset, comment));
        }
    }
}
