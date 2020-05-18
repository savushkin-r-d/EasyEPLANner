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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("IN_DRAINAGE", "В дренаж", false),
                new NonShowedBaseParameter("IN_TANK", "В приемник", false),
                new NonShowedBaseParameter("WAITING_KEY", "Ожидание", false)
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("OUT_WATER", "Проталкивание", false),
                new NonShowedBaseParameter("OUT_TANK", "Из источника", false),
                new NonShowedBaseParameter("WAITING_KEY", "Ожидание", false)
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("WAITING", "Ожидание", false),
                new NonShowedBaseParameter("OUT_WATER", "Проталкивание", false),
                new NonShowedBaseParameter("OUT_TANK", "Из источника", false),
                new NonShowedBaseParameter("IN_TANK", "В приемник", false),
                new NonShowedBaseParameter("IN_DRAINAGE", "В дренаж", false),
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("DRAINAGE", "Дренаж", false)
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("WAITING_HI_LS", 
                "Ожидание появления ВУ", false),
                new NonShowedBaseParameter("WAITING_LOW_LS", 
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("WORKING", "Работа", false),
                new NonShowedBaseParameter("WAITING ", "Ожидание", false),
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("WORKING", "Работа", false),
                new NonShowedBaseParameter("WAITING ", "Ожидание", false),
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
                new NonShowedBaseParameter("","", false),
                new NonShowedBaseParameter("TO_START_TEMPERATURE",
                "Нагрев до заданной температуры"),
                new NonShowedBaseParameter("SLOW_HEAT", "Нагрев заданное время")
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
                 new NonShowedBaseParameter("", "", false),
                 new NonShowedBaseParameter("WAITING_LOW_LS", 
                 "Ожидание пропадания нижнего уровня"),
                 new NonShowedBaseParameter("WATER_2_LOW_LS", 
                 "Наполнение до нижнего уровня"),
                 new NonShowedBaseParameter("WATER_2_HI_LS", 
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
                new NonShowedBaseParameter("", "", false),
                new NonShowedBaseParameter("WAIT", "Ожидание"),
                new NonShowedBaseParameter("IN_TANK", "В танк"),
                new NonShowedBaseParameter("OUT_TANK", "Из танка"),
            };
        }
    }
}
