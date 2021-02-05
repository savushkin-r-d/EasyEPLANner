using System;
using System.Collections.Generic;
using System.Linq;
using EasyEPlanner;

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
            string defaultValue = "", List<DisplayObject> displayObjects = null)
            : base(name, defaultValue, defaultValue)
        {
            this.luaName = luaName;
            currentValueType = ValueType.None;

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
            deviceTypes = new Device.DeviceType[0];
            displayParameters = false;
            foreach (var displayObject in DisplayObjects)
            {
                switch (displayObject)
                {
                    case DisplayObject.Parameters:
                        displayParameters = true;
                        break;

                    case DisplayObject.Signals:
                        deviceTypes = new Device.DeviceType[]
                        {
                            Device.DeviceType.AI,
                            Device.DeviceType.AO,
                            Device.DeviceType.DI,
                            Device.DeviceType.DO
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

        #region реализация ITreeView
        public override bool SetNewValue(string newValue)
        {
            newValue = newValue.Trim();
            base.SetNewValue(newValue);
            currentValueType = GetParameterValueType(newValue);
            return true;
        }

        public override void SetValue(object val)
        {
            currentValueType = GetParameterValueType(val.ToString());
            base.SetValue(val);
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
                return new string[] { Name, Value };
            }
        }

        public override void GetDisplayObjects(out Device.DeviceType[] devTypes, 
            out Device.DeviceSubType[] devSubTypes, out bool displayParameters)
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

            bool isParameter = GetTechObject()?.GetParamsManager()
                .GetParam(value) != null;
            if (isParameter)
            {
                return ValueType.Parameter;
            }

            var deviceManager = Device.DeviceManager.GetInstance();
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

        public string SaveAsLuaTable(string prefix)
        {
            TechObject obj = GetTechObject();
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
                    return SaveMoreThanOneDevice(LuaName, Value);

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
        public string GetNumberParameterStringForSave(string prefix)
        {
            BaseTechObject baseTechObject = null;
            Mode baseMode = null;
            var modes = new List<Mode>();
            string mainObjName = "";

            if (Owner is BaseTechObject)
            {
                baseTechObject = Owner as BaseTechObject;
                mainObjName = $"{baseTechObject.Owner.DisplayText[0]}";
                modes = baseTechObject.Owner.ModesManager.Modes;
                var operation = Parent as BaseOperation;
                baseMode = operation.Owner;
            }

            if (Owner is BaseOperation)
            {
                var operation = Owner as BaseOperation;
                baseTechObject = operation.Owner.Owner.Owner.BaseTechObject;
                mainObjName = $"{baseTechObject.Owner.DisplayText[0]}";
                baseMode = operation.Owner;
                modes = baseMode.Owner.Modes;
            }

            Mode mode = modes
                .Where(x => x.GetModeNumber().ToString() == Value)
                .FirstOrDefault();
            var res = "";
            if (mode != null)
            {
                if (mode.BaseOperation.Name != "")
                {
                    string operationLuaName = mode.BaseOperation.LuaName
                        .ToUpper();
                    TechObject obj = baseTechObject.Owner;
                    string objName = "prg." + obj.NameEplanForFile.ToLower() +
                        obj.TechNumber.ToString();
                    res = $"{prefix}{LuaName} = " +
                        $"{objName}.operations.{operationLuaName}";
                }
                else
                {
                    string message = $"Ошибка обработки параметра " +
                        $"\"{Name}\"." +
                        $" Не задана базовая операция в операции" +
                        $" \"{mode.DisplayText[0]}\", объекта " +
                        $"\"{mainObjName}\".\n";
                    Logs.AddMessage(message);
                }
            }
            else
            {
                string message = $"Ошибка обработки параметра " +
                        $"\"{Name}\"." +
                        $" Указан несуществующий номер операции в операции " +
                        $"\"{baseMode.DisplayText[0]}\" объекта " +
                        $"\"{mainObjName}\".\n";
                Logs.AddMessage(message);
            }


            return res;
        }

        /// <summary>
        /// Сохранить более 1 устройства в параметре.
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="value">Значение параметра</param>
        /// <returns></returns>
        private string SaveMoreThanOneDevice(string luaName, string value)
        {
            string res = "";

            string[] devices = value.Split(' ');
            if (devices.Length > 1)
            {
                string[] modifiedDevices = devices
                    .Select(x => "prg.control_modules." + x).ToArray();
                res = $".{luaName} = " +
                    $"{{ {string.Join(", ", modifiedDevices)} }}";
            }
            else
            {
                res = $"{luaName} = prg.control_modules.{value}";
            }

            return res;
        }

        private TechObject GetTechObject()
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

        public ValueType CurrentValueType => currentValueType;

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

        private object owner;
        private string luaName;
        private List<DisplayObject> displayObjectsFlags;
        private ValueType currentValueType;

        private Device.DeviceType[] deviceTypes;
        private bool displayParameters;
    }
}
