using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using EasyEPlanner;

/// <summary>
/// Пространство имен технологических устройств проекта (клапана, насосы...).
/// </summary>
namespace Device
{    
    /// <summary>
    /// Менеджер описания устройств для проекта.
    /// </summary>
    public class DeviceManager
    {
        public void GetObjectForXML(TreeNode rootNode)
        {
            foreach (IODevice dev in devices)
            {
                if (dev != null)
                {
                    List<string> propertiesList = dev
                        .GetDeviceProperties(dev.DeviceType, dev.DeviceSubType);
                    if (propertiesList != null)
                    {
                        foreach (string strProp in propertiesList)
                        {
                            string nodeName = dev.DeviceType.ToString() + "_" + strProp;
                            if (!rootNode.Nodes.ContainsKey(nodeName))
                            {
                                TreeNode newNode = rootNode.Nodes.Add(nodeName, nodeName);
                                newNode.Nodes.Add(dev.Name + "." + strProp, dev.Name + "." + strProp);
                            }
                            else
                            {
                                TreeNode newNode = rootNode.Nodes.Find(nodeName, false)[0];
                                newNode.Nodes.Add(dev.Name + "." + strProp, dev.Name + "." + strProp);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Очистка устройств  проекта.
        /// </summary>
        public void Clear()
        {
            devices.Clear();
        }

        /// <summary>
        /// Проверка устройств на каналы без привязки
        /// </summary>
        public string Check()
        {
            var res = "";

            foreach (var dev in devices)
            {
                res += dev.Check();
            }

            long startingIP = EasyEPlanner.ProjectConfiguration
                .GetInstance().StartingIPInterval;
            long endingIP = EasyEPlanner.ProjectConfiguration.GetInstance()
                .EndingIPInterval;
            if (startingIP != 0 && endingIP != 0)
            {
                res += CheckDevicesIP(startingIP, endingIP);
            }           

            return res;
        }

        /// <summary>
        /// Проверить IP-адреса устройств.
        /// </summary>
        /// <param name="startingIP">Начало интервала адресов</param>
        /// <param name="endingIP">Конец интервала адресов</param>
        /// <returns>Ошибки</returns>
        private string CheckDevicesIP(long startingIP, long endingIP)
        {
            var errors = new List<string>();
            string ipProperty = "IP";

            var devicesWithIPProperty = Devices
                .Where(x => x.Properties.ContainsKey(ipProperty) &&
                x.Properties[ipProperty] != null).ToArray();
            foreach (var device in devicesWithIPProperty)
            {
                string IPstr = Regex.Match(device.Properties[ipProperty]
                    .ToString(), StaticHelper.CommonConst.IPAddressPattern)
                    .Value;
                if (IPstr == "")
                {
                    continue;
                }

                var devicesWithEqualsIP = devicesWithIPProperty
                    .Where(x => x.Properties[ipProperty].ToString() == 
                    device.Properties[ipProperty].ToString()).ToArray();
                if (devicesWithEqualsIP.Length > 1)
                {
                    var equalsDevicesNames = devicesWithEqualsIP
                        .Select(x => x.EPlanName).ToArray();
                    string error = $"IP-адреса устройств " +
                        $"{string.Join(",", equalsDevicesNames)} совпадают.\n";
                    errors.Add(error);
                }

                long devIP = StaticHelper.IPConverter.ConvertIPStrToLong(IPstr);
                if (devIP - startingIP < 0 || endingIP - devIP < 0)
                {
                    string error = $"IP-адрес устройства {device.EPlanName} " +
                    $"вышел за диапазон.\n";
                    errors.Add(error);
                }
            }

            errors = errors.Distinct().ToList();
            return string.Concat(errors);
        }

        /// <summary>
        /// Сортировка устройств  проекта.
        /// </summary>
        public void Sort()
        {
            devices.Sort();
        }

        /// <summary>
        /// Возвращает устройство по его имени в Eplan
        /// </summary>
        /// <param name="devName">Имя устройств в Eplan</param>
        /// <returns></returns>
        public IODevice GetDeviceByEplanName(string devName)
        {
            foreach (IODevice device in devices)
            {
                if (device.Name == devName) 
                {
                    return device;
                }
            }

            // Если не нашли, возвратим заглушку.
            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            // Устройства нет, вернет состояние заглушки.
            CheckDeviceName(devName, out name, out objectName,
                out objectNumber, out deviceType, out deviceNumber);
            return new IODevice(name, "заглушка", deviceType,
                deviceNumber, objectName, objectNumber);
        }

        /// <summary>
        /// Получение устройства по его имени (ОУ) из глобального списка.
        /// </summary>
        /// <param name="devName">Имя устройства.</param>
        /// <returns>Устройство с заданными именем или устройство-заглушка.</returns>
        public IODevice GetDevice(string devName)
        {
            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            CheckDeviceName(devName, out name, out objectName, out objectNumber,
                out deviceType, out deviceNumber);

            IODevice devStub = new IODevice(name, "заглушка",
                deviceType, deviceNumber, objectName, objectNumber);

            int resDevN = devices.BinarySearch(devStub);

            if (resDevN >= 0)
            {
                return devices[resDevN];
            }

            return devStub;
        }

        /// <summary>
        /// Получить устройство по индексу
        /// </summary>
        /// <param name="index">Индекс устройства</param>
        /// <returns></returns>
        public IODevice GetDeviceByIndex(int index)
        {
            if (index >= 0)
            {
                return devices[index];
            }
            else
            {
                return cap;
            }
        }

        /// <summary>
        /// Получить индекс устройства по его имени из общего пула устройств.
        /// </summary>
        /// <param name="devName">Имя устройства</param>
        /// <returns></returns>
        public int GetDeviceIndex(string devName)
        {
            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            CheckDeviceName(devName, out name, out objectName, out objectNumber,
                out deviceType, out deviceNumber);
            IODevice devStub = new IODevice(name, "заглушка",
                deviceType, deviceNumber, objectName, objectNumber);

            int resDevN = devices.IndexOf(devStub);

            return resDevN;
        }


        /// <summary>
        /// Шаблон для разбора имени для привязки устройств
        /// "==BR2=HUT1+KOAG5-V2"
        /// "+KOAG5-V2"
        /// </summary>
        public static readonly string BINDING_DEVICES_DESCRIPTION_PATTERN =
            @"(?<name>([+-])(?<object_main>([A-Z_]*?)(\d*?))(?<object>[A-Z_]*?)(?<object_n>\d*)(-*)(?<type>[A-Z_]+)(?<n>\d+))(\s+|$)";

        /// <summary>
        ///Шаблоны для разбора имени в остальных случаях: 
        /// "==BR2=HUT1+KOAG5-V2"
        /// "+KOAG5-V2"
        /// "KOAG5V2"
        /// </summary>
        public static readonly string DESCRIPTION_PATTERN =
             @"(?<name>(\+*)(?<object_main>([A-Z_]*?)(\d*?))(?<object>[A-Z_]*?)(?<object_n>\d*)(-*)(?<type>[A-Z_]+)(?<n>\d+))(\s+|$)";

        public static readonly string DESCRIPTION_PATTERN_MULTYLINE =
             @"((?<name>(\+*)(?<object_main>([A-Z_]*?)(\d*?))(?<object>[A-Z_]*?)(?<object_n>\d*)(-*)(?<type>[A-Z_]+)(?<n>\d+))(\s+|$))+";

        /// <summary>
        /// Проверка на корректное имя устройства.
        /// </summary>
        /// <param name="devName">Имя устройство.</param>
        static public bool CheckDeviceName(string fullDevName,
            out string devName, out string objectName, out int objectNumber,
            out string deviceType, out int deviceNumber)
        {
            bool res = false;
            objectName = "";
            objectNumber = 0;
            deviceType = "";
            deviceNumber = 0;

            devName = fullDevName;

            Match match = Regex.Match(fullDevName, DESCRIPTION_PATTERN,
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string type = match.Groups["type"].Value;
                switch (type)
                {
                    case "V":
                    case "VC":
                    case "M":
                    case "N":
                    case "LS":
                    case "TE":
                    case "GS":
                    case "FS":
                    case "FQT":
                    case "AO":
                    case "LT":
                    case "OS":
                    case "DI":
                    case "UPR":
                    case "DO":
                    case "QT":
                    case "AI":
                    case "HA":
                    case "HL":
                    case "SB":
                    case "WT":
                    case "PT":

                    case "Y":
                    case "DEV_VTUG": // Совместимость со старыми проектами

                        objectName = match.Groups["object_main"].Value + match.Groups["object"];
                        if (match.Groups["object_n"].Value != "")
                        {
                            objectNumber = System.Convert.ToInt32(
                                match.Groups["object_n"].Value);
                        }

                        deviceType = match.Groups["type"].Value;
                        if (match.Groups["n"].Value != "")
                        {
                            deviceNumber = System.Convert.ToInt32(
                                match.Groups["n"].Value);
                        }

                        devName = match.Groups["object_main"].Value + match.Groups["object"].Value +
                            match.Groups["object_n"].Value +
                            match.Groups["type"].Value +
                            match.Groups["n"].Value;

                        res = true;
                        break;
                }
            }

            return res;
        }
        /// <summary>
        /// Добавление канала ввода\вывода к устройству.
        /// </summary>
        /// <param name="devName">Имя устройство.</param>
        /// <param name="description">Описание устройства.</param>
        /// <param name="subType">Подтип устройства.</param>
        /// <param name="paramStr">Дополнительный строковый параметр - параметры.</param>
        /// <param name="rtParamStr">Дополнительный строковый параметр - рабочие параметры.</param>
        /// <param name="propStr">Дополнительный строковый параметр - свойства.</param>
        /// <param name="errStr">Описание ошибки при ее наличии.</param>
        public IODevice AddDeviceAndEFunction(string devName, string description,
            string subType, string paramStr, string rtParamStr, string propStr, int dLocation,
            Eplan.EplApi.DataModel.Function oF, out string errStr, string articleName)
        {
            IODevice dev = AddDevice(devName, description, subType, paramStr,
                rtParamStr, propStr, dLocation, out errStr, articleName);

            if (dev != null)
            {
                dev.EplanObjectFunction = oF;
            }

            return dev;
        }

        /// <summary>
        /// Добавление канала ввода\вывода к устройству.
        /// </summary>
        /// <param name="devName">Имя устройство.</param>
        /// <param name="description">Описание устройства.</param>
        /// <param name="subType">Подтип устройства.</param>
        /// <param name="paramStr">Дополнительный строковый параметр - параметры.</param>
        /// <param name="rtParamStr">Дополнительный строковый параметр - рабочие параметры.</param>
        /// <param name="propStr">Дополнительный строковый параметр - свойства.</param>
        /// <param name="errStr">Описание ошибки при ее наличии.</param>
        private IODevice AddDevice(string devName, string description,
            string subType, string paramStr, string rtParamStr, string propStr,
            int dLocation, out string errStr, string articleName)
        {
            errStr = "";
            IODevice dev = null;

            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;


            CheckDeviceName(devName, out name, out objectName,
                out objectNumber, out deviceType, out deviceNumber);

            // Если изделия нет или пустое, то оставляем пустое
            if (articleName == "" || articleName == null)
            {
                articleName = "";
            }

            switch (deviceType)
            {
                case "V":
                    dev = new V(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "VC":
                    dev = new VC(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "M":
                case "N":
                    dev = new M(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "LS":
                    dev = new LS(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "TE":
                    dev = new TE(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "GS":
                    dev = new GS(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "FS":
                    dev = new FS(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "FQT":
                    dev = new FQT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "AO":
                    dev = new AO(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "LT":
                    dev = new LT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "OS":
                case "DI":
                    dev = new DI(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "UPR":
                case "DO":
                    dev = new DO(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "QT":
                    dev = new QT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "AI":
                    dev = new AI(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "HA":
                    dev = new HA(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "HL":
                    dev = new HL(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "SB":
                    dev = new SB(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "WT":
                    dev = new WT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "PT":
                    dev = new PT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "Y":
                    dev = new Y(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;
                case "DEV_VTUG": // Совместимость со старыми проектами
                    dev = new DEV_VTUG(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                default:
                    break;
            }

            if (dev != null)
            {
                if (!devices.Contains(dev))
                {
                    subType = subType.ToUpper();

                    errStr += dev.SetSubType(subType);

                    //Разбор параметров.
                    if (paramStr != "")
                    {
                        //Шаблоны для разбора параметров - 0-20 .
                        const string paramsPattern = @"(?<p_name>\w+)=(?<p_value>-?\d+\.?\d*),*";

                        Match paramsMatch = Regex.Match(paramStr, paramsPattern, RegexOptions.IgnoreCase);
                        while (paramsMatch.Success)
                        {
                            string res;
                            if (paramsMatch.Groups["p_value"].Value.EndsWith("."))
                            {
                                string str = paramsMatch.Groups["p_value"].Value.Remove(paramsMatch.Groups["p_value"].Value.Length - 1);
                                res = dev.SetParameter(paramsMatch.Groups["p_name"].Value, Convert.ToDouble(str));
                            }
                            else
                            {
                                res = dev.SetParameter(paramsMatch.Groups["p_name"].Value,
                                   Convert.ToDouble(paramsMatch.Groups["p_value"].Value));
                            }
                            if (res != "")
                            {
                                errStr += devName + " - " + res;
                            }

                            paramsMatch = paramsMatch.NextMatch();
                        }
                    }

                    //Разбор рабочих параметров.
                    if (rtParamStr != "")
                    {
                        //Шаблоны для разбора параметров - 0-20 .
                        const string paramsPattern = @"(?<p_name>\w+)=(?<p_value>-?\d+\.?\d*),*";

                        Match paramsMatch = Regex.Match(rtParamStr, paramsPattern, RegexOptions.IgnoreCase);
                        while (paramsMatch.Success)
                        {
                            string res;
                            if (paramsMatch.Groups["p_value"].Value.EndsWith("."))
                            {
                                string str = paramsMatch.Groups["p_value"].Value.Remove(paramsMatch.Groups["p_value"].Value.Length - 1);
                                res = dev.SetRuntimeParameter(paramsMatch.Groups["p_name"].Value, Convert.ToDouble(str));
                            }
                            else
                            {
                                res = dev.SetRuntimeParameter(paramsMatch.Groups["p_name"].Value,
                                   Convert.ToDouble(paramsMatch.Groups["p_value"].Value));
                            }
                            if (res != "")
                            {
                                errStr += devName + " - " + res;
                            }

                            paramsMatch = paramsMatch.NextMatch();
                        }
                    }

                    //Разбор свойств.
                    if (propStr != "")
                    {
                        //Шаблоны для разбора параметров - 0-20 .
                        const string propPattern = @"(?<p_name>\w+)=(?<p_value>\'[\w.]*\'),*";

                        Match propsMatch = Regex.Match(propStr, propPattern, RegexOptions.IgnoreCase);
                        while (propsMatch.Success)
                        {
                            string res = dev.SetProperty(propsMatch.Groups["p_name"].Value,
                               propsMatch.Groups["p_value"].Value);

                            if (res != "")
                            {
                                errStr += devName + " - " + res;
                            }
                            if (propsMatch.Groups["p_name"].Value.Equals("IP"))
                            {
                                bool foundMatch = false;
                                var ipprop = propsMatch.Groups["p_value"].Value.Trim(new char[] { '\'' });
                                try
                                {
                                    foundMatch = Regex.IsMatch(ipprop, @"\A(?:^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])$)\Z");
                                }
                                catch
                                {
                                    // Syntax error in the regular expression
                                }
                                if (!foundMatch)
                                {
                                    errStr += String.Format("Устройство {0}: неверный IP-адрес - \'{1}\'.\n", devName, ipprop);
                                }
                            }

                            propsMatch = propsMatch.NextMatch();
                        }
                    }

                    //Установка параметра номер а шкафа для устройства.
                    dev.SetLocation(dLocation);

                    devices.Add(dev);
                }
                else
                {
                    errStr = string.Format("\"{0}\"  - дублируется.",
                        devName);
                }

            }
            else
            {
                errStr = string.Format("\"{0}\" - неизвестное устройство.",
                    devName);
            }
            return dev;
        }

        /// <summary>
        /// Добавление канала ввода\вывода к устройству.
        /// </summary>
        /// <param name="dev">Устройство.</param>
        /// <param name="addressSpace">Адресное пространство.</param>
        /// <param name="node">Узел.</param>
        /// <param name="module">Модуль.</param>
        /// <param name="physicalKlemme">Клемма.</param>
        /// <param name="comment">Описание канала.</param>
        /// <param name="errors">Строка с описанием ошибки при наличии таковой.</param>
        /// <param name="fullModule">Полный номер модуля</param>
        /// <param name="logicalClamp">Логический порядковый номер клеммы</param>
        /// <param name="moduleOffset">Начальный сдвиг модуля</param>
        public void AddDeviceChannel(IODevice dev,
            IO.IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace,
            int node, int module, int physicalKlemme, string comment,
            out string errors, int fullModule, int logicalClamp, int moduleOffset, string channelName)
        {
            dev.SetChannel(addressSpace, node, module, physicalKlemme,
                comment, out errors, fullModule, logicalClamp, moduleOffset, channelName);
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "------------------------------------------------------------------------------\n";
            res += "--Устройства\n";
            res += prefix + "devices =\n";
            res += prefix + "\t{\n";

            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                dev.sortChannels();
                res += dev.SaveAsLuaTable(prefix + "\t\t") + ",\n\n";
            }

            res += prefix + "\t}\n";
            res = res.Replace("\t", "    ");

            return res;
        }

        /// <summary>
        /// Сохранение устройств в виде скрипта Lua. Для последующего доступа
        /// по имени. Строки в виде: "S1V23 = V( 'S1V23' ) ".
        /// </summary>
        public string SaveDevicesAsLuaScript()
        {
            string str = "system = system or {}\n";
            str += "system.init_dev_names = function()\n";

            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                if (dev.ObjectNumber > 0 && dev.ObjectName == "")
                {
                    str += "\t_";
                }
                else
                {
                    str += "\t";
                }
                str += dev.Name + " = " + dev.DeviceType.ToString() + "(\'" + dev.Name + "\')\n";
            }
            str += "\n";

            int i = 0;
            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                str += "\t__" + dev.Name + " = DEVICE( " + i + " )\n";
                i++;
            }
            str += "end\n";
            str = str.Replace("\t", "    ");

            return str;
        }

        /// <summary>
        /// Закрытый конструктор.
        /// </summary>
        private DeviceManager()
        {
            devices = new List<IODevice>();
            InitIOLinkSizesForDevices();
        }

        /// <summary>
        /// Инициализировать информацию об IO-Link устройствах.
        /// </summary>
        private void InitIOLinkSizesForDevices()
        {
            IOLinkSizes = new Dictionary<string, IODevice.IOLinkSize>();
            var lua = new LuaInterface.Lua();
            const string devicesFile = "sys_iolink_devices.lua";
            var fullPath = Path.Combine(
                ProjectManager.GetInstance().SystemFilesPath, devicesFile);

            if (File.Exists(fullPath))
            {
                object[] result = lua.DoFile(fullPath);
                if (result == null)
                {
                    return;
                }

                var dataTables = result[0] as LuaInterface.LuaTable;
                foreach (var table in dataTables.Values)
                {
                    var tableData = table as LuaInterface.LuaTable;
                    string articleName = (string)tableData["articleName"];
                    // Читаем float т.к могут быть 0.5 размер (в словах)
                    float sizeIn = (float)((double)tableData["sizeIn"]);
                    float sizeOut = (float)((double)tableData["sizeOut"]);

                    if (IOLinkSizes.ContainsKey(articleName) == false)
                    {
                        // Для расчета IO-Link округляем до целого, кроме 0
                        // Для настройки - оставляем как есть
                        int intSizeIn = Convert.ToInt32(
                            Math.Round(sizeIn, MidpointRounding.AwayFromZero));
                        int intSizeOut = Convert.ToInt32(
                            Math.Round(sizeOut, MidpointRounding.AwayFromZero));
                        var properties = new IODevice.IOLinkSize
                        {
                            SizeIn = intSizeIn,
                            SizeOut = intSizeOut,
                            SizeInFromFile = sizeIn,
                            SizeOutFromFile = sizeOut
                        };
                        IOLinkSizes.Add(articleName, properties);
                    }
                }
            }
            else
            {
                string template = EasyEPlanner.Properties.Resources
                    .ResourceManager.GetString("IOLinkDevicesFilePattern");
                File.WriteAllText(fullPath, template);
            }
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static DeviceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new DeviceManager();
            }
            return instance;
        }

        /// <summary>
        /// Очистка привязки каналов.
        /// </summary>
        public void ClearDevChannels()
        {
            foreach (IODevice dev in devices)
            {
                dev.ClearChannels();
            }
        }

        public List<IODevice> Devices
        {
            get
            {
                return devices;
            }
        }

        /// <summary>
        /// Функция, проверяющая являются ли переданные устройства
        /// устройствами с AS-интерфейсом
        /// </summary>
        /// <param name="devices">Список ОУ устройств через разделитель</param>
        /// <returns></returns>
        public bool? IsASInterfaceDevices(string devices, out string errors)
        {
            bool? isASInterface = false;
            errors = "";
            const int MinimalDevicesCount = 2;
            var deviceMatches = Regex.Matches(devices, DeviceNamePattern);

            if (deviceMatches.Count < MinimalDevicesCount)
            {
                return isASInterface;
            }

            var checkingList = new List<bool>();
            foreach (Match deviceMatch in deviceMatches)
            {
                var device = GetDevice(deviceMatch.Value);
                if (device.DeviceSubType == DeviceSubType.V_AS_MIXPROOF ||
                    device.DeviceSubType == DeviceSubType.V_AS_DO1_DI2)
                {
                    checkingList.Add(true);
                }
                else
                {
                    checkingList.Add(false);
                }
            }

            checkingList = checkingList.Distinct().ToList();

            if (checkingList.Count == 2)
            {
                isASInterface = null;
                errors += "Проверьте все AS-i модули. В привязке " +
                    "присутствуют не AS-i подтипы.\n ";
            }
            else if (checkingList.Count == 1)
            {
                if (checkingList[0] == true)
                {
                    isASInterface = true;
                }
                else
                {
                    isASInterface = false;
                }
            }

            return isASInterface;
        }

        /// <summary>
        /// Проверяет наличие параметра R_AS_NUMBER в AS-i устройстве.
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool CheckASNumbers(string devices, out string errors)
        {
            var deviceMatches = Regex.Matches(devices, DeviceNamePattern);

            errors = "";
            var errorsBuffer = "";
            foreach (Match deviceMatch in deviceMatches)
            {
                var device = GetDevice(deviceMatch.Value);
                string parameter = device.GetRuntimeParameter("R_AS_NUMBER");
                if (parameter == null)
                {
                    errorsBuffer += $"В устройстве {device.EPlanName} " +
                        $"отсутствует R_AS_NUMBER.\n ";
                }
                else
                {
                    var ASNumber = new int();
                    bool isNumber = int.TryParse(parameter, out ASNumber);
                    if (isNumber == false)
                    {
                        errorsBuffer += $"В устройстве " +
                            $"{device.EPlanName} некорректно задан параметр " +
                            $"R_AS_NUMBER.\n ";
                    }
                    if (isNumber == true && ASNumber < 1 && ASNumber > 62)
                    {
                        errorsBuffer += $"В устройстве " +
                            $"{device.EPlanName} некорректно задан диапазон " +
                            $"R_AS_NUMBER (от 1 до 62).\n ";
                    }
                }
            }

            var isValid = false;
            if (errorsBuffer != "")
            {
                isValid = false;
                errors += errorsBuffer;
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Является ли привязка множественной
        /// </summary>
        /// <param name="devices">Список устройств</param>
        /// <returns></returns>
        public bool IsMultipleBinding(string devices)
        {
            var isMultiple = false;

            var matches = Regex.Matches(devices, DeviceNamePattern);

            if (matches.Count > 1)
            {
                isMultiple = true;
            }

            return isMultiple;
        }

        /// <summary>
        /// Шаблон для получение ОУ устройства.
        /// </summary>
        public const string DeviceNamePattern = "(\\+[A-Z0-9_]*-[A-Z0-9_]+)";

        /// <summary>
        /// Используемое имя для пневмоострова.
        /// </summary>
        public const string ValveTerminalName = "-Y";

        /// <summary>
        /// Шаблон для разбора ОУ пневмоострова
        /// </summary>
        public const string valveTerminalPattern = @"([A-Z0-9]+\-[Y0-9]+)";

        private static IODevice cap = new IODevice("Заглушка", "", 0, "", 0);
        private List<IODevice> devices;       ///Устройства проекта.     
        private static DeviceManager instance;  ///Экземпляр класса.

        /// <summary>
        /// Размеры областей IO-Link для устройств по изделиям
        /// </summary>
        public Dictionary<string, IODevice.IOLinkSize> IOLinkSizes;
    }
}
