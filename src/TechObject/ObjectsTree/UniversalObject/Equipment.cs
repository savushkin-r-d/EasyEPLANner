using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Класс, содержащий оборудование технологического объекта
    /// </summary>
    public class Equipment : TreeViewItem
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
                    property.SetValue(value);
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
                var property = item as BaseParameter;
                if (!property.IsEmpty)
                {
                    equipmentForSave += prefix + $"\t{property.LuaName} = " +
                        $"\'{property.Value}\',\n";
                }
            }

            bool needSaveQuipment = equipmentForSave != "";
            if (needSaveQuipment)
            {
                res += prefix + "equipment = \n" +
                    prefix + "\t{\n";
                res += equipmentForSave;
                res += prefix + "\t},\n";
            }

            return res;
        }

        public void ModifyDevNames(string newTechObjName, int techNumber)
        {
            var properties = items.Select(x => x as BaseParameter).ToArray();
            foreach (var property in properties)
            {
                string oldDevName = property.Value;
                var device = Device.DeviceManager.GetInstance()
                    .GetDevice(oldDevName);
                if (device.Description != StaticHelper.CommonConst.Cap)
                {
                    string newDevName = newTechObjName + techNumber + 
                        device.DeviceType.ToString() + device.DeviceNumber;
                    var newDevice = Device.DeviceManager.GetInstance()
                        .GetDevice(newDevName);
                    if (newDevice.Description != StaticHelper.CommonConst.Cap)
                    {
                        property.SetNewValue(newDevName);
                    }
                }
            }
        }

        public void ModifyDevNames()
        {
            int techNumber = owner.TechNumber;
            string eplanName = owner.NameEplan;

            var properties = items.Select(x => x as BaseParameter).ToArray();
            foreach (var property in properties)
            {
                string oldDevName = property.Value;
                var device = Device.DeviceManager.GetInstance()
                    .GetDevice(oldDevName);
                if (device.Description != StaticHelper.CommonConst.Cap)
                {
                    string newDevName = eplanName + techNumber +
                        device.DeviceType.ToString() + device.DeviceNumber;
                    var newDevice = Device.DeviceManager.GetInstance()
                        .GetDevice(newDevName);
                    if (newDevice.Description != StaticHelper.CommonConst.Cap)
                    {
                        property.SetNewValue(newDevName);
                    }
                }
            }
        }

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
                var device = Device.DeviceManager.GetInstance()
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
            string currentValue = equipment.Value;
            var device = Device.DeviceManager.GetInstance()
                    .GetDeviceByEplanName(currentValue);
            if (equipment.LuaName == "SET_VALUE")
            {
               
                bool isValid = 
                    (device.Description != StaticHelper.CommonConst.Cap ||
                    owner.GetParams().GetParam(currentValue) != null);
                if (!isValid)
                {
                    int objGlobalNumber = TechObjectManager.GetInstance()
                        .GetTechObjectN(owner);
                    errors += $"Отсутствует задание для ПИД регулятора" +
                        $" №{objGlobalNumber}\n";
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
                    string techObjectName = owner.DisplayText[0];
                    errors += $"Проверьте оборудование: \"{equipment.Name}\" " +
                        $"в объекте \"{techObjectName}\". " +
                        $"Не найдено устройство или " +
                        $"задано более 1 устройства.\n";
                }
            }
            return errors;
        }

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
            var property = child as ActiveParameter;
            var copiedObject = copyObject as ActiveParameter;
            bool objectsNotNull = property != null && copiedObject != null;
            if (objectsNotNull)
            {
                property.SetNewValue(copiedObject.Value);
                ModifyDevNames(owner.NameEplan, owner.TechNumber);
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
            var property = child as BaseParameter;
            if (property != null)
            {
                property.SetNewValue("");
                return true;
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

        public override bool IsFilled
        {
            get
            {
                if(items.Where(x => x.IsFilled).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=control_module";
        }

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

        private TechObject owner;
        private List<ITreeViewItem> items;
    }
}
