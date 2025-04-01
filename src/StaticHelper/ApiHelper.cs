using EasyEPlanner.Extensions;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using Spire.Xls;
using System.Text.RegularExpressions;

namespace StaticHelper
{
    public interface IApiHelper
    {
        /// <summary>
        /// Получить функциональный текст из функции
        /// </summary>
        /// <param name="function">Функция</param>
        string GetFunctionalText(Function function);

        /// <summary>
        /// Получить выбранный на графической схеме объект
        /// </summary>
        /// <returns>Выбранный на схеме объект</returns>
        StorableObject GetSelectedObject();

        /// <summary>
        /// Получить выборку из выбранных объектов в Eplan API
        /// </summary>
        /// <returns></returns>
        SelectionSet GetSelectionSet();

        /// <summary>
        /// Получить значение свойства функции Доп. поле с номером propertyIndex
        /// </summary>
        /// <param name="function">Функция Eplan</param>
        /// <param name="propertyIndex">Индекс свойства</param>
        /// <returns></returns>
        string GetSupplementaryFieldValue(Function function, int propertyIndex);

        /// <summary>
        /// Установить значение свойства функции Доп.поле
        /// </summary>
        /// <param name="function">функция Eplan</param>
        /// <param name="propertyIndex">Индекс доп.поля</param>
        /// <param name="value">Устанвливаемое значение</param>
        void SetSupplementaryFieldValue(Function function, int propertyIndex,
            string value);
    }

    /// <summary>
    /// Обертка над Eplan API
    /// </summary>
    public class ApiHelper : IApiHelper
    {
        public SelectionSet GetSelectionSet()
        {
            var selection = new SelectionSet();
            selection.LockSelectionByDefault = false;
            return selection;
        }

        public StorableObject GetSelectedObject()
        {
            SelectionSet selection = GetSelectionSet();
            const bool isFirstObject = true;
            StorableObject selectedObject = selection.
                GetSelectedObject(isFirstObject);
            return selectedObject;
        }

        public string GetFunctionalText(Function function)
        {
            return function.GetFunctionalText();
        }

        public string GetSupplementaryFieldValue(Function function, int propertyIndex)
        {
            return function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex].GetString();
        }

        public void SetSupplementaryFieldValue(Function function, int propertyIndex,
            string value)
        {
            function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex] = value;
        }
    }

    public static class ApiHelperExt
    {
        public static string GetString(this PropertyValue value)
        {
            if (value is null || value.IsEmpty)
                return string.Empty;

            var res = value.ToString(ISOCode.Language.L___);

            if (res == string.Empty)
                res = value.ToString(ISOCode.Language.L_ru_RU);

            return res?.Trim() ?? string.Empty;
        }
    }
}
