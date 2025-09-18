using System;
using System.Collections.Generic;
using System.Linq;
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
            items = new List<ITreeViewItem>();
        }

        /// <summary>
        /// Добавить оборудование.
        /// </summary>
        /// <param name="properties">Список оборудования</param>
        public void AddItems(List<BaseParameter> properties)
        {
            foreach(BaseParameter property in properties)
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
            foreach (ITreeViewItem item in items)
            {
                var property = item as BaseParameter;
                if (property.LuaName == name)
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

            foreach(ITreeViewItem item in items)
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

            string equipmentForSave = "";
            foreach (ITreeViewItem item in items)
            {
                var property = item as EquipmentParameter;
                if (!property.IsEmpty)
                {
                    equipmentForSave += prefix + $"\t{property.LuaName} = " +
                        $"\'{property.Value}\',\n";
                }
            }

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
            items.OfType<BaseParameter>().ToList()
                .ForEach(property =>
                {
                    var newValues = property.Value.Split(' ')
                        .Select(deviceManager.GetDeviceByEplanName)
                        .Where(dev => dev.ObjectName == techObjName)
                        .Select(dev => $"{techObjName}{techNumber}{dev.DeviceDesignation}")
                        .Where(name => deviceManager.GetDeviceByEplanName(name).Description != CommonConst.Cap);

                    if (newValues.Any())
                    {
                        property.SetNewValue(string.Join(" ", newValues));
                    }
                });
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
            var errors = "";

            var equipment = Items.Select(x => x as BaseParameter).ToArray();
            foreach (var equip in equipment)
            {
                SetDeviceAutomatically(equip);
                errors += CheckEquipmentValues(equip);
            }

            return errors;
        }

        /// <summary>
        /// Установка устройств в оборудовании автоматически
        /// </summary>
        /// <param name="equipment">Оборудование</param>
        private void SetDeviceAutomatically(BaseParameter equipment)
        {
            string currentValue = equipment.Value;
            if (equipment.DefaultValue != "" && 
                currentValue == equipment.DefaultValue)
            {
                string deviceName = owner.NameEplan + owner.TechNumber +
                    equipment.DefaultValue;
                var device = EplanDevice.DeviceManager.GetInstance()
                    .GetDevice(deviceName);
                if (device.Description != StaticHelper.CommonConst.Cap)
                {
                    equipment.SetNewValue(deviceName);
                }
                else
                {
                    equipment.SetNewValue("");
                }
            }
        }

        /// <summary>
        /// Проверить параметры и устройства для ПИД
        /// </summary>
        /// <param name="equipment">Оборудование</param>
        /// <returns></returns>
        private string CheckEquipmentValues(BaseParameter equipment)
        {
            var errors = "";
            string techObjectName = owner.DisplayText[0];
            string currentValue = equipment.Value;
            string[] devices = currentValue.Split(' ');
            if (devices.Length > 1)
            {
                errors += CheckMultiValue(devices, equipment, techObjectName);
            }
            else
            {
                errors += CheckSingleValue(currentValue, equipment,
                    techObjectName);
            }

            return errors;
        }

        /// <summary>
        /// Проверка множественных значений в оборудовании
        /// </summary>
        /// <param name="devices">Устройства</param>
        /// <param name="equipment">Оборудование</param>
        /// <param name="techObjectName">Имя объекта</param>
        /// <returns></returns>
        private string CheckMultiValue(string[] devices,
            BaseParameter equipment, string techObjectName)
        {
            string errors = "";
            var unknownDevices = new List<string>();

            foreach (var deviceStr in devices)
            {
                var device = EplanDevice.DeviceManager.GetInstance()
                    .GetDeviceByEplanName(deviceStr);
                if (device.Description == StaticHelper.CommonConst.Cap)
                {
                    unknownDevices.Add(deviceStr);
                }
            }

            if (unknownDevices.Count > 0)
            {
                errors = $"Проверьте оборудование: " +
                    $"\"{equipment.Name}\" в объекте " +
                    $"\"{techObjectName}\". " +
                    $"Некорректные устройства: " +
                    $"{string.Join(",", unknownDevices)}.\n";
            }

            return errors;
        }

        /// <summary>
        /// Проверка одиночных значений в оборудовании
        /// </summary>
        /// <param name="currentValue">Текущее значение</param>
        /// <param name="equipment">Оборудование</param>
        /// <param name="techObjectName">Имя объекта</param>
        /// <returns></returns>
        private string CheckSingleValue(string currentValue,
            BaseParameter equipment, string techObjectName)
        {
            string errors = "";

            var device = EplanDevice.DeviceManager.GetInstance()
                    .GetDeviceByEplanName(currentValue);
            if (equipment.LuaName == "SET_VALUE")
            {
                bool isValid =
                    (device.Description != StaticHelper.CommonConst.Cap ||
                    owner.GetParamsManager().Float
                    .GetParam(currentValue) != null);
                if (!isValid)
                {
                    errors += $"Отсутствует задание для ПИД регулятора" +
                        $" \"{owner.DisplayText[0]}\".\n";
                }
            }
            else
            {
                bool isValid =
                    device.Description != StaticHelper.CommonConst.Cap ||
                    currentValue == "" ||
                    currentValue == equipment.DefaultValue;
                if (!isValid)
                {
                    errors += $"Проверьте оборудование: " +
                        $"\"{equipment.Name}\" в объекте " +
                        $"\"{techObjectName}\". " +
                        $"Некорректное устройство: {currentValue}.\n";
                }
            }

            return errors;
        }
        #endregion

        #region Реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                if (items.Count() > 0)
                {
                    string res = string.Format("Оборудование ({0})",
                        items.Count());
                    return new string[] { res, "" };
                }
                else
                {
                    string res = string.Format("Оборудование");
                    return new string[] { res, "" };
                }
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Replace(object child, 
            object copyObject)
        {
            var property = child as EquipmentParameter;
            var copiedObject = copyObject as EquipmentParameter;
            bool objectsNotNull = property != null && copiedObject != null;
            if (objectsNotNull)
            {
                property.SetNewValue(copiedObject.Value);
                ModifyDevNames();

                property.AddParent(this);
                return property;
            }
            return null;
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        public override bool Delete(object child)
        {
            var treeItem = child as ITreeViewItem;
            if(treeItem != null)
            {
                if(treeItem.IsMainObject)
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

            return false;
        }

        public override bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.Equipment;
            }
        }
        #endregion

        public TechObject Owner
        {
            get
            {
                return owner;
            }
        }

        public override string SystemIdentifier => "control_module";

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }

        #region Синхронизация устройств в объекте.
        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        public void Synch(int[] array)
        {
            foreach(var treeViewItem in items)
            {
                var equipmentParameter = treeViewItem as EquipmentParameter;
                bool validParameter = equipmentParameter != null;
                if(validParameter)
                {
                    equipmentParameter.Synch(array);
                }
            }
        }
        #endregion

        /// <summary>
        /// Сортировка оборудования в списке по-алфавиту
        /// </summary>
        private void Sort()
        {
            items.Sort(delegate (ITreeViewItem x, ITreeViewItem y)
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
                var equipmentItem = genericEquipment.items[index] as BaseParameter;
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
                var refEquipmentItem = refEquipment.items[index] as BaseParameter;
                if (equipmentList.TrueForAll(equipmentItem => (equipmentItem.items[index] as BaseParameter).Value == refEquipmentItem.Value))
                    items[index].SetNewValue(refEquipmentItem.Value);
                else
                    items[index].SetNewValue("");
            }
        }

        public void Clear() => items.Clear();

        public void Autocomplete()
        {
            var techObj = owner.NameEplan;
            var techN = owner.TechNumber;

            foreach(var item in Items.OfType<BaseParameter>())
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

        private TechObject owner;
        private List<ITreeViewItem> items;

        private static IDeviceManager deviceManager { get; set; } = DeviceManager.GetInstance();

        bool IAutocompletable.CanExecute => true;
    }
}
