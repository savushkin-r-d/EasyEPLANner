using EasyEPlanner.Extensions;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
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
            var propertyValue = string.Empty;

            if (!function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex].IsEmpty)
            {
                propertyValue = function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex]
                    .ToString(ISOCode.Language.L___);

                if (propertyValue == string.Empty)
                {
                    propertyValue = function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex]
                        .ToString(ISOCode.Language.L_ru_RU);
                };

                propertyValue = Regex.Replace(propertyValue,
                    CommonConst.RusAsEngPattern, CommonConst.RusAsEngEvaluator);
            }

            if (propertyValue == null)
            {
                propertyValue = string.Empty;
            }

            return propertyValue;
        }
    }
}
