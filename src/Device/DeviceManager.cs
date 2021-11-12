using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using EasyEPlanner;

/// <summary>
/// Пространство имен технологических устройств проекта (клапана, насосы...).
/// </summary>
namespace Device
{
    public interface IDeviceManager
    {
        /// <summary>
        /// Получить индекс устройства по его имени из общего пула устройств.
        /// </summary>
        /// <param name="devName">Имя устройства</param>
        /// <returns></returns>
        int GetDeviceIndex(string devName);

        /// <summary>
        /// Возвращает устройство по его имени в Eplan
        /// </summary>
        /// <param name="devName">Имя устройств в Eplan</param>
        /// <returns></returns>
        IDevice GetDeviceByEplanName(string devName);

        /// <summary>
        /// Получить устройство по индексу
        /// </summary>
        /// <param name="index">Индекс устройства</param>
        /// <returns></returns>
        IDevice GetDeviceByIndex(int index);

        /// <summary>
        /// Устройства проекта
        /// </summary>
        List<IODevice> Devices { get; }

        /// <summary>
        /// Генерация тегов устройств для экспорта в базу каналов.
        /// </summary>
        /// <param name="rootNode">Корневой узел</param>
        void GetObjectForXML(TreeNode rootNode);
    }

    /// <summary>
    /// Менеджер описания устройств для проекта.
    /// </summary>
    public class DeviceManager : IDeviceManager
    {
        public void GetObjectForXML(TreeNode rootNode)
        {
            foreach (IODevice dev in devices)
            {
                if (dev != null)
                {
                    dev.GenerateDeviceTags(rootNode);
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
            var res = string.Empty;

            foreach (var dev in devices)
            {
                res += dev.Check();
            }

            long startingIP = ProjectConfiguration.GetInstance()
                .StartingIPInterval;
            long endingIP = ProjectConfiguration.GetInstance()
                .EndingIPInterval;
            if (startingIP != 0 && endingIP != 0)
            {
                res += CheckDevicesIP(startingIP, endingIP);
            }

            res += CheckPIDInputAndOutputProperties();

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
            string ipProperty = IODevice.Property.IP;

            var devicesWithIPProperty = Devices
                .Where(x => x.Properties.ContainsKey(ipProperty) &&
                x.Properties[ipProperty] != null).ToArray();
            foreach (var device in devicesWithIPProperty)
            {
                string IPstr = Regex.Match(device.Properties[ipProperty]
                    .ToString(), StaticHelper.CommonConst.IPAddressPattern)
                    .Value;
                if (IPstr == string.Empty)
                {
                    continue;
                }

                var devicesWithEqualsIP = devicesWithIPProperty
                    .Where(x => x.Properties[ipProperty].ToString() ==
                    device.Properties[ipProperty].ToString()).ToArray();
                if (devicesWithEqualsIP.Length > 1)
                {
                    var equalsDevicesNames = devicesWithEqualsIP
                        .Select(x => x.EplanName).ToArray();
                    string error = $"IP-адреса устройств " +
                        $"{string.Join(",", equalsDevicesNames)} совпадают.\n";
                    errors.Add(error);
                }

                long devIP = StaticHelper.IPConverter.ConvertIPStrToLong(IPstr);
                if (devIP - startingIP < 0 || endingIP - devIP < 0)
                {
                    string error = $"IP-адрес устройства {device.EplanName} " +
                    $"вышел за диапазон.\n";
                    errors.Add(error);
                }
            }

            errors = errors.Distinct().ToList();
            return string.Concat(errors);
        }

        /// <summary>
        /// Проверить входы и выходы ПИД-регуляторов
        /// </summary>
        /// <returns></returns>
        private string CheckPIDInputAndOutputProperties()
        {
            string res = string.Empty;
            string cap = StaticHelper.CommonConst.Cap;

            var PIDs = Devices.Where(x => x.DeviceType == DeviceType.C);
            foreach (var dev in PIDs)
            {
                foreach (var property in dev.Properties)
                {
                    object value = property.Value;
                    if (value != null)
                    {
                        var devInPropery = GetDevice(value.ToString());
                        if (devInPropery.Description == cap)
                        {
                            res += $"Задано несуществующее устройство для " +
                                $"ПИД-регулятора {dev.Name}, свойство " +
                                $"{property.Key}.\n";
                        }

                        bool allowedDevices =
                            devInPropery.DeviceType != DeviceType.AO &&
                            devInPropery.DeviceType != DeviceType.VC &&
                            devInPropery.DeviceType != DeviceType.M &&
                            devInPropery.DeviceType != DeviceType.C;
                        if (property.Key == dev.Properties.Keys.Last() &&
                            allowedDevices)
                        {
                            res += $"В выходе {property.Key} ПИД-регулятора" +
                                $" {dev.Name} задано некорректное " +
                                $"устройство. Нужно указать AO, VC, M или " +
                                $"другой ПИД-регулятор.\n";
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Сортировка устройств  проекта.
        /// </summary>
        public void Sort()
        {
            devices.Sort();
        }

        public IDevice GetDeviceByEplanName(string devName)
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
            string eplanName;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            // Устройства нет, вернет состояние заглушки.
            CheckDeviceName(devName, out name, out eplanName, out objectName,
                out objectNumber, out deviceType, out deviceNumber);
            return new IODevice(name, eplanName, StaticHelper.CommonConst.Cap,
                deviceType, deviceNumber, objectName, objectNumber);
        }

        /// <summary>
        /// Получение устройства по его имени (ОУ) из глобального списка.
        /// </summary>
        /// <param name="devName">Имя устройства.</param>
        /// <returns>Устройство с заданными именем или устройство-заглушка.</returns>
        public IODevice GetDevice(string devName)
        {
            string name;
            string eplanName;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            CheckDeviceName(devName, out name, out eplanName, out objectName,
                out objectNumber, out deviceType, out deviceNumber);

            IODevice devStub = new IODevice(name, eplanName,
                StaticHelper.CommonConst.Cap, deviceType, deviceNumber,
                objectName, objectNumber);

            int resDevN = devices.BinarySearch(devStub);

            if (resDevN >= 0)
            {
                return devices[resDevN];
            }

            return devStub;
        }

        public IDevice GetDeviceByIndex(int index)
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

        public int GetDeviceIndex(string devName)
        {
            string name;
            string eplanName;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            CheckDeviceName(devName, out name, out eplanName, out objectName,
                out objectNumber, out deviceType, out deviceNumber);
            IODevice devStub = new IODevice(name, eplanName,
                StaticHelper.CommonConst.Cap, deviceType, deviceNumber,
                objectName, objectNumber);

            int resDevN = devices.IndexOf(devStub);

            return resDevN;
        }


        /// <summary>
        /// Шаблон для разбора имени для привязки устройств
        /// "==BR2=HUT1+KOAG5-V2"
        /// "+KOAG5-V2"
        /// </summary>
        public static readonly string BINDING_DEVICES_DESCRIPTION_PATTERN =
            @"(?<name>([+-])(?<object_main>([A-Z_]*?)(\d*?))(?<object>[A-Z_]*?)(?<object_n>\d*)(-+)(?<type>[A-Z_]+)(?<n>\d+))(\s+|$)";

        /// <summary>
        /// Шаблоны для разбора имени в остальных случаях: 
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
        /// <param name="devName">Имя устройства (A1V1).</param>
        /// <param name="eplanName">Имя устройства Eplan (+A1-V1)</param>
        /// <param name="deviceNumber">Номер устройства.</param>
        /// <param name="deviceType">Тип устройства.</param>
        /// <param name="fullDevName">Полное имя устройства.</param>
        /// <param name="objectName">Имя объекта.</param>
        /// <param name="objectNumber">Номер объекта.</param>
        public bool CheckDeviceName(string fullDevName, out string devName,
            out string eplanName, out string objectName, out int objectNumber,
            out string deviceType, out int deviceNumber)
        {
            bool res = false;

            eplanName = string.Empty;
            objectName = string.Empty;
            objectNumber = 0;
            deviceType = string.Empty;
            deviceNumber = 0;
            devName = fullDevName;

            Match match = Regex.Match(fullDevName, DESCRIPTION_PATTERN,
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string devType = match.Groups["type"].Value;
                bool isPID = IsPIDControl(devType);
                if (isPID)
                {
                    devType = DeviceType.C.ToString();
                }

                bool devTypeExist = allowedDevTypes.Contains(devType);
                if (devTypeExist)
                {
                    objectName = match.Groups["object_main"].Value +
                            match.Groups["object"].Value;
                    if (match.Groups["object_n"].Value != string.Empty)
                    {
                        objectNumber = Convert
                            .ToInt32(match.Groups["object_n"].Value);
                    }

                    deviceType = devType;

                    if (match.Groups["n"].Value != string.Empty)
                    {
                        deviceNumber = Convert
                            .ToInt32(match.Groups["n"].Value);
                    }

                    devName = match.Groups["object_main"].Value +
                        match.Groups["object"].Value +
                        match.Groups["object_n"].Value +
                        match.Groups["type"].Value +
                        match.Groups["n"].Value;

                    eplanName = "+" +
                        match.Groups["object_main"].Value +
                        match.Groups["object"].Value +
                        match.Groups["object_n"].Value + "-" +
                        match.Groups["type"].Value +
                        match.Groups["n"].Value;

                    res = true;
                }
            }

            return res;
        }

        private List<string> allowedDevTypes = new List<string>()
        {
            "V",
            "VC",
            "M",
            "N",
            "LS",
            "TE",
            "GS",
            "FS",
            "FQT",
            "AO",
            "LT",
            "OS",
            "DI",
            "UPR",
            "DO",
            "QT",
            "AI",
            "HA",
            "HL",
            "SB",
            "WT",
            "PT",
            "F",
            "Y",
            "DEV_VTUG", // Совместимость со старыми проектами
            "C",
            "HLA",
            "CAM"
        };

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
            errStr = string.Empty;
            IODevice dev = null;

            string name;
            string eplanName;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;


            CheckDeviceName(devName, out name, out eplanName, out objectName,
                out objectNumber, out deviceType, out deviceNumber);
            bool isPID = IsPIDControl(deviceType);
            if (isPID)
            {
                deviceType = DeviceType.C.ToString();
            }

            // Если изделия нет или пустое, то оставляем пустое
            if (articleName == string.Empty || articleName == null)
            {
                articleName = string.Empty;
            }

            switch (deviceType)
            {
                case "V":
                    dev = new V(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "VC":
                    dev = new VC(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "M":
                case "N":
                    dev = new M(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "LS":
                    dev = new LS(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "TE":
                    dev = new TE(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "GS":
                    dev = new GS(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "FS":
                    dev = new FS(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "FQT":
                    dev = new FQT(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "AO":
                    dev = new AO(name, eplanName, description, deviceNumber,
                        objectName, objectNumber);
                    break;

                case "LT":
                    dev = new LT(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "OS":
                case "DI":
                    dev = new DI(name, eplanName, description, deviceNumber,
                        objectName, objectNumber);
                    break;

                case "UPR":
                case "DO":
                    dev = new DO(name, eplanName, description, deviceNumber,
                        objectName, objectNumber);
                    break;

                case "QT":
                    dev = new QT(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "AI":
                    dev = new AI(name, eplanName, description, deviceNumber,
                        objectName, objectNumber);
                    break;

                case "HA":
                    dev = new HA(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "HL":
                    dev = new HL(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "SB":
                    dev = new SB(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "WT":
                    dev = new WT(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "PT":
                    dev = new PT(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "Y":
                    dev = new Y(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;
                case "DEV_VTUG": // Совместимость со старыми проектами
                    dev = new DEV_VTUG(name, eplanName, description,
                        deviceNumber, objectName, objectNumber, articleName);
                    break;

                case "F":
                    dev = new F(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "C":
                    dev = new C(name, eplanName, description, deviceNumber,
                        objectName, objectNumber);
                    break;

                case "HLA":
                    dev = new HLA(name, eplanName, description, deviceNumber,
                        objectName, objectNumber, articleName);
                    break;

                case "CAM":
                    dev = new CAM(name, eplanName, description, deviceNumber,
                        objectName, objectNumber);
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
        public string SaveAsLuaTableForMainIO(string prefix)
        {
            string res = "------------------------------------------------------------------------------\n";
            res += "--Устройства\n";
            res += prefix + "devices =\n";
            res += prefix + "\t{\n";

            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                dev.SortChannels();
                res += dev.SaveAsLuaTable(prefix + "\t\t") + ",\n\n";
            }

            res += prefix + "\t}\n";
            res = res.Replace("\t", "    ");

            return res;
        }

        /// <summary>
        /// Сохранение устройств в виде скрипта Lua. Для последующего доступа
        /// по имени или индексу в main.devices.lua.
        /// </summary>
        public string SaveAsLuaTableForMainDevices()
        {
            string str = "system = system or {}\n";
            str += "system.init_dev_names = function()\n";
            str += GetDevicesForMainDevices();
            str += "end\n";
            str = str.Replace("\t", "    ");

            return str;
        }

        /// <summary>
        /// Генерирует тело скрипта Lua для файла main.devices.lua
        /// </summary>
        /// <returns></returns>
        private string GetDevicesForMainDevices()
        {
            var deviceNameStr = string.Empty;
            var deviceIndexesStr = string.Empty;
            for (int i = 0; i < devices.Count; i++)
            {
                var dev = devices[i];
                bool skipDevice = dev.DeviceType == DeviceType.Y ||
                   dev.DeviceType == DeviceType.DEV_VTUG;
                if (skipDevice) continue;

                deviceNameStr += GenerateDevicesNameForMainDevices(dev);
                deviceIndexesStr += GenerateDevicesIndexForMainDevices(i, dev);
            }

            return string.Join(string.Empty, deviceNameStr, "\n", deviceIndexesStr);
        }

        /// <summary>
        /// Генерирует строку описания имени устройства в общем списке
        /// устройств для main.devices.lua
        /// </summary>
        /// <param name="dev">Устройство</param>
        /// <returns></returns>
        private string GenerateDevicesNameForMainDevices(IODevice dev)
        {
            string temp = string.Empty;
            bool hasObjectNumber = dev.ObjectNumber > 0;
            bool noObjectName = dev.ObjectName == string.Empty;
            if (hasObjectNumber && noObjectName)
            {
                temp += "\t_";
            }
            else
            {
                temp += "\t";
            }

            temp += $"{dev.Name} = {dev.DeviceType}(\'{dev.Name}\')\n";
            return temp;
        }

        /// <summary>
        /// Генерирует строку для описания индекса устройства в общем списке
        /// устройств для main.devices.lua
        /// </summary>
        /// <param name="id">Индекс устройства</param>
        /// <param name="device">Устройство</param>
        /// <returns></returns>
        private string GenerateDevicesIndexForMainDevices(int id,
            IODevice device)
        {
            return "\t__" + device.Name + " = DEVICE( " + id + " )\n";
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
                    errorsBuffer += $"В устройстве {device.EplanName} " +
                        $"отсутствует R_AS_NUMBER.\n ";
                }
                else
                {
                    var ASNumber = new int();
                    bool isNumber = int.TryParse(parameter, out ASNumber);
                    if (isNumber == false)
                    {
                        errorsBuffer += $"В устройстве " +
                            $"{device.EplanName} некорректно задан параметр " +
                            $"R_AS_NUMBER.\n ";
                    }
                    if (isNumber == true && ASNumber < 1 && ASNumber > 62)
                    {
                        errorsBuffer += $"В устройстве " +
                            $"{device.EplanName} некорректно задан диапазон " +
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
        /// Тип устройства является ПИД-ом или нет
        /// </summary>
        /// <param name="devices">Тип устройства</param>
        /// <returns></returns>
        private bool IsPIDControl(string type)
        {
            bool isPID = false;
            const int maxTypeLength = 4;

            if (type.Length <= 1)
            {
                return isPID;
            }

            const int firstChar = 0;
            bool noPID =
                (type[firstChar] == Convert.ToChar($"{DeviceType.V}") ||
                type[firstChar] == Convert.ToChar($"{DeviceType.C}") ||
                type.Contains($"{DeviceType.C}") == false);
            if (noPID)
            {
                return isPID;
            }

            for (int i = 1; i < maxTypeLength; i++)
            {
                if (type[i] == Convert.ToChar(DeviceType.C.ToString()))
                {
                    isPID = true;
                    break;
                }
            }

            return isPID;
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

        private static IODevice cap =
            new IODevice(StaticHelper.CommonConst.Cap, string.Empty,
                StaticHelper.CommonConst.Cap, 0, string.Empty, 0);
        private List<IODevice> devices;       ///Устройства проекта.     
        private static DeviceManager instance;  ///Экземпляр класса.

        /// <summary>
        /// Размеры областей IO-Link для устройств по изделиям
        /// </summary>
        public Dictionary<string, IODevice.IOLinkSize> IOLinkSizes;
    }
}