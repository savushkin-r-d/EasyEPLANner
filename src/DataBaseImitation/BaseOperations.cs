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
                new BaseOperation("","", EmptyProperties()),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams()),
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
                new BaseOperation("", "", EmptyProperties()),
                new BaseOperation("Мойка", "WASHING_CIP", WashParams()),
                new BaseOperation("Наполнение", "FILL", FillParams()),
                new BaseOperation("Хранение", "STORING", EmptyProperties()),
                new BaseOperation("Выдача", "OUT", EmptyProperties()),
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
                new BaseOperation("", "", EmptyProperties()),
                new BaseOperation("Охлаждение", "COOLING", EmptyProperties()),
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
                new BaseOperation("", "", EmptyProperties()),
                new BaseOperation("Охлаждение", "COOLING", EmptyProperties()),
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
                new BaseOperation("", "", EmptyProperties()),
                new BaseOperation("Охлаждение", "COOLING", EmptyProperties()),
            };
        }
    }
}
