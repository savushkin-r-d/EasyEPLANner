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
    public class Imitation
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
            return baseTechObjectArr();
        }

        // Имитиация хранимой процедуры поиска базового имени по имени базового 
        //технологического объекта
        public static string GetBasicName(string baseTechObjectName)
        {
            string basicName = "";

            foreach (BaseTechObject baseTechObject in baseTechObjectArr())
            {
                if (baseTechObject.GetName() == baseTechObjectName)
                {
                    basicName = baseTechObject.GetBasicName();
                }
            }

            return basicName;
        }

        // Получение тех. объекта по номеру
        public static BaseTechObject GetTObject(string name)
        {
            foreach (BaseTechObject baseTechObject in baseTechObjectArr())
            {
                if (name == baseTechObject.GetName())
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
        public static BaseOperationProperty[] GetOperParams(
            string baseOperName, string baseObjectName)
        {
            BaseTechObject currObj = baseTechObjectArr()
                .FirstOrDefault(x => x.GetName().Equals(baseObjectName));
            if (currObj == null)
            {
                return new BaseOperationProperty[0];
            }

            BaseOperation currOper = currObj.BaseOperations
                .FirstOrDefault(x => x.GetName().Equals(baseOperName));
            if (currOper == null)
            {
                return new BaseOperationProperty[0];
            }

            BaseOperationProperty[] operationParams = 
                currOper.BaseOperationProperties;
            if (operationParams == null)
            {
                return new BaseOperationProperty[0];
            }
            return operationParams;
        }

        //---------------- Init params ---------------------------------------------

        // ------TANK PARAMS ----------------------
        // Мойка
        private static BaseOperationProperty[] tankWashParams()
        {
            return new BaseOperationProperty[]
            {
                new BaseOperationProperty("CIP_WASH_END", "Мойка завершена", ""),
                new BaseOperationProperty("DI_CIP_FREE", "МСА свободна", "")
            };
        }

        // ------LINE PARAMS ----------------------
        //Мойка
        private static BaseOperationProperty[] lineWashParams()
        {
            return new BaseOperationProperty[]
            {
                new BaseOperationProperty("CIP_WASH_END", "Мойка завершена", "")
            };
        }

        //---------------------- TEST EMPTY PARAMS ------------------------
        private static BaseOperationProperty[] emptyParams()
        {
            return new BaseOperationProperty[0];
        }

        //---------------- Init operations ---------------------------------------------

        // Базовые операции
        private static BaseOperation[] baseTankOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("", ""),
                new BaseOperation("Мойка", "WASHING_CIP", tankWashParams()),
                new BaseOperation("Наполнение", "luaName1", emptyParams()),
                new BaseOperation("Хранение", "luaName2", emptyParams()),
                new BaseOperation("Выдача", "luaName3", emptyParams()),
            };
        }

        private static BaseOperation[] baseLineOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("",""),
                new BaseOperation("Мойка", "WASHING_CIP", lineWashParams()),
                new BaseOperation("Работа", "luaName1", emptyParams())
            };
        }

        private static BaseOperation[] baseTestOperations()
        {
            return new BaseOperation[]
            {
                new BaseOperation("", ""),
                new BaseOperation("Мойка", "WASHING_CIP", emptyParams()),
                new BaseOperation("Наполнение", "luaName1", emptyParams()),
                new BaseOperation("Хранение", "luaName2", emptyParams()),
                new BaseOperation("Выдача", "luaName3", emptyParams()),
            };
        }

        //---------------- Init objects ---------------------------------------------

        public static BaseTechObject[] baseTechObjectArr()
        {
            return new BaseTechObject[]
            {
                new BaseTechObject("", "", 0, baseTestOperations(),""),
                new BaseTechObject("Автомат", "automat", 2, baseTestOperations(),"automat"),
                new BaseTechObject("Бачок", "_tank", 2, baseTestOperations(), "cooler"),
                new BaseTechObject("Бойлер", "boil", 2, baseTestOperations(), "boil"),
                new BaseTechObject("Мастер", "master", 1, baseTestOperations(), "master"),
                new BaseTechObject("Линия", "line", 2, baseLineOperations(), "line"),
                new BaseTechObject("Линия приемки", "line", 2, baseLineOperations(), "line"),
                new BaseTechObject("Линия выдачи", "line", 2, baseLineOperations(), "line"),
                new BaseTechObject("Пастеризатор", "pasteurizator", 2, baseTestOperations(), "pasteurizator"),
                new BaseTechObject("Пост", "post", 2, baseTestOperations(), "post"),
                new BaseTechObject("Танк", "tank", 1, baseTankOperations(), "tank"),
                new BaseTechObject("Узел подогрева", "heater_node", 2, baseTestOperations(), "heater_node"),
                new BaseTechObject("Узел охлаждения", "cooler_node", 2, baseTestOperations(), "cooler_node"),
                new BaseTechObject("Узел перемешивания", "mix_node", 2, baseTestOperations(), "mix_node")
            };
        }
    }
}
