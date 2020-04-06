using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс реализующий базовый аппарат для технологического объекта
    /// </summary>
    public class BaseTechObject
    {
        public BaseTechObject()
        {
            Name = "";
            EplanName = "";
            S88Level = 0;
            BaseOperations = new BaseOperation[0];
            BaseProperties = new List<BaseProperty>();
            BasicName = "";
            Owner = null;
            Equipment = new BaseProperty[0];
            AggregateProperties = new BaseProperty[0];

        }

        public BaseTechObject(TechObject owner)
        {
            Name = "";
            EplanName = "";
            S88Level = 0;
            BaseOperations = new BaseOperation[0];
            BaseProperties = new List<BaseProperty>();
            BasicName = "";
            Owner = owner;
            Equipment = new BaseProperty[0];
            AggregateProperties = new BaseProperty[0];
        }

        /// <summary>
        /// Имя базового объекта.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// ОУ базового объекта
        /// </summary>
        public string EplanName
        {
            get
            {
                return eplanName;
            }

            set
            {
                eplanName = value;
            }
        }

        /// <summary>
        /// Уровень по S88 иерархии
        /// </summary>
        public int S88Level
        {
            get
            {
                return s88Level;
            }

            set
            {
                s88Level = value;
            }
        }

        /// <summary>
        /// Базовые операции объекта
        /// </summary>
        public BaseOperation[] BaseOperations
        {
            get 
            { 
                return objectOperations; 
            }

            set
            {
                objectOperations = value;
            }
        }

        /// <summary>
        /// Имя объекта для базовой функциональности
        /// </summary>
        public string BasicName
        {
            get 
            {
                return basicName;
            }

            set
            {
                basicName = value;
            }
        }

        /// <summary>
        /// Свойства базового объекта
        /// </summary>
        public List<BaseProperty> BaseProperties
        {
            get
            {
                return objectProperties;
            }

            set
            {
                objectProperties = value;
            }
        }

        /// <summary>
        /// Владелец объекта.
        /// </summary>
        public TechObject Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// Оборудование базового объекта
        /// </summary>
        public BaseProperty[] Equipment
        {
            get
            {
                return equipment;
            }
            
            set
            {
                equipment = value;
            }
        }

        /// <summary>
        /// Получить базовую операцию по имени.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public BaseOperation GetBaseOperationByName(string name)
        {
            var operation = BaseOperations.Where(x => x.Name == name)
                .FirstOrDefault();
            return operation;
        }

        /// <summary>
        /// Получить базовую операцию по Lua-имени
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <returns></returns>
        public BaseOperation GetBaseOperationByLuaName(string luaName)
        {
            var operation = BaseOperations.Where(x => x.LuaName == luaName)
                .FirstOrDefault();
            return operation;
        }

        /// <summary>
        /// Список операций базового объекта
        /// </summary>
        /// <returns></returns>
        public List<string> BaseOperationsList
        {
            get
            {
                return BaseOperations.Select(x => x.Name).ToList();
            }
        }

        /// <summary>
        /// Копия объекта
        /// </summary>
        /// <param name="techObject">Копируемый объект</param>
        /// <returns></returns>
        public BaseTechObject Clone(TechObject techObject)
        {
            var cloned = DataBase.Imitation.BaseTechObjects()
                .Where(x => x.Name == this.Name)
                .FirstOrDefault();
            cloned.Owner = techObject;
            return cloned;
        }

        /// <summary>
        /// Сброс базовых операций объекта
        /// </summary>
        public void ResetBaseOperations()
        {
            foreach (Mode operation in Owner.ModesManager.Modes)
            {
                operation.BaseOperation.Init("");
            }
        }

        /// <summary>
        /// Является ли базовый объект привязываемым к другому объекту.
        /// </summary>
        public virtual bool IsAttachable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Свойства объекта, как агрегата.
        /// </summary>
        public BaseProperty[] AggregateProperties
        {
            get
            {
                if (aggregateProperties == null)
                {
                    return new BaseProperty[0];
                }
                else
                {
                    return aggregateProperties;
                }
            }

            set
            {
                aggregateProperties = value;
            }
        }

        #region Сохранение в prg.lua
        /// <summary>
        /// Сохранить информацию об операциях объекта в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public virtual string SaveToPrgLua(string objName, 
            string prefix)
        {
            var res = "";
            return res;
        }

        /// <summary>
        /// Сохранить оборудование технологического объекта
        /// </summary>
        /// <param name="objName">Имя для сохранения</param>
        /// <returns></returns>
        public string SaveEquipment(string objName)
        {
            var res = "";
            var equipment = this.owner.Equipment;
            bool needWhiteSpace = false;

            foreach (var item in equipment.Items)
            {
                var property = item as BaseProperty;
                var value = property.Value;
                var luaName = property.LuaName;

                if (value != "")
                {
                    if (owner.Params.GetFParam(value) == null)
                    {
                        res += objName + $".{luaName} = " +
                            $"prg.control_modules.{value}\n";
                    }
                    else
                    {
                        res += objName + $".{luaName} = " +
                            $"{objName}.PAR_FLOAT.{value}\n";
                    }
                    
                }
                else
                {
                    res += objName + $".{luaName} = nil\n";
                }

                needWhiteSpace = true;
            }

            if (needWhiteSpace)
            {
                res += "\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить операции объекта
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveOperations(string objName, string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            if (modes.Where(x => x.DisplayText[1] != "").Count() == 0)
            {
                return res;
            }

            res += objName + ".operations = \t\t--Операции.\n";
            res += prefix + "{\n";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name != "")
                {
                    res += prefix + baseOperation.LuaName.ToUpper() + " = " +
                        mode.GetModeNumber() + ",\n";
                }
            }
            res += prefix + "}\n";

            return res;
        }

        /// <summary>
        /// Сохранить номера шагов операций объекта.
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveOperationsSteps(string objName, string prefix)
        {
            var res = "";

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            if (modes.Where(x => x.BaseOperation.Name != "").Count() == 0)
            {
                return res;
            }

            res += objName + ".steps = \t\t--Шаги операций.\n";
            res += prefix + "{\n";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name == "")
                {
                    continue;
                }

                string temp = "";
                foreach (var step in mode.MainSteps)
                {
                    if (step.GetBaseStepName() == "")
                    {
                        continue;
                    }

                    temp += prefix + prefix + step.GetBaseStepLuaName() +
                        " = " + step.GetStepNumber() + ",\n";
                }

                if (temp.Length == 0)
                {
                    string emptyTable = prefix + baseOperation.LuaName
                        .ToUpper() + " = { },\n";
                    res += emptyTable;
                }
                else
                {
                    res += prefix + baseOperation.LuaName.ToUpper() + " =\n";
                    res += prefix + prefix + "{\n";
                    res += temp;
                    res += prefix + prefix + "},\n";
                }               
            }

            res += prefix + "}\n";
            return res;
        }
        #endregion

        private string name;
        private string eplanName;
        private int s88Level;
        private string basicName;
        private TechObject owner;

        private BaseOperation[] objectOperations;
        private List<BaseProperty> objectProperties;
        private BaseProperty[] equipment;
        private BaseProperty[] aggregateProperties;
    }
}
