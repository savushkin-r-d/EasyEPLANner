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

            SubActions.Add(new Action("Включение устройств", owner,
                "on_devices", allowedDevTypes));
            SubActions.Add(new Action("Выключение устройств", owner,
                "off_devices", allowedDevTypes));

            operators = new ComboBoxParameter(
                "operators",
                "Операторы проверки устройств",
                new Dictionary<string, string>
                {
                    { "И-И-И",       "AND-AND-AND" },
                    { "И-И-ИЛИ",     "AND-AND-OR" },
                    { "И-ИЛИ-И",     "ANND-OR-AND" },
                    { "И-ИЛИ-ИЛИ",   "AND-OR-OR" },
                    { "ИЛИ-И-И",     "OR-AND-AND" },
                    { "ИЛИ-И-ИЛИ",   "OR-AND-OR" },
                    { "ИЛИ-ИЛИ-И",   "OR-OR-AND" },
                    { "ИЛИ-ИЛИ-ИЛИ", "OR-OR-OR"},
                },
                "И-И-И");
            items.Add(operators);

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
            clone.Operators.SetNewValue(Operators.Value, true);
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
                    $"{prefix}\t\t{operators.LuaName} = '{operators.LuaValue.Trim()}',\n" +
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

        public override void AddParam(object val, string paramName, int groupNumber)
        {
            switch (paramName)
            {
                case "operators":
                    operators.SetNewValue(val.ToString());
                    break;
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
            else if (SubActions[0].IsFilled || SubActions[1].IsFilled)
                res += " Шаг не указан";
            return res;
        }

        public ComboBoxParameter Operators
        {
            get => operators;
            set => operators = value;
        }

        public ObjectProperty NextStepN
        {
            get => nextStepN;
            set => nextStepN = value;
        }

        private ComboBoxParameter operators;
        private ObjectProperty nextStepN;

        private List<ITreeViewItem> items;
    }
}
