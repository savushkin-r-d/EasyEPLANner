using System;
using System.Runtime.InteropServices;
using PInvoke;
using System.Threading;
using System.Diagnostics;

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
        public static void Start()
        {
            idleThread = new Thread(Run);
            idleThread.Start();
        }

        /// <summary>
        /// Остановить модуль простоя приложения
        /// </summary>
        public static void Stop()
        {
            isRunning = false;
        }

        /// <summary>
        /// Закрытие приложения
        /// </summary>
        public static void CloseApplication()
        {
            EProjectManager.GetInstance().GetCurrentPrj().Close();
            Process eplanProcess = Process.GetCurrentProcess();
            eplanProcess.CloseMainWindow();
        }

        /// <summary>
        /// Запуск модуля
        /// </summary>
        private static void Run()
        {
            isRunning = true;
            while (isRunning)
            {
                CheckIdle();
                Thread.Sleep(idleInterval);
            }
        }

        /// <summary>
        /// Проверка состояния простоя
        /// </summary>
        private static void CheckIdle()
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
            IdleForm.Form.Show();
            IdleForm.Form.RunCountdown();
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
        /// Максимальное время простоя в миллисекундах
        /// </summary>
        private const uint MaxIdleTime = 3600000;

        /// <summary>
        /// Интервал проверки простоя в миллисекундах
        /// </summary>
        private const int idleInterval = 60000;

        /// <summary>
        /// Флаг запуска потока.
        /// </summary>
        private static bool isRunning = true;

        /// <summary>
        /// Поток модуля простоя
        /// </summary>
        private static Thread idleThread;
    }
}
