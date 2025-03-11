namespace EplanDevice
{
    public partial class IODevice
    {
        /// <summary>
        /// Свойство устройства.
        /// </summary>
        public class Property : ITag
        {
            private readonly string name;
            private readonly string description;

            /// <summary> Связанные моторы. </summary>
            public static readonly Property MT = new Property(nameof(MT), "Связанные моторы");

            /// <summary> Датчик давления. </summary>
            public static readonly Property PT = new Property(nameof(PT), "Датчик давления");

            /// <summary> Входное значение (обычно для ПИД-а). </summary>
            public static readonly Property IN_VALUE = new Property(nameof(IN_VALUE), "Входное значение");

            /// <summary> Выходное значение (обычно для ПИД-а). </summary>
            public static readonly Property OUT_VALUE = new Property(nameof(OUT_VALUE), "Выходное значение");

            /// <summary> IP-адрес устройства. </summary>
            public static readonly Property IP = new Property(nameof(IP), "IP-адрес");

            /// <summary> Последовательность сигналов. </summary>
            public static readonly Property SIGNALS_SEQUENCE = new Property(nameof(SIGNALS_SEQUENCE), "Последовательность сигналов");


            /// <summary>
            /// Неявное преобразование параметра в строку с названием
            /// </summary>
            /// <param name="property">Параметр</param>
            public static implicit operator string(Property property) => property.name;

            private Property(string name, string description)
            {
                this.name = name;
                this.description = description;
            }

            public string Name => name;

            public string Description => description;
        }
    }
}
