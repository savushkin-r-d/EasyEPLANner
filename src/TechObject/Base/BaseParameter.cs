using System;
using System.Collections.Generic;
using System.Linq;
using EasyEPlanner;
using EplanDevice;

namespace TechObject
{
    /// <summary>    
    /// Свойство для базовой операции.
    /// </summary>
    public abstract class BaseParameter : Editor.ObjectProperty
    {
        /// <summary>
        /// Абстрактный метод копирования объекта.
        /// </summary>
        /// <returns></returns>
        public abstract new BaseParameter Clone();

        public BaseParameter(string luaName, string name,
            string defaultValue = "", List<DisplayObject> displayObjects = null,
            IDeviceManager deviceManager = null) : base(name, defaultValue,
                defaultValue)
        {
            this.luaName = luaName;
            currentValueType = ValueType.None;
            devicesIndexes = new List<int>();
            this.deviceManager = deviceManager != null ?
                deviceManager : DeviceManager.GetInstance();

            if(displayObjects != null)
            {
                displayObjectsFlags = new List<DisplayObject>();
                foreach(var displayObject in displayObjects)
                {
                    displayObjectsFlags.Add(displayObject);
                }
            }
            else
            {
                displayObjectsFlags = new List<DisplayObject>() 
                { DisplayObject.None };
            }

            SetUpDisplayObjects();
        }

        /// <summary>
        /// Настройка отображаемых объектов
        /// </summary>
        private void SetUpDisplayObjects()
        {
            deviceTypes = new DeviceType[0];
            displayParameters = false;
            foreach (var displayObject in DisplayObjects)
            {
                switch (displayObject)
                {
                    case DisplayObject.Parameters:
                        displayParameters = true;
                        break;

                    case DisplayObject.Signals:
                        deviceTypes = new DeviceType[]
                        {
                            DeviceType.AI,
                            DeviceType.AO,
                            DeviceType.DI,
                            DeviceType.DO
                        };
                        break;
                }
            }
        }

        /// <summary>
        /// Добавить вид отображаемых объектов. Вызывается из LUA.
        /// </summary>
        /// <param name="flagValue">Строковое значение перечисления</param>
        public void AddDisplayObject(string flagValue)
        {
            bool ignoreCase = true;
            var parsed = Enum.TryParse(flagValue, ignoreCase,
                out DisplayObject parsedEnum);
            if(parsed)
            {
                bool replaceNoneValue =
                    displayObjectsFlags.Contains(DisplayObject.None) &&
                    displayObjectsFlags.Count == 1 &&
                    parsedEnum != DisplayObject.None;
                bool setToNoneValue = parsedEnum == DisplayObject.None;
                if (replaceNoneValue || setToNoneValue)
                {
                    displayObjectsFlags.Clear();
                    displayObjectsFlags.Add(parsedEnum);
                }

                if (!displayObjectsFlags.Contains(parsedEnum))
                {
                    displayObjectsFlags.Add(parsedEnum);
                }

                SetUpDisplayObjects();
            }
        }

        public List<DisplayObject> DisplayObjects
        {
            get
            {
                return displayObjectsFlags;
            }
        }

        /// <summary>
        /// Lua имя свойства.
        /// </summary>
        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Пустой ли параметр (nil or '')
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Объект-владелец параметра.
        /// </summary>
        public object Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }

        #region синхронизация устройств
        public virtual void Synch(int[] array)
        {
            if (OnlyDevicesInParameter)
            {
                IDeviceSynchronizeService synchronizer = DeviceSynchronizer
                    .GetSynchronizeService();
                synchronizer.SynchronizeDevices(array, ref devicesIndexes);
                SetValue(GetDevicesString());
            }
        }
        #endregion

        #region реализация ITreeView
        public override bool SetNewValue(string newValue)
        {
            newValue = newValue.Trim();
            ProcessValue(newValue);

            if (OnlyDevicesInParameter)
            {
                base.SetNewValue(GetDevicesString());
            }
            else
            {
                base.SetNewValue(newValue);
            }

            return true;
        }

        public override void SetValue(object val)
        {
            ProcessValue(val);
            
            if (OnlyDevicesInParameter)
            {
                base.SetValue(GetDevicesString());
            }
            else
            {
                base.SetValue(val);
            }
        }

        /// <summary>
        /// Обработать входящее значение
        /// </summary>
        /// <param name="value">Входящее значение</param>
        private void ProcessValue(object value)
        {
            string valueStr = value.ToString();
            SetParameterValueType(valueStr);
            devicesIndexes.Clear();
            if (OnlyDevicesInParameter)
            {
                List<string> splittedDevices = valueStr.Split(' ').ToList();
                devicesIndexes.AddRange(GetDevicesIndexes(splittedDevices));
            }
        }

        private void SetParameterValueType(string valueStr)
        {
            currentValueType = GetParameterValueType(valueStr);
        }

        /// <summary>
        /// Получить индексы устройств
        /// </summary>
        /// <param name="values">Список значений</param>
        /// <returns></returns>
        protected List<int> GetDevicesIndexes(List<string> values)
        {
            var indexes = new List<int>();

            foreach (var value in values)
            {
                int index = deviceManager.GetDeviceIndex(value);
                if (index >= 0)
                {
                    indexes.Add(index);
                }
            }

            return indexes;
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { luaName, Value };
            }
        }

        override public string[] DisplayText
        {
            get
            {
                if (OnlyDevicesInParameter)
                {
                    return new string[] { Name, GetDevicesString() };
                }
                else
                {
                    return new string[] { Name, Value };
                }
            }
        }

        /// <summary>
        /// Получить строку с устройствами
        /// </summary>
        /// <returns></returns>
        protected string GetDevicesString()
        {
            var devices = new List<string>();
            foreach (var devIndex in devicesIndexes)
            {
                IDevice dev = deviceManager.GetDeviceByIndex(devIndex);
                if (dev.Name != StaticHelper.CommonConst.Cap)
                {
                    devices.Add(dev.Name);
                }
            }

            devices = devices.Distinct().ToList();
            return string.Join(" ", devices);
        }

        public override void GetDisplayObjects(out DeviceType[] devTypes, 
            out DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            devSubTypes = null; // Not used;
            devTypes = deviceTypes;
            displayParameters = this.displayParameters;
        }
        #endregion

        /// <summary>
        /// Получить тип параметра в зависимости от введенного значения в поле
        /// </summary>
        /// <param name="value">Значение свойства</param>
        /// <returns></returns>
        private ValueType GetParameterValueType(string value)
        {
            var result = ValueType.Other;

            if (string.IsNullOrEmpty(value))
            {
                return ValueType.None;
            }

            if (IsBoolParameter)
            {
                return ValueType.Boolean;
            }

            if (int.TryParse(value, out _))
            {
                return ValueType.Number;
            }

            bool isParameter = GetCurrentTechObject()?.GetParamsManager()
                .Float.GetParam(value) != null;
            if (isParameter)
            {
                return ValueType.Parameter;
            }

            bool isDevice = deviceManager.GetDeviceByEplanName(value)
                .Description != StaticHelper.CommonConst.Cap;
            if (isDevice)
            {
                return ValueType.Device;
            }

            string[] devices = value.Split(' ');
            if (devices.Length > 1)
            {
                bool haveBadDevices = false;
                var validDevices = new List<bool>();
                foreach (var device in devices)
                {
                    isDevice = deviceManager.GetDeviceByEplanName(device)
                        .Description != StaticHelper.CommonConst.Cap;
                    if (isDevice == false)
                    {
                        haveBadDevices = true;
                    }
                    validDevices.Add(isDevice);
                }

                validDevices = validDevices.Distinct().ToList();
                if (validDevices.Count == 1 && haveBadDevices == false)
                {
                    return ValueType.ManyDevices;
                }
            }

            bool stub = value.ToLower()
                .Contains(StaticHelper.CommonConst.StubForCells.ToLower());
            if (stub)
            {
                return ValueType.Stub;
            }

            return result;
        }

        #region сохранение prg.lua
        public string SaveToPrgLua(string prefix)
        {
            TechObject obj = GetCurrentTechObject();
            var objName = string.Empty;
            if (obj != null)
            {
                objName = "prg." + obj.NameEplanForFile.ToLower() +
                    obj.TechNumber.ToString();
            }

            switch (CurrentValueType)
            {
                case ValueType.Boolean:
                case ValueType.Other:
                    return $"{prefix}{LuaName} = {Value}";

                case ValueType.Device:
                    return $"{prefix}{LuaName}" +
                        $" = prg.control_modules.{Value}";

                case ValueType.Number:
                    return GetNumberParameterStringForSave(prefix);

                case ValueType.Parameter:
                    return $"{prefix}{LuaName} = " +
                        $"{objName}.PAR_FLOAT.{Value}";

                case ValueType.ManyDevices:
                    string paramCode = SaveMoreThanOneDevice(LuaName, Value);
                    return $"{prefix}{paramCode}";

                case ValueType.None:
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Получение строки для сохранения в зависимости от того, кто
        /// владеет параметром. Сам юнит или это параметр агрегата.
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string GetNumberParameterStringForSave(string prefix)
        {
            BaseTechObject baseTechObject = null;
            Mode mainMode = null;
            var modes = new List<Mode>();
            string mainObjDisplayName = "";

            if (Owner is BaseTechObject aggregateBaseTechObject)
            {
                baseTechObject = aggregateBaseTechObject;
                modes = baseTechObject.Owner.ModesManager.Modes;
                
                var baseOperation = Parent as BaseOperation;
                mainMode = baseOperation.Owner;
            }
            else if (Owner is BaseOperation baseOperation)
            {
                baseTechObject = baseOperation.Owner.Owner.Owner.BaseTechObject;
                mainMode = baseOperation.Owner;
                modes = mainMode.Owner.Modes;
            }

            mainObjDisplayName = $"{baseTechObject?.Owner.DisplayText[0]}";

            Mode modeInParameter = modes
                .Where(x => x.GetModeNumber().ToString() == Value)
                .FirstOrDefault();
            var res = string.Empty;
            if (modeInParameter != null)
            {
                if (modeInParameter.BaseOperation.Name != string.Empty)
                {
                    string operationLuaName = modeInParameter.BaseOperation
                        .LuaName.ToUpper();
                    TechObject obj = baseTechObject.Owner;
                    string objVarName =
                        $"prg.{obj.NameEplanForFile.ToLower()}" +
                        $"{obj.TechNumber}";
                    res = $"{prefix}{LuaName} = " +
                        $"{objVarName}.operations.{operationLuaName}";
                }
                else
                {
                    string message = $"Ошибка обработки параметра " +
                        $"\"{Name}\"." +
                        $" Не задана базовая операция в операции" +
                        $" \"{modeInParameter.DisplayText[0]}\", объекта " +
                        $"\"{mainObjDisplayName}\".\n";
                    Logs.AddMessage(message);
                }
            }
            else
            {
                string message = $"Ошибка обработки параметра " +
                        $"\"{Name}\"." +
                        $" Указан несуществующий номер операции в операции " +
                        $"\"{mainMode?.DisplayText[0]}\" объекта " +
                        $"\"{mainObjDisplayName}\".\n";
                Logs.AddMessage(message);
            }

            return res;
        }

        /// <summary>
        /// Сохранить более 1 устройства в параметре.
        /// </summary>
        /// <param name="value">Значение параметра</param>
        /// <param name="luaName">lua-имя параметра</param>
        /// <returns></returns>
        private string SaveMoreThanOneDevice(string luaName, string value)
        {
            string res = "";

            string[] devices = value.Split(' ');
            if (devices.Length > 1)
            {
                string[] modifiedDevices = devices
                    .Select(x => "prg.control_modules." + x).ToArray();
                res = $"{luaName} = " +
                    $"{{ {string.Join(", ", modifiedDevices)} }}";
            }
            else
            {
                res = $"{luaName} = prg.control_modules.{value}";
            }

            return res;
        }
        #endregion

        /// <summary>
        /// Получить технологический объект, в котором находится параметр.
        /// </summary>
        /// <returns></returns>
        private TechObject GetCurrentTechObject()
        {
            if (Owner is BaseTechObject)
            {
                var operation = Parent as BaseOperation;
                return operation.Owner.Owner.Owner;
            }
            else if (Owner is BaseOperation operation)
            {
                return operation.Owner.Owner.Owner;
            }
            else if (Owner is Equipment equipment)
            {
                return equipment.Owner;
            }

            return null;
        }

        public virtual void Check()
        {
            if (!IsEmpty && !Disabled)
            {
                SetParameterValueType(Value);
            }
        }

        /// <summary>
        /// Текущий тип значений, принимаемый параметром
        /// </summary>
        public ValueType CurrentValueType => currentValueType;

        /// <summary>
        /// Флаг, указывающий что в параметр записаны только устройства
        /// </summary>
        private bool OnlyDevicesInParameter =>
            CurrentValueType == ValueType.Device ||
            CurrentValueType == ValueType.ManyDevices;

        public enum DisplayObject
        {
            None = 1,
            Signals,
            Parameters,
        }

        /// <summary>
        /// Возможные типы значений полей параметров объекта
        /// </summary>
        public enum ValueType
        {
            None,
            Other,
            ManyDevices,
            Device,
            Boolean,
            Parameter,
            Number,
            Stub,
        }

        protected IDeviceManager deviceManager;

        private object owner;
        private string luaName;
        private List<DisplayObject> displayObjectsFlags;
        private ValueType currentValueType;
        protected List<int> devicesIndexes;

        private DeviceType[] deviceTypes;
        private bool displayParameters;
    }
}
