using Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechObject;

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
        /// Создание новой группы с типовым объектом на основе базового объекта
        /// </summary>
        /// <param name="baseTechObject"></param>
        /// <param name="baseObject"></param>
        public GenericGroup(BaseTechObject baseTechObject, BaseObject baseObject,
            ITechObjectManager techObjectManager)
        {
            this.techObjectManager = techObjectManager;
            genericTechObject = new GenericTechObject(baseTechObject.Name, 2, string.Empty,
                -1, string.Empty, string.Empty, baseTechObject);
            genericTechObject.GenericGroup = this;
            this.baseObject = baseObject;
            genericTechObject.SetUpFromBaseTechObject();
        }

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
            => (new ITreeViewItem[] { genericTechObject }).Concat(inheritedTechObjects).ToArray();

        public override bool IsInsertable => true;

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
                inheritedTechObjects.Remove(techObject);
                techObjectManager.TechObjects.Remove(techObject);

                baseObject.SetRestrictionOwner();

                techObjectManager.ChangeAttachedObjectsAfterDelete(globalNum);

                ReindexingObjects();

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
            foreach (var techObject in inheritedTechObjects)
            {
                techNumber++;
                if (techObject.TechNumber == techNumber)
                    continue;

                techObject.TechNumber = techNumber;
                techObject.ModifyDevNames(0);
            }
        }



        /// <summary>
        /// Создание физ.объека на основе шаблона
        /// </summary>
        public override ITreeViewItem Insert()
        {
            var techObject = genericTechObject.CreateTechObject(baseObject);

            techObject.AddParent(this);
            techObject.TechNumber = inheritedTechObjects.Last().TechNumber + 1;
        
            inheritedTechObjects.Add(techObject);
            
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
            inheritedTechObjects.Add(techObject);
        }


        private ITechObjectManager techObjectManager;

        /// <summary>
        /// Список тех. объектов в группе
        /// </summary>
        public List<TechObject> InheritedTechObjects => inheritedTechObjects;

        /// <summary>
        /// Типовой объект группы
        /// </summary>
        public GenericTechObject GenericTechObject => genericTechObject;

        private List<TechObject> inheritedTechObjects = new List<TechObject>();
        private GenericTechObject genericTechObject;
        private BaseObject baseObject;
    }




    public class GenericTechObject : TechObject
    {
        public GenericTechObject(string name, int techType, string nameEplan,
            int cooperParamNumber, string NameBC, string attachedObjects,
            BaseTechObject baseTechObject)
            : base(name, null, 0, techType, nameEplan,
                cooperParamNumber, NameBC, attachedObjects, baseTechObject)
        {
            if (Editor.Editor.GetInstance().Editable)
                Update();
        }

        public GenericTechObject(TechObject techObject)
            : base(techObject.Name, null, 0, techObject.TechType, techObject.NameEplan,
                  techObject.CooperParamNumber, techObject.NameBC,
                  techObject.AttachedObjects.Value, techObject.BaseTechObject)
        {
            modes = techObject.ModesManager.Clone(this);
            paramsManager = techObject.GetParamsManager().Clone();

            SetItems();
            Update();
        }

        public void SetUpEvents()
        {
            ValueChanged += UpdateTechObjectsData;
            Equipment.ValueChanged += UpdateEquipment;
            paramsManager.ValueChanged += UpdateParamsManager;
            ModesManager.ValueChanged += UpdateModesManager;
            AttachedObjects.ValueChanged += UpdateAttachedObjects;
        }

        public void Update()
        {
            SetUpEvents();
            UpdateAttachedObjects(null);
            UpdateModesManager(null);
            UpdateEquipment(null);
            UpdateParamsManager(null);
            UpdateTechObjectsData(null);
        }

        private void UpdateAttachedObjects(object sender)
        {
            inheritedTechObjects.ForEach(to => to.AttachedObjects.UpdateOnGenericTechObject(AttachedObjects));
        }

        private void UpdateModesManager(object sender)
        {
            inheritedTechObjects.ForEach(to => to.ModesManager.UpdateOnGenericTechObject(ModesManager));
            inheritedTechObjects.ForEach(to => to.ModifyDevNames(0));
        }
        private void UpdateEquipment(object sender)
        {
            inheritedTechObjects.ForEach(to => to.Equipment.UpdateOnGenericTechObject(equipment));
        }

        private void UpdateParamsManager(object sender)
        {
            inheritedTechObjects.ForEach(to => to.GetParamsManager().UpdateOnGenericTechObject(this.paramsManager));
        }

        private void UpdateTechObjectsData(object sender)
        {
            inheritedTechObjects.ForEach(to => to.UpdateFromGenericTechObject(this));
        }

        public override string[] DisplayText
            => new string[] { $"{Name} - типовой объект", string.Empty };


        /// <summary>
        /// Создать экземпляр тех. объекта
        /// </summary>
        public TechObject CreateTechObject(BaseObject BaseObject)
        {
            var techObject = new TechObject(this.Name,
                BaseObject.GetTechObjectLocalNum, 1, this.TechType,
                this.NameEplan, this.CooperParamNumber, this.NameBC,
                this.AttachedObjects.Value, this.BaseTechObject);

            techObject.GenericTechObject = this;
            BaseObject.AddObject(techObject);
            inheritedTechObjects.Add(techObject);
            return techObject;
        }

        public TechObject SetUpTechObject(TechObject techObject, BaseObject baseObject)
        {
            techObject.GenericTechObject = this;
            inheritedTechObjects.Add(techObject);
            return techObject;
        }

        /// <summary>
        /// Группа типового объекта
        /// </summary>
        public GenericGroup GenericGroup { get; set; } = null;

        /// <summary>
        /// Объекты в группе, созданные на основе типового объекта
        /// </summary>
        private List<TechObject> inheritedTechObjects = new List<TechObject>();
    }
}