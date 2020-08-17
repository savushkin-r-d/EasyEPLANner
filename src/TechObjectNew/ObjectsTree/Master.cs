using NewEditor;
using System.Collections.Generic;

namespace NewTechObject
{
    /// <summary>
    /// Объект мастера проектов.
    /// </summary>
    public class Master : TreeViewItem
    {
        public Master() 
        {
            objects = new List<TechObject>();
            baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObject(name);
        }

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении объекта
        /// </summary>
        public void CheckRestriction(int prev, int curr)
        {
            foreach (TechObject to in objects)
            {
                to.CheckRestriction(prev, curr);
            }
        }

        /// <summary>
        /// Изменение номеров владельцев ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (TechObject to in objects)
            {
                to.SetRestrictionOwner();
            }
        }

        #region реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if(Items.Length > 0)
                {
                    return new string[] { $"{name} ({objects.Count})", "" };
                }
                else
                {
                    return new string[] { name, "" };
                }
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
                var newObject = new TechObject(name, GetTechObjectLocalNum, 
                    1, 1, "MASTER", -1, "MasterObj", "", baseTechObject);

                // Работа со списком в дереве и общим списком объектов.
                objects.Add(newObject);
                TechObjectManager.GetInstance().TechObjectsList
                    .Add(newObject);

                return newObject;
            }
            else
            {
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
            var techObject = child as TechObject;
            if (techObject != null)
            {
                //if (techObject.BaseTechObject.IsAttachable)
                //{
                //    RemoveAttachingToUnit(techObject);
                //}

                int idx = TechObjectManager.GetInstance().TechObjectsList
                    .IndexOf(techObject) + 1;
                CheckRestriction(idx, -1);

                // Работа со списком в дереве и общим списком объектов.
                objects.Remove(techObject);
                TechObjectManager.GetInstance().TechObjectsList
                    .Remove(techObject);

                SetRestrictionOwner();
                //ChangeAttachedObjectsAfterDelete(idx);

                if(objects.Count == 0)
                {
                    Parent.Delete(this);
                }
                return true;
            }

            return false;
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
            if (techObj != null &&
                techObj.BaseTechObject.Name == name)
            {
                int newN = 1;
                if (objects.Count > 0)
                {
                    newN = objects[objects.Count - 1].TechNumber + 1;
                }

                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjN = TechObjectManager.GetInstance()
                    .GetTechObjectN(obj as TechObject);
                int newObjN = TechObjectManager.GetInstance()
                    .TechObjectsList.Count + 1;

                TechObject newObject = (obj as TechObject).Clone(
                    GetTechObjectLocalNum, newN, oldObjN, newObjN);

                // Работа со списком в дереве и общим списком объектов.
                objects.Add(newObject);
                TechObjectManager.GetInstance().TechObjectsList.Add(newObject);

                newObject.ChangeCrossRestriction();
                newObject.Equipment.ModifyDevNames();

                return newObject;
            }

            return null;
        }

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            var techObject = child as TechObject;
            var copiedObject = copyObject as TechObject;
            if (copiedObject != null && techObject != null &&
                copiedObject.BaseTechObject.Name == baseTechObject.Name)
            {
                int newN = techObject.TechNumber;

                //Старый и новый номер объекта - для замены в ограничениях
                int oldObjN = TechObjectManager.GetInstance()
                    .GetTechObjectN(copyObject as TechObject);
                int newObjN = TechObjectManager.GetInstance()
                    .GetTechObjectN(child as TechObject);

                TechObject newObject = (copyObject as TechObject).Clone(
                    GetTechObjectLocalNum, newN, oldObjN, newObjN);

                // Работа со списком в дереве и общим списком объектов.
                int index = objects.IndexOf(techObject);
                objects.Remove(techObject);
                objects.Insert(index, newObject);
                var allTechObjectsList = TechObjectManager.GetInstance()
                    .TechObjectsList;
                int indexInAllObjectsTree = allTechObjectsList
                    .IndexOf(techObject);
                allTechObjectsList.Remove(techObject);
                allTechObjectsList.Insert(indexInAllObjectsTree, newObject);

                newObject.ChangeCrossRestriction(techObject);

                return newObject;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Мастер-объект проекта.
        /// </summary>
        public TechObject MasterObject
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

        string name = "Мастер";
        List<TechObject> objects;
        BaseTechObject baseTechObject;
    }
}
