namespace Device
{
    public partial class IODevice
    {
        /// <summary>
        /// Свойство устройства.
        /// </summary>
        public class Property
        {
            /// <summary>
            /// Связанные моторы.
            /// </summary>
            public const string MT = "MT";

            /// <summary>
            /// Датчик давления.
            /// </summary>
            public const string PT = "PT";

            /// <summary>
            /// Входное значение (обычно для ПИД-а).
            /// </summary>
            public const string IN_VALUE = "IN_VALUE";

            /// <summary>
            /// Выходное значение (обычно для ПИД-а).
            /// </summary>
            public const string OUT_VALUE = "OUT_VALUE";

            /// <summary>
            /// IP-адрес устройства.
            /// </summary>
            public const string IP = "IP";
        }
    }
}
