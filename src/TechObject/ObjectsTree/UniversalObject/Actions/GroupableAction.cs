﻿using Editor;
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

        public override void ModifyDevNames(IDevModifyOptions options)
        {
            Parameters?.OfType<ActionParameter>().ToList()
                .ForEach(p => p.ModifyDevNames(options));

            subActions.ForEach(sa => sa.ModifyDevNames(options));
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
            SubActions?.ForEach(sa => sa.Synch(array));
            parameters?.ForEach(p => p.Synch(array));
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

        override public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            return [.. SubActions.SelectMany(sa => sa.GetObjectToDrawOnEplanPage()).Distinct()];
        }

        private bool IdenticalActions(GroupableAction first, GroupableAction second)
        {
            return first != null && second != null
                && Enumerable.SequenceEqual(
                    first.SubActions.Select(a => (a.Name, a.LuaName)),
                    second.SubActions.Select(a => (a.Name, a.LuaName)))
                && Enumerable.SequenceEqual(
                    first.Parameters.Select(p => (p.Name, p.LuaName)),
                    second.Parameters.Select(p => (p.Name, p.LuaName)));
        }

        public override ITreeViewItem InsertCopy(object obj)
        {
            if (obj is GroupableAction copiedAction)
            {
                if (Parent is Step step)
                {
                    if (IdenticalActions(subActions.FirstOrDefault() as GroupableAction, copiedAction))
                    {
                        return Insert().InsertCopy(obj);
                    }

                    return step.Replace(this, obj);
                }

                if (IdenticalActions(this, copiedAction))
                {
                    foreach (var subActionIndex in Enumerable.Range(0, SubActions.Count))
                        (SubActions[subActionIndex] as Action)?.InsertCopy(copiedAction.SubActions[subActionIndex]);
                    foreach (var parameterIndex in Enumerable.Range(0, Parameters.Count))
                        Parameters[parameterIndex].SetNewValue(copiedAction.Parameters[parameterIndex].Value);
                    return this;
                }
            }

            return null;
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


        public override bool IsMoveable => true;

        public override bool CanMoveUp(object child)
        {
            if (child is not IAction action)
                return false;

            return subActions.FirstOrDefault() != action;
        }

        public override ITreeViewItem MoveUp(object child)
        {
            if (child is not IAction action)
                return null;


            int index = subActions.IndexOf(action);

            if (index <= 0)
                return null;

            SwapSubActions(index, index - 1);

            OnValueChanged(this);
            return child as ITreeViewItem;
        }

        public override bool CanMoveDown(object child)
        {
            if (child is not IAction action)
                return false;

            return subActions.LastOrDefault() != action;
        }

        public override ITreeViewItem MoveDown(object child)
        {
            if (child is not IAction action)
                return null;

            int index = subActions.IndexOf(action);

            if (index > subActions.Count - 2)
                return null;

            SwapSubActions(index, index + 1);

            OnValueChanged(this);
            return child as ITreeViewItem;
        }

        public void SwapSubActions(int firstIndex, int secondIndex)
        {
            (subActions[firstIndex], subActions[secondIndex]) =
                (subActions[secondIndex], subActions[firstIndex]);
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
