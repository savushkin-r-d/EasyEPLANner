using System.Collections.Generic;
using Editor;

namespace TechObject
{
    class BaseObject : TreeViewItem, IBaseObjChangeable
    {
        public BaseObject(string baseTechObjectName,
            ITechObjectManager techObjectManager)
        {
            localObjects = new List<TechObject>();
            baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObjectCopy(baseTechObjectName);
            this.techObjectManager = techObjectManager;
            globalObjectsList = this.techObjectManager.TechObjects;
        }

        /// <summary>
        /// Изменение номеров владельцев ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (TechObject techObject in globalObjectsList)
            {
                techObject.SetRestrictionOwner();
            }
        }

        /// <summary>
        /// Добавить объект при загрузке из LUA
        /// </summary
        /// <param name="obj">Объект</param>
        public void AddObjectWhenLoadFromLua(TechObject obj)
        {
            obj.SetGetLocalN(GetTechObjectLocalNum);
            localObjects.Add(obj);
        }

        #region реализация ITreeViewItem
        public override ITreeViewItem[] Items
        {
            get
            {
                return localObjects.ToArray();
            }
        }

        public override string[] DisplayText
        {
            get
            {
                var name = baseTechObject.Name;
                if (localObjects.Count == 0)
                {
                    return new string[] { name, "" };
                }
                else
                {
                    return new string[] 
                    { 
                        $"{name} ({localObjects.Count})", 
                        "" 
                    };
                }
            }
        }

        public override string[] EditText
        {
            get
            {
                return new string[] { baseTechObject.Name , "" };
            }
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Insert()
        {
            const int techTypeNum = 2;
            const int cooperParamNum = -1;
            ObjectsAdder.Reset();
            var newObject = new TechObject(baseTechObject.Name, 
                GetTechObjectLocalNum, localObjects.Count + 1, techTypeNum, 
                "TANK", cooperParamNum, string.Empty, string.Empty,
                baseTechObject);

            // Работа со списком в дереве и общим списком объектов.
            localObjects.Add(newObject);
            globalObjectsList.Add(newObject);

            // Обозначение начального номера объекта для ограничений.
            SetRestrictionOwner();

            newObject.SetUpFromBaseTechObject();

            newObject.AddParent(this);
            return newObject;
        }

        override public bool Delete(object child)
        {
            const int markAsDelete = -1;
            var techObject = child as TechObject;
            if (techObject != null)
            {
                if (techObject.BaseTechObject.IsAttachable)
                {
                    techObjectManager.RemoveAttachingToObjects(techObject);
                }

                int globalNum = globalObjectsList.IndexOf(techObject) + 1;
                techObjectManager.CheckRestriction(globalNum, markAsDelete);

                // Работа со списком в дереве и общим списком объектов.
                localObjects.Remove(techObject);
                globalObjectsList.Remove(techObject);

                // Обозначение начального номера объекта для ограничений.
                SetRestrictionOwner();

                techObjectManager.ChangeAttachedObjectsAfterDelete(globalNum);

                if (localObjects.Count == 0)
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

        override public ITreeViewItem MoveDown(object child)
        {
            var techObject = child as TechObject;

            if (techObject != null)
            {
                int oldLocalIndex = localObjects.IndexOf(techObject);
                int lastItemNum = localObjects.Count - 1;
                if (oldLocalIndex < lastItemNum)
                {
                    int newLocalIndex = oldLocalIndex + 1; 
                    int oldGlobalIndex = globalObjectsList.IndexOf(techObject);
                    int newGlobalIndex = globalObjectsList
                        .IndexOf(localObjects[newLocalIndex]);

                    techObjectManager.CheckRestriction(
                        oldGlobalIndex + 1, newGlobalIndex + 1);

                    // Работа со списком в дереве
                    localObjects.Remove(techObject);
                    localObjects.Insert(newLocalIndex, techObject);

                    // Глобальный список объектов
                    var temporary = globalObjectsList[oldGlobalIndex];
                    globalObjectsList[oldGlobalIndex] =
                        globalObjectsList[newGlobalIndex];
                    globalObjectsList[newGlobalIndex] = temporary;

                    // Обозначение начального номера объекта для ограничений.
                    SetRestrictionOwner();

                    techObjectManager.ChangeAttachedObjectsAfterMove(
                        oldGlobalIndex + 1, newGlobalIndex + 1);

                    localObjects[newLocalIndex].AddParent(this);
                    return localObjects[newLocalIndex];
                }
            }

            return null;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var techObject = child as TechObject;

            if (techObject != null)
            {
                int oldLocalIndex = localObjects.IndexOf(techObject);
                if (oldLocalIndex > 0)
                {
                    int newLocalIndex = oldLocalIndex - 1;
                    int oldGlobalIndex = globalObjectsList.IndexOf(techObject);
                    int newGlobalIndex = globalObjectsList
                        .IndexOf(localObjects[newLocalIndex]);

                    techObjectManager.CheckRestriction(
                        oldGlobalIndex + 1, newGlobalIndex + 1);

                    // Работа со списком в дереве
                    localObjects.Remove(techObject);
                    localObjects.Insert(newLocalIndex, techObject);

                    // Глобальный список
                    var temporary = globalObjectsList[oldGlobalIndex];
                    globalObjectsList[oldGlobalIndex] =
                        globalObjectsList[newGlobalIndex];
                    globalObjectsList[newGlobalIndex] = temporary;

                    // Обозначение начального номера объекта для ограничений.
                    SetRestrictionOwner();

                    techObjectManager.ChangeAttachedObjectsAfterMove(
                        oldGlobalIndex + 1, newGlobalIndex + 1);

                    localObjects[newLocalIndex].AddParent(this);
                    return localObjects[newLocalIndex];
                }
            }

            return null;
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

            if (techObj.BaseTechObject != null &&
                techObj.BaseTechObject.Name == baseTechObject.Name)
            {
                int newN = 1;
                if (localObjects.Count > 0)
                {
                    newN = localObjects[localObjects.Count - 1].TechNumber + 1;
                }

                int oldObjN = globalObjectsList.IndexOf(techObj) + 1;
                int newObjN = globalObjectsList.Count + 1;

                var newObject = CloneObject(techObj, newN, oldObjN, newObjN);

                // Работа со списком в дереве и общим списком объектов.
                localObjects.Add(newObject);
                globalObjectsList.Add(newObject);

                newObject.ChangeCrossRestriction();
                newObject.Equipment.ModifyDevNames();

                newObject.AddParent(this);
                return newObject;
            }
            else
            {
                ObjectsAdder.Reset();
                if (techObj.MarkToCut)
                {
                    return InsertCuttedCopy(techObj);
                }
            }

            return null;
        }

        /// <summary>
        /// Вставить вырезанный объект
        /// </summary>
        /// <param name="techObj">Объект</param>
        /// <returns></returns>
        private TechObject InsertCuttedCopy(TechObject techObj)
        {
            var techObjParent = techObj.Parent;
            techObjParent.Cut(techObj);

            localObjects.Add(techObj);
            techObj.SetGetLocalN(GetTechObjectLocalNum);
            techObj.InitBaseTechObject(baseTechObject);
            techObj.AddParent(this);
            return techObj;
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var techObject = child as TechObject;
            var copiedObject = copyObject as TechObject;
            bool objectsNotNull = (copiedObject != null && techObject != null);
            bool sameBaseObjectName = 
                copiedObject?.BaseTechObject.Name == baseTechObject.Name;
            if (objectsNotNull && sameBaseObjectName)
            {
                int newN = techObject.TechNumber;
                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjNum = globalObjectsList
                    .IndexOf(copiedObject) + 1;
                int newObjNum = globalObjectsList
                    .IndexOf(techObject) + 1;

                var newObject = CloneObject(copyObject, newN, oldObjNum,
                    newObjNum);

                int localIndex = localObjects.IndexOf(techObject);
                // Работа со списком в дереве
                localObjects.Remove(techObject);
                localObjects.Insert(localIndex, newObject);

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

        /// <summary>
        /// Клонирование объекта (копирование)
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="newN">Номер внутренний объекта</param>
        /// <param name="oldObjNum">Старый глобальный номер</param>
        /// <param name="newObjNum">Новый глобальный номер</param>
        /// <returns></returns>
        private TechObject CloneObject(object obj, int newN, int oldObjNum,
            int newObjNum)
        {
            var techObject = obj as TechObject;
            var stubObject = techObject.Clone(GetTechObjectLocalNum, newN,
                    oldObjNum, newObjNum);

            bool isInsertCopy = newObjNum > globalObjectsList.Count;
            if (isInsertCopy)
            {
                return CloneForInsert(techObject, stubObject, newN, oldObjNum,
                    newObjNum);
            }
            else
            {
                return CloneForReplace(techObject, stubObject, newN, oldObjNum,
                    newObjNum);
            }
        }

        /// <summary>
        /// Клонирование для вставки
        /// </summary>
        /// <param name="cloningObject">Клонируемый объект</param>
        /// <param name="stubObject">Объект-заглушка</param>
        /// <param name="newN">Номер внутренний объекта</param>
        /// <param name="oldObjNum">Старый глобальный номер</param>
        /// <param name="newObjNum">Новый глобальный номер</param>
        /// <returns></returns>
        private TechObject CloneForInsert(TechObject cloningObject,
            TechObject stubObject, int newN, int oldObjNum, int newObjNum)
        {
            globalObjectsList.Add(stubObject);
            var clonedObject = cloningObject.Clone(GetTechObjectLocalNum, newN,
                oldObjNum, newObjNum);
            globalObjectsList.Remove(stubObject);
            return clonedObject;
        }

        /// <summary>
        /// Клонирование для замены
        /// </summary>
        /// <param name="cloningObject">Клонируемый объект</param>
        /// <param name="stubObject">Объект-заглушка</param>
        /// <param name="newN">Номер внутренний объекта</param>
        /// <param name="oldObjNum">Старый глобальный номер</param>
        /// <param name="newObjNum">Новый глобальный номер</param>
        /// <returns></returns>
        private TechObject CloneForReplace(TechObject cloningObject,
            TechObject stubObject, int newN, int oldObjNum, int newObjNum)
        {
            var objectFromGlobalList = globalObjectsList[newObjNum - 1];
            globalObjectsList[newObjNum - 1] = stubObject;
            var clonedObject = cloningObject.Clone(GetTechObjectLocalNum, newN,
                oldObjNum, newObjNum);
            globalObjectsList[newObjNum - 1] = objectFromGlobalList;
            return clonedObject;
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
        /// Получить локальный номер технологического объекта
        /// </summary>
        /// <param name="searchingObject">Искомый объект</param>
        /// <returns></returns>
        private int GetTechObjectLocalNum(object searchingObject)
        {
            var techObject = searchingObject as TechObject;
            int num = localObjects.IndexOf(techObject) + 1;
            return num;
        }

        public void ChangeBaseObj(ITreeViewItem treeItem)
        {
            var techObject = treeItem as TechObject;
            bool success = techObjectManager.ChangeBaseObject(techObject);
            if (success)
            {
                localObjects.Remove(techObject);
                if (localObjects.Count == 0)
                {
                    Parent.Delete(this);
                }
            }
        }

        List<TechObject> localObjects;
        BaseTechObject baseTechObject;
        List<TechObject> globalObjectsList;
        ITechObjectManager techObjectManager;
    }
}
