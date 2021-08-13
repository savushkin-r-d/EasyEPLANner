using System.Collections.Generic;

namespace TechObject
{
    public abstract class GroupableAction : Action
    {
        public GroupableAction(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subActions = new List<IAction>();
        }

        override public void ModifyDevNames(int newTechObjectN,
            int oldTechObjectN, string techObjectName)
        {
            foreach (IAction subAction in subActions)
            {
                subAction.ModifyDevNames(newTechObjectN, oldTechObjectN,
                    techObjectName);
            }
        }

        override public void ModifyDevNames(string newTechObjectName,
            int newTechObjectNumber, string oldTechObjectName,
            int oldTechObjectNumber)
        {
            foreach (IAction subAction in subActions)
            {
                subAction.ModifyDevNames(newTechObjectName,
                    newTechObjectNumber, oldTechObjectName,
                    oldTechObjectNumber);
            }
        }

        #region Синхронизация устройств в объекте.
        /// <summary>
        /// Синхронизация индексов устройств.
        /// </summary>
        /// <param name="array">Массив флагов, определяющих изменение индексов.
        /// </param>
        override public void Synch(int[] array)
        {
            base.Synch(array);
            foreach (IAction subAction in subActions)
            {
                subAction.Synch(array);
            }
        }
        #endregion

        public override bool HasSubActions
        {
            get => true;
        }

        public override List<IAction> SubActions
        {
            get => subActions;
            set => subActions = value;
        }

        #region реализация ITreeViewItem
        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        override public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        override public void Clear()
        {
            foreach (IAction subAction in SubActions)
            {
                subAction.Clear();
            }
        }

        override public string[] DisplayText
        {
            get
            {
                return new string[] { name, ToString() };
            }
        }

        public override string ToString()
        {
            string res = string.Empty;
            foreach (IAction subAction in SubActions)
            {
                res += $"{{ {subAction } }} ";
            }

            return res;
        }
        #endregion

        private List<IAction> subActions;
    }
}
