﻿using System;
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
        private BaseTechObject()
        {
            Name = "";
            EplanName = "";
            S88Level = 0;
            BaseOperations = new List<BaseOperation>();
            BasicName = "";
            Owner = null;
            Equipment = new List<BaseParameter>();
            AggregateParameters = new List<BaseParameter>();
            BindingName = "";
        }

        public BaseTechObject(TechObject owner)
        {
            Name = "";
            EplanName = "";
            S88Level = 0;
            BaseOperations = new List<BaseOperation>();
            BasicName = "";
            Owner = owner;
            Equipment = new List<BaseParameter>();
            AggregateParameters = new List<BaseParameter>();
            BindingName = "";
        }

        public static BaseTechObject EmptyBaseTechObject()
        {
            return new BaseTechObject();
        }

        /// <summary>
        /// Добавить оборудование в базовый объект
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        public void AddEquipment(string luaName, string name, string value)
        {
            Equipment.Add(new ActiveParameter(luaName, name, value));
        }

        /// <summary>
        /// Добавить параметр агрегата
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        public void AddAggregateParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveBoolParameter(luaName, name, defaultValue);
            AggregateParameters.Add(par);
        }

        /// <summary>
        /// Добавить базовую операцию
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public BaseOperation AddBaseOperation(string luaName, string name)
        {
            if (BaseOperations.Count == 0)
            {
                // Пустой объект, если не должно быть выбрано никаких объектов
                BaseOperations.Add(BaseOperation.EmptyOperation());
            }

            var operation = BaseOperation.EmptyOperation();
            operation.LuaName = luaName;
            operation.Name = name;
            BaseOperations.Add(operation);

            return operation;
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
        public List<BaseOperation> BaseOperations
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
        public List<BaseParameter> Equipment
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
            var cloned = Clone();
            cloned.Owner = techObject;
            return cloned;
        }

        /// <summary>
        /// Копия объекта
        /// </summary>
        /// <returns></returns>
        public BaseTechObject Clone()
        {
            var cloned = EmptyBaseTechObject();
            cloned.Name = Name;
            cloned.Owner = Owner;

            var aggregateParameters = new List<BaseParameter>();
            foreach(var aggrPar in AggregateParameters)
            {
                aggregateParameters.Add(aggrPar.Clone());
            }
            cloned.AggregateParameters = aggregateParameters;

            var baseOperations = new List<BaseOperation>();
            foreach(var baseOperation in BaseOperations)
            {
                baseOperations.Add(baseOperation.Clone());
            }
            cloned.BaseOperations = baseOperations;

            cloned.BasicName = BasicName;
            cloned.EplanName = EplanName;

            var equipment = new List<BaseParameter>();
            foreach(var equip in Equipment)
            {
                equipment.Add(equip.Clone());
            }
            cloned.Equipment = equipment;

            cloned.S88Level = S88Level;
            cloned.BindingName = BindingName;
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
                if (S88Level == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Параметры объекта, как агрегата (добавляемые в аппарат).
        /// </summary>
        public List<BaseParameter> AggregateParameters
        {
            get
            {
                if (aggregateProperties == null)
                {
                    return new List<BaseParameter>();
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

        /// <summary>
        /// Имя агрегата при его привязке к аппарату.
        /// </summary>
        public string BindingName
        {
            get
            {
                return bindingName;
            }

            set
            {
                bindingName = value;
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
            res += SaveObjectInfoToPrgLua(objName, prefix);

            var modesManager = this.Owner.ModesManager;
            var modes = modesManager.Modes;
            if (modes.Where(x => x.DisplayText[1] != "").Count() != 0)
            {
                res += SaveOperations(objName, prefix, modes);
                res += SaveOperationsSteps(objName, prefix, modes);
                res += SaveOperationsParameters(objName, prefix, modes);
            }

            res += SaveEquipment(objName);

            return res;
        }

        /// <summary>
        /// Сохранить информацию об объекте в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveObjectInfoToPrgLua(string objName, string prefix)
        {
            var res = "";
            if (EplanName.ToLower() != "tank")
            {
                return res;
            }

            var objects = TechObjectManager.GetInstance();
            var masterObj = objects.Objects
                .Where(x => x.Name.Contains("Мастер")).FirstOrDefault();
            if (masterObj != null)
            {
                res += objName + ".master = prg." + masterObj.NameEplan
                    .ToLower() + masterObj.TechNumber + "\n";
            }

            // Параметры сбрасываемые до мойки.
            res += objName + ".reset_before_wash =\n" +
                prefix + "{\n" +
                prefix + objName + ".PAR_FLOAT.V_ACCEPTING_CURRENT,\n" +
                prefix + objName + ".PAR_FLOAT.PRODUCT_TYPE,\n" +
                prefix + objName + ".PAR_FLOAT.V_ACCEPTING_SET\n" +
                prefix + "}\n";

            return res;
        }

        /// <summary>
        /// Сохранить операции объекта
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        public string SaveOperations(string objName, string prefix, 
            List<Mode> modes)
        {
            var res = "";

            string saveOperations = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name != "")
                {
                    saveOperations += prefix + baseOperation.LuaName.ToUpper() +
                        " = " + mode.GetModeNumber() + ",\n";
                }
            }

            bool isEmpty = saveOperations == "";
            if (!isEmpty)
            {
                res += objName + ".operations = \t\t--Операции.\n";
                res += prefix + "{\n";
                res += saveOperations;
                res += prefix + "}\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить номера шагов операций объекта.
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        public string SaveOperationsSteps(string objName, string prefix,
            List<Mode> modes)
        {
            var res = "";

            string steps = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name == "")
                {
                    continue;
                }

                string stepString = "";
                foreach (var step in mode.MainSteps)
                {
                    if (step.GetBaseStepName() == "")
                    {
                        continue;
                    }
                    stepString += prefix + prefix + step.GetBaseStepLuaName() +
                        " = " + step.GetStepNumber() + ",\n";
                }

                bool stepIsEmpty = stepString == "";
                if (!stepIsEmpty)
                {
                    steps += prefix + baseOperation.LuaName.ToUpper() + " =\n";
                    steps += prefix + prefix + "{\n";
                    steps += stepString;
                    steps += prefix + prefix + "},\n";
                }
            }

            bool stepsIsEmpty = steps == "";
            if(!stepsIsEmpty)
            {
                res += objName + ".steps = \t\t--Шаги операций.\n";
                res += prefix + "{\n";
                res += steps;
                res += prefix + "}\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить параметры операций базового объекта танк.
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        private string SaveOperationsParameters(string objName, string prefix,
            List<Mode> modes)
        {
            var res = "";

            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Properties.Count == 0)
                {
                    return res;
                }

                switch (baseOperation.Name)
                {
                    case "Мойка":
                        res += SaveWashOperationParameters(objName,
                            baseOperation);
                        break;

                    case "Наполнение":
                        res += SaveFillOperationParameters(objName,
                            baseOperation, prefix);
                        break;

                    default:
                        if (baseOperation.Properties.Count < 1)
                        {
                            continue;
                        }

                        string paramsForSave = "";
                        foreach (var param in baseOperation.Properties)
                        {
                            if (!param.IsEmpty) 
                            {
                                paramsForSave += $"{prefix}{param.LuaName} =" +
                                    $" {param.Value},\n";
                            }

                        }

                        bool needSaveParameters = paramsForSave != "";
                        if(needSaveParameters)
                        {
                            res += $"{objName}.{baseOperation.LuaName} =\n";
                            res += prefix + "{\n";
                            res += paramsForSave;
                            res += prefix + "}\n";
                        }
                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить параметры операции мойки
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <returns></returns>
        private string SaveWashOperationParameters(string objName,
            BaseOperation baseOperation)
        {
            var res = "";

            foreach (BaseParameter param in baseOperation.Properties)
            {
                if (!param.IsEmpty)
                {
                    res += objName + "." + param.LuaName +
                        " = prg.control_modules." + param.Value + "\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить параметры операции наполнения.
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="baseOperation">Базовая операция</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveFillOperationParameters(string objName,
            BaseOperation baseOperation, string prefix)
        {
            var res = "";

            string fillParameters = "";
            foreach (BaseParameter param in baseOperation.Properties)
            {
                string val = param.Value;
                switch (param.LuaName)
                {
                    case "OPERATION_AFTER_FILL":
                        var modes = this.Owner.ModesManager.Modes;
                        var mode = modes
                            .Where(x => x.GetModeNumber().ToString() == val)
                            .FirstOrDefault();

                        if (mode != null)
                        {
                            val = mode.BaseOperation.LuaName.ToUpper();
                        }

                        if (!param.IsEmpty)
                        {
                            fillParameters += $"{prefix}{param.LuaName} = " +
                                $"{objName}.operations." + val + ",\n";
                        }
                        break;

                    default:
                        if (!param.IsEmpty)
                        {
                            fillParameters += $"{prefix}{param.LuaName} = " +
                                $"{val},\n";
                        }
                        break;
                }
            }

            bool needSaveParameters = fillParameters != "";
            if (needSaveParameters)
            {
                res += $"{objName}.{baseOperation.LuaName} =\n";
                res += prefix + "{\n";
                res += fillParameters;
                res += prefix + "}\n";
            }

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
            foreach (var item in equipment.Items)
            {
                var property = item as BaseParameter;
                var value = property.Value;
                var luaName = property.LuaName;

                bool isEmpty = property.IsEmpty ||
                    property.Value == property.DefaultValue;
                if (!isEmpty)
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
            }

            return res;
        }
        #endregion

        private string name;
        private string eplanName;
        private int s88Level;
        private string basicName;
        private TechObject owner;
        private string bindingName;

        private List<BaseOperation> objectOperations;
        private List<BaseParameter> equipment;
        private List<BaseParameter> aggregateProperties;
    }
}
