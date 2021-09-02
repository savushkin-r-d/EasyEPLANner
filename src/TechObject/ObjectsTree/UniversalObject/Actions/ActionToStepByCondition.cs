using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Специальное действие - переход к шагу по условию.
    /// </summary>
    public class ActionToStepByCondition : GroupableAction
    {
        /// <summary>
        /// Создание нового действия.
        /// </summary>
        /// <param name="name">Имя действия.</param>
        /// <param name="luaName">Имя действия - как оно будет называться в 
        /// таблице Lua.</param>
        /// <param name="owner">Владелец действия (Шаг)</param>
        public ActionToStepByCondition(string name, Step owner, 
            string luaName) : base(name, owner, luaName)
        {
            var allowedDevTypes = new Device.DeviceType[]
            {
                Device.DeviceType.V,
                Device.DeviceType.GS,
                Device.DeviceType.DI,
                Device.DeviceType.DO
            };

            SubActions.Add(new Action("Включение устройств", owner,
                "on_devices", allowedDevTypes));
            SubActions.Add(new Action("Выключение устройств", owner,
                "off_devices", allowedDevTypes));
        }

        override public IAction Clone()
        {
            var clone = new ActionToStepByCondition(name, owner, luaName);

            clone.SubActions = new List<IAction>();
            foreach (var action in SubActions)
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
        public override string SaveAsLuaTable(string prefix)
        {
            string res = string.Empty;
            if (SubActions.Count == 0)
            {
                return res;
            }

            string groupData = string.Empty;
            foreach (IAction group in SubActions)
            {
                groupData += group.SaveAsLuaTable(prefix + "\t");
            }

            if (groupData != string.Empty)
            {
                res += prefix;
                if (luaName != string.Empty)
                {
                    res += luaName + " =";
                }
                res += " --" + name + "\n" +
                    prefix +"\t{\n" +
                    groupData + 
                    prefix + "\t},\n";
            }

            return res;
        }

        public override void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            var action = SubActions.Where(x => x.LuaName == subActionLuaName)
                .FirstOrDefault();
            if (action != null)
            {
                action.AddDev(index, 0, string.Empty);
            }
        }

        #region Реализация ITreeViewItem
        public override bool Delete(object child)
        {
            if (child is IAction action)
            {
                action.Clear();
                return true;
            }

            return false;
        }
        #endregion
    }
}
