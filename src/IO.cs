///@brief Классы, реализующие минимальную функциональность, необходимую для 
///экспорта описания модулей IO для PAC.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;

/// <summary>
/// Пространство имен классов IO.
/// </summary>
namespace IO
{
    /// <summary>
    /// Описание модуля ввода-вывода IO.
    /// </summary>
    public class IOModuleInfo : ICloneable
    {
        /// <summary>
        /// Добавить информацию о модуле ввода вывода
        /// </summary>
        /// <param name="number">Номер модуля ввода-вывода IO </param>
        /// <param name="name">Имя модуля ввода-вывода IO</param>
        /// <param name="description">Описание модуля ввода-вывода IO</param>
        /// <param name="addressSpaceTypeNum">Тип адресного пространства</param>
        /// <param name="typeName">Имя типа (дискретный выход и др.)</param>
        /// <param name="groupName">Имя серии (прим., 750-800)</param>
        /// <param name="channelClamps">Клеммы каналов ввода-вывода</param>
        /// <param name="channelAddressesIn">Адреса каналов ввода</param>
        /// <param name="channelAddressesOut">Адреса каналов вывода</param>
        /// <param name="DO_count">Количество дискретных выходов</param>
        /// <param name="DI_count">Количество дискретных входов</param>
        /// <param name="AO_count">Количество аналоговых выходов</param>
        /// <param name="AI_count">Количество аналоговых входов</param>
        /// <param name="colorAsStr">Физический цвет модуля</param>
        public static void AddModuleInfo(int number, string name, 
            string description, int addressSpaceTypeNum, string typeName, 
            string groupName, int[] channelClamps, int[] channelAddressesIn,
            int[] channelAddressesOut, int DO_count, int DI_count, 
            int AO_count, int AI_count, string colorAsStr)
        {
            var addressSpaceType = (ADDRESS_SPACE_TYPE)addressSpaceTypeNum;
            Color color = Color.FromName(colorAsStr);

           var moduleInfo = new IOModuleInfo(number, name, description,
                addressSpaceType, typeName, groupName, channelClamps,
                channelAddressesIn, channelAddressesOut, DO_count, DI_count,
                AO_count, AI_count, color);

            if (modules.Where(x => x.Name == moduleInfo.Name).Count() == 0)
            {
                modules.Add(moduleInfo);
            }
        }

        /// <summary>
        /// Получение описания модуля ввода-вывода IO на основе его имени.
        /// </summary>
        /// <param name="name">Имя модуля (750-860).</param>
        /// <param name="isStub">Признак не идентифицированного модуля.</param>
        public static IOModuleInfo GetModuleInfo(string name, out bool isStub)
        {
            isStub = false;

            IOModuleInfo res = modules.Find( x => x.Name == name);
            if (res != null)
            {
                return res.Clone() as IOModuleInfo;
            }
                
            isStub = true;
            return stub;
        }

        /// <summary>
        /// Закрытый конструктор. Используется для создания списка применяемых
        /// модулей.
        /// </summary>
        private IOModuleInfo(int n, string name, string descr,
            ADDRESS_SPACE_TYPE addressSpaceType, string typeName,
            string groupName, int[] channelClamps, int[] clampsAddressIn, 
            int[] clampsAddressOut, int DO_count, int DI_count, int AO_count, 
            int AI_count, Color color)
        {
            this.n = n;
            this.name = name;
            this.description = descr;

            this.addressSpaceType = addressSpaceType;
            this.typeName = typeName;
            this.groupName = groupName;

            this.channelClamps = channelClamps;
            this.channelAddressesIn = clampsAddressIn;
            this.channelAddressesOut = clampsAddressOut;

            this.DO_cnt = DO_count;
            this.DI_cnt = DI_count;
            this.AO_cnt = AO_count;
            this.AI_cnt = AI_count;

            this.moduleColor = color;
        }

        public object Clone()
        {
            var channelClamps = this.ChannelClamps.Clone() as int[];
            var channelAddressesIn = this.ChannelAddressesIn.Clone() as int[];
            var channelAddressesOut = this.ChannelAddressesOut.Clone() as int[];

            return new IOModuleInfo(this.Number, this.Name, this.Description,
                this.AddressSpaceType, this.TypeName, this.GroupName,
                channelClamps, channelAddressesIn, channelAddressesOut, 
                this.DO_count, this.DI_count, this.AO_count, this.AI_count, 
                this.ModuleColor);
        }

        /// <summary>
        /// Имя модуля ввода-вывода IO (серия-номер, например: 750-860).
        /// </summary>        
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Номер модуля ввода-вывода IO (например: 860).
        /// </summary>  
        public int Number
        {
            get
            {
                return n;
            }
        }

        /// <summary>
        /// Описание модуля ввода-вывода IO.
        /// </summary>  
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Тип адресного пространства модуля ввода-вывода IO.
        /// </summary>
        public ADDRESS_SPACE_TYPE AddressSpaceType
        {
            get
            {
                return addressSpaceType;
            }
        }

        /// <summary>
        /// Клеммы каналов ввода-вывода.
        /// </summary>
        public int[] ChannelClamps
        {
            get
            {
                return channelClamps;
            }
        }

        /// <summary>
        /// Адреса каналов ввода.
        /// </summary>
        public int[] ChannelAddressesIn
        {
            get
            {
                return channelAddressesIn;
            }
        }

        /// <summary>
        /// Адреса каналов вывода.
        /// </summary>
        public int[] ChannelAddressesOut
        {
            get
            {
                return channelAddressesOut;
            }
        }

        /// <summary>
        /// Количество дискретных выходов. 
        /// </summary>
        public int DO_count
        {
            get
            {
                return DO_cnt;
            }
        }

        /// <summary>
        /// Количество дискретных входов. 
        /// </summary>
        public int DI_count
        {
            get
            {
                return DI_cnt;
            }
        }

        /// <summary>
        /// Количество аналоговых выходов. 
        /// </summary>
        public int AO_count
        {
            get
            {
                return AO_cnt;
            }
        }

        /// <summary>
        /// Количество аналоговых входов. 
        /// </summary>
        public int AI_count
        {
            get
            {
                return AI_cnt;
            }
        }

        /// <summary>
        /// Имя типа (дискретный выход, аналоговый выход, ...).
        /// </summary>
        public string TypeName
        {
            get
            {
                return typeName;
            }
        }

        /// <summary>
        /// Физический цвет модуля
        /// </summary>
        public Color ModuleColor
        {
            get
            {
                return moduleColor;
            }
        }

        /// <summary>
        /// Имя серии модуля ввода-вывода IO (например 750-800).
        /// </summary>        
        public string GroupName
        {
            get
            {
                return groupName;
            }
        }

        /// <summary>
        /// Тип адресного пространства модуля
        /// </summary>
        public enum ADDRESS_SPACE_TYPE
        {
            NONE,
            DO,
            DI,
            AO,
            AI,
            AOAI,
            DODI,
            AOAIDODI,
        };

        #region Закрытые поля.
        /// <summary>
        /// Номер.
        /// </summary>
        private int n;

        /// <summary>
        /// Имя.
        /// </summary>
        private string name;

        /// <summary>
        /// Имя типа (дискретный выход, аналоговый выход, ...).
        /// </summary>
        private string typeName;

        /// <summary>
        /// Серия модуля (750-800, 750-1500, ...).
        /// </summary>
        private string groupName;

        /// <summary>
        /// Описание.
        /// </summary>
        private string description;

        /// <summary>
        /// Тип адресного пространства ( DO, DI, AO, AI ).
        /// </summary>
        private ADDRESS_SPACE_TYPE addressSpaceType;

        /// <summary>
        /// Клеммы каналов ввода/вывода.
        /// </summary>
        private int[] channelClamps;

        /// <summary>
        /// Адреса каналов вывода.
        /// </summary>
        private int[] channelAddressesOut;

        /// <summary>
        /// Адреса каналов ввода.
        /// </summary>
        private int[] channelAddressesIn;

        /// <summary>
        /// Количество дискретных выходов. 
        /// </summary>
        private int DO_cnt;

        /// <summary>
        /// Количество дискретных входов.
        /// </summary>
        private int DI_cnt;

        /// <summary>
        /// Количество аналоговых выходов. 
        /// </summary>
        private int AO_cnt;

        /// <summary>
        /// Количество аналоговых входов. 
        /// </summary>
        private int AI_cnt;

        /// <summary>
        /// Цвет.
        /// </summary>
        private Color moduleColor;
        #endregion

        /// <summary>
        /// Список модулей ввода-вывода.
        /// </summary>
        private static List<IOModuleInfo> modules = new List<IOModuleInfo>();

        /// <summary>
        /// Заглушка, для возврата в случае поиска неописанных модулей. 
        /// </summary>
        private static IOModuleInfo stub = new IOModuleInfo(0,
            "не определен", "", ADDRESS_SPACE_TYPE.NONE, "", "", new int[0], 
            new int[0], new int[0], 0, 0, 0, 0, Color.LightGray);
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Модуль ввода-вывода IO.
    /// </summary>
    public class IOModule
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="inAddressSpaceOffset">Смещение входного адресного 
        /// пространства модуля.</param>
        /// <param name="outAddressSpaceOffset">Смещение выходного адресного 
        /// пространства модуля.</param>
        /// <param name="info">Описание модуля.</param>
        /// <param name="physicalNumber">Физический номер (из ОУ) устройства.
        /// </param>
        /// <param name="function">Eplan функция модуля.</param>
        public IOModule(int inAddressSpaceOffset, int outAddressSpaceOffset,
            IOModuleInfo info, int physicalNumber, 
            Eplan.EplApi.DataModel.Function function)
        {
            this.inAddressSpaceOffset = inAddressSpaceOffset;
            this.outAddressSpaceOffset = outAddressSpaceOffset;
            this.info = info;
            this.physicalNumber = physicalNumber;
            this.function = function;

            devicesChannels = new List<Device.IODevice.IOChannel>[80];
            devices = new List<Device.IODevice>[80];
        }

        public IOModule(int inAddressSpaceOffset, int outAddressSpaceOffset,
            IOModuleInfo info) : this(inAddressSpaceOffset, 
                outAddressSpaceOffset, info, 0, null)
        {
            // Делегировано в конструктор с 5 параметрами.
        }

        public void AssignChannelToDevice(int chN, Device.IODevice dev,
            Device.IODevice.IOChannel ch)
        {
            if (devices.GetLength(0) <= chN)
            {
                System.Windows.Forms.MessageBox.Show("Error!");
            }

            if (devices[chN] == null)
            {
                devices[chN] = new List<Device.IODevice>();
            }
            if (devicesChannels[chN] == null)
            {
                devicesChannels[chN] = new List<Device.IODevice.IOChannel>();
            }

            devices[chN].Add(dev);
            devicesChannels[chN].Add(ch);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";

            if (info != null)
            {
                res = String.Format("{0} {{ {1,7} }},        --{2,7}", prefix, info.Number, info.Name);
            }
            else
            {
                res = prefix + "{ ? },";
            }

            return res;
        }

        public void SaveAsConnectionArray(ref object[,] res, ref int idx, int p, Dictionary<string, int> modulesCount, Dictionary<string, Color> modulesColor)
        {
            string moduleName = Info.Number.ToString();
            if (modulesCount.ContainsKey(moduleName))
            {
                modulesCount[moduleName]++;
            }
            else
            {
                modulesCount.Add(moduleName, 1);
            }

            if (!modulesColor.ContainsKey(moduleName))
            {
                modulesColor.Add(moduleName, Info.ModuleColor);
            }

            if (Info.ChannelClamps.GetLength(0) != 0)
            {
                if (Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI ||
                    Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    if (this.isIOLink() == true)
                    {
                        foreach (int clamp in Info.ChannelClamps)
                        {
                            res[idx, 0] = p;
                            res[idx, 1] = moduleName;
                            res[idx, 2] = clamp.ToString();
                            res[idx, 3] = "IO-Link";
                            idx++;
                        }
                    }
                    else
                    {
                        foreach (int clamp in Info.ChannelClamps)
                        {
                            res[idx, 0] = p;
                            res[idx, 1] = moduleName;
                            res[idx, 2] = clamp.ToString();
                            res[idx, 3] = "AS interface";
                            idx++;
                        }
                    }
                }
                else
                {
                    foreach (int clamp in Info.ChannelClamps)
                    {
                        res[idx, 0] = p;
                        res[idx, 1] = moduleName;
                        res[idx, 2] = clamp;
                        if (devices[clamp] != null)
                        {
                            string devName = "";
                            int devIdx = 0;
                            foreach (Device.IODevice dev in devices[clamp])
                            {

                                devName += dev.EPlanName + dev.GetConnectionType() + dev.GetRange() + ": " +
                                        devicesChannels[clamp][devIdx].Name + ": " + dev.Description + " " +
                                        devicesChannels[clamp][devIdx].Comment;
                                devName = devName.Replace('\n', ' ');
                                devIdx++;
                            }
                            res[idx, 3] = devName;

                        }
                        idx++;
                    }
                }
            }
            else
            {
                res[idx, 0] = p;
                res[idx, 1] = moduleName;
                idx++;
            }
        }

        public void SaveASInterfaceConnection(int nodeIdx, int moduleIdx, Dictionary<string, object[,]> asInterfaceConnection)
        {
            string key = "Узел №" + nodeIdx.ToString() + " Модуль №" + moduleIdx.ToString();
            if (!asInterfaceConnection.ContainsKey(key))
            {
                if (Info.ChannelClamps.GetLength(0) != 0)
                {
                    object[,] asConnection = new object[Info.ChannelClamps.GetLength(0) * 128, 2];
                    int devIdx = 0;
                    foreach (int clamp in Info.ChannelClamps)
                    {
                        if (devices[clamp] != null)
                        {
                            int deviceCounter = 0;
                            foreach (Device.IODevice dev in devices[clamp])
                            {

                                asConnection[devIdx, 0] = clamp.ToString() + "(" + (deviceCounter + 1).ToString() + ")";
                                string devDescription = dev.EPlanName + dev.GetConnectionType() + dev.GetRange() + ": " +
                                    devicesChannels[clamp][deviceCounter].Name + ": " + dev.Description + " " +
                                    devicesChannels[clamp][deviceCounter].Comment;
                                devDescription = devDescription.Replace('\n', ' ');

                                asConnection[devIdx, 1] = devDescription;

                                devIdx++;
                                deviceCounter++;
                            }
                        }
                    }
                    asInterfaceConnection.Add(key, asConnection);
                }
            }
        }

        /// <summary>
        /// Расчет IO-Link адресов привязанных устройств.
        /// </summary>       
        public void CalculateIOLinkAdresses()
        {
            IOLinkCalculator calculator = new IOLinkCalculator(devices, 
                devicesChannels, Info);
            calculator.Calculate();
        }

        /// <summary>
        /// Описание модуля.
        /// </summary>
        public IOModuleInfo Info
        {
            get
            {
                return info;
            }
        }

        /// <summary>
        /// Смещение входного адресного пространства модуля.
        /// </summary>
        public int InOffset
        {
            get
            {
                return inAddressSpaceOffset;
            }
        }

        /// <summary>
        /// Смещение выходного адресного пространства модуля.
        /// </summary>
        public int OutOffset
        {
            get
            {
                return outAddressSpaceOffset;
            }
        }

        /// <summary>
        /// Номер устройства (из ОУ) прим., 202.
        /// </summary>
        public int PhysicalNumber
        {
            get
            {
                return physicalNumber;
            }
        }

        /// <summary>
        /// Eplan функция модуля.
        /// </summary>
        public Eplan.EplApi.DataModel.Function Function 
        { 
            get 
            {
                return function;
            } 
        }

        /// <summary>
        /// Является ли модуль IO-Link 
        /// </summary>
        /// <returns></returns>
        public bool isIOLink()
        {
            bool isIOLink = false;

            int wago = (int) IOManager.IOLinkModules.Wago;
            int phoenixContactStandard = (int) IOManager.IOLinkModules
                .PhoenixContactStandard;
            int phoenixContactSmart = (int) IOManager.IOLinkModules
                .PhoenixContactSmart;

            if (Info.Number == wago ||
                Info.Number == phoenixContactStandard ||
                Info.Number == phoenixContactSmart)
            {
                isIOLink = true;
            }

            return isIOLink;
        }

        /// Привязанные устройства.
        public List<Device.IODevice>[] devices;
        /// Привязанные каналы.
        public List<Device.IODevice.IOChannel>[] devicesChannels;

        #region Закрытые поля.
        /// <summary>
        /// Смещение входного адресного пространства модуля.
        /// </summary>
        private int inAddressSpaceOffset;

        /// <summary>
        /// Смещение выходного адресного пространства модуля.
        /// </summary>
        private int outAddressSpaceOffset;

        /// <summary>
        /// Описание модуля
        /// </summary>
        private IOModuleInfo info;

        /// <summary>
        /// Физический номер модуля
        /// </summary>
        private int physicalNumber;

        /// <summary>
        /// Eplan функция модуля ввода-вывода.
        /// </summary>
        Eplan.EplApi.DataModel.Function function;
        #endregion
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Узел модулей ввода-вывода IO.
    /// </summary>
    public class IONode
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="typeStr">Тип.</param>
        /// <param name="n">Номер (также используется как адрес для COM-порта).
        /// </param>
        /// <param name="ip">IP-адрес.</param>
        public IONode(string typeStr, int n, string ip, string name)
        {
            this.typeStr = typeStr;
            switch (typeStr)
            {
                case "750-863":
                    type = TYPES.T_INTERNAL_750_86x;
                    break;

                case "750-341":
                case "750-841":
                case "750-352":
                    type = TYPES.T_ETHERNET;
                    break;

                case "750-8202":
                case "750-8203":
                case "750-8204":
                case "750-8206":
                    type = TYPES.T_INTERNAL_750_820x;
                    break;

                case "AXL F BK ETH":
                    type = TYPES.T_PHOENIX_CONTACT;
                    break;
                case "AXC F 2152":
                    type = TYPES.T_PHOENIX_CONTACT_MAIN;
                    break;

                default:
                    type = TYPES.T_EMPTY;
                    break;
            }

            this.ip = ip;
            this.n = n;
            this.name = name;

            iOModules = new List<IOModule>();

            DI_count = 0;
            DO_count = 0;
            AI_count = 0;
            AO_count = 0;
        }

        /// <summary>
        /// Получение списка модулей по имени.
        /// </summary>
        public Dictionary<IOModule, int> GetModulesList(string name)
        {
            Dictionary<IOModule, int> modulesList = new Dictionary<IOModule, int>();
            for (int i = 0; i < IOModules.Count; i++)
            {
                if (IOModules[i].Info.Name == name)
                {
                    modulesList.Add(IOModules[i], i);
                }
            }
            return modulesList;
        }

        /// <summary>
        /// Добавление модуль.
        /// </summary>
        /// <param name="iOModule">Добавляемый модуль.</param>
        private void AddModule(IOModule iOModule)
        {
            iOModules.Add(iOModule);
        }

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>
        /// <param name="iOModule">Вставляемый модуль.</param>
        /// <param name="position">Позиция модуля, начиная с 1.</param>
        public void SetModule(IOModule iOModule, int position)
        {
            if (iOModules.Count < position)
            {
                for (int i = iOModules.Count; i < position; i++)
                {
                    iOModules.Add(new IOModule(0, 0, null));
                }
            }

            iOModules[position - 1] = iOModule;
        }

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>
        /// <param name="iOModule">Вставляемый модуль.</param>
        /// <param name="position">Позиция модуля, начиная с 1.</param>
        public void InsertModule(IOModule iOModule, int position)
        {
            if (iOModules.Count < position)
            {
                for (int i = iOModules.Count; i < position; i++)
                {
                    iOModules.Add(new IOModule(0, 0, null));
                }
            }

            iOModules.Insert(position, iOModule);
        }

        /// <summary>
        /// Получение модуля.
        /// </summary>
        /// <param name="iONode">Индекс модуля.</param>
        /// <returns>Модуль с заданным индексом.</returns>
        public IOModule this[int idx]
        {
            get
            {
                if (idx >= iOModules.Count || idx < 0)
                {
                    return null;
                }
                else
                {
                    return iOModules[idx];
                }
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        public string SaveAsLuaTable(string prefix)
        {
            string str = prefix + "{\n";
            str += prefix + "name    = \'" + name + "\',\n";
            str += prefix + "ntype   = " + (int)type + ", " + "--" + typeStr + "\n";
            str += prefix + "n       = " + n + ",\n";
            str += prefix + "IP      = \'" + ip + "\',\n";
            str += prefix + "modules =\n";
            str += prefix + "\t{\n";

            foreach (IOModule module in iOModules)
            {
                if (module != null)
                {
                    str += module.SaveAsLuaTable(prefix + "\t") + ",\n";
                }
                else
                {
                    str += prefix + "\t" + "{}" + ",\n";
                }
            }

            str += prefix + "\t}\n";
            str += prefix + "}";

            return str;
        }

        public void SaveAsConnectionArray(ref object[,] res, ref int idx, Dictionary<string, int> modulesCount,
            Dictionary<string, Color> modulesColor, int nodeIdx, Dictionary<string, object[,]> asInterfaceConnection)
        {
            for (int i = 0; i < iOModules.Count; i++)
            {
                iOModules[i].SaveAsConnectionArray(ref res, ref idx, i + 1, modulesCount, modulesColor);
                if (iOModules[i].Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI ||
                    iOModules[i].Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    iOModules[i].SaveASInterfaceConnection(nodeIdx, i + 1, asInterfaceConnection);
                }
            }

        }

        /// <summary>
        /// Типы модулей.
        /// </summary>
        public enum TYPES
        {
            T_EMPTY = -1,            /// Не определен.

            T_INTERNAL_750_86x,      /// Модули в управляющем контроллере 750-863.

            T_INTERNAL_750_820x = 2, /// Модули в управляющем контроллере PFC200.

            T_ETHERNET = 100,        /// Удаленный Ethernet узел.             

            T_PHOENIX_CONTACT = 200, /// Модули Phoenix Contact.

            T_PHOENIX_CONTACT_MAIN = 201, /// Модуль Phoenix Contact с управляющей программой.
        };

        /// <summary>
        /// Количество дискретных входов.
        /// </summary>
        public int DI_count { get; set; }

        /// <summary>
        /// Количество дискретных выходов.
        /// </summary>
        public int DO_count { get; set; }

        /// <summary>
        /// Количество аналоговых входов.
        /// </summary>
        public int AI_count { get; set; }

        /// <summary>
        /// Количество аналоговых выходов.
        /// </summary>
        public int AO_count { get; set; }

        /// <summary>
        /// Модули ввода-вывода узла.
        /// </summary>
        public List<IOModule> IOModules 
        { 
            get 
            { 
                return iOModules; 
            } 
        }

        /// <summary>
        /// IP-адрес.
        /// </summary>
        public string IP 
        { 
            get 
            { 
                return ip; 
            } 
        }

        /// <summary>
        /// Тип узла.
        /// </summary>
        public TYPES Type 
        { 
            get 
            { 
                return type; 
            } 
        }

        /// <summary>
        /// Тип узла (строка).
        /// </summary>
        public string TypeStr 
        { 
            get 
            { 
                return typeStr; 
            } 
        }

        /// <summary>
        /// Номер.
        /// </summary>
        public int N 
        { 
            get 
            { 
                return n; 
            } 
        }

        /// <summary>
        /// Полный номер узла.
        /// </summary>
        public int FullN
        {
            get
            {
                if (n == 1)
                {
                    return n;
                }
                else
                {
                    return n * 100;
                }
            }
        }

        #region Закрытые поля.
        /// <summary>
        /// Модули узла.
        /// </summary>
        private List<IOModule> iOModules;

        /// <summary>
        /// Тип узла (строка).
        /// </summary>
        private string typeStr;

        /// <summary>
        /// Тип узла.
        /// </summary>
        private TYPES type;

        /// <summary>
        /// IP-адрес.
        /// </summary>
        private string ip;

        /// <summary>
        /// Номер.
        /// </summary>
        private int n;

        /// <summary>
        /// Имя узла (прим., А100).
        /// </summary>
        private string name;
        #endregion
    }

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
                        str += "\"A" + 100 * node.N + "\" : IP адрес совпадает с \"A" +
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
            string pathToDir = Path.GetDirectoryName(EasyEPlanner.AddInModule
                .OriginalAssemblyPath) + "\\Lua";
            string pathToFile = Path.Combine(pathToDir, fileName);

            if(File.Exists(pathToFile))
            {
                object[] result = lua.DoFile(pathToFile);
                if (result == null)
                {
                    return;
                }

                var dataTables = result[0] as LuaInterface.LuaTable;
                foreach(var table in dataTables.Values)
                {
                    var tableData = table as LuaInterface.LuaTable;

                    int number = Convert.ToInt32((double)tableData["n"]);
                    string name = (string)tableData["name"];
                    string description = (string)tableData["description"];
                    int addressSpaceTypeNumber = Convert.ToInt32(
                        (double)tableData["addressSpaceType"]);
                    string typeName = (string)tableData["typeName"];
                    string groupName = (string)tableData["groupName"];

                    var channelClampsList = new List<int>();
                    var channelAddressesInList = new List<int>();
                    var channelAddressesOutList = new List<int>();

                    var channelClampsTable = tableData[
                        "channelClamps"] as LuaInterface.LuaTable;
                    var channelAddressesInTable = tableData[
                        "channelAddressesIn"] as LuaInterface.LuaTable;
                    var channelAddressesOutTable = tableData[
                        "channelAddressesOut"] as LuaInterface.LuaTable;
                    foreach(var num in channelClampsTable.Values)
                    {
                        channelClampsList.Add(Convert.ToInt32((double)num));
                    }
                    foreach (var num in channelAddressesInTable.Values)
                    {
                        channelAddressesInList.Add(Convert.ToInt32((double)num));
                    }
                    foreach (var num in channelAddressesOutTable.Values)
                    {
                        channelAddressesOutList.Add(Convert.ToInt32((double)num));
                    }

                    var channelClamps = channelClampsList.ToArray().Clone() as int[];
                    var channelAddressesIn = channelAddressesInList.ToArray().Clone() as int[];
                    var channelAddressesOut = channelAddressesOutList.ToArray().Clone() as int[];

                    int DOcnt = Convert.ToInt32((double)tableData["DO_count"]);
                    int DIcnt = Convert.ToInt32((double)tableData["DI_count"]);
                    int AOcnt = Convert.ToInt32((double)tableData["AO_count"]);
                    int AIcnt = Convert.ToInt32((double)tableData["AI_count"]);
                    string color = (string)tableData["Color"];

                    IOModuleInfo.AddModuleInfo(number, name, description,
                        addressSpaceTypeNumber, typeName, groupName,
                        channelClamps, channelAddressesIn, 
                        channelAddressesOut, DOcnt, DIcnt, AOcnt, 
                        AIcnt, color);
                }
            }
            else
            {
                string template = EasyEPlanner.Properties.Resources
                    .ResourceManager.GetString(templateName);
                File.WriteAllText(pathToFile,template);
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

    /// <summary>
    /// Класс, рассчитывающий IO-Link адреса
    /// </summary>
    sealed class IOLinkCalculator
    {
        /// <summary>
        /// Закрытый конструктор для безопасности.
        /// </summary>
        private IOLinkCalculator() { }

        /// <summary>
        /// Конструктор стандартный
        /// </summary>
        /// <param name="devices">Список привязанных устройств</param>
        /// <param name="devicesChannels">Список привязанных каналов</param>
        /// <param name="moduleInfo">Информация о модуле ввода-вывода</param>
        public IOLinkCalculator(List<Device.IODevice>[] devices, 
            List<Device.IODevice.IOChannel>[] devicesChannels,
            IOModuleInfo moduleInfo) 
        {
            this.devices = devices;
            this.devicesChannels = devicesChannels;
            this.moduleInfo = moduleInfo;
        }

        public void Calculate()
        {
            int moduleNumber = moduleInfo.Number;

            switch (moduleNumber)
            {
                case (int) IOManager.IOLinkModules.Wago:
                    CalculateForWago();
                    break;

                case (int) IOManager.IOLinkModules.PhoenixContactStandard:
                    CalculateForPhoenixContact();
                    break;

                case (int) IOManager.IOLinkModules.PhoenixContactSmart:
                    CalculateForPhoenixContact();
                    break;
            }
        }

        /// <summary>
        /// Расчет для IO-Link модуля от Wago.
        /// </summary>
        private void CalculateForWago() 
        {
            foreach (int clamp in moduleInfo.ChannelClamps)
            {
                if (devices[clamp] != null && devices[clamp][0] != null)
                {
                    moduleInfo.ChannelAddressesIn[clamp] = offsetIn;
                    moduleInfo.ChannelAddressesOut[clamp] = offsetOut;
                    offsetIn += devices[clamp][0].IOLinkSizeIn;
                    offsetOut += devices[clamp][0].IOLinkSizeOut;
                }
            }
        }

        /// <summary>
        /// Расчет для стандартного IO-Link модуля от Phoenix Contact.
        /// </summary>
        private void CalculateForPhoenixContact() 
        {
            foreach (int clamp in moduleInfo.ChannelClamps)
            {
                if (devices[clamp] != null && devices[clamp][0] != null)
                {
                    int deviceOffset;
                    Device.IODevice.IOChannel channel = 
                        devicesChannels[clamp][0];
                    Device.IODevice device = devices[clamp][0];
                    if (channel.Name == "DI" || channel.Name == "DO")
                    {
                        int moduleOffset = channel.ModuleOffset;
                        int logicalClamp = channel.LogicalClamp;
                        int discreteOffset = CalculateDiscreteOffsetForIOLink(
                            moduleOffset, logicalClamp);
                        moduleInfo.ChannelAddressesIn[clamp] = discreteOffset;
                        moduleInfo.ChannelAddressesOut[clamp] = discreteOffset;
                    }
                    else
                    {
                        moduleInfo.ChannelAddressesIn[clamp] = offsetIn;
                        moduleInfo.ChannelAddressesOut[clamp] = offsetOut;
                        deviceOffset = device.GetMaxIOLinkSize();
                        offsetIn += deviceOffset;
                        offsetOut += deviceOffset;
                    }
                }
            }
        }

        /// <summary>
        /// Расчет дискретного смещения для IO-Link модуля Phoenix Contact.
        /// </summary>
        /// <param name="logicalClamp">Порядковый номер привязанной 
        /// клеммы</param>
        /// <param name="moduleOffset">Начало смещения модуля в словах (word)
        /// </param>
        /// <returns>Дискретное смещение</returns>
        private int CalculateDiscreteOffsetForIOLink(int moduleOffset,
            int logicalClamp)
        {
            int convertToBytes = moduleOffset * 2;
            int startingDiscreteOffsetInBytes = convertToBytes + 2;
            int convertToBits = startingDiscreteOffsetInBytes * 8;
            logicalClamp -= 1;
            int discreteOffset = convertToBits + logicalClamp;

            return discreteOffset;
        }

        List<Device.IODevice>[] devices;
        List<Device.IODevice.IOChannel>[] devicesChannels;
        IOModuleInfo moduleInfo;

        // Сервисные слова.
        int offsetIn = 3;
        int offsetOut = 3;
    }
}
