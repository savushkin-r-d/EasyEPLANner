using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechObject
{
    /// <summary>
    /// Шаг технологического объекта. Состоит из параллельно выполняемых 
    /// действий.
    /// </summary>
    public class Step : Editor.TreeViewItem
    {
        /// <summary>
        /// Создание нового шага.
        /// </summary>
        /// <param name="name">Имя шага.</param>
        /// <param name="getN">Функция получения номера шага.</param>
        /// <param name="isMode">Признак того, является ли шаг шагом операции.
        /// </param>
        /// <param name="owner">Владелец шага (Состояние)</param>
        public Step(string name, GetN getN, State owner, bool isMode = false)
        {
            this.name = name;
            this.getN = getN;
            this.IsMode = isMode;
            this.owner = owner;
            this.baseStep = new ActiveParameter("", "");
            this.baseStep.Owner = this;

            items = new List<Editor.ITreeViewItem>();

            actions = new List<Action>();
            actions.Add(new Action("Включать", this, 
                "opened_devices",
                new Device.DeviceType[3] { 
                    Device.DeviceType.V, 
                    Device.DeviceType.DO, 
                    Device.DeviceType.M }));

            actions.Add(new Action("Включать реверс", this, 
                "opened_reverse_devices",
                new Device.DeviceType[1] { 
                    Device.DeviceType.M }));

            actions.Add(new Action("Выключать", this, 
                "closed_devices",
                new Device.DeviceType[3] { 
                    Device.DeviceType.V, 
                    Device.DeviceType.DO, 
                    Device.DeviceType.M }));

            actions[2].DrawStyle = Editor.DrawInfo.Style.RED_BOX;
            actions.Add(new Action_WashSeats("Верхние седла", this,
                "opened_upper_seat_v"));

            actions[3].DrawStyle = Editor.DrawInfo.Style.GREEN_UPPER_BOX;
            actions.Add(new Action_WashSeats("Нижние седла", this,
                "opened_lower_seat_v"));

            actions[4].DrawStyle = Editor.DrawInfo.Style.GREEN_LOWER_BOX;
            actions.Add(new Action("Сигналы для включения", this,
                "required_FB",
                new Device.DeviceType[2] {
                    Device.DeviceType.DI,
                    Device.DeviceType.GS }));

            actions.Add(new Action_Wash("Мойка( DI, DO, устройства)", this,
                "wash_data"));

            actions.Add(new Action_DI_DO("Группы DI -> DO DO ...", this,
                "DI_DO"));

            actions.Add(new Action_AI_AO("Группы AI -> AO AO ...", this,
                "AI_AO"));

            items.AddRange(actions.ToArray());

            if (!isMode)
            {
                timeParam = new Editor.ObjectProperty("Время (параметр)", -1);
                nextStepN = new Editor.ObjectProperty(
                    "Номер следующего шага", -1);

                items.Add(timeParam);
                items.Add(nextStepN);
            }

        }

        public Step Clone(GetN getN, string name = "")
        {
            Step clone = (Step)MemberwiseClone();
            clone.getN = getN;

            if (name != "")
            {
                clone.name = name.Substring(3);
            }

            clone.actions = new List<Action>();
            foreach (Action action in actions)
            {
                clone.actions.Add(action.Clone());
            }

            clone.items = new List<Editor.ITreeViewItem>();
            clone.items.AddRange(clone.actions.ToArray());

            if (!IsMode)
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
            string res = "";

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
                if (time_param_n != "")
                {
                    res += prefix + "time_param_n = " + time_param_n + ",\n";
                }
                
                string next_step_n = nextStepN.EditText[1].Trim();
                if (next_step_n != "")
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
        /// <param name="additionalParam">Дополнительный параметр 
        /// (для сложных действий).</param>
        public bool AddDev(string actionLuaName, string devName, 
            int additionalParam = 0)
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
                    act.AddDev(index, additionalParam);
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
        /// <param name="parIdx">Индекс параметра.</param>
        /// <param name="val">Значение параметра.</param>
        public bool AddParam(string actionLuaName, int parIdx, int val)
        {
            foreach (Action act in actions)
            {
                if (act.LuaName == actionLuaName)
                {
                    act.AddParam(parIdx, val);
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

        public void Synch(int[] array)
        {
            foreach (Action action in actions)
            {
                action.Synch(array);
            }
        }

        public string GetStepName()
        {
            return this.name;
        }

        public int GetStepNumber()
        {
            return getN(this);
        }

        public State Owner
        {
            get
            {
                return owner;
            }
        }

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
                    return new string[] { name, "" };
                }

                return new string[] { getN(this) + ". " + name, baseStep.Name };
            }
        }

        override public Editor.ITreeViewItem[] Items
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
            State state = this.Owner;

            Step equalStep = state.Steps
                .Where(x => x.GetBaseStepName() == newVal)
                .FirstOrDefault();
            if (equalStep == null)
            {
                equalStep = state.Steps
                    .Where(x => x.GetBaseStepLuaName() == newVal)
                    .FirstOrDefault();
            }

            if (equalStep != null && newVal != "")
            {
                return false;
            }

            Mode mode = state.Owner;
            BaseParameter baseStep = mode.BaseOperation.Steps
                .Where(x => x.LuaName == newVal).FirstOrDefault();
            if (baseStep == null)
            {
                baseStep = mode.BaseOperation.Steps
                    .Where(x => x.Name == newVal).FirstOrDefault();
            }

            if (baseStep != null)
            {
                this.baseStep = new ActiveParameter(baseStep.LuaName, 
                    baseStep.Name);
                this.baseStep.Owner = this;
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
                //Можем редактировать содержимое обоих колонок.
                return new int[] { 0, 1 };
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
            Action action = child as Action;

            if (action != null)
            {
                action.Clear();
            }

            return false;
        }

        override public bool IsDrawOnEplanPage
        {
            get { return true; }
        }

        override public List<Editor.DrawInfo> GetObjectToDrawOnEplanPage()
        {
            List<Editor.DrawInfo> devToDraw = new List<Editor.DrawInfo>();
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
                State state = this.Owner;
                if (state.IsMain)
                {
                    Mode mode = state.Owner;
                    var stepsNames = mode.BaseOperation.Steps
                        .Select(x => x.Name).ToList();
                    return stepsNames;
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        public override bool ContainsBaseObject
        {
            get
            {
                return true;
            }
        }
        #endregion

        /// <summary>
        /// Проверка действий в шаге
        /// </summary>
        /// <returns>Строка с ошибками</returns>
        public string Check()
        {
            var errors = string.Empty;
            List<int> devicesInAction = new List<int>();
            foreach (Action a in actions)
            {
                if (a.GetType().Name == "Action" &&
                    (a.DisplayText[0].Contains("Включать") ||
                    a.DisplayText[0].Contains("Выключать")))
                {
                    devicesInAction.AddRange(a.DeviceIndex);
                }
            }

            List<int> FindEqual = devicesInAction.GroupBy(x => x)
                .SelectMany(y => y.Skip(1)).Distinct().ToList();

            foreach (int i in FindEqual)
            {
                State state = Owner;
                Mode mode = state.Owner;
                ModesManager modesManager = mode.Owner;
                TechObject techObject = modesManager.Owner;
                Device.IODevice device = Device.DeviceManager.GetInstance()
                    .GetDeviceByIndex(i);
                string msg = $"Неправильно заданы устройства в шаге " +
                    $"\"{GetStepName()}\", операции \"{mode.Name}\"," +
                    $"технологического объекта " +
                    $"\"{techObject.DisplayText[0]}\"\n";
                errors += msg;
            }

            return errors;
        }

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.Properties.Resources.ResourceManager
                .GetString("ostisLink");
            return ostisLink + "?sys_id=phase";
        }

        /// <summary>
        /// Признак шага операции.
        /// </summary>
        private bool IsMode { get; set; }

        private GetN getN;

        private Editor.ObjectProperty nextStepN; ///< Номер следующего шага.
        private Editor.ObjectProperty timeParam; ///< Параметр времени.
        private List<Editor.ITreeViewItem> items;

        private string name;           ///< Имя шага.
        internal List<Action> actions; ///< Список действий шага.
        private State owner;           ///< Владелей элемента

        private BaseParameter baseStep;
    }
}
