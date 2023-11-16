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

        public override void UpdateOnGenericTechObject(IAction genericAction)
        {
            if (genericAction is null)
                return;

            var genericGroupbaleAction = genericAction as GroupableAction;
            if (genericGroupbaleAction is null)
                return;

            foreach (var subActionIndex in Enumerable.Range(0, genericGroupbaleAction.SubActions.Count))
            {
                var genericSubAction = genericGroupbaleAction.SubActions.ElementAtOrDefault(subActionIndex);
                var subAction = SubActions.ElementAtOrDefault(subActionIndex);
                if (subAction is null)
                {
                    subAction = Insert() as IAction;
                }
                
                subAction.UpdateOnGenericTechObject(genericSubAction);
            }

            foreach (var parameterIndex in Enumerable.Range(0, genericGroupbaleAction.Parameters.Count))
            {
                var genericParameter = genericGroupbaleAction.Parameters.ElementAtOrDefault(parameterIndex);

                if (genericParameter.IsFilled)
                {
                    parameters.ElementAtOrDefault(parameterIndex)
                        ?.SetNewValue(genericParameter.Value);
                }
            }
        }

        public override void CreateGenericByTechObjects(IEnumerable<IAction> actions)
        {
            var refAction = actions.OrderBy(a => a.SubActions.Count).First();

            foreach (var subActionIndex in Enumerable.Range(0, refAction.SubActions.Count))
            {
                var subaction = SubActions.ElementAtOrDefault(subActionIndex) ?? Insert() as IAction;
                subaction.CreateGenericByTechObjects(actions.Select(sa => sa.SubActions.ElementAtOrDefault(subActionIndex)));
            }

            if (parameters is null) return;

            foreach (var parIndex in Enumerable.Range(0, parameters.Count))
            {
                var par = parameters.ElementAtOrDefault(parIndex);
                var refPar = (refAction as GroupableAction)?.parameters.ElementAtOrDefault(parIndex);

                if (par != null && refPar != null 
                    && actions.All(a => (a as GroupableAction)?.parameters
                                        .ElementAtOrDefault(parIndex)?.Value == refPar.Value))
                    par.SetNewValue(refPar.Value);
            }
        }

        public override void UpdateOnDeleteGeneric()
        {
            SubActions.ForEach(sa => sa.UpdateOnDeleteGeneric());
        }

        public override bool Empty => subActions.TrueForAll(subAction => subAction.Empty) && (parameters?.TrueForAll(parameter => !parameter.IsFilled) ?? true);

        public List<BaseParameter> Parameters => parameters;

        protected List<IAction> subActions;

        protected List<BaseParameter> parameters;

        protected private const string GroupDefaultName = "Группа";
    }
}
