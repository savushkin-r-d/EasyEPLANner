using Eplan.EplApi.DataModel;
using EplanDevice;
using StaticHelper;
using System.Collections.Generic;
using System.Drawing;

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
        /// Функции клемм с ФСА
        /// </summary>
        Dictionary<int, IEplanFunction> ClampFunctions { get; }

        /// <summary>
        /// Смещение входного адресного пространства модуля.
        /// </summary>
        int InOffset { get; }

        /// <summary>
        /// Смещение выходного адресного пространства модуля.
        /// </summary>
        int OutOffset { get; }

        /// <summary>
        /// Номер устройства (из ОУ) прим., 202.
        /// </summary>
        int PhysicalNumber { get; }

        /// <summary>
        /// Аддресное пространство занимаемое модулем
        /// </summary>
        int AddressArea { get; }

        void AssignChannelToDevice(int chN, IODevice dev, IODevice.IOChannel ch);

        /// <summary>
        /// Расчет IO-Link адресов привязанных устройств.
        /// </summary>
        void CalculateIOLinkAdresses();

        string Check(int moduleIndex, string nodeName);

        /// <summary>
        /// Является ли модуль IO-Link 
        /// </summary>
        /// <returns></returns>
        /// <param name="collectOnlyPhoenixContact">Проверять только модули Phoenix Contact</param>
        bool IsIOLink(bool collectOnlyPhoenixContact = false);

        void SaveAsConnectionArray(ref object[,] res, ref int idx, int p,
            Dictionary<string, int> modulesCount, Dictionary<string, Color> modulesColor);

        void SaveASInterfaceConnection(int nodeIdx, int moduleIdx,
            Dictionary<string, object[,]> asInterfaceConnection);

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        string SaveAsLuaTable(string prefix);

        /// <summary>
        /// Функция модуля на ФСА
        /// </summary>
        IEplanFunction Function {  get; }

        /// <summary>
        /// Добавить функцию клеммы 
        /// </summary>
        /// <param name="clampFunction">Функция</param>
        void AddClampFunction(IEplanFunction clampFunction);

        /// <summary>
        /// Очистить привязку клеммы
        /// </summary>
        /// <param name="clamp">Адрес клеммы</param>
        void ClearBind(int clamp);
    }
}
