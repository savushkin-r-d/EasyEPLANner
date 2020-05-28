using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс, содержащий оборудование технологического объекта
    /// </summary>
    public class Equipment : Editor.TreeViewItem
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="owner">Владелец</param>
        public Equipment(TechObject owner)
        {
            this.owner = owner;
            items = new List<Editor.ITreeViewItem>();
        }

        /// <summary>
        /// Очистить список оборудования
        /// </summary>
        public void Clear()
        {
            items.Clear();
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
        }

        /// <summary>
        /// Добавить оборудование.
        /// </summary>
        /// <param name="property">Оборудование</param>
        private void AddItem(BaseParameter property)
        {
            items.Add(property);
        }

        /// <summary>
        /// Добавить оборудование
        /// </summary>
        /// <param name="name">Lua имя</param>
        /// <param name="value">Значение</param>
        public void AddEquipment(string name, string value)
        {
            foreach (Editor.ITreeViewItem item in items)
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

            foreach(Editor.ITreeViewItem item in items)
            {
                var property = item as BaseParameter;
                equipment.AddItem(property.Clone());
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
            foreach (Editor.ITreeViewItem item in items)
            {
                var property = item as BaseParameter;
                bool isEmpty = property.IsEmpty ||
                    property.Value == property.DefaultValue;
                if (!isEmpty)
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
                if (device.Description != "заглушка")
                {
                    string newDevName = newTechObjName + techNumber + 
                        device.DeviceType.ToString() + device.DeviceNumber;
                    var newDevice = Device.DeviceManager.GetInstance()
                        .GetDevice(newDevName);
                    if (newDevice.Description != "заглушка")
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
                if (device.Description != "заглушка")
                {
                    string newDevName = eplanName + techNumber +
                        device.DeviceType.ToString() + device.DeviceNumber;
                    var newDevice = Device.DeviceManager.GetInstance()
                        .GetDevice(newDevName);
                    if (newDevice.Description != "заглушка")
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
                string currentValue = equip.Value;
                if (currentValue == equip.DefaultValue)
                {
                    string deviceName = owner.NameEplan + owner.TechNumber + 
                        equip.DefaultValue;
                    var device = Device.DeviceManager.GetInstance()
                        .GetDevice(deviceName);
                    if (device.Description != "заглушка")
                    {
                        equip.SetNewValue(deviceName);
                    }
                }

                // Обработка ПИДа
                if (equip.LuaName == "SET_VALUE")
                {
                    bool isValid = false;
                    var device = Device.DeviceManager.GetInstance()
                        .GetDevice(currentValue);
                    if (device.Description != "заглушка")
                    {
                        isValid = true;
                    }

                    if (owner.Params.GetFParam(currentValue) != null)
                    {
                        isValid = true;
                    }

                    if (isValid == false)
                    {
                        errors += $"Отсутствует задание для ПИД регулятора" +
                            $" №{owner.GlobalNumber}\n";
                    }
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

        override public Editor.ITreeViewItem[] Items
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

        public override Editor.ITreeViewItem Replace(object child, 
            object copyObject)
        {
            var property = child as ActiveParameter;
            if (property != null && copyObject is ActiveParameter)
            {
                property.SetNewValue((copyObject as ActiveParameter).Value);
                ModifyDevNames(owner.NameEplan, owner.TechNumber);
                return property as Editor.ITreeViewItem;
            }
            return null;
        }

        public override object Copy()
        {
            return this;
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
            if (child is BaseParameter)
            {
                var property = child as BaseParameter;
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
        #endregion

        private TechObject owner;
        private List<Editor.ITreeViewItem> items;
    }
}
