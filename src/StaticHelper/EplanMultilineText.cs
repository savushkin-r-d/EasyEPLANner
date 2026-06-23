using System;
using System.Linq;
using System.Text;

namespace StaticHelper
{
    /// <summary>
    /// Отображение многострочного текста EPLAN (FUNC_TEXT / FUNC_COMMENT: \n, ¶) в ячейках дерева.
    /// </summary>
    public static class EplanMultilineText
    {
        private static readonly char[] LineSeparators =
        {
            '\r', '\n', '\u00B6', '\u2028', '\u2029',
        };

        /// <summary>
        /// Текст для колонки дерева (одна строка, переносы видны как « · »).
        /// </summary>
        public static string FormatForCell(string text)
        {
            var parts = SplitLines(text);
            if (parts.Length == 0)
                return string.Empty;
            if (parts.Length == 1)
                return parts[0];
            return string.Join(" · ", parts);
        }

        /// <summary>
        /// Текст подсказки с переносами строк.
        /// </summary>
        public static string FormatForTooltip(string text)
        {
            var parts = SplitLines(text);
            if (parts.Length == 0)
                return string.Empty;
            return string.Join(Environment.NewLine, parts);
        }

        /// <summary>
        /// Текст для многострочного редактора (¶ и \n -> перенос строки).
        /// </summary>
        public static string FormatForEditor(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var sb = new StringBuilder(text.Length);
            int index = 0;
            while (index < text.Length)
            {
                char c = text[index];
                if (c == '\r')
                {
                    index++;
                    if (index < text.Length && text[index] == '\n')
                        index++;
                    sb.AppendLine();
                }
                else if (IsLineSeparator(c))
                {
                    index++;
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(c);
                    index++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Текст из редактора -> формат EPLAN (переносы как ¶).
        /// </summary>
        public static string ParseFromEditor(string text)
        {
            return ParseFromEditor(text, "\u00B6");
        }

        /// <summary>
        /// Текст из редактора -> указанный формат переноса строк.
        /// </summary>
        public static string ParseFromEditor(string text, string lineSeparator)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var sb = new StringBuilder(text.Length);
            int index = 0;
            while (index < text.Length)
            {
                char c = text[index];
                if (c == '\r')
                {
                    index++;
                    if (index < text.Length && text[index] == '\n')
                        index++;
                    sb.Append(lineSeparator);
                }
                else if (IsLineSeparator(c))
                {
                    index++;
                    sb.Append(lineSeparator);
                }
                else
                {
                    sb.Append(c);
                    index++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Сравнивает FUNC_TEXT с учетом эквивалентных переносов строк.
        /// </summary>
        public static bool IsSameFunctionalText(string stored, string edited)
        {
            if (stored == edited)
                return true;

            return NormalizeFunctionalText(stored) ==
                NormalizeFunctionalText(edited);
        }

        private static string NormalizeFunctionalText(string text)
        {
            return ParseFromEditor(
                FormatForEditor(text ?? string.Empty),
                CommonConst.NewLineWithCarriageReturn);
        }

        private static bool IsLineSeparator(char c) =>
            c != '\r' && LineSeparators.Contains(c);

        private static string[] SplitLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<string>();

            return text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => part.Length > 0)
                .ToArray();
        }
    }
}
