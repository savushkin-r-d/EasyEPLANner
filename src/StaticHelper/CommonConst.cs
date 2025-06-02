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
                .NumberDecimalSeparator = Dot;
            CultureWithDotInsteadComma.NumberFormat
                .NumberGroupSeparator = Dot;
        }

        /// <summary>
        /// Информация о культуре с точкой вместо запятой.
        /// </summary>
        public static CultureInfo CultureWithDotInsteadComma;

        /// <summary>
        /// Константа точки для настройки культуры.
        /// </summary>
        private const string Dot = ".";

        /// <summary>
        /// MatchEvaluator для regular expression,
        /// замена русских букв на английские
        /// </summary>
        private static string RussianToEnglish(Match m)
        {
            switch (m.ToString()[0])
            {
                case 'А':
                    return "A";
                case 'В':
                    return "B";
                case 'С':
                    return "C";
                case 'Е':
                    return "E";
                case 'К':
                    return "K";
                case 'М':
                    return "M";
                case 'Н':
                    return "H";
                case 'Х':
                    return "X";
                case 'Р':
                    return "P";
                case 'О':
                    return "O";
                case 'Т':
                    return "T";
            }

            return m.ToString();
        }

        /// <summary>
        /// Evaluator для замены заглавных русских букв на английские.
        /// </summary>
        public static MatchEvaluator RusAsEngEvaluator = new MatchEvaluator(
            RussianToEnglish);

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
        /// Шаблон проверки IP-адреса.
        /// </summary>
        public const string IPAddressPattern = @"^ *(?<ip>(?:(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?:\.?\b)){4})) *$";

        /// <summary>
        /// Шаблон чтения диапозонов IP-адресов: "ip1 - ip2, ip3 - ip4"
        /// </summary>
        public const string RangeOfIPAddresses = @"(?:^ *| *)(?:(?<ip>(?:(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?:\.?\b)){4}))(?: *-? *?\b)){2}(?: *,| *$)";

        /// <summary>
        /// Шаблон для Lua-названия (без кириллицы, пробелов и спецсимволов)
        /// </summary>
        public const string LuaNamePattern = @"^\D[^\p{IsCyrillic}|\s|\W]*$";

        /// <summary>
        /// Имя конфигурационного файла приложения.
        /// </summary>
        public const string ConfigFileName = "configuration.ini";

        /// <summary>
        /// Заглушка-триггер для параметров в дополнительных свойствах операции.
        /// </summary>
        public const string StubForCells = "Нет";

        /// <summary>
        /// Edit-значение некоторых незаполненных полей в дереве тех.объектов
        /// </summary>
        public const string EmptyValue = "-1"; 
    }
}
