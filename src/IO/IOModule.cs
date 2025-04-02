using StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IO
{
    /// <summary>
    /// Модуль ввода-вывода IO.
    /// </summary>
    public class IOModule : IIOModule
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
        /// <param name="articleName">Имя изделия модуля ввода-вывода</param>
        /// <param name="function">Eplan функция модуля.</param>
        public IOModule(int inAddressSpaceOffset, int outAddressSpaceOffset,
            IOModuleInfo info, int physicalNumber, string articleName,
            IEplanFunction function)
        {
            this.inAddressSpaceOffset = inAddressSpaceOffset;
            this.outAddressSpaceOffset = outAddressSpaceOffset;
            this.info = info;
            this.physicalNumber = physicalNumber;
            this.function = function;
            this.articleName = articleName;

            devicesChannels = new List<EplanDevice.IODevice.IIOChannel>[80];
            devices = new List<EplanDevice.IIODevice>[80];
        }

        public IOModule(int inAddressSpaceOffset, int outAddressSpaceOffset,
            IOModuleInfo info) : this(inAddressSpaceOffset,
                outAddressSpaceOffset, info, 0, string.Empty, null)
        {
            // Делегировано в конструктор с 5 параметрами.
        }

        public void AssignChannelToDevice(int chN, EplanDevice.IODevice dev,
            EplanDevice.IODevice.IOChannel ch)
        {
            if (devices.GetLength(0) <= chN)
            {
                System.Windows.Forms.MessageBox.Show("Error!");
            }

            if (devices[chN] == null)
            {
                devices[chN] = new List<EplanDevice.IIODevice>();
            }
            if (devicesChannels[chN] == null)
            {
                devicesChannels[chN] = new List<EplanDevice.IODevice.IIOChannel>();
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
                res = String.Format("{0} {{ {1,7} }},        --{2,7}", 
                    prefix, info.Number, info.Name);
            }
            else
            {
                res = prefix + "{ ? },";
            }

            return res;
        }

        public void SaveAsConnectionArray(ref object[,] res, ref int idx, int p, 
            Dictionary<string, int> modulesCount, 
            Dictionary<string, Color> modulesColor)
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
                foreach(int clamp in Info.ChannelClamps)
                {
                    bool isIOLinkDevice = false;
                    res[idx, 0] = p;
                    res[idx, 1] = moduleName;
                    res[idx, 2] = clamp.ToString();
                    res[idx, 3] = GenerateClampStringForExcel(clamp, 
                        ref isIOLinkDevice);

                    if(isIOLinkDevice)
                    {
                        // Для расчета IO-Link округляем до целого, кроме 0
                        // Для настройки - оставляем как есть
                        var dev = devices[clamp][0];
                        float sizeIn = dev.IOLinkProperties.SizeInFromFile;
                        float sizeOut = dev.IOLinkProperties.SizeOutFromFile;
                        
                        const int WordToBitMultiplier = 16;
                        int sizeInBits = Convert.ToInt32(
                            (sizeIn * WordToBitMultiplier));
                        int sizeOutBits = Convert.ToInt32(
                            (sizeOut * WordToBitMultiplier));

                        res[idx, 4] = sizeInBits;
                        res[idx, 5] = sizeOutBits;
                    }

                    idx++;
                }
            }
            else
            {
                res[idx, 0] = p;
                res[idx, 1] = moduleName;
                idx++;
            }
        }

        /// <summary>
        /// Генерация строки с описанием привязанного устройств(-а) к клемме
        /// </summary>
        /// <returns></returns>
        private string GenerateClampStringForExcel(int clamp, 
            ref bool isIOLinkDevice)
        {
            string devName = "";

            bool devicesNotFound = (devices[clamp] == null || 
                devices[clamp].Count == 0);
            if (devicesNotFound)
            {
                return devName;
            }

            bool isASInterfaceOrIOLink = (Info.AddressSpaceType == 
                IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI ||
                Info.AddressSpaceType == 
                IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI);
            if (isASInterfaceOrIOLink)
            {
                if (IsIOLink())
                {
                    var dev = devices[clamp][0];
                    var devChannel = devicesChannels[clamp][0];

                    if (devChannel.Name == "DI" || devChannel.Name == "DO")
                    {
                        devName = dev.EplanName +
                            dev.GetConnectionType() +
                            $"{dev.GetRange()}: " +
                            $"{devChannel.Name}: " +
                            $"{dev.Description} " +
                            $"{devChannel.Comment}";
                    }
                    else
                    {
                        bool isIOLinkVale = devices[clamp].Count == 2 &&
                            devices[clamp][0].Name == devices[clamp][1].Name;
                        if (devices[clamp].Count == 1 || isIOLinkVale)
                        {
                            devName = dev.EplanName +
                                dev.GetConnectionType() +
                                $"{dev.GetRange()}: " +
                                $"{devChannel.Name}: " +
                                $"{dev.Description} " +
                                $"{devChannel.Comment}";
                            isIOLinkDevice = true;
                        }
                        else
                        {
                            devName = "IO-Link, более 1 канала.";
                            isIOLinkDevice = true;
                        }
                    }
                }
                else
                {
                    devName = "AS interface";
                }
            }
            else
            {
                int devIdx = 0;
                foreach (EplanDevice.IODevice dev in devices[clamp])
                {
                    devName += dev.EplanName + 
                        dev.GetConnectionType() + 
                        dev.GetRange() + ": " +
                        devicesChannels[clamp][devIdx].Name + ": " + 
                        dev.Description + " " +
                        devicesChannels[clamp][devIdx].Comment;
                    devIdx++;
                }
            }

            devName = devName.Replace('\n', ' ');
            return devName;
        }

        public void SaveASInterfaceConnection(int nodeIdx, int moduleIdx, 
            Dictionary<string, object[,]> asInterfaceConnection)
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
                        if (devices[clamp] != null && devices[clamp].Count > 1)
                        {
                            int deviceCounter = 0;
                            foreach (EplanDevice.IODevice dev in devices[clamp])
                            {
                                asConnection[devIdx, 0] = clamp.ToString() + "(" + (deviceCounter + 1).ToString() + ")";
                                string devDescription = dev.EplanName + dev.GetConnectionType() + dev.GetRange() + ": " +
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

        public void CalculateIOLinkAdresses()
        {
            IOLinkCalculator calculator = new IOLinkCalculator(devices,
                devicesChannels, Info);
            calculator.Calculate();
        }

        public string Check(int moduleIndex, string nodeName)
        {
            string errors = string.Empty;

            if (Info == null)
            {
                errors += $"В узле {nodeName} не опознан или " +
                    $"отсутствует модуль {moduleIndex + 1}.\n";
            }
            else
            {
                if(IsIOLink())
                {
                    errors += CheckIOLinkSize();
                }
            }

            return errors;
        }

        /// <summary>
        /// Проверка выхода за предел размерности области ввода-вывода для
        /// модуля.
        /// </summary>
        /// <returns>Сообщение об ошибке</returns>
        private string CheckIOLinkSize()
        {
            string errors = string.Empty;

            int devicesSize = 0;
            int clampsCount = devices.Length;
            for (int clamp = 0; clamp < clampsCount; clamp++)
            {
                var devicesOnClamp = devices[clamp];
                if (devicesOnClamp == null)
                {
                    continue;
                }

                if(devicesOnClamp[0].DeviceType == EplanDevice.DeviceType.Y ||
                    devicesOnClamp[0].DeviceType == EplanDevice.DeviceType.DEV_VTUG)
                {
                    devicesSize += devicesOnClamp[0].IOLinkProperties
                        .GetMaxIOLinkSize();
                }
                else
                {
                    devicesSize += CalculateDevicesSize(clamp,
                        devicesOnClamp);
                }
            }

            if(devicesSize > AllowedMaxIOLinkSize)
            {
                int differene = devicesSize - AllowedMaxIOLinkSize;
                errors += $"На модуле ввода-вывода A{PhysicalNumber} " +
                    $"превышен размер области ввода-вывода на " +
                    $"{differene} слов(-о/-а).\n";
            }

            return errors;
        }

        /// <summary>
        /// Расчет размерности устройств ввода-вывода на клемме
        /// модуля ввода-вывода.
        /// </summary>
        /// <param name="moduleClamp">Номер клеммы</param>
        /// <param name="devicesOnClamp">Устройства на клемме</param>
        /// <returns></returns>
        private int CalculateDevicesSize(int moduleClamp, 
            List<EplanDevice.IIODevice> devicesOnClamp)
        {
            int size = 0;
            for (int dev = 0; dev < devicesOnClamp.Count; dev++)
            {
                // AI достаточно для определения размера IO-Link.
                var channel = devicesChannels[moduleClamp][dev];
                if (channel.FullModule == PhysicalNumber &&
                    channel.Name == "AI")
                {
                    size += devicesOnClamp[dev]
                        .IOLinkProperties.GetMaxIOLinkSize();
                }
            }

            return size;
        }

        public IOModuleInfo Info
        {
            get
            {
                return info;
            }
        }

        public int InOffset
        {
            get
            {
                return inAddressSpaceOffset;
            }
        }

        public int OutOffset
        {
            get
            {
                return outAddressSpaceOffset;
            }
        }

        public int PhysicalNumber
        {
            get
            {
                return physicalNumber;
            }
        }

        public IEplanFunction Function => function;

        public string Name
        {
            get
            {
                return $"A{physicalNumber}";
            }
        }

        public string ArticleName
        {
            get
            {
                return articleName;
            }
        }

        public bool IsIOLink(bool collectOnlyPhoenixContact = false)
        {
            bool isIOLink = false;

            int wago = (int)IOManager.IOLinkModules.Wago;
            int phoenixContactStandard = (int)IOManager.IOLinkModules
                .PhoenixContactStandard;
            int phoenixContactSmart = (int)IOManager.IOLinkModules
                .PhoenixContactSmart;

            bool isPhoenixContact = Info?.Number == phoenixContactStandard ||
                Info?.Number == phoenixContactSmart;
            bool isWago = Info?.Number == wago;
            if (collectOnlyPhoenixContact)
            {
                if (isPhoenixContact) isIOLink = true;
            }
            else
            {
                if (isWago || isPhoenixContact) isIOLink = true;
            }

            return isIOLink;
        }

        public void AddClampFunction(IEplanFunction clampFunction)
        {
            ClampFunctions[clampFunction.ClampNumber] = clampFunction;
        }

        public void ClearBind(int clamp)
        {
            Devices[clamp] = null;
            DevicesChannels[clamp] = null;
        }

        /// <summary>
        /// Доступный максимальный размер IO-Link области в словах.
        /// </summary>
        /// <returns></returns>
        public int AllowedMaxIOLinkSize
        {
            get
            {
                if (!IsIOLink())
                {
                    return 0;
                }

                switch (Info?.Number)
                {
                    case (int)IOManager.IOLinkModules.Wago:
                        return Info.AICount;

                    case (int)IOManager.IOLinkModules.PhoenixContactSmart:
                    case (int)IOManager.IOLinkModules.PhoenixContactStandard:
                        return Info.AICount - IOLinkCalculator.MasterDataPXC;

                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Адрессное пространство занимаемое модулем
        /// </summary>
        public int AddressArea => Info?.LocalbusData ?? 0;

        public List<EplanDevice.IIODevice>[] Devices
        {
            get
            {
                return devices;
            }
        }

        public List<EplanDevice.IODevice.IIOChannel>[] DevicesChannels
        {
            get
            {
                return devicesChannels;
            }
        }

        public Dictionary<int, IEplanFunction> ClampFunctions { get; private set; } = [];

        #region Закрытые поля.
        private List<EplanDevice.IIODevice>[] devices;
        private List<EplanDevice.IODevice.IIOChannel>[] devicesChannels;
        private int inAddressSpaceOffset;
        private int outAddressSpaceOffset;
        private IOModuleInfo info;
        private int physicalNumber;
        IEplanFunction function;
        private string articleName;
        #endregion
    }
}
