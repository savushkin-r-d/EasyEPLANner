using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticHelper
{
    public interface IDeviceEplanFunction
    {
        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Подтип (Доп. поле [2])
        /// </summary>
        string SubType { get; set; }

        /// <summary>
        /// Параметры (Доп поле [3])
        /// </summary>
        string Parameters { get; set; }

        /// <summary>
        /// Свойства (Доп поле [4])
        /// </summary>
        string Properties { get; set; }

        /// <summary>
        /// Свойства (Доп поле [5])
        /// </summary>
        string RuntimeParameters { get; set; }

        /// <summary>
        /// Старое название устройства (импорт из ICP CON, Доп. поле [10])
        /// </summary>
        string OldDeviceName {  get; set; }

        /// <summary>
        /// Изделие
        /// </summary>
        string Article { get; }

        /// <summary>
        /// Функция существует на ФСА
        /// </summary>
        bool IsValid {get;}
    }
}
