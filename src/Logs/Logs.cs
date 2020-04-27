namespace EasyEPlanner
{
    public static class Logs
    {
        /// <summary>
        /// Инициализация формы логов.
        /// </summary>
        /// <param name="logForm"></param>
        public static void Init(ILog logForm)
        {
            log = logForm;
        }

        /// <summary>
        /// Добавление сообщения в лог.
        /// </summary>
        public static void AddMessage(string msg)
        {
            log.AddMessage(msg);
        }

        /// <summary>
        /// Установление прогресса.
        /// </summary>
        public static void SetProgress(int msg)
        {
            log.SetProgress(msg);
        }

        /// <summary>
        /// Отображение окна сообщений лога.
        /// </summary>
        public static void Show()
        {
            log.ShowLog();
        }

        /// <summary>
        /// Скрыть окно логов.
        /// </summary>
        public static void Hide()
        {
            log.HideLog();
        }

        /// <summary>
        /// Прокрутить логи до последней строки.
        /// </summary>
        public static void ShowLastLine()
        {
            log.ShowLastLine();
        }

        /// <summary>
        /// Отключить кнопки на форме логов.
        /// </summary>
        public static void DisableButtons()
        {
            log.DisableOkButton();
        }

        /// <summary>
        /// Включить кнопки на форме логов.
        /// </summary>
        public static void EnableButtons()
        {
            log.EnableOkButton();
        }

        /// <summary>
        /// Проверить лог на null.
        /// </summary>
        /// <returns></returns>
        public static bool IsNull()
        {
            return log.IsNull();
        }

        /// <summary>
        /// Проверить окно логов на пустоту.
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty()
        {
            return log.IsEmpty();
        }

        /// <summary>
        /// Очистить окно логов.
        /// </summary>
        public static void Clear()
        {
            log.Clear();
        }

        private static ILog log;
    }
}
