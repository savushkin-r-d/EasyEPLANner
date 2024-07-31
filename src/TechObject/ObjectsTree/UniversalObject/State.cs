using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Editor;

namespace TechObject
{
    /// <summary>
    /// Состояние операции. Содержит группу шагов, выполняемых последовательно
    /// (или в ином порядке).
    /// </summary>
    public class State : TreeViewItem
    {
        /// <summary>
        /// Получение шага по номеру (нумерация с -1 - шаг операции, который 
        /// выполняется пока включена операция).
        /// </summary>
        /// <param name="idx">Номер шага.</param>        
        /// <returns>Шаг с заданным номером.</returns>
        public Step this[int idx]
        {
            get
            {
                if (MissingMainStep)
                {
                   modeStep = AddNewStepToItems(true);
                   modeStep.AddParent(this);
                }

                if (steps.Count > 0)
                {
                    if (idx == -1)
                    {
                        return steps[0];
                    }

                    if (idx < steps.Count - 1)
                    {
                        return steps[idx + 1];
                    }
                }

                return null;
            }
        }

        public int GetStepN(object step)
        {
            return steps.IndexOf(step as Step);
        }

        /// <summary>
        /// Создание нового состояния.
        /// </summary>
        /// <param name="stateType">Тип состояния</param>
        /// <param name="needMainStep">Надо ли основной шаг.</param>
        /// <param name="owner">Владелец состояния (Операция)</param>
        public State(StateType stateType, Mode owner, bool needMainStep = false)
        {
            name = stateStr[(int)stateType];
            Type = stateType;
            this.owner = owner;
            steps = new List<Step>();

            NeedMainStep = needMainStep;

            if (needMainStep)
            {
                modeStep = AddNewStepToItems(needMainStep);
                modeStep.AddParent(this);
            }
        }

        public State Clone(string name = "")
        {
            State clone = (State)MemberwiseClone();
            clone.Type = Type;

            if (name != string.Empty)
            {
                clone.name = name;
            }

            clone.steps = new List<Step>();

            if (modeStep != null)
            {
                clone.modeStep = modeStep.Clone(clone.GetStepN);
                clone.steps.Add(clone.modeStep);
            }

            for (int idx = 1; idx < steps.Count; idx++)
            {
                clone.steps.Add(steps[idx].Clone(clone.GetStepN));
            }

            clone.Steps.ForEach(step => step.ValueChanged += (sender) => clone.OnValueChanged(sender));

            return clone;
        }

        public void ModifyDevNames(IDevModifyOptions modifyOptions)
        {
            steps.ForEach(step => step.ModifyDevNames(modifyOptions));
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            if (steps.Count == 0) return string.Empty;

            string res = string.Empty;

            if (modeStep != null)
            {
                res += modeStep.SaveAsLuaTable(prefix, true);
            }

            if (steps.Count > 1)
            {
                int i = 1;
                res += prefix + "steps =\n" +
                    prefix + "\t{\n";

                for (int j = 1; j < steps.Count; j++)
                {
                    res += prefix + "\t[ " + i++ + " ] =\n";
                    res += steps[j].SaveAsLuaTable(prefix + "\t\t");
                }

                res += prefix + "\t},\n";
            }

            if (res != string.Empty)
            {
                res = prefix + "--\'" + name + "\'\n" + res;
            }

            return res;
        }


        /// <summary>
        /// Сохранениe действия "Во время операции" в таблицу Excel
        /// </summary>
        /// <returns>Описание в виде таблицы Excel</returns>
        public string[] SaveAsExcel()
        {
            var res = new List<string>();

            res.Add(name);
            foreach (IAction action in Steps.First().GetActions)
            {
                res.Add(action.SaveAsExcel());
            }

            return res.ToArray();
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="stepName">Имя шага.</param>
        /// <param name="baseStepLuaName">Имя базового шага</param>
        public Step AddStep(string stepName, string baseStepLuaName)
        {
            if (MissingMainStep)
            {
                modeStep = AddNewStepToItems(MissingMainStep);
                modeStep.AddParent(this);
            }

            Step newStep = AddNewStepToItems(false, stepName);
            newStep.AddParent(this);

            if (!string.IsNullOrEmpty(baseStepLuaName))
            {
                newStep.SetNewValue(baseStepLuaName, true);
            }
                
            newStep.ValueChanged += (sender) => OnValueChanged(sender);
            OnValueChanged(this);

            return newStep;
        }

        public List<Step> Steps
        {
            get
            {
                return steps;
            }
        }

        #region Синхронизация устройств в объекте.
        public void Synch(int[] array)
        {
            foreach (Step step in steps)
            {
                step.Synch(array);
            }
        }
        #endregion

        /// <summary>
        /// Удаление списка шагов.
        /// </summary>
        public void Clear()
        {
            steps.Clear();
            modeStep = null;
        }

        /// <summary>
        /// Имя состояния
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public Mode Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Проверка шагов состояния
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;
            List<Step> steps = Steps;

            foreach (Step step in steps)
            {
                errors += step.Check();
            }

            return errors;
        }

        /// <summary>
        /// Добавить новый шаг в состояние
        /// </summary>
        /// <param name="isMain">Главный шаг или нет (первый)</param>
        /// <param name="stepName">Имя шага (дефолт, если пустое)</param>
        /// <returns>Добавленный шаг</returns>
        private Step AddNewStepToItems(bool isMain = false,
            string stepName = "")
        {
            Step step;
            if (isMain)
            {
                step = new Step(Step.MainStepName, GetStepN, this, true);
            }
            else
            {
                string newStepName =
                    stepName == string.Empty ? Step.NewStepName : stepName;
                step = new Step(newStepName, GetStepN, this);
            }

            steps.Add(step);

            step.ValueChanged += (sender) => OnValueChanged(sender);
            OnValueChanged(this);

            return step;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = name;
                if (steps.Count > 1)
                {
                    res += " (" + (steps.Count - 1) + ")";
                }

                return new string[] { res, string.Empty };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return steps.ToArray();
            }
        }

        override public bool IsReplaceable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            var step = child as Step;
            if (step != null)
            {
                bool ignoreDeleting = steps.IndexOf(step) == 0 && NeedMainStep;
                if (ignoreDeleting)
                {
                    return false;
                }

                steps.Remove(step);
                return true;
            }

            var action = child as IAction;
            if (action != null)
            {
                action.Clear();
                return false;
            }

            return false;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var step = child as Step;
            if (step is null)
                return null;

            int index = steps.IndexOf(step);

            if (index <= 1)
                return null;


            SwapSteps(index, index - 1);
            OnValueChanged(this);

            return child as ITreeViewItem;
        }

        override public ITreeViewItem MoveDown(object child)
        {
            var step = child as Step;
            if (step is null)
                return null;

            int index = steps.IndexOf(step);
            
            if (index > steps.Count - 2 || index <= 0)
                return null;

            
            SwapSteps(index, index + 1);
            OnValueChanged(this);

            return child as ITreeViewItem;
        }

        private void SwapSteps(int index1, int index2)
        {
            (steps[index1], steps[index2]) = (steps[index2], steps[index1]);
            Editor.Editor.GetInstance().RefreshObject(this);
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var step = child as Step;
            var copy = copyObject as Step;
            bool objectsNotNull = step != null && copy != null;
            if (objectsNotNull)
            {
                Step newStep = copy.Clone(GetStepN);
                if (!step.BaseObjectsList.Contains(newStep.GetBaseStepName()))
                {
                    bool editBaseStep = true;
                    newStep.SetNewValue(string.Empty, editBaseStep);
                }

                int index = steps.IndexOf(step);
                steps.Remove(step);
                steps.Insert(index, newStep);
                bool isModeStep = index == 0;
                if (isModeStep)
                {
                    modeStep = newStep;
                }

                newStep.Owner = this;
               
                index = steps.IndexOf(newStep);

                newStep.AddParent(this);
                return newStep;
            }

            return null;
        }

        public override ITreeViewItem InsertCopy(object copyObject)
        {
            var copy = copyObject as Step;
            if(copy != null)
            {
                var newStep = copy.Clone(GetStepN);
                steps.Add(newStep);
                newStep.AddParent(this);
                newStep.Owner = this;
                return newStep;
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

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem Insert()
        {
            if (MissingMainStep)
            {
                modeStep = AddNewStepToItems(MissingMainStep);
                modeStep.AddParent(this);

                return modeStep;
            }

            Step newStep = AddNewStepToItems();
            newStep.AddParent(this);

            newStep.ValueChanged += (sender) => OnValueChanged(sender);
            OnValueChanged(this);

            return newStep;
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
            foreach (Step step in steps)
            {
                List<DrawInfo> devToDrawTmp = step
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
        #endregion

        public override string SystemIdentifier => "state";

        public override void UpdateOnGenericTechObject(ITreeViewItem genericObject)
        {
            if (genericObject is null)
            {
                Steps.ForEach(step => step.UpdateOnGenericTechObject(null));
                return;
            }

            if (!(genericObject is State genericState))
                return;

            foreach (var stepIndex in Enumerable.Range(0, genericState.Steps.Count))
            {
                var genericStep = genericState.Steps.ElementAtOrDefault(stepIndex);

                var step = Steps.ElementAtOrDefault(stepIndex);
                var stepByBaseStep = Steps.Find(s => s.GetBaseStepLuaName() == genericStep.GetBaseStepLuaName() && genericStep.GetBaseStepLuaName() != "");
                var stepsByName = Steps.FindAll(s => s.GetStepName() == genericStep.GetStepName());

                if (step != null)
                {
                    if (stepsByName.Count == 1 && stepsByName[0] != null && stepsByName[0] != step)
                        SwapSteps(stepIndex, Steps.IndexOf(stepsByName[0]));

                    if (stepByBaseStep != null && step != stepByBaseStep)
                    {
                        step.SetNewValue("", true);
                        stepByBaseStep.SetNewValue("", true);
                    }

                    if (!string.IsNullOrEmpty(genericStep.GetBaseStepLuaName()))
                        step.SetNewValue(genericStep.GetBaseStepLuaName(), true);
                    step.SetNewValue(genericStep.GetStepName());
                }
                else step = AddStep(genericStep.GetStepName(), genericStep.GetBaseStepLuaName());

                step.UpdateOnGenericTechObject(genericStep);
            }
        }

        public override void CreateGenericByTechObjects(IEnumerable<ITreeViewItem> itemList)
        {
            var states = itemList.Cast<State>().ToList();

            var refState = states.OrderBy(state => state.Steps.Count).FirstOrDefault();
            if (refState is null)
                return;

            foreach (var stepIndex in Enumerable.Range(0, refState.steps.Count))
            {
                var refStep = refState.steps[stepIndex];
                foreach (var stateIndex in Enumerable.Range(0, states.Count))
                {
                    var step = states.ElementAtOrDefault(stateIndex)?.steps.ElementAtOrDefault(stepIndex);
                    if (step is null || step.GetBaseStepName() != refStep.GetBaseStepName())
                        return;
                }

                Step newGenericStep;

                if (stepIndex == 0 && MissingMainStep)
                    newGenericStep = Insert() as Step;
                else if (stepIndex == 0)
                    newGenericStep = Steps[0];
                else
                    newGenericStep = AddStep(refStep.GetStepName(), refStep.GetBaseStepLuaName());

                newGenericStep
                    .CreateGenericByTechObjects(states.Select(state => state.Steps[stepIndex]));
            }
        }

        public override void UpdateOnDeleteGeneric()
        {
            Steps.ForEach(step => step.UpdateOnDeleteGeneric());
        }

        public bool Empty
        {
            get
            {
                if(steps.Where(x => x.Empty == true).Count() == steps.Count)
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
        /// Отсутствует главный шаг (Во время операции)
        /// </summary>
        public bool MissingMainStep => modeStep is null || Items.Any() is false;

        /// <summary>
        /// Нужен главный шаг
        /// </summary>
        public bool NeedMainStep { get; private set; }

        /// <summary>
        /// Тип состояния
        /// </summary>
        public StateType Type { get; private set; }

        public enum StateType : int
        {
            IDLE = 0,   // Простой
            RUN,        // Выполнение
            PAUSE,      // Пауза
            STOP,       // Остановка       

            STARTING = 10,   // Запускается
            PAUSING,    // Становится в паузу
            UNPAUSING,  // Выходит из паузы
            STOPPING,   // Останавливается
        }

        public static readonly ReadOnlyDictionary<int, string> stateStr = new ReadOnlyDictionary<int, string>
            (new Dictionary<int, string>()
            {
                [0] = "Простой",
                [1] = "Выполнение",
                [2] = "Пауза",
                [3] = "Остановка",
                [10] = "Запускается",
                [11] = "Становится в паузу",
                [12] = "Выходит из паузы",
                [13] = "Останавливается",
            });

        private string name;        ///< Имя.
        private List<Step> steps;   ///< Список шагов.
        private Step modeStep;      ///< Шаг.
        private readonly Mode owner;///< Владелец элемента
    }
}
