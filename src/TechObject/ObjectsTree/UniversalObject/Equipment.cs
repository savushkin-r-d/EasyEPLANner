using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor;
using Eplan.EplApi.Base;
using EplanDevice;
using StaticHelper;

namespace TechObject
{
    /// <summary>
    /// Класс, содержащий оборудование технологического объекта
    /// </summary>
    public class Equipment : TreeViewItem, IAutocompletable
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="owner">Владелец</param>
        public Equipment(TechObject owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Добавить оборудование.
        /// </summary>
        /// <param name="properties">Список оборудования</param>
        public void AddItems(List<BaseParameter> properties)
        {
            foreach (BaseParameter property in properties)
            {
                property.Owner = this;
                property.ValueChanged += sender => OnValueChanged(sender);
                items.Add(property);
            }
            Sort();
        }

        /// <summary>
        /// Добавить оборудование.
        /// </summary>
        /// <param name="property">Оборудование</param>
        private void AddItem(BaseParameter property)
        {
            property.ValueChanged += sender => OnValueChanged(sender);
            items.Add(property);
            Sort();
        }

        /// <summary>
        /// Добавить оборудование
        /// </summary>
        /// <param name="name">Lua имя</param>
        /// <param name="value">Значение</param>
        public void SetEquipmentValue(string name, string value)
        {
            foreach (ITreeViewItem item in GetDescendantsParameters())
            {
                var property = item as BaseParameter;
                if (property.LuaName.ToUpper() == name.ToUpper())
                {
                    property.SetNewValue(value);
                }
            }
        }

        /// <summary>
        /// Копировать оборудование
        /// </summary>
        /// <param name="clone">Новый владелец</param>
        /// <returns></returns>
        public Equipment Clone(TechObject clone)
        {
            var equipment = new Equipment(clone);

            foreach (ITreeViewItem item in items)
            {
                var property = item as BaseParameter;
                var newProperty = property.Clone();
                newProperty.Owner = this;
                equipment.AddItem(newProperty);
            }

            equipment.ValueChanged += sender => OnValueChanged(sender);

            return equipment;
        }

        /// <summary>
        /// Сохранение в виде Lua таблицы
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveAsLuaTable(string prefix)
        {
            var res = "";
            if (items.Count == 0)
            {
                return res;
            }

            string equipmentForSave = GetDescendantsParameters()
                .OfType<EquipmentParameter>()
                .Where(p => !p.IsEmpty)
                .Select(p => $"{prefix}\t{p.LuaName} = \'{p.Value}\',\n")
                .Aggregate("", (r, p) => r + p);

            bool needSaveQuipment = equipmentForSave != "";
            if (needSaveQuipment)
            {
                res += prefix + "equipment =\n" +
                    prefix + "\t{\n";
                res += equipmentForSave;
                res += prefix + "\t},\n";
            }

            return res;
        }


        /// <summary>
        /// Модификация названий устройств в соответствие тех.объекту
        /// </summary>
        /// <remarks>
        /// Так как в поле "Оборудование" привязывается устройства только данного объекта, <br/>
        /// то нужно устройств заменить только объект на новый.
        /// </remarks>
        /// <param name="techObjName">Название тех.объекта</param>
        /// <param name="techNumber">Тех.номер объекта</param>
        public void ModifyDevNames(string techObjName, int techNumber)
        {
            foreach (var property in GetDescendantsParameters().OfType<EquipmentParameter>())
            {
                property.ModifyDevNames(techObjName, techNumber);
            }
        }

        public void ModifyDevNames()
        {
            int techNumber = owner.TechNumber;
            string eplanName = owner.NameEplan;

            ModifyDevNames(eplanName, techNumber);
        }

        #region Проверка и автоматическое заполнение оборудования
        public string Check()
        {
            var errors = new StringBuilder();

            foreach (var equip in items.OfType<EquipmentParameter>())
            {
                errors.Append(equip.Check(owner.NameEplan, owner.TechNumber, owner.DisplayText[0]));
            }

            return errors.ToString();
        }
        #endregion

        #region Реализация ITreeViewItem
        public override string[] DisplayText => [Name, ""];

        public string Name => items.Count > 0 ? $"Оборудование ({items.Count})" : "Оборудование";


        public override ITreeViewItem[] Items => [.. items];

        public override bool IsReplaceable => true;

        public override ITreeViewItem Replace(object child, object copyObject)
        {
            if (child is EquipmentParameter equip)
                return equip.InsertCopy(copyObject);
            
            return null;
        }

        public override bool IsCopyable => true;

        public override bool Delete(object child)
        {
            if (child is not ITreeViewItem treeItem)
                return false;

            if (treeItem.IsMainObject)
            {
                var objEquips = Items.Select(x => x as BaseParameter)
                    .ToArray();
                foreach (var equip in objEquips)
                {
                    equip.SetNewValue("");
                }
                return true;
            }
            else
            {
                treeItem.SetNewValue("");
                return true;
            }
        }

        public override bool IsDeletable => true;

        public override ImageIndexEnum ImageIndex => ImageIndexEnum.Equipment;
        #endregion

        public TechObject Owner => owner;

        public override string SystemIdentifier => "control_module";

        public override bool ShowWarningBeforeDelete => true;

        #region Синхронизация устройств в объекте.
        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        public void Synch(int[] array)
        {
            foreach(var equipmentParameter in GetDescendantsParameters()
                .OfType<EquipmentParameter>())
            {
                equipmentParameter.Synch(array);
            }
        }
        #endregion

        /// <summary>
        /// Сортировка оборудования в списке по-алфавиту
        /// </summary>
        private void Sort()
        {
            items.Sort(delegate (BaseParameter x, BaseParameter y)
            {
                return x.DisplayText[0].CompareTo(y.DisplayText[0]);
            });
        }

        /// <summary>
        /// Обновление оборудования на основе типового объекта
        /// </summary>
        /// <remarks>
        /// Если поле оборудования в типовом объекте не заполнено,
        /// то оно не влияет на значение данного поля оборудования,
        /// иначе изменяет значение на заданное в типовом объекте.
        /// Список оборудования фиксированный и зависит от базового объекта.
        /// </remarks>
        /// <param name="genericEquipment"> Оборудование типового объекта </param>
        public void UpdateOnGenericTechObject(Equipment genericEquipment)
        {
            foreach (var index in Enumerable.Range(0, items.Count))
            {
                var equipmentItem = genericEquipment.items[index];
                if (equipmentItem.IsFilled is false)
                    continue;

                items[index].SetNewValue(equipmentItem.Value);
            }
        }


        public override void CreateGenericByTechObjects(IEnumerable<ITreeViewItem> itemList)
        {
           var equipmentList = itemList.Cast<Equipment>().ToList();

            var refEquipment = equipmentList.FirstOrDefault();

            equipmentList.Skip(1).ToList()
                .ForEach(item => item.ModifyDevNames(refEquipment.owner.NameEplan, refEquipment.owner.TechNumber));

            foreach (var index in Enumerable.Range(0, refEquipment.items.Count))
            {
                var refEquipmentItem = refEquipment.items[index];
                if (equipmentList.TrueForAll(equipmentItem => (equipmentItem.items[index]).Value == refEquipmentItem.Value))
                    items[index].SetNewValue(refEquipmentItem.Value);
                else
                    items[index].SetNewValue("");
            }
        }

        public void Clear() => items.Clear();

        bool IAutocompletable.CanExecute => true;

        public void Autocomplete()
        {
            var techObj = owner.NameEplan;
            var techN = owner.TechNumber;

            foreach (var item in Items.OfType<BaseParameter>())
            {
                if (item.Value == item.DefaultValue && item.Value != string.Empty ||
                    item.Value == string.Empty && item.DefaultValue != string.Empty)
                {
                    var dev = $"{techObj}{techN}{item.DefaultValue}";
                    if (deviceManager.GetDevice(dev).Description is not CommonConst.Cap)
                        item.SetNewValue($"{techObj}{techN}{item.DefaultValue}");
                }
            }
        }

        public List<BaseParameter> GetDescendantsParameters()
        {
            return [.. items.SelectMany(i => i.GetDescendants())];
        }

        private readonly TechObject owner;
        private readonly List<BaseParameter> items = [];

        private static IDeviceManager deviceManager { get; set; } = DeviceManager.GetInstance();
    }
}
