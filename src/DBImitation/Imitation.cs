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
        // Возврат базовых технологических объектов
        public static BaseTechObject[] GetBaseTechObjects()
        {
            return BaseTechObjectArr();
        }

        // Имитиация хранимой процедуры поиска базового имени по имени базового 
        //технологического объекта
        public static string GetBasicName(string baseTechObjectName)
        {
            string basicName = "";

            foreach (BaseTechObject baseTechObject in BaseTechObjectArr())
            {
                if (baseTechObject.Name == baseTechObjectName)
                {
                    basicName = baseTechObject.BasicName;
                }
            }

            return basicName;
        }

        // Получение тех. объекта по номеру
        public static BaseTechObject GetTObject(string name)
        {
            foreach (BaseTechObject baseTechObject in BaseTechObjectArr())
            {
                if (name == baseTechObject.Name)
                {
                    return baseTechObject;
                }
            }
            return null;
        }

        //---------------- Empty params ---------------------------------------
        public static BaseProperty[] EmptyProperties()
        {
            return new BaseProperty[0];
        }

        //---------------- Empty operations -----------------------------------
        public static BaseOperation[] BaseEmptyOperations()
        {
            return new BaseOperation[0];
        }

        //---------------- Init objects ---------------------------------------
        public static BaseTechObject[] BaseTechObjectArr()
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
