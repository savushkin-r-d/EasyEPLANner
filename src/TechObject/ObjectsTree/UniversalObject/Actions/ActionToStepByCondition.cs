using System.Collections.Generic;
using System.Linq;
using Editor;

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
            items = new List<ITreeViewItem>();

            var allowedDevTypes = new EplanDevice.DeviceType[]
            {
                EplanDevice.DeviceType.V,
                EplanDevice.DeviceType.GS,
                EplanDevice.DeviceType.DI,
                EplanDevice.DeviceType.DO
            };

            SubActions.Add(new ActionGroup("Включение устройств", owner,
                "on_devices_groups", allowedDevTypes, null));
            SubActions.Add(new ActionGroup("Выключение устройств", owner,
                "off_devices_groups", allowedDevTypes, null));            

            nextStepN = new ObjectProperty("Шаг", -1, -1);
            items.Add(nextStepN);
        }

        override public IAction Clone()
        {
            var clone = new ActionToStepByCondition(name, owner, luaName);

            clone.SubActions = new List<IAction>();
            foreach (var action in SubActions)
            {
                clone.SubActions.Add(action.Clone());
            }
            clone.NextStepN.SetNewValue(NextStepN.Value);
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
                if(int.Parse(NextStepN.Value.Trim()) <= 0)
                {
                    NextStepN.SetNewValue("-1");
                }

                groupData += $"{prefix}\tparameters = --Параметры условия\n" +
                    $"{prefix}\t\t{{\n" +
                    $"{prefix}\t\tnext_step_n = {NextStepN.Value.Trim()},\n" +
                    $"{prefix}\t\t}},\n";

                res += prefix;
                if (luaName != string.Empty)
                {
                    res += luaName + " =";
                }
                res += " --" + name + "\n" +
                    prefix + "\t{\n" +
                    groupData +
                    prefix + "\t},\n";
            }

            return res;
        }

        public override string SaveAsExcel()
        {
            string res = "";

            foreach (var subAction in SubActions)
            {
                var subActionText = subAction.SaveAsExcel();
                if (subActionText != string.Empty)
                {
                    res += $"{subAction.Name}:\n{subActionText}";
                }
            }

            if (res != string.Empty)
            {
                res += $"{NextStepN.Name}: {NextStepN.DisplayText[1]}";
            }

            return res;
        }

        public override void AddParam(object val, string paramName, int groupNumber)
        {
            switch (paramName)
            {
                case "next_step_n":
                    NextStepN.SetNewValue(val.ToString());
                    break;
            }
        }

        public override void AddDev(int index, int groupNumber,
            string subActionLuaName)
        {
            var action = SubActions.Where(x => x.LuaName == subActionLuaName)
                .FirstOrDefault();
            if (action != null)
            {
                action.AddDev(index, groupNumber, string.Empty);
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

        public override ITreeViewItem[] Items
        {
            get
            {
                return base.Items.Concat(items).ToArray();
            }
        }
        #endregion

        public override string ToString()
        {
            string res = base.ToString();

            if (int.Parse(nextStepN.Value) > 0)
                res += $" -> {NextStepN.Value.Trim()}";
            return res;
        }

        public ObjectProperty NextStepN
        {
            get => nextStepN;
            set => nextStepN = value;
        }

        private ObjectProperty nextStepN;

        private List<ITreeViewItem> items;
    }
}
