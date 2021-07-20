namespace Device
{
    public partial class IODevice
    {
        /// <summary>
        /// Параметр устройства.
        /// </summary>
        public class Parameter
        {
            /// <summary>
            /// Номинальная нагрузка в кг.
            /// </summary>
            public const string P_NOMINAL_W = "P_NOMINAL_W";

            /// <summary>
            /// Рабочий коэффициент передачи
            /// </summary>
            public const string P_RKP = "P_RKP";

            /// <summary>
            /// Сдвиг нуля.
            /// </summary>
            public const string P_C0 = "P_C0";

            /// <summary>
            /// Время порогового фильтра.
            /// </summary>
            public const string P_DT = "P_DT";

            /// <summary>
            /// Время включения.
            /// </summary>
            public const string P_ON_TIME = "P_ON_TIME";

            /// <summary>
            /// Обратная связь, 1/0 (Да/Нет)
            /// </summary>
            public const string P_FB = "P_FB";

            /// <summary>
            /// Аварийное значение.
            /// </summary>
            public const string P_ERR = "P_ERR";

            /// <summary>
            /// Минимальное значение.
            /// </summary>
            public const string P_MIN_V = "P_MIN_V";

            /// <summary>
            /// Максимальное значение.
            /// </summary>
            public const string P_MAX_V = "P_MAX_V";

            /// <summary>
            /// Давление, на которое настроен датчик.
            /// </summary>
            public const string P_MAX_P = "P_MAX_P";

            /// <summary>
            /// Радиус танка.
            /// </summary>
            public const string P_R = "P_R";

            /// <summary>
            /// Высота конической части танка.
            /// </summary>
            public const string P_H_CONE = "P_H_CONE";

            /// <summary>
            /// Высота усеченной части танка.
            /// </summary>
            public const string P_H_TRUNC = "P_H_TRUNC";

            /// <summary>
            /// Минимальное значение для потока.
            /// </summary>
            public const string P_MIN_F = "P_MIN_F";

            /// <summary>
            /// Максимальное значение для потока.
            /// </summary>
            public const string P_MAX_F = "P_MAX_F";

            /// <summary>
            /// Параметр k.
            /// </summary>
            public const string P_k = "P_k";

            /// <summary>
            /// Параметр Ti.
            /// </summary>
            public const string P_Ti = "P_Ti";

            /// <summary>
            /// Параметр Td.
            /// </summary>
            public const string P_Td = "P_Td";

            /// <summary>
            /// Интервал расчёта.
            /// </summary>
            public const string P_dt = "P_dt";

            /// <summary>
            /// Максимальное значение входной величины.
            /// </summary>
            public const string P_max = "P_max";

            /// <summary>
            /// Минимальное значение входной величины.
            /// </summary>
            public const string P_min = "P_min";

            /// <summary>
            /// Время выхода на режим регулирования.
            /// </summary>
            public const string P_acceleration_time = "P_acceleration_time";

            /// <summary>
            /// Ручной режим, 0 - авто, 1 - ручной.
            /// </summary>
            public const string P_is_manual_mode = "P_is_manual_mode";

            /// <summary>
            /// Заданное ручное значение выходного сигнала.
            /// </summary>
            public const string P_U_manual = "P_U_manual";

            /// <summary>
            /// Параметр k2.
            /// </summary>
            public const string P_k2 = "P_k2";

            /// <summary>
            /// Параметр Ti2.
            /// </summary>
            public const string P_Ti2 = "P_Ti2";

            /// <summary>
            /// Параметр Td2.
            /// </summary>
            public const string P_Td2 = "P_Td2";

            /// <summary>
            /// Максимальное значение выходной величины.
            /// </summary>
            public const string P_out_max = "P_out_max";

            /// <summary>
            /// Минимальное значение выходной величины.
            /// </summary>
            public const string P_out_min = "P_out_min";

            /// <summary>
            /// Обратного (реверсивного) действия, 0 - false, 1 - true.
            /// </summary>
            public const string P_is_reverse = "P_is_reverse";

            /// <summary>
            /// Нулевое стартовое значение, 0 - false, 1 - true.
            /// </summary>
            public const string P_is_zero_start = "P_is_zero_start";

            /// <summary>
            /// Диаметр вала, м.
            /// </summary>
            public const string P_SHAFT_DIAMETER = "P_SHAFT_DIAMETER";

            /// <summary>
            /// Передаточное число
            /// </summary>
            public const string P_TRANSFER_RATIO = "P_TRANSFER_RATIO";
        }
    }
}
