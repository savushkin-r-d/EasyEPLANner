using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Windows.Forms;
using IO;
using System.Linq;
using StaticHelper;

/// <summary>
/// Пространство имен технологических устройств проекта (клапана, насосы...).
/// </summary>
namespace Device
{

    /// Типы устройств.
    public enum DeviceType
    {
        NONE = -1,      ///< Тип не определен.

        V = 0,   ///< Клапан. 
        VC,      ///< Управляемый клапан. 
        M,       ///< Двигатель.
        LS,      ///< Уровень (есть/нет).
        TE,      ///< Температура.        
        FS,      ///< Расход (есть/нет).
        GS,      ///< Датчик положения. 
        FQT,     ///< Счетчик.        
        LT,      ///< Уровень (значение).        
        QT,      ///< Концентрация.

        HA,      ///< Звуковая сигнализация.
        HL,      ///< Световая сигнализация.
        SB,      ///< Кнопка.
        DI,      ///< Дискретный входной сигнал.
        DO,      ///< Дискретный выходной сигнал.
        AI,      ///< Аналоговый входной сигнал.
        AO,      ///< Аналоговый выходной сигнал.
        WT,      ///< Датчик веса.
        PT,      ///< Датчик давления.

        Y,       ///< Пневмоостров Festo
        DEV_VTUG,///< Пневмоостров Festo (совместимость со старыми проектами).
    };

    /// Подтипы устройств.
    public enum DeviceSubType
    {
        NONE = -1,      ///< Подтип не определен.

        //V
        V_DO1 = 1,        ///< Клапан с одним каналом управления.
        V_DO2,            ///< Клапан с двумя каналами управления.
        V_DO1_DI1_FB_OFF, ///< Клапан с одним каналом управления и одной обратной связью (выключенное состояние).
        V_DO1_DI1_FB_ON,  ///< Клапан с одним каналом управления и одной обратной связью (включенное состояние).
        V_DO1_DI2,        ///< Клапан с одним каналом управления и двумя обратными связями.
        V_DO2_DI2,        ///< Клапан с двумя каналами управления и двумя обратными связями.
        V_MIXPROOF,       ///< Клапан микспруф.
        V_AS_MIXPROOF,    ///< Клапан с двумя каналами управления и двумя обратными связями с AS интерфейсом (микспруф).
        V_BOTTOM_MIXPROOF,///< Клапан с промывкой и двумя обратными связями (донный).
        V_AS_DO1_DI2,     ///< Клапан с одним каналом управления и двумя обратными связями с AS интерфейсом.
        V_DO2_DI2_BISTABLE,///< Клапан с двумя каналами управления и двумя обратными связями бистабильный.

        V_IOLINK_VTUG_DO1, ///< IO-Link VTUG клапан с одним каналом управления.

        V_IOLINK_VTUG_DO1_FB_OFF, ///< IO-Link VTUG клапан с одним каналом управления и одной обратной связью (выключенное состояние).
        V_IOLINK_VTUG_DO1_FB_ON,  ///< IO-Link VTUG клапан с одним каналом управления и одной обратной связью (включенное состояние).
        V_IOLINK_MIXPROOF, ///< Клапан микспруф с IO-Link
        V_IOLINK_DO1_DI2,  ///< Клапан с одним каналом управления и двумя обратными связями с IO-Link интерфейсом. 

        //LS
        LS_MIN = 1,     ///< Подключение по схеме минимум.
        LS_MAX,         ///< Подключение по схеме максимум.

        LS_IOLINK_MIN,  ///< IO-Link уровень. Подключение по схеме минимум.
        LS_IOLINK_MAX,  ///< IO-Link уровень. Подключение по схеме максимум.

        LS_VIRT,        ///< Виртуальный датчик уровня.

        //M,  
        M = 1,          ///< Мотор без управления частотой вращения.
        M_FREQ,         ///< Мотор с управлением частотой вращения.
        M_REV,          ///< Мотор с реверсом без управления частотой вращения. Реверс включается совместно.
        M_REV_FREQ,     ///< Мотор с реверсом с управлением частотой вращения. Реверс включается совместно.
        M_REV_2,        ///< Мотор с реверсом без управления частотой вращения. Реверс включается отдельно.
        M_REV_FREQ_2,   ///< Мотор с реверсом с управлением частотой вращения. Реверс включается отдельно.

        /// Мотор с реверсом. Реверс включается отдельно. Отдельный сигнал
        /// аварии.   
        M_REV_2_ERROR,
        /// Мотор с реверсом с управлением частотой вращения. Реверс
        /// включается отдельно. Отдельный сигнал аварии.   
        M_REV_FREQ_2_ERROR,
        /// <summary>
        /// Мотор, управляемый частотником Altivar. Связь с частотником по Ethernet.
        /// Реверс и аварии опциональны.
        /// </summary>
        M_ATV,

        FQT = 1,        ///< Счетчик.
        FQT_F,          ///< Счетчик с расходом.
        FQT_F_OK,       ///< Счетчик c расходом c диагностикой.
        FQT_VIRT,       ///Виртуальный счётчик (без привязки к модулям).

        //QT
        QT = 1,         ///< Концентратомер.
        QT_OK,          ///< Концентратомер c диагностикой.
        QT_IOLINK,       ///< IO-Link концентратомер.

        //DO
        DO = 1,         ///Дискретный выход с привязкой к модулям
        DO_VIRT,        ///Виртуальный дискретный выход(без привязки к модулям)
                        ///
        //DI
        DI = 1,         ///Дискретный вход с привязкой к модулям
        DI_VIRT,        ///Виртуальный дискретный вход(без привязки к модулям)

        //AO
        AO = 1,         ///Аналоговый выход с привязкой к модулям ввода-вывода
        AO_VIRT,        ///Виртуальный аналоговый выход (без привязки к модулям ввода-вывода)
                        ///
        //AI
        AI = 1,         ///Аналоговый вход с привязкой к модулям ввода-вывода
        AI_VIRT,        ///Виртуальный аналоговый вход (без привязки к модулям ввода-вывода)
                        ///
        //LT
        LT = 1,         ///Текущий уровень без дополнительных параметров
        LT_CYL,         ///Текущий уровень для цилиндрического танка
        LT_CONE,        ///Текущий уровень для танка с конусом
        LT_TRUNC,       ///Текущий уровень для танка с усеченным цилиндром

        LT_IOLINK,      ///< IO-Link текущий уровень без дополнительных параметров.

        LT_VIRT,        ///< Виртуальный текущий уровень.

        //Y
        Y = 1,          /// Обычный пневмоостров Festo
        DEV_VTUG_8,     /// Festo подтип DEV_VTUG_8
        DEV_VTUG_16,    /// Festo подтип DEV_VTUG_16
        DEV_VTUG_24,    /// Festo подтип DEV_VTUG_24

        //TE
        TE = 1,         ///< Текущая температура.
        TE_IOLINK,      ///< IO-Link текущая температура.

        //PT
        PT = 1,         ///< Датчик давления
        PT_IOLINK,      ///< IO-Link датчик давления
        DEV_SPAE        ///< IO-Link датчик давления воздуха    
    };

    /// <summary>
    /// Устройство - клапан, насос и т.д.
    /// </summary>
    public class Device : IComparable, IEquatable<Device>
    {

        public static int Compare(Device dx, Device dy)
        {
            if (dx == null && dy == null)
                return 0;

            if (dx == null)
                return -1;

            if (dy == null)
                return 1;


            if (dx.dType == dy.dType)
            {
                if (dx.objectName == dy.objectName)
                {
                    if (dx.ObjectNumber == dy.ObjectNumber)
                    {
                        if (dx.DeviceNumber == dy.DeviceNumber)
                        {
                            return 0;
                        }
                        else
                        {
                            return dx.DeviceNumber.CompareTo(dy.DeviceNumber);
                        }
                    }
                    else
                    {
                        return dx.ObjectNumber.CompareTo(dy.ObjectNumber);
                    }
                }
                else
                {
                    return dx.objectName.CompareTo(dy.objectName);
                }
            }
            else
            {
                return dx.dType.CompareTo(dy.dType);
            }
        }
        /// <summary>
        /// Конструктор на основе имени.
        /// </summary>
        /// <param name="name">Имя устройства (формат - А1V12).</param>
        /// <param name="description">Описание устройства.</param>
        protected Device(string fullName, string description,
            int deviceNumber, string objectName, int objectNumber)
        {
            this.name = fullName;
            this.description = description;

            this.deviceNumber = deviceNumber;

            this.objectName = objectName;
            this.objectNumber = objectNumber;
        }

        /// <summary>
        /// Установка подтипа устройства.
        /// </summary>
        public virtual string SetSubType(string subType)
        {
            try
            {
                dSubType = (DeviceSubType)Enum.Parse(typeof(DeviceSubType),
                    subType.ToUpper());
            }
            catch
            {
                dSubType = DeviceSubType.NONE;
            }

            return "";
        }

        /// <summary>
        /// Получение типа подключения для устройства
        /// </summary>
        public virtual string GetConnectionType()
        {
            return "";
        }

        /// <summary>
        /// Получение диапазона настройки
        /// </summary>
        public virtual string GetRange()
        {
            return "";
        }

        public Device Clone()
        {
            return (Device)MemberwiseClone();
        }

        #region Реализация интерфейсов IComparable, IEquatable<Device>.
        /// <summary>
        /// Метод проверки равенства объектов (для поиска).
        /// </summary>
        public bool Equals(Device otherDevice)
        {
            if (otherDevice == null)
            {
                return false;
            }

            if (name.Equals(otherDevice.name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверки равенства объектов (для поиска).
        /// </summary>
        public override bool Equals(Object otherDevice)
        {
            if (otherDevice == null)
                return base.Equals(otherDevice);

            return Equals(otherDevice as Device);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>
        /// Метод сравнения объектов (для сортировки).
        /// </summary>
        public int CompareTo(object otherDevice)
        {
            if (otherDevice == null)
                return 1;

            Device obj = otherDevice as Device;

            if (dType == obj.dType)
            {
                return string.CompareOrdinal(name, obj.name);
            }
            else
            {
                return dType.CompareTo(obj.dType);
            }
        }
        #endregion

        /// <summary>
        /// Имя устройства (например - А1V12).
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Имя устройства (например "+А1-V12").
        /// </summary>
        public string EPlanName
        {
            get
            {
                return "+" + objectName +
                    (objectNumber > 0 ? objectNumber.ToString() : "") +
                    "-" + DeviceType + DeviceNumber;
            }
        }

        /// <summary>
        /// Номер устройства.
        /// </summary>
        public long DeviceNumber
        {
            get
            {
                return deviceNumber;
            }
        }

        /// <summary>
        /// Тип устройства.
        /// </summary>
        public DeviceType DeviceType
        {
            get
            {
                return dType;
            }
        }

        /// <summary>
        /// Тип устройства.
        /// </summary>
        public DeviceSubType DeviceSubType
        {
            get
            {
                return dSubType;
            }
        }

        /// <summary>
        /// Описание устройства.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Объект устройство.
        /// </summary>
        public string ObjectName
        {
            get
            {
                return objectName;
            }
        }

        /// <summary>
        /// Номер объекта устройства.
        /// </summary>
        public int ObjectNumber
        {
            get
            {
                return objectNumber;
            }
        }

        /// <summary>
        /// Отладочное строковое представление устройства.
        /// </summary>
        public virtual string DPrint()
        {
            return String.Format("{0,8} {1} ", name, description);
        }

        #region Защищенные поля.

        protected internal string name; /// Имя устройства (А1V12).

        protected int deviceNumber;     /// Номер устройства (12 в R1V12).

        protected DeviceType dType = DeviceType.NONE;
        protected DeviceSubType dSubType = DeviceSubType.NONE;

        protected int dLocation; /// Номер узла, в котором должно располагаться утсройство.

        protected string objectName;    /// Объект устройства (R в R1V12).
        protected int objectNumber;     /// Номер объекта (1 в R1V12)

        protected internal string description;  /// Описание устройства (пример - "Отсечной клапан линии").   
        #endregion
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство, подключенное к модулям ввода\вывод IO.
    /// </summary>
    public class IODevice : Device
    {

        /// <summary>
        /// Получение строкового представления подтипа устройства.
        /// </summary>
        public static string GetDeviceSubTypeStr(DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.NONE:
                    return dt.ToString();

                case DeviceType.V:
                    switch (dst)
                    {
                        case DeviceSubType.V_DO1:
                            return "V_DO1";
                        case DeviceSubType.V_DO2:
                            return "V_DO2";
                        case DeviceSubType.V_DO1_DI1_FB_OFF:
                            return "V_DO1_DI1_FB_OFF";
                        case DeviceSubType.V_DO1_DI1_FB_ON:
                            return "V_DO1_DI1_FB_ON";
                        case DeviceSubType.V_DO1_DI2:
                            return "V_DO1_DI2";
                        case DeviceSubType.V_DO2_DI2:
                            return "V_DO2_DI2";
                        case DeviceSubType.V_MIXPROOF:
                            return "V_MIXPROOF";
                        case DeviceSubType.V_IOLINK_MIXPROOF:
                            return "V_IOLINK_MIXPROOF";
                        case DeviceSubType.V_AS_MIXPROOF:
                            return "V_AS_MIXPROOF";
                        case DeviceSubType.V_BOTTOM_MIXPROOF:
                            return "V_BOTTOM_MIXPROOF";
                        case DeviceSubType.V_AS_DO1_DI2:
                            return "V_AS_DO1_DI2";
                        case DeviceSubType.V_IOLINK_DO1_DI2:
                            return "V_IOLINK_DO1_DI2";
                        case DeviceSubType.V_DO2_DI2_BISTABLE:
                            return "V_DO2_DI2_BISTABLE";
                        case DeviceSubType.V_IOLINK_VTUG_DO1:
                            return "V_IOLINK_VTUG_DO1";
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF:
                            return "V_IOLINK_VTUG_DO1_FB_OFF";
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON:
                            return "V_IOLINK_VTUG_DO1_FB_ON";
                    }
                    break;

                case DeviceType.VC:
                    return dt.ToString();

                case DeviceType.M:
                    switch (dst)
                    {
                        case DeviceSubType.M:
                            return "M";
                        case DeviceSubType.M_FREQ:
                            return "M_FREQ";
                        case DeviceSubType.M_REV:
                            return "M_REV";
                        case DeviceSubType.M_REV_FREQ:
                            return "M_REV_FREQ";
                        case DeviceSubType.M_REV_2:
                            return "M_REV_2";
                        case DeviceSubType.M_REV_FREQ_2:
                            return "M_REV_FREQ_2";
                        case DeviceSubType.M_REV_2_ERROR:
                            return "M_REV_2_ERROR";
                        case DeviceSubType.M_REV_FREQ_2_ERROR:
                            return "M_REV_FREQ_2_ERROR";
                        case DeviceSubType.M_ATV:
                            return "M_ATV";
                    }
                    break;

                case DeviceType.LS:
                    switch (dst)
                    {
                        case DeviceSubType.LS_MIN:
                            return "LS_MIN";
                        case DeviceSubType.LS_MAX:
                            return "LS_MAX";
                        case DeviceSubType.LS_IOLINK_MIN:
                            return "LS_IOLINK_MIN";
                        case DeviceSubType.LS_IOLINK_MAX:
                            return "LS_IOLINK_MAX";
                        case DeviceSubType.LS_VIRT:
                            return "LS_VIRT";
                    }
                    break;

                case DeviceType.FQT:
                    switch (dst)
                    {
                        case DeviceSubType.FQT:
                            return "FQT";
                        case DeviceSubType.FQT_F:
                            return "FQT_F";
                        case DeviceSubType.FQT_F_OK:
                            return "FQT_F_OK";
                        case DeviceSubType.FQT_VIRT:
                            return "FQT_VIRT";
                    }
                    break;

                case DeviceType.QT:
                    switch (dst)
                    {
                        case DeviceSubType.QT:
                            return "QT";
                        case DeviceSubType.QT_OK:
                            return "QT_OK";
                        case DeviceSubType.QT_IOLINK:
                            return "QT_IOLINK";
                    }
                    break;

                case DeviceType.TE:
                    switch (dst)
                    {
                        case DeviceSubType.TE:
                            return "TE";
                        case DeviceSubType.TE_IOLINK:
                            return "TE_IOLINK";
                    }
                    break;

                case DeviceType.FS:
                    return dt.ToString();

                case DeviceType.GS:
                    return dt.ToString();

                case DeviceType.LT:
                    switch (dst)
                    {
                        case DeviceSubType.LT:
                            return "LT";
                        case DeviceSubType.LT_IOLINK:
                            return "LT_IOLINK";
                        case DeviceSubType.LT_CONE:
                            return "LT_CONE";
                        case DeviceSubType.LT_CYL:
                            return "LT_CYL";
                        case DeviceSubType.LT_TRUNC:
                            return "LT_TRUNC";
                        case DeviceSubType.LT_VIRT:
                            return "LT_VIRT";
                    }
                    break;

                case DeviceType.HA:
                    return dt.ToString();

                case DeviceType.HL:
                    return dt.ToString();

                case DeviceType.SB:
                    return dt.ToString();

                case DeviceType.DI:
                    switch (dst)
                    {
                        case DeviceSubType.DI:
                            return "DI";
                        case DeviceSubType.DI_VIRT:
                            return "DI_VIRT";
                    }
                    break;

                case DeviceType.DO:
                    switch (dst)
                    {
                        case DeviceSubType.DO:
                            return "DO";
                        case DeviceSubType.DO_VIRT:
                            return "DO_VIRT";
                    }
                    break;

                case DeviceType.PT:
                    switch (dst)
                    {
                        case DeviceSubType.PT:
                            return "PT";

                        case DeviceSubType.PT_IOLINK:
                            return "PT_IOLINK";

                        case DeviceSubType.DEV_SPAE:
                            return "DEV_SPAE";
                    }
                    break;

                case DeviceType.AI:
                    switch (dst)
                    {
                        case DeviceSubType.AI:
                            return "AI";
                        case DeviceSubType.AI_VIRT:
                            return "AI_VIRT";
                    }
                    break;

                case DeviceType.AO:
                    switch (dst)
                    {
                        case DeviceSubType.AO:
                            return "AO";
                        case DeviceSubType.AO_VIRT:
                            return "AO_VIRT";
                    }
                    break;

                case DeviceType.WT:
                    return dt.ToString();
            }
            return "";
        }

        /// <summary>
        /// Получение свойств устройства.
        /// </summary>
        public static List<string> GetDeviceProperties(DeviceType dt, DeviceSubType dst)
        {
            switch (dt)
            {
                case DeviceType.NONE:
                    return null;

                case DeviceType.V:
                    switch (dst)
                    {
                        case DeviceSubType.V_DO1:
                        case DeviceSubType.V_DO2:
                        case DeviceSubType.V_IOLINK_VTUG_DO1:
                            return new List<string>(new string[] { "ST", "M" });
                        case DeviceSubType.V_DO1_DI1_FB_OFF:
                        case DeviceSubType.V_DO1_DI1_FB_ON:
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF:
                        case DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON:
                            return new List<string>(new string[] { "ST", "M", "P_ON_TIME", "P_FB", "FB_OFF_ST" });
                        case DeviceSubType.V_DO1_DI2:
                        case DeviceSubType.V_DO2_DI2:
                        case DeviceSubType.V_DO2_DI2_BISTABLE:
                        case DeviceSubType.V_MIXPROOF:
                        case DeviceSubType.V_AS_MIXPROOF:
                        case DeviceSubType.V_AS_DO1_DI2:
                        case DeviceSubType.V_BOTTOM_MIXPROOF:                            
                        case DeviceSubType.V_IOLINK_MIXPROOF:
                        case DeviceSubType.V_IOLINK_DO1_DI2:
                            return new List<string>(new string[] 
                            { 
                                "ST", 
                                "M", 
                                "P_ON_TIME", 
                                "P_FB", 
                                "FB_OFF_ST", 
                                "FB_ON_ST" 
                            });
                    }
                    break;

                case DeviceType.VC:
                    return new List<string>(new string[] { "ST", "M", "V" });

                case DeviceType.M:
                    switch (dst)
                    {
                        case DeviceSubType.M:
                        case DeviceSubType.M_FREQ:
                        case DeviceSubType.M_REV:
                        case DeviceSubType.M_REV_FREQ:
                        case DeviceSubType.M_REV_2:
                        case DeviceSubType.M_REV_FREQ_2:
                        case DeviceSubType.M_REV_2_ERROR:
                            return new List<string>(new string[] { "ST", "M", "P_ON_TIME", "V" });
                    }
                    break;

                case DeviceType.LS:
                case DeviceType.FS:
                case DeviceType.GS:
                    return new List<string>(new string[] { "ST", "M", "P_DT" });

                case DeviceType.TE:
                    switch (dst)
                    {
                        case DeviceSubType.TE:
                            return new List<string>(new string[] { "M", "P_CZ", "V" });
                        case DeviceSubType.TE_IOLINK:
                            return new List<string>(new string[] { "M", "V" });
                    }
                    break;
                case DeviceType.LT:
                    switch (dst)
                    {
                        case DeviceSubType.LT:
                            return new List<string>(new string[] { "M", "P_CZ", "V" });
                        case DeviceSubType.LT_CYL:
                            return new List<string>(new string[] { "M", "P_CZ", "V", "P_MAX_P", "P_R", "CLEVEL" });
                        case DeviceSubType.LT_CONE:
                            return new List<string>(new string[] { "M", "P_CZ", "V", "P_MAX_P", "P_R", "P_H_CONE", "CLEVEL" });
                        case DeviceSubType.LT_TRUNC:
                            return new List<string>(new string[] { "M", "P_CZ", "V", "P_MAX_P", "P_R", "P_H_TRUNC", "CLEVEL" });

                        case DeviceSubType.LT_IOLINK:
                            return new List<string>(new string[] { "M", "V" });
                    }
                    break;

                case DeviceType.HA:
                    return new List<string>(new string[] { "ST", "M" });
                case DeviceType.HL:
                case DeviceType.SB:
                case DeviceType.DI:
                    return new List<string>(new string[] { "ST", "M", "P_DT" });
                case DeviceType.DO:
                    return new List<string>(new string[] { "ST", "M" });


                case DeviceType.AI:
                case DeviceType.AO:
                case DeviceType.PT:
                    switch (dst)
                    {
                        case DeviceSubType.PT:
                            return new List<string>(new string[] { "ST", "M", "V", "P_MIN_V", "P_MAX_V", "P_CZ" });

                        case DeviceSubType.PT_IOLINK:
                            return new List<string>(new string[] { "M", "V" });

                        case DeviceSubType.DEV_SPAE:
                            return new List<string>(new string[] { "M", "V" });
                    }
                    break;

                case DeviceType.FQT:
                    switch (dst)
                    {
                        case DeviceSubType.FQT:
                            return new List<string>(new string[] { "ST", "M", "V", "ABS_V" });
                        case DeviceSubType.FQT_F:
                            return new List<string>(new string[] { "ST", "M", "V", "P_MIN_FLOW", "P_MAX_FLOW", "P_CZ", "F", "P_DT", "ABS_V" });
                    }
                    break;

                case DeviceType.QT:
                    switch (dst)
                    {
                        case DeviceSubType.QT:
                            return new List<string>(new string[] { "ST", "M", "V", "P_MIN_V", "P_MAX_V", "P_CZ" });
                        case DeviceSubType.QT_OK:
                            return new List<string>(new string[] { "ST", "M", "V", "OK", "P_MIN_V", "P_MAX_V", "P_CZ" });
                        case DeviceSubType.QT_IOLINK:
                            return new List<string>(new string[] { "ST", "M", "V", "OK", "P_MIN_V", "P_MAX_V" });
                    }
                    break;


                case DeviceType.WT:
                    return new List<string>(new string[] { "ST", "M", "V", "P_NOMINAL_W", "P_DT", "P_RKP" });
            }

            return null;
        }

        /// <summary>
        /// Конструктор на основе имени.
        /// </summary>
        /// <param name="name">Имя устройства (формат - А1V12).</param>
        /// <param name="description">Описание устройства.</param>
        /// <param name="deviceType">Тип устройства (V для А1V12).</param>
        /// <param name="deviceNumber">Номер устройства (12 для А1V12).</param>
        /// <param name="objectName">Объект устройства (А для А1V12).</param>
        /// <param name="objectNumber">Номер объекта устройства (1 для А1V12).</param>
        protected internal IODevice(string fullName, string description,
            string deviceType, int deviceNumber, string objectName, int objectNumber)
            : this(fullName, description, deviceNumber, objectName, objectNumber)
        {
            try
            {
                dType = (DeviceType)Enum.Parse(typeof(DeviceType), deviceType, true);
            }
            catch (Exception)
            {
                dType = DeviceType.NONE;
            }
        }

        /// <summary>
        /// Конструктор на основе имени.
        /// </summary>
        /// <param name="name">Имя устройства (формат - А1V12).</param>
        /// <param name="description">Описание устройства.</param>
        /// <param name="deviceNumber">Номер устройства (12 для А1V12).</param>
        /// <param name="objectName">Объект устройства (А для А1V12).</param>
        /// <param name="objectNumber">Номер объекта устройства (1 для А1V12).</param>
        protected internal IODevice(string fullName, string description,
            int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            DO = new List<IOChannel>();
            DI = new List<IOChannel>();
            AO = new List<IOChannel>();
            AI = new List<IOChannel>();

            parameters = new Dictionary<string, object>();
            rtParameters = new Dictionary<string, object>();
            properties = new Dictionary<string, object>();
        }


        /// <summary>
        /// Установка номера узла, в котором подключается устройство.
        /// </summary>
        public void SetLocation(int devLocation)
        {
            dLocation = devLocation;
        }

        /// <summary>
        /// Установка параметра.
        /// </summary>
        public string SetParameter(string name, double value)
        {
            string res = "";

            object val = null;
            if (parameters.TryGetValue(name, out val))
            {
                parameters[name] = value;
            }
            else
            {
                res = string.Format("\"{0}\" - параметр не найден\n", name);
            }

            return res;
        }

        /// <summary>
        /// Установка рабочего параметра.
        /// </summary>
        public string SetRuntimeParameter(string name, double value)
        {
            string res = "";

            object val = null;
            if (rtParameters.TryGetValue(name, out val))
            {
                rtParameters[name] = value;
            }
            else
            {
                res = string.Format("\"{0}\" - рабочий параметр не найден\n", name);
            }

            return res;
        }

        /// <summary>
        /// Получение рабочего параметра.
        /// </summary>
        /// <param name="name">Имя устройства</param>
        /// <returns></returns>
        public string GetRuntimeParameter(string name)
        {
            object value = default;

            if (rtParameters.TryGetValue(name, out value))
            {
                return value.ToString();
            }
            else
            {
                return ""; 
            }
        }

        /// <summary>
        /// Сброс канала ввода\вывода.
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства канала.
        /// </param>   
        /// <param name="comment">Комментарий к каналу.</param>
        /// <param name="error">Строка с описанием ошибки при возникновении 
        /// таковой.</param>
        public bool ClearChannel(
            IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace,
            string comment, string channelName)
        {
            List<IOChannel> findedChannels = GetChannels(addressSpace, 
                channelName, comment);

            if (findedChannels.Count > 0)
            {
                foreach (IOChannel channel in findedChannels)
                {
                    channel.Clear();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Установка канала ввода\вывода.
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства канала.
        /// </param>
        /// <param name="node">Номер узла.</param>
        /// <param name="module">Номер модуля.</param>
        /// <param name="physicalKlemme">Номер клеммы.</param>
        /// <param name="comment">Комментарий к каналу.</param>
        /// <param name="error">Строка с описанием ошибки при возникновении 
        /// таковой.</param>
        public bool SetChannel(IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace,
            int node, int module, int physicalKlemme, string comment, 
            out string error, int fullModule, int logicalPort, 
            int moduleOffset, string channelName)
        {
            error = "";

            List<IOChannel> findedChannels = GetChannels(addressSpace, 
                channelName, comment);

            if (findedChannels.Count > 0)
            {
                foreach (IOChannel channel in findedChannels)
                {
                    if (!channel.IsEmpty())
                    {
                        error = string.Format(
                            "\"{0}\" : канал {1}.\"{2}\" уже привязан " +
                            "к A{3}.{4} \"{5}\".",
                            name, addressSpace, comment,
                            100 * (channel.Node + 1) + channel.Module, 
                            channel.PhysicalClamp, channel.Comment);
                        return false;
                    }

                    channel.SetChannel(node, module, physicalKlemme, 
                        fullModule, logicalPort, moduleOffset);
                    
                    List<IONode> nodes = IOManager.GetInstance().IONodes;
                    if (nodes.Count > node && 
                        nodes[node].IOModules.Count > module - 1)
                    {
                        nodes[node].IOModules[module - 1]
                            .AssignChannelToDevice(
                            physicalKlemme, this, channel);
                    }
                }
                return true;
            }
            else
            {
                error = string.Format(
                    "\"{0}\" : нет такого канала {1}:\"{2}\".",
                    name, addressSpace, comment);
                return false;
            }
        }

        /// <summary>
        /// Получить каналы устройства, которые привязывались или будут
        /// привязываться к модулю ввода-вывода
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства
        /// модуля ввода-вывода</param>
        /// <param name="channelName">Имя канала для IO-Link</param>
        /// <param name="comment">Комментарий канала</param>
        /// <returns></returns>
        private List<IOChannel> GetChannels(
            IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace, string channelName,
            string comment)
        {
            var IO = new List<IOChannel>();

            switch (addressSpace)
            {
                case IOModuleInfo.ADDRESS_SPACE_TYPE.DO:
                    IO.AddRange(DO);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.DI:
                    IO.AddRange(DI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AO:
                    IO.AddRange(AO);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AI:
                    IO.AddRange(AI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AOAI:
                    IO.AddRange(AO);
                    IO.AddRange(AI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.DODI:
                    IO.AddRange(DO);
                    IO.AddRange(DI);
                    break;

                case IOModuleInfo.ADDRESS_SPACE_TYPE.AOAIDODI:
                    switch (channelName)
                    {
                        default:
                            IO.AddRange(AO);
                            IO.AddRange(AI);
                            break;

                        case "DO":
                            IO.AddRange(DO);
                            break;

                        case "DI":
                            IO.AddRange(DI);
                            break;
                    }
                    break;
            }

            List<IOChannel> findedChannels = IO.FindAll(delegate (IOChannel channel)
            {
                return channel.Comment == comment;
            });

            return findedChannels;
        }

        /// <summary>
        /// Установка свойства. У устройства могут быть дополнительные свойства,
        /// которые задаются отдельно (доп. поле №4).
        /// </summary>
        virtual public string SetProperty(string name, object value)
        {
            string res = "";

            object val = null;
            if (properties.TryGetValue(name, out val))
            {
                properties[name] = value;
            }
            else
            {
                res = string.Format("\"{0}\" - свойство не найдено\n", name);
            }

            return res;
        }

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public virtual string Check()
        {
            string res = "";

            foreach (IOChannel ch in DO)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал DO \"{1}\".\n",
                        name, ch.Comment);
                }
            }
            foreach (IOChannel ch in DI)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал  DI \"{1}\".\n",
                        name, ch.Comment);
                }
            }
            foreach (IOChannel ch in AO)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал  AO \"{1}\".\n",
                        name, ch.Comment);
                }
            }
            foreach (IOChannel ch in AI)
            {
                if (ch.IsEmpty())
                {
                    res += string.Format("\"{0}\" : не привязанный канал  AI \"{1}\".\n",
                        name, ch.Comment);
                }
            }

            foreach (var par in parameters)
            {
                if (par.Value == null)
                {
                    res += string.Format("\"{0}\" : не задан параметр \"{1}\".\n",
                        name, par.Key);
                }
            }

            foreach (var par in rtParameters)
            {
                if (par.Value == null)
                {
                    res += string.Format("\"{0}\" : не задан рабочий параметр \"{1}\".\n",
                        name, par.Key);
                }
            }

            foreach (var prop in properties)
            {
                if (prop.Value == null)
                {
                    res += string.Format("\"{0}\" : не задано свойство \"{1}\".\n",
                        name, prop.Key);
                }
            }

            return res;
        }

        /// <summary>
        /// Связанная функция.        
        /// </summary>
        public Eplan.EplApi.DataModel.Function EplanObjectFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Сохранение в виде массива данных (для экспорта в таблицу).
        /// </summary>
        /// <returns>Количество записанных строк.</returns>
        public int SaveAsArray(ref object[,] arr, int row, int maxColumn)
        {
            int column = 0;

            arr[row, column++] = name;
            arr[row, column++] = description;
            arr[row, column++] = dType.ToString();
            if (dType != DeviceType.NONE && dSubType != DeviceSubType.NONE)
            {
                arr[row, column++] =
                    IODevice.GetDeviceSubTypeStr(dType, dSubType);
            }
            else
            {
                arr[row, column++] = "";
            }

            //Параметры.
            foreach (var par in parameters)
            {
                arr[row, column++] = par.Key;
                arr[row, column++] = par.Value;
                if (column >= maxColumn) break;
            }

            return 1;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        virtual public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "{\n";
            res += prefix + "name    = \'" + Name + "\',\n";
            res += prefix + "descr   = \'" + Description.Replace("\n", ". ") + "\',\n";
            res += prefix + "dtype   = " + (int)dType + ",\n";
            res += prefix + "subtype = " + (int)dSubType + ", -- " + GetDeviceSubTypeStr(dType, dSubType) + ", " + ArticleName + "\n";

            if (properties.Count > 0)
            {
                res += prefix + "prop = --Дополнительные свойства\n";
                res += prefix + "\t{\n";

                foreach (var prop in properties)
                {
                    if (prop.Value != null)
                    {
                        res += prefix + string.Format("\t{0} = {1},\n", prop.Key, prop.Value);
                    }
                }
                res += prefix + "\t},\n";
            }

            int bindedDO = CountOfBindedChannels(DO);
            if (DO.Count > 0 && bindedDO > 0)
            {
                res += prefix + "DO =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in DO)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            int bindedDI = CountOfBindedChannels(DI);
            if (DI.Count > 0 && bindedDI > 0)
            {
                res += prefix + "DI =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in DI)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            int bindedAO = CountOfBindedChannels(AO);
            if (AO.Count > 0 && bindedAO > 0)
            {
                res += prefix + "AO =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in AO)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            int bindedAI = CountOfBindedChannels(AI);
            if (AI.Count > 0 && bindedAI > 0)
            {
                res += prefix + "AI =\n";
                res += prefix + "\t{\n";
                foreach (IOChannel ch in AI)
                {
                    res += ch.SaveAsLuaTable(prefix + "\t\t");
                }
                res += prefix + "\t},\n";
            }

            if (rtParameters.Count > 0)
            {
                string tmp = "";

                foreach (var par in rtParameters)
                {
                    if (par.Value != null)
                    {
                        string tmpForSpacebars = $"\t\t\t\t{par.Value},";
                        int tmpForSpacebarsLength = tmpForSpacebars.Length % 4;
                        string spacebars = new string(' ', 4 + (4 - tmpForSpacebarsLength));

                        tmp += $"\t\t\t\t{par.Value},{spacebars}--{par.Key}\n";
                    }
                }

                if (tmp != "")
                {
                    res += prefix + "rt_par = \n\t\t\t\t{\n";
                    res += tmp;
                    res += prefix + "\t\t},\n";
                }
            }

            if (parameters.Count > 0)
            {
                string tmp = "";
                foreach (var par in parameters)
                {
                    if (par.Value != null)
                    {
                        tmp += par.Value + " --[[" + par.Key + "]], ";
                    }
                }

                if (tmp != "")
                {
                    res += prefix + "par = {";
                    res += tmp.Remove(tmp.Length - 1 - 1);
                    res += " }\n";
                }
            }

            res += prefix + "}";

            return res;
        }

        /// <summary>
        /// Сортировка каналов устройства для соответствия.
        /// </summary>
        public void sortChannels()
        {
            if (dType == DeviceType.V)
            {
                if (DI.Count > 1)
                {
                    List<IOChannel> tmp = new List<IOChannel>();

                    foreach (string descr in new string[] { "Открыт", "Закрыт" })
                    {
                        IOChannel resCh = DI.Find(delegate (IOChannel ch)
                        {
                            return ch.Comment == descr;
                        });

                        if (resCh != null)
                        {
                            tmp.Add(resCh);
                        }

                    }

                    DI = tmp;
                }

                if (DO.Count > 1)
                {
                    List<IOChannel> tmp2 = new List<IOChannel>();

                    foreach (string descr in new string[] { "Открыть", "Открыть мини", "Открыть ВС",
                                    "Открыть НС", "Закрыть" })
                    {
                        IOChannel resCh = DO.Find(delegate (IOChannel ch)
                        {
                            return ch.Comment == descr;
                        });

                        if (resCh != null)
                        {
                            tmp2.Add(resCh);
                        }

                    }

                    DO = tmp2;
                }
            }
        }

        /// <summary>
        /// Получение списка каналов устройства.
        /// </summary>
        public List<IOChannel> Channels
        {
            get
            {
                List<IOChannel> res = new List<IOChannel>();
                res.AddRange(DO);
                res.AddRange(DI);
                res.AddRange(AO);
                res.AddRange(AI);

                return res;
            }
        }


        /// <summary>
        /// Очистка привязки каналов устройства.
        /// </summary>
        public void ClearChannels()
        {
            foreach (IOChannel ch in Channels)
            {
                ch.Clear();
            }
        }

        /// <summary>
        /// Возвращает максимальны размер байтовой области для модулей ввода
        /// вывода при расчете IO-Link адресов если используется
        /// Phoenix Contact
        /// </summary>
        /// <returns></returns>
        public int GetMaxIOLinkSize()
        {
            return IOLinkSizeOut > IOLinkSizeIn ? IOLinkSizeOut : IOLinkSizeIn;
        }

        /// <summary>
        /// Возвращает количество каналов, которые привязаны к модулю
        /// ввода-вывода
        /// </summary>
        /// <param name="channels">Список каналов устройства 
        /// (AO,AI,DO,DI)</param>
        /// <returns></returns>
        private int CountOfBindedChannels(List<IOChannel> channels)
        {
            var count = 0;

            foreach (var channel in channels)
            {
                if (channel.IsEmpty() == false)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Свойство содержащее изделие, которое используется для устройства
        /// </summary>
        public string ArticleName { get; set; } = "";

        #region Закрытые поля.
        protected List<IOChannel> DO; ///Каналы дискретных выходов.
        protected List<IOChannel> DI; ///Каналы дискретных входов.
        protected List<IOChannel> AO; ///Каналы аналоговых выходов.
        protected List<IOChannel> AI; ///Каналы аналоговых входов.

        protected Dictionary<string, object> parameters;   ///Параметры.
        protected Dictionary<string, object> rtParameters; ///Рабочие параметры.
        protected Dictionary<string, object> properties;

        internal int IOLinkSizeIn = 0;
        internal int IOLinkSizeOut = 0;
        #endregion

        /// <summary>
        /// Канал ввода-вывода.
        /// </summary>
        public class IOChannel
        {
            public static int Compare(IOChannel wx, IOChannel wy)
            {
                if (wx == null && wy == null)
                    return 0;

                if (wx == null)
                    return -1;

                if (wy == null)
                    return 1;

                return wx.ToInt().CompareTo(wy.ToInt());
            }

            /// <param name="node">Номер узла.</param>
            /// <param name="module">Номер модуля.</param>
            /// <param name="physicalClamp">Физический номер клеммы.</param>
            /// <param name="fullModule">Полный номер модуля (101).</param>
            /// <param name="logicalClamp">Порядковый логический номер клеммы.</param>
            /// <param name="moduleOffset">Сдвиг модуля к которому привязан канал.</param>
            public void SetChannel(int node, int module, int physicalClamp, int fullModule,
                int logicalClamp, int moduleOffset)
            {
                this.node = node;
                this.module = module;
                this.physicalClamp = physicalClamp;

                this.fullModule = fullModule;
                this.logicalClamp = logicalClamp;
                this.moduleOffset = moduleOffset;
            }

            /// <summary>
            /// Сброс привязки канала ввода-вывода.
            /// </summary>
            public void Clear()
            {
                node = -1;
                module = -1;
                physicalClamp = -1;
                fullModule = -1;
                logicalClamp = -1;
                moduleOffset = -1;
            }

            /// <param name="name">Имя канала (DO, DI, AO, AI).</param>
            /// <param name="node">Номер узла.</param>
            /// <param name="module">Номер модуля.</param>
            /// <param name="clamp">Номер клеммы.</param>
            /// <param name="comment">Комментарий к каналу.</param>
            public IOChannel(string name, int node, int module, int clamp, string comment)
            {
                this.name = name;

                this.node = node;
                this.module = module;
                this.physicalClamp = clamp;
                this.comment = comment;
            }

            private int ToInt()
            {
                switch (name)
                {
                    case "DO":
                        return 0;

                    case "DI":
                        return 1;

                    case "AI":
                        return 2;

                    case "AO":
                        return 3;

                    case "AIAO":
                        return 4;

                    case "DODI":
                        return 5;

                    default:
                        return 6;
                }
            }

            /// <summary>
            /// Сохранение в виде таблицы Lua.
            /// </summary>
            /// <param name="prefix">Префикс (для выравнивания).</param>
            public string SaveAsLuaTable(string prefix)
            {
                string res = "";

                if (IOManager.GetInstance()[node] != null &&
                    IOManager.GetInstance()[node][module - 1] != null &&
                    physicalClamp >= 0)
                {
                    res += prefix + "{\n";

                    int offset;
                    switch (name)
                    {
                        case "DO":
                            offset = CalculateDO();
                            break;

                        case "AO":
                            offset = CalculateAO();
                            break;

                        case "DI":
                            offset = CalculateDI();
                            break;

                        case "AI":
                            offset = CalculateAI();
                            break;

                        default:
                            offset = -1;
                            break;
                    }

                    if (comment != "")
                    {
                        res += prefix + "-- " + comment + "\n";
                    }

                    res += prefix + $"node          = {node},\n";
                    res += prefix + $"offset        = {offset},\n";
                    res += prefix + $"physical_port = {physicalClamp},\n";
                    res += prefix + $"logical_port  = {logicalClamp},\n";
                    res += prefix + $"module_offset = {moduleOffset}\n";

                    res += prefix + "},\n";
                }

                return res;
            }

            /// <summary>
            /// Расчет AI адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateAI() 
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesIn.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    offset = md.InOffset;
                    offset += md.Info.ChannelAddressesIn[physicalClamp];

                    return offset;
                }

                return offset;
            }

            /// <summary>
            /// Расчет AO адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateAO() 
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesOut.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    offset = md.OutOffset;
                    offset += md.Info.ChannelAddressesOut[physicalClamp];
                    
                    return offset;
                }

                return offset;
            }

            /// <summary>
            /// Расчет DI адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateDI() 
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesIn.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    if (md.isIOLink() == true)
                    {
                        offset = 0;
                    }
                    else
                    {
                        offset = md.InOffset;
                    }
                    offset += md.Info.ChannelAddressesIn[physicalClamp];

                    return offset;
                }

                return offset;
            }

            /// <summary>
            /// Расчет DO адреса для сохранения в файл
            /// </summary>
            /// <returns>Адрес</returns>
            private int CalculateDO() 
            {
                int offset = -1;

                if (physicalClamp <= IOManager.GetInstance()[node][module - 1]
                    .Info.ChannelAddressesOut.Length)
                {
                    IOModule md = IOManager.GetInstance()[node][module - 1];
                    if (md.isIOLink() == true)
                    {
                        offset = 0;
                    }
                    else
                    {
                        offset = md.OutOffset;
                    }
                    offset += md.Info.ChannelAddressesOut[physicalClamp];
                    
                    return offset;
                }

                return offset;
            }

            public bool IsEmpty()
            {
                return node == -1;
            }

            /// <summary>
            /// Номер узла.
            /// </summary>
            public int Node
            {
                get
                {
                    return node;
                }
            }

            /// <summary>
            /// Номер модуля.
            /// </summary>
            public int Module
            {
                get
                {
                    return module;
                }
            }

            /// <summary>
            /// Физический номер клеммы.
            /// </summary>
            public int PhysicalClamp
            {
                get
                {
                    return physicalClamp;
                }
            }

            /// <summary>
            /// Полный номер модуля
            /// </summary>
            public int FullModule
            {
                get
                {
                    return fullModule;
                }
            }

            /// <summary>
            /// Комментарий
            /// </summary>
            public string Comment
            {
                get
                {
                    return comment;
                }
            }

            /// <summary>
            /// Имя канала (DI,DO, AI,AO)
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
            }

            /// <summary>
            /// Логический номер клеммы (порядковый)
            /// </summary>
            public int LogicalClamp
            {
                get
                {
                    return logicalClamp;
                }
            }

            /// <summary>
            /// Сдвиг начала модуля
            /// </summary>
            public int ModuleOffset
            {
                get
                {
                    return moduleOffset;
                }
            }

            /// <summary>
            /// Шаблон для разбора комментария к устройству.
            /// </summary>
            public const string ChannelCommentPattern =
                @"(Открыть мини(?n:\s+|$))|" +
                @"(Открыть НС(?n:\s+|$))|" +
                @"(Открыть ВС(?n:\s+|$))|" +
                @"(Открыть(?n:\s+|$))|" +
                @"(Закрыть(?n:\s+|$))|" +
                @"(Открыт(?n:\s+|$))|" +
                @"(Закрыт(?n:\s+|$))|" +
                @"(Объем(?n:\s+|$))|" +
                @"(Поток(?n:\s+|$))|" +
                @"(Пуск(?n:\s+|$))|" +
                @"(Реверс(?n:\s+|$))|" +
                @"(Обратная связь(?n:\s+|$))|" +
                @"(Частота вращения(?n:\s+|$))|" +
                @"(Авария(?n:\s+|$))|" +
                @"(Напряжение моста\(\+Ud\)(?n:\s+|$))|" +
                @"(Референсное напряжение\(\+Uref\)(?n:\s+|$))";

            #region Закрытые поля
            private int node;            ///Номер узла.
            private int module;          ///Номер модуля.
            private int fullModule;      ///Полный номер модуля.
            private int physicalClamp;   ///Физический номер клеммы.
            private string comment;      ///Комментарий.
            private string name;         ///Имя канала (DO, DI, AO ,AI).
            private int logicalClamp;    ///Логический номер клеммы.
            private int moduleOffset;    ///Сдвиг начала модуля.
            #endregion
        }

    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - клапан.
    /// Параметры:
    /// 1. P_ON_TIME - время включения, мсек.
    /// </summary>
    public class V : IODevice
    {
        public V(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.V;
            ArticleName = articleName;
        }

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public override string Check()
        {
            string res = base.Check();

            if (dSubType == DeviceSubType.NONE)
            {
                res += string.Format("\"{0}\" - не задан тип (V_DO1, V_DO2, ...).\n",
                    name);
            }


            if ((dSubType == DeviceSubType.V_IOLINK_VTUG_DO1 ||
                    dSubType == DeviceSubType.V_IOLINK_VTUG_DO1_FB_OFF ||
                    dSubType == DeviceSubType.V_IOLINK_VTUG_DO1_FB_ON) &&
                AO[0].Node >= 0 && AO[0].Module > 0)
            {
                // DEV_VTUG - поддержка старых проектов
                if (
                    IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0] != null &&
                    IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0].DeviceType != DeviceType.Y &&
                    IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0].DeviceType != DeviceType.DEV_VTUG)
                {
                    res += string.Format("\"{0}\" - первым в списке привязанных устройств должен идти пневмоостров Festo VTUG.\n",
                        name);
                }
                else
                {
                    var vtug = IOManager.GetInstance()[AO[0].Node][AO[0].Module - 1].devices[AO[0].PhysicalClamp][0];
                    switch (vtug.DeviceSubType)
                    {
                        case DeviceSubType.DEV_VTUG_8:
                            rtParameters["R_VTUG_SIZE"] = 1;
                            break;

                        case DeviceSubType.DEV_VTUG_16:
                            rtParameters["R_VTUG_SIZE"] = 2;
                            break;

                        case DeviceSubType.DEV_VTUG_24:
                            rtParameters["R_VTUG_SIZE"] = 3;
                            break;
                    }
                }
            }

            return res;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "V_DO1":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    break;

                case "V_DO2":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Закрыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    break;

                case "V_DO1_DI1_FB_OFF":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_DO1_DI1_FB_ON":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_DO1_DI2":
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_DO2_DI2":
                case "V_DO2_DI2_BISTABLE":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Закрыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_MIXPROOF":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть НС"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть ВС"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_IOLINK_MIXPROOF":
                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    IOLinkSizeIn = 32;
                    IOLinkSizeOut = 8;
                    break;

                case "V_AS_MIXPROOF":
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    rtParameters.Add("R_AS_NUMBER", null);
                    break;

                case "V_BOTTOM_MIXPROOF":
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть мини"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Открыть НС"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Закрыт"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Открыт"));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_AS_DO1_DI2":
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);

                    rtParameters.Add("R_AS_NUMBER", null);
                    break;

                case "V_IOLINK_DO1_DI2":
                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    
                    IOLinkSizeIn = 32;
                    IOLinkSizeOut = 8;
                    break;

                case "V_IOLINK_VTUG_DO1":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    break;

                case "V_IOLINK_VTUG_DO1_FB_OFF":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "V_IOLINK_VTUG_DO1_FB_ON":
                    rtParameters.Add("R_VTUG_NUMBER", null);
                    rtParameters.Add("R_VTUG_SIZE", 1);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_ON_TIME", null);
                    parameters.Add("P_FB", 1);
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип (V_DO1, V_DO2, ...).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (V_DO1, V_DO2, ...).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - управляемый клапан.
    /// </summary>
    public class VC : IODevice
    {
        public VC(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.VC;
            ArticleName = articleName;

            AO.Add(new IOChannel("AO", -1, -1, -1, ""));
        }

        public override string SetSubType(string subType)
        {
            dSubType = DeviceSubType.NONE;

            return "";
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - аварийная звуковая сигнализация.
    /// </summary>
    public class HA : IODevice
    {
        public HA(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HA;
            ArticleName = articleName;

            DO.Add(new IOChannel("DO", -1, -1, -1, ""));
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - световая сигнализация.
    /// </summary>
    public class HL : IODevice
    {
        public HL(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.HL;
            ArticleName = articleName;

            DO.Add(new IOChannel("DO", -1, -1, -1, ""));
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - кнопка.
    /// </summary>
    public class SB : IODevice
    {
        public SB(string fullName, string description,
                                int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.SB;
            ArticleName = articleName;

            DI.Add(new IOChannel("DI", -1, -1, -1, ""));
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - дискретный вход.
    /// </summary>
    public class DI : IODevice
    {
        public DI(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DI;

            parameters.Add("P_DT", null);
        }
        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DI_VIRT":
                    dSubType = DeviceSubType.DI_VIRT;
                    break;
                case "DI":
                case "":
                    dSubType = DeviceSubType.NONE;
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (DI, DI_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - дискретный выход.
    /// </summary>
    public class DO : IODevice
    {
        public DO(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DO;
        }
        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DO_VIRT":
                    dSubType = DeviceSubType.DO_VIRT;
                    break;
                case "DO":
                case "":
                    dSubType = DeviceSubType.NONE;
                    DO.Add(new IOChannel("DO", -1, -1, -1, ""));

                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (DO, DO_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - аналоговый вход.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение.
    /// 2. P_MAX_V - максимальное значение.
    /// 3. P_C0    - сдвиг нуля.
    /// </summary>
    public class AI : IODevice
    {
        public AI(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AI;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "AI_VIRT":
                    dSubType = DeviceSubType.AI_VIRT;
                    break;
                case "AI":
                case "":
                    dSubType = DeviceSubType.NONE;

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    break;
                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (AI, AI_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_V") && parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." + parameters["P_MAX_V"].ToString();
            }
            return range;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - аналоговый выход.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение.
    /// 2. P_MAX_V - максимальное значение.
    /// </summary>
    public class AO : IODevice
    {
        public AO(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.AO;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "AO_VIRT":
                    dSubType = DeviceSubType.AO_VIRT;
                    break;
                case "AO":
                case "":
                    dSubType = DeviceSubType.NONE;

                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);

                    AO.Add(new IOChannel("AO", -1, -1, -1, ""));

                    break;
                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (AO, AO_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_V") && parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." + parameters["P_MAX_V"].ToString();
            }
            return range;
        }

    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - датчик наличия потока.
    /// Параметры:
    /// 1. P_DT - время порогового фильтра, мсек.    
    /// </summary>
    public class FS : IODevice
    {
        public FS(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.FS;
            ArticleName = articleName;

            DI.Add(new IOChannel("DI", -1, -1, -1, ""));

            parameters.Add("P_DT", null);
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - датчик проводимости.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение.
    /// 2. P_MAX_V - максимальное значение.
    /// 3. P_C0    - сдвиг нуля.
    /// </summary>
    public class QT : IODevice
    {
        public QT(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.QT;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, ""));
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "QT":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);
                    break;

                case "QT_OK":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "QT_IOLINK":
                    IOLinkSizeIn = 6;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (QT, QT_OK, QT_IOLINK).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (QT, QT_OK, QT_IOLINK).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_V") && parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." + parameters["P_MAX_V"].ToString();
            }
            return range;
        }

        /// <summary>
        /// Проверка устройства на корректную инициализацию.
        /// </summary>
        /// <returns>Строка с описанием ошибки.</returns>
        public override string Check()
        {
            string res = base.Check();

            if (this.DeviceSubType != DeviceSubType.QT_IOLINK)
            {
                if (parameters.Count < 2)
                {
                    res += string.Format("{0} - не указан диапазон измерений\n",
                            name);
                }
            }
            return res;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - датчик положения.
    /// Параметры:
    /// 1. P_DT - время порогового фильтра, мсек.
    /// </summary>
    public class GS : IODevice
    {
        public GS(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.GS;
            ArticleName = articleName;

            DI.Add(new IOChannel("DI", -1, -1, -1, ""));

            parameters.Add("P_DT", null);
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - датчик давления.
    /// Параметры:
    /// 1. P_C0    - сдвиг нуля.
    /// 2. P_MIN_V - минимальное значение.
    /// 3. P_MAX_V - максимальное значение.
    /// </summary>
    public class PT : IODevice
    {
        public PT(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.PT;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, ""));
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = "";
            switch (subType)
            {
                case "PT":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);
                    break;

                case "PT_IOLINK":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_MIN_V", null);
                    parameters.Add("P_MAX_V", null);
                    IOLinkSizeIn = 1;
                    break;

                case "DEV_SPAE":
                    IOLinkSizeIn = 1;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (PT, PT_IOLINK, DEV_SPAE).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (PT, PT_IOLINK, DEV_SPAE).\n", Name);
                    break;
            }
            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_V") && parameters.ContainsKey("P_MAX_V"))
            {
                range = "_" + parameters["P_MIN_V"].ToString() + ".." + parameters["P_MAX_V"].ToString();
            }
            return range;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - датчик текущего уровня.
    /// Параметры:
    /// 1. P_C0 - сдвиг нуля.    
    /// </summary>
    public class LT : IODevice
    {
        public LT(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.LT;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "LT":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    break;

                case "LT_CYL":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    break;

                case "LT_CONE":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    parameters.Add("P_H_CONE", null);
                    break;

                case "LT_TRUNC":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));

                    parameters.Add("P_C0", null);
                    parameters.Add("P_MAX_P", null);
                    parameters.Add("P_R", null);
                    parameters.Add("P_H_TRUNC", null);
                    break;

                case "LT_IOLINK":
                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    IOLinkSizeIn = 1;
                    break;

                case "LT_VIRT":
                    dSubType = DeviceSubType.LT_VIRT;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (LT, LT_CYL, LT_CONE, LT_TRUNC, LT_IOLINK, LT_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (LT, LT_CYL, LT_CONE, LT_TRUNC, LT_IOLINK, LT_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - датчик температуры.
    /// Параметры:
    /// 1. P_C0  - сдвиг нуля.
    /// 2. P_ERR - аварийное значение температуры.
    /// </summary>
    public class TE : IODevice
    {
        public TE(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.TE;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, ""));
        }

        public override string SetSubType(string subType)
        {
            base.SetSubType(subType);

            string errStr = "";
            switch (subType)
            {
                case "TE":
                    parameters.Add("P_C0", null);
                    parameters.Add("P_ERR", null);
                    break;

                case "TE_IOLINK":
                    IOLinkSizeIn = 1;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (TE, TE_IOLINK).\n", Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (TE, TE_IOLINK).\n", Name);
                    break;
            }
            return errStr;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - мотор.
    /// Параметры:
    /// 1. P_ON_TIME - время включения, мсек.
    /// </summary>
    public class M : IODevice
    {
        public M(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.M;

            DO.Add(new IOChannel("DO", -1, -1, -1, "Пуск"));

            parameters.Add("P_ON_TIME", null);
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "M":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));

                    break;

                case "M_FREQ":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    break;

                case "M_REV":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));

                    break;

                case "M_REV_FREQ":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    break;

                case "M_REV_2":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));

                    break;

                case "M_REV_FREQ_2":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    break;


                case "M_REV_2_ERROR":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Авария"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));

                    break;

                case "M_REV_FREQ_2_ERROR":
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Обратная связь"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, "Авария"));
                    DO.Add(new IOChannel("DO", -1, -1, -1, "Реверс"));
                    AO.Add(new IOChannel("AO", -1, -1, -1, "Частота вращения"));

                    break;

                case "M_ATV":
                    DO.Clear();
                    properties.Add("IP", null);
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (M, M_FREQ, M_REV, M_REV_FREQ, M_REV_2," +
                        " M_REV_FREQ_2, M_REV_2_ERROR, M_REV_FREQ_2_ERROR, M_ATV).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (M, M_FREQ, M_REV, M_REV_FREQ, M_REV_2," +
                        " M_REV_FREQ_2, M_REV_2_ERROR, M_REV_FREQ_2_ERROR, M_ATV).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - счетчик.
    /// Параметры:
    /// 1. P_MIN_V - минимальное значение потока.
    /// 2. P_MAX_V - максимальное значение потока.
    /// 3. P_C0    - сдвиг нуля для потока.
    /// </summary>
    public class FQT : IODevice
    {
        public FQT(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.FQT;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "FQT":
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Объем"));
                    break;

                case "FQT_F":
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Объем"));
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Поток"));

                    parameters.Add("P_MIN_F", null);
                    parameters.Add("P_MAX_F", null);
                    parameters.Add("P_C0", null);
                    parameters.Add("P_DT", null);

                    properties.Add("MT", null); //Связанные моторы.

                    break;

                case "FQT_F_OK":
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Объем"));
                    AI.Add(new IOChannel("AI", -1, -1, -1, "Поток"));
                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));

                    parameters.Add("P_MIN_F", null);
                    parameters.Add("P_MAX_F", null);
                    parameters.Add("P_C0", null);
                    parameters.Add("P_DT", null);

                    properties.Add("MT", null); //Связанные моторы.

                    break;

                case "FQT_VIRT":
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип" +
                        " (FQT, FQT_F, FQT_F_OK, FQT_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип" +
                        " (FQT, FQT_F, FQT_F_OK, FQT_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetRange()
        {
            string range = "";
            if (parameters.ContainsKey("P_MIN_F") && parameters.ContainsKey("P_MAX_F"))
            {
                range = "_" + parameters["P_MIN_F"].ToString() + ".." + parameters["P_MAX_F"].ToString();
            }
            return range;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - предельный уровень.
    /// Параметры:
    /// 1. P_DT - время порогового фильтра, мсек.
    /// </summary>
    public class LS : IODevice
    {
        public LS(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.LS;
            ArticleName = articleName;
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "LS_MIN":
                    parameters.Add("P_DT", null);

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "LS_MAX":
                    parameters.Add("P_DT", null);

                    DI.Add(new IOChannel("DI", -1, -1, -1, ""));
                    break;

                case "LS_IOLINK_MIN":
                    parameters.Add("P_DT", null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    IOLinkSizeIn = 1;
                    break;

                case "LS_IOLINK_MAX":
                    parameters.Add("P_DT", null);

                    AI.Add(new IOChannel("AI", -1, -1, -1, ""));
                    IOLinkSizeIn = 1;
                    break;

                case "LS_VIRT":
                    dSubType = DeviceSubType.LS_VIRT;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип (LS_MIN, " +
                        "LS_MAX, LS_IOLINK_MIN, LS_IOLINK_MAX, LS_VIRT).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (LS_MIN, " +
                        "LS_MAX, LS_IOLINK_MIN, LS_IOLINK_MAX, LS_VIRT).\n",
                        Name);
                    break;
            }

            return errStr;
        }

        public override string GetConnectionType()
        {
            string connectionType = "";
            switch (dSubType)
            {
                case DeviceSubType.LS_MIN:
                case DeviceSubType.LS_IOLINK_MIN:
                    connectionType = "_Min";
                    break;

                case DeviceSubType.LS_MAX:
                case DeviceSubType.LS_IOLINK_MAX:
                    connectionType = "_Max";
                    break;


                default:
                    connectionType = "";
                    break;
            }
            return connectionType;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Технологическое устройство - тензодатчик(датчик веса).
    /// Параметры:
    /// 1. P_NOMINAL_W - Номинальная нагрузка в кг.
    /// 2. P_RKP - рабочий коэффициент передачи
    /// 3. P_C0    - сдвиг нуля.
    /// </summary>
    public class WT : IODevice
    {
        public WT(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.WT;
            ArticleName = articleName;

            AI.Add(new IOChannel("AI", -1, -1, -1, "Напряжение моста(+Ud)"));
            AI.Add(new IOChannel("AI", -1, -1, -1, "Референсное напряжение(+Uref)"));

            parameters.Add("P_NOMINAL_W", null);
            parameters.Add("P_RKP", null);
            parameters.Add("P_C0", null);
            parameters.Add("P_DT", null);
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Устройство - пневмоостров.
    /// </summary>

    public class Y : IODevice
    {

        public Y(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber, string articleName)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.Y;
            ArticleName = articleName;

            AO.Add(new IOChannel("AO", -1, -1, -1, ""));
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DEV_VTUG_8":
                    IOLinkSizeOut = 1;
                    break;

                case "DEV_VTUG_16":
                    IOLinkSizeOut = 2;
                    break;

                case "DEV_VTUG_24":
                    IOLinkSizeOut = 3;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип (DEV_VTUG_8, ...).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (DEV_VTUG_8, ...).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }


    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Устройство - пневмоостров.
    /// СОВМЕСТИМОСТЬ СО СТАРЫМИ ПРОЕКТАМИ
    /// </summary>
    public class DEV_VTUG : IODevice
    {
        public DEV_VTUG(string fullName, string description,
                    int deviceNumber, string objectName, int objectNumber)
            : base(fullName, description, deviceNumber, objectName, objectNumber)
        {
            dSubType = DeviceSubType.NONE;
            dType = DeviceType.DEV_VTUG;

            AO.Add(new IOChannel("AO", -1, -1, -1, ""));
        }

        public override string SetSubType(string subtype)
        {
            base.SetSubType(subtype);

            string errStr = "";
            switch (subtype)
            {
                case "DEV_VTUG_8":
                    IOLinkSizeOut = 1;
                    break;

                case "DEV_VTUG_16":
                    IOLinkSizeOut = 2;
                    break;

                case "DEV_VTUG_24":
                    IOLinkSizeOut = 3;
                    break;

                case "":
                    errStr = string.Format("\"{0}\" - не задан тип (DEV_VTUG_8, ...).\n",
                        Name);
                    break;

                default:
                    errStr = string.Format("\"{0}\" - неверный тип (DEV_VTUG_8, ...).\n",
                        Name);
                    break;
            }

            return errStr;
        }
    }

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>
    /// Менеджер описания устройств для проекта.
    /// </summary>
    public class DeviceManager
    {
        public void GetObjectForXML(TreeNode rootNode)
        {
            foreach (IODevice dev in devices)
            {
                if (dev != null)
                {
                    List<string> propertiesList =
                        IODevice.GetDeviceProperties(dev.DeviceType, dev.DeviceSubType);
                    if (propertiesList != null)
                    {
                        foreach (string strProp in propertiesList)
                        {
                            string nodeName = dev.DeviceType.ToString() + "_" + strProp;
                            if (!rootNode.Nodes.ContainsKey(nodeName))
                            {
                                TreeNode newNode = rootNode.Nodes.Add(nodeName, nodeName);
                                newNode.Nodes.Add(dev.Name + "." + strProp, dev.Name + "." + strProp);
                            }
                            else
                            {
                                TreeNode newNode = rootNode.Nodes.Find(nodeName, false)[0];
                                newNode.Nodes.Add(dev.Name + "." + strProp, dev.Name + "." + strProp);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы (массива строк, чисел и т.д.).
        /// </summary>
        public object[,] SaveAsArray()
        {
            int MAX_TYPE_CNT = Enum.GetValues(typeof(DeviceType)).Length;
            const int MAX_SUBTYPE_CNT = 20;
            int[] countDev = new int[MAX_TYPE_CNT];
            int[,] countSubDev = new int[MAX_TYPE_CNT, MAX_SUBTYPE_CNT];

            int RowsCnt = devices.Count;
            const int MAX_COL = 20;

            object[,] res = new object[RowsCnt, MAX_COL];

            int idx = 0;
            foreach (IODevice dev in devices)
            {
                idx += dev.SaveAsArray(ref res, idx, MAX_COL);
            }

            return res;
        }

        public TreeView SaveConnectionAsTree()
        {
            TreeView tree = new TreeView();
            foreach (DeviceType devType in Enum.GetValues(typeof(DeviceType)))
            {
                TreeNode typeNode = new TreeNode(devType.ToString());
                typeNode.Tag = devType;
                tree.Nodes.Add(typeNode);
            }

            foreach (IODevice dev in devices)
            {
                TreeNode parent = null;
                TreeNode devNode = null;

                foreach (TreeNode node in tree.Nodes)
                {
                    if ((DeviceType)node.Tag == dev.DeviceType)
                    {
                        parent = node;
                        break;
                    }
                }

                //Не найден тип устройства.
                if (parent == null)
                {
                    break;
                }

                if (dev.ObjectName != "")
                {
                    string objectName = dev.ObjectName + dev.ObjectNumber;
                    TreeNode devParent = null;

                    foreach (TreeNode node in parent.Nodes)
                    {
                        if ((node.Tag is String) &&
                            (string)node.Tag == objectName)
                        {
                            devParent = node;
                            break;
                        }
                    }

                    if (devParent == null)
                    {
                        devParent = new TreeNode(objectName);
                        devParent.Tag = objectName;
                        parent.Nodes.Add(devParent);
                    }


                    devNode = new TreeNode(dev.Name + " - " + dev.Description);
                    devNode.Tag = dev;
                    devParent.Nodes.Add(devNode);

                }
                else
                {

                    devNode = new TreeNode(dev.Name + " - " + dev.Description);
                    devNode.Tag = dev;
                    parent.Nodes.Add(devNode);
                }
                foreach (IODevice.IOChannel ch in dev.Channels)
                {
                    string chNodeName = "";
                    if (!ch.IsEmpty())
                    {
                        chNodeName = ch.Name + " " + ch.Comment +
                            $" (A{ch.FullModule}:" + ch.PhysicalClamp + ")";
                    }
                    else
                    {
                        chNodeName = ch.Name + " " + ch.Comment;
                    }
                    devNode.Nodes.Add(chNodeName);
                }

            }

            return tree;
        }

        /// <summary>
        /// Сохранение сводной информации в виде таблицы.
        /// </summary>
        public object[,] SaveSummaryAsArray()
        {
            int MAX_TYPE_CNT = Enum.GetValues(typeof(DeviceType)).Length;
            const int MAX_SUBTYPE_CNT = 20;
            int[] countDev = new int[MAX_TYPE_CNT];
            int[,] countSubDev = new int[MAX_TYPE_CNT, MAX_SUBTYPE_CNT];

            const int MAX_ROW = 200;
            const int MAX_COL = 20;

            object[,] res = new object[MAX_ROW, MAX_COL];

            foreach (IODevice dev in devices)
            {
                countDev[(int)dev.DeviceType + 1]++;
                countSubDev[(int)dev.DeviceType + 1, (int)dev.DeviceSubType + 1]++;
            }

            //Сводная таблица.
            int idx = 0;
            foreach (DeviceType devType in Enum.GetValues(typeof(DeviceType)))
            {
                if (devType == DeviceType.V || devType == DeviceType.M ||
                    devType == DeviceType.LS)
                {
                    for (int i = 0; i < MAX_SUBTYPE_CNT; i++)
                    {
                        if (countSubDev[(int)devType + 1, i] > 0)
                        {
                            res[idx, 0] = IODevice.GetDeviceSubTypeStr(
                                devType, (DeviceSubType)i - 1);
                            res[idx, 1] = countSubDev[(int)devType + 1, i];

                            idx++;
                        }
                    }
                }
                else
                {
                    if (countDev[(int)devType + 1] > 0)
                    {
                        res[idx, 0] = devType.ToString();
                        res[idx, 1] = countDev[(int)devType + 1];

                        idx++;
                    }
                }
            }

            res[idx, 0] = "Всего";
            res[idx, 1] = devices.Count;

            return res;
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
            var res = "";

            foreach (var dev in devices)
            {
                res += dev.Check();
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

        /// <summary>
        /// Возвращает устройство по его имени в Eplan
        /// </summary>
        /// <param name="devName">Имя устройств в Eplan</param>
        /// <returns></returns>
        public IODevice GetDeviceByEplanName(string devName)
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
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            // Устройства нет, вернет состояние заглушки.
            CheckDeviceName(devName, out name, out objectName,
                out objectNumber, out deviceType, out deviceNumber);
            return new IODevice(name, "заглушка", deviceType,
                deviceNumber, objectName, objectNumber);
        }

        /// <summary>
        /// Получение устройства по его имени (ОУ) из глобального списка.
        /// </summary>
        /// <param name="devName">Имя устройства.</param>
        /// <returns>Устройство с заданными именем или устройство-заглушка.</returns>
        public IODevice GetDevice(string devName)
        {
            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            CheckDeviceName(devName, out name, out objectName, out objectNumber,
                out deviceType, out deviceNumber);

            IODevice devStub = new IODevice(name, "заглушка",
                deviceType, deviceNumber, objectName, objectNumber);

            int resDevN = devices.BinarySearch(devStub);

            if (resDevN >= 0)
            {
                return devices[resDevN];
            }

            return devStub;
        }

        public int GetDeviceListNumber(string devName)
        {
            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;

            CheckDeviceName(devName, out name, out objectName, out objectNumber,
                out deviceType, out deviceNumber);

            IODevice devStub = new IODevice(name, "заглушка",
                deviceType, deviceNumber, objectName, objectNumber);

            int resDevN = devices.IndexOf(devStub);

            return resDevN;
        }


        /// <summary>
        /// Шаблон для разбора имени для привязки устройств
        /// "==BR2=HUT1+KOAG5-V2"
        /// "+KOAG5-V2"
        /// </summary>
        public static readonly string BINDING_DEVICES_DESCRIPTION_PATTERN =
            @"(?<name>([+-])(?<object_main>([A-Z_]*?)(\d*?))(?<object>[A-Z_]*?)(?<object_n>\d*)(-*)(?<type>[A-Z_]+)(?<n>\d+))(\s+|$)";

        /// <summary>
        ///Шаблоны для разбора имени в остальных случаях: 
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
        /// <param name="devName">Имя устройство.</param>
        static public bool CheckDeviceName(string fullDevName,
            out string devName, out string objectName, out int objectNumber,
            out string deviceType, out int deviceNumber)
        {
            bool res = false;
            objectName = "";
            objectNumber = 0;
            deviceType = "";
            deviceNumber = 0;

            devName = fullDevName;

            Match match = Regex.Match(fullDevName, DESCRIPTION_PATTERN,
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string type = match.Groups["type"].Value;
                switch (type)
                {
                    case "V":
                    case "VC":
                    case "M":
                    case "N":
                    case "LS":
                    case "TE":
                    case "GS":
                    case "FS":
                    case "FQT":
                    case "AO":
                    case "LT":
                    case "OS":
                    case "DI":
                    case "UPR":
                    case "DO":
                    case "QT":
                    case "AI":
                    case "HA":
                    case "HL":
                    case "SB":
                    case "WT":
                    case "PT":

                    case "Y":
                    case "DEV_VTUG": // Совместимость со старыми проектами

                        objectName = match.Groups["object_main"].Value + match.Groups["object"];
                        if (match.Groups["object_n"].Value != "")
                        {
                            objectNumber = System.Convert.ToInt32(
                                match.Groups["object_n"].Value);
                        }

                        deviceType = match.Groups["type"].Value;
                        if (match.Groups["n"].Value != "")
                        {
                            deviceNumber = System.Convert.ToInt32(
                                match.Groups["n"].Value);
                        }

                        devName = match.Groups["object_main"].Value + match.Groups["object"].Value +
                            match.Groups["object_n"].Value +
                            match.Groups["type"].Value +
                            match.Groups["n"].Value;

                        res = true;
                        break;
                }
            }

            return res;
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
            errStr = "";
            IODevice dev = null;

            string name;
            string objectName;
            int objectNumber;
            string deviceType;
            int deviceNumber;


            CheckDeviceName(devName, out name, out objectName,
                out objectNumber, out deviceType, out deviceNumber);

            // Если изделия нет или пустое, то оставляем пустое
            if (articleName == "" || articleName == null)
            {
                articleName = "";
            }

            switch (deviceType)
            {
                case "V":
                    dev = new V(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "VC":
                    dev = new VC(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "M":
                case "N":
                    dev = new M(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "LS":
                    dev = new LS(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "TE":
                    dev = new TE(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "GS":
                    dev = new GS(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "FS":
                    dev = new FS(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "FQT":
                    dev = new FQT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "AO":
                    dev = new AO(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "LT":
                    dev = new LT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "OS":
                case "DI":
                    dev = new DI(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "UPR":
                case "DO":
                    dev = new DO(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "QT":
                    dev = new QT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "AI":
                    dev = new AI(name, description, deviceNumber, objectName,
                        objectNumber);
                    break;

                case "HA":
                    dev = new HA(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "HL":
                    dev = new HL(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "SB":
                    dev = new SB(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "WT":
                    dev = new WT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "PT":
                    dev = new PT(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;

                case "Y":
                    dev = new Y(name, description, deviceNumber, objectName,
                        objectNumber, articleName);
                    break;
                case "DEV_VTUG": // Совместимость со старыми проектами
                    dev = new DEV_VTUG(name, description, deviceNumber, objectName,
                        objectNumber);
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
        public string SaveAsLuaTable(string prefix)
        {
            string res = "------------------------------------------------------------------------------\n";
            res += "--Устройства\n";
            res += prefix + "devices =\n";
            res += prefix + "\t{\n";

            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                dev.sortChannels();
                res += dev.SaveAsLuaTable(prefix + "\t\t") + ",\n\n";
            }

            res += prefix + "\t}\n";
            res = res.Replace("\t", "    ");

            return res;
        }

        /// <summary>
        /// Сохранение устройств в виде скрипта Lua. Для последующего доступа
        /// по имени. Строки в виде: "S1V23 = V( 'S1V23' ) ".
        /// </summary>
        public string SaveDevicesAsLuaScript()
        {
            string str = "system = system or {}\n";
            str += "system.init_dev_names = function()\n";

            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                if (dev.ObjectNumber > 0 && dev.ObjectName == "")
                {
                    str += "\t_";
                }
                else
                {
                    str += "\t";
                }
                str += dev.Name + " = " + dev.DeviceType.ToString() + "(\'" + dev.Name + "\')\n";
            }
            str += "\n";

            int i = 0;
            foreach (IODevice dev in devices)
            {
                if (dev.DeviceType == DeviceType.Y ||
                    dev.DeviceType == DeviceType.DEV_VTUG) continue;

                str += "\t__" + dev.Name + " = DEVICE( " + i + " )\n";
                i++;
            }
            str += "end\n";
            str = str.Replace("\t", "    ");

            return str;
        }

        /// <summary>
        /// Закрытый конструктор.
        /// </summary>
        private DeviceManager()
        {
            devices = new List<IODevice>();

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

        public IODevice GetDeviceByIndex(int index)
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
                    errorsBuffer += $"В устройстве {device.EPlanName} " +
                        $"отсутствует R_AS_NUMBER.\n ";
                }
                else
                {
                    var ASNumber = new int();
                    bool isNumber = int.TryParse(parameter, out ASNumber);
                    if (isNumber == false)
                    {
                        errorsBuffer += $"В устройстве " +
                            $"{device.EPlanName} некорректно задан параметр " +
                            $"R_AS_NUMBER.\n ";
                    }
                    if (isNumber == true && ASNumber < 1 && ASNumber > 62)
                    {
                        errorsBuffer += $"В устройстве " +
                            $"{device.EPlanName} некорректно задан диапазон " +
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

        private static IODevice cap = new IODevice("Заглушка", "", 0, "", 0);
        private List<IODevice> devices;       ///Устройства проекта.     
        private static DeviceManager instance;  ///Экземпляр класса.
    }
}
