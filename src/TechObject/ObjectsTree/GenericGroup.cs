using Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGenericObject
    {

    }

    /// <summary>
    /// Типовой (логический) объект,
    /// определяющий базовую функциональность
    /// связанных физических объектов
    /// </summary>
    public class GenericGroup : TreeViewItem, IGenericObject
    {
        /// <summary>
        /// Создание новой группы на основе типовго объекта 
        /// (используется при загрузке из LUA)
        /// </summary>
        /// <param name="genericTechObject"></param>
        /// <param name="baseObject"></param>
        public GenericGroup(GenericTechObject genericTechObject, BaseObject baseObject,
            ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
            this.genericTechObject = genericTechObject;
            genericTechObject.GenericGroup = this;
            this.baseObject = baseObject;
        }

        /// <summary>
        /// Создание новой группы и типового на основе технологического объекта
        /// </summary>
        /// <param name="techObject"> Тех. объект </param>
        /// <param name="baseObject"> базовый объект </param>
        public GenericGroup(TechObject techObject, BaseObject baseObject,
            ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
            genericTechObject = new GenericTechObject(techObject);
            genericTechObject.GenericGroup = this;
            this.baseObject = baseObject;
        }


        public override string[] DisplayText
            => new string[] { $"{genericTechObject.Name} ({Items.Count() - 1})", string.Empty };

        public override ITreeViewItem[] Items
            => (new ITreeViewItem[] { genericTechObject }).Concat(InheritedTechObjects).ToArray();


        public override bool IsInsertableCopy => true;

        public override ITreeViewItem InsertCopy(object obj)
        {
            var techObject = obj as TechObject;
            
            if (techObject is null)
                return null;

            if (techObject.MarkToCut)
                return InsertCuttedCopy(techObject);


            var clone = techObject.Clone(baseObject.GetTechObjectLocalNum,
                InheritedTechObjects.Last().TechNumber + 1,
                techObjectManager.TechObjects.IndexOf(techObject) + 1,
                techObjectManager.TechObjects.Count + 1);

            baseObject.AddLocalObject(clone);
            techObjectManager.TechObjects.Add(clone);
            genericTechObject.SetUpTechObject(clone, baseObject);
            clone.AddParent(this);

            genericTechObject.Update();

            return clone;
        }

        public TechObject InsertCuttedCopy(TechObject techObject)
        {
            if (techObject.Parent != baseObject)
                return null;

            techObject.Parent.Cut(techObject);

            techObject.AddParent(this);
            genericTechObject.SetUpTechObject(techObject, baseObject);

            genericTechObject.Update();

            return techObject;
        }

        public override bool IsDeletable => true;

        public override bool ShowWarningBeforeDelete => true;

        public override bool Delete(object child)
        {
            if (child is TechObject techObject)
            {
                if (techObject.BaseTechObject.IsAttachable)
                {
                    techObjectManager.RemoveAttachingToObjects(techObject);
                }

                int globalNum = techObjectManager.GetTechObjectN(techObject);
                techObjectManager.CheckRestriction(globalNum, -1);

                baseObject.RemoveLocalObject(techObject);
                InheritedTechObjects.Remove(techObject);
                techObjectManager.TechObjects.Remove(techObject);

                baseObject.SetRestrictionOwner();

                techObjectManager.ChangeAttachedObjectsAfterDelete(globalNum);

                //ReindexingObjects();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверка и нумерации объектов в группе после удаленияэлемента
        /// </summary>
        public void ReindexingObjects()
        {
            int techNumber = 0;
            foreach (var techObject in InheritedTechObjects)
            {
                techNumber++;
                if (techObject.TechNumber == techNumber)
                    continue;

                techObject.TechNumber = techNumber;
                techObject.ModifyDevNames(0);
            }
        }

        public override bool IsInsertable => true;

        /// <summary>
        /// Создание тех. объека на основе типового объекта
        /// </summary>
        public override ITreeViewItem Insert()
        {
            var techObject = genericTechObject.CreateTechObject(baseObject);

            techObject.AddParent(this);
            techObject.TechNumber = InheritedTechObjects.Last().TechNumber + 1;

            //inheritedTechObjects.Add(techObject);

            return techObject;
        }

        /// <summary>
        /// Добавить технологический объект в группу
        /// (при загрузке из LUA)
        /// </summary>
        /// <param name="techObject"></param>
        public void AddTechObjectWhenLoadFromLua(TechObject techObject)
        {
            GenericTechObject.SetUpTechObject(techObject, baseObject);
            techObject.AddParent(this);
            //inheritedTechObjects.Add(techObject);
        }


        private ITechObjectManager techObjectManager;

        /// <summary>
        /// Список тех. объектов в группе
        /// </summary>
        public List<TechObject> InheritedTechObjects => genericTechObject.InheritedTechObjects;

        /// <summary>
        /// Типовой объект группы
        /// </summary>
        public GenericTechObject GenericTechObject => genericTechObject;

        //private List<TechObject> inheritedTechObjects = new List<TechObject>();
        private GenericTechObject genericTechObject;
        private BaseObject baseObject;
    }
}
