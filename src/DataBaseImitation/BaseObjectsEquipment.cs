using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace DataBase
{
    /// <summary>
    /// Класс, содержащий оборудование для технологических объектов
    /// </summary>
    public partial class Imitation
    {
        /// <summary>
        /// Пустой список с оборудованием.
        /// </summary>
        /// <returns></returns>
        public static BaseParameter[] EmptyEquipment()
        {
            return new BaseParameter[0];
        }

        /// <summary>
        /// Получить список оборудования базового узла перемешивания.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] MixerEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("mix", "Мешалка", "M1"),
                new ShowedBaseParameter("bar", "Датчик решетки люка", "GS2"),
                new ShowedBaseParameter("hatch", "Датчик крышки люка", "GS1"),
                new ShowedBaseParameter("LT", "Датчик текущего уровня", "LT1")
            };
        }

        /// <summary>
        /// Получить список оборудования базового танка.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] TankEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("hatch", "Датчик крышки люка", "GS1"),
                new ShowedBaseParameter("LS_up", "Датчик верхнего уровня", "LS2"),
                new ShowedBaseParameter("LS_down", "Датчик нижнего уровня", "LS1"),
                new ShowedBaseParameter("LT", "Датчик текущего уровня", "LT1"),
                new ShowedBaseParameter("TE","Датчик температуры", "TE1")
            };
        }

        /// <summary>
        /// Получить список оборудования базового узла охлаждения
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] CoolerNodeEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("TE", "Датчик температуры", "TE1")
            };
        }

        /// <summary>
        /// Получить список оборудования базового узла охлаждения ПИД
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] CoolerNodePIDEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("TE", "Датчик температуры", "TE1"),
                new ShowedBaseParameter("VC", "Регулирующий клапан", "VC1"),
                new ShowedBaseParameter("SET_VALUE", "Задание", ""),
            };
        }

        /// <summary>
        /// Получить Список оборудования для бачка откачки ледяной воды.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] WaterTankEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("M1", "Насос", "M1"),
                new ShowedBaseParameter("LS_up", "Датчик верхнего уровня", "LS2"),
                new ShowedBaseParameter("LS_down", "Датчик нижнего уровня", "LS1"),
            };
        }

        /// <summary>
        /// Получить Список оборудования для бачка откачки ледяной воды.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] WaterTankPIDEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("M1", "Насос", "M1"),
                new ShowedBaseParameter("LS_up", "Датчик верхнего уровня", "LS2"),
                new ShowedBaseParameter("LS_down", "Датчик нижнего уровня", "LS1"),
                new ShowedBaseParameter("LT", "Датчик текущего уровня", "LT1"),
                new ShowedBaseParameter("SET_VALUE", "Задание", ""),
            };
        }

        /// <summary>
        /// Получить список оборудования для ПИД узла давления.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] PressureNodePIDEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("M1", "Мотор", "M1"),
                new ShowedBaseParameter("PT", "Датчик давления", "PT1"),
                new ShowedBaseParameter("SET_VALUE", "Задание", ""),
            };
        }

        /// <summary>
        /// Получить список оборудования для ПИД узла нагрева.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] HeaterNodePIDEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("TE", "Датчик температуры", "TE1"),
                new ShowedBaseParameter("VC", "Регулирующий клапан", "VC1"),
                new ShowedBaseParameter("SET_VALUE", "Задание", ""),
            };
        }
      
        /// Получить список оборудования для ПИД узла расхода.
        /// </summary>
        /// <returns></returns>
        static public BaseParameter[] FlowNodePIDEquipment()
        {
            return new BaseParameter[]
            {
                new ShowedBaseParameter("FQT1", "Счетчик", "FQT1"),
                new ShowedBaseParameter("M1", "Насос", "M1"),
                new ShowedBaseParameter("SET_VALUE", "Задание", ""),
            };
        }
    }
}
