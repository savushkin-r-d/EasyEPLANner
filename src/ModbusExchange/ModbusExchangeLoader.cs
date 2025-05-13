using EasyEPlanner.ModbusExchange.Model;
using EplanDevice;
using LuaInterface;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.ModbusExchange
{
    public static class ModbusExchangeLoader
    {
        public static void Load(this IExchange exchange)
        {
            var lua = PrepareLuaState();

            var path = GetPathToLoad();
            var files = Directory.GetFiles(path, "gate_*.lua",
                SearchOption.TopDirectoryOnly);

            foreach (var filePath in files)
            {
                var modelName = Regex.Replace(
                    Path.GetFileNameWithoutExtension(filePath),
                    "^gate_", string.Empty);

                exchange.AddModel(modelName);
                var model = exchange.SelectedModel; 

                using var reader = new StreamReader(filePath,
                    EncodingDetector.DetectFileEncoding(filePath), true);

                lua["gate"] = lua.DoString(reader.ReadToEnd())[0];
                lua.GetFunction("init")?.Call(exchange.SelectedModel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetPathToLoad()
        {
            var projectName = EProjectManager.GetInstance().GetCurrentProjectName();
            return ProjectManager.GetInstance()
                .GetPtusaProjectsPath(projectName) + projectName;
        }


        private static Lua PrepareLuaState()
        {
            var lua = new Lua();

            lua.RegisterFunction("AddSignal", null,
                typeof(ModbusExchangeLoader).GetMethod(nameof(AddSignal),
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static));

            lua.RegisterFunction("GetGroup", null,
                typeof(ModbusExchangeLoader).GetMethod(nameof(GetGroup),
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static));

            string sysLuaPath = Path.Combine(
                ProjectManager.GetInstance().SystemFilesPath,
                "sys_modbus_exchange.lua");

            using var luaReader = new StreamReader(sysLuaPath,
                EncodingDetector.DetectFileEncoding(sysLuaPath), true);

            var chunk = luaReader.ReadToEnd();
            lua.DoString(chunk);

            return lua;
        }

        private static void AddSignal(IGroup group, string description,
            string devName, string dataType, int word, int bit) 
        {
            var signal = new Signal(description, dataType, word, bit);
            var dev = DeviceManager.GetInstance().GetDevice(devName);

            if (dev.Description != CommonConst.Cap)
            {
                signal.Device = dev;
            }

            group.Add(signal);
        }

        private static IGroup GetGroup(IGroup group, string description)
        {
            if (description == string.Empty)
                return group;

            var subgroup = group.Items.OfType<IGroup>()
                .FirstOrDefault(g => g.Description == description);

            if (subgroup is null)
            {
                subgroup = new Model.Group(description);
                group.Add(subgroup);
            }

            return subgroup;
        }
    }
}
