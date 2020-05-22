using EasyEPlanner;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace IO
{
    /// <summary>
    /// Все узлы модулей ввода-вывода IO. Содержит минимальную функциональность, 
    /// необходимую для экспорта для PAC.
    /// </summary>
    public class IOManager
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        private IOManager()
        {
            iONodes = new List<IONode>();
            InitIOModulesInfo();
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static IOManager GetInstance()
        {
            if (null == instance)
            {
                instance = new IOManager();
            }

            return instance;
        }

        /// <summary>
        /// Получение модуля по номеру узла и смещение (поддержка считывания
        /// информации об устройствах из старого описания *.ds4). 
        /// </summary>        
        /// <param name="n">Номер (c единицы).</param>
        /// <param name="offset">Смещение.</param>
        /// <param name="addressSpaceType">Тип адресного пространства.</param>
        public IOModule GetModuleByOffset(int n, int offset,
            IOModuleInfo.ADDRESS_SPACE_TYPE addressSpaceType)
        {
            IOModule res = null;

            if (iONodes.Count >= n && n > 0)
            {
                int idx = 0;

                foreach (IOModule module in IONodes[n - 1].IOModules)
                {
                    if (module.Info.AddressSpaceType == addressSpaceType)
                    {
                        int moduleOffset = 0;
                        switch (addressSpaceType)
                        {
                            case IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                            case IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                                moduleOffset = module.InOffset;
                                break;

                            case IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                            case IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                                moduleOffset = module.OutOffset;
                                break;
                        }

                        if (moduleOffset > offset)
                        {
                            break;
                        }
                        else
                        {
                            res = module;
                        }
                    }

                    idx++;
                }
            }

            return res;
        }

        /// <summary>
        /// Получить модуль ввода-вывода по его физическому номеру (прим., 202)
        /// </summary>
        /// <param name="number">Физический номер</param>
        /// <returns>Модуль ввода-вывода</returns>
        public IOModule GetModuleByPhysicalNumber(int number)
        {
            IOModule findedModule = null;
            foreach (IONode node in iONodes)
            {
                foreach (IOModule module in node.IOModules)
                {
                    if (module.PhysicalNumber == number)
                    {
                        return module;
                    }
                }
            }

            if (findedModule == null)
            {
                const string Message = "Модуль не найден";
                throw new Exception(Message);
            }
            return findedModule;
        }

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>        
        /// <param name="n">Номер (c единицы).</param>
        /// <param name="type">Тип (например 750-352).</param>
        /// <param name="IP">IP-адрес.</param>
        public void AddNode(int n, string type, string IP, string name)
        {
            if (iONodes.Count < n)
            {
                for (int i = iONodes.Count; i < n; i++)
                {
                    iONodes.Add(new IONode("750-xxx", i + 1, "", ""));
                }
            }

            iONodes[n - 1] = new IONode(type, n, IP, name);
        }

        /// <summary>
        /// Получение узла.
        /// </summary>
        /// <param name="iONode">Индекс узла.</param>
        /// <returns>Узел с заданным индексом.</returns>
        public IONode this[int idx]
        {
            get
            {
                if (idx >= iONodes.Count || idx < 0)
                {
                    return null;
                }
                else
                {
                    return iONodes[idx];
                }
            }
        }

        /// <summary>
        /// Сброс информации о модулях IO.
        /// </summary>
        public void Clear()
        {
            iONodes.Clear();
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        public string SaveAsLuaTable(string prefix)
        {
            string str = "--Узлы IO\n" +
                "nodes =\n" + "\t{\n";
            foreach (IONode node in iONodes)
            {
                if (node == null)
                {
                    continue;
                }

                str += node.SaveAsLuaTable("\t\t") + ",\n";
            }
            str += "\t}\n";

            str = str.Replace("\t", "    ");
            return str;
        }

        /// <summary>
        /// Проверка корректного заполнения узлами.
        /// </summary>
        public string Check()
        {
            var str = "";

            int idx = 100;
            foreach (IONode node in iONodes)
            {
                if (node != null && node.Type == IONode.TYPES.T_EMPTY)
                {
                    str += "Отсутствует узел \"A" + idx + "\".\n";
                }
                idx += 100;

                foreach (IONode node2 in iONodes)
                {
                    if (node == node2) continue;

                    if (node.IP == node2.IP && node.IP != "")
                    {
                        str += "\"A" + 100 * node.N + 
                            "\" : IP адрес совпадает с \"A" +
                            100 * node2.N + "\" - " + node.IP + ".\n";
                    }
                }
            }

            long startingIP = EasyEPlanner.ProjectConfiguration
                .GetInstance().StartingIPInterval;
            long endingIP = EasyEPlanner.ProjectConfiguration.GetInstance()
                .EndingIPInterval;
            if (startingIP != 0 && endingIP != 0)
            {
                str += CheckIONodesIP(startingIP, endingIP);
            }

            return str;
        }

        /// <summary>
        /// Проверка IP-адресов узлов ввода-вывода
        /// </summary>
        /// <param name="endingIP">Конец интервала адресов</param>
        /// <param name="startingIP">Начало интервала адресов</param>
        /// <returns>Ошибки</returns>
        private string CheckIONodesIP(long startingIP, long endingIP)
        {
            string errors = "";
            var plcWithIP = IONodes;
            foreach (var node in plcWithIP)
            {
                string IPstr = node.IP;
                if (IPstr == "")
                {
                    continue;
                }

                long nodeIP = StaticHelper.IPConverter
                    .ConvertIPStrToLong(IPstr);
                if (nodeIP - startingIP < 0 || endingIP - nodeIP < 0)
                {
                    errors += $"IP-адрес узла A{node.FullN} " +
                        $"вышел за диапазон.\n";
                }
            }

            return errors;
        }


        /// <summary>
        /// Расчет IO-Link адресов привязанных устройств для всех модулей
        /// ввода-вывода.
        /// </summary>
        public void CalculateIOLinkAdresses()
        {
            foreach (IONode node in IOManager.GetInstance().IONodes)
            {
                foreach (IOModule module in node.IOModules)
                {
                    module.CalculateIOLinkAdresses();
                }
            }
        }

        /// <summary>
        /// Инициализировать модули информацию о модулях ввода-вывода.
        /// </summary>
        private void InitIOModulesInfo()
        {
            var lua = new LuaInterface.Lua();
            const string fileName = "sys_io.lua";
            const string templateName = "sysIOLuaFilePattern";
            string pathToFile = Path.Combine(
                ProjectManager.GetInstance().SystemFilesPath, fileName);

            if (File.Exists(pathToFile))
            {
                object[] result = lua.DoFile(pathToFile);
                if (result == null)
                {
                    return;
                }

                var dataTables = result[0] as LuaInterface.LuaTable;
                foreach (var table in dataTables.Values)
                {
                    var tableData = table as LuaInterface.LuaTable;

                    int number = Convert.ToInt32((double)tableData["n"]);
                    string name = (string)tableData["name"];
                    string description = (string)tableData["description"];
                    int addressSpaceTypeNumber = Convert.ToInt32(
                        (double)tableData["addressSpaceType"]);
                    string typeName = (string)tableData["typeName"];
                    string groupName = (string)tableData["groupName"];

                    var channelClamps = new List<int>();
                    var channelAddressesIn = new List<int>();
                    var channelAddrOut = new List<int>();

                    var channelClampsTable = tableData[
                        "channelClamps"] as LuaInterface.LuaTable;
                    var channelAddressesInTable = tableData[
                        "channelAddressesIn"] as LuaInterface.LuaTable;
                    var channelAddressesOutTable = tableData[
                        "channelAddressesOut"] as LuaInterface.LuaTable;
                    foreach (var num in channelClampsTable.Values)
                    {
                        channelClamps.Add(Convert.ToInt32((double)num));
                    }
                    foreach (var num in channelAddressesInTable.Values)
                    {
                        channelAddressesIn.Add(Convert.ToInt32((double)num));
                    }
                    foreach (var num in channelAddressesOutTable.Values)
                    {
                        channelAddrOut.Add(Convert.ToInt32((double)num));
                    }

                    int DOcnt = Convert.ToInt32((double)tableData["DO_count"]);
                    int DIcnt = Convert.ToInt32((double)tableData["DI_count"]);
                    int AOcnt = Convert.ToInt32((double)tableData["AO_count"]);
                    int AIcnt = Convert.ToInt32((double)tableData["AI_count"]);
                    string color = (string)tableData["Color"];

                    IOModuleInfo.AddModuleInfo(number, name, description,
                        addressSpaceTypeNumber, typeName, groupName,
                        channelClamps.ToArray(), channelAddressesIn.ToArray(),
                        channelAddrOut.ToArray(), DOcnt, DIcnt, AOcnt, AIcnt,
                        color);
                }
            }
            else
            {
                string template = EasyEPlanner.Properties.Resources
                    .ResourceManager.GetString(templateName);
                File.WriteAllText(pathToFile, template);
                MessageBox.Show("Файл с описанием модулей ввода-вывода" +
                    " не найден. Будет создан пустой файл (без описания).", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<IONode> IONodes
        {
            get
            {
                return iONodes;
            }
        }

        /// <summary>
        /// Номера IO-Link модулей, которые используются
        /// </summary>
        public enum IOLinkModules
        {
            Wago = 657,
            PhoenixContactSmart = 1088132,
            PhoenixContactStandard = 1027843,
        }

        /// <summary>
        /// Шаблон для разбора имени узла, модуля ввода-вывода (прим., А100).
        /// </summary>
        public const string IONamePattern = @"=*-A(?<n>\d+)";

        #region Закрытые поля.
        private List<IONode> iONodes;     ///Узлы проекта.
        private static IOManager instance;  ///Экземпляр класса.
        #endregion
    }
}
