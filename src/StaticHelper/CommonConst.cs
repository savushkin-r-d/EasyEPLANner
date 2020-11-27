using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace StaticHelper
{
    /// <summary>
    /// Класс содержащий общие константы  для проекта.
    /// </summary>
    public static class CommonConst
    {
        static CommonConst()
        {
            CultureWithDotInsteadComma = (CultureInfo)Thread.CurrentThread
               .CurrentCulture.Clone();
            CultureWithDotInsteadComma.NumberFormat
                .NumberDecimalSeparator = Comma;
            CultureWithDotInsteadComma.NumberFormat
                .NumberGroupSeparator = Comma;
        }

        /// <summary>
        /// Информация о культуре с точкой вместо запятой.
        /// </summary>
        public static CultureInfo CultureWithDotInsteadComma;

        /// <summary>
        /// Константа точки для настройки культуры.
        /// </summary>
        private const string Comma = ".";

        /// <summary>
        /// Evaluator для замены заглавных русских букв на английские.
        /// </summary>
        public static MatchEvaluator RusAsEnsEvaluator = new MatchEvaluator(
            ApiHelper.RussianToEnglish);

        /// <summary>
        /// Шаблон для поиска русских букв.
        /// </summary>
        public const string RusAsEngPattern = @"[АВСЕКМНХРОТ]";

        /// <summary>
        /// Символ переноса строки.
        /// </summary>
        public const string NewLine = "\n";

        /// <summary>
        /// Символ переноса строки с возвратом каретки.
        /// </summary>
        public const string NewLineWithCarriageReturn = "\r\n";

        /// <summary>
        /// Текст "Резерв".
        /// </summary>
        public const string Reserve = "Резерв";

        /// <summary>
        /// Текст "Заглушка"
        /// </summary>
        public const string Cap = "Заглушка";

        /// <summary>
        /// Шаблон поиска IP-адреса.
        /// </summary>
        public const string IPAddressPattern = @"\b(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";

        /// <summary>
        /// Имя конфигурационного файла приложения.
        /// </summary>
        public const string ConfigFileName = "configuration.ini";

        /// <summary>
        /// Заглушка-триггер для параметров в дополнительных свойствах операции.
        /// </summary>
        public const string StubForCells = "Нет";
    }
}
