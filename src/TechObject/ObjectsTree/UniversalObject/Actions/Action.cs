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
        void GetDisplayObjects(out EplanDevice.DeviceType[] validTypes,
                    out EplanDevice.DeviceSubType[] validSubTypes,
                    out bool displayParameter);

        /// <summary>
        /// Индексы устройств в дейсвии.
        /// </summary>
        List<int> DeviceIndex { get; set; }

        IAction Clone();

        /// <summary>
        /// Добавление устройства к действию.
        /// </summary>
        /// <param name="groupNumber">Номер группы в действии.</param>
        /// <param name="index">Индекс устройства</param>
        /// <param name="subActionLuaName">Lua-имя поддействия</param>
        void AddDev(int index, int groupNumber, string subActionLuaName);
        
        string LuaName { get; }

        bool HasSubActions { get; }

        List<IAction> SubActions { get; set; }

        /// <summary>
        /// Очищение списка устройств.
        /// </summary>
        void Clear();

        bool Empty { get; }

        bool IsFilled { get; }

        string Name { get; }

        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение 
        /// индексов.</param>
        void Synch(int[] array);

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        string SaveAsLuaTable(string prefix);

        /// <summary>
        /// Сохраняет устройства действия в одну строку без префикса:
        /// "{ 'dev1', 'dev2', 'dev3' }"
        /// </summary>
        /// <returns>Описание Lua в виде одной строки</returns>
        string SaveAsLuaTableInline();

        /// <summary>
        /// Сохранения действия в клетке Excel
        /// </summary>
        /// <returns>Описание действия в клетке Excel</returns>
        string SaveAsExcel();

        void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName);

        void ModifyDevNames(string newTechObjectName, int newTechObjectNumber,
            string oldTechObjectName, int oldTechObjNumber);

        List<DrawInfo> GetObjectToDrawOnEplanPage();

        DrawInfo.Style DrawStyle { get; set; }

        /// <summary>
        /// Добавление параметра к действию.
        /// </summary>
        /// <param name="val">Значение параметра.</param>
        /// <param name="grouNumber">Индекс группы в действии </param>
        /// <param name="paramName">Имя параметра</param>
        void AddParam(object val, string paramName, int grouNumber);

        List<string> DevicesNames { get; }

        IDeviceProcessingStrategy GetDeviceProcessingStrategy();

        void SetDeviceProcessingStrategy(IDeviceProcessingStrategy strategy);

        Step Owner { get; set; }

        void AddParent(ITreeViewItem parent);
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
        public Action(string name, Step owner, string luaName,
            EplanDevice.DeviceType[] devTypes, EplanDevice.DeviceSubType[] devSubTypes,
            IDeviceProcessingStrategy actionProcessorStrategy,
            EplanDevice.IDeviceManager deviceManager) : this(name, owner, luaName,
                devTypes, devSubTypes, actionProcessorStrategy)
        {
            this.deviceManager = deviceManager;
        }

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
        public Action(string name, Step owner, string luaName,
            EplanDevice.DeviceType[] devTypes, EplanDevice.DeviceSubType[] devSubTypes,
            IDeviceProcessingStrategy actionProcessorStrategy)
        : this(name, owner, luaName, devTypes, devSubTypes)
        {
            SetDeviceProcessingStrategy(actionProcessorStrategy);
        }

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
        public Action(string name, Step owner, string luaName,
            EplanDevice.DeviceType[] devTypes, EplanDevice.DeviceSubType[] devSubTypes)
        : this(name, owner, luaName, devTypes)
        {
            this.devSubTypes = devSubTypes;
        }

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="devTypes">Типы устройств, допустимые для 
        /// редактирования.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action(string name, Step owner, string luaName,
            EplanDevice.DeviceType[] devTypes) : this(name, owner, luaName)
        {
            this.devTypes = devTypes;
        }

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public Action(string name, Step owner, string luaName)
        {
            this.name = name;
            this.luaName = luaName;
            this.owner = owner;

            deviceIndex = new List<int>();
        }

        public virtual IAction Clone()
        {
            var clone = (Action)MemberwiseClone();
            clone.SetDeviceProcessingStrategy(deviceProcessingStrategy);

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
                EplanDevice.IDevice device = deviceManager.GetDeviceByIndex(index);
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
                EplanDevice.IDevice device = deviceManager.GetDeviceByIndex(index);
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

        public virtual string SaveAsLuaTable(string prefix)
        {
            if (deviceIndex.Count == 0)
            {
                return string.Empty;
            }

            string res = prefix;
            if (LuaName != string.Empty)
            {
                res += $"{LuaName} =";
            }

            if (Name != string.Empty)
            {
                res += $" --{name}";
            }

            res += $"\n{prefix}\t{{\n";
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

        public string SaveAsLuaTableInline()
        {
            if (deviceIndex.Count == 0)
            {
                return string.Empty;
            }

            string res = string.Empty;

            res += $"{{ ";

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
            res += $" }}";

            return res;
        }

        public virtual string SaveAsExcel()
        {
            if (deviceIndex.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", DevicesNames);
        }

        public virtual void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            var device = deviceManager.GetDeviceByIndex(index);
            if (device.Description != StaticHelper.CommonConst.Cap)
            {
                deviceIndex.Add(index);
            }
        }

        public virtual void AddParam(object val, string paramName,
            int groupNumber) { }

        virtual public void Clear()
        {
            deviceIndex.Clear();
        }

        public void SetDeviceProcessingStrategy(IDeviceProcessingStrategy strategy)
        {
            deviceProcessingStrategy = strategy;
            if (strategy != null)
            {
                strategy.Action = this;
            }
        }

        public IDeviceProcessingStrategy GetDeviceProcessingStrategy()
        {
            if (deviceProcessingStrategy == null)
            {
                SetDeviceProcessingStrategy(new DefaultActionProcessorStrategy());
            }

            return deviceProcessingStrategy;
        }

        #region Синхронизация устройств в объекте.
        virtual public void Synch(int[] array)
        {
            IDeviceSynchronizeService synchronizer = DeviceSynchronizer
                .GetSynchronizeService();
            synchronizer.SynchronizeDevices(array, ref deviceIndex);
        }
        #endregion

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

        public List<string> DevicesNames
        {
            get
            {
                var devices = deviceIndex
                    .Select(x => deviceManager.GetDeviceByIndex(x).Name)
                    .ToList();
                return devices;
            }
        }

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, ToString() };
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
                EplanDevice.DeviceManager.DESCRIPTION_PATTERN_MULTYLINE,
                RegexOptions.IgnoreCase);
            if (!strMatch.Success)
            {
                return false;
            }

            IList<int> allowedDevicesId = GetDeviceProcessingStrategy()
                .ProcessDevices(newName, deviceManager);
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
                return new string[] { string.Empty, ToString() };
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

        override public void GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
            out EplanDevice.DeviceSubType[] devSubTypes, out bool displayParameters)
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

        virtual public DrawInfo.Style DrawStyle { get; set; } = DrawInfo.Style
            .GREEN_BOX;

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

        public override bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        public override bool IsReplaceable
        {
            get
            {
                return true;
            }
        }
        #endregion

        public virtual bool HasSubActions
        {
            get => false;
        }

        public virtual List<IAction> SubActions
        {
            get => null;
            set { } // Not used
        }

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

        public Step Owner
        {
            get => owner;
            set => owner = value;
        }

        public override string ToString()
        {
            bool hasDevices = DeviceIndex.Count > 0;
            if (hasDevices)
            {
                return $"{string.Join(" ", DevicesNames)}";
            }
            else
            {
                return string.Empty;
            }
        }

        protected string luaName;
        protected string name;
        protected List<int> deviceIndex;

        protected EplanDevice.DeviceType[] devTypes;
        protected EplanDevice.DeviceSubType[] devSubTypes;

        protected Step owner;

        IDeviceProcessingStrategy deviceProcessingStrategy;
        EplanDevice.IDeviceManager deviceManager = EplanDevice.DeviceManager
            .GetInstance();
    }

    namespace ActionProcessingStrategy
    {
        public interface IDeviceProcessingStrategy
        {
            IList<int> ProcessDevices(string devicesStr,
                EplanDevice.IDeviceManager deviceManager);

            IAction Action { get; set; }
        }

        public class DefaultActionProcessorStrategy : IDeviceProcessingStrategy
        {
            public virtual IList<int> ProcessDevices(
                string devicesStr, EplanDevice.IDeviceManager deviceManager)
            {
                Match match = Regex.Match(devicesStr,
                    EplanDevice.DeviceManager.DESCRIPTION_PATTERN, RegexOptions.
                    IgnoreCase);

                var validDevices = new List<int>();
                while (match.Success)
                {
                    string str = match.Groups["name"].Value;
                    EplanDevice.IDevice device = deviceManager
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
            private bool ValidateDevice(EplanDevice.IDevice device)
            {
                bool isValidType = false;
                EplanDevice.DeviceType deviceType = device.DeviceType;
                EplanDevice.DeviceSubType deviceSubType = device.DeviceSubType;

                Action.GetDisplayObjects(out EplanDevice.DeviceType[] validTypes,
                    out EplanDevice.DeviceSubType[] validSubTypes, out _);

                if (validTypes == null)
                {
                    return true;
                }
                else
                {
                    foreach (EplanDevice.DeviceType type in validTypes)
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
                        foreach (EplanDevice.DeviceSubType subType in validSubTypes)
                        {
                            if ((subType == deviceSubType) && isValidType)
                            {
                                isValidSubType = true;
                            }
                        }

                        return isValidType && isValidSubType;
                    }
                }

                return isValidType;
            }

            public IAction Action { get; set; }
        }

        public class OneInManyOutActionProcessingStrategy :
            DefaultActionProcessorStrategy
        {
            public OneInManyOutActionProcessingStrategy(
                EplanDevice.DeviceType[] allowedInputDevTypes)
            {
                this.allowedInputDevTypes = allowedInputDevTypes;
            }

            public override IList<int> ProcessDevices(string devicesStr,
                EplanDevice.IDeviceManager deviceManager)
            {
                IList<int> validatedDevicesId =
                    base.ProcessDevices(devicesStr, deviceManager);

                var idDevDict = new Dictionary<int, EplanDevice.IDevice>();
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

                bool incorrectCountInputDevs = idDevDict
                    .Where(x => allowedInputDevTypes.Contains(
                        x.Value.DeviceType))
                    .Count() > 1;
                List<int> devList;
                if (incorrectCountInputDevs)
                {
                    devList = idDevDict.Where(x => allowedInputDevTypes
                            .Contains(x.Value.DeviceType) == false)
                        .Select(x => x.Key)
                        .ToList();
                }
                else
                {
                    devList = idDevDict
                        .ToList()
                        .OrderBy(x => x.Value.DeviceType,
                            new OneInManyDevicesComparer(allowedInputDevTypes))
                        .Select(x => x.Key)
                        .ToList();
                }

                return devList;
            }

            class OneInManyDevicesComparer : IComparer<EplanDevice.DeviceType>
            {
                public OneInManyDevicesComparer(
                    EplanDevice.DeviceType[] allowedFirstPlaceDevTypes)
                {
                    this.allowedFirstPlaceDevTypes = allowedFirstPlaceDevTypes;
                }

                public int Compare(EplanDevice.DeviceType x, EplanDevice.DeviceType y)
                {
                    if (x == y) return 0;

                    if(allowedFirstPlaceDevTypes.Contains(x) &&
                        !allowedFirstPlaceDevTypes.Contains(y))
                    {
                        return -1;
                    }

                    if (!allowedFirstPlaceDevTypes.Contains(x) &&
                        allowedFirstPlaceDevTypes.Contains(y))
                    {
                        return 1;
                    }

                    return x.ToString().CompareTo(y.ToString());
                }

                private EplanDevice.DeviceType[] allowedFirstPlaceDevTypes;
            }

            private EplanDevice.DeviceType[] allowedInputDevTypes;
        }
    }
}
