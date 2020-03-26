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
        /// Получить пустой массив операций.
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] BaseEmptyOperations()
        {
            return new BaseOperation[0];
        }

        /// <summary>
        /// Получить операции базового объекта "Линия".
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] LineOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams(),
                WashSteps()),
                new BaseOperation("Наполнение", "FILL", EmptyProperties(), 
                LineFillSteps()),
                new BaseOperation("Выдача", "OUT", EmptyProperties(), 
                LineOutSteps())
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Линия приемки".
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] LineInOperations()
        {
            return new BaseOperation[]
           {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams(),
                WashSteps()),
                new BaseOperation("Наполнение", "FILL", EmptyProperties(),
                LineFillSteps())
           };
        }

        /// <summary>
        /// Получить операции базового объекта "Линия выдачи".
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] LineOutOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams(),
                WashSteps()),
                new BaseOperation("Выдача", "OUT", EmptyProperties(),
                LineOutSteps())
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Танк"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] TankOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams(),
                WashSteps()),
                new BaseOperation("Наполнение", "FILL", FillParams(), 
                EmptyProperties()),
                new BaseOperation("Хранение", "STORING", EmptyProperties(), 
                EmptyProperties()),
                new BaseOperation("Выдача", "OUT", EmptyProperties(), 
                EmptyProperties()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Узел охлаждения"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] CoolerNodeOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Охлаждение", "COOLING", EmptyProperties(), 
                EmptyProperties()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Узел охлаждения ПИД"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] CoolerNodePIDOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Охлаждение", "COOLING", EmptyProperties(), 
                EmptyProperties()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Бачок ледяной воды"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] WaterTankOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Охлаждение", "COOLING", CoolingParams(), 
                EmptyProperties()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Узел давления ПИД"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] PressureNodePIDOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Работа", "WORKING", EmptyProperties(),
                EmptyProperties()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Узел нагрева ПИД"
        /// </summary>
        public static BaseOperation[] HeaterNodePIDOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Нагрев", "HEATING", EmptyProperties(),
                EmptyProperties())
            };
        }
    }
}
