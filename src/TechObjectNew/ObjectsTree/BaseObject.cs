using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEditor;

namespace NewTechObject
{
    class BaseObject : TreeViewItem
    {
        public BaseObject(string baseTechObjectName)
        {
            objects = new List<TechObject>();
            baseTechObject = BaseTechObjectManager.GetInstance()
                .GetTechObject(baseTechObjectName);
        }

        #region реализация ITreeViewItem
        public override ITreeViewItem[] Items
        {
            get
            {
                return objects.ToArray();
            }
        }

        public override string[] DisplayText
        {
            get
            {
                var name = baseTechObject.Name;
                if (objects.Count == 0)
                {
                    return new string[] { name, "" };
                }
                else
                {
                    return new string[] { $"{name} ({objects.Count})", "" };
                }
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
            //TODO: set TechType
            //TODO: nameBC set
            //TODO: set TechNumber (prevObj + 1)
            ObjectsAdder.Reset();
            var newObject = new TechObject(baseTechObject.Name, GetTechObjectN,
                objects.Count + 1, 1, baseTechObject.EplanName.ToUpper(), -1,
                baseTechObject.EplanName, "", baseTechObject);
            objects.Add(newObject);
            return newObject;
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

                int idx = objects.IndexOf(techObject) + 1;
                //CheckRestriction(idx, -1);

                objects.Remove(techObject);
                //SetRestrictionOwner();
                //ChangeAttachedObjectsAfterDelete(idx);

                if (objects.Count == 0)
                {
                    Parent.Delete(this);
                }
                return true;
            }

            return false;
        }

        override public ITreeViewItem MoveDown(object child)
        {
            var techObject = child as TechObject;

            if (techObject != null)
            {
                int index = objects.IndexOf(techObject);
                if (index <= objects.Count - 2)
                {
                    //CheckRestriction(index + 1, index + 2);

                    objects.Remove(techObject);
                    objects.Insert(index + 1, techObject);

                    //SetRestrictionOwner();
                    //ChangeAttachedObjectsAfterMove(index, index + 1);
                    return objects[index];
                }
            }

            return null;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var techObject = child as TechObject;

            if (techObject != null)
            {
                int index = objects.IndexOf(techObject);
                if (index > 0)
                {
                    //CheckRestriction(index + 1, index);

                    objects.Remove(techObject);
                    objects.Insert(index - 1, techObject);

                    //SetRestrictionOwner();
                    //ChangeAttachedObjectsAfterMove(index, index - 1);
                    return objects[index];
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
            if (techObj != null &&
                techObj.BaseTechObject.Name == baseTechObject.Name)
            {
                int newN = 1;
                if (objects.Count > 0)
                {
                    newN = objects[objects.Count - 1].TechNumber + 1;
                }

                //Старый и новый номер объекта - для замены в ограничениях
                //int oldObjN = GetTechObjectN(obj as TechObject);
                //int newObjN = objects.Count + 1;

                TechObject newObject = (obj as TechObject).Clone(
                    GetTechObjectN, newN/*, oldObjN, newObjN*/);
                objects.Add(newObject);

                //newObject.ChangeCrossRestriction();
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
                //int oldObjN = GetTechObjectN(copyObject as TechObject);
                //int newObjN = GetTechObjectN(child as TechObject);

                TechObject newObject = (copyObject as TechObject).Clone(
                    GetTechObjectN, newN/*, oldObjN, newObjN*/);
                int index = objects.IndexOf(techObject);
                objects.Remove(techObject);
                objects.Insert(index, newObject);
                //newObject.ChangeCrossRestriction(techObject);

                return newObject;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Получить локальный номер технологического объекта
        /// </summary>
        /// <param name="searchingObject">Искомый объект</param>
        /// <returns></returns>
        private int GetTechObjectN(object searchingObject)
        {
            var techObject = searchingObject as TechObject;
            int num = objects.IndexOf(techObject) + 1;
            return num;
        }

        List<TechObject> objects;
        BaseTechObject baseTechObject;
    }
}
