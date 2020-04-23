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
        /// <param name="DO_count">Количество дискретных выходов</param>
        /// <param name="DI_count">Количество дискретных входов</param>
        /// <param name="AO_count">Количество аналоговых выходов</param>
        /// <param name="AI_count">Количество аналоговых входов</param>
        /// <param name="colorAsStr">Физический цвет модуля</param>
        public static void AddModuleInfo(int number, string name,
            string description, int addressSpaceTypeNum, string typeName,
            string groupName, int[] channelClamps, int[] channelAddressesIn,
            int[] channelAddressesOut, int DO_count, int DI_count,
            int AO_count, int AI_count, string colorAsStr)
        {
            var addressSpaceType = (ADDRESS_SPACE_TYPE)addressSpaceTypeNum;
            Color color = Color.FromName(colorAsStr);

            var moduleInfo = new IOModuleInfo(number, name, description,
                 addressSpaceType, typeName, groupName, channelClamps,
                 channelAddressesIn, channelAddressesOut, DO_count, DI_count,
                 AO_count, AI_count, color);

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
        private IOModuleInfo(int n, string name, string descr,
            ADDRESS_SPACE_TYPE addressSpaceType, string typeName,
            string groupName, int[] channelClamps, int[] clampsAddressIn,
            int[] clampsAddressOut, int DO_count, int DI_count, int AO_count,
            int AI_count, Color color)
        {
            this.n = n;
            this.name = name;
            this.description = descr;

            this.addressSpaceType = addressSpaceType;
            this.typeName = typeName;
            this.groupName = groupName;

            this.channelClamps = channelClamps;
            this.channelAddressesIn = clampsAddressIn;
            this.channelAddressesOut = clampsAddressOut;

            this.DO_cnt = DO_count;
            this.DI_cnt = DI_count;
            this.AO_cnt = AO_count;
            this.AI_cnt = AI_count;

            this.moduleColor = color;
        }

        public object Clone()
        {
            var channelClamps = this.ChannelClamps.Clone() as int[];
            var channelAddressesIn = this.ChannelAddressesIn.Clone() as int[];
            var channelAddressesOut = this.ChannelAddressesOut.Clone() as int[];

            return new IOModuleInfo(this.Number, this.Name, this.Description,
                this.AddressSpaceType, this.TypeName, this.GroupName,
                channelClamps, channelAddressesIn, channelAddressesOut,
                this.DO_count, this.DI_count, this.AO_count, this.AI_count,
                this.ModuleColor);
        }

        /// <summary>
        /// Имя модуля ввода-вывода IO (серия-номер, например: 750-860).
        /// </summary>        
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Номер модуля ввода-вывода IO (например: 860).
        /// </summary>  
        public int Number
        {
            get
            {
                return n;
            }
        }

        /// <summary>
        /// Описание модуля ввода-вывода IO.
        /// </summary>  
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Тип адресного пространства модуля ввода-вывода IO.
        /// </summary>
        public ADDRESS_SPACE_TYPE AddressSpaceType
        {
            get
            {
                return addressSpaceType;
            }
        }

        /// <summary>
        /// Клеммы каналов ввода-вывода.
        /// </summary>
        public int[] ChannelClamps
        {
            get
            {
                return channelClamps;
            }
        }

        /// <summary>
        /// Адреса каналов ввода.
        /// </summary>
        public int[] ChannelAddressesIn
        {
            get
            {
                return channelAddressesIn;
            }
        }

        /// <summary>
        /// Адреса каналов вывода.
        /// </summary>
        public int[] ChannelAddressesOut
        {
            get
            {
                return channelAddressesOut;
            }
        }

        /// <summary>
        /// Количество дискретных выходов. 
        /// </summary>
        public int DO_count
        {
            get
            {
                return DO_cnt;
            }
        }

        /// <summary>
        /// Количество дискретных входов. 
        /// </summary>
        public int DI_count
        {
            get
            {
                return DI_cnt;
            }
        }

        /// <summary>
        /// Количество аналоговых выходов. 
        /// </summary>
        public int AO_count
        {
            get
            {
                return AO_cnt;
            }
        }

        /// <summary>
        /// Количество аналоговых входов. 
        /// </summary>
        public int AI_count
        {
            get
            {
                return AI_cnt;
            }
        }

        /// <summary>
        /// Имя типа (дискретный выход, аналоговый выход, ...).
        /// </summary>
        public string TypeName
        {
            get
            {
                return typeName;
            }
        }

        /// <summary>
        /// Физический цвет модуля
        /// </summary>
        public Color ModuleColor
        {
            get
            {
                return moduleColor;
            }
        }

        /// <summary>
        /// Имя серии модуля ввода-вывода IO (например 750-800).
        /// </summary>        
        public string GroupName
        {
            get
            {
                return groupName;
            }
        }

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

        #region Закрытые поля.
        /// <summary>
        /// Номер.
        /// </summary>
        private int n;

        /// <summary>
        /// Имя.
        /// </summary>
        private string name;

        /// <summary>
        /// Имя типа (дискретный выход, аналоговый выход, ...).
        /// </summary>
        private string typeName;

        /// <summary>
        /// Серия модуля (750-800, 750-1500, ...).
        /// </summary>
        private string groupName;

        /// <summary>
        /// Описание.
        /// </summary>
        private string description;

        /// <summary>
        /// Тип адресного пространства ( DO, DI, AO, AI ).
        /// </summary>
        private ADDRESS_SPACE_TYPE addressSpaceType;

        /// <summary>
        /// Клеммы каналов ввода/вывода.
        /// </summary>
        private int[] channelClamps;

        /// <summary>
        /// Адреса каналов вывода.
        /// </summary>
        private int[] channelAddressesOut;

        /// <summary>
        /// Адреса каналов ввода.
        /// </summary>
        private int[] channelAddressesIn;

        /// <summary>
        /// Количество дискретных выходов. 
        /// </summary>
        private int DO_cnt;

        /// <summary>
        /// Количество дискретных входов.
        /// </summary>
        private int DI_cnt;

        /// <summary>
        /// Количество аналоговых выходов. 
        /// </summary>
        private int AO_cnt;

        /// <summary>
        /// Количество аналоговых входов. 
        /// </summary>
        private int AI_cnt;

        /// <summary>
        /// Цвет.
        /// </summary>
        private Color moduleColor;
        #endregion

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