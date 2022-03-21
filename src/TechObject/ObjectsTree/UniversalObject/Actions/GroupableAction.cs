using Editor;
using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    public abstract class GroupableAction : Action
    {
        public GroupableAction(string name, Step owner, string luaName)
            : base(name, owner, luaName)
        {
            subActions = new List<IAction>();
            treeViewItems = new List<ITreeViewItem>();
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

        public List<ITreeViewItem> TreeViewItems
        {
            get => treeViewItems;
            set => treeViewItems = value;
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

        override public ITreeViewItem[] Items
        {
            get
            {
                return SubActions.Cast<ITreeViewItem>()
                    .ToArray();
            }
        }

        override public DrawInfo.Style DrawStyle
        {
            get
            {
                return base.DrawStyle;
            }
            set
            {
                base.DrawStyle = value;
                if (SubActions != null)
                {
                    foreach (var subAction in SubActions)
                    {
                        subAction.DrawStyle = DrawStyle;
                    }
                }
            }
        }
        #endregion

        public override string ToString()
        {
            string res = string.Empty;
            foreach (IAction subAction in SubActions)
            {
                res += $"{{ {subAction } }} ";
            }

            return res;
        }

        private List<IAction> subActions;
        
        protected private const string GroupDefaultName = "Группа";
    }
}
