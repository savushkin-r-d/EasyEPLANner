using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace DataBase
{
    public partial class Imitation
    {
        /// <summary>
        /// Получить пустой массив параметров.
        /// </summary>
        /// <returns></returns>
        public static List<BaseParameter> EmptyParams()
        {
            return new List<BaseParameter>();
        }

        /// <summary>
        /// Получить параметры операции "Мойка" для танков.
        /// </summary>
        /// <returns></returns>
        private static List<BaseParameter> TankWashParams()
        {
            var parameters = new List<BaseParameter>
            {
                new ActiveParameter("CIP_WASH_END", "Мойка завершена"),
                new ActiveParameter("DI_CIP_FREE", "МСА свободна"),
                new PassiveParameter("DRAINAGE", "Номер шага дренаж"),
                new ActiveParameter("CIP_WASH_REQUEST", 
                "Автоматическое включение мойки"),
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Мойка" для линий.
        /// </summary>
        /// <returns></returns>
        private static List<BaseParameter> LineWashParams()
        {
            var parameters = new List<BaseParameter>
            {
                new ActiveParameter("CIP_WASH_END", "Мойка завершена"),
                new ActiveParameter("CIP_WASH_REQUEST",
                "Автоматическое включение мойки"),
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Мойка" для пастеризатора.
        /// </summary>
        /// <returns></returns>
        private static List<BaseParameter> POUWashParams()
        {
            var parameters = new List<BaseParameter>
            {
                new ActiveParameter("CIP_WASH_END", "Мойка завершена"),
                new PassiveParameter("DRAINAGE", "Номер шага дренаж"),
                new ActiveParameter("CIP_WASH_REQUEST",
                "Автоматическое включение мойки"),
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Наполнение".
        /// </summary>
        /// <returns></returns>
        private static List<BaseParameter> FillParams()
        {
            var parameters = new List<BaseParameter>
            {
                new ActiveParameter("OPERATION_AFTER_FILL",
                "Номер операции после наполнения")
            };

            return parameters;
        }

        /// <summary>
        /// Параметры операции "Выдача"
        /// </summary>
        /// <returns></returns>
        private static List<BaseParameter> OutParams()
        {
            var parameters = new List<BaseParameter>
            {
                new ActiveBoolParameter("NEED_STORING_AFTER", 
                "Включить хранение после выдачи", "true"),
            };

            return parameters;
        }

        /// <summary>
        /// Получить параметры операции "Охлаждение"
        /// </summary>
        /// <returns></returns>
        private static List<BaseParameter> CoolingParams()
        {
            var parameters = new List<BaseParameter>
            {
                new ActiveBoolParameter("ACTIVE_WORKING", "Активная работа", "false")
            };

            return parameters;
        }
    }
}
