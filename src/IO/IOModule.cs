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
                            if (devices[clamp] != null && 
                                devices[clamp].Count == 1)
                            {
                                string devName = "";
                                int devIdx = 0;
                                foreach (Device.IODevice dev in devices[clamp])
                                {
                                    devName += dev.EPlanName +
                                        dev.GetConnectionType() +
                                        $"{dev.GetRange()}: " +
                                        $"{devicesChannels[clamp][devIdx].Name}: " +
                                        $"{dev.Description} " +
                                        $"{devicesChannels[clamp][devIdx].Comment}. " +
                                        $"In: {dev.IOLinkProperties.SizeIn}, " +
                                        $"Out: {dev.IOLinkProperties.SizeOut}";
                                    devName = devName.Replace('\n', ' ');
                                    devIdx++;
                                }
                                res[idx, 3] = devName;

                            }
                            else if (devices[clamp] != null && 
                                devices[clamp].Count > 1)
                            {
                                res[idx, 3] = "IO-Link, более 1 канала. " +
                                    $"In: {devices[clamp][0].IOLinkProperties.SizeIn}, " +
                                    $"Out: {devices[clamp][0].IOLinkProperties.SizeOut}";
                            }
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
}
