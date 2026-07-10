using EasyEPlanner;
using IO.ViewModel;
using StaticHelper;

namespace IO
{
    /// <summary>
    /// Применение функционального текста клеммы из редактора структуры ПЛК.
    /// </summary>
    public static class ClampFunctionalTextBinder
    {
        /// <summary>
        /// Обновляет FUNC_TEXT клеммы и пересчитывает привязку по тексту.
        /// </summary>
        /// <returns>Текст изменился и привязка обновлена.</returns>
        public static bool TryApply(IClamp clamp, string newText)
        {
            if (clamp.ClampFunction is not EplanFunction eplanClamp ||
                eplanClamp.Function is null ||
                clamp.Module.Info is not IOModuleInfo moduleInfo)
            {
                return false;
            }

            string currentText = eplanClamp.FunctionalText ?? string.Empty;
            if (EplanMultilineText.IsSameFunctionalText(currentText, newText))
            {
                return false;
            }

            ClampBindingUpdater.ApplyFunctionalText(
                eplanClamp,
                moduleInfo,
                currentText,
                newText,
                clamp.Reset);

            return true;
        }
    }
}
