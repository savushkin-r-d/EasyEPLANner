using System.Collections.Generic;

namespace IO
{
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
        public IOLinkCalculator(List<EplanDevice.IODevice>[] devices,
            List<EplanDevice.IODevice.IOChannel>[] devicesChannels,
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
                case (int)IOManager.IOLinkModules.Wago:
                    CalculateForWago();
                    break;

                case (int)IOManager.IOLinkModules.PhoenixContactStandard:
                    CalculateForPhoenixContact();
                    break;

                case (int)IOManager.IOLinkModules.PhoenixContactSmart:
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
                    offsetIn += devices[clamp][0].IOLinkProperties.SizeIn;
                    offsetOut += devices[clamp][0].IOLinkProperties.SizeOut;
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
                    EplanDevice.IODevice.IOChannel channel =
                        devicesChannels[clamp][0];
                    EplanDevice.IODevice device = devices[clamp][0];
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
                        deviceOffset = device.IOLinkProperties.GetMaxIOLinkSize();
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

        List<EplanDevice.IODevice>[] devices;
        List<EplanDevice.IODevice.IOChannel>[] devicesChannels;
        IOModuleInfo moduleInfo;

        // Сервисные слова.
        int offsetIn = 3;
        int offsetOut = 3;

        public const int MasterDataPXC = 3;
    }
}
