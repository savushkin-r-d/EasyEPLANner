using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PInvoke;

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
            PrepareForRunning();
            await Task.Run(RunAfterPreparing);
        }

        /// <summary>
        /// Остановить модуль простоя приложения
        /// </summary>
        public static void Stop()
        {
            //TODO: Остановка модуля
        }

        /// <summary>
        /// Подготовить к запуску модуль простоя приложения
        /// </summary>
        private static void PrepareForRunning()
        {
            //TODO: подготовка модуля к запуску
        }

        /// <summary>
        /// Запуск модуля после подготовки
        /// </summary>
        private static void RunAfterPreparing()
        {
            //TODO: запуск модуля после подготовки
        }

        /// <summary>
        /// Получить время последнего ввода пользователя
        /// </summary>
        /// <returns>Время в секундах</returns>
        static uint GetLastInputTime()
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

            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }
    }
}
