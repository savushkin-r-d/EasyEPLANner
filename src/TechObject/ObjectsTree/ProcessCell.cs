using Editor;
using System.Collections.Generic;

namespace TechObject
{
    /// <summary>
    /// Объект ячейки процесса (мастера).
    /// </summary>
    public class ProcessCell : TreeViewItem, IBaseObjChangeable
    {
        public ProcessCell(ITechObjectManager techObjectManager) 
        {
            objects = new List<TechObject>();
            baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObjectCopy(Name);
            this.techObjectManager = techObjectManager;
            globalObjectsList = this.techObjectManager.TechObjects;
        }

        /// <summary>
        /// Изменение номеров владельцев ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (TechObject techObject in objects)
            {
                techObject.SetRestrictionOwner();
            }
        }

        /// <summary>
        /// Добавление объекта при загрузке из LUA
        /// </summary>
        /// <param name="obj">Объект</param>
        public void AddObjectWhenLoadFromLua(TechObject obj)
        {
            obj.SetGetLocalN(GetTechObjectLocalNum);
            objects.Add(obj);
        }

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if(Items.Length > 0)
                {
                    return new string[] { $"{Name} ({objects.Count})", "" };
                }
                else
                {
                    return new string[] { Name, "" };
                }
            }
        }

        public override string[] EditText
        {
            get
            {
                return new string[] { Name, "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        public override ITreeViewItem Insert()
        {
            if(Items.Length == 0)
            {
                var newObject = new TechObject(Name, GetTechObjectLocalNum, 
                    1, 1, "MASTER", -1, "MasterObj", "", baseTechObject);

                // Работа со списком в дереве и общим списком объектов.
                objects.Add(newObject);
                globalObjectsList.Add(newObject);

                // Обозначение начального номера объекта для ограничений.
                SetRestrictionOwner();

                newObject.SetUpFromBaseTechObject();

                newObject.AddParent(this);
                return newObject;
            }
            else
            {
                ObjectsAdder.Reset();
                return null;
            }
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            const int markAsDelete = -1;
            var techObject = child as TechObject;
            if (techObject != null)
            {
                int globalIndex = globalObjectsList.IndexOf(techObject) + 1;
                techObjectManager.CheckRestriction(globalIndex, markAsDelete);

                // Работа со списком в дереве и общим списком объектов.
                objects.Remove(techObject);
                globalObjectsList.Remove(techObject);

                // Обозначение начального номера объекта для ограничений.
                SetRestrictionOwner();

                techObjectManager.ChangeAttachedObjectsAfterDelete(globalIndex);

                if (objects.Count == 0)
                {
                    Parent.Delete(this);
                }
                return true;
            }

            return false;
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem InsertCopy(object obj)
        {
            var techObj = obj as TechObject;
            if(techObj == null)
            {
                return null;
            }
            
            bool processCellNotAdd = objects.Count == 0;
            bool correctedBaseObject = techObj.BaseTechObject != null &&
                techObj.BaseTechObject.Name == Name;
            if (correctedBaseObject && processCellNotAdd)
            {
                int newN = 1;
                if (objects.Count > 0)
                {
                    newN = objects[objects.Count - 1].TechNumber + 1;
                }
                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjNum = globalObjectsList
                    .IndexOf(obj as TechObject) + 1;
                int newObjNum = globalObjectsList.Count + 1;

                var newObject = (obj as TechObject).Clone(GetTechObjectLocalNum,
                    newN, oldObjNum, newObjNum);

                // Работа со списком в дереве и общим списком объектов.
                objects.Add(newObject);
                globalObjectsList.Add(newObject);

                // Для корректного копирования ограничений
                newObject.ChangeCrossRestriction();

                newObject.Equipment.ModifyDevNames();

                newObject.AddParent(this);
                return newObject;
            }
            else
            {
                ObjectsAdder.Reset();
                if (techObj.MarkToCut && processCellNotAdd)
                {
                    return InsertCuttedCopy(techObj);
                }
            }

            return null;
        }

        /// <summary>
        /// Вставка вырезанного объекта
        /// </summary>
        /// <param name="techObj">Объект</param>
        /// <returns></returns>
        private TechObject InsertCuttedCopy(TechObject techObj)
        {
            var techObjParent = techObj.Parent;
            techObjParent.Cut(techObj);

            objects.Add(techObj);
            techObj.SetGetLocalN(GetTechObjectLocalNum);
            techObj.InitBaseTechObject(baseTechObject);
            techObj.AddParent(this);
            return techObj;
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var techObject = child as TechObject;
            var copiedObject = copyObject as TechObject;
            bool objectsNotNull = copiedObject != null && techObject != null;
            bool sameBaseObjectName =
                copiedObject?.BaseTechObject.Name == baseTechObject.Name;
            if (objectsNotNull && sameBaseObjectName)
            {
                int newNum = techObject.TechNumber;
                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjNum = globalObjectsList
                    .IndexOf(copyObject as TechObject) + 1;
                int newObjNum = globalObjectsList
                    .IndexOf(child as TechObject) + 1;

                TechObject newObject = (copyObject as TechObject).Clone(
                    GetTechObjectLocalNum, newNum, oldObjNum, newObjNum);

                // Работа со списком в дереве
                int localIndex = objects.IndexOf(techObject);
                objects.Remove(techObject);
                objects.Insert(localIndex, newObject);
                
                // Глобальный список объектов
                int globalIndex = globalObjectsList.IndexOf(techObject);
                globalObjectsList.Remove(techObject);
                globalObjectsList.Insert(globalIndex, newObject);

                // Для корректного копирования ограничений
                newObject.ChangeCrossRestriction(techObject);

                newObject.AddParent(this);
                return newObject;
            }

            return null;
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }
        #endregion

        /// <summary>
        /// Ячейка процесса (мастер-объект) проекта.
        /// </summary>
        public TechObject ProcessCellObject
        {
            get
            {
                if(objects[0] != null)
                {
                    return objects[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Получить локальный номер технологического объекта
        /// </summary>
        /// <param name="searchingObject">Искомый объект</param>
        /// <returns></returns>
        private int GetTechObjectLocalNum(object searchingObject)
        {
            var techObject = searchingObject as TechObject;
            int num = objects.IndexOf(techObject) + 1;
            return num;
        }

        public void ChangeBaseObj(ITreeViewItem treeItem)
        {
            var techObject = treeItem as TechObject;
            bool success = techObjectManager.ChangeBaseObject(techObject);
            if (success)
            {
                objects.Remove(techObject);
                if (objects.Count == 0)
                {
                    Parent.Delete(this);
                }
            }
        }

        public const string Name = "Ячейка процесса";

        List<TechObject> objects;
        BaseTechObject baseTechObject;
        List<TechObject> globalObjectsList;
        ITechObjectManager techObjectManager;
    }
}
