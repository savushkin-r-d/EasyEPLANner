using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PInvoke;
using System.Threading;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс, отвечающий за модуль простоя приложения
    /// </summary>
    public static class IdleModule
    {
        /// <summary>
        /// Запустить модуль простоя приложения
        /// </summary>
        public async static void Start()
        {
            await Task.Run(Run);
        }

        /// <summary>
        /// Остановить модуль простоя приложения
        /// </summary>
        public static void Stop()
        {
            idleTimer.Change(Timeout.Infinite, Timeout.Infinite);
            idleTimer.Dispose();
        }

        /// <summary>
        /// Запуск модуля
        /// </summary>
        private static void Run()
        {
            callbackFunction = new TimerCallback(CheckIdle);
            idleTimer = new Timer(callbackFunction, null, 0, idleInterval);
        }

        /// <summary>
        /// Проверка состояния простоя
        /// </summary>
        /// <param name="obj">Состояние из таймера</param>
        private static void CheckIdle(object obj)
        {
            if (GetLastInputTime() > MaxIdleTime)
            {
                ShowCountdownWindow();
            }
        }

        /// <summary>
        /// Показать окно с обратным отсчетом
        /// </summary>
        private static void ShowCountdownWindow()
        {
            idleForm.Init();
            idleForm.Show();
        }

        /// <summary>
        /// Получить время последнего ввода пользователя
        /// </summary>
        /// <returns>Время в миллисекундах</returns>
        private static uint GetLastInputTime()
        {
            uint idleTime = 0;
            PI.LASTINPUTINFO lastInputInfo = new PI.LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (PI.GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? idleTime : 0);
        }

        /// <summary>
        /// Максимальное время простоя в секундах
        /// </summary>
        private const uint MaxIdleTime = 120000;

        /// <summary>
        /// Интервал проверки простоя в секундах
        /// </summary>
        private const int idleInterval = 10000;

        /// <summary>
        /// Таймер
        /// </summary>
        private static Timer idleTimer;

        /// <summary>
        /// Делегат функции для таймера
        /// </summary>
        private static TimerCallback callbackFunction;

        /// <summary>
        /// Форма с отсчетом
        /// </summary>
        private static IdleForm idleForm = new IdleForm();
    }
}
