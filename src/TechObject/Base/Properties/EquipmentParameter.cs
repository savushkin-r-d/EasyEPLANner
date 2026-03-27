using EasyEPlanner;
using EasyEPlanner.PxcIolinkConfiguration.Models;
using Editor;
using EplanDevice;
using StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TechObject
{
    /// <summary>
    /// Параметр оборудования объекта
    /// </summary>
    public class EquipmentParameter : BaseParameter
    {
        public EquipmentParameter(string luaName, string name,
            string defaultValue = "", List<DisplayObject> displayObjects = null)
            : base(luaName, name, defaultValue, displayObjects) 
        { 
            parameterIndexes = new List<int>();
        }

        public override BaseParameter Clone()
        {
            var newProperty = new EquipmentParameter(LuaName, Name,
                DefaultValue, DisplayObjects);
            newProperty.SetNewValue(Value);
            newProperty.NeedDisable = NeedDisable;
            newProperty.relatedParameters
                .AddRange(relatedParameters.Select(rp => rp.Clone()).OfType<EquipmentParameter>());
            
            return newProperty;
        }

        #region Синхронизация устройств
        /// <summary>
        /// Синхронизация устройств в объекте
        /// </summary>
        /// <param name="array">Массив с индексами синхронизации</param>
        public override void Synch(int[] array)
        {
            // parameterIndexes - не синхронизируем т.к это не устройства.
            IDeviceSynchronizeService synchronizer = DeviceSynchronizer
                .GetSynchronizeService();
            synchronizer.SynchronizeDevices(array, ref devicesIndexes);

            SetValue(GetDevicesAndParametersString());
        }
        #endregion

        public override bool IsEmpty => Value == "" || Value == "" && DefaultValue == "";

        #region Реализация ITreeViewItem
        public override string[] DisplayText => [Name, Value == "" ? StaticHelper.CommonConst.StubForCells : Value];

        public override string[] EditText => [Name, Value];

        public override bool SetNewValue(string newValue)
        {
            devicesIndexes.Clear();
            parameterIndexes.Clear();

            bool emptyOrDefault = newValue == string.Empty ||
                newValue == DefaultValue;
            if (emptyOrDefault)
            {
                SetValue(newValue.Trim());
                return true;
            }

            List<string> values = newValue.Split(' ').ToList();

            List<int> devices = GetDevicesIndexes(values);
            List<int> parameters = GetParametersIndexes(values);

            devicesIndexes.AddRange(devices);
            parameterIndexes.AddRange(parameters);

            devicesIndexes.Sort();
            parameterIndexes.Sort();

            SetValue(GetDevicesAndParametersString());

            return true;
        }

        /// <summary>
        /// Получить индексы параметров
        /// </summary>
        /// <param name="values">Список значений</param>
        /// <returns></returns>
        private List<int> GetParametersIndexes(List<string> values)
        {
            var indexes = new List<int>();
            var equipment = Owner as Equipment;
            if(equipment != null)
            {
                TechObject techObject = equipment.Owner;
                if(techObject != null)
                {
                    foreach(var value in values)
                    {
                        Param param = techObject.GetParamsManager()
                            .Float.GetParam(value);
                        if(param == null)
                        {
                            continue;
                        }

                        indexes.Add(param.GetParameterNumber - 1);
                    }
                }
            }

            return indexes;
        }

        /// <summary>
        /// Получить представление значений в виде строки
        /// </summary>
        /// <returns></returns>
        private string GetDevicesAndParametersString()
        {
            string devices = GetDevicesString();
            string parameters = GetParametersString();

            string result = $"{devices} {parameters}";
            return result.Trim();
        }

        /// <summary>
        /// Получить строку с параметрами
        /// </summary>
        /// <returns></returns>
        private string GetParametersString()
        {
            var parameters = new List<string>();
            if (Owner is Equipment equipment)
            {
                var techObject = equipment.Owner;
                if (techObject != null)
                {
                    foreach (var paramIndex in parameterIndexes)
                    {
                        Param param = techObject.GetParamsManager()
                            .Float.GetParam(paramIndex);
                        parameters.Add(param.GetNameLua());
                    }
                }
            }

            return string.Join(" ", parameters);
        }

        public override object Owner
        {
            get => base.Owner;
            set
            {
                base.Owner = value;
                relatedParameters.ForEach(p => p.Owner = value);
            }
        }

        public Equipment Equipment => Owner as Equipment;

        public override bool IsReplaceable => true;

        override public bool IsCopyable => true;

        public override bool IsDeletable => true;

        public override bool IsInsertableCopy => true;

        public override ITreeViewItem[] Items => [.. relatedParameters];

        public override bool Delete(object child)
        {
            if (child is not EquipmentParameter equip)
                return false;

            equip.SetNewValue("");
            return true;
        }

        public override ITreeViewItem Replace(object child, object copyObject)
        {
            if (child is EquipmentParameter equip)
                return equip.InsertCopy(copyObject);

            return null;
        }

        public override ITreeViewItem InsertCopy(object obj)
        {
            if (obj is EquipmentParameter equip)
            {
                SetNewValue(equip.Value);
                ModifyDevNames();
                return this;
            }

            return null;
        }

        public override void GetDisplayObjects(out DeviceType[] devTypes,
            out DeviceSubType[] devSubTypes,
            out bool displayParameters)
        {
            devTypes = null;
            devSubTypes = null;
            displayParameters = LuaName.EndsWith("SET_VALUE");
        }
        #endregion

        /// <summary>
        /// Модифицировать значения параметров в соответсвиии с объектом
        /// </summary>
        /// <param name="techObjName">ОУ</param>
        /// <param name="techNumber">Номер тех.объекта</param>
        public void ModifyDevNames(string techObjName, int techNumber)
        {
            var newValues = Value.Split(' ')
                .Select(deviceManager.GetDeviceByEplanName)
                .Where(dev => dev.ObjectName == techObjName)
                .Select(dev => $"{techObjName}{techNumber}{dev.DeviceDesignation}")
                .Where(name => deviceManager.GetDeviceByEplanName(name).Description != CommonConst.Cap);

            if (newValues.Any())
            {
                SetNewValue(string.Join(" ", newValues));
            }
        }

        /// <summary>
        /// Модифицировать значения параметров в соответсвиии с объектом
        /// </summary>
        public void ModifyDevNames()
        {
            if (Owner is Equipment equipment)
            {
                var number = equipment.Owner.TechNumber;
                string eplanName = equipment.Owner.NameEplan;

                ModifyDevNames(eplanName, number);
            }
        }

        /// <summary>
        /// Получить все параметры, включая вложенные
        /// </summary>
        public override List<BaseParameter> GetDescendants()
        {
            var r = new List<BaseParameter>
            {
                this
            };

            r.AddRange(relatedParameters.SelectMany(p => p.GetDescendants()));
            return r;
        }

        /// <summary>
        /// Добавить параметр оборудования
        /// </summary>
        /// <param name="luaName">Lua-название параметра</param>
        /// <param name="name">Обозначение параметра</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Добавленные параметр</returns>
        public BaseParameter AddEquipment(string luaName, string name, string defaultValue)
        {
            var related = new EquipmentParameter(luaName, name, defaultValue)
            {
                Owner = Owner,
            };

            relatedParameters.Add(related);
            return related;
        }


        /// <summary>
        /// Проверка значений параметра
        /// </summary>
        /// <param name="nameEplan">ОУ</param>
        /// <param name="techNumber">Номер объекита</param>
        /// <param name="techObjectName">Отображаемое название объекта для ошибки</param>
        public StringBuilder Check(string nameEplan, int techNumber, string techObjectName)
        {
            if (DefaultValue != "" && Value == DefaultValue)
            {
                string deviceName = $"{nameEplan}{techNumber}{DefaultValue}";
                var device = deviceManager.GetDevice(deviceName);
                if (device.Description != CommonConst.Cap)
                {
                    SetNewValue(deviceName);
                }
                else
                {
                    SetNewValue("");
                }
            }
            var err = new StringBuilder();
            var wrongDevices = Value.Split(' ').Where(v => !CheckValue(v));

            if (wrongDevices.Any())
            {
                err.Append($"Проверьте оборудование: \"{Name}\" в объекте \"{techObjectName}\". Некорректные значения: {string.Join(", ", wrongDevices)}.\n");
            }

            if (!IsEmpty)
            {
                foreach (var parameter in relatedParameters)
                {
                    if (parameter.IsEmpty)
                    {
                        err.Append($"Проверьте оборудование: \"{Name}\" в объекте \"{techObjectName}\". Поле не заполнено.\n");
                    }

                    err.Append(parameter.Check(nameEplan, techNumber, techObjectName));
                }
            }

            return err;
        }


        /// <summary>
        /// Проверить строковое значение параметра
        /// </summary>
        /// <param name="value">Значание</param>
        public bool CheckValue(string value)
        {
            if (value == "" || value == DefaultValue)
                return true;

            var device = DeviceManager.GetInstance().GetDeviceByEplanName(value);
            if (device.Description != CommonConst.Cap)
                return true;

            // EquipmentParameter может использовать параметр объекта
            if (LuaName.EndsWith("SET_VALUE"))
            {
                return Equipment?.Owner.GetParamsManager().Float.GetParam(value) != null;
            }

            return false;
        }

        /// <summary>
        /// Индексы параметров
        /// </summary>
        private List<int> parameterIndexes;

        private readonly List<EquipmentParameter> relatedParameters = [];
    }
}
