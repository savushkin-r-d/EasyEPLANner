using Editor;
using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Операции технологического объекта.
    /// </summary>
    public class ModesManager : TreeViewItem
    {
        public ModesManager(TechObject owner)
        {
            this.owner = owner;

            modes = new List<Mode>();
        }

        public ModesManager Clone(TechObject owner)
        {
            ModesManager clone = (ModesManager)MemberwiseClone();

            clone.modes = new List<Mode>();
            foreach (Mode mode in modes)
            {
                clone.modes.Add(mode.Clone(clone.GetModeN, clone));
            }

            clone.owner = owner;

            return clone;
        }

        public void ModifyDevNames(int oldTechObjectN)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyDevNames(owner.TechNumber, oldTechObjectN,
                    owner.NameEplan);
            }
        }

        public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    owner.NameEplan, owner.TechNumber);
            }
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = prefix + "modes =\n" +
                prefix + "\t{\n";

            int i = 1;
            foreach (Mode mode in modes)
            {
                res += prefix + "\t[ " + i++ + " ] =\n";
                res += mode.SaveAsLuaTable(prefix + "\t\t");
            }

            res += prefix + "\t},\n";
            return res;
        }

        /// <summary>
        /// Сохранение ограничений в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveRestrictionAsLua(string prefix)
        {
            string res = "";
            string tmp2 = "";
            int i = 1;
            foreach (Mode mode in modes)
            {
                string tmp = "";
                string comment = $"\t\t--{mode.Name}";

                tmp += mode.SaveRestrictionAsLua(prefix + "\t\t");
                if (tmp != "")
                {
                    tmp2 += prefix + "\t[ " + i.ToString() + " ] =" +
                        comment + "\n" + tmp;
                }
                i++;
            }
            if (tmp2 != "")
            {
                res += prefix + "\t{\n" + tmp2 + prefix + "\t},";
            }

            return res;
        }

        /// <summary>
        /// Получение номера операции в списке операций. 
        /// Нумерация начинается с 1.
        /// </summary>
        /// <param name="mode">Операция, номер которой хотим получить.</param>
        /// <returns>Номер заданной операции.</returns>
        public int GetModeN(object mode)
        {
            return modes.IndexOf(mode as Mode) + 1;
        }

        /// <summary>
        /// Добавление новой операции.
        /// </summary>
        /// <param name="modeName">Имя операции.</param>
        /// <param name="baseOperationName">Имя базовой операции</param>
        /// <returns>Добавленная операция.</returns>
        public Mode AddMode(string modeName, string baseOperationName)
        {
            Mode newMode = new Mode(modeName, GetModeN, this);

            // Установка имени базовой операции в Mode
            newMode.SetNewValue(baseOperationName, true);

            modes.Add(newMode);

            ChangeRestrictionModeOwner(newMode);

            return newMode;
        }

        /// <summary>
        /// Добавление новой операции.
        /// </summary>
        /// <param name="modeName">Имя операции.</param>
        /// <param name="baseOperationName">Имя базовой операции</param>
        /// <param name="extraParams">Значения параметров базовой операции
        /// </param>
        /// <returns>Добавленная операция.</returns>
        public Mode AddMode(string modeName, string baseOperationName,
            ObjectProperty[] extraParams)
        {
            Mode newMode = new Mode(modeName, GetModeN, this);
            modes.Add(newMode);

            // Установка имени базовой операции в Mode
            newMode.SetNewValue(baseOperationName, true);

            // Установка параметров базовой операции
            newMode.SetBaseOperExtraParams(extraParams);

            newMode.BaseOperation.Check();

            ChangeRestrictionModeOwner(newMode);
            return newMode;
        }

        /// <summary>
        /// Добавление нового шага.
        /// </summary>
        /// <param name="modeN">Номер операции, куда добавляем шаг.</param>
        /// <param name="stateN">Номер состояния, куда добавляем шаг.</param>
        /// <param name="stepName">Имя шага.</param>
        /// <returns>true  - шаг добавлен.</returns>
        /// <returns>false - шаг не добавлен.</returns>
        public bool AddStep(int modeN, int stateN, string stepName)
        {
            if (modes.Count > modeN)
            {
                modes[modeN].AddStep(stateN, stepName);
                return true;
            }

            return false;
        }

        public TechObject Owner
        {
            get
            {
                return owner;
            }
        }

        public List<Mode> Modes
        {
            get
            {
                return modes;
            }
        }

        #region Синхронизация устройств в объекте.
        public void Synch(int[] array)
        {
            foreach (Mode mode in modes)
            {
                mode.Synch(array);
            }
        }
        #endregion

        public void CheckRestriction(int prev, int curr)
        {
            foreach (Mode mode in modes)
            {
                mode.CheckRestriction(prev, curr);
            }
        }

        public void ChangeCrossRestriction(ModesManager oldModesMngr = null)
        {
            for (int i = 0; i < modes.Count; i++)
            {
                if ((oldModesMngr != null) &&
                    (i < oldModesMngr.Modes.Count))
                {
                    modes[i].ChangeCrossRestriction(oldModesMngr.Modes[i]);
                }
                else
                {
                    modes[i].ChangeCrossRestriction();
                }
            }
            if (oldModesMngr != null)
            {
                if (oldModesMngr.Modes.Count > modes.Count)
                {
                    for (int i = modes.Count; i < oldModesMngr.Modes.Count;
                        i++)
                    {
                        int tobjNum = TechObjectManager.GetInstance()
                            .GetTechObjectN(owner);
                        TechObjectManager.GetInstance()
                            .ChangeModeNum(tobjNum, i + 1, -1);
                    }
                }
            }
        }

        public void ModifyRestrictObj(int oldObjN, int newObjN)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyRestrictObj(oldObjN, newObjN);

            }
        }

        public void ChangeModeNum(int objNum, int prev, int curr)
        {
            foreach (Mode mode in modes)
            {
                mode.ChangeModeNum(objNum, prev, curr);
            }
        }

        private void ChangeRestrictionModeOwner(Mode newMode)
        {
            int objParentNum = TechObjectManager.GetInstance()
                .GetTechObjectN(owner);
            int modeParentNum = GetModeN(newMode);
            newMode.SetRestrictionOwner(objParentNum, modeParentNum);
        }

        /// <summary>
        /// При перемщении, удалении объекта нужно менять родителей у 
        /// ограничений
        /// </summary>
        public void SetRestrictionOwner()
        {
            foreach (Mode mode in modes)
            {
                ChangeRestrictionModeOwner(mode);
            }
        }

        /// <summary>
        /// Очистить базовые операции
        /// </summary>
        public void ClearBaseOperations()
        {
            foreach (Mode m in Modes)
            {
                m.ClearBaseOperation();
            }
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                string res = "Операции";
                if (modes.Count > 0)
                {
                    res += " (" + modes.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return modes.ToArray();
            }
        }

        override public bool Delete(object child)
        {
            var mode = child as Mode;
            if (mode != null)
            {
                int idx = modes.IndexOf(mode) + 1;
                int tobjNum = TechObjectManager.GetInstance()
                    .GetTechObjectN(owner);
                TechObjectManager.GetInstance().ChangeModeNum(tobjNum, idx, -1);
                modes.Remove(mode);

                foreach (Mode newMode in modes)
                {
                    ChangeRestrictionModeOwner(newMode);
                }

                return true;
            }
            return false;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var mode = child as Mode;
            if (mode != null)
            {
                int index = modes.IndexOf(mode);
                if (index > 0)
                {

                    int tobjNum = TechObjectManager.GetInstance()
                        .GetTechObjectN(owner);
                    TechObjectManager.GetInstance()
                        .ChangeModeNum(tobjNum, index + 1, index);

                    modes.Remove(mode);
                    modes.Insert(index - 1, mode);

                    foreach (Mode newMode in modes)
                    {
                        ChangeRestrictionModeOwner(newMode);
                    }

                    modes[index].AddParent(this);
                    return modes[index];
                }
            }

            return null;
        }

        override public ITreeViewItem MoveDown(object child)
        {
            var mode = child as Mode;
            if (mode != null)
            {
                int index = modes.IndexOf(mode);
                if (index <= modes.Count - 2)
                {

                    int tobjNum = TechObjectManager.GetInstance()
                        .GetTechObjectN(owner);
                    TechObjectManager.GetInstance()
                        .ChangeModeNum(tobjNum, index + 1, index + 2);

                    modes.Remove(mode);
                    modes.Insert(index + 1, mode);

                    foreach (Mode newMode in modes)
                    {
                        ChangeRestrictionModeOwner(newMode);
                    }

                    modes[index].AddParent(this);
                    return modes[index];
                }
            }

            return null;
        }

        override public ITreeViewItem Replace(object child,
            object copyObject)
        {
            var mode = child as Mode;
            var copyMode = copyObject as Mode;
            bool objectsNotNull = mode != null && copyMode != null;
            if (objectsNotNull)
            {
                Mode newMode = copyMode.Clone(GetModeN, this, copyMode.Name);
                if (!mode.BaseObjectsList.Contains(copyMode.BaseOperation.Name))
                {
                    bool editBaseOperation = true;
                    newMode.SetNewValue(string.Empty, editBaseOperation);
                }
                
                int index = modes.IndexOf(mode);
                modes.Remove(mode);
                modes.Insert(index, newMode);

                string modeTechObjName = Owner.NameEplan;
                int modeTechObjNumber = Owner.TechNumber;
                string copyObjTechObjName = copyMode.Owner.Owner.NameEplan;
                int copyObjTechObjNumber = copyMode.Owner.Owner.TechNumber;
                if (modeTechObjName == copyObjTechObjName)
                {
                    newMode.ModifyDevNames(owner.TechNumber, -1,
                        owner.NameEplan);
                }
                else
                {
                    newMode.ModifyDevNames(modeTechObjName, modeTechObjNumber,
                        copyObjTechObjName, copyObjTechObjNumber);
                }

                ChangeRestrictionModeOwner(newMode);
                int newN = TechObjectManager.GetInstance()
                    .GetTechObjectN(Owner);
                int oldN = TechObjectManager.GetInstance()
                    .GetTechObjectN(copyMode.Owner.Owner);
                newMode.ModifyRestrictObj(oldN, newN);

                newMode.ChangeCrossRestriction(mode);

                newMode.AddParent(this);
                return newMode;
            }

            return null;
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem InsertCopy(object obj)
        {
            var objMode = obj as Mode;
            if (objMode != null)
            {
                Mode newMode = objMode.Clone(GetModeN, this);
                modes.Add(newMode);

                string objModeTechObjectName = objMode.Owner.Owner.NameEplan;
                int objMobeTechObjectNumber = objMode.Owner.Owner.TechNumber;
                if (owner.NameEplan == objModeTechObjectName)
                {
                    newMode.ModifyDevNames(owner.TechNumber, -1,
                        owner.NameEplan);
                }
                else
                {
                    newMode.ModifyDevNames(owner.NameEplan, owner.TechNumber,
                        objModeTechObjectName, objMobeTechObjectNumber);
                }

                ChangeRestrictionModeOwner(newMode);
                newMode.ChangeCrossRestriction();

                newMode.AddParent(this);
                return newMode;
            }

            return null;
        }

        override public bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        override public ITreeViewItem Insert()
        {
            Mode newMode = new Mode(Mode.DefaultModeName, GetModeN, this);
            modes.Add(newMode);

            ChangeRestrictionModeOwner(newMode);

            newMode.AddParent(this);
            return newMode;
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
                if (modes.Where(x => x.IsFilled).Count() > 0)
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
                return ImageIndexEnum.ModesManager;
            }
        }
        #endregion

        public override string GetLinkToHelpPage()
        {
            string ostisLink = EasyEPlanner.ProjectManager.GetInstance()
                .GetOstisHelpSystemLink();
            return ostisLink + "?sys_id=operation";
        }

        public void SetUpFromBaseTechObject(BaseTechObject baseTechObject)
        {
            var baseOperations = baseTechObject.BaseOperations;
            if (baseOperations.Count == 0)
            {
                return;
            }

            var filteredOperations = baseOperations
                .Where(x => x.Name != string.Empty &&
                x.DefaultPosition > 0)
                .OrderBy(x => x.DefaultPosition);

            foreach (var baseOperation in filteredOperations)
            {
                while(modes.Count < baseOperation.DefaultPosition - 1)
                {
                    Insert();
                }

                Mode insertedMode = (Mode)Insert();
                insertedMode.SetUpFromBaseTechObject(baseOperation);
            }
        }

        private List<Mode> modes; /// Список операций.
        private TechObject owner; /// Технологический объект.
    }
}
