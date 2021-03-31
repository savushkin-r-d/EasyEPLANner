using System.Collections.Generic;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Операция технологического объекта. Состоит из последовательно 
    /// (или в ином порядке) выполняемых шагов.
    /// </summary>
    public class Mode : TreeViewItem
    {
        /// <summary>
        /// Получение состояния номеру (нумерация с 0).
        /// </summary>
        /// <param name="idy">Номер состояния.</param>        
        /// <returns>Состояние с заданным номером.</returns>
        public State this[int idy]
        {
            get
            {
                int statesCount = (int)State.StateType.STATES_CNT;
                if (idy < statesCount)
                {
                    return stepsMngr[idy];
                }

                return null;
            }
        }

        /// <summary>
        /// Создание новой операции.
        /// </summary>
        /// <param name="name">Имя операции.</param>
        /// <param name="getN">Функция получения номера операции.</param>
        /// <param name="newOwner">Владелец операции (Менеджер операций)
        /// </param>
        public Mode(string name, GetN getN, ModesManager newOwner)
        {
            this.name = name;
            this.getN = getN;
            this.owner = newOwner;

            restrictionMngr = new RestrictionManager();

            stepsMngr = new List<State>();

            var lastState = State.StateType.STATES_CNT;
            var secondState = State.StateType.PAUSE;
            stepsMngr.Add(new State(State.StateType.RUN, this, true));
            for (var state = secondState; state < lastState; state++)
            {
                stepsMngr.Add(new State(state, this));
            }

            operPar = new OperationParams();

            // Экземпляр класса базовой операции
            baseOperation = new BaseOperation(this); 

            SetItems();
        }

        /// <summary>
        /// Добавление полей в массив для отображения на дереве.
        /// </summary>
        void SetItems()
        {
            items = new ITreeViewItem[stepsMngr.Count + 3];

            for (int i = 0; i < stepsMngr.Count; i++)
            {
                items[i] = stepsMngr[i];
            }

            items[stepsMngr.Count] = operPar;
            items[stepsMngr.Count + 1] = restrictionMngr;
            items[stepsMngr.Count + 2] = baseOperation;
        }

        public OperationParams GetOperationParams()
        {
            return operPar;
        }

        public Mode Clone(GetN getN, ModesManager newOwner, string name = "")
        {
            Mode clone = (Mode)MemberwiseClone();
            clone.getN = getN;
            clone.owner = newOwner;
            clone.baseOperation = baseOperation.Clone(clone);

            if (name != string.Empty)
            {
                clone.name = name;
            }

            clone.stepsMngr = new List<State>();
            for (int idx = 0; idx < stepsMngr.Count; idx++)
            {
                clone.stepsMngr.Add(stepsMngr[idx].Clone());
            }

            clone.operPar = operPar.Clone(clone);

            clone.restrictionMngr = restrictionMngr.Clone();
            clone.SetItems();

            return clone;
        }

        public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.ModifyDevNames(newTechObjectN, oldTechObjectN,
                    techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName, 
            int oldTechObjectNumber)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.ModifyDevNames(newTechObjectName,
                    newTechObjectNumber, oldTechObjectName, 
                    oldTechObjectNumber);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "{\n" +
                prefix + "name = \'" + name + "\',\n" +
                prefix + "base_operation = \'" + baseOperation.LuaName + 
                "\',\n";

            res += baseOperation.SaveAsLuaTable(prefix);

            string tmp;
            string tmp_2 = string.Empty;

            for (int j = 0; j < stepsMngr.Count; j++)
            {
                tmp = stepsMngr[j].SaveAsLuaTable(prefix + "\t\t");
                if (tmp != string.Empty)
                {
                    tmp_2 += prefix + "\t[ " + (j + 1) + " ] =\n";
                    tmp_2 += prefix + "\t\t{\n";
                    tmp_2 += tmp;
                    tmp_2 += prefix + "\t\t},\n";
                }
            }
            if (tmp_2 != string.Empty)
            {
                res += prefix + "states =\n" +
                    prefix + "\t{\n";
                res += tmp_2;
                res += prefix + "\t},\n";
            }


            res += prefix + "},\n";
            return res;
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="stepName">Имя шага.</param>
        /// <param name="baseStepLuaName">Имя базового шага</param>
        /// <param name="stateN">Номер состояния</param>
        public void AddStep(int stateN, string stepName, 
            string baseStepLuaName = "")
        {
            int statesCount = (int)State.StateType.STATES_CNT;
            if (stateN >= 0 && stateN < statesCount)
            {
                stepsMngr[stateN].AddStep(stepName, baseStepLuaName);
            }
        }

        #region Синхронизация устройств в объекте.
        public void Synch(int[] array)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.Synch(array);
            }

            baseOperation.Synch(array);
        }
        #endregion


        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = string.Empty;
            string tmp = string.Empty;
            tmp += restrictionMngr.SaveRestrictionAsLua(prefix);
            if (tmp != string.Empty)
            {
                res += prefix + "{\n" + tmp + prefix + "},\n";
            }
            return res;
        }

        public RestrictionManager GetRestrictionManager()
        {
            return restrictionMngr;
        }

        /// <summary>
        /// Функция установки ограничений для операции
        /// </summary>
        /// <param name="luaName">Lua имя объекта</param>
        /// <param name="value">Новая строка ограничений</param>
        public void SetRestriction(string luaName, string value)
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                if (restrict.LuaName == luaName)
                {
                    restrict.SetNewValue(value);
                }
            }
        }

        /// <summary>
        /// Функция установки ограничений для операции
        /// </summary>
        /// <param name="luaName">Lua имя объекта</param>
        /// <param name="value">Новая строка ограничений</param>
        public void AddRestriction(string luaName, int ObjNum, int ModeNum)
        {

            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                if (restrict.LuaName == luaName)
                {
                    restrict.AddRestriction(ObjNum, ModeNum);
                }
            }
        }

        /// <summary>
        /// Функция удаления ограничения для операции
        /// </summary>
        public void DelRestriction(string luaName, int ObjNum, int ModeNum)
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                if (restrict.LuaName == luaName)
                {
                    restrict.DelRestriction(ObjNum, ModeNum);
                }
            }
        }

        /// <summary>
        /// Функция для сортировки ограничений после считывания из файла
        /// </summary>
        public void SortRestriction()
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                restrict.SortRestriction();
            }
        }

        /// <summary>
        /// Функция для задания номеров родителей ограничений
        /// </summary>
        public void SetRestrictionOwner(int objNum, int modeNum)
        {
            foreach (Restriction restrict in restrictionMngr.Restrictions)
            {
                restrict.SetRestrictionOwner(objNum, modeNum);
            }
        }

        public void CheckRestriction(int prev, int curr)
        {
            restrictionMngr.CheckRestriction(prev, curr);
        }

        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {

            restrictionMngr.ModifyRestrictObj(oldObjN, newObjN);
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            restrictionMngr.ChangeModeNum(objNum, prev, curr);
        }

        public void ChangeCrossRestriction(Mode oldMode = null)
        {
            if (oldMode == null)
            {
                restrictionMngr.ChangeCrossRestriction();
            }
            else
            {
                restrictionMngr.ChangeCrossRestriction(oldMode
                    .GetRestrictionManager());
            }
        }

        /// <summary>
        /// Проверить, выбрана ли такая базовая операция или нет
        /// </summary>
        /// <param name="baseOperationName">Проверяемое имя базовой операции
        /// </param>
        /// <returns></returns>
        private bool CheckTheSameBaseOperations(string baseOperationName)
        {
            var objectAlreadyContainsThisOperation = false;
            var modes = this.owner;

            foreach(var mode in modes.Modes)
            {
                if ((mode.BaseOperation.Name == baseOperationName ||
                    mode.BaseOperation.LuaName == baseOperationName) &&
                    baseOperationName != string.Empty)
                {
                    objectAlreadyContainsThisOperation = true;
                }
            }

            return objectAlreadyContainsThisOperation;
        }

        // Установка параметров базовой операции
        public void SetBaseOperExtraParams(ObjectProperty[] extraParams)
        {
            baseOperation.SetExtraProperties(extraParams);
        }

        /// <summary>
        /// Получить базовую операцию для этой операции
        /// </summary>
        public BaseOperation BaseOperation
        {
            get
            {
                return baseOperation;
            }
        }

        // Получение номера операции
        public int GetModeNumber()
        {
            return getN(this);
        }

        public ModesManager Owner
        {
            get
            {
                return owner;
            }
        }

        public string Name
        {
            get { return name; }
        }

        public List<State> States
        {
            get
            {
                return stepsMngr;
            }
        }

        /// <summary>
        /// Проверка состояний состоящих из шагов
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;
            List<State> stepsManager = stepsMngr;

            foreach (State state in stepsManager)
            {
                errors += state.Check();
            }

            errors += BaseOperation.Check();

            return errors;
        }

        /// <summary>
        /// Очистка базовой операции
        /// </summary>
        public void ClearBaseOperation()
        {
            this.SetNewValue(string.Empty, true);
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = getN(this) + ". " + name;

                return new string[] { res, baseOperation.Name };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return items;
            }
        }

        override public bool SetNewValue(string newName)
        {
            name = newName;
            return true;
        }

        public override bool SetNewValue(string newBaseOperationName, 
            bool isBaseOper)
        {
            bool similarBaseOperation = CheckTheSameBaseOperations(
                newBaseOperationName);
            // Инициализация базовой операции по имени
            if (baseOperation.Name != newBaseOperationName &&
                similarBaseOperation == false)
            {
                baseOperation.Init(newBaseOperationName, this);
                return true;
            }

            return false;
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
                //Можем редактировать содержимое двух колонок.
                return new int[] { 0, 1 };
            }
        }

        override public string[] EditText
        {
            get
            {
                return new string[] { name, baseOperation.Name };
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
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

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var selectedState = child as State;
            var copiedState = copyObject as State;
            bool statesNotNull = selectedState != null && copiedState != null;
            if (statesNotNull)
            {
                State newState = copiedState.Clone();
                if (newState.Name != selectedState.Name)
                {
                    newState.Name = selectedState.Name;
                }
                int index = stepsMngr.IndexOf(selectedState);
                stepsMngr.Remove(selectedState);
                stepsMngr.Insert(index, newState);
                SetItems();

                newState.AddParent(this);
                return newState;
            }

            var selectesRestrMan = child as RestrictionManager;
            var copiedRestrMan = copyObject as RestrictionManager;
            bool restrictionNotNull =
                selectesRestrMan != null && copiedRestrMan != null;
            if (restrictionNotNull)
            {
                for (int i = 0; i < selectesRestrMan.Restrictions.Count; i++)
                {
                    selectesRestrMan.Replace(selectesRestrMan.Items[i],
                        copiedRestrMan.Items[i]);
                }

                int objNum = TechObjectManager.GetInstance()
                    .GetTechObjectN(owner.Owner);
                int modeNum = getN(this);

                foreach (var restrict in selectesRestrMan.Restrictions)
                {
                    restrict.SetRestrictionOwner(objNum, modeNum);
                }

                selectesRestrMan.AddParent(this);
                return selectesRestrMan;
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

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        override public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            var devToDraw = new List<DrawInfo>();
            foreach (State stpMngr in stepsMngr)
            {
                List<DrawInfo> devToDrawTmp = stpMngr
                    .GetObjectToDrawOnEplanPage();
                foreach (DrawInfo dinfo in devToDrawTmp)
                {
                    bool isSetFlag = false;
                    for (int i = 0; i < devToDraw.Count; i++)
                    {
                        if (devToDraw[i].DrawingDevice.Name == 
                            dinfo.DrawingDevice.Name)
                        {
                            isSetFlag = true;
                            if (devToDraw[i].DrawingStyle != dinfo.DrawingStyle)
                            {
                                devToDraw.Add(new DrawInfo(
                                    DrawInfo.Style.GREEN_RED_BOX,
                                    devToDraw[i].DrawingDevice));
                                devToDraw.RemoveAt(i);
                            }
                        }
                    }

                    if (isSetFlag == false)
                    {
                        devToDraw.Add(dinfo);
                    }
                }
            }

            return devToDraw;
        }

        public override List<string> BaseObjectsList
        {
            get
            {
                ModesManager modesManager = Owner;
                TechObject techObject = modesManager.Owner;
                BaseTechObject baseTechObject = techObject.BaseTechObject;
                List<string> baseModesList = baseTechObject.BaseOperationsList;
                return baseModesList;
            }
        }

        public override bool ContainsBaseObject
        {
            get
            {
                return true;
            }
        }

        public override bool ShowWarningBeforeDelete
        {
            get
            {
                return true;
            }
        }

        public override bool NeedRebuildParent
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
                return ImageIndexEnum.Mode;
            }
        }

        public override bool IsMode
        {
            get
            {
                return true;
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=operation";
        }

        public void SetUpFromBaseTechObject(BaseOperation baseOperation)
        {
            bool setBaseOperation = true;
            SetNewValue(baseOperation.Name, setBaseOperation);

            foreach(var state in States)
            {
                var stateSteps = baseOperation
                    .GetStateBaseSteps(state.Type)
                    .Where(x => x.DefaultPosition > 0)
                    .OrderBy(x => x.DefaultPosition);
                foreach(var baseStep in stateSteps)
                {
                    while(state.Steps.Count < baseStep.DefaultPosition)
                    {
                        state.Insert();
                    }

                    var newStep = (Step)state.Insert();
                    newStep.SetUpFromBaseTechObject(baseStep);
                }
            }
        }

        private GetN getN;

        private string name;           /// Имя операции.
        private List<State> stepsMngr;/// Список шагов операции для состояний.
        private RestrictionManager restrictionMngr;
        private ITreeViewItem[] items;

        private OperationParams operPar;

        private ModesManager owner;

        private BaseOperation baseOperation; /// Базовая операция

        public const string DefaultModeName = "Новая операция";
    }
}
