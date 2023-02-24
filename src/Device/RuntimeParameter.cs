namespace EplanDevice
{
    public partial class IODevice
    {
        /// <summary>
        /// Параметр времени выполнения устройства.
        /// </summary>
        public class RuntimeParameter
        {
            /// <summary>
            /// Номер клапана на пневмоострове
            /// </summary>
            public const string R_VTUG_NUMBER = "R_VTUG_NUMBER";

            /// <summary>
            /// Размер области клапана для пневмоострова
            /// </summary>
            public const string R_VTUG_SIZE = "R_VTUG_SIZE";

            /// <summary>
            /// Номер клапана в AS-i.
            /// </summary>
            public const string R_AS_NUMBER = "R_AS_NUMBER";

            /// <summary>
            /// Тип красного сигнала устройства при подаче на него сигнала DO. 
            /// (Постоянный или мигающий). 0 - мигающий, 1 - постоянный.
            /// </summary>
            public const string R_CONST_RED = "R_CONST_RED";

            /// <summary>
            /// Номер клеммы пневмоострова для сигнала "Открыть"
            /// </summary>
            public const string R_ID_ON = "R_ID_ON";

            /// <summary>
            /// Номер клеммы пневмоострова для сигнала "Открыть верхнее седло"
            /// </summary>
            public const string R_ID_UPPER_SEAT = "R_ID_UPPER_SEAT";

            /// <summary>
            /// Номер клеммы пневмоострова для сигнала "Открыть нижнее седло"
            /// </summary>
            public const string R_ID_LOWER_SEAT = "R_ID_LOWER_SEAT";


        }
    }
}
