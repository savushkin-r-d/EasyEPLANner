using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyEPlanner;
using Editor;
using TechObject.ActionProcessingStrategy;

namespace TechObject
{
    public interface IAction
    {
        void GetDisplayObjects(out Device.DeviceType[] validTypes,
                    out Device.DeviceSubType[] validSubTypes,
                    out bool displayParameter);

        List<int> DeviceIndex { get; set; }
    }

    /// <summary>
    /// Действие над устройствами (включение, выключение и т.д.).
    /// </summary>
    public class Action : TreeViewItem, IAction
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="devTypes">Типы устройств, допустимые для 
        /// редактирования.</param>
        /// <param name="devSubTypes">Подтипы устройств, допустимые 
        /// для редактирования.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        /// <param name="actionProcessorStrategy">Стратегия обработки
        /// устройств в действии</param>
        /// <param name="deviceManager">Менеджер устройств</param>
        public Action(string name, Step owner, string luaName = "",
            Device.DeviceType[] devTypes = null,
            Device.DeviceSubType[] devSubTypes = null,
            IActionProcessorStrategy actionProcessorStrategy = null,
            Device.IDeviceManager deviceManager = null)
        {
            this.name = name;
            this.luaName = luaName;
            this.devTypes = devTypes;
            this.devSubTypes = devSubTypes;
            deviceIndex = new List<int>();
            this.owner = owner;

            DrawStyle = DrawInfo.Style.GREEN_BOX;

            this.deviceManager = deviceManager ?? Device.DeviceManager
                .GetInstance();

            SetActionProcessingStrategy(actionProcessorStrategy);
        }

        public virtual Action Clone()
        {
            var clone = (Action)MemberwiseClone();
            clone.SetActionProcessingStrategy(actionProcessorStrategy);

            clone.deviceIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                clone.deviceIndex.Add(index);
            }

            return clone;
        }

        virtual public void ModifyDevNames(int newTechObjectN,
            int oldTechObjectN, string techObjectName)
        {
            List<int> tmpIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                tmpIndex.Add(index);
            }

            foreach (int index in deviceIndex)
            {
                var newDevName = string.Empty;
                Device.IDevice device = deviceManager.GetDeviceByIndex(index);
                int objNum = device.ObjectNumber;
                string objName = device.ObjectName;

                if (objNum > 0)
                {
                    //Для устройств в пределах объекта меняем номер объекта.
                    if (techObjectName == objName)
                    {
                        // COAG2V1 --> COAG1V1
                        if (objNum == newTechObjectN && oldTechObjectN != -1)
                        {
                            newDevName = objName + oldTechObjectN +
                                device.DeviceType.ToString() + device.
                                DeviceNumber;
                        }
                        if (oldTechObjectN == -1 ||
                            oldTechObjectN == objNum)
                        {
                            //COAG1V1 --> COAG2V1
                            newDevName = objName + newTechObjectN +
                                device.DeviceType.ToString() + device
                                .DeviceNumber;
                        }
                    }
                }

                if (newDevName != string.Empty)
                {
                    int indexOfDeletingElement = tmpIndex.IndexOf(index);
                    tmpIndex.Remove(index);
                    int tmpDevInd = deviceManager.GetDeviceIndex(newDevName);
                    if (tmpDevInd >= 0)
                    {
                        tmpIndex.Insert(indexOfDeletingElement, tmpDevInd);
                    }
                }
            }

            deviceIndex = tmpIndex;
        }

        virtual public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName, 
            int oldTechObjNumber)
        {
            List<int> tmpIndex = new List<int>();
            foreach (int index in deviceIndex)
            {
                tmpIndex.Add(index);
            }

            foreach (int index in deviceIndex)
            {
                var newDevName = string.Empty;
                Device.IDevice device = deviceManager.GetDeviceByIndex(index);
                int objNum = newTechObjectNumber;
                string objName = device.ObjectName;

                if (objName == oldTechObjectName &&
                    device.ObjectNumber == oldTechObjNumber)
                {
                    newDevName = newTechObjectName + objNum +
                        device.DeviceType.ToString() + device.DeviceNumber;
                }

                if (newDevName != string.Empty)
                {
                    int indexOfDeletingElement = tmpIndex.IndexOf(index);
                    tmpIndex.Remove(index);
                    int tmpDevInd = deviceManager.GetDeviceIndex(newDevName);
                    if (tmpDevInd >= 0)
                    {
                        tmpIndex.Insert(indexOfDeletingElement, tmpDevInd);
                    }
                }
            }

            deviceIndex = tmpIndex;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public virtual string SaveAsLuaTable(string prefix)
        {
            if (deviceIndex.Count == 0)
            {
                return string.Empty;
            }

            string res = prefix;
            if (LuaName != string.Empty)
            {
                res += $"{LuaName} = ";
            }

            res += $"--{name}\n{prefix}\t{{\n";
            res += $"{prefix}\t";

            int devicesCounter = 0;
            foreach (int index in deviceIndex)
            {
                var device = deviceManager.GetDeviceByIndex(index);
                string devName = device.Name;
                if (devName != StaticHelper.CommonConst.Cap)
                {
                    devicesCounter++;
                    res += $"'{devName}', ";
                }
            }

            if (devicesCounter == 0)
            {
                return string.Empty;
            }

            res = res.Remove(res.Length - 2, 2);
            res += "\n";

            res += $"{prefix}\t}},\n";
            return res;
        }

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="groupNumber">Номер группы в действии.</param>
        /// <param name="index">Индекс устройства</param>
        /// <param name="washGroupIndex">Индекс группы в действии мойка 
        /// (устройства)</param>
        public virtual void AddDev(int index, int groupNumber = 0,
            int washGroupIndex = 0)
        {
            var device = deviceManager.GetDeviceByIndex(index);
            if (device.Description != StaticHelper.CommonConst.Cap)
            {
                deviceIndex.Add(index);
            }
        }

        /// <summary>
        /// Добавление параметра к действию.
        /// </summary>
        /// <param name="val">Значение параметра.</param>
        /// <param name="washGroupIndex">Индекс группы мойки в действии
        /// (устройства)</param>
        public virtual void AddParam(object val, int washGroupIndex = 0) { }
            
        /// <summary>
        /// Очищение списка устройств.
        /// </summary>
        virtual public void Clear()
        {
            deviceIndex.Clear();
        }

        #region Синхронизация устройств в объекте.
        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        virtual public void Synch(int[] array)
        {
            IDeviceSynchronizeService synchronizer = DeviceSynchronizer
                .GetSynchronizeService();
            synchronizer.SynchronizeDevices(array, ref deviceIndex);
        }
        #endregion

        /// <summary>
        /// Получение/установка устройств.
        /// </summary>
        public List<int> DeviceIndex
        {
            get
            {
                return deviceIndex;
            }
            set
            {
                deviceIndex = value;
            }
        }

        public string LuaName
        {
            get
            {
                return luaName;
            }
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                var res = string.Empty;

                foreach (int index in deviceIndex)
                {
                    res += $"{deviceManager.GetDeviceByIndex(index).Name} ";
                }

                if (res != string.Empty)
                {
                    res = res.Remove(res.Length - 1);
                }

                return new string[] { name, res };
            }
        }

        override public bool SetNewValue(string newName)
        {
            newName = newName.Trim();

            if (newName == string.Empty)
            {
                Clear();
                return true;
            }

            Match strMatch = Regex.Match(newName,
                Device.DeviceManager.DESCRIPTION_PATTERN_MULTYLINE,
                RegexOptions.IgnoreCase);
            if (!strMatch.Success)
            {
                return false;
            }

            IList<int> allowedDevicesId =
                actionProcessorStrategy.ProcessDevices(newName, deviceManager);
            DeviceIndex.Clear();
            deviceIndex.AddRange(allowedDevicesId);

            return true;
        }

        override public bool IsEditable
        {
            get
            {
                return true;
            }
        }

        override public int[] EditablePart
        {
            get
            {
                //Можем редактировать содержимое второй колонки.
                return new int[] { -1, 1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                string res = string.Empty;
                foreach (int index in deviceIndex)
                {
                    res += $"{deviceManager.GetDeviceByIndex(index).Name} ";
                }

                if (res != string.Empty)
                {
                    res = res.Remove(res.Length - 1);
                }

                return new string[] { string.Empty, res };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return true;
            }
        }

        override public void GetDisplayObjects(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            devTypes = this.devTypes;
            devSubTypes = this.devSubTypes;
            displayParameters = false;
        }

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        virtual public DrawInfo.Style DrawStyle { get; set; }

        override public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            var devToDraw = new List<DrawInfo>();
            foreach (int index in deviceIndex)
            {
                devToDraw.Add(new DrawInfo(DrawStyle, 
                    deviceManager.GetDeviceByIndex(index)));
            }

            return devToDraw;
        }

        public override bool IsFilled
        {
            get
            {
                if (Empty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                switch(luaName)
                {
                    case OpenDevices:
                        return ImageIndexEnum.ActionON;

                    case CloseDevices:
                        return ImageIndexEnum.ActionOFF;

                    case RequiredFB:
                        return ImageIndexEnum.ActionSignals;

                    default:
                        return ImageIndexEnum.NONE;
                }
            }
        }
        #endregion

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool Empty
        {
            get
            {
                if (deviceIndex.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetActionProcessingStrategy(
            IActionProcessorStrategy strategy)
        {
            if (strategy == null)
            {
                actionProcessorStrategy = new DefaultActionProcessorStrategy();
            }
            else
            {
                actionProcessorStrategy = strategy;
            }

            actionProcessorStrategy.Action = this;
        }

        public IActionProcessorStrategy GetActionProcessingStrategy()
            => actionProcessorStrategy;


        protected string luaName; // Имя действия в таблице Lua.
        protected string name; // Имя действия.
        protected List<int> deviceIndex; // Список устройств.

        protected Device.DeviceType[] devTypes; // Отображаемые типы
        protected Device.DeviceSubType[] devSubTypes; // Отображаемые подтипы.

        protected Step owner; // Владелец элемента.

        protected private const string GroupDefaultName = "Группа";

        public const string OpenDevices = "opened_devices";
        public const string CloseDevices = "closed_devices";
        public const string OpenReverseDevices = "opened_reverse_devices";
        public const string RequiredFB = "required_FB";

        protected private const string DO = "DO";
        protected private const string DI = "DI";
        protected private const string Devices = "devices";
        protected private const string ReverseDevices = "rev_devices";

        IActionProcessorStrategy actionProcessorStrategy;
        Device.IDeviceManager deviceManager;
    }

    namespace ActionProcessingStrategy
    {
        public interface IActionProcessorStrategy
        {
            IList<int> ProcessDevices(string devicesStr,
                Device.IDeviceManager deviceManager);

            IAction Action { get; set; }
        }

        public class DefaultActionProcessorStrategy : IActionProcessorStrategy
        {
            public virtual IList<int> ProcessDevices(
                string devicesStr, Device.IDeviceManager deviceManager)
            {
                Match match = Regex.Match(devicesStr,
                    Device.DeviceManager.DESCRIPTION_PATTERN, RegexOptions.
                    IgnoreCase);

                var validDevices = new List<int>();
                while (match.Success)
                {
                    string str = match.Groups["name"].Value;
                    Device.IDevice device = deviceManager
                        .GetDeviceByEplanName(str);
                    bool isValid = ValidateDevice(device);
                    if (isValid)
                    {
                        int tmpDeviceIndex = deviceManager.GetDeviceIndex(str);
                        if (tmpDeviceIndex >= 0 &&
                            !validDevices.Contains(tmpDeviceIndex))
                        {
                            validDevices.Add(tmpDeviceIndex);
                        }
                    }

                    match = match.NextMatch();
                }

                return validDevices;
            }

            /// <summary>
            /// Функция проверки добавляемого устройства
            /// </summary>
            /// <param name="device">Устройство</param>
            /// <returns></returns>
            private bool ValidateDevice(Device.IDevice device)
            {
                bool isValidType = false;
                Device.DeviceType deviceType = device.DeviceType;
                Device.DeviceSubType deviceSubType = device.DeviceSubType;

                Action.GetDisplayObjects(out Device.DeviceType[] validTypes,
                    out Device.DeviceSubType[] validSubTypes, out _);

                if (validTypes == null)
                {
                    return true;
                }
                else
                {
                    foreach (Device.DeviceType type in validTypes)
                    {
                        if (type == deviceType)
                        {
                            isValidType = true;
                            break;
                        }
                        else
                        {
                            isValidType = false;
                        }
                    }

                    if (validSubTypes != null)
                    {
                        bool isValidSubType = false;
                        foreach (Device.DeviceSubType subType in validSubTypes)
                        {
                            if ((subType == deviceSubType) && isValidType)
                            {
                                isValidSubType = true;
                            }
                        }

                        if (isValidSubType && isValidSubType)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return isValidType;
            }

            public IAction Action { get; set; }
        }

        public class OneInManyOutActionProcessingStrategy :
            DefaultActionProcessorStrategy
        {
            public override IList<int> ProcessDevices(string devicesStr,
                Device.IDeviceManager deviceManager)
            {
                IList<int> validatedDevicesId =
                    base.ProcessDevices(devicesStr, deviceManager);

                var idDevDict = new Dictionary<int, Device.IDevice>();
                foreach(var devId in validatedDevicesId)
                {
                    var dev = deviceManager.GetDeviceByIndex(devId);
                    idDevDict.Add(devId, dev);
                }

                var newInputDevs = idDevDict
                    .Where(x => allowedInputDevTypes.Contains(
                        x.Value.DeviceType) == true)
                    .ToList();
                if (newInputDevs.Count > 1)
                {
                    foreach (var newInputDevPair in newInputDevs)
                    {
                        var newInputDevId = newInputDevPair.Key;
                        if (Action.DeviceIndex.Contains(newInputDevId))
                        {
                            idDevDict.Remove(newInputDevId);
                        }
                    }
                }

                List<int> devList;
                //bool isCorrectList = idDevDict
                //    .Where(x => allowedInputDevTypes.Contains(
                //        x.Value.DeviceType))
                //    .Count() <= 1;
                //if (isCorrectList)
                //{
                    devList = idDevDict
                        .ToList()
                        .OrderBy(x => x.Value.DeviceType,
                            new OneInManyDevicesComparer(allowedInputDevTypes))
                        .Select(x => x.Key)
                        .ToList();
                //}
                //else
                //{
                //    devList = idDevDict
                //        .Where(x => allowedInputDevTypes.Contains(
                //            x.Value.DeviceType) == false)
                //        .Select(x => x.Key)
                //        .ToList();
                //}

                return devList;
            }

            class OneInManyDevicesComparer : IComparer<Device.DeviceType>
            {
                public OneInManyDevicesComparer(
                    List<Device.DeviceType> allowedFirstPlaceDevTypes)
                {
                    this.allowedFirstPlaceDevTypes = allowedFirstPlaceDevTypes;
                }

                public int Compare(Device.DeviceType x, Device.DeviceType y)
                {
                    if (allowedFirstPlaceDevTypes.Contains(x)) return -1;
                    //if (allowedFirstPlaceDevTypes.Contains(y)) return 1;
                    //if (allowedFirstPlaceDevTypes.Contains(x) &&
                    //    allowedFirstPlaceDevTypes.Contains(y)) return 0;

                    return x.ToString().CompareTo(y.ToString());
                }

                private List<Device.DeviceType> allowedFirstPlaceDevTypes;
            }

            private List<Device.DeviceType> allowedInputDevTypes =
                new List<Device.DeviceType>()
                { 
                    Device.DeviceType.AI,
                    Device.DeviceType.DI,
                    Device.DeviceType.GS
                };
        }
    }
}
