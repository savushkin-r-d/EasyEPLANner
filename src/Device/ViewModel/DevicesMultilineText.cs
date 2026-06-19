using EplanDevice;
using System;
using System.Linq;
using System.Text;

namespace EasyEPlanner.Devices.ViewModel
{
    /// <summary>
    /// Отображение многострочного текста EPLAN (FUNC_COMMENT: \n, ¶) в ячейках дерева.
    /// </summary>
    public static class DevicesMultilineText
    {
        /// <summary>
        /// Актуальное описание из EPLAN (после правки в форме — из Function).
        /// </summary>
        public static string GetEplanDescription(IODevice device)
        {
            if (device?.Function != null)
                return device.Function.Description ?? string.Empty;

            return device?.Description ?? string.Empty;
        }

        private static readonly char[] LineSeparators =
        {
            '\r', '\n', '\u00B6', '\u2028', '\u2029',
        };

        /// <summary>
        /// Текст для колонки «Значение» (одна строка, разделитель виден в UI).
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
        /// Текст для многострочного редактора (¶ и \n → перенос строки).
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
        /// Текст из редактора → формат EPLAN (переносы как ¶).
        /// </summary>
        public static string ParseFromEditor(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("\r\n", "\u00B6")
                .Replace("\n", "\u00B6")
                .Replace("\r", "\u00B6");
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
