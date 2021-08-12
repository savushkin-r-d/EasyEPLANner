using System.Collections.Generic;
using System.Linq;
using Editor;
using TechObject.ActionProcessingStrategy;

namespace TechObject
{
    /// <summary>
    /// Действие с возможностью группировки объектов
    /// </summary>
    public class ActionGroup : Action
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        /// <param name="devSubTypes">Допустимые подтипы устройств</param>
        /// <param name="devTypes">Допустимые типы устройств</param>
        /// <param name="actionProcessorStrategy">Стратегия обработки
        /// устройств в группе действий</param>
        public ActionGroup(string name, Step owner, string luaName,
            Device.DeviceType[] devTypes = null,
            Device.DeviceSubType[] devSubTypes = null,
            IActionProcessorStrategy actionProcessorStrategy = null)
            : base(name, owner, luaName)
        {
            subActions = new List<IAction>();
            AddNewAction(owner, devTypes, devSubTypes,
                actionProcessorStrategy);
        }

        public override Action Clone()
        {
            var clone = (ActionGroup)base.Clone();
            clone.subActions = new List<IAction>();
            foreach (IAction action in subActions)
            {
                clone.subActions.Add(action.Clone());
            }

            return clone;
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

        public override void AddDev(int index, int groupNumber,
            int washGroupIndex = 0)
        {
            while (subActions.Count <= groupNumber)
            {
                InsertNewAction();
            }

            subActions[groupNumber].AddDev(index, 0);
            deviceIndex.Add(index);
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

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            if (subActions.Count == 0)
            {
                return string.Empty;
            }

            string res = string.Empty;

            foreach (IAction group in subActions)
            {
                res += group.SaveAsLuaTable(prefix + "\t");
            }

            if (res != string.Empty)
            {
                res = prefix + luaName + " = --" + name + "\n" +
                    prefix + "\t{\n" +
                    res +
                    prefix + "\t},\n";
            }

            return res;
        }

        #region Реализация ITreeViewItem
        override public string[] DisplayText
        {
            get
            {
                var deviceManager = Device.DeviceManager
                    .GetInstance();
                string res = string.Empty;

                foreach (IAction group in subActions)
                {
                    res += "{";

                    foreach (int index in group.DeviceIndex)
                    {
                        res += deviceManager.GetDeviceByIndex(index).Name + 
                            " ";
                    }

                    if (group.DeviceIndex.Count > 0)
                    {
                        res = res.Remove(res.Length - 1);
                    }

                    res += "} ";
                }

                res = res.Remove(res.Length - 1);

                return new string[] { name, res };
            }
        }

        override public ITreeViewItem[] Items
        {
            get
            {
                return subActions.Cast<ITreeViewItem>().ToArray();
            }
        }

        override public bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        override public bool Delete(object child)
        {
            var subAction = child as IAction;
            if (subAction != null)
            {
                int minCount = 1;
                if(subActions.Count > minCount)
                {
                    subActions.Remove(subAction);
                    return true;
                }
            }

            return false;
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
            ITreeViewItem newAction = InsertNewAction();
            newAction.AddParent(this);
            return newAction;
        }

        override public void Clear()
        {
            foreach (IAction subAction in subActions)
            {
                subAction.Clear();
            }
        }

        override public bool IsUseDevList
        {
            get
            {
                return false;
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
                if (subActions != null)
                {
                    foreach(var subAction in subActions)
                    {
                        subAction.DrawStyle = DrawStyle;
                    }
                }
            }
        }

        public override ImageIndexEnum ImageIndex
        {
            get
            {
                switch(luaName)
                {
                    case OpenedUpperSeats:
                        return ImageIndexEnum.ActionWashUpperSeats;

                    case OpenedLowerSeats:
                        return ImageIndexEnum.ActionWashLowerSeats;

                    case DIDO:
                    case AIAO:
                        return ImageIndexEnum.ActionDIDOPairs;

                    default:
                        return ImageIndexEnum.NONE;
                }
            }
        }

        public override void GetDisplayObjects(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            subActions.First().GetDisplayObjects(out devTypes, out devSubTypes,
                out displayParameters);
        }
        #endregion

        public override bool HasSubActions
        {
            get => true;
        }

        public override List<IAction> SubActions => subActions;

        private IAction AddNewAction(Step owner, Device.DeviceType[] devTypes,
            Device.DeviceSubType[] devSubTypes,
            IActionProcessorStrategy strategy)
        {
            var newAction = new Action(GroupDefaultName, owner,
                string.Empty, devTypes, devSubTypes, strategy);
            newAction.DrawStyle = DrawStyle;
            subActions.Add(newAction);

            return newAction;
        }

        private ITreeViewItem InsertNewAction()
        {
            IAction firstSubAction = subActions.First();
            firstSubAction.GetDisplayObjects(out Device.DeviceType[] devTypes,
                out Device.DeviceSubType[] devSubTypes, out _);
            IActionProcessorStrategy strategy = firstSubAction
                .GetActionProcessingStrategy();

            IAction newAction = AddNewAction(owner, devTypes, devSubTypes,
                strategy);

            return (ITreeViewItem)newAction;
        }

        private List<IAction> subActions;

        public const string AIAO = "AI_AO";
        public const string DIDO = "DI_DO";
        public const string OpenedLowerSeats = "opened_lower_seat_v";
        public const string OpenedUpperSeats = "opened_upper_seat_v";
    }
}
