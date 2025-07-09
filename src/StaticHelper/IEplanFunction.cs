using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StaticHelper
{
    /// <summary>
    /// Обертка для <see cref="Function">функции</see> на ФСА
    /// </summary>
    public interface IEplanFunction : IDeviceEplanFunction
    {
        /// <summary>
        /// Поле "Начальный адрес карты ПЛК".
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// Доп.поле [<paramref name="propertyIndex"/>].
        /// </summary>
        /// <param name="propertyIndex">Номер доп. поля</param>
        /// <returns>Содержимое доп. поля</returns>
        string GetSupplemenataryField(int propertyIndex);

        /// <summary>
        /// Установить значение в Доп.поле [<paramref name="propertyIndex"/>].
        /// </summary>
        /// <param name="propertyIndex">Номер доп. поля</param>
        /// <param name="value">Новое значение</param>
        void SetSupplementaryField(int propertyIndex, string value);

        /// <summary>
        /// Сетевой шлюз.
        /// </summary>
        string Gateway { get; set; }

        /// <summary>
        /// Маска подсети.
        /// </summary>
        string SubnetMask { get; set; }

        /// <summary>
        /// Номер клеммы.
        /// </summary>
        int ClampNumber { get; }

        /// <summary>
        /// Видимое название.
        /// </summary>
        string VisibleName { get; }

        /// <summary>
        /// Название.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Вложенные функции.
        /// </summary>
        IEnumerable<IEplanFunction> SubFunctions { get; }

        /// <summary>
        /// Функциональный текст.
        /// </summary>
        string FunctionalText { get; set; }

        /// <summary>
        /// Размещен на схеме.
        /// </summary>
        bool PlacedOnCircuit { get; }

        /// <summary>
        /// Главная функция.
        /// </summary>
        bool IsMainFunction {  get; }

        /// <summary>
        /// Состояние <see cref="IO.ViewModel.IExpandable">развертуности</see> 
        /// на <see cref="IO.View.IOViewControl"/>.
        /// </summary>
        /// <remarks>
        /// reserved Доп. поле [13]
        /// </remarks>
        bool Expanded { get; set; }

        /// <summary>
        /// Функция выключена (Доп. поле [1] == 1)
        /// </summary>
        bool Off { get; set; }

        /// <summary>
        /// Заблокировать функцию на ФСА для редактирования
        /// </summary>
        void Lock();
    }
}
