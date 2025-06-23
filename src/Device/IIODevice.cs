using IO;
using StaticHelper;
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

        /// <summary>
        /// Проверка устройства на разрешенный тип
        /// </summary>
        /// <param name="allowed"> Массив разрешенных типов </param>
        bool AllowedType(params DeviceType[] allowed);

        /// <summary>
        /// Проверка устройства на разрешенный подтип
        /// </summary>
        /// <param name="allowed"> Массив разрешенных подтипов </param>
        bool AllowedSubtype(params DeviceSubType[] allowed);

        /// <summary>
        /// Сброс канала ввода\вывода.
        /// </summary>
        /// <param name="addressSpace">Тип адресного пространства канала.
        /// </param>   
        /// <param name="comment">Комментарий к каналу.</param>
        /// <param name="error">Строка с описанием ошибки при возникновении 
        /// таковой.</param>
        bool ClearChannel(IOModuleInfo.ADDRESS_SPACE_TYPE addressSpace,
            string comment, string channelName);

        /// <summary>
        /// Связанная функция на ФСА.        
        /// </summary>
        IEplanFunction Function { get; }

        /// <summary>
        /// Установить значение параметра
        /// </summary>
        /// <param name="name">Название параметра</param>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        string SetParameter(string name, double value);

        /// <summary>
        /// Обновить параметры на ФСА
        /// </summary>
        void UpdateParameters();

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        string SetProperty(string name, object value);

        /// <summary>
        /// Обновить свойства на ФСА
        /// </summary>
        void UpdateProperties();

        /// <summary>
        /// Установить значение рабочего параметра
        /// </summary>
        /// <param name="name">Название рабочего параметра</param>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        string SetRuntimeParameter(string name, double value);

        /// <summary>
        /// Обновить рабочие параметры на ФСА
        /// </summary>
        void UpdateRuntimeParameters();

        /// <summary>
        /// Список свойств устройства,
        /// для которых можно установить несколько значений
        /// </summary>
        List<string> MultipleProperties { get; }
    }
}
