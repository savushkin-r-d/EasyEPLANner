using System.Collections.Generic;

namespace IO
{
    /// <summary>
    /// Все узлы модулей ввода-вывода IO. Содержит минимальную функциональность, 
    /// необходимую для экспорта для PAC.
    /// </summary>
    public interface IIOManager
    {
        /// <summary>
        /// Узлы ввода вывода
        /// </summary>
        List<IIONode> IONodes { get; }

        /// <summary>
        /// Расчет IO-Link адресов привязанных устройств для всех модулей
        /// ввода-вывода.
        /// </summary>
        void CalculateIOLinkAdresses();
    }
}
