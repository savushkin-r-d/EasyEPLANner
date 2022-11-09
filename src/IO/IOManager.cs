using EasyEPlanner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IO
{
    public class IOManager : IIOManager
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        private IOManager()
        {
            iONodes = new List<IIONode>();
            InitIoModulesInfo();
            InitIoNodesInfo();
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
        public IIOModule GetModuleByOffset(int n, int offset,
            IOModuleInfo.ADDRESS_SPACE_TYPE addressSpaceType)
        {
            IIOModule res = null;

            if (iONodes.Count >= n && n > 0)
            {
                int idx = 0;

                foreach (var module in IONodes[n - 1].IOModules)
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
        public IIOModule GetModuleByPhysicalNumber(int number)
        {
            IOModule findedModule = null;
            foreach (var node in iONodes)
            {
                foreach (var module in node.IOModules)
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
        public void AddNode(int n, int nodeNumber, string type, string IP,
            string name, string location)
        {
            int voidNodeNumber = nodeNumber - 100;
            if (iONodes.Count < n)
            {
                for (int i = iONodes.Count; i < n; i++)
                {
                    if (voidNodeNumber == 0)
                        voidNodeNumber = 1;
                    iONodes.Add(new IONode("750-xxx", i + 1,
                        voidNodeNumber,
                        "", "", ""));
                    voidNodeNumber -= 100;
                }
            }

            iONodes[n - 1] = new IONode(type, n, nodeNumber, IP, name, location);
        }

        /// <summary>
        /// Получение узла.
        /// </summary>
        /// <param name="iONode">Индекс узла.</param>
        /// <returns>Узел с заданным индексом.</returns>
        public IIONode this[int idx]
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
            foreach (var node in iONodes)
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

            foreach (var node in iONodes)
            {
                if (node != null && node.Type == IONode.TYPES.T_EMPTY)
                {
                    str += "Отсутствует узел \"A" + node.NodeNumber + "\".\n";

                    str += CheckNodeIPEquality(node);

                    for (int i = 0; i < node.IOModules.Count; i++)
                    {
                        str += node.IOModules[i].Check(i, node.Name);
                    }
                }
            }

            long startingIP = ProjectConfiguration
                .GetInstance().StartingIPInterval;
            long endingIP = ProjectConfiguration.GetInstance()
                .EndingIPInterval;
            if (startingIP != 0 && endingIP != 0)
            {
                str += CheckIONodesIPRange(startingIP, endingIP);
            }

            return str;
        }

        /// <summary>
        /// Проверка совпадения IP-адресов в узлах ввода-вывода.
        /// </summary>
        /// <param name="node">Узел ввода вывода</param>
        /// <returns></returns>
        private string CheckNodeIPEquality(IIONode node)
        {
            string str = string.Empty;
            foreach (var node2 in iONodes)
            {
                if (node == node2) continue;

                if (node.IP == node2.IP && node.IP != "")
                {
                    str += $"\"{node.Name}\": IP адрес совпадает с " +
                        $"\"{node2.Name}\" - {node.IP}.\n";
                }
            }

            return str;
        }

        /// <summary>
        /// Проверка попадания в диапазон IP-адресов узлов ввода-вывода
        /// </summary>
        /// <param name="endingIP">Конец интервала адресов</param>
        /// <param name="startingIP">Начало интервала адресов</param>
        /// <returns>Ошибки</returns>
        private string CheckIONodesIPRange(long startingIP, long endingIP)
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
                    errors += $"IP-адрес узла A{node.NodeNumber} " +
                        $"вышел за диапазон.\n";
                }
            }

            return errors;
        }

        public void CalculateIOLinkAdresses()
        {
            foreach (var node in GetInstance().IONodes)
            {
                foreach (var module in node.IOModules)
                {
                    if(module.Info == null)
                    {
                        continue;
                    }

                    module.CalculateIOLinkAdresses();
                }
            }
        }

        /// <summary>
        /// Инициализировать модули информацию о модулях ввода-вывода.
        /// </summary>
        private void InitIoModulesInfo()
        {
            var lua = new LuaInterface.Lua();
            const string fileName = "sys_io_modules.lua";
            const string templateName = "sysIoModulesLuaFilePattern";
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

        private void InitIoNodesInfo()
        {
            var lua = new LuaInterface.Lua();
            const string fileName = "sys_io_nodes.lua";
            const string templateName = "sysIoNodesLuaFilePattern";
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

                    string name = (string)tableData["name"];
                    int enumNumber = Convert.ToInt32((double)tableData["type"]);
                    IONode.TYPES type = (IONode.TYPES)enumNumber;
                    bool isCoupler = (bool)tableData["isCoupler"];

                    IONodeInfo.AddNodeInfo(name, type, isCoupler);
                }
            }
            else
            {
                string template = EasyEPlanner.Properties.Resources
                    .ResourceManager.GetString(templateName);
                File.WriteAllText(pathToFile, template);
                MessageBox.Show("Файл с описанием узлов ввода-вывода" +
                    " не найден. Будет создан пустой файл (без описания).",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public List<IIONode> IONodes
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
        private List<IIONode> iONodes;     ///Узлы проекта.
        private static IOManager instance;  ///Экземпляр класса.
        #endregion
    }
}
