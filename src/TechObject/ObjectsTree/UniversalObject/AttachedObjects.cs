using Editor;
using System.Collections.Generic;
using System.Linq;
using TechObject.AttachedObjectStrategy;

namespace TechObject
{
    /// <summary>
    /// Класс привязки объектов к объекту
    /// </summary>
    public class AttachedObjects : ObjectProperty
    {
        public AttachedObjects(string attachedObjects, TechObject owner,
            IAttachedObjectsStrategy strategy) :
            base(strategy.Name, attachedObjects)
        {
            this.owner = owner;
            this.strategy = strategy;
            SetValue(attachedObjects);
        }

        public override bool SetNewValue(string newValue)
        {
            string oldValue = Value;
            var thisObjNum = TechObjectManager.GetInstance()
                .GetTechObjectN(Owner);
            List<int> newNumbers = strategy.GetValidTechObjNums(newValue,
                thisObjNum);

            newValue = string.Join(" ", newNumbers);
            string sortedValue = MoveNewValuesInTheEndOfString(newValue,
                oldValue);
            base.SetNewValue(sortedValue);

            if (strategy.UseInitialization)
            {
                var replacedObjects = new Dictionary<TechObject, TechObject>();

                List<TechObject> newObjects = newNumbers
                    .Select(x => TechObjectManager.GetInstance().GetTObject(x))
                    .ToList();
                List<TechObject> oldObjects = strategy
                    .GetValidTechObjNums(oldValue, thisObjNum)
                    .Select(x => TechObjectManager.GetInstance().GetTObject(x))
                    .ToList();
                replacedObjects = FindReplacedObjects(oldObjects,
                    newObjects);
                ChangeOwnerInReplacedObjectsProperties(replacedObjects);

                List<int> deletedAgregatesNumbers = FindDeletedObjects(
                    oldValue, newValue, replacedObjects);
                RemoveDeletedObjects(deletedAgregatesNumbers);

                InitAttachedObjects(newNumbers);
            }

            return true;
        }

        /// <summary>
        /// Переместить новые значения в конец строки
        /// </summary>
        /// <param name="newValue">Строка с новыми значениями</param>
        /// <param name="oldValue">Строка со старыми значениями</param>
        /// <returns></returns>
        private string MoveNewValuesInTheEndOfString(string newValue,
            string oldValue)
        {
            char charSeparator = ' ';
            List<string> oldValues = oldValue.Split(charSeparator).ToList();
            List<string> newValues = newValue.Split(charSeparator).ToList();
            
            if(oldValues.Count >= newValues.Count)
            {
                return newValue;
            }
            else
            {
                List<string> differenceInValues = newValues
                    .Except(oldValues).ToList();
                foreach(var diffValue in differenceInValues)
                {
                    oldValues.Add(diffValue);
                }

                string stringSeparator = " ";
                return string.Join(stringSeparator, oldValues);
            }
        }

        public override bool SetNewValue(IDictionary<int, List<int>> newDict)
        {
            return SetNewValue(string.Join(" ", newDict.Keys));
        }

        /// <summary>
        /// Проверить и найти замененные объекты
        /// </summary>
        /// <param name="oldObjects">Старый список объектов</param>
        /// <param name="newObjects">Новый список объектов</param>
        /// <returns></returns>
        private Dictionary<TechObject, TechObject> FindReplacedObjects(
            List<TechObject> oldObjects, List<TechObject> newObjects)
        {
            var replacedObjects = new Dictionary<TechObject, TechObject>();

            foreach (var newObj in newObjects)
            {
                var replacedObject = oldObjects
                    .Where(x => x.BaseTechObject.Name ==
                    newObj.BaseTechObject.Name &&
                    x.GetLocalNum != newObj.GetLocalNum)
                    .FirstOrDefault();
                if (replacedObject != null)
                {
                    replacedObjects.Add(replacedObject, newObj);
                }
            }

            return replacedObjects;
        }

        /// <summary>
        /// Заменить владельца в замененных объектах
        /// </summary>
        /// <param name="replacedObjects">Замененные объекты</param>
        private void ChangeOwnerInReplacedObjectsProperties(
            Dictionary<TechObject, TechObject> replacedObjects)
        {
            if (replacedObjects.Count == 0)
            {
                return;
            }

            foreach (var objects in replacedObjects)
            {
                TechObject oldObject = objects.Key;
                TechObject newObject = objects.Value;

                foreach (var mode in owner.ModesManager.Modes)
                {
                    mode.BaseOperation.ChangePropertiesOwner(
                        oldObject.BaseTechObject, newObject.BaseTechObject);
                    mode.SetItems();
                }
            }
        }

        /// <summary>
        /// Найти удаленные объекты
        /// </summary>
        /// <param name="oldValue">Старое значение поля</param>
        /// <param name="newValue">Новое значение поля</param>
        /// <param name="replacedObjects">Словарь замененных объектов.
        /// Ключ - старый объект, значение - новый объект.</param>
        /// <returns></returns>
        private List<int> FindDeletedObjects(string oldValue,
            string newValue,
            Dictionary<TechObject, TechObject> replacedObjects)
        {
            var oldNumbers = new List<int>();
            var newNumbers = new List<int>();
            if (oldValue != null && oldValue != string.Empty)
            {
                oldNumbers = oldValue.Split(' ').Select(int.Parse).ToList();
            }
            if (newValue != null && newValue != string.Empty)
            {
                newNumbers = newValue.Split(' ').Select(int.Parse).ToList();
            }

            foreach (var newNum in newNumbers)
            {
                if (oldNumbers.Contains(newNum))
                {
                    oldNumbers.Remove(newNum);
                }
            }

            foreach (var replacedObj in replacedObjects?.Keys)
            {
                var replacedObjNum = TechObjectManager.GetInstance()
                    .GetTechObjectN(replacedObj);
                if (oldNumbers.Contains(replacedObjNum))
                {
                    oldNumbers.Remove(replacedObjNum);
                }
            }

            return oldNumbers;
        }

        /// <summary>
        /// Удалить привязку объекта к объекту.
        /// </summary>
        /// <param name="objectNumbers">Список объектов</param>
        private void RemoveDeletedObjects(List<int> objectNumbers)
        {
            if (objectNumbers.Count == 0)
            {
                return;
            }

            foreach (var number in objectNumbers)
            {
                TechObject removingObject = TechObjectManager
                    .GetInstance().GetTObject(number);
                BaseTechObject removingBaseTechObject = removingObject
                    .BaseTechObject;
                List<BaseParameter> properties = removingBaseTechObject
                    .AggregateParameters;

                var deletingProperties = new List<BaseParameter>();
                if (properties.Count != 0)
                {
                    deletingProperties.AddRange(properties);
                }

                if (removingBaseTechObject.MainAggregateParameter != null)
                {
                    deletingProperties.Add(removingBaseTechObject
                        .MainAggregateParameter);
                }
                TechObject thisTechObject = owner;
                List<Mode> modes = thisTechObject.ModesManager.Modes;
                foreach (var mode in modes)
                {
                    mode.BaseOperation.RemoveProperties(deletingProperties);
                    mode.SetItems();
                }
            }
        }

        /// <summary>
        /// Инициализация данных 
        /// </summary>
        /// <param name="objectsNumbrers">Список корректных номеров
        /// привязанных объектов</param>
        private void InitAttachedObjects(List<int> objectsNumbrers)
        {
            foreach (var number in objectsNumbrers)
            {
                TechObject attachedAggregate = TechObjectManager
                    .GetInstance().GetTObject(number);
                BaseTechObject attachedBaseTechObject = attachedAggregate
                    .BaseTechObject;
                List<BaseParameter> properties = attachedBaseTechObject
                    .AggregateParameters;

                var addingProperties = new List<BaseParameter>();
                if (properties.Count != 0)
                {
                    addingProperties.AddRange(properties);
                }

                if (attachedBaseTechObject.MainAggregateParameter != null)
                {
                    addingProperties.Add(attachedBaseTechObject
                        .MainAggregateParameter);
                }
                TechObject thisThechObject = owner;
                List<Mode> modes = thisThechObject.ModesManager.Modes;
                foreach (var mode in modes)
                {
                    if (mode.BaseOperation.Name == string.Empty)
                    {
                        continue;
                    }

                    mode.BaseOperation.AddProperties(addingProperties,
                        attachedBaseTechObject);
                    mode.BaseOperation.Check();
                    mode.SetItems();
                }
            }
        }

        /// <summary>
        /// Проверить привязанные объекты на инициализацию и 
        /// инициализировать, если не инициализировано.
        /// </summary>
        public string Check()
        {
            var res = "";
            var objNum = TechObjectManager.GetInstance().GetTechObjectN(Owner);
            List<int> numbers = strategy.GetValidTechObjNums(Value, objNum);
            string checkedValue = string.Join(" ", numbers);
            if (checkedValue != Value)
            {
                int objGlobalNumber = TechObjectManager.GetInstance()
                    .GetTechObjectN(owner);
                res += $"Проверьте {Name.ToLower()} в объекте: " +
                    $"{objGlobalNumber}." +
                    $"{owner.Name + owner.TechNumber}. " +
                    $"В поле присутствуют агрегаты, которые нельзя " +
                    $"привязывать.\n";
            }

            InitAttachedObjects(numbers);
            return res;
        }

        private string GenerateAttachedObjectsString()
        {
            string value = Value ?? string.Empty;
            if (value == string.Empty)
            {
                return value;
            }

            var objectNums = value.Split(' ').Select(int.Parse);
            var objectNames = new List<string>();
            foreach (var objNum in objectNums)
            {
                TechObject findedObject = TechObjectManager.GetInstance()
                    .GetTObject(objNum);
                if (findedObject != null)
                {
                    string name = $"\"{findedObject.Name} " +
                        $"{findedObject.TechNumber}\"";
                    objectNames.Add(name);
                }
            }

            return string.Join(", ", objectNames);
        }

        public List<string> GetAttachedObjectsName()
        {
            var objectNames = new List<string>();
            if (Value == string.Empty)
            {
                return objectNames;
            }

            string[] nums = Value.Split(' ');
            foreach (var num in nums)
            {
                bool converted = int.TryParse(num, out int objNum);
                if(converted)
                {
                    var obj = TechObjectManager.GetInstance()
                        .GetTObject(objNum);
                    string objName = obj.NameEplanForFile.ToLower() + 
                        obj.TechNumber;
                    objectNames.Add(objName);
                }
            }

            return objectNames;
        }

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                return new string[]
                {
                        Name,
                        GenerateAttachedObjectsString()
                };
            }
        }

        public override bool NeedRebuildParent
        {
            get
            {
                return true;
            }
        }

        public override int[] EditablePart
        {
            get
            {
                return new int[] { -1, -1 };
            }
        }

        override public bool IsEditable
        {
            get
            {
                BaseTechObject baseTechObject = owner?.BaseTechObject;
                bool editable = baseTechObject?.IsAttachable == true;
                return editable;
            }
        }

        public override bool IsFilled
        {
            get
            {
                return Value != string.Empty;
            }
        }

        public override bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        public override bool Delete(object child)
        {
            bool cleared = SetNewValue(string.Empty);
            return cleared;
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }
        #endregion

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

        public IAttachedObjectsStrategy WorkStrategy 
        { 
            get => strategy; 
        }

        private TechObject owner;
        private IAttachedObjectsStrategy strategy;
    }

    namespace AttachedObjectStrategy
    {
        /// <summary>
        /// Интерфейс стратегии фильтрации привязываемых объектов
        /// </summary>
        public interface IAttachedObjectsStrategy
        {
            /// <summary>
            /// Название поля
            /// </summary>
            string Name { get; set; }

            /// <summary>
            /// Lua-имя стратегии
            /// </summary>
            string LuaName { get; set; }

            /// <summary>
            /// Получить корректные номера технологических объектов из
            /// входной строки
            /// </summary>
            /// <param name="value">Входная строка</param>
            /// <param name="objNum">Номер редактируемого объекта</param>
            /// <returns></returns>
            List<int> GetValidTechObjNums(string value, int objNum);

            /// <summary>
            /// Нужно ли инициализировать привязанные объекты
            /// </summary>
            bool UseInitialization { get; }

            List<BaseTechObjectManager.ObjectType> AllowedObjects { get; set; }
        }

        /// <summary>
        /// Стратегия для привязки объектов с их инициализацией в объекте.
        /// </summary>
        public class AttachedWithInitStrategy : BaseStrategy,
            IAttachedObjectsStrategy
        {
            public AttachedWithInitStrategy(string name, string luaName,
                List<BaseTechObjectManager.ObjectType> allowedObjects)
                : base(name, luaName, allowedObjects)
            { }

            public bool UseInitialization
            {
                get
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Стратегия для привязки объектов без их инициализации в объекте.
        /// </summary>
        public class AttachedWithoutInitStrategy : BaseStrategy,
            IAttachedObjectsStrategy
        {
            public AttachedWithoutInitStrategy(string name, string luaName,
                List<BaseTechObjectManager.ObjectType> allowedObjects)
                : base(name, luaName, allowedObjects)
            { }

            public bool UseInitialization
            {
                get
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Базовая стратегия проверки разрешенных объектов при привязке 
        /// объектов
        /// </summary>
        public abstract class BaseStrategy
        {
            public BaseStrategy(string name, string luaName,
                List<BaseTechObjectManager.ObjectType> allowedObjects)
            {
                Name = name;
                LuaName = luaName;
                AllowedObjects = allowedObjects;
            }

            /// <summary>
            /// Получить корректные номера технологических объектов из
            /// входной строки
            /// </summary>
            /// <param name="value">Входная строка</param>
            /// <param name="selectedObjNum">Номер редактируемого объекта
            /// <returns></returns>
            public List<int> GetValidTechObjNums(string value,
                int selectedObjNum)
            {
                var numbers = new List<int>();
                string[] numbersAsStringArray = value.Split(' ').ToArray();

                List<int> allowedObjectsNums = AllowedObjects?
                    .Select(x => (int)x).ToList();
                foreach (var numAsString in numbersAsStringArray)
                {
                    int.TryParse(numAsString, out int number);
                    if (number <= 0)
                    {
                        continue;
                    }

                    TechObject obj = TechObjectManager.GetInstance()
                        .GetTObject(number);
                    if (obj.BaseTechObject == null)
                    {
                        return new List<int>();
                    }

                    var objValues = obj?.AttachedObjects.Value.Split(' ')
                        .Where(x => int.TryParse(x, out _))
                        .Select(x => int.Parse(x)).ToList();
                    if (objValues?.Contains(selectedObjNum) == true)
                    {
                        continue;
                    }

                    bool correctBaseObject = allowedObjectsNums
                        .Contains(obj.BaseTechObject.S88Level);
                    if (correctBaseObject)
                    {
                        numbers.Add(number);
                    }
                }

                numbers = numbers.Distinct().ToList();
                return numbers;
            }

            /// <summary>
            /// Отображаемое имя группы
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Lua-имя группы
            /// </summary>
            public string LuaName { get; set; }

            /// <summary>
            /// Разрешенные для добавления в группу объекты
            /// </summary>
            public List<BaseTechObjectManager.ObjectType> AllowedObjects { get; set; }
        }
    }
}
