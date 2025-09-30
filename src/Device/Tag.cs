namespace EplanDevice
{
    public partial class IODevice
    {
        /// <summary>
        /// Тэг устройства.
        /// </summary>
        public class Tag : ITag
        {
            /// <summary> Состояние </summary>
            public static readonly Tag ST = new Tag(nameof(ST), "Состояние");

            /// <summary> Ручной режим. </summary>
            public static readonly Tag M = new Tag(nameof(M), "Ручной режим");

            /// <summary> Аналоговое значение. </summary>
            public static readonly Tag V = new Tag(nameof(V), "Аналоговое значение");

            /// <summary> Сдвиг нуля. </summary>
            public static readonly Tag P_CZ = new Tag(nameof(P_CZ), "Сдвиг нуля");

            /// <summary> Индикация местонахождения устройства. </summary>
            public static readonly Tag BLINK = new Tag(nameof(BLINK), "Индикация местонахождения устройства");

            /// <summary> Состояние по стандарту NAMUR. </summary>
            public static readonly Tag NAMUR_ST = new Tag(nameof(NAMUR_ST), "Состояние по стандарту NAMUR");

            /// <summary> Открыт. </summary>
            public static readonly Tag OPENED = new Tag(nameof(OPENED), "Открыт");

            /// <summary> Закрыт. </summary>
            public static readonly Tag CLOSED = new Tag(nameof(CLOSED), "Закрыт");

            /// <summary> Текущее состояние обратной связи (на отключенное состояние). </summary>
            public static readonly Tag FB_OFF_ST = new Tag(nameof(FB_OFF_ST), "Текущее состояние обратной связи (на отключенное состояние)");

            /// <summary> Текущее состояние обратной связи (на включенное состояние). </summary>
            public static readonly Tag FB_ON_ST = new Tag(nameof(FB_ON_ST), "Текущее состояние обратной связи (на включенное состояние)");

            /// <summary> Сигнал управления. </summary>
            public static readonly Tag CS = new Tag(nameof(CS), "Сигнал управления");

            /// <summary> Ошибка. </summary>
            public static readonly Tag ERR = new Tag(nameof(ERR), "Ошибка");

            /// <summary> Температура. </summary>
            public static readonly Tag T = new Tag(nameof(T), "Температура");

            /// <summary> Проверка устройства. </summary>
            public static readonly Tag OK = new Tag(nameof(OK), "Проверка устройства");

            /// <summary> Реверс (обычно мотор). </summary>
            public static readonly Tag R = new Tag(nameof(R), "Реверс");

            /// <summary> Частота (обычно мотор). </summary>
            public static readonly Tag FRQ = new Tag(nameof(FRQ), "Частота");

            /// <summary> Обороты в минуту (обычно мотор). </summary>
            public static readonly Tag RPM = new Tag(nameof(RPM), "Обороты в минуту");

            /// <summary> Расширенное состояние (обычно мотор). </summary>
            public static readonly Tag EST = new Tag(nameof(EST), "Расширенное состояние");

            /// <summary> Пересчитанный уровень (обычно в уровне). </summary>
            public static readonly Tag CLEVEL = new Tag(nameof(CLEVEL), "Пересчитанный уровень");

            /// <summary> Состояние голубой лампочки. </summary>
            public static readonly Tag L_BLUE = new Tag(nameof(L_BLUE), "Состояние голубой лампочки");

            /// <summary> Состояние красной лампочки. </summary>
            public static readonly Tag L_RED = new Tag(nameof(L_RED), "Состояние красной лампочки");

            /// <summary> Состояние желтой лампочки. </summary>
            public static readonly Tag L_YELLOW = new Tag(nameof(L_YELLOW), "Состояние желтой лампочки");

            /// <summary> Состояние зеленой лампочки. </summary>
            public static readonly Tag L_GREEN = new Tag(nameof(L_GREEN), "Состояние зеленой лампочки");

            /// <summary> Состояние сирены. </summary>
            public static readonly Tag L_SIREN = new Tag(nameof(L_SIREN), "Состояние сирены");

            /// <summary> Абсолютное значение. </summary>
            public static readonly Tag ABS_V = new Tag(nameof(ABS_V), "Абсолютное значение");

            /// <summary> Минимальное значение потока. </summary>
            public static readonly Tag P_MIN_FLOW = new Tag(nameof(P_MIN_FLOW), "Минимальное значение потока");

            /// <summary> Максимальное значение потока. </summary>
            public static readonly Tag P_MAX_FLOW = new Tag(nameof(P_MAX_FLOW), "Максимальное значение потока");

            /// <summary> Расход. </summary>
            public static readonly Tag F = new Tag(nameof(F), "Расход");

            /// <summary> Состояние канала. </summary>
            public static readonly Tag ST_CH = new Tag(nameof(ST_CH), "Состояние канала");

            /// <summary> Заданный ток канала. </summary>
            public static readonly Tag NOMINAL_CURRENT_CH = new Tag(nameof(NOMINAL_CURRENT_CH), "Заданный ток канала");

            /// <summary> Текущий ток канала. </summary>
            public static readonly Tag LOAD_CURRENT_CH = new Tag(nameof(LOAD_CURRENT_CH), "Текущий ток канала");

            /// <summary> Авария канала. </summary>
            public static readonly Tag ERR_CH = new Tag(nameof(ERR_CH), "Авария канала");

            /// <summary> Задание. </summary>
            public static readonly Tag Z = new Tag(nameof(Z), "Задание");

            /// <summary> Результат обработки. </summary>
            public static readonly Tag RESULT = new Tag(nameof(RESULT), "Результат обработки");

            /// <summary> Готовность. </summary>
            public static readonly Tag READY = new Tag(nameof(READY), "Готовность");

            /// <summary> Суммарный ток. </summary>
            public static readonly Tag SUM_CURRENTS = new Tag(nameof(SUM_CURRENTS), "Суммарный ток");

            /// <summary> Напряжение. </summary>
            public static readonly Tag VOLTAGE = new Tag(nameof(VOLTAGE), "Напряжение");

            /// <summary> Превышение 90% нагрузки. </summary>
            public static readonly Tag OUT_POWER_90 = new Tag(nameof(OUT_POWER_90), "Превышение 90% нагрузки");

            /// <summary> Сегодня, счетчик 1. </summary>
            public static readonly Tag DAY_T1 = new Tag(nameof(DAY_T1), "Сегодня, счетчик 1");

            /// <summary> Вчера, счетчик 1. </summary>
            public static readonly Tag PREV_DAY_T1 = new Tag(nameof(PREV_DAY_T1), "Вчера, счетчик 1");

            /// <summary> Сегодня, счетчик 2. </summary>
            public static readonly Tag DAY_T2 = new Tag(nameof(DAY_T2), "Сегодня, счетчик 2");

            /// <summary> Вчера, счетчик 2. </summary>
            public static readonly Tag PREV_DAY_T2 = new Tag(nameof(PREV_DAY_T2), "Вчера, счетчик 2");


            /// <summary>
            /// Неявное преобразование параметра в строку с названием
            /// </summary>
            /// <param name="tag">Параметр</param>
            public static implicit operator string(Tag tag) => tag.Name;

            public override string ToString() => Name;

            private Tag(string name, string description)
            {
                Name = name;
                Description = description;
            }

            public string Name { get; private set; }

            public string Description { get; private set; }
        }
    }
}
