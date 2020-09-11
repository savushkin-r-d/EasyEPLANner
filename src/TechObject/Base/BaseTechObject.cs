using EasyEPlanner;
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
            var equipment = new ActiveParameter(luaName, name, value);
            equipment.Owner = this;
            Equipment.Add(equipment);
        }


        /// <summary>
        /// Добавить активный параметр агрегата
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        public void AddActiveParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveParameter(luaName, name, defaultValue);
            par.Owner = this;
            AggregateParameters.Add(par);
        }

        /// <summary>
        /// Добавить активный булевый параметр агрегата
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        public void AddActiveBoolParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveBoolParameter(luaName, name, defaultValue);
            par.Owner = this;
            AggregateParameters.Add(par);
        }

        /// <summary>
        /// Добавить главный параметр агрегата
        /// </summary>
        /// <param name="luaName"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public void AddMainAggregateParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new MainAggregateParameter(luaName, name, defaultValue);
            par.Owner = this;
            aggregateMainParameter = par;
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
            if (MainAggregateParameter != null)
            {
                cloned.MainAggregateParameter = MainAggregateParameter.Clone() 
                    as MainAggregateParameter;
            }

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
                var newEquip = equip.Clone();
                newEquip.Owner = this;
                equipment.Add(newEquip);
            }
            cloned.Equipment = equipment;

            cloned.S88Level = S88Level;
            cloned.BindingName = BindingName;
            cloned.IsPID = IsPID;
            return cloned;
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

        /// <summary>
        /// Главный параметр агрегата
        /// </summary>
        public MainAggregateParameter MainAggregateParameter
        {
            get
            {
                return aggregateMainParameter;
            }
            set
            {
                aggregateMainParameter = value;
            }
        }

        /// <summary>
        /// Является ли объект ПИД-регулятором
        /// </summary>
        public bool IsPID { get; set; }

        #region Сохранение в prg.lua
        /// <summary>
        /// Сохранить информацию об объекте в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveObjectInfoToPrgLua(string objName, string prefix)
        {
            var res = "";
            if (EplanName.ToLower() != "tank")
            {
                return res;
            }

            var masterObj = TechObjectManager.GetInstance().ProcessCell;
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
        public string SaveOperationsParameters(TechObject obj, string objName,
            string prefix, List<Mode> modes)
        {
            var res = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Properties.Count == 0)
                {
                    continue;
                }

                string paramsForSave = "";
                foreach (var parameter in baseOperation.Properties)
                {
                    bool isEmpty = parameter.IsEmpty || 
                        parameter.Value == "" ||
                        parameter.NeedDisable;
                    if (isEmpty)
                    {
                        continue;
                    }

                    ParameterValueType type = 
                        GetParameterValueType(obj, parameter);
                    switch (type)
                    {
                        case ParameterValueType.Boolean:
                            paramsForSave += $"{prefix}{parameter.LuaName} = " +
                                    $"{parameter.Value},\n";
                            break;
                        case ParameterValueType.Device:
                            paramsForSave += $"{prefix}{parameter.LuaName}" +
                                $" = prg.control_modules.{parameter.Value},\n";
                            break;
                        case ParameterValueType.Number:
                            paramsForSave += GetNumberParameterStringForSave(
                                prefix, parameter, mode);
                            break;
                        case ParameterValueType.Parameter:
                            paramsForSave += $"{prefix}{parameter.LuaName} = " +
                            $"{objName}.PAR_FLOAT.{parameter.Value},\n";
                            break;
                        case ParameterValueType.Other:
                            paramsForSave += $"{prefix}{parameter.LuaName} = " +
                                    $"{parameter.Value},\n";
                            break;
                    }
                }

                bool needSaveParameters = paramsForSave != "";
                if (needSaveParameters)
                {
                    res += $"{objName}.{baseOperation.LuaName} =\n";
                    res += prefix + "{\n";
                    res += paramsForSave;
                    res += prefix + "}\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Получение строки для сохранения в зависимости от того, кто
        /// владеет параметром. Сам юнит или это параметр агрегата.
        /// </summary>
        /// <param name="parameter">Параметр для обработки</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="mainObjMode">Проверяемая операция главного объекта
        /// </param>
        /// <returns></returns>
        public string GetNumberParameterStringForSave(string prefix, 
            BaseParameter parameter, Mode mainObjMode)
        {
            BaseTechObject baseTechObject = null;
            List<Mode> modes = new List<Mode>();
            string mainObjName = "";

            if (parameter.Owner is BaseTechObject)
            {
                baseTechObject = parameter.Owner as BaseTechObject;
                mainObjName = $"{baseTechObject.Owner.DisplayText[0]}";
                modes = baseTechObject.Owner.ModesManager.Modes;
            }

            if (parameter.Owner is BaseOperation)
            {
                var operation = parameter.Owner as BaseOperation;
                baseTechObject = operation.Owner.Owner.Owner.BaseTechObject;
                mainObjName = $"{baseTechObject.Owner.DisplayText[0]}";
                modes = operation.Owner.Owner.Modes;
            }

            string parameterValue = parameter.Value;
            var mode = modes
                .Where(x => x.GetModeNumber().ToString() == parameterValue)
                .FirstOrDefault();
            var res = "";
            if (mode != null)
            {
                if (mode.BaseOperation.Name != "")
                {
                    parameterValue = mode.BaseOperation.LuaName.ToUpper();
                    TechObject obj = baseTechObject.Owner;
                    string objName = "prg." + obj.NameEplanForFile.ToLower() +
                        obj.TechNumber.ToString();
                    res = $"{prefix}{parameter.LuaName} = " +
                        $"{objName}.operations." + parameterValue + ",\n";
                }
                else
                {
                    string message = $"Ошибка обработки параметра " +
                        $"\"{parameter.Name}\"." +
                        $" Не задана базовая операция в операции" +
                        $" \"{mode.DisplayText[0]}\", объекта " +
                        $"\"{mainObjName}\".\n";
                    Logs.AddMessage(message);
                }
            }
            else
            {
                string message = $"Ошибка обработки параметра " +
                        $"\"{parameter.Name}\"." +
                        $" Указан несуществующий номер операции в операции " +
                        $"\"{mainObjMode.DisplayText[0]}\" объекта " +
                        $"\"{mainObjName}\".\n";
                Logs.AddMessage(message);
            }
            

            return res;
        }

        /// <summary>
        /// Получить тип параметра в зависимости от введенного значения в поле
        /// </summary>
        /// <param name="parameter">Параметр для проверки</param>
        /// <returns></returns>
        private ParameterValueType GetParameterValueType(TechObject obj,
            BaseParameter parameter)
        {
            var result = ParameterValueType.Other;
            var parameterValue = parameter.Value;

            if (parameter.IsBoolParameter)
            {
                result = ParameterValueType.Boolean;
                return result;
            }

            if (int.TryParse(parameterValue, out _))
            {
                result = ParameterValueType.Number;
                return result;
            }

            bool isParameter = obj.GetParamsManager()
                .GetParam(parameterValue) != null;
            if (isParameter)
            {
                result = ParameterValueType.Parameter;
                return result;
            }

            var deviceManager = Device.DeviceManager.GetInstance();
            bool isDevice = deviceManager.GetDeviceByEplanName(parameterValue)
                .Description != StaticHelper.CommonConst.Cap;
            if (isDevice)
            {
                result = ParameterValueType.Device;
                return result;
            }

            string[] devices = parameterValue.Split(' ');
            if (devices.Length > 1)
            {
                bool haveBadDevices = false;
                var validDevices = new List<bool>();
                foreach (var device in devices)
                {
                    isDevice = deviceManager.GetDeviceByEplanName(device)
                        .Description != StaticHelper.CommonConst.Cap;
                    if (isDevice == false)
                    {
                        haveBadDevices = true;
                    }
                    validDevices.Add(isDevice);
                }

                validDevices = validDevices.Distinct().ToList();
                if (validDevices.Count == 1 && haveBadDevices == false)
                {
                    result = ParameterValueType.ManyDevices;
                    return result;
                }
            }

            return result;
        }


        /// <summary>
        /// Сохранить оборудование технологического объекта
        /// </summary>
        /// <param name="objName">Имя для сохранения</param>
        /// <param name="equipment">Объект</param>
        /// <returns></returns>
        public string SaveEquipment(TechObject obj, string objName)
        {
            var res = "";
            Equipment equipment = obj.Equipment;
            foreach (var item in equipment.Items)
            {
                var property = item as BaseParameter;
                var value = property.Value;
                var luaName = property.LuaName;

                var parameterType = GetParameterValueType(obj, property);
                switch (parameterType)
                {
                    case ParameterValueType.Device:
                        res += objName + $".{luaName} = " +
                            $"prg.control_modules.{value}\n";
                        break;
                    case ParameterValueType.ManyDevices:
                        res += SaveMoreThanOneDeviceInEquipment(objName,
                            luaName, value);
                        break;
                    case ParameterValueType.Parameter:
                        res += objName + $".{luaName} = " +
                            $"{objName}.PAR_FLOAT.{value}\n";
                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить более 1 устройства в одном оборудовании.
        /// </summary>
        /// <param name="luaName">LUA имя оборудования</param>
        /// <param name="objName">Имя объекта</param>
        /// <param name="value">Значение параметра</param>
        /// <returns></returns>
        private string SaveMoreThanOneDeviceInEquipment(string objName,
            string luaName, string value)
        {
            string res = "";

            string[] devices = value.Split(' ');
            if (devices.Length > 1)
            {
                string[] modifiedDevices = devices
                    .Select(x => "prg.control_modules." + x).ToArray();
                res = objName + $".{luaName} = " +
                    $"{{ {string.Join(", ", modifiedDevices)} }} \n";
            }
            else
            {
                res = objName + $".{luaName} = prg.control_modules.{value}\n";
            }

            return res;
        }
        #endregion

        /// <summary>
        /// Возможные типы значений полей параметров объекта
        /// </summary>
        enum ParameterValueType
        {
            Other,
            ManyDevices,
            Device,
            Boolean,
            Parameter,
            Number,
        }

        private string name;
        private string eplanName;
        private int s88Level;
        private string basicName;
        private TechObject owner;
        private string bindingName;

        private List<BaseOperation> objectOperations;
        private List<BaseParameter> equipment;
        private List<BaseParameter> aggregateProperties;
        private MainAggregateParameter aggregateMainParameter;
    }
}
