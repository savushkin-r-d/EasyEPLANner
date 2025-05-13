using EasyEPlanner.ModbusExchange.Model;
using Eplan.EplApi.DataModel.E3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange
{

    public static class ModbusExchangeSaver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        public static void Save(this IExchange exchange)
        {
            var path = GetPathToSave();

            foreach (var model in exchange.Models)
            {
                using var writer = new StreamWriter(Path.Combine(path, $"gate_{model.Name}.lua"), false);

                writer.Write(SaveModel(model));
            }

            var modbusExchangePath = Path.Combine(path, $"modbusexchange.lua");
            if (exchange.Models.Any() && !File.Exists(modbusExchangePath))
            {
                using var reader = new StreamReader(GetModbusExchangePatternPath());
                using var writer = new StreamWriter(modbusExchangePath, false);

                writer.Write(reader.ReadToEnd());
            }
        }


        private static string GetModbusExchangePatternPath()
        {
            return Path.Combine(ProjectManager.GetInstance().SystemFilesPath,
                "modbus_exchange_pattern.lua");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetPathToSave()
        {
            var projectName = EProjectManager.GetInstance().GetCurrentProjectName();
            return ProjectManager.GetInstance()
                .GetPtusaProjectsPath(projectName) + projectName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static string SaveModel(IGateway gateway)
        {
            var rslt = new StringBuilder()
                .Append($"local gate =\n")
                .Append($"{{\n")
                .Append($"    ip = '{gateway.IP}',\n")
                .Append($"    port = {gateway.Port},\n")
                .Append($"    read =\n")
                .Append(SaveMainGroup(gateway.Read, "    "))
                .Append($"    write =\n")
                .Append(SaveMainGroup(gateway.Write, "    "))
                .Append($"}}\n")
                .Append($"\n")
                .Append($"local s, mt = pcall( require, 'modbusexchange' )\n")
                .Append($"mt = s and mt or {{ }}\n")
                .Append($"return setmetatable( gate, mt )\n");

            return rslt.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static StringBuilder SaveMainGroup(IGroup group, string prefix)
        {
            var rslt = new StringBuilder()
                .Append($"{prefix}{{\n")
                .Append($"{prefix}    offset = 0,\n")
                .Append(SaveGroup(group, prefix + "    "))
                .Append(string.Join("", group.Items.OfType<IGroup>()
                    .Select(g => SaveGroup(g, prefix + "    "))))
                .Append($"{prefix}}},\n");

            return rslt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string SaveGroup(IGroup group, string prefix)
        {
            var name = group.Description is Gateway.READ or Gateway.WRITE ? 
                string.Empty : group.Description;

            var signals = group.Items.OfType<ISignal>();

            if (!signals.Any())
                return string.Empty;

            var offset = signals.First().Word;

            var rslt = new StringBuilder()
                .Append($"{prefix}{{\n")
                .Append($"{prefix}    name = '{name}',\n")
                .Append($"{prefix}    offset = {offset},\n")
                .Append(string.Join("", signals
                    .Select(s => SaveSignal(s, offset, prefix + "    "))))
                .Append($"{prefix}}},\n");

            return rslt.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="offset"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string SaveSignal(ISignal signal, int offset, string prefix)
        {
            return $"{prefix}{{ '{signal.Description}'," +
                $" '{signal.Device?.Name ?? string.Empty}'," +
                $" '{signal.DataType}'," +
                $" {signal.Word - offset}," +
                $" {signal.Bit}}},\n";
        }
    }
}
