using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticHelper
{
    public static class SuppField
    {
        /// <summary>
        /// Флаг, указывающий активность устройства для чтения с ФСА
        /// </summary>
        public static readonly int Off = 1;

        /// <summary>
        /// Подтип
        /// </summary>
        public static readonly int Subtype = 2;

        /// <summary>
        /// Параметры
        /// </summary>
        public static readonly int Parameters = 3;

        /// <summary>
        /// Свойства
        /// </summary>
        public static readonly int Properties = 4;

        /// <summary>
        /// Рабочие параметры
        /// </summary>
        public static readonly int RuntimeParameters = 5;

        /// <summary>
        /// Старое название устройства в wago.dsx
        /// </summary>
        public static readonly int OldDeviceName = 10;

        /// <summary>
        /// Состояние развернутости модулей для окна
        /// <see cref="IO.View.IOViewControl">Структура ПЛК</see>>
        /// </summary>
        public static readonly int Expanded = 13;

        /// <summary>
        /// Сетевой шлюз
        /// </summary>
        public static readonly int Gateway = 15;
    }
}
