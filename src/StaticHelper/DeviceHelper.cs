using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using System.Text.RegularExpressions;

namespace StaticHelper
{
    public interface IDeviceHelper
    {
        /// <summary>
        /// Получить имя изделия устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetArticleName(Function function);

        /// <summary>
        /// Получить описание устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetDescription(Function function);

        /// <summary>
        /// Получить свойства устройства для IOL-Conf.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetIolConfProperties(Function function);

        /// <summary>
        /// Получить номер шкафа, где располагается устройство.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        int GetLocation(Function function);

        /// <summary>
        /// Получить имя устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetName(Function function);

        /// <summary>
        /// Получить параметры устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetParameters(Function function);

        /// <summary>
        /// Установить параметры устройства
        /// </summary>
        /// <param name="function">Eplan-функция устройства</param>
        /// <param name="value">Параметры</param>
        void SetParameters(Function function, string value);

        /// <summary>
        /// Получить свойства устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetProperties(Function function);

        /// <summary>
        /// Установить свойства устройства
        /// </summary>
        /// <param name="function">Eplan-функция устройства</param>
        /// <param name="value">Свойства</param>
        void SetProperties(Function function, string value);

        /// <summary>
        /// Получить параметры времени выполнения.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetRuntimeParameters(Function function);

        /// <summary>
        /// Получить подтип устройства.
        /// </summary>
        /// <param name="function">Функция устройства</param>
        /// <returns></returns>
        string GetSubType(Function function);
    }

    public class DeviceHelper : IDeviceHelper
    {
        IApiHelper apiHelper;

        public DeviceHelper(IApiHelper apiHelper)
        {
            this.apiHelper = apiHelper;
        }

        public string GetArticleName(Function function)
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

        public string GetName(Function function)
        {
            var name = function.Name;
            name = Regex.Replace(name, CommonConst.RusAsEngPattern,
                    CommonConst.RusAsEngEvaluator);
            return name;
        }

        public string GetDescription(Function function)
        {
            var description = string.Empty;
            string descriptionPattern = "([\'\"])";

            if (!function.Properties.FUNC_COMMENT.IsEmpty)
            {
                description = function.Properties.FUNC_COMMENT
                    .ToString(ISOCode.Language.L___);

                if (description == string.Empty)
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

        public string GetSubType(Function function)
        {
            int subTypeIndex = 2;
            return apiHelper.GetSupplementaryFieldValue(function, subTypeIndex);
        }

        public string GetParameters(Function function)
        {
            int parametersIndex = 3;
            return apiHelper.GetSupplementaryFieldValue(function, parametersIndex);
        }

        public void SetParameters(Function function, string value)
        {
            apiHelper.SetSupplementaryFieldValue(function, 3, value);
        }

        public string GetProperties(Function function)
        {
            int propertiesIndex = 4;
            return apiHelper.GetSupplementaryFieldValue(function, propertiesIndex);
        }

        public void SetProperties(Function function, string value)
        {
            apiHelper.SetSupplementaryFieldValue(function, 4, value);
        }

        public string GetRuntimeParameters(Function function)
        {
            int runtimeParametersIndex = 5;
            return apiHelper.GetSupplementaryFieldValue(function, runtimeParametersIndex);
        }

        public int GetLocation(Function function)
        {
            int locationIndex = 6;
            var locationString = apiHelper.GetSupplementaryFieldValue(function, locationIndex);
            bool parsed = int.TryParse(locationString, out int deviceLocation);
            if (!parsed) deviceLocation = 0;

            return deviceLocation;
        }

        public string GetIolConfProperties(Function function)
        {
            int iolConfPropertiesIndex = 7;
            return apiHelper.GetSupplementaryFieldValue(function, iolConfPropertiesIndex);
        }
    }
}
