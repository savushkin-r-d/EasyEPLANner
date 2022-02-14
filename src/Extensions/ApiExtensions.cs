using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;

namespace EasyEPlanner.Extensions
{
    public static class ApiExtensions
    {
        public static string GetFunctionalText(this Function function)
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
                return string.Empty;
            }

            return functionalText;
        }
    }
}
