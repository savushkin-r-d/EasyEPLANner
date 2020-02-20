using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
