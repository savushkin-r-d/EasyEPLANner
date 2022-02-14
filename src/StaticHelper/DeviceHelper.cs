using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using System.Text.RegularExpressions;

namespace StaticHelper
{
    public static class DeviceHelper
    {
        /// <summary>
        /// Получить имя изделия устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetArticleName(Function function)
        {
            var articleName = string.Empty;
            if (function == null) return articleName;

            var articlesRefs = function.ArticleReferences;
            if (articlesRefs.Length > 0 &&
                !string.IsNullOrEmpty(function.ArticleReferences[0].PartNr))
            {
                articleName = function.ArticleReferences[0].PartNr;
            }

            return articleName;
        }

        /// <summary>
        /// Получить имя устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetName(Function function)
        {
            var name = function.Name;
            name = Regex.Replace(name, CommonConst.RusAsEngPattern,
                    CommonConst.RusAsEngEvaluator);
            return name;
        }

        /// <summary>
        /// Получить описание устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetDescription(Function function)
        {
            var description = string.Empty;
            string descriptionPattern = "([\'\"])";

            if (!function.Properties.FUNC_COMMENT.IsEmpty)
            {
                description = function.Properties.FUNC_COMMENT
                    .ToString(ISOCode.Language.L___);

                if (description == "")
                {
                    description = function.Properties.FUNC_COMMENT
                        .ToString(ISOCode.Language.L_ru_RU);
                }

                description = Regex.Replace(description,
                    descriptionPattern, string.Empty);
            }

            if (description == null)
            {
                description = string.Empty;
            }

            return description;
        }

        /// <summary>
        /// Получить подтип устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetSubType(Function function)
        {
            int subTypeIndex = 2;
            return ApiHelper.GetSupplementaryFieldValue(function, subTypeIndex);
        }

        /// <summary>
        /// Получить свойства устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetProperties(Function function)
        {
            int propertiesIndex = 4;
            return ApiHelper.GetSupplementaryFieldValue(function, propertiesIndex);
        }

        /// <summary>
        /// Получить параметры времени выполнения.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetRuntimeParameters(Function function)
        {
            int runtimeParametersIndex = 5;
            return ApiHelper.GetSupplementaryFieldValue(function, runtimeParametersIndex);
        }

        /// <summary>
        /// Получить номер шкафа, где располагается устройство.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static int GetLocation(Function function)
        {
            int locationIndex = 6;
            var locationString = ApiHelper.GetSupplementaryFieldValue(function, locationIndex);
            bool parsed = int.TryParse(locationString, out int deviceLocation);
            if (!parsed) deviceLocation = 0;

            return deviceLocation;
        }

        /// <summary>
        /// Получить параметры устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        public static string GetParameters(Function function)
        {
            int parametersIndex = 3;
            return ApiHelper.GetSupplementaryFieldValue(function, parametersIndex);
        }
    }
}
