using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Операции технологического объекта.
    /// </summary>
    public class ModesManager : Editor.TreeViewItem
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

        public void SetNewOwnerDevNames(string newTechObjectName,
            int newTechObjectNumber)
        {
            foreach (Mode mode in modes)
            {
                mode.ModifyDevNames(newTechObjectName, newTechObjectNumber,
                    owner.NameEplan);
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

            res += prefix + "\t}\n";
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
                string comment = "\t\t--Операция №" + i.ToString();

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
            Editor.ObjectProperty[] extraParams)
        {
            Mode newMode = new Mode(modeName, GetModeN, this);

            // Установка имени базовой операции в Mode
            newMode.SetNewValue(baseOperationName, true);

            // Установка параметров базовой операции
            newMode.SetBaseOperExtraParams(extraParams);

            modes.Add(newMode);

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

        /// <summary>
        /// Изменение технологического объекта-владельца операций.
        /// </summary>
        /// <param name="newOwner">Новый технологический объект-владелец 
        /// операций.</param>
        /// <returns></returns>
        public void ChngeOwner(TechObject newOwner)
        {
            owner = newOwner;
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

        public void Synch(int[] array)
        {
            foreach (Mode mode in modes)
            {
                mode.Synch(array);
            }
        }

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

        override public Editor.ITreeViewItem[] Items
        {
            get
            {
                return modes.ToArray();
            }
        }

        override public bool Delete(object child)
        {
            Mode mode = child as Mode;

            if (mode != null)
            {
                int idx = modes.IndexOf(mode) + 1;

                int tobjNum = TechObjectManager.GetInstance()
                    .GetTechObjectN(owner);
                TechObjectManager.GetInstance()
                    .ChangeModeNum(tobjNum, idx, -1);

                modes.Remove(mode);

                foreach (Mode newMode in modes)
                {
                    ChangeRestrictionModeOwner(newMode);
                }
                return true;
            }

            return false;
        }

        override public Editor.ITreeViewItem MoveUp(object child)
        {
            Mode mode = child as Mode;

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

                    return modes[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem MoveDown(object child)
        {
            Mode mode = child as Mode;

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

                    return modes[index];
                }
            }

            return null;
        }

        override public Editor.ITreeViewItem Replace(object child, 
            object copyObject)
        {
            Mode mode = child as Mode;
            if (copyObject is Mode && mode != null)
            {

                Mode newMode = (copyObject as Mode).Clone(GetModeN, this, 
                    mode.EditText[0]);
                int index = modes.IndexOf(mode);

                modes.Remove(mode);

                modes.Insert(index, newMode);

                newMode.ModifyDevNames(owner.TechNumber, -1, owner.NameEplan);

                ChangeRestrictionModeOwner(newMode);
                newMode.ChangeCrossRestriction(mode);

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

        override public Editor.ITreeViewItem InsertCopy(object obj)
        {
            if (obj is Mode)
            {
                Mode newMode = (obj as Mode).Clone(GetModeN, this);
                modes.Add(newMode);

                newMode.ModifyDevNames(owner.TechNumber, -1, owner.NameEplan);

                ChangeRestrictionModeOwner(newMode);
                newMode.ChangeCrossRestriction();

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

        override public Editor.ITreeViewItem Insert()
        {
            Mode newMode = new Mode("Новая операция", GetModeN, this);
            modes.Add(newMode);

            ChangeRestrictionModeOwner(newMode);

            return newMode;
        }
        #endregion

        private List<Mode> modes; /// Список операций.
        private TechObject owner; /// Технологический объект.
    }
}
