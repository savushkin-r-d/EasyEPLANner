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
            objects = new List<ITreeViewItem>();
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
            ObjectsAdder.Reset();
            var newObject = new TechObject(baseTechObject.Name,
                objects.Count + 1, 1, baseTechObject.EplanName.ToUpper(), -1,
                baseTechObject.EplanName, "");
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
        #endregion

        List<ITreeViewItem> objects;
        BaseTechObject baseTechObject;
    }
}
