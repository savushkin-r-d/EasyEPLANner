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
        /// Пустые шаги операции.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] EmptySteps()
        {
            return new BaseParameter[0];
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия приемки" для операции "Наполнение"
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] LineFillSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("IN_DRAINAGE", "В дренаж", false),
                new ActiveParameter("IN_TANK", "В приемник", false),
                new ActiveParameter("WAITING_KEY", "Ожидание", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия выдачи" для операции "Выдача";
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] LineOutSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("OUT_WATER", "Проталкивание", false),
                new ActiveParameter("OUT_TANK", "Из источника", false),
                new ActiveParameter("WAITING_KEY", "Ожидание", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия выдачи", "Линия приемки" для операции
        /// "Работа"
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] LineWorkSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("WAITING", "Ожидание", false),
                new ActiveParameter("OUT_WATER", "Проталкивание", false),
                new ActiveParameter("OUT_TANK", "Из источника", false),
                new ActiveParameter("IN_TANK", "В приемник", false),
                new ActiveParameter("IN_DRAINAGE", "В дренаж", false),
            };
        }

        /// <summary>
        /// Шаги объектов "Танк", "Линия приемки", "Линия", "Линия выдачи" для
        /// операции мойка.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] WashSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("DRAINAGE", "Дренаж", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Бачок лёдводы", "Бачок лёдводы ПИД" для операции
        /// охлаждения.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] WaterTankSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("WAITING_HI_LS", 
                "Ожидание появления ВУ", false),
                new ActiveParameter("WAITING_LOW_LS", 
                "Ожидание пропадания НУ", false),
            };
        }

        /// <summary>
        /// Шаги для операции Подогрев объекта "Узел нагрева"
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] HeaterNodeHeatingSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("WORKING", "Работа", false),
                new ActiveParameter("WAITING ", "Ожидание", false),
            };
        }

        /// <summary>
        /// Шаги для операции Охлаждение.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] CoolingSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("WORKING", "Работа", false),
                new ActiveParameter("WAITING ", "Ожидание", false),
            };
        }

        /// <summary>
        /// Шаги для операции Томление.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] SlowHeatSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("","", false),
                new ActiveParameter("TO_START_TEMPERATURE",
                "Нагрев до заданной температуры"),
                new ActiveParameter("SLOW_HEAT", "Нагрев заданное время")
            };
        }

        /// <summary>
        /// Шаги для операции "Нагрев" объекта "Бойлер".
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] BoilerHeatingSteps()
        {
            return new BaseParameter[] 
            {
                 new ActiveParameter("", "", false),
                 new ActiveParameter("WAITING_LOW_LS", 
                 "Ожидание пропадания нижнего уровня"),
                 new ActiveParameter("WATER_2_LOW_LS", 
                 "Наполнение до нижнего уровня"),
                 new ActiveParameter("WATER_2_HI_LS", 
                 "Наполнение до верхнего уровня"),
            };          
        }
      
        /// Шаги для базовой операции "Работа" базового объекта "Танк".
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] TankWorkSteps()
        {
            return new BaseParameter[]
            {
                new ActiveParameter("", "", false),
                new ActiveParameter("WAIT", "Ожидание"),
                new ActiveParameter("IN_TANK", "В танк"),
                new ActiveParameter("OUT_TANK", "Из танка"),
            };
        }
    }
}
