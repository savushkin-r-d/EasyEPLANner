using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

namespace DataBase
{
    /// <summary>
    /// Имитация базы данных.
    /// </summary>
    public partial class Imitation
    {
        /// <summary>
        /// Получить базовое название объекта по его обычному названию.
        /// </summary>
        /// <param name="baseTechObjectName">Название объекта</param>
        /// <returns></returns>
        public static string GetBasicName(string baseTechObjectName)
        {
            string basicName = "";

            foreach (BaseTechObject baseTechObject in BaseTechObjects())
            {
                if (baseTechObject.Name == baseTechObjectName)
                {
                    basicName = baseTechObject.BasicName;
                }
            }

            return basicName;
        }

        /// <summary>
        /// Получить базовый технологический объект по обычному названию.
        /// </summary>
        /// <param name="name">Название объекта</param>
        /// <returns></returns>
        public static BaseTechObject GetTechObject(string name)
        {
            foreach (BaseTechObject baseTechObject in BaseTechObjects())
            {
                if (name == baseTechObject.Name)
                {
                    return baseTechObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Получить пустой массив свойств.
        /// </summary>
        /// <returns></returns>
        public static BaseProperty[] EmptyProperties()
        {
            return new BaseProperty[0];
        }

        /// <summary>
        /// Получить массив всех базовых объектов.
        /// </summary>
        /// <returns></returns>
        public static BaseTechObject[] BaseTechObjects()
        {
            return new BaseTechObject[]
            {
                new BaseTechObject(), // Пустой объект.
                new BaseAutomat(),
                new BaseWaterTank(),
                new BaseBoiler(),
                new BaseMaster(),
                new BaseLine(),
                new BaseLineIn(),
                new BaseLineOut(),
                new BasePOU(),
                new BasePost(),
                new BaseTank(),
                new BaseHeater(),
                new BaseCooler(),
                new BaseMixer()
            };
        }
    }
}
