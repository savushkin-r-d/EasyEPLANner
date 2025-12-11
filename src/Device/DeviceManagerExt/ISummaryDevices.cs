using System.Collections.Generic;

namespace EplanDevice
{
    /// <summary>
    /// Сводка устройств по проекту
    /// </summary>
    public interface ISummaryDevices
    {
        /// <summary>
        /// Получить количество использованных устройств по типам и подтипам
        /// </summary>
        Dictionary<string, Dictionary<string, int>> NumberUsedTypes();
    }
}