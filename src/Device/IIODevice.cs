using System.Collections.Generic;

namespace EplanDevice
{
    public interface IIODevice : IDevice
    {
        /// <summary>
        /// IOL-Conf свойства в устройстве.
        /// </summary>
        Dictionary<string, double> IolConfProperties { get; }

        /// <summary>
        /// Свойство содержащее изделие, которое используется для устройства
        /// </summary>
        string ArticleName { get; set; }

        /// <summary>
        /// IO-Link свойства устройства
        /// </summary>
        IODevice.IOLinkSize IOLinkProperties { get; }

        /// <summary>
        /// Установить свойство IOL-Conf в устройстве (переопределить в шаблоне)
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="value">Устанавливаемое значение</param>
        void SetIolConfProperty(string propertyName, double value);
    }
}
