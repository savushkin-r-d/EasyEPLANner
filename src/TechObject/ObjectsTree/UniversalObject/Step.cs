using System.Collections.Generic;
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
            this.name = name ?? string.Empty;
            this.getN = getN;
            IsMainStep = isMainStep;
            Owner = owner;
            baseStep = new BaseStep(string.Empty, string.Empty);
            baseStep.Owner = this;

            items = new List<ITreeViewItem>();

            actions = new List<IAction>();

            AddDefaultActions(isMainStep);
        }

        /// <summary>
        /// Добавить стандартные действия
        /// </summary>
        /// <param name="isMainStep">Главный шаг</param>
        private void AddDefaultActions(bool isMainStep)
        {
            var checkedDevices = new Action("Проверяемые устройства",
                this, "checked_devices", null, null);
            actions.Add(checkedDevices);
            
            var openDevices = new Action(openDevicesActionName, this,
                "opened_devices",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.V,
                    EplanDevice.DeviceType.DO,
                    EplanDevice.DeviceType.M
                });
            openDevices.ImageIndex = ImageIndexEnum.ActionON;
            actions.Add(openDevices);


            var openDevicesActionGroup = new ActionGroupCustom(
                "Включать с задержкой", this, "delay_opened_devices",
                () => 
                {
                    var openedDeviceAction = new ActionCustom("Группа",
                        this, "");
                    openedDeviceAction.CreateAction(new Action("Включать",
                        this,"",
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.V,
                            EplanDevice.DeviceType.DO,
                            EplanDevice.DeviceType.M
                        }));
                    openedDeviceAction.CreateParameter(new ActiveParameter("",
                        "Задержка включения"));
                    return openedDeviceAction;
                });
            actions.Add(openDevicesActionGroup);


            var openReverse = new Action("Включать реверс", this,
                "opened_reverse_devices",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.M
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.M_REV_FREQ,
                    EplanDevice.DeviceSubType.M_REV_FREQ_2,
                    EplanDevice.DeviceSubType.M_REV_FREQ_2_ERROR,
                    EplanDevice.DeviceSubType.M_ATV,
                    EplanDevice.DeviceSubType.M_ATV_LINEAR,
                    EplanDevice.DeviceSubType.M,
                    EplanDevice.DeviceSubType.M_VIRT,
                });
            actions.Add(openReverse);

            var closeDevices = new Action(closeDevicesActionName, this,
                "closed_devices",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.V,
                    EplanDevice.DeviceType.DO,
                    EplanDevice.DeviceType.M
                });
            closeDevices.DrawStyle = DrawInfo.Style.RED_BOX;
            closeDevices.ImageIndex = ImageIndexEnum.ActionOFF;
            actions.Add(closeDevices);

            var closeDevicesActionGroup = new ActionGroupCustom(
                "Выключать с задержкой", this, "delay_closed_devices",
                () =>
                {
                    var closedDeviceAction = new ActionCustom("Группа",
                        this, "");
                    closedDeviceAction.CreateAction(new Action("Выключать",
                        this, "",
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.V,
                            EplanDevice.DeviceType.DO,
                            EplanDevice.DeviceType.M
                        }));
                    closedDeviceAction.CreateParameter(new ActiveParameter("",
                        "Задержка выключения"));
                    return closedDeviceAction;
                });
            actions.Add(closeDevicesActionGroup);

            var openUpperSeats = new ActionGroup("Верхние седла", this,
                "opened_upper_seat_v",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.V,
                    EplanDevice.DeviceType.DO
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.V_MIXPROOF,
                    EplanDevice.DeviceSubType.V_AS_MIXPROOF,
                    EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                    EplanDevice.DeviceSubType.V_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT
                });
            openUpperSeats.DrawStyle = DrawInfo.Style.GREEN_UPPER_BOX;
            openUpperSeats.ImageIndex = ImageIndexEnum.ActionWashUpperSeats;
            actions.Add(openUpperSeats);

            var openLowerSeats = new ActionGroup("Нижние седла", this,
                "opened_lower_seat_v",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.V,
                    EplanDevice.DeviceType.DO
                },
                new EplanDevice.DeviceSubType[]
                {
                    EplanDevice.DeviceSubType.V_MIXPROOF,
                    EplanDevice.DeviceSubType.V_AS_MIXPROOF,
                    EplanDevice.DeviceSubType.V_IOLINK_MIXPROOF,
                    EplanDevice.DeviceSubType.V_VIRT,
                    EplanDevice.DeviceSubType.DO,
                    EplanDevice.DeviceSubType.DO_VIRT
                });
            openLowerSeats.DrawStyle = DrawInfo.Style.GREEN_LOWER_BOX;
            openLowerSeats.ImageIndex = ImageIndexEnum.ActionWashLowerSeats;
            actions.Add(openLowerSeats);

            var requiredFb = new Action("Сигналы для включения", this,
                "required_FB",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.DI,
                    EplanDevice.DeviceType.GS
                });
            requiredFb.ImageIndex = ImageIndexEnum.ActionSignals;
            actions.Add(requiredFb);

            var groupWash = new ActionGroupWash("Устройства", this,
                ActionGroupWash.MultiGroupAction);
            groupWash.ImageIndex = ImageIndexEnum.ActionWash;
            actions.Add(groupWash);

            var pairsDiDoAllowedDevTypes = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.SB,
                EplanDevice.DeviceType.DO,
                EplanDevice.DeviceType.HL,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.LS,
                EplanDevice.DeviceType.FS
            };

            var pairsDiDoAllowedInputTypes = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.SB,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.LS,
                EplanDevice.DeviceType.FS
            };

            // Специальное действие - выдача дискретных сигналов 
            // при наличии входного дискретного сигнала.
            var groupDIDO = new ActionGroup(groupDIDOActionName, this,
                "DI_DO", pairsDiDoAllowedDevTypes, null,
                new OneInManyOutActionProcessingStrategy(pairsDiDoAllowedInputTypes));
            groupDIDO.ImageIndex = ImageIndexEnum.ActionDIDOPairs;
            actions.Add(groupDIDO);


            // Специальное действие - выдача дискретных сигналов 
            // при пропадании входного дискретного сигнала.
            var groupInvertedDiDo = new ActionGroup(groupDIDOActionNameInverted,
                this, "inverted_DI_DO", pairsDiDoAllowedDevTypes, null, 
                new OneInManyOutActionProcessingStrategy(pairsDiDoAllowedInputTypes));
            groupInvertedDiDo.ImageIndex = ImageIndexEnum.ActionDIDOPairs;
            actions.Add(groupInvertedDiDo);

            var pairsAiAoAllowedInputTypes = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.AI,
                EplanDevice.DeviceType.PT,
                EplanDevice.DeviceType.LT,
                EplanDevice.DeviceType.FQT,
                EplanDevice.DeviceType.QT,
                EplanDevice.DeviceType.TE,
            };

            // Специальное действие - выдача аналоговых сигналов при
            // наличии входного  аналогового сигнала.
            var groupAiAo = new ActionGroup(groupAIAOActionName, this,
                "AI_AO",
                new EplanDevice.DeviceType[]
                {
                    EplanDevice.DeviceType.AI,
                    EplanDevice.DeviceType.AO,
                    EplanDevice.DeviceType.M,
                    EplanDevice.DeviceType.PT,
                    EplanDevice.DeviceType.LT,
                    EplanDevice.DeviceType.FQT,
                    EplanDevice.DeviceType.QT,
                    EplanDevice.DeviceType.TE,
                    EplanDevice.DeviceType.VC
                },
                null,
                new OneInManyOutActionProcessingStrategy(pairsAiAoAllowedInputTypes));
            groupAiAo.ImageIndex = ImageIndexEnum.ActionDIDOPairs;
            actions.Add(groupAiAo);

            var enableStepBySignal = new ActionGroupCustom(
                "Сигнал для включения шага", this, "enable_step_by_signal",
                () =>
                {
                    var enableStepBySignalAction = new ActionCustom("Группа",
                        this, "");
                    enableStepBySignalAction.CreateAction(new Action("Сигналы",
                        this, "",
                        new EplanDevice.DeviceType[]
                        {
                            EplanDevice.DeviceType.DI,
                        }));
                    enableStepBySignalAction.CreateParameter(new ActiveBoolParameter("",
                        "Выключать шаг по пропаданию сигнала", "false"));
                    return enableStepBySignalAction;
                });

            actions.Add(enableStepBySignal);

            items.AddRange(actions.Cast<ITreeViewItem>().ToArray());

            if (!isMainStep)
            {
                var toStepByConditionAction = new ActionToStepByCondition(
                    "Переход к шагу по условию", this,
                    "to_step_if_devices_in_specific_state");
                actions.Add(toStepByConditionAction);
                items.Add(toStepByConditionAction);

                timeParam = new ObjectProperty("Время (параметр)", -1, -1);
                items.Add(timeParam);

                nextStepN = new ObjectProperty("Номер следующего шага", -1, -1);
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

            clone.actions = new List<IAction>();
            foreach (IAction action in actions)
            {
                clone.actions.Add(action.Clone());
            }

            clone.items = new List<ITreeViewItem>();
            clone.items.AddRange(clone.actions.Cast<ITreeViewItem>().ToArray());

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
            foreach (IAction action in actions)
            {
                action.ModifyDevNames(newTechObjectN,
                    oldTechObjectN, techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (IAction action in actions)
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
                foreach (IAction action in actions)
                {
                    res += action.SaveAsLuaTable(prefix);
                }
            }
            else
            {
                res += prefix + "{\n";
                res += prefix + "name = \'" + name + "\',\n";

                string time_param_n = timeParam.EditText[1].Trim();
                string next_step_n = nextStepN.EditText[1].Trim();

                double leadTime = double.Parse(time_param_n,
                    System.Globalization.CultureInfo.InvariantCulture);
                
                if (leadTime <= 0)
                {
                    next_step_n = "-1";
                    time_param_n = "-1";
                    nextStepN.SetNewValue("-1");
                    timeParam.SetNewValue("-1");
                }

                if (time_param_n != string.Empty)
                {
                    res += prefix + "time_param_n = " + time_param_n + ",\n";
                }

                if (next_step_n != string.Empty)
                {
                    res += prefix + "next_step_n = " + next_step_n + ",\n";
                }

                string baseStepName = baseStep.LuaName;
                res += prefix + $"baseStep = \'{baseStepName}\',\n";

                foreach (IAction action in actions)
                {
                    res += action.SaveAsLuaTable(prefix);
                }

                res += prefix + "},\n";
            }

            return res;
        }
   
        /// <summary>
        /// Сохарение шага в виде таблицы Excel
        /// </summary>
        /// <returns>Описание в виде списка строк(клеток) Excel</returns>
        public string[] SaveAsExcel()
        {
            var res = new List<string>();

            res.Add(name);
            foreach (IAction action in actions)
            {
                res.Add(action.SaveAsExcel()); 
            }
            res.Add(TimeParam);
            res.Add(NextStepN);

            return res.ToArray();
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
        /// <param name="subActionLuaName">Имя поддействия</param>
        public bool AddDev(string actionLuaName, string devName,
            int groupNumber, string subActionLuaName)
        {
            int devId = EplanDevice.DeviceManager.GetInstance()
                .GetDeviceIndex(devName);
            if (devId == -1)
            {
                return false;
            }

            var action = GetActionByLuaName(actionLuaName, actions);
            bool haveAction = action != null;
            if(haveAction)
            {
                action.AddDev(devId, groupNumber, subActionLuaName);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Добавление параметра.
        /// Вызывается из Lua-скрипта sys.lua.
        /// </summary>
        /// <param name="actionLuaName">Имя действия в Lua.</param>
        /// <param name="val">Значение параметра.</param>
        /// <param name="groupNumber">Индекс группы в действии
        /// мойки (устройства)</param>
        /// <param name="paramName">Имя параметра</param>
        public bool AddParam(string actionLuaName, object val, string paramName,
            int groupNumber)
        {
            IAction action = GetActionByLuaName(actionLuaName, actions);
            bool haveAction = action != null;
            if (haveAction)
            {
                action.AddParam(val, paramName, groupNumber);
                return true;
            }

            return false;
        }

        private IAction GetActionByLuaName(string name, List<IAction> actions)
        {
            foreach (IAction act in actions)
            {
                if (act.LuaName == string.Empty)
                {
                    break;
                }

                if(act.LuaName == name)
                {
                    return act;
                }

                if (name == ActionGroupWash.SingleGroupAction)
                {
                    return actions.Where(x => x.LuaName ==
                    ActionGroupWash.MultiGroupAction).First();
                }
            }

            return null;
        }

        public List<IAction> GetActions
        {
            get
            {
                return actions;
            }
        }

        #region Синхронизация устройств в объекте.
        public void Synch(int[] array)
        {
            foreach (IAction action in actions)
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

        public override ITreeViewItem Replace(object child, object copyObject)
        {
            var copyAction = copyObject as IAction;
            var childAction = child as IAction;
            bool notNullObjects = copyAction != null && childAction != null;
            if (notNullObjects)
            {
                bool canReplace = copyAction.LuaName == childAction.LuaName;
                if (canReplace)
                {
                    var newAction = copyAction.Clone();

                    int index = actions.IndexOf(childAction);
                    actions.RemoveAt(index);
                    actions.Insert(index, newAction);

                    newAction.Owner = this;

                    newAction.AddParent(this);

                    var childActionAsITreeViewItem = (ITreeViewItem)childAction;
                    var newActionAsITreeViewItem = (ITreeViewItem)newAction;
                    index = items.IndexOf(childActionAsITreeViewItem);
                    items.RemoveAt(index);
                    items.Insert(index, newActionAsITreeViewItem);
                    return newAction as ITreeViewItem;
                }
            }

            return null;
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
            var action = child as IAction;
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
            foreach (IAction action in actions)
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
                EplanDevice.IDevice device = EplanDevice.DeviceManager.GetInstance()
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
                x.Name == groupDIDOActionName ||
                x.Name == groupDIDOActionNameInverted);

            foreach(var group in checkingActionsGroups)
            {
                bool hasError = false;
                var groupActions = group.SubActions;
                foreach(IAction groupAction in groupActions)
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

        public string NextStepN
        {
            get { return nextStepN.DisplayText[1].Trim(); }
        }

        public string TimeParam
        {
            get { return timeParam.DisplayText[1].Trim(); }
        }

        private GetN getN;

        private ObjectProperty nextStepN; ///< Номер следующего шага.
        private ObjectProperty timeParam; ///< Параметр времени.
        private List<ITreeViewItem> items;

        private string name; ///< Имя шага.
        internal List<IAction> actions; ///< Список действий шага.

        private string openDevicesActionName = "Включать";
        private string closeDevicesActionName = "Выключать";
        private string groupDIDOActionName = "Группы DI -> DO DO ...";
        private string groupDIDOActionNameInverted =
            "Группы инвертированный DI -> DO DO ...";
        private string groupAIAOActionName = "Группы AI -> AO AO ...";

        private BaseStep baseStep;
    }
}
