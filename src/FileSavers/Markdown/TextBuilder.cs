using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner.FileSavers.Markdown
{
    /// <summary>
    /// Конструктор для текста
    /// </summary>
    public interface ITextBuilder 
    {
        /// <summary>
        /// Установить отступ для конструктора
        /// </summary>
        /// <param name="offset">Отступ в конструкторе для всех строк</param>
        ITextBuilder WithOffset(string offset);

        /// <summary>
        /// Добавить строки в конструктор
        /// </summary>
        /// <param name="lines">Строки</param>
        ITextBuilder Lines(params object[] lines);

        /// <summary>
        /// Получить строки конструктора с применением параметров
        /// </summary>
        /// <returns>Коллекция строк конструктора</returns>
        IEnumerable<string> Build();

        /// <summary>
        /// Результат конструктора (агрегация строк в текст)
        /// </summary>
        /// <param name="separator">Разделитель строк (по-умолчанию '\n')</param>
        /// <returns></returns>
        string Result(string separator = "\n");
    }

    /// <summary>
    /// Конструктор для текста
    /// </summary>
    public class TextBuilder : ITextBuilder
    {
        /// <summary>
        /// Строки текста
        /// </summary>
        private List<string> lines = new List<string>();

        /// <summary>
        /// Отступ
        /// </summary>
        private string offset = string.Empty;

        public TextBuilder()
        {

        }

        /// <summary>
        /// Конструктор с инициализацией первой строки
        /// </summary>
        /// <param name="initLine">Первая строка</param>
        public TextBuilder(string initLine)
        {
            lines.Add(initLine);
        }

        public ITextBuilder Lines(params object[] values)
        {

            foreach (var value in values)
            {
                switch (value)
                {
                    case string line:
                        lines.Add(line);
                        break;

                    case IEnumerable<string> enumerable:
                        lines.AddRange(enumerable);
                        break;

                    case ITextBuilder lineBuilder:
                        lines.AddRange(lineBuilder.Build());
                        break;

                    case IEnumerable<ITextBuilder> lineBuilders:
                        foreach(var lineBuilder in lineBuilders)
                        {
                            lines.AddRange(lineBuilder.Build());
                        }
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Применить отступ к строкам
        /// </summary>
        private void Appplyoffset()
        {
            if (string.IsNullOrEmpty(offset))
                return;

            lines = (from line in lines
                     select $"{offset}{line}")
                     .ToList();
        }

        public IEnumerable<string> Build()
        {
            Appplyoffset();

            return lines;
        }

        public string Result(string separator = "\n")
        {
            if (lines.Count == 0)
                return "";

            Appplyoffset();

            return string.Join(separator, lines) + separator; 
        }


        public ITextBuilder WithOffset(string prefix)
        {
            this.offset = prefix;
            return this;
        }
    }
}
