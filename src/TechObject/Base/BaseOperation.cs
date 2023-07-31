using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    public interface IBaseOperation
    {
        /// <summary>
        /// Получить имя операции
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Получить Lua-имя операции
        /// </summary>
        string LuaName { get; set; }

        /// <summary>
        /// Получить свойства базовой операции
        /// </summary>
        List<BaseParameter> Properties { get; set; }

        /// <summary>
        /// Инициализация базовой операции по имени
        /// </summary>
        /// <param name="baseOperName">Имя операции</param>
        /// <param name="mode">Операция владелец</param>
        void Init(string baseOperName, Mode mode);

        /// <summary>
        /// Копирование объекта
        /// </summary>
        /// <param name="owner">Новая операция-владелец объекта</param>
        /// <returns></returns>
        BaseOperation Clone(Mode owner);

        /// <summary>
        /// Сохранение в виде таблицы Lua
        /// </summary>
        /// <param name="prefix">Префикс (отступ)</param>
        /// <returns></returns>
        string SaveAsLuaTable(string prefix);

        void Synch(int[] array);

        /// <summary>
        /// Сброс доп.свойств привязанных агрегатов
        /// </summary>
        void SetExtraProperties(List<BaseParameter> properties);

        /// <summary>
        /// Установка свойств базовой операции
        /// </summary>
        /// <param name="extraParams">Свойства операции</param>
        void SetExtraProperties(ObjectProperty[] extraParams);
    }

    /// <summary>
    /// Класс реализующий базовую операцию для технологического объекта
    /// </summary>
    public class BaseOperation : TreeViewItem, IBaseOperation
    {
        public BaseOperation(Mode owner)
        {
            Name = string.Empty;
            LuaName = string.Empty;
            Properties = new List<BaseParameter>();
            states = new Dictionary<string, List<BaseStep>>();
            this.owner = owner;
        }

        /// <summary>
        /// Возвращает пустой объект - базовая операция.
        /// </summary>
        /// <returns></returns>
        public static BaseOperation EmptyOperation()
        {
            return new BaseOperation(string.Empty, string.Empty,
                new List<BaseParameter>(),
                new Dictionary<string, List<BaseStep>>());
        }

        /// <summary>
        /// Конструктор для инициализации базовой операции и параметров
        /// </summary>
        /// <param name="name">Имя операции</param>
        /// <param name="luaName">Lua имя операции</param>
        /// <param name="baseOperationProperties">Свойства операции</param>
        /// <param name="baseStates">Состояния операции с базовыми шагами</param>
        public BaseOperation(string name, string luaName, 
            List<BaseParameter> baseOperationProperties, 
            Dictionary<string, List<BaseStep>> baseStates)
        {
            Name = name;
            LuaName = luaName;
            Properties = baseOperationProperties;
            states = baseStates;
        }

        /// <summary>
        /// Добавить базовый шаг
        /// </summary>
        /// <param name="stateTypeStr">Тип состояния</param>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultPosition">Стандартная позиция шага в 
        /// базовой операции.</param>
        public void AddStep(string stateTypeStr, string luaName, string name,
            int defaultPosition)
        {
            var step = new BaseStep(name, luaName, defaultPosition);
            step.Owner = this;

            if (states.ContainsKey(stateTypeStr))
            {
                states[stateTypeStr].Add(step);
            }
            else
            {
                var emptyStep = new BaseStep(string.Empty, string.Empty);
                emptyStep.Owner = this;
                
                var stepsList = new List<BaseStep>();
                stepsList.Add(emptyStep);
                stepsList.Add(step);

                states.Add(stateTypeStr, stepsList);
            }
        }

        /// <summary>
        /// Добавить активный параметр
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        /// <returns>Добавленный параметр</returns>
        public ActiveParameter AddActiveParameter(string luaName, string name, 
            string defaultValue)
        {
            var par = new ActiveParameter(luaName, name, defaultValue);
            par.Owner = this;
            Properties.Add(par);
            return par;
        }
        
        /// <summary>
        /// Добавить активный булевый параметр
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        public void AddActiveBoolParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveBoolParameter(luaName, name,
                defaultValue);
            par.Owner = this;
            Properties.Add(par);
        }

        public string Name
        {
            get => operationName;
            set => operationName = value;
        }

        public string LuaName
        {
            get => luaOperationName;
            set => luaOperationName = value;
        }

        /// <summary>
        /// Состояния базовой операции.
        /// Ключ - Lua-имя состояния (из перечисления).
        /// Значение - базовые шаги.
        /// </summary>
        public Dictionary<string, List<BaseStep>> States
        {
            get
            {
                return states;
            }
        }

        public List<BaseStep> GetStateBaseSteps(State.StateType stateType)
        {
            var steps = new List<BaseStep>();
            if (StateExist(stateType))
            {
                steps = states[$"{stateType}"];
            }

            return steps;
        }

        public List<string> GetStateStepsNames(State.StateType stateType)
        {
            var stepsNames = new List<string>();
            if (StateExist(stateType))
            {
                stepsNames = states[$"{stateType}"]
                    .Select(x => x.Name).ToList();
            }

            return stepsNames;
        }

        private bool StateExist(State.StateType stateType)
        {
            string stateTypeStr = stateType.ToString();
            return states.ContainsKey(stateTypeStr);
        }

        public void Init(string baseOperName, Mode mode)
        {
            TechObject techObject = owner.Owner.Owner;
            BaseTechObject baseTechObject = techObject.BaseTechObject;
            string baseTechObjectName = baseTechObject?.Name ?? string.Empty;

            ResetOperationSteps();

            if (baseTechObjectName != string.Empty)
            {
                BaseOperation operation;
                operation = techObject.BaseTechObject
                    .GetBaseOperationByName(baseOperName);
                if (operation == null)
                {
                    operation = techObject.BaseTechObject
                        .GetBaseOperationByLuaName(baseOperName);
                }

                if (operation != null)
                {
                    Name = operation.Name;
                    LuaName = operation.LuaName;
                    Properties = operation.Properties
                        .Select(x => x.Clone())
                        .ToList();
                    foreach(var property in Properties)
                    {
                        property.Owner = this;
                        property.Parent = this;
                    }

                    states = operation.States;
                    foreach(var state in states)
                    {
                        foreach(var step in states[state.Key])
                        {
                            step.Owner = this;
                        }
                    }

                    owner = mode;
                    if(mode.Name == Mode.DefaultModeName)
                    {
                        mode.SetNewValue(operation.Name);
                    }
                }
            }
            else
            {
                Name = string.Empty;
                LuaName = string.Empty;
                baseOperationProperties = new List<BaseParameter>();
                states = new Dictionary<string, List<BaseStep>>();
            }

            techObject.AttachedObjects.Check();
            SetItems();
        }

        /// <summary>
        /// Сбросить базовые шаги базовой операции
        /// </summary>
        private void ResetOperationSteps()
        {
            foreach (var state in owner.States)
            {
                foreach(var step in state.Steps)
                {
                    step.SetNewValue(string.Empty, true);
                }
            }
        }

        /// <summary>
        /// Добавление полей в массив для отображения на дереве
        /// </summary>
        private void SetItems()
        {
            var showedParameters = new List<BaseParameter>();
            foreach (var parameter in Properties)
            {
                showedParameters.Add(parameter);
            }
            items = showedParameters.ToArray();
        }

        public string SaveAsLuaTable(string prefix)
        {
            var res = string.Empty;
            var propertiesCountForSave = Properties.Count();
            if (Properties == null || propertiesCountForSave <= 0)
            {
                return res;
            }

            string paramsForSave = string.Empty;
            foreach (var operParam in Properties)
            {
                if(!operParam.NeedDisable && !operParam.IsEmpty)
                {
                    paramsForSave += "\t" + prefix + operParam.LuaName +
                        " = \'" + operParam.Value + "\',\n";
                }
            }

            if (paramsForSave != string.Empty)
            {
                res += prefix + "props =\n" + prefix + "\t{\n";
                res += paramsForSave;
                res += prefix + "\t},\n";
            }

            return res;
        }

        public void SetExtraProperties(ObjectProperty[] extraParams)
        {
            SetExtraProperties(extraParams
                .ToDictionary(prop => prop.DisplayText[0], prop => prop.Value));
        }

        public void SetExtraProperties(List<BaseParameter> properties)
        {
            SetExtraProperties(properties.ToDictionary(prop => prop.LuaName, prop => prop.Value));
        }

        public void SetExtraProperties(Dictionary<string, string> extraProperties)
        {
            foreach (var property in Properties.Where(obj => extraProperties.ContainsKey(obj.LuaName)))
                property.SetNewValue(extraProperties[property.LuaName]);
        }

        public List<BaseParameter> Properties
        {
            get => baseOperationProperties;
            set => baseOperationProperties = value;
        }

        public Mode Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Номер операции в списке операций объекта. Если 0 - не задано.
        /// </summary>
        public int DefaultPosition { get; set; } = 0;
       
        /// <summary>
        /// Добавить свойства базовой операции.
        /// </summary>
        /// <param name="properties">Массив свойств</param>
        /// <param name="owner">Объект первоначальный владелец свойств</param>
        public void AddProperties(List<BaseParameter> properties, object owner)
        {
            foreach(var property in properties)
            {
                var equalPropertiesCount = Properties
                    .Where(x => x.LuaName == property.LuaName).Count();
                if (equalPropertiesCount == 0)
                {
                    var newProperty = property.Clone();
                    newProperty.Owner = owner;
                    newProperty.Parent = this;
                    Properties.Add(newProperty);
                }
            }

            SetItems();
        }

        /// <summary>
        /// Удалить свойства базовой операции.
        /// </summary>
        /// <param name="properties">Массив свойств</param>
        public void RemoveProperties(List<BaseParameter> properties)
        {
            foreach (var property in properties)
            {
                var deletingProperty = Properties
                    .Where(x => x.LuaName == property.LuaName)
                    .FirstOrDefault();
                if (deletingProperty != null)
                {
                    Properties.Remove(deletingProperty);
                }
            }
            SetItems();
        }

        /// <summary>
        /// Изменить владельца свойств агрегата в аппарате.
        /// </summary>
        /// <param name="oldOwner">Старый базовый объект</param>
        /// <param name="newOwner">Новый базовый объект</param>
        public void ChangePropertiesOwner(BaseTechObject oldOwner,
            BaseTechObject newOwner)
        {
            foreach(var property in Properties)
            {
                if (property.Owner == oldOwner)
                {
                    property.Owner = newOwner;
                }
            }
        }

        /// <summary>
        /// Проверка базовой операции
        /// </summary>
        public string Check()
        {
            string errors = string.Empty;
            foreach (var property in Properties)
            {
                property.Check();

                bool notStub = !property.Value.ToLower()
                    .Contains(StaticHelper.CommonConst.StubForCells
                    .ToLower());
                if (notStub)
                {
                    CheckNotEmptyDisabledAggregateProperties(property,
                        ref errors);
                }
            }

            return errors;
        }

        /// <summary>
        /// Проверка пустых не отключенных параметров агрегатов
        /// </summary>
        /// <param name="property">Свойство</param>
        /// <param name="errors">Список ошибок</param>
        private void CheckNotEmptyDisabledAggregateProperties(
            BaseParameter property, ref string errors)
        {
            // Disabled устанавливается, когда был хотя бы раз запущен UI
            // редактора технологических объектов,
            // а если не был запущен, то надо смотреть NeedDisable.
            bool notEmptyDisabledAggregateProperty =
                property.Owner is BaseTechObject &&
                property.Disabled == false &&
                property.NeedDisable == false &&
                (property.Value == "");
            if (notEmptyDisabledAggregateProperty)
            {
                string modeName = owner.DisplayText[0];
                string techObjName = Owner.Owner.Owner.DisplayText[0];
                string message = $"Свойство \"{property.Name}\" в " +
                    $"операции \"{modeName}\", объекта \"{techObjName}\"" +
                    $" не заполнено.\n";
                errors += message;
            }
        }

        public BaseOperation Clone(Mode owner)
        {
            var operation = Clone();
            operation.owner = owner;
            return operation;
        }

        /// <summary>
        /// Копирование объекта
        /// </summary>
        /// <returns></returns>
        public BaseOperation Clone()
        {
            var operation = EmptyOperation();
            List<BaseParameter> properties = CloneProperties(operation);
            Dictionary<string, List<BaseStep>> states = CloneStates(operation);

            operation.Name = operationName;
            operation.LuaName = luaOperationName;
            operation.Properties = properties;
            operation.states = states;
            operation.owner = Owner;
            operation.DefaultPosition = DefaultPosition;

            operation.SetItems();

            return operation;
        }

        /// <summary>
        /// Копирование доп. свойств базовой операции
        /// </summary>
        /// <param name="newOwner">Новая базовая операция-владелец</param>
        /// <returns></returns>
        private List<BaseParameter> CloneProperties(BaseOperation newOwner)
        {
            var properties = new List<BaseParameter>();

            for (int i = 0; i < baseOperationProperties.Count; i++)
            {
                BaseParameter oldProperty = baseOperationProperties[i];
                BaseParameter newProperty = oldProperty.Clone();
                if (oldProperty.Owner is BaseTechObject)
                {
                    var obj = oldProperty.Owner as BaseTechObject;
                    if (obj.IsAttachable)
                    {
                        newProperty.Owner = oldProperty.Owner;
                    }
                }
                else
                {
                    newProperty.Owner = newOwner;
                }
                properties.Add(newProperty);
            }

            return properties;
        }

        /// <summary>
        /// Копирование состояний и базовых шагов
        /// </summary>
        /// <param name="newOwner">Новая операция-владелец</param>
        /// <returns></returns>
        private Dictionary<string, List<BaseStep>> CloneStates(
            BaseOperation newOwner)
        {
            var clonedStates = new Dictionary<string, List<BaseStep>>();
            foreach (var state in States)
            {
                var clonedSteps = new List<BaseStep>();
                List<BaseStep> steps = state.Value;
                foreach (var step in steps)
                {
                    BaseStep clonedStep = step.Clone();
                    clonedStep.Owner = newOwner;
                    clonedSteps.Add(clonedStep);
                }

                clonedStates.Add(state.Key, clonedSteps);
            }

            return clonedStates;
        }

        #region синхронизация устройств
        public void Synch(int[] array)
        {
            foreach(var property in baseOperationProperties)
            {
                property.Synch(array);
            }
        }
        #endregion

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                if (items.Count() > 0)
                {
                    string res = string.Format("Доп. свойства ({0})", 
                        items.Count());
                    return new string[] { res, string.Empty };
                }
                else
                {
                    string res = string.Format("Доп. свойства");
                    return new string[] { res, string.Empty };
                }
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items;
            }
        }

        public override bool Delete(object child)
        {
            if (child is ActiveParameter)
            {
                var property = child as ActiveParameter;
                property.SetNewValue(string.Empty);
                return true;
            }
            return false;
        }

        public override bool IsFilled
        {
            get
            {
                if(items.Where(x=> x.IsFilled).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=process_parameter";
        }

        private ITreeViewItem[] items = new ITreeViewItem[0];
        
        private List<BaseParameter> baseOperationProperties;
        private string operationName;
        private string luaOperationName;
        private Dictionary<string, List<BaseStep>> states;

        private Mode owner;
    }
}
