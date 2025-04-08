using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO.ViewModel
{
    /// <summary>
    /// Перечисление индексов изображений
    /// </summary>
    public enum Icon
    {
        /// <summary> Нет изображения. </summary>
        None = -1,

        /// <summary> Шкаф. </summary>
        Cab = 0,

        /// <summary> Узел. </summary>
        Node,

        /// <summary> Модуль (черный). </summary>
        BlackModule,

        /// <summary> Модуль (серый). </summary>
        GrayModule,

        /// <summary> Модуль (зеленый). </summary>
        GreenModule,

        /// <summary> Модуль (lime). </summary>
        LimeModule,

        /// <summary> Модуль (оранжевый). </summary>
        OrangeModule,

        /// <summary> Модуль (красный). </summary>
        RedModule,

        /// <summary> Модуль (фиолетовый). </summary>
        VioletModule,

        /// <summary> Модуль (желтый). </summary>
        YellowModule,

        /// <summary> Клемма. </summary>
        Clamp,

        /// <summary> Кабель (устройства). </summary>
        Cable,
    }

    /// <summary>
    /// Элемент имеет изображение в основном столбце.
    /// </summary>
    /// <remarks>
    /// Определяется явно.
    /// </remarks>
    public interface IHasIcon
    {
        /// <summary>
        /// Индекс изображения.
        /// </summary>
        Icon Icon { get; }
    }

    /// <summary>
    /// Элемент имеет иконку в столбце описания.
    /// </summary>
    /// <remarks>
    /// Определяется явно.
    /// </remarks>
    public interface IHasDescriptionIcon
    {
        /// <summary>
        /// Индекс изображения.
        /// </summary>
        Icon Icon { get; }
    }
}
