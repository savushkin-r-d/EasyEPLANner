using System.Text.RegularExpressions;

namespace StaticHelper
{
    /// <summary>
    /// Класс содержащий общие константы  для проекта.
    /// </summary>
    public static class CommonConst
    {
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
        public static string ConfigFileName = "configuration.ini";
    }
}
