using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Состояние операции. Содержит группу шагов, выполняемых последовательно
    /// (или в ином порядке).
    /// </summary>
    public class State : Editor.TreeViewItem
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
                if (modeStep == null) //Добавляем, если его нет.
                {
                    modeStep = new Step("Во время операции", this.GetStepN, 
                        this, isMain);
                    steps.Add(modeStep);
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
        /// <param name="name">Имя состояния.</param>
        /// <param name="isMain">Надо ли дополнительные действия.</param>
        /// <param name="needModeStep">Надо ли основной шаг.</param>
        /// <param name="owner">Владелец состояния (Операция)</param>
        public State(string name, bool isMain, Mode owner, 
            bool needMainStep = false)
        {
            this.name = name;
            this.isMain = isMain;
            this.owner = owner;
            steps = new List<Step>();

            if (needMainStep)
            {
                modeStep = new Step("Во время операции", this.GetStepN, this, 
                    true);
                steps.Add(modeStep);
            }
        }

        public State Clone(string name = "")
        {
            State clone = (State)MemberwiseClone();

            if (name != "")
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

            return clone;
        }

        public void ModifyDevNames(int newTechObjectN, int oldTechObjectN,
            string techObjectName)
        {
            foreach (Step step in steps)
            {
                step.ModifyDevNames(newTechObjectN, oldTechObjectN,
                    techObjectName);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName, 
            int oldTechObjectNumber)
        {
            foreach (Step step in steps)
            {
                step.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    oldTechObjectName, oldTechObjectNumber);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            if (steps.Count == 0) return "";

            string res = "";

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

            if (res != "")
            {
                res = prefix + "--\'" + name + "\'\n" + res;
            }

            return res;
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="stepName">Имя шага.</param>
        /// <param name="baseStepLuaName">Имя базового шага</param>
        public void AddStep(string stepName, string baseStepLuaName)
        {
            Step newStep = new Step(stepName, GetStepN, this);
            newStep.SetNewValue(baseStepLuaName, true);

            if (modeStep == null)
            {
                modeStep = new Step("Во время операции", GetStepN, this, 
                    isMain);
                steps.Add(modeStep);
            }

            steps.Add(newStep);
        }

        public List<Step> Steps
        {
            get
            {
                return steps;
            }
        }

        public void Synch(int[] array)
        {
            foreach (Step step in steps)
            {
                step.Synch(array);
            }
        }

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
        /// Является ли состояние главным (Выполнение).
        /// </summary>
        public bool IsMain
        {
            get
            {
                return isMain;
            }
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

                return new string[] { res, "" };
            }
        }

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return steps.ToArray();
            }
        }

        override public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return false;
            }
        }

        override public bool IsMoveable
        {
            get
            {
                return false;
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
            Step step = child as Step;

            if (step != null)
            {
                const string ignoreStateName = "Выполнение";
                if (steps.IndexOf(step) == 0 &&
                    steps[0].Owner.name == ignoreStateName)
                {
                    //Не удаляем шаг операции.
                    return false;
                }

                steps.Remove(step);
                return true;
            }

            Action action = child as Action;
            if (action != null)
            {
                action.Clear();
                return false;
            }

            return false;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            Step step = child as Step;

            if (step != null)
            {
                int index = steps.IndexOf(step);
                if (index > 1)
                {
                    steps.Remove(step);
                    steps.Insert(index - 1, step);
                    return steps[index];
                }

            }

            return null;
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            Step step = child as Step;

            if (step != null)
            {
                int index = steps.IndexOf(step);
                if (index <= steps.Count - 2 && index > 0)
                {
                    steps.Remove(step);
                    steps.Insert(index + 1, step);
                    return steps[index];
                }

            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child,
            object copyObject)
        {
            Step step = child as Step;
            if (copyObject is Step && step != null)
            {
                Step newStep = (copyObject as Step).Clone(GetStepN);
                int index = steps.IndexOf(step);
                steps.Remove(step);

                steps.Insert(index, newStep);

                index = steps.IndexOf(newStep);

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

        override public object Copy()
        {
            return this;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public Editor.ITreeViewItem Insert()
        {
            if (modeStep == null)
            {
                modeStep = new Step("Во время операции", GetStepN, this, 
                    isMain);
                steps.Add(modeStep);
                return modeStep;
            }
            else if (this.Items.Count() == 0)
            {
                modeStep = new Step("Во время операции", GetStepN, this,
                    isMain);
                steps.Add(modeStep);
                return modeStep;
            }

            Step newStep = new Step("Новый шаг", GetStepN, this);
            steps.Add(newStep);

            return newStep;
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
            foreach (Step step in steps)
            {
                List<Editor.DrawInfo> devToDrawTmp = step
                    .GetObjectToDrawOnEplanPage();
                foreach (Editor.DrawInfo dinfo in devToDrawTmp)
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
                                devToDraw.Add(new Editor.DrawInfo(Editor
                                    .DrawInfo.Style.GREEN_RED_BOX,
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

        public override bool IsFilled
        {
            get
            {
                if(Empty)
                {
                    return false;
                }
                else 
                {
                    return true;
                }
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=state";
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

        private string name;        ///< Имя.
        internal List<Step> steps;  ///< Список шагов.
        private Step modeStep;      ///< Шаг.
        bool isMain;                ///< Надо ли дополнительные действия.
        private Mode owner;         ///< Владелец элемента
    }
}
