using System.Collections.Generic;

namespace IO
{
    public interface IIOModule
    {
        /// <summary>
        /// Описание модуля.
        /// </summary>
        IOModuleInfo Info { get; }

        /// <summary>
        /// Имя модуля на схеме ПЛК (А***).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Имя изделия в модуле.
        /// </summary>
        string ArticleName { get; }

        /// <summary>
        /// Привязанные устройства.
        /// </summary>
        List<EplanDevice.IIODevice>[] Devices { get; }

        /// <summary>
        /// Привязанные каналы.
        /// </summary>
        List<EplanDevice.IODevice.IIOChannel>[] DevicesChannels { get; }

        /// <summary>
        /// Является ли модуль IO-Link 
        /// </summary>
        /// <returns></returns>
        /// <param name="collectOnlyPhoenixContact">Проверять только модули Phoenix Contact</param>
        bool IsIOLink(bool collectOnlyPhoenixContact = false);
    }
}
