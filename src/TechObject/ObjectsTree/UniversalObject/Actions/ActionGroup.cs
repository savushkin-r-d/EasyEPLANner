using System.Collections.Generic;
using System.Linq;
using Editor;
using TechObject.ActionProcessingStrategy;

namespace TechObject
{
    /// <summary>
    /// Действие с возможностью группировки объектов
    /// </summary>
    public class ActionGroup : GroupableAction
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
            EplanDevice.DeviceType[] devTypes,
            EplanDevice.DeviceSubType[] devSubTypes,
            IDeviceProcessingStrategy actionProcessorStrategy)
            : base(name, owner, luaName)
        {
            AddNewAction(owner, devTypes, devSubTypes,
                actionProcessorStrategy);
        }

        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться 
        /// в таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        /// <param name="devSubTypes">Допустимые подтипы устройств</param>
        /// <param name="devTypes">Допустимые типы устройств</param>
        public ActionGroup(string name, Step owner, string luaName,
            EplanDevice.DeviceType[] devTypes, EplanDevice.DeviceSubType[] devSubTypes)
            : this (name, owner, luaName, devTypes, devSubTypes, null) { }

        public override void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            while (SubActions.Count <= groupNumber)
            {
                InsertNewAction();
            }

            SubActions[groupNumber].AddDev(index, 0, string.Empty);
        }

        public override IAction Clone()
        {
            var clone = (GroupableAction)base.Clone();
            clone.SubActions = new List<IAction>();
            foreach (IAction action in SubActions)
            {
                clone.SubActions.Add(action.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        override public string SaveAsLuaTable(string prefix)
        {
            if (SubActions.Count == 0)
            {
                return string.Empty;
            }

            string res = string.Empty;

            foreach (IAction group in SubActions)
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
        override public bool Delete(object child)
        {
            var subAction = child as IAction;
            if (subAction != null)
            {
                int minCount = 1;
                if(SubActions.Count > minCount)
                {
                    SubActions.Remove(subAction);
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

        public override void GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
            out EplanDevice.DeviceSubType[] devSubTypes, out bool displayParameters)
        {
            SubActions.First().GetDisplayObjects(out devTypes, out devSubTypes,
                out displayParameters);
        }
        #endregion

        private IAction AddNewAction(Step owner, EplanDevice.DeviceType[] devTypes,
            EplanDevice.DeviceSubType[] devSubTypes,
            IDeviceProcessingStrategy strategy)
        {
            var newAction = new Action(GroupDefaultName, owner,
                string.Empty, devTypes, devSubTypes, strategy);
            newAction.DrawStyle = DrawStyle;
            SubActions.Add(newAction);

            return newAction;
        }

        private ITreeViewItem InsertNewAction()
        {
            IAction firstSubAction = SubActions.First();
            firstSubAction.GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
                out EplanDevice.DeviceSubType[] devSubTypes, out _);
            IDeviceProcessingStrategy strategy = firstSubAction
                .GetDeviceProcessingStrategy();

            IAction newAction = AddNewAction(owner, devTypes, devSubTypes,
                strategy);

            return (ITreeViewItem)newAction;
        }
    }
}
