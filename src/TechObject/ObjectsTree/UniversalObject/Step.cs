﻿using System.Collections.Generic;
using System.Linq;
using Editor;
using TechObject.ActionProcessingStrategy;

namespace TechObject
{
    /// <summary>
    /// Шаг технологического объекта. Состоит из параллельно выполняемых 
    /// действий.
    /// </summary>
    public class Step : TreeViewItem
    {
        /// <summary>
        /// Создание нового шага.
        /// </summary>
        /// <param name="name">Имя шага.</param>
        /// <param name="getN">Функция получения номера шага.</param>
        /// <param name="isMainStep">Признак того, является ли шаг 
        /// шагом операции. </param>
        /// <param name="owner">Владелец шага (Состояние)</param>
        public Step(string name, GetN getN, State owner,
            bool isMainStep = false)
        {
            this.name = name;
            this.getN = getN;
            IsMainStep = isMainStep;
            Owner = owner;
            baseStep = new BaseStep(string.Empty, string.Empty);
            baseStep.Owner = this;

            items = new List<ITreeViewItem>();

            actions = new List<Action>();
            actions.Add(new Action(openDevicesActionName, this,
                Action.OpenDevices,
                new Device.DeviceType[]
                { 
                    Device.DeviceType.V, 
                    Device.DeviceType.DO, 
                    Device.DeviceType.M
                }));

            actions.Add(new Action("Включать реверс", this, 
                Action.OpenReverseDevices,
                new Device.DeviceType[]
                { 
                    Device.DeviceType.M
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.M_REV_FREQ,
                    Device.DeviceSubType.M_REV_FREQ_2,
                    Device.DeviceSubType.M_REV_FREQ_2_ERROR,
                    Device.DeviceSubType.M_ATV,
                    Device.DeviceSubType.M
                }));

            actions.Add(new Action(closeDevicesActionName, this,
                Action.CloseDevices,
                new Device.DeviceType[]
                { 
                    Device.DeviceType.V, 
                    Device.DeviceType.DO, 
                    Device.DeviceType.M
                }));
            actions[2].DrawStyle = DrawInfo.Style.RED_BOX;

            actions.Add(new ActionGroup("Верхние седла", this,
                ActionGroup.OpenedUpperSeats,
                new Device.DeviceType[]
                {
                    Device.DeviceType.V
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.V_MIXPROOF,
                    Device.DeviceSubType.V_AS_MIXPROOF,
                    Device.DeviceSubType.V_IOLINK_MIXPROOF
                }));
            actions[3].DrawStyle = DrawInfo.Style.GREEN_UPPER_BOX;

            actions.Add(new ActionGroup("Нижние седла", this,
                ActionGroup.OpenedLowerSeats,
                new Device.DeviceType[]
                {
                    Device.DeviceType.V
                },
                new Device.DeviceSubType[]
                {
                    Device.DeviceSubType.V_MIXPROOF,
                    Device.DeviceSubType.V_AS_MIXPROOF,
                    Device.DeviceSubType.V_IOLINK_MIXPROOF
                }));
            actions[4].DrawStyle = DrawInfo.Style.GREEN_LOWER_BOX;

            actions.Add(new Action("Сигналы для включения", this,
                Action.RequiredFB,
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.GS
                }));

            actions.Add(new ActionGroupWash("Устройства", this,
                ActionGroupWash.SingleGroupAction));

            // Специальное действие - выдача дискретных сигналов 
            // при наличии входного дискретного сигнала.
            actions.Add(new ActionGroup(groupDIDOActionName, this,
                ActionGroup.DIDO,
                new Device.DeviceType[]
                {
                    Device.DeviceType.DI,
                    Device.DeviceType.SB,
                    Device.DeviceType.DO
                },
                null,
                new OneInManyOutActionProcessingStrategy()));

            // Специальное действие - выдача аналоговых сигналов при
            // наличии входного  аналогового сигнала.
            actions.Add(new ActionGroup(groupAIAOActionName, this,
                ActionGroup.AIAO,
                new Device.DeviceType[]
                {
                    Device.DeviceType.AI,
                    Device.DeviceType.AO,
                    Device.DeviceType.M
                },
                null,
                new OneInManyOutActionProcessingStrategy()));

            items.AddRange(actions.ToArray());

            if (!isMainStep)
            {
                timeParam = new ObjectProperty("Время (параметр)", -1, -1);
                nextStepN = new ObjectProperty("Номер следующего шага", -1, -1);

                items.Add(timeParam);
                items.Add(nextStepN);
            }
        }

        public Step Clone(GetN getN, string name = "")
        {
            Step clone = (Step)MemberwiseClone();
            clone.getN = getN;

            if (name != string.Empty)
            {
                clone.name = name.Substring(3);
            }

            clone.actions = new List<Action>();
            foreach (Action action in actions)
            {
                clone.actions.Add(action.Clone());
            }

            clone.items = new List<ITreeViewItem>();
            clone.items.AddRange(clone.actions.ToArray());

            if (!IsMainStep)
            {
                clone.timeParam = timeParam.Clone();
                clone.nextStepN = nextStepN.Clone();

                clone.items.Add(clone.timeParam);
                clone.items.Add(clone.nextStepN);
            }

            clone.baseStep = baseStep.Clone();
            clone.baseStep.Owner = this;

            return clone;
        }

        public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Action action in actions)
            {
                action.ModifyDevNames(newTechObjectN,
                    oldTechObjectN, techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (Action action in actions)
            {
                action.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    oldTechObjectName, oldTechObjectNumber);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <param name="isShortForm">Сохранять ли сокращенном виде (для  
        /// операции без вывода названия шага).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix, bool isShortForm = false)
        {
            string res = string.Empty;

            if (isShortForm)
            {
                foreach (Action action in actions)
                {
                    res += action.SaveAsLuaTable(prefix);
                }
            }
            else
            {
                res += prefix + "{\n";
                res += prefix + "name = \'" + name + "\',\n";

                string time_param_n = timeParam.EditText[1].Trim();
                if (time_param_n != string.Empty)
                {
                    res += prefix + "time_param_n = " + time_param_n + ",\n";
                }

                string next_step_n = nextStepN.EditText[1].Trim();
                if (next_step_n != string.Empty)
                {
                    res += prefix + "next_step_n = " + next_step_n + ",\n";
                }

                string baseStepName = baseStep.LuaName;
                res += prefix + $"baseStep = \'{baseStepName}\',\n";

                foreach (Action action in actions)
                {
                    res += action.SaveAsLuaTable(prefix);
                }

                res += prefix + "},\n";
            }

            return res;
        }

        /// <summary>
        /// Добавление параметров.
        /// </summary>
        /// <param name="time_param_n">Номер параметра со временем шага.
        /// </param>
        /// <param name="next_step_n">Номер следующего шага.</param>
        public void SetPar(int timeParamN, int nextStepN)
        {
            this.timeParam.SetNewValue(timeParamN.ToString());
            this.nextStepN.SetNewValue(nextStepN.ToString());
        }

        /// <summary>
        /// Добавление устройства.
        /// </summary>
        /// <param name="actionLuaName">Имя действия в Lua.</param>
        /// <param name="devName">Имя устройства.</param>
        /// <param name="groupNumber">Номер группы.</param>
        /// <param name="washGroupIndex">Номер группы для действия 
        /// мойки (устройства)</param>
        /// <param name="innerActionIndex">Индекс внутреннего действия.</param>
        public bool AddDev(string actionLuaName, string devName,
            int groupNumber = 0, int washGroupIndex = 0)
        {
            int index = Device.DeviceManager.GetInstance()
                .GetDeviceIndex(devName);
            if (index == -1)
            {
                return false;
            }

            foreach (Action act in actions)
            {
                if (act.LuaName == actionLuaName)
                {
                    act.AddDev(index, groupNumber, washGroupIndex);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Добавление параметра.
        /// 
        /// Вызывается из Lua-скрипта sys.lua.
        /// </summary>
        /// <param name="actionLuaName">Имя действия в Lua.</param>
        /// <param name="val">Значение параметра.</param>
        /// <param name="washGroupIndex">Индекс группы в действии
        /// мойки (устройства)</param>
        public bool AddParam(string actionLuaName, object val,
            int washGroupIndex = 0)
        {
            foreach (Action act in actions)
            {
                if (act.LuaName == actionLuaName)
                {
                    act.AddParam(val, washGroupIndex);
                    return true;
                }
            }

            return false;
        }

        public List<Action> GetActions
        {
            get
            {
                return actions;
            }
        }

        #region Синхронизация устройств в объекте.
        public void Synch(int[] array)
        {
            foreach (Action action in actions)
            {
                action.Synch(array);
            }
        }
        #endregion

        public string GetStepName()
        {
            return this.name;
        }

        public int GetStepNumber()
        {
            return getN(this);
        }

        public State Owner { get; set; }

        /// <summary>
        /// Lua-имя базового шага
        /// </summary>
        /// <returns></returns>
        public string GetBaseStepLuaName()
        {
            return baseStep.LuaName;
        }

        /// <summary>
        /// Имя базового шага
        /// </summary>
        /// <returns></returns>
        public string GetBaseStepName()
        {
            return baseStep.Name;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                if (getN(this) == 0)
                {
                    return new string[] { name, string.Empty };
                }

                return new string[] { getN(this) + ". " + name, baseStep.Name };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items.ToArray();
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;
            return true;
        }

        public override bool SetNewValue(string newVal, bool isExtraValue)
        {
            State state = Owner;

            Step equalStep = state.Steps
                .Where(x => x.GetBaseStepName() == newVal)
                .FirstOrDefault();
            if (equalStep == null)
            {
                equalStep = state.Steps
                    .Where(x => x.GetBaseStepLuaName() == newVal)
                    .FirstOrDefault();
            }

            if (equalStep != null && newVal != string.Empty)
            {
                return false;
            }

            Mode mode = state.Owner;
            BaseStep baseStep = mode.BaseOperation
                .GetStateBaseSteps(state.Type)
                .Where(x => x.LuaName == newVal).FirstOrDefault();
            if (baseStep == null)
            {
                baseStep = mode.BaseOperation
                    .GetStateBaseSteps(state.Type)
                    .Where(x => x.Name == newVal).FirstOrDefault();
            }

            if (baseStep != null)
            {
                this.baseStep = baseStep.Clone();
                this.baseStep.Owner = this;
                if (name.Contains(NewStepName) && baseStep.Name != string.Empty)
                {
                    name = baseStep.Name;
                }

                return true;
            }

            return false;
        }

        override public bool IsEditable
        {
            get
            {
                if (IsMode)
                {
                    return false;
                }

                return true;
            }
        }

        override public int[] EditablePart
        {
            get
            {
                // Можем редактировать содержимое левой колонки, или обе.
                return IsMainStep ? new int[] { 0, -1 } : new int[] { 0, 1 };
            }
        }

        override public bool IsMoveable
        {
            get
            {
                return true;
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, baseStep.Name };
            }
        }

        override public bool IsCopyable
        {
            get
            {
                return true;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                if (IsMode)
                {
                    return false;
                }

                return true;
            }
        }

        override public bool Delete(object child)
        {
            var action = child as Action;
            if (action != null)
            {
                action.Clear();
            }

            if (child.GetType() == typeof(ObjectProperty))
            {
                var objectProperty = child as ObjectProperty;
                objectProperty.Delete(this);
            }

            return false;
        }

        override public bool IsDrawOnEplanPage
        {
            get { return true; }
        }

        override public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            List<DrawInfo> devToDraw = new List<DrawInfo>();
            foreach (Action action in actions)
            {
                devToDraw.AddRange(action.GetObjectToDrawOnEplanPage());
            }

            return devToDraw;
        }

        public override List<string> BaseObjectsList
        {
            get
            {
                State state = Owner;
                Mode mode = state.Owner;
                List<string> stepsNames = mode.BaseOperation
                    .GetStateStepsNames(state.Type);
                return stepsNames;
            }
        }

        public override bool ContainsBaseObject
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
                if (items.Where(x => x.IsFilled).Count() > 0)
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
                return ImageIndexEnum.Step;
            }
        }
        #endregion

        #region Проверка действий на ошибки
        /// <summary>
        /// Проверка действий в шаге
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;

            State state = Owner;
            Mode mode = state.Owner;
            ModesManager modesManager = mode.Owner;
            TechObject techObject = modesManager.Owner;
            string techObjName = techObject.DisplayText[0];
            string modeName = mode.Name;

            errors += CheckOpenAndCloseActions(techObjName, modeName);
            errors += CheckInOutGroupActions(techObjName, modeName);
            return errors;
        }

        private string CheckOpenAndCloseActions(string techObjName,
            string modeName)
        {
            var errors = string.Empty;
            var devicesInAction = new List<int>();

            var checkingActionsDevs = actions
                .Where(x => x.Name == openDevicesActionName ||
                x.Name == closeDevicesActionName)
                .Select(y => y.DeviceIndex);
            foreach(var devList in checkingActionsDevs)
            {
                devicesInAction.AddRange(devList);
            }

            List<int> FindEqual = devicesInAction.GroupBy(x => x)
                .SelectMany(y => y.Skip(1)).Distinct().ToList();

            foreach (int i in FindEqual)
            {
                Device.IDevice device = Device.DeviceManager.GetInstance()
                    .GetDeviceByIndex(i);
                string msg = $"Неправильно задано устройство {device.Name} " +
                    $"в действиях \"{openDevicesActionName}\" и " +
                    $"\"{closeDevicesActionName}\", в шаге " +
                    $"\"{GetStepName()}\", операции \"{modeName}\"," +
                    $"технологического объекта " +
                    $"\"{techObjName}\"\n";
                errors += msg;
            }

            return errors;
        }

        private string CheckInOutGroupActions(string techObjName,
            string modeName)
        {
            var errors = string.Empty;

            var checkingActionsGroups = actions
                .Where(x => x.Name == groupAIAOActionName ||
                x.Name == groupDIDOActionName);

            foreach(var group in checkingActionsGroups)
            {
                bool hasError = false;
                var groupActions = group.Items;
                foreach(Action groupAction in groupActions)
                {
                    if(groupAction.Empty)
                    {
                        continue;
                    }

                    int devsCount = groupAction.DeviceIndex.Count;
                    if (devsCount == 1)
                    {
                        hasError = true;
                    }

                    var devices = new List<Device.IDevice>();
                    foreach(var devId in groupAction.DeviceIndex)
                    {
                        devices.Add(Device.DeviceManager.GetInstance()
                            .GetDeviceByIndex(devId));
                    }

                    bool hasInput = devices
                        .Any(x => x.DeviceType == Device.DeviceType.DI ||
                        x.DeviceType == Device.DeviceType.AI);
                    bool hasOutput = devices
                        .Any(x => x.DeviceType == Device.DeviceType.DO ||
                        x.DeviceType == Device.DeviceType.AO);
                    if (!hasInput || !hasOutput)
                    {
                        hasError = true;
                    }
                }

                if (hasError)
                {
                    errors += $"Неправильно заполнены сигналы в " +
                        $"действии \"{group.Name}\", " +
                        $"шаге \"{GetStepName()}\", " +
                        $"операции \"{modeName}\", " +
                        $"технологического объекта " +
                        $"\"{techObjName}\"\n";

                    hasError = false;
                }
            }

            return errors;
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=phase";
        }

        public void SetUpFromBaseTechObject(BaseStep baseStep)
        {
            bool setBaseStep = true;
            SetNewValue(baseStep.Name, setBaseStep);
        }

        public bool Empty
        {
            get
            {
                if(actions.Where(x => x.Empty == true).Count() == actions.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Имя нового шага.
        /// </summary>
        public static string NewStepName
        {
            get
            {
                return "Новый шаг";
            }
        }

        /// <summary>
        /// Имя главного шага состояния.
        /// </summary>
        public static string MainStepName
        {
            get
            {
                return "Во время операции";
            }
        }

        /// <summary>
        /// Признак шага операции.
        /// </summary>
        private bool IsMainStep { get; set; }

        private GetN getN;

        private ObjectProperty nextStepN; ///< Номер следующего шага.
        private ObjectProperty timeParam; ///< Параметр времени.
        private List<ITreeViewItem> items;

        private string name;           ///< Имя шага.
        internal List<Action> actions; ///< Список действий шага.

        private string openDevicesActionName = "Включать";
        private string closeDevicesActionName = "Выключать";
        private string groupDIDOActionName = "Группы DI -> DO DO ...";
        private string groupAIAOActionName = "Группы AI -> AO AO ...";

        private BaseStep baseStep;
    }
}
