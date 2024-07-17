using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Aga.Controls.Tree;
using BrightIdeasSoftware;
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
        List<int> DevicesIndex { get; set; }

        /// <summary>
        /// Индексы устройств из типового объекта (с удаленными исключенными устройствами)
        /// </summary>
        List<int> GenericDevicesIndexAfterExclude { get; }

        /// <summary>
        /// Все индексы устройств (в действии и в действии типового объекта)
        /// </summary>
        List<int> GetAllDeviceIndex();

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

        List<string> GetGenericDevicesNames();

        List<string> GetAllDevicesNames();

        IDeviceProcessingStrategy GetDeviceProcessingStrategy();

        void SetDeviceProcessingStrategy(IDeviceProcessingStrategy strategy);

        Step Owner { get; set; }

        void AddParent(ITreeViewItem parent);

        void UpdateOnGenericTechObject(IAction genericAction);

        void CreateGenericByTechObjects(IEnumerable<IAction> actions);

        void UpdateOnDeleteGeneric();
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

            clone.deviceIndex = new List<int>(deviceIndex);
            clone.genericDeviceIndex = new List<int>();
            clone.excludedGenericDeviceIndex = new List<int>();

            return clone;
        }

        virtual public void ModifyDevNames(int newTechObjectN,
            int oldTechObjectN, string techObjectName)
        {
            if (oldTechObjectN != 0)
                deviceIndex = ModifyDevNamesChangeTechNumbers(newTechObjectN,
                    oldTechObjectN, techObjectName, deviceIndex);
            genericDeviceIndex = ModifyDevNamesChangeTechNumbers(newTechObjectN,
                oldTechObjectN, techObjectName, genericDeviceIndex, true);
            deviceIndex = IndexesExclude(deviceIndex, genericDeviceIndex);
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

        private List<int> ModifyDevNamesChangeTechNumbers(int newTechObjectN,
            int oldTechObjectN, string techObjectName, List<int> devs, bool isGeneric = false)
        {
            List<int> tmpDevs = new List<int>();
            tmpDevs.AddRange(devs);

            foreach (int dev in devs)
            {
                var newDevName = ModifyDevNameChangeTechNumber(techObjectName, dev, newTechObjectN, oldTechObjectN);

                if (string.IsNullOrEmpty(newDevName))
                    continue;

                int devIndex = tmpDevs.IndexOf(dev);
                int tmpDev = deviceManager.GetDeviceIndex(newDevName);

                if (tmpDev >= 0)
                    tmpDevs[devIndex] = tmpDev;
                else if (isGeneric)
                    tmpDevs.RemoveAt(devIndex);
            }

            return tmpDevs;
        }

        private string ModifyDevNameChangeTechNumber(string techObjectName, int index, int newID, int oldID)
        {
            var newDevName = string.Empty;
            EplanDevice.IDevice device = deviceManager.GetDeviceByIndex(index);
            int objNum = device.ObjectNumber;
            string objName = device.ObjectName;

            if (objNum <= 0 || techObjectName != objName)
                return string.Empty;

            //Для устройств в пределах объекта меняем номер объекта.
            if (objNum == newID && oldID != -1)
            { // 1 -> 2 - COAG2V1 --> COAG1V1
                return $"{objName}{oldID}{device.DeviceDesignation}";
            }
            if (oldID == -1 || oldID == objNum)
            { // COAG1V1 --> COAG(new_id)V1; COAGxV1 -> COAG1V1, COAG2V1 ...
                return $"{objName}{newID}{device.DeviceDesignation}";
            }

            return string.Empty;
        }

        public virtual string SaveAsLuaTable(string prefix)
        {
            if (GetAllDeviceIndex().Count == 0)
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
            foreach (int index in GetAllDeviceIndex())
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
            if (GetAllDeviceIndex().Count == 0)
            {
                return string.Empty;
            }

            string res = string.Empty;

            res += $"{{ ";

            int devicesCounter = 0;
            foreach (int index in GetAllDeviceIndex())
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

        public virtual void UpdateOnGenericTechObject(IAction genericAction)
        {
            if (genericAction is null)
            {
                deviceIndex.AddRange(genericDeviceIndex);
                genericDeviceIndex.Clear();
                return;
            }

            SetGenericDevices((genericAction as Action).DevicesIndex);
        }

        public virtual void CreateGenericByTechObjects(IEnumerable<IAction> actions)
        {
            var refTechObject = actions.First().Owner.Owner.Owner.Owner.Owner;
            actions.Skip(1).ToList().ForEach(action 
                => action.ModifyDevNames(refTechObject.TechNumber,
                    action.Owner.Owner.Owner.Owner.Owner.TechNumber,
                    refTechObject.NameEplan));

            deviceIndex = actions.Skip(1).Aggregate(new HashSet<int>(actions.First().DevicesIndex),
                (h, e) => { h.IntersectWith(e.DevicesIndex); return h; }).ToList();
        }

        public override void UpdateOnDeleteGeneric()
        {
            deviceIndex = deviceIndex.Concat(GenericDevicesIndexAfterExclude).ToList();
            genericDeviceIndex.Clear();
            excludedGenericDeviceIndex.Clear();
        }
        

        #region Синхронизация устройств в объекте.
        virtual public void Synch(int[] array)
        {
            IDeviceSynchronizeService synchronizer = DeviceSynchronizer
                .GetSynchronizeService();
            synchronizer.SynchronizeDevices(array, ref deviceIndex);
        }
        #endregion

        public List<int> DevicesIndex
        {
            get => deviceIndex;
            set => deviceIndex = value;
        }

        public List<int> GetAllDeviceIndex() => DevicesIndex
            .Concat(GenericDevicesIndexAfterExclude).ToList();

        public List<int> GenericDevicesIndexAfterExclude
        {
            get => IndexesExclude(genericDeviceIndex, excludedGenericDeviceIndex);
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

        public List<string> GetGenericDevicesNames() =>
            GenericDevicesIndexAfterExclude
                .Select(x => deviceManager.GetDeviceByIndex(x).Name)
                .ToList();

        public List<string> GetAllDevicesNames() => DevicesNames
            .Concat(GetGenericDevicesNames()).ToList();

        #region Реализация ITreeViewItem

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, ToString() };
            }
        }

        public override IRenderer[] CellRenderer =>
            new IRenderer[] { null, GenericDevicesRenderer };

        /// <summary>
        /// Подсветка устройств из типового объекта
        /// </summary>
        private HighlightTextRenderer GenericDevicesRenderer
        {
            get
            {
                genericDevicesRenderer.Filter.ContainsStrings =
                    new string[] { string.Join(" ", GetGenericDevicesNames()) };
                return genericDevicesRenderer;
            }
        }

        private readonly HighlightTextRenderer genericDevicesRenderer = new HighlightTextRenderer()
        {
            Filter = TextMatchFilter.Contains(Editor.Editor.GetInstance().EditorForm.editorTView, string.Empty),
            FillBrush = new SolidBrush(Color.YellowGreen),
            FramePen = new Pen(Color.White),
        };

        /// <summary>
        /// Исключить из списка-источника элементы списка исключений
        /// </summary>
        /// <param name="source"> Список-источник </param>
        /// <param name="exclude"> Список с исключениями </param>
        private List<int> IndexesExclude(List<int> source, List<int> exclude)
            => source.Where(dev => !exclude.Contains(dev)).ToList();

        override public bool SetNewValue(string newName)
        {
            newName = newName.Trim();

            if (newName == string.Empty)
            {
                Clear();
                excludedGenericDeviceIndex = genericDeviceIndex;
                OnValueChanged(this);
                return true;
            }

            Match strMatch = Regex.Match($"{newName}",
                EplanDevice.DeviceManager.DESCRIPTION_PATTERN_MULTYLINE,
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
            if (!strMatch.Success)
            {
                return false;
            }

            List<int> allowedDevicesId = GetDeviceProcessingStrategy()
                .ProcessDevices($"{newName}", deviceManager).ToList();

            excludedGenericDeviceIndex = IndexesExclude(genericDeviceIndex, allowedDevicesId);

            allowedDevicesId = IndexesExclude(allowedDevicesId, genericDeviceIndex);

            DevicesIndex.Clear();
            deviceIndex.AddRange(allowedDevicesId);

            OnValueChanged(this);

            return true;
        }

        /// <summary>
        /// Загружены ли данные проекта?
        /// </summary>
        [ExcludeFromCodeCoverage]
        private bool ProjectDataIsLoaded
        {
            get
            {
                try
                {
                    return EProjectManager.GetInstance().ProjectDataIsLoaded;
                }
                catch 
                { 
                    return true; 
                }
            }
            
        }

        /// <summary>
        /// Установить устройства из типового объекта
        /// </summary>
        public bool SetGenericDevices(List<int> genericDevicesID)
        {
            var devicesID = IndexesExclude(deviceIndex, genericDevicesID);

            if (ProjectDataIsLoaded)
            {
                excludedGenericDeviceIndex = excludedGenericDeviceIndex
                    .Where(excluded => genericDevicesID.Contains(excluded)).ToList();
            }
            else
            { // При загрузке из LUA
                if (genericDevicesID.Any())
                {
                    excludedGenericDeviceIndex = IndexesExclude(genericDevicesID, deviceIndex);
                }
            }

            deviceIndex.Clear();
            deviceIndex.AddRange(devicesID);

            genericDeviceIndex.Clear();
            genericDeviceIndex.AddRange(genericDevicesID);

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

        public override bool IsFilled => !Empty;

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

        public override bool IsInsertableCopy => true;

        public override ITreeViewItem InsertCopy(object obj)
        {
            if (obj is Action copiedAction)
            {
                SetNewValue(copiedAction.ToString());
                return this;
            }
            return null;
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

        public virtual bool Empty => GetAllDeviceIndex().Count == 0;

        public Step Owner
        {
            get => owner;
            set => owner = value;
        }

        public override string ToString()
        {
            bool hasDevices = (DevicesIndex.Concat(genericDeviceIndex)).ToList().Count > 0;

            var devs = string.Join(" ", DevicesNames);
            var genDevs = string.Join(" ", GetGenericDevicesNames());

            if (hasDevices)
            {
                return $"{devs} {genDevs}".Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public void OnSubActionChanged(object sender)
        {
            OnValueChanged(sender);
        }

        public override string SystemIdentifier => "process_action";

        protected string luaName;
        protected string name;

        protected List<int> deviceIndex;
        /// <summary>
        /// Типовые устройства для действия
        /// </summary>
        protected List<int> genericDeviceIndex = new List<int>();
        /// <summary>
        /// Исключенные типовые устройства
        /// </summary>
        protected List<int> excludedGenericDeviceIndex = new List<int>();

        protected EplanDevice.DeviceType[] devTypes;
        protected EplanDevice.DeviceSubType[] devSubTypes;

        protected Step owner;

        IDeviceProcessingStrategy deviceProcessingStrategy;
        
        private static EplanDevice.IDeviceManager deviceManager = EplanDevice.DeviceManager
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
                EplanDevice.DeviceType deviceType = device.DeviceType;
                EplanDevice.DeviceSubType deviceSubType = device.DeviceSubType;

                Action.GetDisplayObjects(out EplanDevice.DeviceType[] validTypes,
                    out EplanDevice.DeviceSubType[] validSubTypes, out _);

                return (validTypes?.Contains(deviceType) ?? true) 
                    && (validSubTypes?.Contains(deviceSubType) ?? true);
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
                        if (Action.DevicesIndex.Contains(newInputDevId))
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
