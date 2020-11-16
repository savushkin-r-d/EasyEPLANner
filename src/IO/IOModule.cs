using System;
using System.Collections.Generic;
using System.Drawing;

namespace IO
{
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
                if (isIOLink())
                {
                    var dev = devices[clamp][0];
                    var devChannel = devicesChannels[clamp][0];

                    if (devChannel.Name == "DI" || devChannel.Name == "DO")
                    {
                        devName = dev.EPlanName +
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
                            devName = dev.EPlanName +
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
                foreach (Device.IODevice dev in devices[clamp])
                {
                    devName += dev.EPlanName + 
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

            int wago = (int)IOManager.IOLinkModules.Wago;
            int phoenixContactStandard = (int)IOManager.IOLinkModules
                .PhoenixContactStandard;
            int phoenixContactSmart = (int)IOManager.IOLinkModules
                .PhoenixContactSmart;

            if (Info?.Number == wago ||
                Info?.Number == phoenixContactStandard ||
                Info?.Number == phoenixContactSmart)
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
}
