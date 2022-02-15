namespace EplanDevice
{
    public partial class IODevice
    {
        public interface IIOChannel
        {
            /// <summary>
            /// Логический номер клеммы (порядковый)
            /// </summary>
            int LogicalClamp { get; }

            /// <summary>
            /// Имя канала (DI,DO, AI,AO)
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Комментарий
            /// </summary>
            string Comment { get; }

            /// <summary>
            /// Сдвиг начала модуля
            /// </summary>
            int ModuleOffset { get; }

            /// <summary>
            /// Полный номер модуля
            /// </summary>
            int FullModule { get; }
        }
    }
}
