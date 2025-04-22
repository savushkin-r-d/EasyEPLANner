using EasyEPlanner.Extensions;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticHelper
{
    /// <summary>
    /// Обертка для <see cref="Function">функции</see> на ФСА
    /// </summary>
    public interface IEplanFunction
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
    }

    /// <summary>
    /// <inheritdoc cref="IEplanFunction"/>
    /// </summary>
    /// <param name="function"></param>
    [ExcludeFromCodeCoverage]
    public class EplanFunction(Function function) : IEplanFunction
    {
        public Function Function => function;

        public string IP 
        {
            get => function.Properties.FUNC_PLCGROUP_STARTADDRESS.GetString();
            set => function.Properties.FUNC_PLCGROUP_STARTADDRESS = value;
        }

        public string SubnetMask 
        { 
            get => function.Properties.FUNC_PLC_SUBNETMASK.GetString(); 
            set => function.Properties.FUNC_PLC_SUBNETMASK = value;
        }

        public string Gateway 
        { 
            get => GetSupplemenataryField(15); 
            set => SetSupplementaryField(15, value); 
        }

        public int ClampNumber => int.TryParse(
            function.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.GetString(),
            out int clamp) ? clamp : -1;

        public string VisibleName => function.VisibleName;

        public IEnumerable<IEplanFunction> SubFunctions => function.SubFunctions.Select(f => new EplanFunction(f));

        public string FunctionalText 
        { 
            get => function.GetFunctionalText();
            set => function.Properties.FUNC_TEXT = value;
        }

        public string Name => function.Name;

        public bool PlacedOnCircuit => function.Page.PageType is DocumentTypeManager.DocumentType.Circuit;

        public string GetSupplemenataryField(int propertyIndex)
            => function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex].GetString();

        public void SetSupplementaryField(int propertyIndex, string value)
        {
            try
            {
                function.LockObject();
            }
            catch 
            {
                //do nothing
            }
            
            function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex] = value;
        }

        public override bool Equals(object other) 
            => function == (other as EplanFunction)?.Function;

        public override int GetHashCode() 
            => function.GetHashCode();

        public bool IsMainFunction => function.IsMainFunction;

        public bool Expanded 
        {
            get => bool.TryParse(GetSupplemenataryField(13), out var res) && res;
            set => SetSupplementaryField(13, value.ToString());
        }
    }
}
