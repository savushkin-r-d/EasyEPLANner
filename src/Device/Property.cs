namespace EplanDevice
{
    public partial class IODevice
    {
        /// <summary>
        /// Свойство устройства.
        /// </summary>
        public class Property : ITag
        {
            /// <summary> Связанные моторы. </summary>
            public static readonly Property MT = 
                new(nameof(MT), "Связанные моторы");

            /// <summary> Датчик давления. </summary>
            public static readonly Property PT = 
                new(nameof(PT), "Датчик давления");

            /// <summary> Входное значение (обычно для ПИД-а). </summary>
            public static readonly Property IN_VALUE = 
                new(nameof(IN_VALUE), "Входное значение");

            /// <summary> Выходное значение (обычно для ПИД-а). </summary>
            public static readonly Property OUT_VALUE = 
                new(nameof(OUT_VALUE), "Выходное значение");

            /// <summary> IP-адрес устройства. </summary>
            public static readonly Property IP = 
                new(nameof(IP), "IP-адрес");

            /// <summary> Последовательность сигналов. </summary>
            public static readonly Property SIGNALS_SEQUENCE = 
                new(nameof(SIGNALS_SEQUENCE), "Последовательность сигналов");

            /// <summary> Сигнал DI. </summary>
            public static readonly Property DI_dev = 
                new(nameof(DI_dev), "Сигнал DI");

            /// <summary> Сигнал AI. </summary>
            public static readonly Property AI_dev =
                new(nameof(AI_dev), "Сигнал AI");

            /// <summary> Сигнал DO. </summary>
            public static readonly Property DO_dev =
                new(nameof(DO_dev), "Сигнал DO");

            /// <summary> Сигнал AO. </summary>
            public static readonly Property AO_dev =
                new(nameof(AO_dev), "Сигнал AO");


            /// <summary>
            /// Неявное преобразование параметра в строку с названием
            /// </summary>
            /// <param name="property">Параметр</param>
            public static implicit operator string(Property property) 
                => property.Name;

            private Property(string name, string description)
            {
                Name = name;
                Description = description;
            }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public override string ToString() => Name;
        }
    }
}
