using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Операция технологического объекта. Состоит из последовательно 
    /// (или в ином порядке) выполняемых шагов.
    /// </summary>
    public class Mode : Editor.TreeViewItem
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
                if (idy < (int)StateName.STATES_CNT)
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

            stepsMngr.Add(new State(StateStr[(int)StateName.RUN], true, this, 
                true));
            for (StateName i = StateName.PAUSE; i < StateName.STATES_CNT; i++)
            {
                stepsMngr.Add(new State(StateStr[(int)i], false, this));
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
            items = new Editor.ITreeViewItem[stepsMngr.Count + 3];

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

            if (name != "")
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

        /// <summary>
        /// Изменение менеджера операций объекта-владельца операций.
        /// </summary>
        /// <param name="newOwner">Новый менеджер операций.</param>
        /// <returns></returns>
        public void ChangeOwner(ModesManager newOwner)
        {
            owner = newOwner;
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
            int newTechObjectNumber, string oldTechObjectName)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.ModifyDevNames(newTechObjectName,
                    newTechObjectNumber, oldTechObjectName);
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
                prefix + "base_operation = \'" + baseOperation.Name + 
                "\',\n";

            res += baseOperation.SaveAsLuaTable(prefix);

            int i = 1;
            string tmp;
            string tmp_2 = "";

            for (int j = 0; j < stepsMngr.Count; j++)
            {
                tmp = stepsMngr[j].SaveAsLuaTable(prefix + "\t\t");
                if (tmp != "")
                {
                    tmp_2 += prefix + "\t[ " + i++ + " ] =\n";
                    tmp_2 += prefix + "\t\t{\n";
                    tmp_2 += tmp;
                    tmp_2 += prefix + "\t\t},\n";
                }
            }
            if (tmp_2 != "")
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
            if (stateN >= 0 && stateN < (int)StateName.STATES_CNT)
            {
                stepsMngr[stateN].AddStep(stepName, baseStepLuaName);
            }
        }

        public List<Step> MainSteps
        {
            get
            {
                return stepsMngr[0].Steps;
            }
        }

        public void Synch(int[] array)
        {
            foreach (State stpsMngr in stepsMngr)
            {
                stpsMngr.Synch(array);
            }
        }


        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            string tmp = "";
            tmp += restrictionMngr.SaveRestrictionAsLua(prefix);
            if (tmp != "")
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
                if (mode.GetBaseOperation().Name == baseOperationName &&
                    baseOperationName != "")
                {
                    objectAlreadyContainsThisOperation = true;
                }
            }

            return objectAlreadyContainsThisOperation;
        }

        // Установка параметров базовой операции
        public void SetBaseOperExtraParams(Editor.ObjectProperty[] extraParams)
        {
            baseOperation.SetExtraProperties(extraParams);
        }

        public BaseOperation GetBaseOperation()
        {
            return baseOperation;
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

            return errors;
        }

        /// <summary>
        /// Очистка базовой операции
        /// </summary>
        public void ClearBaseOperation()
        {
            this.SetNewValue("", true);
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

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                //return stepsMngr.ToArray();
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
                baseOperation.Init(newBaseOperationName);
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

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {

            if (child is State)
            {
                State stpMngr = child as State;
                if (copyObject is State && stpMngr != null)
                {
                    State newStpMngr = (copyObject as State).Clone();
                    int index = stepsMngr.IndexOf(stpMngr);
                    stepsMngr.Remove(stpMngr);
                    stepsMngr.Insert(index, newStpMngr);

                    return newStpMngr;
                }
            }

            if (child is RestrictionManager)
            {
                RestrictionManager restrictMan = child as RestrictionManager;

                if (copyObject is RestrictionManager && restrictMan != null)
                {
                    var copyMan = copyObject as RestrictionManager;
                    for (int i = 0; i < restrictMan.Restrictions.Count; i++)
                    {
                        restrictMan.Replace(restrictMan.Items[i], 
                            copyMan.Items[i]);
                    }

                    int objNum = TechObjectManager.GetInstance()
                        .GetTechObjectN(owner.Owner);
                    int modeNum = getN(this);

                    foreach (Restriction restrict in restrictMan.Restrictions)
                    {
                        restrict.SetRestrictionOwner(objNum, modeNum);
                    }
                    return restrictMan;
                }
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

        override public object Copy()
        {
            return this;
        }

        override public bool IsInsertable
        {
            get
            {
                return false;
            }
        }

        override public bool IsDrawOnEplanPage
        {
            get
            {
                return true;
            }
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            List<Editor.DrawInfo> devToDraw = new List<Editor.DrawInfo>();
            foreach (State stpMngr in stepsMngr)
            {
                List<Editor.DrawInfo> devToDrawTmp = stpMngr
                    .GetObjectToDrawOnEplanPage();
                foreach (Editor.DrawInfo dinfo in devToDrawTmp)
                {
                    bool isSetFlag = false;
                    for (int i = 0; i < devToDraw.Count; i++)
                    {
                        if (devToDraw[i].dev.Name == dinfo.dev.Name)
                        {
                            isSetFlag = true;
                            if (devToDraw[i].style != dinfo.style)
                            {
                                devToDraw.Add(new Editor.DrawInfo(Editor
                                    .DrawInfo.Style.GREEN_RED_BOX,
                                    devToDraw[i].dev));
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
        #endregion

        public enum StateName
        {
            RUN = 0,// Выполнение
            PAUSE,  // Пауза
            STOP,   // Остановка

            STATES_CNT = 3,
        }

        public string[] StateStr =
            {
            "Выполнение",
            "Пауза",
            "Остановка",
            };

        private GetN getN;

        private string name;           /// Имя операции.
        internal List<State> stepsMngr;/// Список шагов операции для состояний.
        private RestrictionManager restrictionMngr;
        private Editor.ITreeViewItem[] items;

        private OperationParams operPar;

        private ModesManager owner;

        private BaseOperation baseOperation; /// Базовая операция
    }
}
