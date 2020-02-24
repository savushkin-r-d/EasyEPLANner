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
        /// Запустить поток модуля простоя приложения
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
        /// Закрыть приложение.
        /// </summary>
        public static void CloseApplication()
        {
            Process eplanProcess = Process.GetCurrentProcess();
            var isClosed = eplanProcess.CloseMainWindow();
            if (isClosed == false)
            {
                var project = EProjectManager.GetInstance().GetCurrentPrj();
                if (project != null)
                {
                    EProjectManager.GetInstance().SyncAndSave();
                    Thread.Sleep(500);
                    project.Close();
                }
                Thread.Sleep(500);
                eplanProcess.Kill();          
            }
            else
            {
                eplanProcess.Close();
            }
        }

        /// <summary>
        /// Запустить модуль
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
        /// Показать форму с таймером и запустить таймер
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
        private const uint MaxIdleTime = 60 * 60 * 1000;

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
