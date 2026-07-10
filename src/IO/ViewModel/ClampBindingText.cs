using StaticHelper;
using System;
using System.Linq;

namespace IO.ViewModel
{
    /// <summary>
    /// Форматирование описания клеммы с учетом нескольких привязок.
    /// </summary>
    public static class ClampBindingText
    {
        public const string Separator = " || ";

        /// <summary>
        /// Описание клеммы с несколькими привязками для колонки дерева.
        /// </summary>
        public static string FormatForCell(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.Contains(Separator))
                return EplanMultilineText.FormatForCell(text);

            return string.Join(" · ",
                text.Split(new[] { Separator }, StringSplitOptions.None)
                    .Select(EplanMultilineText.FormatForCell));
        }

        /// <summary>
        /// Описание клеммы с несколькими привязками для подсказки.
        /// </summary>
        public static string FormatForTooltip(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.Contains(Separator))
                return EplanMultilineText.FormatForTooltip(text);

            return string.Join(Environment.NewLine,
                text.Split(new[] { Separator }, StringSplitOptions.None)
                    .Select(EplanMultilineText.FormatForTooltip));
        }
    }
}
