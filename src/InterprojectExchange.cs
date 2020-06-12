using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner
{
    /// <summary>
    /// Межконтроллерный обмен сигналами. Главный класс
    /// </summary>
    public class InterprojectExchange
    {
        /// <summary>
        /// Начало настройки межконтроллерного обмена.
        /// </summary>
        public static void Start()
        {
            var instance = GetInstance();
            instance.ReadSignals();
            instance.LoadCurrentInterprojectExchange();
            instance.ShowForm();
        }

        /// <summary>
        /// Загрузка текущих сигналов.
        /// </summary>
        private void ReadSignals()
        {
            //TODO: обновить список сигналов проекта, считать их для настройки.
        }

        /// <summary>
        /// Загрузить текущие данные по межпроектному обмену сигналами.
        /// </summary>
        private void LoadCurrentInterprojectExchange()
        {
            //TODO: чтение данных по проекту и перекрестных данных
        }

        /// <summary>
        /// Показать форму для работы с межпроектным обменом.
        /// </summary>
        private void ShowForm()
        {
            //TODO: Форма с загруженными данными
            form = new InterprojectExchangeForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Получить экземпляр класса. Singleton
        /// </summary>
        /// <returns></returns>
        public static InterprojectExchange GetInstance()
        {
            if (interprojectExchange == null)
            {
                interprojectExchange = new InterprojectExchange();
            }

            return interprojectExchange;
        }

        private InterprojectExchangeForm form;
        private static InterprojectExchange interprojectExchange;
    }
}
