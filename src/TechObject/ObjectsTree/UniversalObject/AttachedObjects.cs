using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<int> newNumbers = strategy.GetValidTechObjNums(newValue);
            
            newValue = string.Join(" ", newNumbers);
            base.SetNewValue(newValue);

            if (strategy.UseInitialization)
            {
                var replacedObjects = new Dictionary<TechObject, TechObject>();

                List<TechObject> newObjects = newNumbers
                    .Select(x => TechObjectManager.GetInstance().GetTObject(x))
                    .ToList();
                List<TechObject> oldObjects = strategy
                    .GetValidTechObjNums(oldValue)
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
            List<int> numbers = strategy.GetValidTechObjNums(Value);
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
            /// <returns></returns>
            List<int> GetValidTechObjNums(string value);

            /// <summary>
            /// Нужно ли инициализировать привязанные объекты
            /// </summary>
            bool UseInitialization { get; }
        }

        /// <summary>
        /// Стратегия для привязки агрегатов
        /// </summary>
        public class AttachedAggregatesStrategy : BaseStrategy,
            IAttachedObjectsStrategy
        {
            public AttachedAggregatesStrategy() : base() 
            {
                Name = "Привязанные агрегаты";
                LuaName = "attached_objects";
            }

            public List<int> GetValidTechObjNums(string value)
            {
                return GetValidTechObjNums(value, allowedObjects);
            }

            public bool UseInitialization
            {
                get
                {
                    return true;
                }
            }

            private List<BaseTechObjectManager.ObjectType> allowedObjects =
                new List<BaseTechObjectManager.ObjectType>() 
                {
                    BaseTechObjectManager.ObjectType.Aggregate
                };
        }

        /// <summary>
        /// Стратегия для привязки танков
        /// </summary>
        public class AttachedTanksStrategy : BaseStrategy,
            IAttachedObjectsStrategy
        {
            public AttachedTanksStrategy(string name = "", 
                string luaName = "") : base()
            {
                Name = name == string.Empty ? "Группа танков" : name;
                LuaName = luaName == string.Empty ? "tanks" : luaName;
            }

            public List<int> GetValidTechObjNums(string value)
            {
                return GetValidTechObjNums(value, allowedObjects);
            }

            public bool UseInitialization
            {
                get
                {
                    return false;
                }
            }

            private List<BaseTechObjectManager.ObjectType> allowedObjects =
                new List<BaseTechObjectManager.ObjectType>()
                {
                    BaseTechObjectManager.ObjectType.Unit
                };
        }

        /// <summary>
        /// Базовая стратегия проверки разрешенных объектов при привязке 
        /// объектов
        /// </summary>
        public abstract class BaseStrategy
        {
            public BaseStrategy() { }

            /// <summary>
            /// Получить корректные номера технологических объектов из
            /// входной строки
            /// </summary>
            /// <param name="value">Входная строка</param>
            /// <param name="allowedObjects">Разрешенные объекты по S88</param>
            /// <returns></returns>
            protected List<int> GetValidTechObjNums(string value,
                List<BaseTechObjectManager.ObjectType> allowedObjects)
            {
                var numbers = new List<int>();
                string[] numbersAsStringArray = value.Split(' ').ToArray();

                List<int> allowedObjectsNums = allowedObjects?
                    .Select(x => (int)x).ToList();
                foreach (var numAsString in numbersAsStringArray)
                {
                    int.TryParse(numAsString, out int number);
                    if (number == 0)
                    {
                        continue;
                    }

                    TechObject obj = TechObjectManager.GetInstance()
                        .GetTObject(number);
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
        }
    }
}
