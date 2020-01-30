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
        // Базовые операции
        private static BaseOperation[] baseOperationsList()
        {
            return new BaseOperation[]
            {
                new BaseOperation("", ""),
                new BaseOperation("Мойка", "WASHING_CIP"),
                new BaseOperation("Наполнение", "luaName1"),
                new BaseOperation("Хранение", "luaName2"),
                new BaseOperation("Выдача", "luaName3"),
            };
        }

        // Возврат базовых операций
        public static BaseOperation[] GetBaseOperations()
        {
            return baseOperationsList();
        }

        // Возврат базовых технологических объектов
        public static BaseTechObject[] GetBaseTechObjects()
        {
            return BaseTechObjectArr();
        }

        // Имитиация хранимой процедуры поиска ОУ по имени базового 
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

        // Поиск Lua имени операции
        public static string FindOperationLuaName(string name)
        {
            var luaName = "";
            luaName = baseOperationsList().First(x => x.GetName()
            .Contains(name)).GetLuaName();
            return luaName;
        }

        // Возврат параметров базовой операции по имени из БД
        public static BaseProperty[] GetOperParams(
            string baseOperName, string baseObjectName)
        {
            BaseTechObject currObj = BaseTechObjectArr()
                .Where(x => x.Name.Equals(baseObjectName)).FirstOrDefault();
            BaseOperation currOper = currObj.BaseOperations
                .Where(x => x.GetName().Equals(baseOperName)).FirstOrDefault();
            
            if (currOper == null) 
            {
                return new BaseProperty[0];
            } 

            BaseProperty[] operationParams = currOper.BaseOperationProperties;

            if (operationParams == null)
            {
                return new BaseProperty[0];
            }

            return operationParams;
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
