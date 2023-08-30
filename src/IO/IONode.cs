using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace IO
{
    /// <summary>
    /// Узел модулей ввода-вывода IO.
    /// </summary>
    public class IONode : IIONode
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="typeStr">Тип.</param>
        /// <param name="n">Номер (также используется как адрес для COM-порта).
        /// </param>
        /// <param name="ip">IP-адрес.</param>
        /// <param name="name">Имя на схеме (А100 и др.).</param>
        public IONode(string typeStr, int n, int nodeNumber , string ip,
            string name, string location)
        {
            this.typeStr = typeStr;
            var nodeinfo = IONodeInfo.GetNodeInfo(typeStr, out _);
            IsCoupler = nodeinfo.IsCoupler;
            type = nodeinfo.Type;
            
            AddressArea = IOAddressArea.GetModulesPerNodeAmount(type);

            this.ip = ip;
            this.n = n;
            this.nodeNumber = nodeNumber;
            this.name = name;
            this.location = location;

            iOModules = new List<IIOModule>();

            DI_count = 0;
            DO_count = 0;
            AI_count = 0;
            AO_count = 0;
        }

        public void SetModule(IIOModule iOModule, int position)
        {
            if (position > AddressArea?.ModulesPerNodeMax)
            {
                throw new Exception($"Модуль \"{iOModule.Name}\" " +
                    $"выходит за диапозон максимального количества модулей для узла \"{name}\". ");
            }

            if (currentAddressArea + iOModule.AddressArea > AddressArea.AddressAreaMax)
            {
                throw new Exception($"Модуль \"{iOModule.Name}\" {iOModule.ArticleName} ({iOModule.AddressArea} byte) " +
                    $"выходит за диапозон адрессного пространства узла \"{name}\" [{currentAddressArea}/{AddressArea.AddressAreaMax}]. ");
            }



            if (iOModules.Count < position)
            {
                for (int i = iOModules.Count; i < position; i++)
                {
                    iOModules.Add(StubIOModule);
                }
            }

            currentAddressArea += iOModule.AddressArea;

            iOModules[position - 1] = iOModule;
        }

        [ExcludeFromCodeCoverage]
        public virtual IIOModule StubIOModule => new IOModule(0, 0, null);

        public IIOModule this[int idx]
        {
            get
            {
                if (idx >= iOModules.Count || idx < 0)
                {
                    return null;
                }
                else
                {
                    return iOModules[idx];
                }
            }
        }

        public string SaveAsLuaTable(string prefix)
        {
            string str = prefix + "{\n";
            str += prefix + "name    = \'" + name + "\',\n";
            str += prefix + "ntype   = " + (int)type + ", " + "--" + typeStr + "\n";
            str += prefix + "n       = " + n + ",\n";
            str += prefix + "IP      = \'" + ip + "\',\n";
            str += prefix + "modules =\n";
            str += prefix + "\t{\n";

            foreach (var module in iOModules)
            {
                if (module != null)
                {
                    str += module.SaveAsLuaTable(prefix + "\t") + ",\n";
                }
                else
                {
                    str += prefix + "\t" + "{}" + ",\n";
                }
            }

            str += prefix + "\t}\n";
            str += prefix + "}";

            return str;
        }

        public void SaveAsConnectionArray(ref object[,] res, ref int idx, 
            Dictionary<string, int> modulesCount,
            Dictionary<string, Color> modulesColor, int nodeIdx, 
            Dictionary<string, object[,]> asInterfaceConnection)
        {
            for (int i = 0; i < iOModules.Count; i++)
            {
                iOModules[i].SaveAsConnectionArray(ref res, ref idx, i + 1, modulesCount, modulesColor);
                if (iOModules[i].Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI ||
                    iOModules[i].Info.AddressSpaceType == IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI)
                {
                    iOModules[i].SaveASInterfaceConnection(nodeIdx, i + 1, asInterfaceConnection);
                }
            }

        }

        /// <summary>
        /// Типы модулей.
        /// </summary>
        public enum TYPES
        {
            T_EMPTY = -1, /// Не определен.
            T_INTERNAL_750_86x, /// Модули в контроллере 750-863.
            T_INTERNAL_750_820x = 2, /// Модули в контроллере PFC200.
            T_ETHERNET = 100, /// Удаленный Ethernet узел.             
            T_PHOENIX_CONTACT = 200, /// Модули в контроллере Phoenix Contact.
            T_PHOENIX_CONTACT_MAIN = 201, /// Контроллер Phoenix Contact
        };

        /// <summary>
        /// Максимальное количество доступных для
        /// подключения к узлу модулей
        /// </summary>
        public class IOAddressArea
        {
            /// <summary> Узлы Phoenix Contact </summary>
            public static readonly IOAddressArea PHOENIX_CONTACT = new IOAddressArea(63, 1482);
            /// <summary> Узлы Wago </summary>
            public static readonly IOAddressArea WAGO = new IOAddressArea(64, 0);

            protected IOAddressArea(int modulesPerNodeMax, int addressAreaMax)
            {
                ModulesPerNodeMax = modulesPerNodeMax;
                AddressAreaMax = addressAreaMax;
            }

            public static IOAddressArea GetModulesPerNodeAmount(TYPES type)
            {
                switch (type)
                {
                    case TYPES.T_INTERNAL_750_86x:
                    case TYPES.T_INTERNAL_750_820x:
                    case TYPES.T_ETHERNET:
                        return WAGO;
                    case TYPES.T_PHOENIX_CONTACT:
                    case TYPES.T_PHOENIX_CONTACT_MAIN:
                        return PHOENIX_CONTACT;
                    default: return null;
                }
            }

            public int ModulesPerNodeMax { get; }
            public int AddressAreaMax { get; }
        }

        public int DI_count { get; set; }

        public int DO_count { get; set; }

        public int AI_count { get; set; }

        public int AO_count { get; set; }

        public List<IIOModule> IOModules
        {
            get
            {
                return iOModules;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string IP
        {
            get
            {
                return ip;
            }
        }

        public TYPES Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Объем адрессного пространства
        /// </summary>
        public IOAddressArea AddressArea { get; }

        public string TypeStr
        {
            get
            {
                return typeStr;
            }
        }

        public int N
        {
            get
            {
                return n;
            }
        }

        public int NodeNumber
        {
            get
            {
                return nodeNumber;
            }
        }

        public string Location
        {
            get
            {
                return location;
            }
        }

        public bool IsCoupler { get; private set; } = false;

        #region Закрытые поля.
        /// <summary>
        /// Модули узла.
        /// </summary>
        private List<IIOModule> iOModules;

        /// <summary>
        /// Тип узла (строка).
        /// </summary>
        private string typeStr;

        /// <summary>
        /// Тип узла.
        /// </summary>
        private TYPES type;

        /// <summary>
        /// IP-адрес.
        /// </summary>
        private string ip;

        /// <summary>
        /// Номер.
        /// </summary>
        private int n;

        /// <summary>
        /// Полный номер узла.
        /// </summary>
        private int nodeNumber;

        /// <summary>
        /// Имя узла (прим., А100).
        /// </summary>
        private string name;

        /// <summary>
        /// Местоположение узла (прим., +MCC1)
        /// </summary>
        private string location;

        /// <summary>
        /// Текущее занимаемое модулями адресное пространство
        /// </summary>
        private int currentAddressArea = 0;
        #endregion
    }
}
