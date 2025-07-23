using System;
using System.Collections.Generic;
using System.Linq;

namespace EplanDevice
{
    /// Типы устройств.
    public enum DeviceType
    {
        NONE = -1, ///< Тип не определен.

        V = 0, ///< Клапан. 
        VC, ///< Управляемый клапан. 
        M, ///< Двигатель.
        LS, ///< Уровень (есть/нет).
        TE, ///< Температура.        
        FS, ///< Расход (есть/нет).
        GS, ///< Датчик положения. 
        FQT, ///< Счетчик.        
        LT, ///< Уровень (значение).        
        QT, ///< Концентрация.
        HA, ///< Звуковая сигнализация.
        HL, ///< Световая сигнализация.
        SB, ///< Кнопка.
        DI, ///< Дискретный входной сигнал.
        DO, ///< Дискретный выходной сигнал.
        AI, ///< Аналоговый входной сигнал.
        AO, ///< Аналоговый выходной сигнал.
        WT, ///< Датчик веса.
        PT, ///< Датчик давления.
        F, ///< Автоматический выключатель.
        C, ///< ПИД-регулятор
        HLA, ///< Сигнальная колонна.
        CAM, ///< Камера.
        PDS, ///< Сигнальный датчик перепада давления
        TS, ///< Сигнальный датчик температуры
        G, ///< Блок питания с автоматическим выключателем.
        WATCHDOG, ///< Устройство проверки связи
        EY, ///< Преобразователь IO-Link

        // Эти устройства всегда в конце т.к их нет в контроллере.
        Y,       ///< Пневмоостров Festo
        DEV_VTUG,///< Пневмоостров Festo (совместимость со старыми проектами).
    };

    /// <summary>
    /// Методы расширения для <see cref="DeviceType"/>
    /// </summary>
    public static class DeviceTypeExtensions
    {
        /// <summary>
        /// Получить список <see cref="DeviceSubType">подтипов</see> типа
        /// </summary>
        public static IEnumerable<DeviceSubType> SubTypes(this DeviceType type)
            => DSTExt.DeviceSubTypes
            .Where(st => (int)st / DSTExt.TypeMultiplier == (int)type);

        /// <summary>
        /// Получить список названий <see cref="DeviceSubType">подтипов</see> типа
        /// </summary>
        public static IEnumerable<string> SubTypeNames(this DeviceType type) => 
            type.SubTypes().Select(st => st.ToString());


        /// <summary>
        /// Список всех типов
        /// </summary>
        public static List<DeviceType> DeviceTypes => deviceTypes ??=
            [.. Enum.GetValues(typeof(DeviceType)).OfType<DeviceType>()];

        private static List<DeviceType> deviceTypes;
    }

}
