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
            List<TechObject> oldObjects = GetValidTechObjNums(oldValue)
                .Select(x => TechObjectManager.GetInstance().GetTObject(x))
                .ToList();

            List<int> newNumbers = GetValidTechObjNums(newValue);
            List<TechObject> newObjects = newNumbers
                .Select(x => TechObjectManager.GetInstance().GetTObject(x))
                .ToList();
            newValue = string.Join(" ", newNumbers);

            base.SetNewValue(newValue);

            Dictionary<TechObject, TechObject> replacedObjects =
                CheckReplacedObjects(oldObjects, newObjects);
            ChangeOwnerInReplacedObjectsProperties(replacedObjects);
            List<int> deletedAgregatesNumbers = FindDeletedObjects(
                oldValue, newValue, replacedObjects);
            RemoveDeletedObjects(deletedAgregatesNumbers);

            InitAttachedObjects(newNumbers);
            return true;
        }

        public override bool SetNewValue(
            SortedDictionary<int, List<int>> newDict)
        {
            var objectsList = new List<int>();
            foreach (var objectNumber in newDict.Keys)
            {
                objectsList.Add(objectNumber);
            }
            objectsList.Sort();

            return SetNewValue(string.Join(" ", objectsList));
        }

        /// <summary>
        /// Получить корректные номера технологических объектов из
        /// входной строки
        /// </summary>
        /// <param name="inputString">Входная строка</param>
        /// <returns></returns>
        private List<int> GetValidTechObjNums(string inputString)
        {
            var numbers = new List<int>();
            string[] numbersAsStringArray = inputString.Split(' ')
                .ToArray();

            foreach (var numAsString in numbersAsStringArray)
            {
                int number;
                int.TryParse(numAsString, out number);
                if (number == 0)
                {
                    continue;
                }

                TechObject obj = TechObjectManager.GetInstance()
                    .GetTObject(number);
                bool correctBaseObject = obj.BaseTechObject != null &&
                    obj.BaseTechObject.IsAttachable;
                if (correctBaseObject)
                {
                    numbers.Add(number);
                }
            }

            numbers = numbers.Distinct().ToList();
            return numbers;
        }

        /// <summary>
        /// Проверить и найти замененные объекты
        /// </summary>
        /// <param name="oldObjects">Старый список объектов</param>
        /// <param name="newObjects">Новый список объектов</param>
        /// <returns></returns>
        private Dictionary<TechObject, TechObject> CheckReplacedObjects(
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

            foreach (var replacedObj in replacedObjects.Keys)
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
                    if (mode.BaseOperation.Name == "")
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
            List<int> numbers = GetValidTechObjNums(Value);
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
                bool baseObjectIsExist = owner.BaseTechObject != null;
                if (baseObjectIsExist)
                {
                    string baseObjectName = owner.BaseTechObject.Name;
                    if (allowedBaseObjects.Contains(baseObjectName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public override bool IsFilled
        {
            get
            {
                if (Value != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
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

        private string[] allowedBaseObjects = new string[]
        {
                "Танк",
                "Линия",
                "Линия приемки",
                "Линия выдачи"
        };

        public IAttachedObjectsStrategy WorkStrategy 
        { 
            get => strategy; 
        }

        private TechObject owner;
        private IAttachedObjectsStrategy strategy;
    }

    namespace AttachedObjectStrategy
    {
        public interface IAttachedObjectsStrategy
        {
            string Name { get; }
        }

        public class AttachedAggregatesStrategy : IAttachedObjectsStrategy
        {
            public AttachedAggregatesStrategy() { }

            public string Name
            {
                get => "Привязанные агрегаты";
            }
        }

        public class AttachedTanksStrategy : IAttachedObjectsStrategy
        {
            public AttachedTanksStrategy() { }

            public string Name
            {
                get => "Группа танков";
            }
        }
    }
}
