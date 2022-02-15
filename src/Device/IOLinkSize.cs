namespace EplanDevice
{
    public partial class IODevice
    {
        /// <summary>
        /// Размер IO-Link областей устройства ввода-вывода.
        /// </summary>
        public class IOLinkSize
        {
            public IOLinkSize()
            {
                SizeIn = 0;
                SizeOut = 0;
            }

            /// <summary>
            /// Возвращает максимальны размер байтовой области для модулей ввода
            /// вывода при расчете IO-Link адресов если используется
            /// Phoenix Contact
            /// </summary>
            /// <returns></returns>
            public int GetMaxIOLinkSize()
            {
                return SizeOut > SizeIn ? SizeOut : SizeIn;
            }

            /// <summary>
            /// Размер области входа приведенный к слову (целому)
            /// </summary>
            public int SizeIn { get; set; }

            /// <summary>
            /// Размер области выхода приведенный к слову (целому)
            /// </summary>
            public int SizeOut { get; set; }

            /// <summary>
            /// Размер области входа из файла
            /// </summary>
            public float SizeInFromFile { get; set; }

            /// <summary>
            /// Размер области выхода из файла
            /// </summary>
            public float SizeOutFromFile { get; set; }
        }
    }
}
