using IO;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// Технологическое устройство, подключенное к модулям ввода\вывод IO.
    /// </summary>
    public class IODevice : Device
    {
        /// <summary>
        /// Получение строкового представления подтипа устройства.
        /// </summary>
        public virtual string GetDeviceSubTypeStr(DeviceType dt,
            DeviceSubType dst)
        {
            switch(dt)
            {
                case DeviceType.NONE:
                    return dt.ToString();
            }
            return "";
        }

        /// <summary>
        /// Получение свойств устройства.
        /// </summary>
        public virtual List<string> GetDeviceProperties(DeviceType dt, 
            DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.NONE:
                    return null; 
            }
            return null;
        }

        /// <summary>
        /// Конструктор на основе имени.
        /// </summary>
        /// <param name="name">Имя устройства (формат - А1V12).</param>
        /// <param name="description">Описание устройства.</param>
        /// <param name="deviceType">Тип устройства (V для А1V12).</param>
        /// <param name="deviceNumber">Номер устройства (12 для А1V12).</param>
        /// <param name="objectName">Объект устройства (А для А1V12).</param>
        /// <param name="objectNumber">Номер объекта устройства (1 для А1V12).</param>
        protected internal IODevice(string fullName, string description,
            string deviceType, int deviceNumber, string objectName, int objectNumber)
            : this(fullName, description, deviceNumber, objectName, objectNumber)
        {
            try
            {
                dType = (DeviceType)Enum.Parse(typeof(DeviceType), deviceType, true);
            }
            catch (Exception)
            {
                dType = DeviceType.NONE;
            }
        }

        /// <summary>
        /// Конструктор на основе имени.
        /// </summary>
        /// <param name="name">Имя устройства (формат - А1V12).</param>
        /// <param name="description">Описание устройства.</param>
        /// <param name="deviceNumber">Номер устройства (12 для А1V12).</param>
        /// <param name="objectName">Объект устройства (А для А1V12).</param>
        /// <param name="objectNumber">Номер объекта устройства (1 для А1V12).</param>
        protected internal IODevice(string fullName, string description,
            int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            DO = new List<IOChannel>();
            DI = new List<IOChannel>();
            AO = new List<IOChannel>();
            AI = new List<IOChannel>();

            parameters = new Dictionary<string, object>();
            rtParameters = new Dictionary<string, object>();
            properties = new Dictionary<string, object>();

            IOLinkProperties = new IOLinkSize();
        }

        /// <summary>
        /// Установить размеры IO-Link областей устройства.
        /// </summary>
        /// <param name="articleName">Изделие устройства</param>
        public void SetIOLinkSizes(string articleName)
        {
            var articles = DeviceManager.GetInstance().IOLinkSizes;
            if (articles.ContainsKey(articleName) == false)
            {
                return;
            }

            var sizes = articles[articleName];
            this.IOLinkProperties.SizeIn = sizes.SizeIn;
            this.IOLinkProperties.SizeOut = sizes.SizeOut;
        }

        /// <summary>
        /// Установка номера узла, в котором подключается устройство.
        /// </summary>
        public void SetLocation(int devLocation)
        {
            dLocation = devLocation;
        }

        /// <summary>
        /// Установка параметра.
        /// </summary>
        public string SetParameter(string name, double value)
        {
            string res = "";

            object val = null;
            if (parameters.TryGetValue(name, out val))
            {
                parameters[name] = value;
            }
            else
            {
                res = string.Format("\"{0}\" - параметр не найден\n", name);
            }

            return res;
        }

        /// <summary>
        /// Установка рабочего параметра.
        /// </summary>
        public string SetRuntimeParameter(string name, double value)
        {
            string res = "";

            object val = null;
            if (rtParameters.TryGetValue(name, out val))
            {
                rtParameters[name] = value;
            }
            else
            {
                res = string.Format("\"{0}\" - рабочий параметр не найден\n", name);
            }

            return res;
        }

        /// <summary>
        /// Получение рабочего параметра.
        /// </summary>
        /// <param name="name">Имя устройства</param>
        /// <returns></returns>
        public string GetRuntimeParameter(string name)
        {
            object value = default;

            if (rtParameters.TryGetValue(name, out value))
            {
                return value.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Сброс канала ввода\вывода.
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства канала.
        /// </param>   
        /// <param name="comment">Комментарий к каналу.</param>
        /// <param name="error">Строка с описанием ошибки при возникновении 
        /// таковой.</param>
        public bool ClearChannel(
            IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace,
            string comment, string channelName)
        {
            List<IOChannel> findedChannels = GetChannels(addressSpace,
                channelName, comment);

            if (findedChannels.Count > 0)
            {
                foreach (IOChannel channel in findedChannels)
                {
                    channel.Clear();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Установка канала ввода\вывода.
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства канала.
        /// </param>
        /// <param name="node">Номер узла.</param>
        /// <param name="module">Номер модуля.</param>
        /// <param name="physicalKlemme">Номер клеммы.</param>
        /// <param name="comment">Комментарий к каналу.</param>
        /// <param name="error">Строка с описанием ошибки при возникновении 
        /// таковой.</param>
        public bool SetChannel(IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace,
            int node, int module, int physicalKlemme, string comment,
            out string error, int fullModule, int logicalPort,
            int moduleOffset, string channelName)
        {
            error = "";

            List<IOChannel> findedChannels = GetChannels(addressSpace,
                channelName, comment);

            if (findedChannels.Count > 0)
            {
                foreach (IOChannel channel in findedChannels)
                {
                    if (!channel.IsEmpty())
                    {
                        error = string.Format(
                            "\"{0}\" : канал {1}.\"{2}\" уже привязан " +
                            "к A{3}.{4} \"{5}\".",
                            name, addressSpace, comment,
                            100 * (channel.Node + 1) + channel.Module,
                            channel.PhysicalClamp, channel.Comment);
                        return false;
                    }

                    channel.SetChannel(node, module, physicalKlemme,
                        fullModule, logicalPort, moduleOffset);

                    List<IONode> nodes = IOManager.GetInstance().IONodes;
                    if (nodes.Count > node &&
                        nodes[node].IOModules.Count > module - 1)
                    {
                        nodes[node].IOModules[module - 1]
                            .AssignChannelToDevice(
                            physicalKlemme, this, channel);
                    }
                }
                return true;
            }
            else
            {
                error = string.Format(
                    "\"{0}\" : нет такого канала {1}:\"{2}\".",
                    name, addressSpace, comment);
                return false;
            }
        }

        /// <summary>
        /// Получить каналы устройства, которые привязывались или будут
        /// привязываться к модулю ввода-вывода
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства
        /// модуля ввода-вывода</param>
        /// <param name="channelName">Имя канала для IO-Link</param>
        /// <param name="comment">Комментарий канала</param>
        /// <returns></returns>
        private List<IOChannel> GetChannels(
            IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace, string channelName,
            string comment)
        {
            var IO = new List<IOChannel>();

            switch (addressSpace)
            {
                case IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                    IO.AddRange(DO);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                    IO.AddRange(DI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                    IO.AddRange(AO);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                    IO.AddRange(AI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI:
                    IO.AddRange(AO);
                    IO.AddRange(AI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.DODI:
                    IO.AddRange(DO);
                    IO.AddRange(DI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI:
                    switch (channelName)
                    {
                        default:
                            IO.AddRange(AO);
                            IO.AddRange(AI);
                            break;

                        case "DO":
                            IO.AddRange(DO);
                            break;

                        case "DI":
                            IO.AddRange(DI);
                            break;
                    }
                    break;
            }

            bool haveNewLineSymbol = comment
                .Contains(CommonConst.NewLineWithCarriageReturn) ||
                comment.Contains(CommonConst.NewLine);
            if (haveNewLineSymbol)
            {
                comment = comment
                    .Replace(CommonConst.NewLineWithCarriageReturn, "");
                comment = comment
                    .Replace(CommonConst.NewLine, "");
            }

            List<IOChannel> findedChannels = IO.FindAll(delegate (IOChannel channel)
            {
                return channel.Comment == comment;
            });

            return findedChannels;
        }

        /// <summary>
        /// Установка свойства. У устройства могут быть дополнительные свойства,
        /// которые задаются отдельно (доп. поле №4).
        /// </summary>
        virtual public string SetProperty(string name, object value)
        {
            string res = "";

            object val = null;
            if (properties.TryGetValue(name, out val))
            {
                properties[name] = value;
            }
            else
            {
                res = string.Format("\"{0}\" - свойство не найдено\n", name);
            }

            return res;
        }

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public virtual string Check()
        {
            string res = "";

            foreach (IOChannel ch in DO)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал DO \"{1}\".\n",
                        name, ch.Comment);
                }
            }
            foreach (IOChannel ch in DI)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал  DI \"{1}\".\n",
                        name, ch.Comment);
                }
            }
            foreach (IOChannel ch in AO)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал  AO \"{1}\".\n",
                        name, ch.Comment);
                }
            }
            foreach (IOChannel ch in AI)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал  AI \"{1}\".\n",
                        name, ch.Comment);
                }
            }

            foreach (var par in parameters)
            {
                if (par.Value == null)
                {
                    res += string.Format("\"{0}\" : не задан параметр \"{1}\".\n",
                        name, par.Key);
                }
            }

            foreach (var par in rtParameters)
            {
                if (par.Value == null)
                {
                    res += string.Format("\"{0}\" : не задан рабочий параметр \"{1}\".\n",
                        name, par.Key);
                }
            }

            foreach (var prop in properties)
            {
                if (prop.Value == null)
                {
                    res += string.Format("\"{0}\" : не задано свойство \"{1}\".\n",
                        name, prop.Key);
                }
            }

            return res;
        }

        /// <summary>
        /// Связанная функция.        
        /// </summary>
        public Eplan.EplApi.DataModel.Function EplanObjectFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Сохранение в виде массива данных (для экспорта в таблицу).
        /// </summary>
        /// <returns>Количество записанных строк.</returns>
        public int SaveAsArray(ref object[,] arr, int row, int maxColumn)
        {
            int column = 0;

            arr[row, column++] = name;
            arr[row, column++] = description;
            arr[row, column++] = dType.ToString();
            if (dType != DeviceType.NONE && dSubType != DeviceSubType.NONE)
            {
                arr[row, column++] = GetDeviceSubTypeStr(dType, dSubType);
            }
            else
            {
                arr[row, column++] = "";
            }

            //Параметры.
            foreach (var par in parameters)
            {
                arr[row, column++] = par.Key;
                arr[row, column++] = par.Value;
                if (column >= maxColumn) break;
            }

            return 1;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        virtual public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "{\n";
            res += prefix + "name    = \'" + Name + "\',\n";
            res += prefix + "descr   = \'" + Description.Replace("\n", ". ") + 
                "\',\n";
            res += prefix + "dtype   = " + (int)dType + ",\n";
            res += prefix + "subtype = " + (int)dSubType + ", -- " + 
                GetDeviceSubTypeStr(dType, dSubType) + "\n";
            res += prefix + $"article = \'{ArticleName}\',\n";

            if (properties.Count > 0)
            {
                var validProperties = properties
                    .Where(x => x.Value.ToString() != "\'\'" &&
                    x.Value != null &&
                    x.Value.ToString() != "");
                if (validProperties.Count() > 0)
                {
                    res += prefix + "prop = --Дополнительные свойства\n";
                    res += prefix + "\t{\n";

                    foreach (var prop in properties)
                    {
                        res += prefix + $"\t{prop.Key} = {prop.Value},\n";
                    }
                    res += prefix + "\t},\n";
                }
            }

            int bindedDO = CountOfBindedChannels(DO);
            if (DO.Count > 0 && bindedDO > 0)
            {
                res += prefix + "DO =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in DO)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            int bindedDI = CountOfBindedChannels(DI);
            if (DI.Count > 0 && bindedDI > 0)
            {
                res += prefix + "DI =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in DI)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            int bindedAO = CountOfBindedChannels(AO);
            if (AO.Count > 0 && bindedAO > 0)
            {
                res += prefix + "AO =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in AO)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            int bindedAI = CountOfBindedChannels(AI);
            if (AI.Count > 0 && bindedAI > 0)
            {
                res += prefix + "AI =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in AI)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            if (rtParameters.Count > 0)
            {
                string tmp = "";

                foreach (var par in rtParameters)
                {
                    if (par.Value != null)
                    {
                        string tmpForSpacebars = $"\t\t\t\t{par.Value},";
                        int tmpForSpacebarsLength = tmpForSpacebars.Length % 4;
                        var spacebars = 
                            new string(' ', 4 + (4 - tmpForSpacebarsLength));

                        tmp += $"\t\t\t\t{par.Value},{spacebars}--{par.Key}\n";
                    }
                }

                if (tmp != "")
                {
                    res += prefix + "rt_par = \n\t\t\t\t{\n";
                    res += tmp;
                    res += prefix + "\t\t},\n";
                }
            }

            if (parameters.Count > 0)
            {
                string tmp = "";
                foreach (var par in parameters)
                {
                    if (par.Value != null)
                    {
                        tmp += par.Value + " --[[" + par.Key + "]], ";
                    }
                }

                if (tmp != "")
                {
                    res += prefix + "par = {";
                    res += tmp.Remove(tmp.Length - 1 - 1);
                    res += " }\n";
                }
            }

            res += prefix + "}";

            return res;
        }

        /// <summary>
        /// Сортировка каналов устройства для соответствия.
        /// </summary>
        public void sortChannels()
        {
            if (dType == DeviceType.V)
            {
                if (DI.Count > 1)
                {
                    List<IOChannel> tmp = new List<IOChannel>();

                    foreach (string descr in new string[] { "Открыт", "Закрыт" })
                    {
                        IOChannel resCh = DI.Find(delegate (IOChannel ch)
                        {
                            return ch.Comment == descr;
                        });

                        if (resCh != null)
                        {
                            tmp.Add(resCh);
                        }

                    }

                    DI = tmp;
                }

                if (DO.Count > 1)
                {
                    List<IOChannel> tmp2 = new List<IOChannel>();

                    foreach (string descr in new string[] { "Открыть", "Открыть мини", "Открыть ВС",
                                    "Открыть НС", "Закрыть" })
                    {
                        IOChannel resCh = DO.Find(delegate (IOChannel ch)
                        {
                            return ch.Comment == descr;
                        });

                        if (resCh != null)
                        {
                            tmp2.Add(resCh);
                        }

                    }

                    DO = tmp2;
                }
            }
        }

        /// <summary>
        /// Получение списка каналов устройства.
        /// </summary>
        public List<IOChannel> Channels
        {
            get
            {
                List<IOChannel> res = new List<IOChannel>();
                res.AddRange(DO);
                res.AddRange(DI);
                res.AddRange(AO);
                res.AddRange(AI);

                return res;
            }
        }


        /// <summary>
        /// Очистка привязки каналов устройства.
        /// </summary>
        public void ClearChannels()
        {
            foreach (IOChannel ch in Channels)
            {
                ch.Clear();
            }
        }

        /// <summary>
        /// Возвращает количество каналов, которые привязаны к модулю
        /// ввода-вывода
        /// </summary>
        /// <param name="channels">Список каналов устройства 
        /// (AO,AI,DO,DI)</param>
        /// <returns></returns>
        private int CountOfBindedChannels(List<IOChannel> channels)
        {
            var count = 0;

            foreach (var channel in channels)
            {
                if (channel.IsEmpty() == false)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Получить свойства устройства.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                return properties;
            }
        }

        /// <summary>
        /// Получить параметры устройства.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get
            {
                return parameters;
            }
        }

        /// <summary>
        /// Свойство содержащее изделие, которое используется для устройства
        /// </summary>
        public string ArticleName { get; set; } = "";

        #region Закрытые поля.
        protected List<IOChannel> DO; ///Каналы дискретных выходов.
        protected List<IOChannel> DI; ///Каналы дискретных входов.
        protected List<IOChannel> AO; ///Каналы аналоговых выходов.
        protected List<IOChannel> AI; ///Каналы аналоговых входов.

        protected Dictionary<string, object> parameters;   ///Параметры.
        protected Dictionary<string, object> rtParameters; ///Рабочие параметры.
        protected Dictionary<string, object> properties; ///Свойства.

        internal IOLinkSize IOLinkProperties; ///IO-Link свойства устройства.
        #endregion

        /// <summary>
        /// Канал ввода-вывода.
        /// </summary>
        public class IOChannel
        {
            public static int Compare(IOChannel wx, IOChannel wy)
            {
                if (wx == null && wy == null)
                    return 0;

                if (wx == null)
                    return -1;

                if (wy == null)
                    return 1;

                return wx.ToInt().CompareTo(wy.ToInt());
            }

            /// <param name="node">Номер узла.</param>
            /// <param name="module">Номер модуля.</param>
            /// <param name="physicalClamp">Физический номер клеммы.</param>
            /// <param name="fullModule">Полный номер модуля (101).</param>
            /// <param name="logicalClamp">Порядковый логический номер клеммы.</param>
            /// <param name="moduleOffset">Сдвиг модуля к которому привязан канал.</param>
            public void SetChannel(int node, int module, int physicalClamp, int fullModule,
                int logicalClamp, int moduleOffset)
            {
                this.node = node;
                this.module = module;
                this.physicalClamp = physicalClamp;

                this.fullModule = fullModule;
                this.logicalClamp = logicalClamp;
                this.moduleOffset = moduleOffset;
            }

            /// <summary>
            /// Сброс привязки канала ввода-вывода.
            /// </summary>
            public void Clear()
            {
                node = -1;
                module = -1;
                physicalClamp = -1;
                fullModule = -1;
                logicalClamp = -1;
                moduleOffset = -1;
            }

            /// <param name="name">Имя канала (DO, DI, AO, AI).</param>
            /// <param name="node">Номер узла.</param>
            /// <param name="module">Номер модуля.</param>
            /// <param name="clamp">Номер клеммы.</param>
            /// <param name="comment">Комментарий к каналу.</param>
            public IOChannel(string name, int node, int module, int clamp, string comment)
            {
                this.name = name;

                this.node = node;
                this.module = module;
                this.physicalClamp = clamp;
                this.comment = comment;
            }

            private int ToInt()
            {
                switch (name)
                {
                    case "DO":
                        return 0;

                    case "DI":
                        return 1;

                    case "AI":
                        return 2;

                    case "AO":
                        return 3;

                    case "AIAO":
                        return 4;

                    case "DODI":
                        return 5;

                    default:
                        return 6;
                }
            }

            /// <summary>
            /// Сохранение в виде таблицы Lua.
            /// </summary>
            /// <param name="prefix">Префикс (для выравнивания).</param>
            public string SaveAsLuaTable(string prefix)
            {
                string res = "";

                if (IOManager.GetInstance()[node] != null &&
                    IOManager.GetInstance()[node][module - 1] != null &&
                    physicalClamp >= 0)
                {
                    res += prefix + "{\n";

                    int offset;
                    switch (name)
                    {
                        case "DO":
                            offset = CalculateDO();
                            break;

                        case "AO":
                            offset = CalculateAO();
                            break;

                        case "DI":
                            offset = CalculateDI();
                            break;

                        case "AI":
                            offset = CalculateAI();
                            break;

                        default:
                            offset = -1;
                            break;
                    }

                    if (comment != "")
                    {
                        res += prefix + "-- " + comment + "\n";
                    }

                    res += prefix + $"node          = {node},\n";
                    res += prefix + $"offset        = {offset},\n";
                    res += prefix + $"physical_port = {physicalClamp},\n";
                    res += prefix + $"logical_port  = {logicalClamp},\n";
                    res += prefix + $"module_offset = {moduleOffset}\n";

                    res += prefix + "},\n";
                }

                return res;
            }

            /// <summary>
            /// Расчет AI адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateAI()
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesIn.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    offset = md.InOffset;
                    offset += md.Info.ChannelAddressesIn[physicalClamp];

                    return offset;
                }

                return offset;
            }

            /// <summary>
            /// Расчет AO адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateAO()
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesOut.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    offset = md.OutOffset;
                    offset += md.Info.ChannelAddressesOut[physicalClamp];

                    return offset;
                }

                return offset;
            }

            /// <summary>
            /// Расчет DI адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateDI()
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesIn.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    if (md.isIOLink() == true)
                    {
                        offset = 0;
                    }
                    else
                    {
                        offset = md.InOffset;
                    }
                    offset += md.Info.ChannelAddressesIn[physicalClamp];

                    return offset;
                }

                return offset;
            }

            /// <summary>
            /// Расчет DO адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateDO()
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesOut.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    if (md.isIOLink() == true)
                    {
                        offset = 0;
                    }
                    else
                    {
                        offset = md.OutOffset;
                    }
                    offset += md.Info.ChannelAddressesOut[physicalClamp];

                    return offset;
                }

                return offset;
            }

            public bool IsEmpty()
            {
                return node == -1;
            }

            /// <summary>
            /// Номер узла.
            /// </summary>
            public int Node
            {
                get
                {
                    return node;
                }
            }

            /// <summary>
            /// Номер модуля.
            /// </summary>
            public int Module
            {
                get
                {
                    return module;
                }
            }

            /// <summary>
            /// Физический номер клеммы.
            /// </summary>
            public int PhysicalClamp
            {
                get
                {
                    return physicalClamp;
                }
            }

            /// <summary>
            /// Полный номер модуля
            /// </summary>
            public int FullModule
            {
                get
                {
                    return fullModule;
                }
            }

            /// <summary>
            /// Комментарий
            /// </summary>
            public string Comment
            {
                get
                {
                    return comment;
                }
            }

            /// <summary>
            /// Имя канала (DI,DO, AI,AO)
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
            }

            /// <summary>
            /// Логический номер клеммы (порядковый)
            /// </summary>
            public int LogicalClamp
            {
                get
                {
                    return logicalClamp;
                }
            }

            /// <summary>
            /// Сдвиг начала модуля
            /// </summary>
            public int ModuleOffset
            {
                get
                {
                    return moduleOffset;
                }
            }

            /// <summary>
            /// Шаблон для разбора комментария к устройству.
            /// </summary>
            public const string ChannelCommentPattern =
                @"(Открыть мини(?n:\s+|$))|" +
                @"(Открыть НС(?n:\s+|$))|" +
                @"(Открыть ВС(?n:\s+|$))|" +
                @"(Открыть(?n:\s+|$))|" +
                @"(Закрыть(?n:\s+|$))|" +
                @"(Открыт(?n:\s+|$))|" +
                @"(Закрыт(?n:\s+|$))|" +
                @"(Объем(?n:\s+|$))|" +
                @"(Поток(?n:\s+|$))|" +
                @"(Пуск(?n:\s+|$))|" +
                @"(Реверс(?n:\s+|$))|" +
                @"(Обратная связь(?n:\s+|$))|" +
                @"(Частота вращения(?n:\s+|$))|" +
                @"(Авария(?n:\s+|$))|" +
                @"(Напряжение моста\(\+Ud\)(?n:\s+|$))|" +
                @"(Референсное напряжение\(\+Uref\)(?n:\s+|$))";

            #region Закрытые поля
            private int node;            ///Номер узла.
            private int module;          ///Номер модуля.
            private int fullModule;      ///Полный номер модуля.
            private int physicalClamp;   ///Физический номер клеммы.
            private string comment;      ///Комментарий.
            private string name;         ///Имя канала (DO, DI, AO ,AI).
            private int logicalClamp;    ///Логический номер клеммы.
            private int moduleOffset;    ///Сдвиг начала модуля.
            #endregion
        }

        /// <summary>
        /// Размер IO-Link областей устройства ввода-вывода.
        /// </summary>
        public class IOLinkSize
        {
            public IOLinkSize()
            {
                SizeIn = 0;
                SizeOut = 0;
            }

            public IOLinkSize(int sizeIn, int sizeOut)
            {
                SizeIn = sizeIn;
                SizeOut = sizeOut;
            }

            /// <summary>
            /// Возвращает максимальны размер байтовой области для модулей ввода
            /// вывода при расчете IO-Link адресов если используется
            /// Phoenix Contact
            /// </summary>
            /// <returns></returns>
            public int GetMaxIOLinkSize()
            {
                return SizeOut > SizeIn ? SizeOut : SizeIn;
            }

            public int SizeIn { get; set; }
            public int SizeOut { get; set; }
        }

    }
}
