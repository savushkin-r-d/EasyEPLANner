namespace Device
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
            /// Номинальная нагрузка.
            /// </summary>
            public const string P_NOMINAL_W = "P_NOMINAL_W";

            /// <summary>
            /// Время порогового фильтра (дельта).
            /// </summary>
            public const string P_DT = "P_DT";

            /// <summary>
            /// Рабочий коэффициент передачи.
            /// </summary>
            public const string P_RKP = "P_RKP";

            /// <summary>
            /// Сдвиг нуля.
            /// </summary>
            public const string P_CZ = "P_CZ";

            /// <summary>
            /// Световая индикация устройства.
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
            /// Время включения.
            /// </summary>
            public const string P_ON_TIME = "P_ON_TIME";

            /// <summary>
            /// Включение/выключение обратной связи.
            /// </summary>
            public const string P_FB = "P_FB";

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
            /// Минимальное значение.
            /// </summary>
            public const string P_MIN_V = "P_MIN_V";

            /// <summary>
            /// Максимальное значение.
            /// </summary>
            public const string P_MAX_V = "P_MAX_V";

            /// <summary>
            /// Температура.
            /// </summary>
            public const string T = "T";

            /// <summary>
            /// Проверка устройства.
            /// </summary>
            public const string OK = "OK";

            /// <summary>
            /// Значение ошибки (значение во время ошибки датчика).
            /// </summary>
            public const string P_ERR = "P_ERR";

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
            /// Высота конусной части танка.
            /// </summary>
            public const string P_H_CONE = "P_H_CONE";

            /// <summary>
            /// Максимальное давление.
            /// </summary>
            public const string P_MAX_P = "P_MAX_P";

            /// <summary>
            /// Радиус танка.
            /// </summary>
            public const string P_R = "P_R";

            /// <summary>
            /// Пересчитанный уровень (обычно в уровне).
            /// </summary>
            public const string CLEVEL = "CLEVEL";

            /// <summary>
            /// Высота усеченной части танка.
            /// </summary>
            public const string P_H_TRUNC = "P_H_TRUNC";

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
