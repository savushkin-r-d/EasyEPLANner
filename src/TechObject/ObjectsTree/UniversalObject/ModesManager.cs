using Editor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            clone.Modes.ForEach(mode => mode.ValueChanged += (sender) => clone.OnValueChanged(sender));

            return clone;
        }

        public void ModifyDevNames(IDevModifyOptions options)
        {
            modes.ForEach(mode => mode.ModifyDevNames(options));
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

            newMode.ValueChanged += (sender) => OnValueChanged(sender);
            OnValueChanged(this);

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

            newMode.ValueChanged += (sender) => OnValueChanged(sender);   
            OnValueChanged(this);

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

        public TechObject Owner => owner;

        public TechObject TechObject => Owner;

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
                        TechObjectManager.GetInstance()
                            .ChangeModeNum(owner, i + 1, -1);
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

        public void ChangeModeNum(TechObject techObject, int prev, int curr)
        {
            foreach (Mode mode in modes)
            {
                mode.ChangeModeNum(techObject, prev, curr);
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
                TechObjectManager.GetInstance().ChangeModeNum(owner, idx, -1);
                modes.Remove(mode);

                foreach (Mode newMode in modes)
                {
                    ChangeRestrictionModeOwner(newMode);
                }

                return true;
            }
            return false;
        }

        public override bool CanMoveUp(object child)
        {
            if(child is Mode mode)
            {
                return modes.FirstOrDefault() != mode;
            }

            return false;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var mode = child as Mode;
            if (mode is null)
                return null;

            int index = modes.IndexOf(mode);

            if (index <= 0)
                return null;

            SwapModes(index, index - 1);

            OnValueChanged(this);
            return child as ITreeViewItem;
        }

        public override bool CanMoveDown(object child)
        {
            if (child is Mode mode)
            {
                return modes.LastOrDefault() != mode;
            }

            return false;
        }

        override public ITreeViewItem MoveDown(object child)
        {
            var mode = child as Mode;
            if (mode is null)
                return null;

            
            int index = modes.IndexOf(mode);

            if (index > Modes.Count - 2)
                return null;
              
            SwapModes(index, index + 1);

            OnValueChanged(this);
            return child as ITreeViewItem;
        }

        private void SwapModes(int index1, int index2)
        {
            TechObjectManager.GetInstance().ChangeModeNum(owner, index1 + 1, index2 + 1);

            (modes[index1], modes[index2]) = (modes[index2], modes[index1]);
            modes.ForEach(ChangeRestrictionModeOwner);


            Editor.Editor.GetInstance().RefreshObject(this);
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            if (!(child is Mode targetOperation && copyObject is Mode copiedOperation))
                return null;

            Mode newOperation = copiedOperation.Clone(GetModeN, this, copiedOperation.Name);

            if (owner.BaseTechObject.BaseOperationsList.Count > 0 &&
                !Owner.BaseTechObject.BaseOperationsList.Contains(copiedOperation.BaseOperation.Name) ||
                !string.IsNullOrEmpty(copiedOperation.BaseOperation?.LuaName) &&
                modes.Except(new List<Mode>() { targetOperation }).ToList().Exists(o => o.BaseOperation.LuaName == copiedOperation.BaseOperation.LuaName))
            {
                // reset base object
                newOperation.SetNewValue(string.Empty, true);
            }

            int index = modes.IndexOf(targetOperation);
            modes.Remove(targetOperation);
            modes.Insert(index, newOperation);

            string modeTechObjName = Owner.NameEplan;
            int modeTechObjNumber = Owner.TechNumber;
            string copyObjTechObjName = copiedOperation.Owner.Owner.NameEplan;
            if (modeTechObjName == copyObjTechObjName)
            {
                newOperation.ModifyDevNames(new DevModifyOptions(Owner, Owner.NameEplan, -1));
            }
            else
            {
                newOperation.ModifyDevNames(new DevModifyOptions(Owner, copyObjTechObjName, Owner.TechNumber));
            }

            ChangeRestrictionModeOwner(newOperation);
            int newN = TechObjectManager.GetInstance()
                .GetTechObjectN(Owner);
            int oldN = TechObjectManager.GetInstance()
                .GetTechObjectN(copiedOperation.Owner.Owner);
            newOperation.ModifyRestrictObj(oldN, newN);

            newOperation.ChangeCrossRestriction(targetOperation);

            newOperation.AddParent(this);
            return newOperation;
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
            var copiedOperation = obj as Mode;

            if (copiedOperation is null)
                return null;

            Mode newOperation = copiedOperation.Clone(GetModeN, this);

            if (owner.BaseTechObject.BaseOperationsList.Count > 0 &&
                !Owner.BaseTechObject.BaseOperationsList.Contains(copiedOperation.BaseOperation.Name) ||
                !string.IsNullOrEmpty(copiedOperation.BaseOperation?.LuaName) &&
                modes.Exists(o => o.BaseOperation.LuaName == copiedOperation.BaseOperation.LuaName))
            {
                // reset base object
                newOperation.SetNewValue(string.Empty, true);    
            }

            modes.Add(newOperation);

            string techObjectName = copiedOperation.Owner.Owner.NameEplan;
            if (owner.NameEplan == techObjectName)
            {
                newOperation.ModifyDevNames(new DevModifyOptions(Owner, Owner.NameEplan, -1));
            }
            else
            {
                newOperation.ModifyDevNames(new DevModifyOptions(Owner, techObjectName, Owner.TechNumber));
            }

            ChangeRestrictionModeOwner(newOperation);
            newOperation.ChangeCrossRestriction();

            newOperation.AddParent(this);
            return newOperation;
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

            newMode.ValueChanged += (sender) => OnValueChanged(sender);
            OnValueChanged(this);

            return newMode;
        }

        public override bool NeedRebuildParent
        {
            get
            {
                return true;
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

        public override string SystemIdentifier => "operation";

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

        public void UpdateOnGenericTechObject(ModesManager genericModesManager)
        {
            if (genericModesManager is null)
            {
                modes.ForEach(mode => mode.UpdateOnGenericTechObject(null));
                return;
            }

            foreach (var index in Enumerable.Range(0, genericModesManager.modes.Count)) 
            {
                var genericMode = genericModesManager.modes.ElementAtOrDefault(index);
                var mode = modes.ElementAtOrDefault(index);
                var modeByBaseOperation = modes.Find(m => m.BaseOperation.LuaName == genericMode.BaseOperation.LuaName && genericMode.BaseOperation.Name != "");
                var modesByName = modes.FindAll(m => m.Name == genericMode.Name);
                
                // Перемещение операций
                if (modesByName.Count == 1 &&
                    mode != null &&
                    modesByName[0] != null &&
                    modesByName[0] != mode)
                {
                    SwapModes(index, modes.IndexOf(modesByName[0]));
                    mode = modes.ElementAtOrDefault(index);
                }

                // Если базовая операция наследуемой операции не совпадет с типовой, то сбрасываем
                if (modeByBaseOperation != null &&
                    mode != null &&
                    mode != modeByBaseOperation)
                {
                    mode.SetNewValue("", true);
                    modeByBaseOperation.SetNewValue("", true);
                }


                if (genericMode is null)
                    continue;

                if (mode is null)
                {
                    mode = AddMode(genericMode.Name, genericMode.BaseOperation.LuaName);
                    mode.AddParent(this);
                }
                else
                {
                    mode.SetNewValue(genericMode.BaseOperation.LuaName, true);
                    mode.SetNewValue(genericMode.Name);
                }

                mode.UpdateOnGenericTechObject(genericMode);
            }
        }

        public override void CreateGenericByTechObjects(IEnumerable<ITreeViewItem> itemList)
        {
            var modesManagers = itemList.Cast<ModesManager>().ToList();

            var refModesManagerModes = modesManagers.OrderBy(manager => manager.Modes.Count)
                .FirstOrDefault()?.modes ?? new List<Mode>();

            foreach (var modeIndex in Enumerable.Range(0, refModesManagerModes.Count))
            {
                var refMode = refModesManagerModes[modeIndex];
                foreach (var managerIndex in Enumerable.Range(0, modesManagers.Count)) 
                {
                    var mode = modesManagers.ElementAtOrDefault(managerIndex)?.modes.ElementAtOrDefault(modeIndex);
                    if (mode is null || mode.BaseOperation.LuaName != refMode.BaseOperation.LuaName)
                        return;
                }

                var newGenericMode = AddMode(refMode.Name, refMode.BaseOperation.Name);
                newGenericMode.CreateGenericByTechObjects(modesManagers.Select(manager => manager.modes[modeIndex]));
            }
        }

        public override void UpdateOnDeleteGeneric()
        {
            modes.ForEach(mode => mode.UpdateOnDeleteGeneric());
        }

        /// <summary> Список операций. </summary>
        private List<Mode> modes;
        /// <summary> Технологический объект. </summary>
        private TechObject owner;
    }
}
