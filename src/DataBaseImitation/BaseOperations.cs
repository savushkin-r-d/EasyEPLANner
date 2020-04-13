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
                EmptySteps()),
                new BaseOperation("Хранение", "STORING", EmptyProperties(),
                EmptySteps()),
                new BaseOperation("Выдача", "OUT", OutParams(),
                EmptySteps()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Узел охлаждения" и
        /// "Узел охлаждения ПИД"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] CoolerNodeOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Охлаждение", "COOLING", EmptyProperties(),
                CoolingSteps()),
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
                WaterTankSteps()),
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
                EmptySteps()),
            };
        }

        /// <summary>
        /// Получить операции базового объекта "Узел нагрева ПИД" и
        /// "Узел нагрева"
        /// </summary>
        public static BaseOperation[] HeaterNodeOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Нагрев", "HEATING", EmptyProperties(),
                HeatingSteps()),
            };
        }
              
        /// Получить операции базового объекта "Узел расхода ПИД"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] FlowNodePIDOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Работа", "WORKING", EmptyProperties(),
                EmptySteps()),
            };
        }

        /// <summary>
        /// Операции базового объекта "Пастеризатор"
        /// </summary>
        /// <returns></returns>
        public static BaseOperation[] POUOperations()
        {
            return new BaseOperation[]
            {
                BaseOperation.EmptyOperation(),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams(),
                WashSteps())
            };
        }
    }
}
