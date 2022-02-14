using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using System.Text.RegularExpressions;

namespace StaticHelper
{
    /// <summary>
    /// Обертка над Eplan API
    /// </summary>
    public static class ApiHelper
    {
        /// <summary>
        /// Получить выборку из выбранных объектов в Eplan API
        /// </summary>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet()
        {
            var selection = new SelectionSet();
            selection.LockSelectionByDefault = false;
            return selection;
        }

        /// <summary>
        /// Получить выбранный на графической схеме объект
        /// </summary>
        /// <returns>Выбранный на схеме объект</returns>
        public static StorableObject GetSelectedObject()
        {
            SelectionSet selection = GetSelectionSet();
            const bool isFirstObject = true;
            StorableObject selectedObject = selection.
                GetSelectedObject(isFirstObject);
            return selectedObject;
        }

        /// <summary>
        /// Получить функциональный текст из функции
        /// </summary>
        /// <param name="function">Функция</param>
        /// <returns></returns>
        public static string GetFunctionalText(Function function)
        {
            string functionalText = function.Properties.FUNC_TEXT_AUTOMATIC
                .ToString(ISOCode.Language.L___);

            if (string.IsNullOrEmpty(functionalText))
            {
                functionalText = function.Properties.FUNC_TEXT_AUTOMATIC
                    .ToString(ISOCode.Language.L_ru_RU);
            }
            if (string.IsNullOrEmpty(functionalText))
            {
                functionalText = function.Properties.FUNC_TEXT
                    .ToString(ISOCode.Language.L_ru_RU);
            }
            if (string.IsNullOrEmpty(functionalText))
            {
                return "";
            }

            return functionalText;
        }

        public static string GetSupplementaryFieldValue(Function function, int propertyIndex)
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
