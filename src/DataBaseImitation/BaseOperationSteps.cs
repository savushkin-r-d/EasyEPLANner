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
        public static BaseProperty[] EmptySteps()
        {
            return new BaseProperty[0];
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия приемки" для операции "Наполнение"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineFillSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("IN_DRAINAGE", "В дренаж", false),
                new NonShowedBaseProperty("IN_TANK", "В приемник", false),
                new NonShowedBaseProperty("WAITING_KEY", "Ожидание", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия выдачи" для операции "Выдача";
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineOutSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("OUT_WATER", "Проталкивание", false),
                new NonShowedBaseProperty("OUT_TANK", "Из источника", false),
                new NonShowedBaseProperty("WAITING_KEY", "Ожидание", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Линия", "Линия выдачи", "Линия приемки" для операции
        /// "Работа"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] LineWorkSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("WAITING", "Ожидание", false),
                new NonShowedBaseProperty("OUT_WATER", "Проталкивание", false),
                new NonShowedBaseProperty("OUT_TANK", "Из источника", false),
                new NonShowedBaseProperty("IN_TANK", "В приемник", false),
                new NonShowedBaseProperty("IN_DRAINAGE", "В дренаж", false),
            };
        }

        /// <summary>
        /// Шаги объектов "Танк", "Линия приемки", "Линия", "Линия выдачи" для
        /// операции мойка.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] WashSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("DRAINAGE", "Дренаж", false)
            };
        }

        /// <summary>
        /// Шаги объектов "Бачок лёдводы", "Бачок лёдводы ПИД" для операции
        /// охлаждения.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] WaterTankSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("WAITING_HI_LS", 
                "Ожидание появления ВУ", false),
                new NonShowedBaseProperty("WAITING_LOW_LS", 
                "Ожидание пропадания НУ", false),
            };
        }

        /// <summary>
        /// Шаги для операции Подогрев объекта "Узел нагрева"
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] HeaterNodeHeatingSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("WORKING", "Работа", false),
                new NonShowedBaseProperty("WAITING ", "Ожидание", false),
            };
        }

        /// <summary>
        /// Шаги для операции Охлаждение.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] CoolingSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("WORKING", "Работа", false),
                new NonShowedBaseProperty("WAITING ", "Ожидание", false),
            };
        }

        /// <summary>
        /// Шаги для операции Томление.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] SlowHeatSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("","", false),
                new NonShowedBaseProperty("TO_START_TEMPERATURE",
                "Нагрев до заданной температуры"),
                new NonShowedBaseProperty("SLOW_HEAT", "Нагрев заданное время")
            };
        }

        /// <summary>
        /// Шаги для операции "Нагрев" объекта "Бойлер".
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] BoilerHeatingSteps()
        {
            return new BasseProperty[] 
            {
                 new NonShowedBaseProperty("", "", false),
                 new NonShowedBaseProperty("WAITING_LOW_LS", 
                 "Ожидание пропадания нижнего уровня"),
                 new NonShowedBaseProperty("WATER_2_LOW_LS", 
                 "Наполнение до нижнего уровня"),
                 new NonShowedBaseProperty("WATER_2_HI_LS", 
                 "Наполнение до верхнего уровня"),
            };          
        }
      
        /// Шаги для базовой операции "Работа" базового объекта "Танк".
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] TankWorkSteps()
        {
            return new BaseProperty[]
            {
                new NonShowedBaseProperty("", "", false),
                new NonShowedBaseProperty("WAIT", "Ожидание"),
                new NonShowedBaseProperty("IN_TANK", "В танк"),
                new NonShowedBaseProperty("OUT_TANK", "Из танка"),
            };
        }
    }
}
