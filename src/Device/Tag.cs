namespace EplanDevice
{
    public partial class IODevice
    {
        /// <summary>
        /// Тэг устройства.
        /// </summary>
        public class Tag
        {
            /// <summary>
            /// Состояние.
            /// </summary>
            public const string ST = "ST";

            /// <summary>
            /// Ручной режим.
            /// </summary>
            public const string M = "M";

            /// <summary>
            /// Аналоговое значение.
            /// </summary>
            public const string V = "V";

            /// <summary>
            /// Сдвиг нуля.
            /// </summary>
            public const string P_CZ = "P_CZ";

            /// <summary>
            /// Индикация местонахождения устройства.
            /// </summary>
            public const string BLINK = "BLINK";

            /// <summary>
            /// Состояние по стандарту NAMUR.
            /// </summary>
            public const string NAMUR_ST = "NAMUR_ST";

            /// <summary>
            /// Открыт.
            /// </summary>
            public const string OPENED = "OPENED";

            /// <summary>
            /// Закрыт.
            /// </summary>
            public const string CLOSED = "CLOSED";

            /// <summary>
            /// Текущее состояние обратной связи (на отключенное состояние).
            /// </summary>
            public const string FB_OFF_ST = "FB_OFF_ST";

            /// <summary>
            /// Текущее состояние обратной связи (на включенное состояние).
            /// </summary>
            public const string FB_ON_ST = "FB_ON_ST";

            /// <summary>
            /// Сигнал управления.
            /// </summary>
            public const string CS = "CS";

            /// <summary>
            /// Ошибка.
            /// </summary>
            public const string ERR = "ERR";

            /// <summary>
            /// Температура.
            /// </summary>
            public const string T = "T";

            /// <summary>
            /// Проверка устройства.
            /// </summary>
            public const string OK = "OK";

            /// <summary>
            /// Реверс (обычно мотор).
            /// </summary>
            public const string R = "R";

            /// <summary>
            /// Частота (обычно мотор).
            /// </summary>
            public const string FRQ = "FRQ";

            /// <summary>
            /// Обороты в минуту (обычно мотор).
            /// </summary>
            public const string RPM = "RPM";

            /// <summary>
            /// Расширенное состояние (обычно мотор).
            /// </summary>
            public const string EST = "EST";

            /// <summary>
            /// Пересчитанный уровень (обычно в уровне).
            /// </summary>
            public const string CLEVEL = "CLEVEL";

            /// <summary>
            /// Состояние голубой лампочки.
            /// </summary>
            public const string L_BLUE = "L_BLUE";

            /// <summary>
            /// Состояние красной лампочки.
            /// </summary>
            public const string L_RED = "L_RED";

            /// <summary>
            /// Состояние желтой лампочки.
            /// </summary>
            public const string L_YELLOW = "L_YELLOW";

            /// <summary>
            /// Состояние зеленой лампочки.
            /// </summary>
            public const string L_GREEN = "L_GREEN";

            /// <summary>
            /// Состояние сирены.
            /// </summary>
            public const string L_SIREN = "L_SIREN";

            /// <summary>
            /// Абсолютное значение.
            /// </summary>
            public const string ABS_V = "ABS_V";

            /// <summary>
            /// Минимальное значение потока.
            /// </summary>
            public const string P_MIN_FLOW = "P_MIN_FLOW";

            /// <summary>
            /// Максимальное значение потока.
            /// </summary>
            public const string P_MAX_FLOW = "P_MAX_FLOW";

            /// <summary>
            /// Расход.
            /// </summary>
            public const string F = "F";

            /// <summary>
            /// Состояние канала.
            /// </summary>
            public const string ST_CH = "ST_CH";

            /// <summary>
            /// Заданный ток канала.
            /// </summary>
            public const string NOMINAL_CURRENT_CH = "NOMINAL_CURRENT_CH";

            /// <summary>
            /// Текущий ток канала.
            /// </summary>
            public const string LOAD_CURRENT_CH = "LOAD_CURRENT_CH";

            /// <summary>
            /// Авария канала.
            /// </summary>
            public const string ERR_CH = "ERR_CH";

            /// <summary>
            /// Задание.
            /// </summary>
            public const string Z = "Z";

            /// <summary>
            /// Результат обработки.
            /// </summary>
            public const string RESULT = "RESULT";

            /// <summary>
            /// Готовность.
            /// </summary>
            public const string READY = "READY";

            /// <summary>
            /// Суммарный ток.
            /// </summary>
            public const string SUM_CURRENTS = "SUM_CURRENTS";

            /// <summary>
            /// Напряжение.
            /// </summary>
            public const string VOLTAGE = "VOLTAGE";

            /// <summary>
            /// Превышение 90% нагрузки.
            /// </summary>
            public const string OUT_POWER_90 = "OUT_POWER_90";

        }
    }
}
