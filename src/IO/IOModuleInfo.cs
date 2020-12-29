using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        /// <param name="DOCount">Количество дискретных выходов</param>
        /// <param name="DICount">Количество дискретных входов</param>
        /// <param name="AOCount">Количество аналоговых выходов</param>
        /// <param name="AICount">Количество аналоговых входов</param>
        /// <param name="colorAsString">Физический цвет модуля</param>
        public static void AddModuleInfo(int number, string name,
            string description, int addressSpaceTypeNum, string typeName,
            string groupName, int[] channelClamps, int[] channelAddressesIn,
            int[] channelAddressesOut, int DOCount, int DICount,
            int AOCount, int AICount, string colorAsString)
        {
            var addressSpaceType = (ADDRESS_SPACE_TYPE)addressSpaceTypeNum;
            Color color = Color.FromName(colorAsString);

            var moduleInfo = new IOModuleInfo(number, name, description,
                 addressSpaceType, typeName, groupName, channelClamps,
                 channelAddressesIn, channelAddressesOut, DOCount, DICount,
                 AOCount, AICount, color);

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

            IOModuleInfo res = modules.Find(x => x.Name == name);
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
        /// <param name="n">Номер модуля ввода-вывода IO </param>
        /// <param name="name">Имя модуля ввода-вывода IO</param>
        /// <param name="description">Описание модуля ввода-вывода IO</param>
        /// <param name="addressSpaceType">Тип адресного пространства</param>
        /// <param name="typeName">Имя типа (дискретный выход и др.)</param>
        /// <param name="groupName">Имя серии (прим., 750-800)</param>
        /// <param name="channelClamps">Клеммы каналов ввода-вывода</param>
        /// <param name="channelAddressesIn">Адреса каналов ввода</param>
        /// <param name="channelAddressesOut">Адреса каналов вывода</param>
        /// <param name="DOCount">Количество дискретных выходов</param>
        /// <param name="DICount">Количество дискретных входов</param>
        /// <param name="AOCount">Количество аналоговых выходов</param>
        /// <param name="AICount">Количество аналоговых входов</param>
        /// <param name="color">Физический цвет модуля</param>
        private IOModuleInfo(int n, string name, string description,
            ADDRESS_SPACE_TYPE addressSpaceType, string typeName,
            string groupName, int[] channelClamps, int[] channelAddressesIn,
            int[] channelAddressesOut, int DOCount, int DICount, int AOCount,
            int AICount, Color color)
        {
            Number = n;
            Name = name;
            Description = description;

            AddressSpaceType = addressSpaceType;
            TypeName = typeName;
            GroupName = groupName;

            ChannelClamps = channelClamps;
            ChannelAddressesIn = channelAddressesIn;
            ChannelAddressesOut = channelAddressesOut;

            DO_count = DOCount;
            DI_count = DICount;
            AO_count = AOCount;
            AI_count = AICount;

            ModuleColor = color;
        }

        public object Clone()
        {
            var channelClamps = ChannelClamps.Clone() as int[];
            var channelAddressesIn = ChannelAddressesIn.Clone() as int[];
            var channelAddressesOut = ChannelAddressesOut.Clone() as int[];

            return new IOModuleInfo(Number, Name, Description,
                AddressSpaceType, TypeName, GroupName, channelClamps,
                channelAddressesIn, channelAddressesOut, DO_count, DI_count,
                AO_count, AI_count, ModuleColor);
        }

        /// <summary>
        /// Имя модуля ввода-вывода IO (серия-номер, например: 750-860).
        /// </summary>        
        public string Name { get; set; }

        /// <summary>
        /// Номер модуля ввода-вывода IO (например: 860).
        /// </summary>  
        public int Number { get; set; }

        /// <summary>
        /// Описание модуля ввода-вывода IO.
        /// </summary>  
        public string Description { get; set; }

        /// <summary>
        /// Тип адресного пространства модуля ввода-вывода IO.
        /// </summary>
        public ADDRESS_SPACE_TYPE AddressSpaceType { get; set; }

        /// <summary>
        /// Клеммы каналов ввода-вывода.
        /// </summary>
        public int[] ChannelClamps { get; set; }

        /// <summary>
        /// Адреса каналов ввода.
        /// </summary>
        public int[] ChannelAddressesIn { get; set; }

        /// <summary>
        /// Адреса каналов вывода.
        /// </summary>
        public int[] ChannelAddressesOut { get; set; }

        /// <summary>
        /// Количество дискретных выходов. 
        /// </summary>
        public int DO_count { get; set; }

        /// <summary>
        /// Количество дискретных входов. 
        /// </summary>
        public int DI_count { get; set; }

        /// <summary>
        /// Количество аналоговых выходов. 
        /// </summary>
        public int AO_count { get; set; }

        /// <summary>
        /// Количество аналоговых входов. 
        /// </summary>
        public int AI_count { get; set; }

        /// <summary>
        /// Имя типа (дискретный выход, аналоговый выход, ...).
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Физический цвет модуля
        /// </summary>
        public Color ModuleColor { get; set; }

        /// <summary>
        /// Имя серии модуля ввода-вывода IO (например 750-800).
        /// </summary>        
        public string GroupName { get; set; }

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
}